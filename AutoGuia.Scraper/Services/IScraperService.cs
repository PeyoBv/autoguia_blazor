using AutoGuia.Core.Entities;
using AutoGuia.Scraper.Models;

namespace AutoGuia.Scraper.Services;

/// <summary>
/// Interfaz que define el contrato para servicios de scraping específicos de tiendas.
/// Permite implementar scrapers para diferentes tiendas de manera uniforme.
/// </summary>
public interface IScraperService
{
    /// <summary>
    /// Nombre identificador de la tienda que maneja este scraper.
    /// </summary>
    string TiendaNombre { get; }

    /// <summary>
    /// Scrapea un producto específico de la tienda.
    /// </summary>
    /// <param name="producto">Producto a buscar y scrapear</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Resultado del scraping con precio y datos encontrados</returns>
    Task<ScrapeResult> ScrapearProducto(Producto producto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si la tienda está disponible para scraping.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si la tienda está disponible</returns>
    Task<bool> VerificarDisponibilidad(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene información sobre la configuración del scraper.
    /// </summary>
    /// <returns>Información de configuración</returns>
    ScraperInfo ObtenerInformacion();
}

/// <summary>
/// Información sobre un scraper específico.
/// </summary>
public record ScraperInfo
{
    /// <summary>
    /// Nombre de la tienda.
    /// </summary>
    public string NombreTienda { get; init; } = string.Empty;

    /// <summary>
    /// URL base de la tienda.
    /// </summary>
    public string UrlBase { get; init; } = string.Empty;

    /// <summary>
    /// Indica si el scraper está habilitado.
    /// </summary>
    public bool Habilitado { get; init; }

    /// <summary>
    /// Delay recomendado entre requests (en milisegundos).
    /// </summary>
    public int DelayEntreRequests { get; init; } = 1000;

    /// <summary>
    /// Versión del scraper.
    /// </summary>
    public string Version { get; init; } = "1.0.0";
}