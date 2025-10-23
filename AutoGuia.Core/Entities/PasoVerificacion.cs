using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoGuia.Core.Entities;

/// <summary>
/// Representa un paso específico en la verificación de una causa posible
/// (ej: "1. Abre el capó", "2. Localiza el filtro de aire", "3. Inspecciona el estado del filtro")
/// Los pasos están ordenados secuencialmente para guiar al usuario
/// </summary>
[Table("pasos_verificacion", Schema = "diagnostico")]
public class PasoVerificacion
{
    /// <summary>
    /// Identificador único del paso de verificación
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Identificador de la causa posible a la que pertenece este paso
    /// </summary>
    [ForeignKey("CausaPosible")]
    [Column("causa_posible_id")]
    public int CausaPosibleId { get; set; }

    /// <summary>
    /// Descripción breve del paso a realizar
    /// </summary>
    [Required]
    [StringLength(255)]
    [Column("descripcion")]
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>
    /// Orden secuencial del paso (1, 2, 3, etc.)
    /// </summary>
    [Column("orden")]
    public int Orden { get; set; }

    /// <summary>
    /// Instrucciones detalladas para ejecutar el paso correctamente
    /// </summary>
    [StringLength(1000)]
    [Column("instrucciones_detalladas")]
    public string InstruccionesDetalladas { get; set; } = string.Empty;

    /// <summary>
    /// Indicadores que confirman que el paso fue ejecutado correctamente
    /// (ej: "El filtro debe estar limpio y sin obstrucciones")
    /// </summary>
    [StringLength(500)]
    [Column("indicadores_exito")]
    public string? IndicadoresExito { get; set; }

    /// <summary>
    /// Fecha de creación del registro
    /// </summary>
    [Column("fecha_creacion")]
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Causa posible a la que pertenece este paso de verificación
    /// </summary>
    public virtual CausaPosible CausaPosible { get; set; } = null!;
}
