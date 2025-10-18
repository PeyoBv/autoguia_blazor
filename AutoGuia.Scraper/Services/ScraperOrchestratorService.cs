using AutoGuia.Infrastructure.Data;
using AutoGuia.Scraper.Interfaces;
using AutoGuia.Scraper.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoGuia.Scraper.Services;

/// <summary>
/// Servicio orquestador que coordina la ejecución de múltiples scrapers.
/// Implementa el patrón Orchestrator para gestionar el flujo de scraping.
/// </summary>
public class ScraperOrchestratorService
{
    private readonly IEnumerable<IScraper> _scrapers;
    private readonly AutoGuiaDbContext _context;
    private readonly OfertaUpdateService _ofertaUpdateService;
    private readonly ILogger<ScraperOrchestratorService> _logger;

    public ScraperOrchestratorService(
        IEnumerable<IScraper> scrapers,
        AutoGuiaDbContext context,
        OfertaUpdateService ofertaUpdateService,
        ILogger<ScraperOrchestratorService> logger)
    {
        _scrapers = scrapers;
        _context = context;
        _ofertaUpdateService = ofertaUpdateService;
        _logger = logger;
    }

    /// <summary>
    /// Ejecuta el scraping para un producto específico en todas las tiendas configuradas.
    /// </summary>
    /// <param name="productoId">ID del producto a scrapear.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    public async Task EjecutarScrapingAsync(int productoId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("🚀 Iniciando scraping para producto ID: {ProductoId}", productoId);

