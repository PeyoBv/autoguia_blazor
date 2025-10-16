using System.ComponentModel.DataAnnotations;

namespace AutoGuia.Core.Entities
{
    public class Vehiculo
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Marca { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Modelo { get; set; } = string.Empty;
        
        [Range(1900, 2030)]
        public int Ano { get; set; }
        
        [StringLength(50)]
        public string? Version { get; set; }
        
        [StringLength(20)]
        public string? Patente { get; set; }
        
        [StringLength(50)]
        public string? TipoMotor { get; set; }
        
        [StringLength(30)]
        public string? TipoCombustible { get; set; }
        
        [StringLength(30)]
        public string? Transmision { get; set; }
        
        public int? Kilometraje { get; set; }
        
        [StringLength(50)]
        public string? Color { get; set; }
        
        [StringLength(1000)]
        public string? Descripcion { get; set; }
        
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        
        public bool EsActivo { get; set; } = true;
        
        // Relación con usuario
        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; } = null!;
    }
}