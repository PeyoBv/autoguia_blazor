using System.ComponentModel.DataAnnotations;

namespace AutoGuia.Core.DTOs;

/// <summary>
/// DTO de solicitud para registrar feedback del usuario
/// Indica si la respuesta del diagnóstico fue útil
/// </summary>
public class FeedbackRequestDto
{
    /// <summary>
    /// Indica si la respuesta del asistente fue útil para el usuario
    /// </summary>
    [Required]
    public bool FueUtil { get; set; }
}