            // 1. Obtener el producto de la base de datos
            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.Id == productoId, cancellationToken);

            if (producto == null)
            {
                _logger.LogWarning("⚠️ Producto con ID {ProductoId} no encontrado", productoId);
                return;
            }

            if (string.IsNullOrWhiteSpace(producto.NumeroDeParte))
            {
                _logger.LogWarning("⚠️ Producto {ProductoId} no tiene número de parte configurado", productoId);
                return;
            }

            _logger.LogInformation("📦 Producto encontrado: {Nombre} - Número de parte: {NumeroDeParte}", 
                producto.Nombre, producto.NumeroDeParte);

            // 2. Obtener todas las tiendas activas
            var tiendas = await _context.Tiendas
                .Where(t => t.EsActivo)
                .ToListAsync(cancellationToken);

            if (!tiendas.Any())
            {
                _logger.LogWarning("⚠️ No hay tiendas activas configuradas");
                return;
            }

            _logger.LogInformation("🏪 Se encontraron {Count} tiendas activas", tiendas.Count);

            // 3. Lista para acumular todas las ofertas
            var todasLasOfertas = new List<OfertaDto>();

            // 4. Ejecutar scraping en cada tienda
            foreach (var tienda in tiendas)
            {
                try
                {
                    _logger.LogInformation("🔍 Scrapeando en tienda: {TiendaNombre}", tienda.Nombre);

                    // Buscar el scraper apropiado para esta tienda
                    var scraper = _scrapers.FirstOrDefault(s => 
                        s.EstaHabilitado && s.PuedeScrapearTienda(tienda.Nombre));

                    if (scraper == null)
                    {
                        _logger.LogWarning("⚠️ No se encontró scraper para la tienda: {TiendaNombre}", tienda.Nombre);
                        continue;
                    }

                    _logger.LogInformation("✅ Usando scraper: {TipoScraper} para {TiendaNombre}", 
                        scraper.TipoScraper, scraper.TiendaNombre);

                    // Ejecutar el scraping
                    var ofertas = await scraper.ScrapearProductosAsync(
                        producto.NumeroDeParte, 
                        tienda.Id, 
                        cancellationToken);

                    if (ofertas?.Any() == true)
                    {
                        todasLasOfertas.AddRange(ofertas);
                        _logger.LogInformation("✅ Se encontraron {Count} ofertas en {TiendaNombre}", 
                            ofertas.Count(), tienda.Nombre);
                    }
                    else
                    {
                        _logger.LogInformation("ℹ️ No se encontraron ofertas en {TiendaNombre}", tienda.Nombre);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error al scrapear la tienda: {TiendaNombre}", tienda.Nombre);
                    
                    // Agregar oferta con error para registro
                    todasLasOfertas.Add(new OfertaDto
                    {
                        ProductoId = productoId,
                        TiendaId = tienda.Id,
                        TiendaNombre = tienda.Nombre,
                        TieneErrores = true,
                        MensajeError = ex.Message,
                        FechaScrapeo = DateTime.UtcNow
                    });
                }
            }

            // 5. Actualizar la base de datos con todas las ofertas
            if (todasLasOfertas.Any())
            {
                _logger.LogInformation("💾 Actualizando base de datos con {Count} ofertas totales", todasLasOfertas.Count);
                // Procesar cada oferta individualmente
                foreach (var oferta in todasLasOfertas.Where(o => !o.TieneErrores))
                {
                    try
                    {
                        var productoDb = await _context.Productos.FindAsync(oferta.ProductoId);
                        var tiendaDb = await _context.Tiendas.FindAsync(oferta.TiendaId);
                        
                        if (productoDb != null && tiendaDb != null)
                        {
                            var resultado = new AutoGuia.Scraper.Models.ScrapeResult
                            {
                                Exitoso = true,
                                Precio = oferta.Precio,
                                UrlProducto = oferta.UrlProducto,
                                EnStock = oferta.StockDisponible
                            };
                            
                            await _ofertaUpdateService.ActualizarOferta(productoDb, tiendaDb, resultado, cancellationToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error al actualizar oferta para producto {ProductoId}", oferta.ProductoId);
                    }
                }
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("✅ Base de datos actualizada exitosamente");
            }

            _logger.LogInformation("🎉 Scraping completado para producto ID: {ProductoId}", productoId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error crítico al ejecutar scraping para producto ID: {ProductoId}", productoId);
            throw;
        }
    }

    /// <summary>
    /// Ejecuta el scraping para múltiples productos en paralelo.
    /// </summary>
    /// <param name="productosIds">Lista de IDs de productos a scrapear.</param>
    /// <param name="maxParallelism">Número máximo de scrapers en paralelo.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    public async Task EjecutarScrapingMasivoAsync(
        IEnumerable<int> productosIds, 
        int maxParallelism = 3,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("🚀 Iniciando scraping masivo para {Count} productos", productosIds.Count());

        var semaphore = new SemaphoreSlim(maxParallelism);
        var tasks = new List<Task>();

        foreach (var productoId in productosIds)
        {
            await semaphore.WaitAsync(cancellationToken);

            var task = Task.Run(async () =>
            {
                try
                {
                    await EjecutarScrapingAsync(productoId, cancellationToken);
                }
                finally
                {
                    semaphore.Release();
                }
            }, cancellationToken);

            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
        _logger.LogInformation("🎉 Scraping masivo completado");
    }

    /// <summary>
    /// Obtiene estadísticas de los scrapers disponibles.
    /// </summary>
    public async Task<Dictionary<string, object>> ObtenerEstadisticasScrapersAsync()
    {
        var stats = new Dictionary<string, object>
        {
            ["TotalScrapers"] = _scrapers.Count(),
            ["ScrapersHabilitados"] = _scrapers.Count(s => s.EstaHabilitado),
            ["ScrapersDeshabilitados"] = _scrapers.Count(s => !s.EstaHabilitado),
            ["Scrapers"] = _scrapers.Select(s => new
            {
                s.TiendaNombre,
                s.TipoScraper,
                s.EstaHabilitado
            }).ToList()
        };

        var tiendasActivas = await _context.Tiendas.CountAsync(t => t.EsActivo);
        stats["TiendasActivas"] = tiendasActivas;

        return stats;
    }
}
