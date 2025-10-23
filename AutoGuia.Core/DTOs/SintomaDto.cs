using System.ComponentModel.DataAnnotations;

namespace AutoGuia.Core.DTOs;

/// <summary>
/// DTO para transferencia de datos de síntomas automotrices
/// Utilizado en respuestas API y comunicación entre capas
/// </summary>
public class SintomaDto
{
    /// <summary>
    /// Identificador único del síntoma
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Descripción del síntoma en lenguaje comprensible
    /// </summary>
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>
    /// Descripción técnica del síntoma para especialistas
    /// </summary>
    public string DescripcionTecnica { get; set; } = string.Empty;

    /// <summary>
    /// Nivel de urgencia (1=Bajo, 2=Medio, 3=Alto, 4=Crítico)
    /// </summary>
    public int NivelUrgencia { get; set; }

    /// <summary>
    /// Identificador del sistema automotriz asociado
    /// </summary>
    public int SistemaAutomotrizId { get; set; }

    /// <summary>
    /// Nombre del sistema automotriz (para evitar joins adicionales)
    /// </summary>
    public string? NombreSistema { get; set; }
}
