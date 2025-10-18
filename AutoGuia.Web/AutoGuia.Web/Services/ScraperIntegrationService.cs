using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using AutoGuia.Core.DTOs;
using AutoGuia.Core.Entities;
using AutoGuia.Scraper.Interfaces;
using AutoGuia.Infrastructure.Data;
using AutoGuia.Infrastructure.Services;

namespace AutoGuia.Web.Services;

public class ScraperIntegrationService : IScraperIntegrationService
{
    private readonly IEnumerable<IScraper> _scrapers;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ScraperIntegrationService> _logger;
    private readonly AutoGuiaDbContext _context;
    
    public ScraperIntegrationService(
        IEnumerable<IScraper> scrapers,
        IMemoryCache cache,
        ILogger<ScraperIntegrationService> logger,
        AutoGuiaDbContext context)
    {
        _scrapers = scrapers;
        _cache = cache;
        _logger = logger;
        _context = context;
    }

    public async Task<List<OfertaDto>> ObtenerOfertasEnTiempoRealAsync(
        string numeroDeParte, 
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"ofertas_{numeroDeParte.ToLower()}";
        
        if (_cache.TryGetValue<List<OfertaDto>>(cacheKey, out var cached))
        {
            _logger.LogInformation("Cache hit para {Parte}", numeroDeParte);
            return cached;
        }

        _logger.LogInformation("Ejecutando scrapers para {Parte}", numeroDeParte);

        var tiendas = await _context.Tiendas
            .Where(t => t.EsActivo)
            .ToDictionaryAsync(t => t.Nombre, t => t.Id);

        var scrapersActivos = _scrapers.Where(s => s.EstaHabilitado).ToList();
        var tareas = scrapersActivos.Select(async s =>
        {
            try
            {
                if (!tiendas.TryGetValue(s.TiendaNombre, out var tid))
                    return Enumerable.Empty<AutoGuia.Scraper.DTOs.OfertaDto>();

                return await s.ScrapearProductosAsync(numeroDeParte, tid, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en {Scraper}", s.TiendaNombre);
                return Enumerable.Empty<AutoGuia.Scraper.DTOs.OfertaDto>();
            }
        });

        var resultados = await Task.WhenAll(tareas);
        var ofertas = resultados.SelectMany(r => r)
            .Select(o => new OfertaDto
            {
                ProductoId = o.ProductoId,
                TiendaId = o.TiendaId,
                TiendaNombre = o.TiendaNombre,
                Precio = o.Precio,
                UrlProductoEnTienda = o.UrlProducto,
                EsDisponible = o.StockDisponible,
                FechaActualizacion = DateTime.UtcNow
            })
            .ToList();

        _cache.Set(cacheKey, ofertas, TimeSpan.FromHours(24));
        return ofertas;
    }

    public Task<bool> LimpiarCacheAsync(string numeroDeParte)
    {
        _cache.Remove($"ofertas_{numeroDeParte.ToLower()}");
        return Task.FromResult(true);
    }

    /// <summary>
    /// Ejecuta el scraping para un producto y actualiza/crea ofertas en la base de datos.
    /// Actúa como traductor entre Scraper.DTOs.OfertaDto y Core.Entities.Oferta.
    /// </summary>
    /// <param name="productoId">ID del producto a scrapear</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Número de ofertas procesadas</returns>
    public async Task<int> EjecutarYActualizarPrecios(int productoId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("🔄 Iniciando scraping y actualización de precios para ProductoId: {ProductoId}", productoId);

        try
        {
            // 1. Obtener el producto de la BD para validar que existe
            var producto = await _context.Productos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == productoId, cancellationToken);

            if (producto == null)
            {
                _logger.LogWarning("⚠️ Producto con ID {ProductoId} no encontrado", productoId);
                return 0;
            }

            // 2. Obtener tiendas activas
            var tiendasDict = await _context.Tiendas
                .Where(t => t.EsActivo)
                .ToDictionaryAsync(t => t.Nombre, t => t.Id, cancellationToken);

            if (!tiendasDict.Any())
            {
                _logger.LogWarning("⚠️ No hay tiendas activas disponibles");
                return 0;
            }

            // 3. Ejecutar scrapers en paralelo
            var scrapersActivos = _scrapers.Where(s => s.EstaHabilitado).ToList();
            _logger.LogInformation("🔧 Ejecutando {Count} scrapers activos", scrapersActivos.Count);

            var tareas = scrapersActivos.Select(async scraper =>
            {
                try
                {
                    // Validar que la tienda existe en BD
                    if (!tiendasDict.TryGetValue(scraper.TiendaNombre, out var tiendaId))
                    {
                        _logger.LogWarning("⚠️ Tienda '{TiendaNombre}' no encontrada en BD. Omitiendo scraper.", 
                            scraper.TiendaNombre);
                        return Enumerable.Empty<AutoGuia.Scraper.DTOs.OfertaDto>();
                    }

                    _logger.LogDebug("🔍 Ejecutando scraper: {TiendaNombre}", scraper.TiendaNombre);

                    // Ejecutar el scraper
                    var ofertas = await scraper.ScrapearProductosAsync(
                        producto.NumeroDeParte, 
                        tiendaId, 
                        cancellationToken);

                    _logger.LogInformation("✅ {TiendaNombre}: {Count} ofertas encontradas",
                        scraper.TiendaNombre, 
                        ofertas?.Count() ?? 0);

                    return ofertas ?? Enumerable.Empty<AutoGuia.Scraper.DTOs.OfertaDto>();
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("⏸️ Scraper '{TiendaNombre}' cancelado", scraper.TiendaNombre);
                    return Enumerable.Empty<AutoGuia.Scraper.DTOs.OfertaDto>();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error en scraper '{TiendaNombre}': {Message}",
                        scraper.TiendaNombre, 
                        ex.Message);
                    return Enumerable.Empty<AutoGuia.Scraper.DTOs.OfertaDto>();
                }
            });

            var resultadosScrapers = await Task.WhenAll(tareas);
            var ofertasScraperDto = resultadosScrapers.SelectMany(r => r).ToList();

            if (!ofertasScraperDto.Any())
            {
                _logger.LogWarning("⚠️ No se obtuvieron ofertas de ningún scraper para ProductoId: {ProductoId}", 
                    productoId);
                return 0;
            }

            _logger.LogInformation("📦 Total de ofertas scrapeadas: {Count}", ofertasScraperDto.Count);

            // 4. TRADUCCIÓN: Convertir Scraper.DTOs.OfertaDto → Core.Entities.Oferta y actualizar BD
            int ofertasActualizadas = 0;
            int ofertasCreadas = 0;

            foreach (var ofertaDto in ofertasScraperDto)
            {
                try
                {
                    // Buscar si la oferta ya existe (por ProductoId y TiendaId)
                    var ofertaExistente = await _context.Ofertas
                        .FirstOrDefaultAsync(
                            o => o.ProductoId == ofertaDto.ProductoId && 
                                 o.TiendaId == ofertaDto.TiendaId,
                            cancellationToken);

                    if (ofertaExistente != null)
                    {
                        // ACTUALIZAR oferta existente
                        var precioAnterior = ofertaExistente.Precio;

                        ofertaExistente.Precio = ofertaDto.Precio;
                        ofertaExistente.UrlProductoEnTienda = ofertaDto.UrlProducto;
                        ofertaExistente.EsDisponible = ofertaDto.StockDisponible;
                        ofertaExistente.FechaActualizacion = DateTime.UtcNow;

                        // Detectar si es oferta (precio bajó)
                        if (ofertaDto.Precio < precioAnterior)
                        {
                            ofertaExistente.EsOferta = true;
                            ofertaExistente.PrecioAnterior = precioAnterior;
                            _logger.LogInformation("🎉 ¡OFERTA! TiendaId: {TiendaId} - Precio bajó de {Anterior} a {Nuevo}",
                                ofertaDto.TiendaId, precioAnterior, ofertaDto.Precio);
                        }
                        else
                        {
                            ofertaExistente.EsOferta = false;
                        }

                        _context.Ofertas.Update(ofertaExistente);
                        ofertasActualizadas++;

                        _logger.LogDebug("🔄 Oferta actualizada: ProductoId={ProductoId}, TiendaId={TiendaId}, Precio={Precio}",
                            ofertaDto.ProductoId, ofertaDto.TiendaId, ofertaDto.Precio);
                    }
                    else
                    {
                        // CREAR nueva oferta
                        var nuevaOferta = new Oferta
                        {
                            ProductoId = ofertaDto.ProductoId,
                            TiendaId = ofertaDto.TiendaId,
                            Precio = ofertaDto.Precio,
                            UrlProductoEnTienda = ofertaDto.UrlProducto,
                            EsDisponible = ofertaDto.StockDisponible,
                            EsOferta = false,
                            FechaCreacion = DateTime.UtcNow,
                            FechaActualizacion = DateTime.UtcNow,
                            EsActivo = true
                        };

                        await _context.Ofertas.AddAsync(nuevaOferta, cancellationToken);
                        ofertasCreadas++;

                        _logger.LogDebug("➕ Nueva oferta creada: ProductoId={ProductoId}, TiendaId={TiendaId}, Precio={Precio}",
                            ofertaDto.ProductoId, ofertaDto.TiendaId, ofertaDto.Precio);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error al procesar oferta: ProductoId={ProductoId}, TiendaId={TiendaId}",
                        ofertaDto.ProductoId, ofertaDto.TiendaId);
                    // Continuar con la siguiente oferta
                }
            }

            // 5. Guardar TODOS los cambios de una sola vez
            var cambiosGuardados = await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "✅ Scraping completado para ProductoId {ProductoId}: " +
                "{Created} ofertas creadas, {Updated} actualizadas, {Total} cambios guardados",
                productoId, ofertasCreadas, ofertasActualizadas, cambiosGuardados);

            // 6. Limpiar caché para forzar recarga
            _cache.Remove($"ofertas_{producto.NumeroDeParte.ToLower()}");

            return ofertasCreadas + ofertasActualizadas;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error general en EjecutarYActualizarPrecios para ProductoId: {ProductoId}", 
                productoId);
            throw; // Re-lanzar para que el llamador pueda manejar el error
        }
    }
}
