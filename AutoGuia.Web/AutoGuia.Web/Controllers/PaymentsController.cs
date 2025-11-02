using AutoGuia.Core.DTOs;
using AutoGuia.Infrastructure.Services.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AutoGuia.Web.Controllers;

/// <summary>
/// Controlador para gestionar pagos con Transbank
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly ITransbankGateway _transbankGateway;
    private readonly ISubscriptionBillingService _billingService;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(
        ITransbankGateway transbankGateway,
        ISubscriptionBillingService billingService,
        ILogger<PaymentsController> logger)
    {
        _transbankGateway = transbankGateway;
        _billingService = billingService;
        _logger = logger;
    }

    // ==================== INSCRIPCIÓN DE TARJETA ====================

    /// <summary>
    /// Inicia el proceso de inscripción de una tarjeta
    /// </summary>
    [HttpPost("inscripcion/iniciar")]
    public async Task<ActionResult<IniciarInscripcionResponseDto>> IniciarInscripcion(
        [FromBody] IniciarInscripcionRequestDto request)
    {
        try
        {
            // 🔍 Log para debugging
            _logger.LogInformation("⚡ Endpoint /api/payments/inscripcion/iniciar llamado");
            _logger.LogInformation("📦 Request recibido: Email={Email}, Username={Username}, PlanId={PlanId}, UsuarioId={UsuarioId}, ReturnUrl={ReturnUrl}", 
                request?.Email ?? "NULL", 
                request?.Username ?? "NULL", 
                request?.PlanId ?? 0, 
                request?.UsuarioId ?? "NULL",
                request?.ReturnUrl ?? "NULL");

            if (request == null)
            {
                _logger.LogError("❌ Request es NULL");
                return BadRequest(new { message = "Request body vacío" });
            }

            if (string.IsNullOrEmpty(request.UsuarioId))
            {
                _logger.LogError("❌ UsuarioId está vacío o null");
                return BadRequest(new { message = "UsuarioId es requerido" });
            }

            if (request.PlanId <= 0)
            {
                _logger.LogError("❌ PlanId inválido: {PlanId}", request.PlanId);
                return BadRequest(new { message = "PlanId inválido" });
            }

            _logger.LogInformation("✅ Iniciando inscripción de tarjeta para usuario {UsuarioId}, Plan {PlanId}", 
                request.UsuarioId, request.PlanId);

            var resultado = await _transbankGateway.IniciarInscripcionAsync(request, request.UsuarioId);

            if (!resultado.Success)
            {
                return BadRequest(new { message = resultado.ErrorMessage });
            }

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al iniciar inscripción de tarjeta");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Confirma la inscripción de una tarjeta después del redirect de Transbank
    /// </summary>
    [HttpPost("inscripcion/confirmar")]
    [HttpGet("inscripcion/confirmar")] // GET también para redirect de Transbank
    public async Task<ActionResult<ConfirmarInscripcionResponseDto>> ConfirmarInscripcion(
        [FromQuery] string? token,
        [FromBody] ConfirmarInscripcionRequestDto? request)
    {
        try
        {
            var tokenToUse = token ?? request?.Token;

            if (string.IsNullOrEmpty(tokenToUse))
            {
                return BadRequest(new { message = "Token no proporcionado" });
            }

            _logger.LogInformation("Confirmando inscripción con token {Token}", tokenToUse);

            var resultado = await _transbankGateway.ConfirmarInscripcionAsync(tokenToUse);

            if (!resultado.Success)
            {
                return BadRequest(new { message = resultado.ErrorMessage });
            }

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al confirmar inscripción de tarjeta");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Elimina un medio de pago
    /// </summary>
    [HttpDelete("medios-pago/{paymentMethodId}")]
    [Authorize]
    public async Task<ActionResult> EliminarMedioPago(int paymentMethodId)
    {
        try
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(usuarioId))
            {
                return Unauthorized();
            }

            var resultado = await _transbankGateway.EliminarMedioPagoAsync(paymentMethodId, usuarioId);

            if (!resultado)
            {
                return NotFound(new { message = "Medio de pago no encontrado" });
            }

            return Ok(new { message = "Medio de pago eliminado exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar medio de pago");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    // ==================== CONSULTAS ====================

    /// <summary>
    /// Obtiene los medios de pago del usuario autenticado
    /// </summary>
    [HttpGet("medios-pago")]
    [Authorize]
    public async Task<ActionResult<List<PaymentMethodDto>>> ObtenerMediosPago()
    {
        try
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(usuarioId))
            {
                return Unauthorized();
            }

            var mediosPago = await _transbankGateway.ObtenerMediosPagoAsync(usuarioId);

            return Ok(mediosPago);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener medios de pago");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene el medio de pago predeterminado
    /// </summary>
    [HttpGet("medios-pago/predeterminado")]
    [Authorize]
    public async Task<ActionResult<PaymentMethodDto>> ObtenerMedioPagoPredeterminado()
    {
        try
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(usuarioId))
            {
                return Unauthorized();
            }

            var medioPago = await _transbankGateway.ObtenerMedioPagoPredeterminadoAsync(usuarioId);

            if (medioPago == null)
            {
                return NotFound(new { message = "No se encontró un medio de pago predeterminado" });
            }

            return Ok(medioPago);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener medio de pago predeterminado");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Establece un medio de pago como predeterminado
    /// </summary>
    [HttpPut("medios-pago/{paymentMethodId}/predeterminado")]
    [Authorize]
    public async Task<ActionResult> EstablecerMedioPagoPredeterminado(int paymentMethodId)
    {
        try
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(usuarioId))
            {
                return Unauthorized();
            }

            var resultado = await _transbankGateway.EstablecerMedioPagoPredeterminadoAsync(
                paymentMethodId,
                usuarioId);

            if (!resultado)
            {
                return BadRequest(new { message = "No se pudo establecer el medio de pago como predeterminado" });
            }

            return Ok(new { message = "Medio de pago establecido como predeterminado" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al establecer medio de pago predeterminado");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene el historial de transacciones del usuario
    /// </summary>
    [HttpGet("transacciones")]
    [Authorize]
    public async Task<ActionResult<List<TransbankTransactionDto>>> ObtenerHistorialTransacciones(
        [FromQuery] int limit = 50)
    {
        try
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(usuarioId))
            {
                return Unauthorized();
            }

            var transacciones = await _transbankGateway.ObtenerHistorialTransaccionesAsync(usuarioId, limit);

            return Ok(transacciones);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener historial de transacciones");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    // ==================== COBROS ====================

    /// <summary>
    /// Realiza un cobro con token (para testing - en producción usar billing service)
    /// </summary>
    [HttpPost("cobrar")]
    [Authorize]
    public async Task<ActionResult<CobrarConTokenResponseDto>> CobrarConToken(
        [FromBody] CobrarConTokenRequestDto request)
    {
        try
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(usuarioId))
            {
                return Unauthorized();
            }

            // Verificar que el usuario está cobrando a su propia cuenta
            if (request.UsuarioId != usuarioId)
            {
                return Forbid();
            }

            var resultado = await _transbankGateway.CobrarConTokenAsync(request);

            if (!resultado.Success)
            {
                return BadRequest(new { message = resultado.ErrorMessage });
            }

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar cobro");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Procesa el cobro inicial de una suscripción
    /// </summary>
    [HttpPost("suscripciones/{suscripcionId}/cobrar-inicial")]
    [Authorize]
    public async Task<ActionResult> CobrarSuscripcionInicial(int suscripcionId)
    {
        try
        {
            _logger.LogInformation("Procesando cobro inicial de suscripción {SuscripcionId}", suscripcionId);

            var (success, message, transactionId) = await _billingService.ProcesarCobroInicialAsync(suscripcionId);

            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message, transactionId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar cobro inicial de suscripción");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Renueva manualmente una suscripción
    /// </summary>
    [HttpPost("suscripciones/{suscripcionId}/renovar")]
    [Authorize]
    public async Task<ActionResult> RenovarSuscripcion(int suscripcionId)
    {
        try
        {
            _logger.LogInformation("Renovando suscripción {SuscripcionId}", suscripcionId);

            var (success, message) = await _billingService.RenovarSuscripcionAsync(suscripcionId);

            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al renovar suscripción");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    // ==================== WEBHOOK ====================

    /// <summary>
    /// Procesa notificaciones webhook de Transbank
    /// </summary>
    [HttpPost("webhook")]
    [AllowAnonymous] // Transbank no envía auth headers
    public async Task<ActionResult<WebhookResponseDto>> ProcesarWebhook(
        [FromBody] TransbankWebhookDto webhook)
    {
        try
        {
            _logger.LogInformation("Recibido webhook de Transbank. Token: {Token}", webhook.Token);

            var resultado = await _transbankGateway.ProcesarWebhookAsync(webhook);

            if (!resultado.Success)
            {
                _logger.LogWarning("Webhook procesado con errores: {Message}", resultado.Message);
                return BadRequest(resultado);
            }

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar webhook de Transbank");
            return StatusCode(500, new WebhookResponseDto
            {
                Success = false,
                Message = "Error interno del servidor"
            });
        }
    }

    // ==================== INFORMACIÓN ====================

    /// <summary>
    /// Obtiene información del entorno de Transbank configurado
    /// </summary>
    [HttpGet("info")]
    [Authorize(Roles = "Admin")] // Solo administradores
    public ActionResult<object> ObtenerInformacion()
    {
        return Ok(new
        {
            environment = _transbankGateway.GetEnvironmentInfo(),
            isSandbox = _transbankGateway.IsSandbox
        });
    }
}
