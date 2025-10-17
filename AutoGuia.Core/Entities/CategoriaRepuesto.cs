using System.ComponentModel.DataAnnotations;

namespace AutoGuia.Core.Entities
{
    /// <summary>
    /// Entidad que representa una categoría de repuestos
    /// </summary>
    public class CategoriaRepuesto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descripcion { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public bool EsActivo { get; set; } = true;

        // Navegación: Una categoría puede tener muchos repuestos
        public virtual ICollection<Repuesto> Repuestos { get; set; } = new List<Repuesto>();
    }
}