using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoGuia.Core.Entities
{
    /// <summary>
    /// Une un Producto con una Tienda a un precio específico
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
        public virtual Producto Producto { get; set; } = null!;
        public virtual Tienda Tienda { get; set; } = null!;
        
        // Propiedades de auditoría
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
        public bool EsActivo { get; set; } = true;
    }
}