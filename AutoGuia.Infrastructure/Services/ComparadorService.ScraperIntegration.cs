using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoGuia.Core.DTOs;
using AutoGuia.Infrastructure.Data;

namespace AutoGuia.Infrastructure.Services
{
    /// <summary>
    /// Clase parcial de ComparadorService - Métodos de integración con scrapers
    /// </summary>
    public partial class ComparadorService
    {
        /// <summary>
        /// Obtiene ofertas en tiempo real ejecutando scrapers.
        /// </summary>
        public async Task<IEnumerable<OfertaDto>> ObtenerOfertasEnTiempoRealAsync(
            string numeroDeParte, 
            CancellationToken cancellationToken = default)
        {
            if (_scraperService == null)
            {
                _logger.LogWarning("ScraperIntegrationService no está disponible. Devolviendo ofertas de BD.");
                
                // Fallback: buscar en BD
                var ofertasDb = await _context.Ofertas
                    .Include(o => o.Producto)
                    .Include(o => o.Tienda)
                    .Where(o => o.Producto.NumeroDeParte.Contains(numeroDeParte))
                    .Select(o => new OfertaDto
                    {
                        Id = o.Id,
                        ProductoId = o.ProductoId,
                        TiendaId = o.TiendaId,
                        TiendaNombre = o.Tienda.Nombre,
                        TiendaLogo = o.Tienda.LogoUrl,
                        Precio = o.Precio,
                        UrlProductoEnTienda = o.UrlProductoEnTienda,
                        EsDisponible = o.EsDisponible,
                        FechaActualizacion = o.FechaActualizacion
                    })
                    .ToListAsync(cancellationToken);

                return ofertasDb;
            }

            _logger.LogInformation(
                "🔄 Obteniendo ofertas en tiempo real con scrapers para '{NumeroDeParte}'",
                numeroDeParte);

            // Ejecutar scrapers en tiempo real
            var ofertas = await _scraperService.ObtenerOfertasEnTiempoRealAsync(
                numeroDeParte, 
                cancellationToken);

            return ofertas;
        }

        /// <summary>
        /// Limpia el caché de ofertas para forzar una nueva búsqueda.
        /// </summary>
        public async Task<bool> LimpiarCacheOfertasAsync(string numeroDeParte)
        {
            if (_scraperService == null)
            {
                _logger.LogWarning("ScraperIntegrationService no está disponible.");
                return false;
            }

            return await _scraperService.LimpiarCacheAsync(numeroDeParte);
        }
    }
}
