namespace Rodavia.Core.DTOs;

// ==================== REQUEST DTOs ====================

/// <summary>
/// Request para iniciar inscripción de tarjeta en Transbank OneClick
/// </summary>
public class IniciarInscripcionRequestDto
{
    /// <summary>
    /// Email del usuario que inscribe la tarjeta
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del usuario que inscribe la tarjeta
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// URL de retorno después de inscribir la tarjeta
    /// </summary>
    public string ReturnUrl { get; set; } = string.Empty;

    /// <summary>
    /// ID del plan que el usuario quiere suscribirse
    /// </summary>
    public int PlanId { get; set; }

    /// <summary>
    /// ID del usuario que realiza la inscripción
    /// </summary>
    public string UsuarioId { get; set; } = string.Empty;
}

/// <summary>
/// Request para confirmar inscripción después del redirect de Transbank
/// </summary>
public class ConfirmarInscripcionRequestDto
{
    /// <summary>
    /// Token de la transacción de Transbank
    /// </summary>
    public string Token { get; set; } = string.Empty;
}

/// <summary>
/// Request para realizar un cobro con token OneClick
/// </summary>
public class CobrarConTokenRequestDto
{
    /// <summary>
    /// ID del usuario a cobrar
    /// </summary>
    public string UsuarioId { get; set; } = string.Empty;

    /// <summary>
    /// ID del medio de pago (tarjeta inscrita)
    /// </summary>
    public int PaymentMethodId { get; set; }

    /// <summary>
    /// Monto a cobrar en CLP
    /// </summary>
    public decimal Monto { get; set; }

    /// <summary>
    /// ID de la suscripción relacionada (opcional)
    /// </summary>
    public int? SuscripcionId { get; set; }

    /// <summary>
    /// Orden de compra única (para idempotencia)
    /// </summary>
    public string BuyOrder { get; set; } = string.Empty;

    /// <summary>
    /// Número de cuotas (1 por defecto)
    /// </summary>
    public int Cuotas { get; set; } = 1;
}

/// <summary>
/// Request para procesar webhook de Transbank
/// </summary>
public class TransbankWebhookDto
{
    /// <summary>
    /// Token de la transacción
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Orden de compra
    /// </summary>
    public string BuyOrder { get; set; } = string.Empty;

    /// <summary>
    /// Código de autorización
    /// </summary>
    public string? AuthorizationCode { get; set; }

    /// <summary>
    /// Monto de la transacción
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Estado de la transacción
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Código de respuesta
    /// </summary>
    public string? ResponseCode { get; set; }

    /// <summary>
    /// Fecha de la transacción
    /// </summary>
    public DateTime? TransactionDate { get; set; }
}

// ==================== RESPONSE DTOs ====================

/// <summary>
/// Response al iniciar inscripción de tarjeta
/// </summary>
public class IniciarInscripcionResponseDto
{
    /// <summary>
    /// Token de la transacción generado por Transbank
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// URL de Transbank a donde redirigir al usuario
    /// </summary>
    public string UrlWebpay { get; set; } = string.Empty;

    /// <summary>
    /// ID de la transacción en AutoGuía
    /// </summary>
    public int TransactionId { get; set; }

    /// <summary>
    /// Indica si la operación fue exitosa
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Mensaje de error (si aplica)
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Response al confirmar inscripción de tarjeta
/// </summary>
public class ConfirmarInscripcionResponseDto
{
    /// <summary>
    /// Indica si la inscripción fue exitosa
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Token TBK para cobros futuros
    /// </summary>
    public string? TbkToken { get; set; }

    /// <summary>
    /// Últimos 4 dígitos de la tarjeta
    /// </summary>
    public string? Last4Digits { get; set; }

    /// <summary>
    /// Tipo de tarjeta
    /// </summary>
    public string? CardType { get; set; }

    /// <summary>
    /// ID del medio de pago creado
    /// </summary>
    public int? PaymentMethodId { get; set; }

    /// <summary>
    /// Código de autorización de Transbank
    /// </summary>
    public string? AuthorizationCode { get; set; }

    /// <summary>
    /// Mensaje de error (si la inscripción falló)
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Código de respuesta de Transbank
    /// </summary>
    public string? ResponseCode { get; set; }
}

/// <summary>
/// Response al realizar un cobro con token
/// </summary>
public class CobrarConTokenResponseDto
{
    /// <summary>
    /// Indica si el cobro fue exitoso
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// ID de la transacción en AutoGuía
    /// </summary>
    public int TransactionId { get; set; }

    /// <summary>
    /// Código de autorización de Transbank
    /// </summary>
    public string? AuthorizationCode { get; set; }

    /// <summary>
    /// Monto cobrado
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Orden de compra
    /// </summary>
    public string BuyOrder { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de la transacción
    /// </summary>
    public DateTime? TransactionDate { get; set; }

    /// <summary>
    /// Código de respuesta de Transbank
    /// </summary>
    public string? ResponseCode { get; set; }

    /// <summary>
    /// Mensaje de respuesta
    /// </summary>
    public string? ResponseMessage { get; set; }

    /// <summary>
    /// Mensaje de error (si el cobro falló)
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Últimos 4 dígitos de la tarjeta usada
    /// </summary>
    public string? Last4Digits { get; set; }
}

/// <summary>
/// Response al procesar webhook
/// </summary>
public class WebhookResponseDto
{
    /// <summary>
    /// Indica si el webhook fue procesado exitosamente
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Mensaje de resultado
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// ID de la transacción procesada
    /// </summary>
    public int? TransactionId { get; set; }
}

// ==================== INFO DTOs ====================

/// <summary>
/// DTO para información de medio de pago
/// </summary>
public class PaymentMethodDto
{
    public int Id { get; set; }
    public string Last4Digits { get; set; } = string.Empty;
    public string? CardType { get; set; }
    public string? ExpirationDate { get; set; }
    public string? CardholderName { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
    public DateTime InscriptionDate { get; set; }
    public int FailedAttempts { get; set; }
    public string CardMask { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// DTO para información de transacción
/// </summary>
public class TransbankTransactionDto
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string BuyOrder { get; set; } = string.Empty;
    public string? AuthorizationCode { get; set; }
    public string? ResponseMessage { get; set; }
    public DateTime? TransactionDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string TypeDescription { get; set; } = string.Empty;
    public string StatusDescription { get; set; } = string.Empty;
}
