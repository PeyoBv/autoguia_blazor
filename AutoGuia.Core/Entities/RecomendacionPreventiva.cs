using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoGuia.Core.Entities;

/// <summary>
/// Representa una recomendación de mantenimiento preventivo para evitar una causa posible
/// (ej: "Cambiar filtro de aire cada 20,000 km o 12 meses", "Revisar nivel de aceite mensualmente")
/// </summary>
[Table("recomendaciones_preventivas", Schema = "diagnostico")]
public class RecomendacionPreventiva
{
    /// <summary>
    /// Identificador único de la recomendación preventiva
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Identificador de la causa posible que se previene con esta recomendación
    /// </summary>
    [ForeignKey("CausaPosible")]
    [Column("causa_posible_id")]
    public int CausaPosibleId { get; set; }

    /// <summary>
    /// Descripción breve de la acción preventiva recomendada
    /// </summary>
    [Required]
    [StringLength(255)]
    [Column("descripcion")]
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>
    /// Detalle ampliado de la recomendación y su importancia
    /// </summary>
    [StringLength(500)]
    [Column("detalle")]
    public string Detalle { get; set; } = string.Empty;

    /// <summary>
    /// Frecuencia recomendada en kilómetros (0 si no aplica frecuencia por kilometraje)
    /// </summary>
    [Column("frecuencia_kilometros")]
    public int FrecuenciaKilometros { get; set; }

    /// <summary>
    /// Frecuencia recomendada en meses (0 si no aplica frecuencia temporal)
    /// </summary>
    [Column("frecuencia_meses")]
    public int FrecuenciaMeses { get; set; }

    /// <summary>
    /// Fecha de creación del registro
    /// </summary>
    [Column("fecha_creacion")]
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Causa posible que se previene con esta recomendación
    /// </summary>
    public virtual CausaPosible CausaPosible { get; set; } = null!;
}
