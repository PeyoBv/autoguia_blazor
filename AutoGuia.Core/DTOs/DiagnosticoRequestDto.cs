using System.ComponentModel.DataAnnotations;

namespace AutoGuia.Core.DTOs;

/// <summary>
/// DTO de solicitud para el endpoint de diagnóstico
/// Contiene la descripción del síntoma proporcionada por el usuario
/// </summary>
public class DiagnosticoRequestDto
{
    /// <summary>
    /// Descripción del síntoma en lenguaje natural del usuario
    /// </summary>
    [Required(ErrorMessage = "La descripción del síntoma es requerida")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "La descripción debe tener entre 10 y 1000 caracteres")]
    public string DescripcionSintoma { get; set; } = string.Empty;
}
