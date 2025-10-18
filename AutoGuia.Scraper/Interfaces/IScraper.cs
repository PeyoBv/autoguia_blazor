using AutoGuia.Scraper.DTOs;

namespace AutoGuia.Scraper.Interfaces;

/// <summary>
/// Interfaz base para todos los scrapers de tiendas.
/// Implementa el patrón Strategy para permitir múltiples estrategias de scraping.
/// </summary>
public interface IScraper
{
    /// <summary>
    /// Nombre identificador de la tienda que maneja este scraper.
    /// Debe coincidir con el nombre de la tienda en la base de datos.
    /// </summary>
    string TiendaNombre { get; }

    /// <summary>
    /// Tipo de scraper (API, HTML, Playwright).
    /// </summary>
    string TipoScraper { get; }

    /// <summary>
    /// Indica si este scraper está habilitado para ejecución.
    /// </summary>
    bool EstaHabilitado { get; }

    /// <summary>
    /// Realiza el scraping de productos usando el número de parte.
    /// </summary>
    /// <param name="numeroDeParte">Número de parte del producto a buscar.</param>
    /// <param name="tiendaId">ID de la tienda en la base de datos.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns>Lista de ofertas encontradas para el producto.</returns>
    Task<IEnumerable<OfertaDto>> ScrapearProductosAsync(
        string numeroDeParte, 
        int tiendaId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Valida si el scraper puede procesar una tienda específica.
    /// </summary>
    /// <param name="tiendaNombre">Nombre de la tienda a validar.</param>
    /// <returns>True si puede procesar la tienda, False en caso contrario.</returns>
    bool PuedeScrapearTienda(string tiendaNombre);

    /// <summary>
    /// Obtiene la configuración específica del scraper (timeouts, headers, etc.).
    /// </summary>
    /// <returns>Diccionario con la configuración del scraper.</returns>
    Task<Dictionary<string, string>> ObtenerConfiguracionAsync();
}
