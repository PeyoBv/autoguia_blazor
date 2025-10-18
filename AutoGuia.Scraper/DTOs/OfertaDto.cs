namespace AutoGuia.Scraper.DTOs;

/// <summary>
/// DTO que representa una oferta de producto obtenida del scraping.
/// </summary>
public class OfertaDto
{
    /// <summary>
    /// ID del producto en la base de datos AutoGuía.
    /// </summary>
    public int ProductoId { get; set; }

    /// <summary>
    /// ID de la tienda en la base de datos AutoGuía.
    /// </summary>
    public int TiendaId { get; set; }

    /// <summary>
    /// Nombre de la tienda (para logging y referencia).
    /// </summary>
    public string TiendaNombre { get; set; } = string.Empty;

    /// <summary>
    /// Precio del producto en la tienda.
    /// </summary>
    public decimal Precio { get; set; }

    /// <summary>
    /// URL directa al producto en la tienda.
    /// </summary>
    public string UrlProducto { get; set; } = string.Empty;

    /// <summary>
    /// Indica si el producto está disponible en stock.
    /// </summary>
    public bool StockDisponible { get; set; }

    /// <summary>
    /// Cantidad de unidades disponibles (si la tienda lo proporciona).
    /// </summary>
    public int? CantidadDisponible { get; set; }

    /// <summary>
    /// Descripción o título del producto en la tienda.
    /// </summary>
    public string? Descripcion { get; set; }

    /// <summary>
    /// URL de la imagen del producto (si está disponible).
    /// </summary>
    public string? ImagenUrl { get; set; }

    /// <summary>
    /// Tiempo de entrega estimado (si la tienda lo proporciona).
    /// </summary>
    public string? TiempoEntrega { get; set; }

    /// <summary>
    /// Calificación del vendedor o producto (si está disponible).
    /// </summary>
    public decimal? Calificacion { get; set; }

    /// <summary>
    /// Fecha y hora en que se obtuvo esta oferta.
    /// </summary>
    public DateTime FechaScrapeo { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Indica si hubo errores al obtener esta oferta.
    /// </summary>
    public bool TieneErrores { get; set; }

    /// <summary>
    /// Mensaje de error si TieneErrores es true.
    /// </summary>
    public string? MensajeError { get; set; }
}
