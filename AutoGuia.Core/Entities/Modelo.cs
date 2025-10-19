using System.ComponentModel.DataAnnotations;

namespace AutoGuia.Core.Entities
{
    /// <summary>
    /// Representa un modelo específico de una marca de vehículo
    /// </summary>
    public class Modelo
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;
        
        [Required]
        public int MarcaId { get; set; }
        
        [StringLength(500)]
        public string? ImagenUrl { get; set; }
        
        [StringLength(1000)]
        public string? Descripcion { get; set; }
        
        public int? AnioInicioProduccion { get; set; }
        public int? AnioFinProduccion { get; set; }
        
        // Propiedades de navegación
        public virtual Marca Marca { get; set; } = null!;
        
        // Propiedades de auditoría
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public bool EsActivo { get; set; } = true;
    }
}