using AutoGuia.Core.Entities;
using AutoGuia.Infrastructure.Data;
using AutoGuia.Scraper.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoGuia.Scraper.Services;

/// <summary>
/// Servicio que maneja la actualización de ofertas en la base de datos.
/// Implementa lógica de upsert (insertar o actualizar) basada en los resultados del scraping.
/// </summary>
public class OfertaUpdateService : IOfertaUpdateService
{
    private readonly AutoGuiaDbContext _context;
    private readonly ILogger<OfertaUpdateService> _logger;

    public OfertaUpdateService(
        AutoGuiaDbContext context,
        ILogger<OfertaUpdateService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<bool> ActualizarOferta(Producto producto, Tienda tienda, ScrapeResult resultado, CancellationToken cancellationToken = default)
    {
        if (producto == null) throw new ArgumentNullException(nameof(producto));
        if (tienda == null) throw new ArgumentNullException(nameof(tienda));
        if (resultado == null) throw new ArgumentNullException(nameof(resultado));

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        _logger.LogInformation("💾 Actualizando oferta: Producto={ProductoId}, Tienda={TiendaId}, Precio={Precio}",
            producto.Id, tienda.Id, resultado.Precio);

        try
        {
            // Si el scraping falló, marcar como no disponible
            if (!resultado.Exitoso)
            {
                _logger.LogWarning("❌ Scraping falló para Producto={ProductoId}, Tienda={TiendaId}: {Error}",
                    producto.Id, tienda.Id, resultado.MensajeError);
                
                return await MarcarOfertaComoNoDisponible(producto.Id, tienda.Id, cancellationToken);
            }

            // Buscar oferta existente
            var ofertaExistente = await _context.Ofertas
                .FirstOrDefaultAsync(o => o.ProductoId == producto.Id && o.TiendaId == tienda.Id,
                    cancellationToken);

            var ahora = DateTime.UtcNow;
            bool esNuevaOferta = ofertaExistente == null;
            bool preciosCambiaron = false;

            if (esNuevaOferta)
            {
                // Crear nueva oferta
                var nuevaOferta = new Oferta
                {
                    ProductoId = producto.Id,
                    TiendaId = tienda.Id,
                    Precio = resultado.Precio,
                    UrlProductoEnTienda = resultado.UrlProducto ?? string.Empty,
                    EsDisponible = true,
                    EsOferta = false,
                    FechaCreacion = ahora,
                    FechaActualizacion = ahora,
                    EsActivo = true
                };

                _context.Ofertas.Add(nuevaOferta);
                _logger.LogInformation("🆕 Nueva oferta creada: Producto={ProductoNombre}, Tienda={TiendaNombre}, Precio=${Precio}",
                    producto.Nombre, tienda.Nombre, resultado.Precio);
            }
            else
            {
                // Verificar si el precio cambió
                var precioNuevo = resultado.Precio;
                preciosCambiaron = Math.Abs(ofertaExistente.Precio - precioNuevo) > 0.01m;

                if (preciosCambiaron)
                {
                    // Guardar precio anterior si es una reducción
                    var esReduccion = precioNuevo < ofertaExistente.Precio;
                    if (esReduccion)
                    {
                        ofertaExistente.PrecioAnterior = ofertaExistente.Precio;
                        ofertaExistente.EsOferta = true;
                    }

                    ofertaExistente.Precio = precioNuevo;
                    _logger.LogInformation("💰 Precio actualizado: Producto={ProductoNombre}, Tienda={TiendaNombre}, " +
                        "PrecioAnterior=${PrecioAnterior}, PrecioNuevo=${PrecioNuevo}, EsOferta={EsOferta}",
                        producto.Nombre, tienda.Nombre, ofertaExistente.PrecioAnterior, precioNuevo, esReduccion);
                }

                // Actualizar URL si cambió
                if (!string.IsNullOrEmpty(resultado.UrlProducto) && 
                    ofertaExistente.UrlProductoEnTienda != resultado.UrlProducto)
                {
                    ofertaExistente.UrlProductoEnTienda = resultado.UrlProducto;
                    _logger.LogDebug("🔗 URL del producto actualizada: {NuevaUrl}", resultado.UrlProducto);
                }

                // Marcar como disponible y actualizar fecha
                ofertaExistente.EsDisponible = true;
                ofertaExistente.FechaActualizacion = ahora;
            }

            // Guardar cambios en la base de datos
            var filasAfectadas = await _context.SaveChangesAsync(cancellationToken);
            
            stopwatch.Stop();
            _logger.LogInformation("✅ Oferta {Accion} exitosamente en {TiempoMs}ms - FilasAfectadas={FilasAfectadas}",
                esNuevaOferta ? "creada" : "actualizada", stopwatch.ElapsedMilliseconds, filasAfectadas);

            return filasAfectadas > 0;
        }
        catch (DbUpdateException ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "❌ Error de base de datos actualizando oferta: Producto={ProductoId}, Tienda={TiendaId}. " +
                "Tiempo transcurrido: {TiempoMs}ms",
                producto.Id, tienda.Id, stopwatch.ElapsedMilliseconds);
            
            return false;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "❌ Error inesperado actualizando oferta: Producto={ProductoId}, Tienda={TiendaId}. " +
                "Tiempo transcurrido: {TiempoMs}ms",
                producto.Id, tienda.Id, stopwatch.ElapsedMilliseconds);
            
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> MarcarOfertaComoNoDisponible(int productoId, int tiendaId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("🚫 Marcando oferta como no disponible: ProductoId={ProductoId}, TiendaId={TiendaId}",
            productoId, tiendaId);

        try
        {
            var filasActualizadas = await _context.Ofertas
                .Where(o => o.ProductoId == productoId && o.TiendaId == tiendaId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(o => o.EsDisponible, false)
                    .SetProperty(o => o.FechaActualizacion, DateTime.UtcNow));

            if (filasActualizadas > 0)
            {
                _logger.LogInformation("✅ Oferta marcada como no disponible: ProductoId={ProductoId}, TiendaId={TiendaId}",
                    productoId, tiendaId);
            }
            else
            {
                _logger.LogWarning("⚠️ No se encontró oferta para marcar como no disponible: ProductoId={ProductoId}, TiendaId={TiendaId}",
                    productoId, tiendaId);
            }

            return filasActualizadas > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error marcando oferta como no disponible: ProductoId={ProductoId}, TiendaId={TiendaId}",
                productoId, tiendaId);
            
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<OfertaStats> ObtenerEstadisticas(int? tiendaId = null, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("📊 Obteniendo estadísticas de ofertas. TiendaId={TiendaId}", tiendaId);

        try
        {
            var query = _context.Ofertas.AsQueryable();
            
            if (tiendaId.HasValue)
            {
                query = query.Where(o => o.TiendaId == tiendaId.Value);
            }

            var hoy = DateTime.UtcNow.Date;
            
            var statsData = await query
                .GroupBy(o => 1) // Agrupar todo en un solo grupo
                .Select(g => new {
                    TotalOfertas = g.Count(),
                    OfertasActivas = g.Count(o => o.EsActivo && o.EsDisponible),
                    OfertasActualizadasHoy = g.Count(o => o.FechaActualizacion.Date == hoy),
                    OfertasNoDisponibles = g.Count(o => !o.EsDisponible),
                    UltimaActualizacion = g.Max(o => o.FechaActualizacion)
                })
                .FirstOrDefaultAsync(cancellationToken);

            // Crear OfertaStats desde el resultado
            var stats = statsData != null 
                ? new OfertaStats(
                    statsData.TotalOfertas,
                    statsData.OfertasActivas,
                    statsData.OfertasActualizadasHoy,
                    statsData.OfertasNoDisponibles,
                    statsData.UltimaActualizacion)
                : new OfertaStats(0, 0, 0, 0, DateTime.MinValue);

            _logger.LogInformation("📊 Estadísticas obtenidas: Total={Total}, Activas={Activas}, " +
                "ActualizadasHoy={ActualizadasHoy}, NoDisponibles={NoDisponibles}",
                stats.TotalOfertas, stats.OfertasActivas, stats.OfertasActualizadasHoy, stats.OfertasNoDisponibles);

            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error obteniendo estadísticas de ofertas");
            
            // Retornar estadísticas vacías en caso de error
            return new OfertaStats(0, 0, 0, 0, DateTime.MinValue);
        }
    }
}