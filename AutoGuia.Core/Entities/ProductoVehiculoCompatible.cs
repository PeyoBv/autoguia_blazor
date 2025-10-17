using System.ComponentModel.DataAnnotations;

namespace AutoGuia.Core.Entities
{
    /// <summary>
    /// Tabla de enlace para la relación muchos-a-muchos entre Producto y Modelo de vehículo
    /// Representa la compatibilidad de un producto con un modelo específico de vehículo
    /// </summary>
    public class ProductoVehiculoCompatible
    {
        [Required]
        public int ProductoId { get; set; }
        
        [Required]
        public int ModeloId { get; set; }
        
        [Required]
        public int Ano { get; set; } // Año específico de compatibilidad
        
        [StringLength(500)]
        public string? NotasCompatibilidad { get; set; } // Ej: "Solo versión 2.0L", "Excepto turbo"
        
        // Propiedades de navegación
        public virtual Producto Producto { get; set; } = null!;
        public virtual Modelo Modelo { get; set; } = null!;
        
        // Propiedades de auditoría
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public bool EsActivo { get; set; } = true;
    }
}