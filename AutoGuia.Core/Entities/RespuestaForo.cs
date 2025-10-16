using System.ComponentModel.DataAnnotations;

namespace AutoGuia.Core.Entities
{
    public class RespuestaForo
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(2000)]
        public string Contenido { get; set; } = string.Empty;
        
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaActualizacion { get; set; }
        
        public int Likes { get; set; } = 0;
        
        public bool EsRespuestaAceptada { get; set; } = false;
        public bool EsActivo { get; set; } = true;
        
        // Relaciones
        public int PublicacionId { get; set; }
        public virtual PublicacionForo Publicacion { get; set; } = null!;
        
        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; } = null!;
        
        // Respuesta padre para hilos anidados (opcional)
        public int? RespuestaPadreId { get; set; }
        public virtual RespuestaForo? RespuestaPadre { get; set; }
        public virtual ICollection<RespuestaForo> RespuestasHijas { get; set; } = new List<RespuestaForo>();
    }
}