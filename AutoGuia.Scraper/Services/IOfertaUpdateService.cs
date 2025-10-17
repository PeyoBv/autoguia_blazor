using AutoGuia.Core.Entities;
using AutoGuia.Scraper.Models;

namespace AutoGuia.Scraper.Services;

/// <summary>
/// Interfaz que define el contrato para actualizar ofertas en la base de datos.
/// Maneja la lógica de upsert (insertar o actualizar) basada en los resultados del scraping.
/// </summary>
public interface IOfertaUpdateService
{
    /// <summary>
    /// Actualiza o crea una oferta en la base de datos basada en el resultado del scraping.
    /// </summary>
    /// <param name="producto">Producto para el cual se actualizará la oferta</param>
    /// <param name="tienda">Tienda que ofrece el producto</param>
    /// <param name="resultado">Resultado del scraping con precio y URL del producto</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si se actualizó correctamente, False en caso contrario</returns>
    Task<bool> ActualizarOferta(Producto producto, Tienda tienda, ScrapeResult resultado, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marca ofertas como no disponibles cuando el scraping falla.
    /// </summary>
    /// <param name="productoId">ID del producto</param>
    /// <param name="tiendaId">ID de la tienda</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si se marcó correctamente, False en caso contrario</returns>
    Task<bool> MarcarOfertaComoNoDisponible(int productoId, int tiendaId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene estadísticas de las ofertas actualizadas.
    /// </summary>
    /// <param name="tiendaId">ID de la tienda (opcional)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Estadísticas de ofertas</returns>
    Task<OfertaStats> ObtenerEstadisticas(int? tiendaId = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Estadísticas de ofertas para reporting.
/// </summary>
public record OfertaStats(
    int TotalOfertas,
    int OfertasActivas,
    int OfertasActualizadasHoy,
    int OfertasNoDisponibles,
    DateTime UltimaActualizacion
);