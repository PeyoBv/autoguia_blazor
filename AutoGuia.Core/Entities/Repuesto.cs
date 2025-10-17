using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoGuia.Core.Entities
{
    /// <summary>
    /// Entidad que representa un repuesto automotriz
    /// </summary>
    public class Repuesto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Descripcion { get; set; }

        [StringLength(100)]
        public string? NumeroDeParte { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? PrecioEstimado { get; set; }

        [StringLength(100)]
        public string? Marca { get; set; }

        [StringLength(200)]
        public string? Modelo { get; set; }

        [StringLength(50)]
        public string? Anio { get; set; }

        public string? ImagenUrl { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public bool EsActivo { get; set; } = true;
        public bool EsDisponible { get; set; } = true;

        // Clave foránea a CategoriaRepuesto
        public int CategoriaRepuestoId { get; set; }

        // Navegación: Un repuesto pertenece a una categoría
        public virtual CategoriaRepuesto CategoriaRepuesto { get; set; } = null!;
    }
}