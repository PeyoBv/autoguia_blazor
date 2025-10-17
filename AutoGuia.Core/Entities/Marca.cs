using System.ComponentModel.DataAnnotations;

namespace AutoGuia.Core.Entities
{
    /// <summary>
    /// Representa una marca de vehículo
    /// </summary>
    public class Marca
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? LogoUrl { get; set; }
        
        [StringLength(1000)]
        public string? Descripcion { get; set; }
        
        // Propiedades de navegación
        public virtual ICollection<Modelo> Modelos { get; set; } = new List<Modelo>();
        
        // Propiedades de auditoría
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public bool EsActivo { get; set; } = true;
    }
}