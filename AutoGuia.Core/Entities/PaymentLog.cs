using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoGuia.Core.Entities;

/// <summary>
/// Niveles de severidad de los logs de pago
/// </summary>
public enum PaymentLogLevel
{
    /// <summary>
    /// Información general
    /// </summary>
    Info = 1,

    /// <summary>
    /// Advertencia
    /// </summary>
    Warning = 2,

    /// <summary>
    /// Error
    /// </summary>
    Error = 3,

    /// <summary>
    /// Error crítico
    /// </summary>
    Critical = 4
}

/// <summary>
/// Log de eventos relacionados con pagos y transacciones
/// </summary>
public class PaymentLog
{
    /// <summary>
    /// Identificador único del log
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// ID de la transacción relacionada (opcional)
    /// </summary>
    public int? TransactionId { get; set; }

    /// <summary>
    /// Transacción asociada
    /// </summary>
    [ForeignKey(nameof(TransactionId))]
    public virtual TransbankTransaction? Transaction { get; set; }

    /// <summary>
    /// ID del usuario relacionado (opcional)
    /// </summary>
    public string? UsuarioId { get; set; }

    /// <summary>
    /// Nivel de severidad del log
    /// </summary>
    public PaymentLogLevel Level { get; set; } = PaymentLogLevel.Info;

    /// <summary>
    /// Evento registrado (ej: "PAYMENT_INITIATED", "PAYMENT_APPROVED", "WEBHOOK_RECEIVED")
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Event { get; set; } = string.Empty;

    /// <summary>
    /// Mensaje descriptivo del evento
    /// </summary>
    [Required]
    [StringLength(1000)]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Información adicional en formato JSON
    /// </summary>
    [Column(TypeName = "text")]
    public string? AdditionalData { get; set; }

    /// <summary>
    /// Stack trace del error (si aplica)
    /// </summary>
    [Column(TypeName = "text")]
    public string? StackTrace { get; set; }

    /// <summary>
    /// IP desde donde se originó el evento
    /// </summary>
    [StringLength(50)]
    public string? IpAddress { get; set; }

    /// <summary>
    /// User Agent del cliente
    /// </summary>
    [StringLength(500)]
    public string? UserAgent { get; set; }

    /// <summary>
    /// Endpoint o método que generó el log
    /// </summary>
    [StringLength(200)]
    public string? Source { get; set; }

    /// <summary>
    /// Duración de la operación en milisegundos (si aplica)
    /// </summary>
    public int? DurationMs { get; set; }

    /// <summary>
    /// Fecha de creación del log
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Propiedades calculadas

    /// <summary>
    /// Descripción del nivel de log
    /// </summary>
    [NotMapped]
    public string LevelDescription => Level switch
    {
        PaymentLogLevel.Info => "Información",
        PaymentLogLevel.Warning => "Advertencia",
        PaymentLogLevel.Error => "Error",
        PaymentLogLevel.Critical => "Crítico",
        _ => "Desconocido"
    };

    /// <summary>
    /// Color CSS asociado al nivel de log (para UI)
    /// </summary>
    [NotMapped]
    public string LevelColor => Level switch
    {
        PaymentLogLevel.Info => "text-info",
        PaymentLogLevel.Warning => "text-warning",
        PaymentLogLevel.Error => "text-danger",
        PaymentLogLevel.Critical => "text-danger fw-bold",
        _ => "text-secondary"
    };
}
