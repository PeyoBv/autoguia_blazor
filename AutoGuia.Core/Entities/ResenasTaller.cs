using System.ComponentModel.DataAnnotations;

namespace AutoGuia.Core.Entities
{
    public class ResenasTaller
    {
        public int Id { get; set; }
        
        [Range(1, 5)]
        public int Calificacion { get; set; }
        
        [StringLength(1000)]
        public string? Comentario { get; set; }
        
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        
        public bool EsActivo { get; set; } = true;
        
        // Relaciones
        public int TallerId { get; set; }
        public virtual Taller Taller { get; set; } = null!;
        
        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; } = null!;
    }
}