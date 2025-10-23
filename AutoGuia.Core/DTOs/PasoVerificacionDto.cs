using System.ComponentModel.DataAnnotations;

namespace AutoGuia.Core.DTOs;

/// <summary>
/// DTO para transferencia de pasos de verificación ordenados
/// Utilizado para construir guías paso a paso en la UI
/// </summary>
public class PasoVerificacionDto
{
    /// <summary>
    /// Identificador único del paso de verificación
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Orden secuencial del paso (1, 2, 3, etc.)
    /// Crítico para mostrar los pasos en el orden correcto en la UI
    /// </summary>
    public int Orden { get; set; }

    /// <summary>
    /// Descripción breve del paso a realizar
    /// </summary>
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>
    /// Instrucciones detalladas para ejecutar el paso
    /// </summary>
    public string InstruccionesDetalladas { get; set; } = string.Empty;

    /// <summary>
    /// Indicadores que confirman la correcta ejecución del paso (opcional)
    /// </summary>
    public string? IndicadoresExito { get; set; }
}
