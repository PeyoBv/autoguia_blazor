using System.ComponentModel.DataAnnotations;

namespace AutoGuia.Core.Entities
{
    /// <summary>
    /// Representa el producto único, independiente de la tienda
    /// </summary>
    public class Producto
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Nombre { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string NumeroDeParte { get; set; } = string.Empty; // Clave para comparar
        
        [StringLength(1000)]
        public string? Descripcion { get; set; }
        
        [StringLength(500)]
        public string? ImagenUrl { get; set; }
        
        // Propiedades de navegación
        public virtual ICollection<Oferta> Ofertas { get; set; } = new List<Oferta>();
        public virtual ICollection<ProductoVehiculoCompatible> VehiculosCompatibles { get; set; } = new List<ProductoVehiculoCompatible>();
        
        // Propiedades de auditoría
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public bool EsActivo { get; set; } = true;
    }
}