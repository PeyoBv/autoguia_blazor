using System.ComponentModel.DataAnnotations;

namespace AutoGuia.Core.DTOs;

/// <summary>
/// DTO para transferencia de datos de causas posibles con información completa
/// Incluye pasos de verificación y recomendaciones preventivas anidadas
/// </summary>
public class CausaPosibleDto
{
    /// <summary>
    /// Identificador único de la causa posible
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Descripción breve de la causa posible
    /// </summary>
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>
    /// Nivel de probabilidad (1=Baja, 2=Media, 3=Alta, 4=Muy Alta)
    /// </summary>
    public int NivelProbabilidad { get; set; }

    /// <summary>
    /// Descripción técnica detallada de la causa
    /// </summary>
    public string DescripcionDetallada { get; set; } = string.Empty;

    /// <summary>
    /// Indica si requiere atención de un mecánico profesional
    /// </summary>
    public bool RequiereServicioProfesional { get; set; }

    /// <summary>
    /// Lista de pasos ordenados para verificar esta causa
    /// </summary>
    public List<PasoVerificacionDto> PasosVerificacion { get; set; } = new();

    /// <summary>
    /// Lista de recomendaciones preventivas para evitar esta causa
    /// </summary>
    public List<RecomendacionPreventivaDto> Recomendaciones { get; set; } = new();
}
