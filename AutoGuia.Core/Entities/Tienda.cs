using System.ComponentModel.DataAnnotations;

namespace AutoGuia.Core.Entities
{
    /// <summary>
    /// Representa la tienda que vende el producto
    /// </summary>
    public class Tienda
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Nombre { get; set; } = string.Empty;
        
        [Required]
        [StringLength(500)]
        public string UrlSitioWeb { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? LogoUrl { get; set; }
        
        [StringLength(1000)]
        public string? Descripcion { get; set; }
        
        // Propiedades de navegación
        public virtual ICollection<Oferta> Ofertas { get; set; } = new List<Oferta>();
        
        // Propiedades de auditoría
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public bool EsActivo { get; set; } = true;
    }
}