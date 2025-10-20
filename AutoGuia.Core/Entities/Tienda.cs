#nullable enable

using System.ComponentModel.DataAnnotations;

namespace AutoGuia.Core.Entities;

/// <summary>
/// Representa una tienda que vende productos automotrices
/// </summary>
public class Tienda
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Nombre { get; set; } = string.Empty;
    
    [StringLength(1000)]
    public string? Descripcion { get; set; }
    
    [StringLength(500)]
    public string UrlSitioWeb { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? LogoUrl { get; set; }
    
    [StringLength(200)]
    public string? Direccion { get; set; }
    
    [StringLength(50)]
    public string? Telefono { get; set; }
    
    [StringLength(200)]
    public string? Email { get; set; }
    
    public bool EsConfiable { get; set; } = true;
    public bool EsVerificada { get; set; } = false;
    
    // Propiedades de navegación
    public virtual ICollection<Oferta> Ofertas { get; set; } = new List<Oferta>();
    
    // Propiedades de auditoría
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
    public bool EsActivo { get; set; } = true;
}
