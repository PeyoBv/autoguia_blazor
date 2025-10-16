using System.ComponentModel.DataAnnotations;

namespace AutoGuia.Core.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Apellido { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string? Telefono { get; set; }
        
        [StringLength(500)]
        public string? Biografia { get; set; }
        
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
        
        public bool EsActivo { get; set; } = true;
        
        // Propiedades específicas del sector automotriz
        [StringLength(100)]
        public string? EspecialidadAutomotriz { get; set; }
        
        public int? AnosExperiencia { get; set; }
        
        // Relaciones
        public virtual ICollection<PublicacionForo> PublicacionesForo { get; set; } = new List<PublicacionForo>();
        public virtual ICollection<RespuestaForo> RespuestasForo { get; set; } = new List<RespuestaForo>();
        public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
    }
}