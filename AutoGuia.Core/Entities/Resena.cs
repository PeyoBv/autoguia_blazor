using System.ComponentModel.DataAnnotations;

namespace AutoGuia.Core.Entities
{
    /// <summary>
    /// Entidad que representa una reseña de un usuario sobre un taller
    /// </summary>
    public class Resena
    {
        /// <summary>
        /// Identificador único de la reseña
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Calificación numérica del taller (1-5 estrellas)
        /// </summary>
        [Range(1, 5, ErrorMessage = "La calificación debe estar entre 1 y 5")]
        public int Calificacion { get; set; }

        /// <summary>
        /// Comentario opcional del usuario sobre el taller
        /// </summary>
        [MaxLength(1000, ErrorMessage = "El comentario no puede exceder los 1000 caracteres")]
        public string Comentario { get; set; } = string.Empty;

        /// <summary>
        /// Fecha y hora cuando se publicó la reseña
        /// </summary>
        public DateTime FechaPublicacion { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Identificador del taller al que pertenece esta reseña
        /// </summary>
        [Required]
        public int TallerId { get; set; }

        /// <summary>
        /// Identificador del usuario que escribió la reseña
        /// </summary>
        [Required]
        public string UsuarioId { get; set; } = string.Empty;

        // Propiedades de navegación
        /// <summary>
        /// Taller al que pertenece esta reseña
        /// </summary>
        public virtual Taller? Taller { get; set; }

        /// <summary>
        /// Nombre del usuario (para mostrar en la interfaz)
        /// </summary>
        public string NombreUsuario { get; set; } = string.Empty;
    }
}