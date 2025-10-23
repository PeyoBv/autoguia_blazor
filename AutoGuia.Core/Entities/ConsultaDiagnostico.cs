using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoGuia.Core.Entities;

/// <summary>
/// Representa una consulta de diagnóstico realizada por un usuario
/// Almacena el historial de interacciones con el asistente automotriz
/// (ej: "Mi auto pierde fuerza al acelerar", "Sale humo blanco del escape")
/// </summary>
[Table("consultas_diagnostico", Schema = "diagnostico")]
public class ConsultaDiagnostico
{
    /// <summary>
    /// Identificador único de la consulta de diagnóstico
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Identificador del usuario que realizó la consulta
    /// </summary>
    [ForeignKey("Usuario")]
    [Column("usuario_id")]
    public int UsuarioId { get; set; }

    /// <summary>
    /// Descripción del síntoma en palabras del usuario
    /// </summary>
    [Required]
    [StringLength(1000)]
    [Column("sintoma_descrito")]
    public string SintomaDescrito { get; set; } = string.Empty;

    /// <summary>
    /// Fecha y hora en que se realizó la consulta
    /// </summary>
    [Column("fecha_consulta")]
    public DateTime FechaConsulta { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Respuesta generada por el asistente de diagnóstico
    /// </summary>
    [StringLength(2000)]
    [Column("respuesta_asistente")]
    public string RespuestaAsistente { get; set; } = string.Empty;

    /// <summary>
    /// Indica si el usuario marcó la respuesta como útil (feedback)
    /// </summary>
    [Column("fue_util")]
    public bool FueUtil { get; set; } = false;

    /// <summary>
    /// Identificador del síntoma relacionado identificado por el sistema (nullable)
    /// Permite vincular consultas de texto libre con síntomas catalogados
    /// </summary>
    [ForeignKey("SintomaRelacionado")]
    [Column("sintoma_relacionado_id")]
    public int? SintomaRelacionadoId { get; set; }

    /// <summary>
    /// Usuario que realizó la consulta
    /// </summary>
    public virtual Usuario Usuario { get; set; } = null!;

    /// <summary>
    /// Síntoma catalogado relacionado con la consulta (nullable)
    /// Permite analizar patrones y mejorar el sistema
    /// </summary>
    public virtual Sintoma? SintomaRelacionado { get; set; }
}
