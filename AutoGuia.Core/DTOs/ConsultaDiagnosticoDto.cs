using System.ComponentModel.DataAnnotations;

namespace AutoGuia.Core.DTOs;

/// <summary>
/// DTO para transferencia de datos del historial de consultas de diagnóstico
/// Utilizado para mostrar consultas previas del usuario con feedback y síntomas relacionados
/// </summary>
public class ConsultaDiagnosticoDto
{
    /// <summary>
    /// Identificador único de la consulta
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Descripción del síntoma en palabras del usuario
    /// </summary>
    public string SintomaDescrito { get; set; } = string.Empty;

    /// <summary>
    /// Fecha y hora en que se realizó la consulta
    /// </summary>
    public DateTime FechaConsulta { get; set; }

    /// <summary>
    /// Respuesta generada por el asistente de diagnóstico
    /// </summary>
    public string RespuestaAsistente { get; set; } = string.Empty;

    /// <summary>
    /// Indica si el usuario marcó la respuesta como útil (feedback)
    /// </summary>
    public bool FueUtil { get; set; }

    /// <summary>
    /// Identificador del síntoma relacionado identificado por el sistema (nullable)
    /// </summary>
    public int? SintomaRelacionadoId { get; set; }

    /// <summary>
    /// Nombre del síntoma relacionado (para evitar joins adicionales en UI)
    /// </summary>
    public string? NombreSintomaRelacionado { get; set; }
}
