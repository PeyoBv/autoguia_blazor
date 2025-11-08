using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rodavia.Core.Entities;

/// <summary>
/// Tipos de transacción de Transbank
/// </summary>
public enum TransbankTransactionType
{
    /// <summary>
    /// Inscripción de nueva tarjeta (OneClick)
    /// </summary>
    Inscription = 1,

    /// <summary>
    /// Cobro automático recurrente
    /// </summary>
    RecurringCharge = 2,

    /// <summary>
    /// Cobro manual (primera cuota o ajuste)
    /// </summary>
    ManualCharge = 3,

    /// <summary>
    /// Reversa de transacción
    /// </summary>
    Reversal = 4,

    /// <summary>
    /// Anulación de transacción
    /// </summary>
    Refund = 5
}

/// <summary>
/// Estados de una transacción de Transbank
/// </summary>
public enum TransbankTransactionStatus
{
    /// <summary>
    /// Transacción iniciada, pendiente de confirmación
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Transacción aprobada exitosamente
    /// </summary>
    Approved = 2,

    /// <summary>
    /// Transacción rechazada
    /// </summary>
    Rejected = 3,

    /// <summary>
    /// Transacción anulada
    /// </summary>
    Cancelled = 4,

    /// <summary>
    /// Transacción con error
    /// </summary>
    Error = 5,

    /// <summary>
    /// Transacción expirada (timeout)
    /// </summary>
    Expired = 6,

    /// <summary>
    /// Transacción revertida
    /// </summary>
    Reversed = 7
}

/// <summary>
/// Registro de transacción con Transbank (OneClick)
/// </summary>
public class TransbankTransaction
{
    /// <summary>
    /// Identificador único de la transacción
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// ID del usuario asociado (FK a IdentityUser)
    /// </summary>
    [Required]
    public string UsuarioId { get; set; } = string.Empty;

    /// <summary>
    /// ID del medio de pago utilizado (opcional, NULL para inscripciones)
    /// </summary>
    public int? PaymentMethodId { get; set; }

    /// <summary>
    /// Medio de pago asociado
    /// </summary>
    [ForeignKey(nameof(PaymentMethodId))]
    public virtual PaymentMethod? PaymentMethod { get; set; }

    /// <summary>
    /// ID de la suscripción relacionada (opcional)
    /// </summary>
    public int? SuscripcionId { get; set; }

    /// <summary>
    /// Suscripción asociada
    /// </summary>
    [ForeignKey(nameof(SuscripcionId))]
    public virtual Suscripcion? Suscripcion { get; set; }

    /// <summary>
    /// Tipo de transacción
    /// </summary>
    public TransbankTransactionType Type { get; set; }

    /// <summary>
    /// Estado actual de la transacción
    /// </summary>
    public TransbankTransactionStatus Status { get; set; } = TransbankTransactionStatus.Pending;

    /// <summary>
    /// Token de transacción de Transbank (para seguimiento)
    /// </summary>
    [Required]
    [StringLength(200)]
    public string TransactionToken { get; set; } = string.Empty;

    /// <summary>
    /// Código de autorización de Transbank (cuando es aprobada)
    /// </summary>
    [StringLength(50)]
    public string? AuthorizationCode { get; set; }

    /// <summary>
    /// Monto de la transacción en CLP
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Orden de compra única (idempotencia)
    /// </summary>
    [Required]
    [StringLength(100)]
    public string BuyOrder { get; set; } = string.Empty;

    /// <summary>
    /// Número de cuotas (1 para pago al contado)
    /// </summary>
    public int Installments { get; set; } = 1;

    /// <summary>
    /// Código de respuesta de Transbank
    /// </summary>
    [StringLength(10)]
    public string? ResponseCode { get; set; }

    /// <summary>
    /// Mensaje de respuesta de Transbank
    /// </summary>
    [StringLength(500)]
    public string? ResponseMessage { get; set; }

    /// <summary>
    /// Fecha de contabilización de la transacción
    /// </summary>
    public DateTime? AccountingDate { get; set; }

    /// <summary>
    /// Fecha de la transacción en Transbank
    /// </summary>
    public DateTime? TransactionDate { get; set; }

    /// <summary>
    /// URL de retorno después de la transacción (para inscripciones)
    /// </summary>
    [StringLength(500)]
    public string? ReturnUrl { get; set; }

    /// <summary>
    /// Entorno usado (Sandbox o Production)
    /// </summary>
    [Required]
    [StringLength(20)]
    public string Environment { get; set; } = "Sandbox";

    /// <summary>
    /// Request JSON completo enviado a Transbank (para auditoría)
    /// </summary>
    [Column(TypeName = "text")]
    public string? RequestPayload { get; set; }

    /// <summary>
    /// Response JSON completo recibido de Transbank (para auditoría)
    /// </summary>
    [Column(TypeName = "text")]
    public string? ResponsePayload { get; set; }

    /// <summary>
    /// Número de reintentos realizados
    /// </summary>
    public int RetryCount { get; set; } = 0;

    /// <summary>
    /// Mensaje de error (si la transacción falló)
    /// </summary>
    [StringLength(1000)]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// IP del usuario que inició la transacción
    /// </summary>
    [StringLength(50)]
    public string? UserIp { get; set; }

    /// <summary>
    /// User Agent del navegador
    /// </summary>
    [StringLength(500)]
    public string? UserAgent { get; set; }

    /// <summary>
    /// Indica si el webhook fue procesado correctamente
    /// </summary>
    public bool WebhookProcessed { get; set; } = false;

    /// <summary>
    /// Fecha de procesamiento del webhook
    /// </summary>
    public DateTime? WebhookProcessedAt { get; set; }

    /// <summary>
    /// Fecha de creación del registro
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha de última actualización del registro
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Propiedades calculadas

    /// <summary>
    /// Verifica si la transacción fue exitosa
    /// </summary>
    [NotMapped]
    public bool IsSuccessful => Status == TransbankTransactionStatus.Approved;

    /// <summary>
    /// Verifica si la transacción está pendiente
    /// </summary>
    [NotMapped]
    public bool IsPending => Status == TransbankTransactionStatus.Pending;

    /// <summary>
    /// Verifica si la transacción falló
    /// </summary>
    [NotMapped]
    public bool IsFailed => Status is TransbankTransactionStatus.Rejected 
                                    or TransbankTransactionStatus.Error 
                                    or TransbankTransactionStatus.Cancelled;

    /// <summary>
    /// Descripción del tipo de transacción
    /// </summary>
    [NotMapped]
    public string TypeDescription => Type switch
    {
        TransbankTransactionType.Inscription => "Inscripción de tarjeta",
        TransbankTransactionType.RecurringCharge => "Cobro recurrente",
        TransbankTransactionType.ManualCharge => "Cobro manual",
        TransbankTransactionType.Reversal => "Reversa",
        TransbankTransactionType.Refund => "Reembolso",
        _ => "Desconocido"
    };

    /// <summary>
    /// Descripción del estado de la transacción
    /// </summary>
    [NotMapped]
    public string StatusDescription => Status switch
    {
        TransbankTransactionStatus.Pending => "Pendiente",
        TransbankTransactionStatus.Approved => "Aprobada",
        TransbankTransactionStatus.Rejected => "Rechazada",
        TransbankTransactionStatus.Cancelled => "Cancelada",
        TransbankTransactionStatus.Error => "Error",
        TransbankTransactionStatus.Expired => "Expirada",
        TransbankTransactionStatus.Reversed => "Revertida",
        _ => "Desconocido"
    };
}
