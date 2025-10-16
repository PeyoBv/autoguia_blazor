using System.ComponentModel.DataAnnotations;

namespace AutoGuia.Core.Entities
{
    public class PublicacionForo
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Titulo { get; set; } = string.Empty;
        
        [Required]
        [StringLength(5000)]
        public string Contenido { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? Categoria { get; set; }
        
        // Etiquetas separadas por coma
        [StringLength(500)]
        public string? Etiquetas { get; set; }
        
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaActualizacion { get; set; }
        
        public int Vistas { get; set; } = 0;
        public int Likes { get; set; } = 0;
        
        public bool EsDestacado { get; set; } = false;
        public bool EsCerrado { get; set; } = false;
        public bool EsActivo { get; set; } = true;
        
        // Relaciones
        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; } = null!;
        
        public virtual ICollection<RespuestaForo> Respuestas { get; set; } = new List<RespuestaForo>();
    }
}