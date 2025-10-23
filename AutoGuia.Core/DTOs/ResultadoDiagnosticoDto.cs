using System.ComponentModel.DataAnnotations;

namespace AutoGuia.Core.DTOs;

/// <summary>
/// DTO principal de respuesta del sistema de diagnóstico
/// Contiene el resultado completo del análisis con causas posibles y recomendaciones
/// </summary>
public class ResultadoDiagnosticoDto
{
    /// <summary>
    /// Nivel de urgencia del síntoma identificado (1=Bajo, 2=Medio, 3=Alto, 4=Crítico)
    /// </summary>
    public int NivelUrgencia { get; set; }

    /// <summary>
    /// Descripción del síntoma identificado por el sistema
    /// </summary>
    public string SintomaIdentificado { get; set; } = string.Empty;

    /// <summary>
    /// Lista de causas posibles ordenadas por probabilidad
    /// Cada causa incluye pasos de verificación y recomendaciones preventivas
    /// </summary>
    public List<CausaPosibleDto> CausasPosibles { get; set; } = new();

    /// <summary>
    /// Recomendación general del sistema basada en el análisis
    /// </summary>
    public string Recomendacion { get; set; } = string.Empty;

    /// <summary>
    /// Indica si se recomienda acudir a un servicio profesional
    /// </summary>
    public bool SugerirServicioProfesional { get; set; }

    /// <summary>
    /// Identificador del síntoma relacionado en la base de datos (nullable)
    /// Permite vincular la consulta con el catálogo de síntomas
    /// </summary>
    public int? SintomaRelacionadoId { get; set; }
}
