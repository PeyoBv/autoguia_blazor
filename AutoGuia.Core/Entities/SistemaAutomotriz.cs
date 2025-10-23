using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoGuia.Core.Entities;

/// <summary>
/// Representa un sistema automotriz del vehículo (ej: Motor, Transmisión, Frenos, etc.)
/// Utilizado para clasificar y organizar síntomas de diagnóstico.
/// </summary>
[Table("sistemas_automotrices", Schema = "diagnostico")]
public class SistemaAutomotriz
{
    /// <summary>
    /// Identificador único del sistema automotriz
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Nombre del sistema automotriz (ej: "Motor", "Transmisión", "Sistema de Frenos")
    /// </summary>
    [Required]
    [StringLength(100)]
    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Descripción detallada del sistema y sus componentes principales
    /// </summary>
    [StringLength(500)]
    [Column("descripcion")]
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>
    /// Indica si el sistema está activo en el catálogo
    /// </summary>
    [Column("es_activo")]
    public bool EsActivo { get; set; } = true;

    /// <summary>
    /// Fecha de creación del registro
    /// </summary>
    [Column("fecha_creacion")]
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Colección de síntomas asociados a este sistema automotriz
    /// </summary>
    public virtual ICollection<Sintoma> Sintomas { get; set; } = new List<Sintoma>();
}
