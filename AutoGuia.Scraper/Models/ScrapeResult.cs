namespace AutoGuia.Scraper.Models;

/// <summary>
/// Resultado de una operación de scraping de un producto específico.
/// Encapsula toda la información obtenida del scraping de una tienda.
/// </summary>
public record ScrapeResult
{
    /// <summary>
    /// Indica si el scraping fue exitoso.
    /// </summary>
    public bool Exitoso { get; init; }

    /// <summary>
    /// Precio encontrado del producto (solo válido si Exitoso = true).
    /// </summary>
    public decimal Precio { get; init; }

    /// <summary>
    /// URL directa al producto en la tienda.
    /// </summary>
    public string UrlProducto { get; init; } = string.Empty;

    /// <summary>
    /// Mensaje de error descriptivo (solo válido si Exitoso = false).
    /// </summary>
    public string MensajeError { get; init; } = string.Empty;

    /// <summary>
    /// Información adicional del scraping (opcional).
    /// </summary>
    public string? InformacionAdicional { get; init; }

    /// <summary>
    /// Indica si el producto está disponible/en stock.
    /// </summary>
    public bool EnStock { get; init; } = true;

    /// <summary>
    /// Precio original antes de descuentos (opcional).
    /// </summary>
    public decimal? PrecioOriginal { get; init; }

    /// <summary>
    /// Timestamp del scraping.
    /// </summary>
    public DateTime FechaHoraScraping { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Crea un resultado exitoso.
    /// </summary>
    public static ScrapeResult CrearExitoso(decimal precio, string urlProducto, bool enStock = true, decimal? precioOriginal = null)
    {
        return new ScrapeResult
        {
            Exitoso = true,
            Precio = precio,
            UrlProducto = urlProducto,
            EnStock = enStock,
            PrecioOriginal = precioOriginal
        };
    }

    /// <summary>
    /// Crea un resultado fallido con mensaje de error.
    /// </summary>
    public static ScrapeResult CrearFallido(string mensajeError, string? informacionAdicional = null)
    {
        return new ScrapeResult
        {
            Exitoso = false,
            MensajeError = mensajeError,
            InformacionAdicional = informacionAdicional
        };
    }
}