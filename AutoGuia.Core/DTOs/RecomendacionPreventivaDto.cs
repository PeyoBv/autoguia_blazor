using System.ComponentModel.DataAnnotations;

namespace AutoGuia.Core.DTOs;

/// <summary>
/// DTO para transferencia de recomendaciones de mantenimiento preventivo
/// Incluye frecuencias sugeridas por kilometraje y tiempo
/// </summary>
public class RecomendacionPreventivaDto
{
    /// <summary>
    /// Identificador único de la recomendación preventiva
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Descripción breve de la acción preventiva recomendada
    /// </summary>
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>
    /// Detalle ampliado de la recomendación y su importancia
    /// </summary>
    public string Detalle { get; set; } = string.Empty;

    /// <summary>
    /// Frecuencia recomendada en kilómetros (0 si no aplica)
    /// </summary>
    public int FrecuenciaKilometros { get; set; }

    /// <summary>
    /// Frecuencia recomendada en meses (0 si no aplica)
    /// </summary>
    public int FrecuenciaMeses { get; set; }
}
