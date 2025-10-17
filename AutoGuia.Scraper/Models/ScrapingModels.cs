namespace AutoGuia.Scraper.Models;

/// <summary>
/// Representa un producto scrapeado de una tienda online.
/// </summary>
public class ScrapedProduct
{
    /// <summary>
    /// Identificador único del producto en la tienda externa.
    /// </summary>
    public string ExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del producto.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descripción detallada del producto.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Marca del producto.
    /// </summary>
    public string Brand { get; set; } = string.Empty;

    /// <summary>
    /// Modelo/referencia del producto.
    /// </summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Categoría del producto.
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// URL de la imagen del producto.
    /// </summary>
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// URL del producto en la tienda original.
    /// </summary>
    public string ProductUrl { get; set; } = string.Empty;

    /// <summary>
    /// Precio actual del producto.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Precio original (antes de descuentos).
    /// </summary>
    public decimal? OriginalPrice { get; set; }

    /// <summary>
    /// Indica si el producto está en stock.
    /// </summary>
    public bool InStock { get; set; } = true;

    /// <summary>
    /// Cantidad en stock (si está disponible).
    /// </summary>
    public int? StockQuantity { get; set; }

    /// <summary>
    /// Tienda de origen.
    /// </summary>
    public string StoreName { get; set; } = string.Empty;

    /// <summary>
    /// Fecha y hora del scraping.
    /// </summary>
    public DateTime ScrapedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Atributos adicionales del producto (ej: especificaciones técnicas).
    /// </summary>
    public Dictionary<string, string> Attributes { get; set; } = new();
}

/// <summary>
/// Representa una oferta/precio scrapeado de una tienda online.
/// </summary>
public class ScrapedOffer
{
    /// <summary>
    /// Identificador único de la oferta en la tienda externa.
    /// </summary>
    public string ExternalOfferId { get; set; } = string.Empty;

    /// <summary>
    /// Identificador del producto asociado.
    /// </summary>
    public string ProductExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Precio de la oferta.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Precio original (antes de descuentos).
    /// </summary>
    public decimal? OriginalPrice { get; set; }

    /// <summary>
    /// Porcentaje de descuento.
    /// </summary>
    public decimal? DiscountPercentage { get; set; }

    /// <summary>
    /// Indica si el producto está disponible.
    /// </summary>
    public bool IsAvailable { get; set; } = true;

    /// <summary>
    /// Cantidad en stock.
    /// </summary>
    public int? StockQuantity { get; set; }

    /// <summary>
    /// Información de envío.
    /// </summary>
    public string? ShippingInfo { get; set; }

    /// <summary>
    /// Costo de envío.
    /// </summary>
    public decimal? ShippingCost { get; set; }

    /// <summary>
    /// Tiempo estimado de entrega.
    /// </summary>
    public string? DeliveryTime { get; set; }

    /// <summary>
    /// Tienda de origen.
    /// </summary>
    public string StoreName { get; set; } = string.Empty;

    /// <summary>
    /// URL directa a la oferta.
    /// </summary>
    public string OfferUrl { get; set; } = string.Empty;

    /// <summary>
    /// Fecha y hora del scraping.
    /// </summary>
    public DateTime ScrapedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha de validez de la oferta.
    /// </summary>
    public DateTime? ValidUntil { get; set; }

    /// <summary>
    /// Información adicional de la oferta.
    /// </summary>
    public Dictionary<string, string> AdditionalInfo { get; set; } = new();
}

/// <summary>
/// Resultado de una operación de scraping.
/// </summary>
public class ScrapingResult
{
    /// <summary>
    /// Indica si la operación fue exitosa.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Mensaje descriptivo del resultado.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Número de productos procesados.
    /// </summary>
    public int ProductsProcessed { get; set; }

    /// <summary>
    /// Número de ofertas procesadas.
    /// </summary>
    public int OffersProcessed { get; set; }

    /// <summary>
    /// Errores encontrados durante el proceso.
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Tiempo total de ejecución.
    /// </summary>
    public TimeSpan ExecutionTime { get; set; }

    /// <summary>
    /// Tienda procesada.
    /// </summary>
    public string StoreName { get; set; } = string.Empty;
}