using System.ComponentModel.DataAnnotations;

namespace AutoGuia.Core.DTOs
{
    /// <summary>
    /// DTO para mostrar reseñas en la interfaz
    /// </summary>
    public class ResenaDto
    {
        public int Id { get; set; }
        
        [Range(1, 5)]
        public int Calificacion { get; set; }
        
        public string Comentario { get; set; } = string.Empty;
        
        public DateTime FechaPublicacion { get; set; }
        
        public int TallerId { get; set; }
        
        public string UsuarioId { get; set; } = string.Empty;
        
        public string NombreUsuario { get; set; } = string.Empty;
        
        /// <summary>
        /// Fecha formateada para mostrar en la UI
        /// </summary>
        public string FechaFormateada => FechaPublicacion.ToString("dd/MM/yyyy");
        
        /// <summary>
        /// Tiempo transcurrido desde la publicación
        /// </summary>
        public string TiempoTranscurrido
        {
            get
            {
                var diferencia = DateTime.UtcNow - FechaPublicacion;
                
                if (diferencia.TotalDays >= 1)
                    return $"hace {(int)diferencia.TotalDays} día{((int)diferencia.TotalDays > 1 ? "s" : "")}";
                
                if (diferencia.TotalHours >= 1)
                    return $"hace {(int)diferencia.TotalHours} hora{((int)diferencia.TotalHours > 1 ? "s" : "")}";
                
                if (diferencia.TotalMinutes >= 1)
                    return $"hace {(int)diferencia.TotalMinutes} minuto{((int)diferencia.TotalMinutes > 1 ? "s" : "")}";
                
                return "hace un momento";
            }
        }
    }

    /// <summary>
    /// DTO para crear nuevas reseñas
    /// </summary>
    public class CrearResenaDto
    {
        [Required(ErrorMessage = "La calificación es requerida")]
        [Range(1, 5, ErrorMessage = "La calificación debe estar entre 1 y 5")]
        public int Calificacion { get; set; }

        [MaxLength(1000, ErrorMessage = "El comentario no puede exceder los 1000 caracteres")]
        public string Comentario { get; set; } = string.Empty;

        [Required]
        public int TallerId { get; set; }
    }

    /// <summary>
    /// DTO para estadísticas de reseñas
    /// </summary>
    public class EstadisticasResenaDto
    {
        public decimal CalificacionPromedio { get; set; }
        public int TotalResenas { get; set; }
        public Dictionary<int, int> DistribucionCalificaciones { get; set; } = new();
        
        /// <summary>
        /// Promedio formateado con 1 decimal
        /// </summary>
        public string PromedioFormateado => CalificacionPromedio.ToString("F1");
    }
}