using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoGuia.Core.Entities;

/// <summary>
/// Representa una causa posible de un síntoma automotriz específico
/// (ej: "Filtro de aire obstruido", "Bujías desgastadas", "Correa de distribución rota")
/// </summary>
[Table("causas_posibles", Schema = "diagnostico")]
public class CausaPosible
{
    /// <summary>
    /// Identificador único de la causa posible
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Identificador del síntoma al que está asociada esta causa
    /// </summary>
    [ForeignKey("Sintoma")]
    [Column("sintoma_id")]
    public int SintomaId { get; set; }

    /// <summary>
    /// Descripción breve de la causa posible
    /// </summary>
    [Required]
    [StringLength(255)]
    [Column("descripcion")]
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>
    /// Nivel de probabilidad de esta causa (1=Baja, 2=Media, 3=Alta, 4=Muy Alta)
    /// </summary>
    [Column("nivel_probabilidad")]
    public int NivelProbabilidad { get; set; } = 1;

    /// <summary>
    /// Descripción técnica detallada de la causa y sus efectos
    /// </summary>
    [StringLength(1000)]
    [Column("descripcion_detallada")]
    public string DescripcionDetallada { get; set; } = string.Empty;

    /// <summary>
    /// Indica si la verificación/reparación requiere un mecánico profesional
    /// </summary>
    [Column("requiere_servicio_profesional")]
    public bool RequiereServicioProfesional { get; set; } = false;

    /// <summary>
    /// Fecha de creación del registro
    /// </summary>
    [Column("fecha_creacion")]
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Síntoma al que pertenece esta causa posible
    /// </summary>
    public virtual Sintoma Sintoma { get; set; } = null!;

    /// <summary>
    /// Colección de pasos de verificación para confirmar esta causa
    /// </summary>
    public virtual ICollection<PasoVerificacion> PasosVerificacion { get; set; } = new List<PasoVerificacion>();

    /// <summary>
    /// Colección de recomendaciones preventivas para evitar esta causa
    /// </summary>
    public virtual ICollection<RecomendacionPreventiva> Recomendaciones { get; set; } = new List<RecomendacionPreventiva>();
}
