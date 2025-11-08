using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rodavia.Core.Entities;

/// <summary>
/// Medio de pago (tarjeta) inscrito vía Transbank OneClick para cobros recurrentes
/// </summary>
public class PaymentMethod
{
    /// <summary>
    /// Identificador único del medio de pago
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// ID del usuario propietario del medio de pago (FK a IdentityUser)
    /// </summary>
    [Required]
    public string UsuarioId { get; set; } = string.Empty;

    /// <summary>
    /// Token TBK (Transbank Token) para realizar cobros recurrentes
    /// </summary>
    [Required]
    [StringLength(200)]
    public string TbkToken { get; set; } = string.Empty;

    /// <summary>
    /// Últimos 4 dígitos de la tarjeta inscrita
    /// </summary>
    [Required]
    [StringLength(4)]
    public string Last4Digits { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de tarjeta (Visa, Mastercard, etc.)
    /// </summary>
    [StringLength(50)]
    public string? CardType { get; set; }

    /// <summary>
    /// Fecha de expiración de la tarjeta (MM/YY)
    /// </summary>
    [StringLength(7)]
    public string? ExpirationDate { get; set; }

    /// <summary>
    /// Nombre del titular de la tarjeta
    /// </summary>
    [StringLength(200)]
    public string? CardholderName { get; set; }

    /// <summary>
    /// Email asociado al medio de pago
    /// </summary>
    [StringLength(200)]
    public string? Email { get; set; }

    /// <summary>
    /// Indica si este es el medio de pago predeterminado del usuario
    /// </summary>
    public bool IsDefault { get; set; } = false;

    /// <summary>
    /// Indica si el medio de pago está activo y puede usarse
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Fecha de inscripción del medio de pago
    /// </summary>
    public DateTime InscriptionDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha de la última validación exitosa del token
    /// </summary>
    public DateTime? LastValidationDate { get; set; }

    /// <summary>
    /// Número de intentos de cobro fallidos consecutivos
    /// </summary>
    public int FailedAttempts { get; set; } = 0;

    /// <summary>
    /// Fecha del último intento de cobro fallido
    /// </summary>
    public DateTime? LastFailedAttempt { get; set; }

    /// <summary>
    /// Razón de desactivación (si está inactivo)
    /// </summary>
    [StringLength(500)]
    public string? InactiveReason { get; set; }

    /// <summary>
    /// Fecha de creación del registro
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha de última actualización del registro
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navegación

    /// <summary>
    /// Transacciones realizadas con este medio de pago
    /// </summary>
    public virtual ICollection<TransbankTransaction> Transactions { get; set; } = new List<TransbankTransaction>();

    // Propiedades calculadas

    /// <summary>
    /// Máscara de tarjeta para mostrar (****1234)
    /// </summary>
    [NotMapped]
    public string CardMask => $"**** **** **** {Last4Digits}";

    /// <summary>
    /// Descripción legible del medio de pago
    /// </summary>
    [NotMapped]
    public string Description => $"{CardType ?? "Tarjeta"} terminada en {Last4Digits}";

    /// <summary>
    /// Verifica si el medio de pago requiere validación (muchos fallos recientes)
    /// </summary>
    [NotMapped]
    public bool RequiresValidation => FailedAttempts >= 3;
}
