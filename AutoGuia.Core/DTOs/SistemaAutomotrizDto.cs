using System.ComponentModel.DataAnnotations;

namespace AutoGuia.Core.DTOs;

/// <summary>
/// DTO para transferencia de datos de sistemas automotrices
/// Incluye lista de síntomas para navegación jerárquica en la UI
/// </summary>
public class SistemaAutomotrizDto
{
    /// <summary>
    /// Identificador único del sistema automotriz
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre del sistema automotriz (ej: "Motor", "Transmisión", "Frenos")
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Descripción del sistema y sus componentes principales
    /// </summary>
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>
    /// Lista de síntomas asociados a este sistema automotriz
    /// Permite navegación jerárquica Sistema → Síntomas en la interfaz
    /// </summary>
    public List<SintomaDto> Sintomas { get; set; } = new();
}
