using System.ComponentModel.DataAnnotations;

namespace AutoGuia.Core.Entities
{
    public class Taller
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Nombre { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string? Descripcion { get; set; }
        
        [Required]
        [StringLength(300)]
        public string Direccion { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Ciudad { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Region { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string? CodigoPostal { get; set; }
        
        [StringLength(20)]
        public string? Telefono { get; set; }
        
        [EmailAddress]
        [StringLength(200)]
        public string? Email { get; set; }
        
        [Url]
        [StringLength(300)]
        public string? SitioWeb { get; set; }
        
        // Coordenadas para el mapa
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }
        
        // Información de negocio
        [StringLength(100)]
        public string? HorarioAtencion { get; set; }
        
        public decimal? CalificacionPromedio { get; set; }
        
        public int TotalResenas { get; set; } = 0;
        
        // Especialidades
        [StringLength(500)]
        public string? Especialidades { get; set; }
        
        // Servicios ofrecidos (JSON string para flexibilidad)
        [StringLength(2000)]
        public string? ServiciosOfrecidos { get; set; }
        
        public bool EsVerificado { get; set; } = false;
        public bool EsActivo { get; set; } = true;
        
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
        public DateTime? FechaVerificacion { get; set; }
        
        // Relaciones
        public virtual ICollection<Resena> Resenas { get; set; } = new List<Resena>();
    }
}