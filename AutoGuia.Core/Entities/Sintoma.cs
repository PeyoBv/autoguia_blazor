using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoGuia.Core.Entities;

/// <summary>
/// Representa un síntoma específico de un problema automotriz 
/// (ej: "Pérdida de potencia", "Humo blanco del escape", "Ruido al frenar")
/// </summary>
[Table("sintomas", Schema = "diagnostico")]
public class Sintoma
{
    /// <summary>
    /// Identificador único del síntoma
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Identificador del sistema automotriz al que pertenece este síntoma
    /// </summary>
    [ForeignKey("SistemaAutomotriz")]
    [Column("sistema_automotriz_id")]
    public int SistemaAutomotrizId { get; set; }

    /// <summary>
    /// Descripción del síntoma en lenguaje comprensible para el usuario
    /// </summary>
    [Required]
    [StringLength(255)]
    [Column("descripcion")]
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>
    /// Descripción técnica del síntoma para mecánicos y especialistas
    /// </summary>
    [StringLength(500)]
    [Column("descripcion_tecnica")]
    public string DescripcionTecnica { get; set; } = string.Empty;

    /// <summary>
    /// Nivel de urgencia del síntoma (1=Bajo, 2=Medio, 3=Alto, 4=Crítico)
    /// </summary>
    [Column("nivel_urgencia")]
    public int NivelUrgencia { get; set; } = 1;

    /// <summary>
    /// Indica si el síntoma está activo en el catálogo
    /// </summary>
    [Column("es_activo")]
    public bool EsActivo { get; set; } = true;

    /// <summary>
    /// Fecha de creación del registro
    /// </summary>
    [Column("fecha_creacion")]
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Sistema automotriz al que pertenece este síntoma
    /// </summary>
    public virtual SistemaAutomotriz SistemaAutomotriz { get; set; } = null!;

    /// <summary>
    /// Colección de causas posibles asociadas a este síntoma
    /// </summary>
    public virtual ICollection<CausaPosible> CausasPosibles { get; set; } = new List<CausaPosible>();
}
