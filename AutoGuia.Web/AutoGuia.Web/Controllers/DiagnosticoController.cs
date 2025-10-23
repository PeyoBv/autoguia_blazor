using Microsoft.AspNetCore.Mvc;
using AutoGuia.Core.DTOs;
using AutoGuia.Core.Interfaces;
using System.Security.Claims;

namespace AutoGuia.Web.Controllers;

/// <summary>
/// Controlador API para el sistema de diagnóstico automotriz
/// Proporciona endpoints REST para análisis de síntomas, consultas y feedback
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DiagnosticoController : ControllerBase
{
    private readonly IDiagnosticoService _diagnosticoService;

    public DiagnosticoController(IDiagnosticoService diagnosticoService)
    {
        _diagnosticoService = diagnosticoService;
    }

    /// <summary>
    /// Diagnostica un síntoma descrito por el usuario
    /// Requiere autenticación
    /// </summary>
    /// <param name="request">Solicitud con descripción del síntoma</param>
    /// <returns>Resultado completo del diagnóstico con causas, pasos y recomendaciones</returns>
    [HttpPost("diagnosticar")]
    public async Task<IActionResult> DiagnosticarSintoma([FromBody] DiagnosticoRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (usuarioId == 0)
            return Unauthorized(new { mensaje = "Usuario no autenticado" });

        var resultado = await _diagnosticoService.DiagnosticarSintomaAsync(request.DescripcionSintoma, usuarioId);
        return Ok(resultado);
    }

    /// <summary>
    /// Obtiene todos los síntomas de un sistema automotriz específico
    /// </summary>
    /// <param name="sistemaId">Identificador del sistema automotriz</param>
    /// <returns>Lista de síntomas del sistema</returns>
    [HttpGet("sintomas/{sistemaId}")]
    public async Task<IActionResult> ObtenerSintomasPorSistema(int sistemaId)
    {
        var sintomas = await _diagnosticoService.ObtenerSintomasPorSistemaAsync(sistemaId);
        return Ok(sintomas);
    }

    /// <summary>
    /// Obtiene los detalles completos de una causa posible
    /// </summary>
    /// <param name="causaId">Identificador de la causa posible</param>
    /// <returns>Causa con pasos de verificación y recomendaciones</returns>
    [HttpGet("causa/{causaId}")]
    public async Task<IActionResult> ObtenerCausaDetalles(int causaId)
    {
        var causa = await _diagnosticoService.ObtenerCausaConDetallesAsync(causaId);
        if (causa == null)
            return NotFound(new { mensaje = "Causa no encontrada" });

        return Ok(causa);
    }

    /// <summary>
    /// Obtiene el historial de consultas del usuario autenticado
    /// Requiere autenticación
    /// </summary>
    /// <returns>Lista de consultas previas con respuestas y feedback</returns>
    [HttpGet("historial")]
    public async Task<IActionResult> ObtenerHistorial()
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (usuarioId == 0)
            return Unauthorized(new { mensaje = "Usuario no autenticado" });

        var historial = await _diagnosticoService.ObtenerHistorialAsync(usuarioId);
        return Ok(historial);
    }

    /// <summary>
    /// Registra el feedback del usuario sobre un diagnóstico
    /// </summary>
    /// <param name="consultaId">Identificador de la consulta</param>
    /// <param name="request">Feedback del usuario (útil o no útil)</param>
    /// <returns>Confirmación del registro</returns>
    [HttpPost("feedback/{consultaId}")]
    public async Task<IActionResult> RegistrarFeedback(int consultaId, [FromBody] FeedbackRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _diagnosticoService.RegistrarFeedbackAsync(consultaId, request.FueUtil);
        return Ok(new { mensaje = "Feedback registrado exitosamente" });
    }
}
