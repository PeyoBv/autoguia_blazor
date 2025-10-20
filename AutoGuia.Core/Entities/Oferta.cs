#nullable enable

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoGuia.Core.Entities;

/// <summary>
/// Representa una oferta de producto en una tienda específica a un precio determinado
/// </summary>
public class Oferta
{
    public int Id { get; set; }
    
    [Required]
    public int ProductoId { get; set; }
    
    [Required]
    public int TiendaId { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Precio { get; set; }
    
    [Required]
    [StringLength(1000)]
    public string UrlProductoEnTienda { get; set; } = string.Empty;
    
    [StringLength(200)]
    public string? SKU { get; set; } // Código interno de la tienda
    
    public bool EsDisponible { get; set; } = true;
    public bool EsOferta { get; set; } = false;
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal? PrecioAnterior { get; set; } // Para ofertas
    
    // Propiedades de navegación
    public virtual Producto? Producto { get; set; }
    public virtual Tienda? Tienda { get; set; }
    
    // Propiedades de auditoría
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
    public bool EsActivo { get; set; } = true;
}