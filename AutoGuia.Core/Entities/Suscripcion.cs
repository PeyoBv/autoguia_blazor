using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoGuia.Core.Entities;

/// <summary>
/// Suscripción de un usuario a un plan específico
/// </summary>
public class Suscripcion
{
    /// <summary>
    /// Identificador único de la suscripción
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// ID del usuario suscrito (FK a IdentityUser)
    /// </summary>
    [Required]
    public string UsuarioId { get; set; } = string.Empty;

    /// <summary>
    /// ID del plan suscrito (FK a Plan)
    /// </summary>
    [Required]
    public int PlanId { get; set; }

    /// <summary>
    /// Plan asociado a la suscripción
    /// </summary>
    [ForeignKey(nameof(PlanId))]
    public virtual Plan? Plan { get; set; }

    /// <summary>
    /// Fecha de inicio de la suscripción
    /// </summary>
    public DateTime FechaInicio { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha de vencimiento de la suscripción
    /// </summary>
    public DateTime FechaVencimiento { get; set; }

    /// <summary>
    /// Estado actual de la suscripción
    /// </summary>
    public EstadoSuscripcion Estado { get; set; } = EstadoSuscripcion.Activa;

    /// <summary>
    /// Método de pago utilizado (ej: "Tarjeta de Crédito", "PayPal", "WebPay", "MercadoPago")
    /// </summary>
    [StringLength(100)]
    public string? MetodoPago { get; set; }

    /// <summary>
    /// Referencia de factura o comprobante de pago
    /// </summary>
    [StringLength(200)]
    public string? ReferenciaFactura { get; set; }

    /// <summary>
    /// ID de transacción del proveedor de pagos
    /// </summary>
    [StringLength(200)]
    public string? TransaccionId { get; set; }

    /// <summary>
    /// Monto pagado en la suscripción
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal MontoPagado { get; set; }

    /// <summary>
    /// Indica si la suscripción se renovará automáticamente
    /// </summary>
    public bool RenovacionAutomatica { get; set; } = false;

    /// <summary>
    /// Fecha en que se canceló la suscripción (si aplica)
    /// </summary>
    public DateTime? FechaCancelacion { get; set; }

    /// <summary>
    /// Motivo de cancelación de la suscripción
    /// </summary>
    [StringLength(500)]
    public string? MotivoCancelacion { get; set; }

    /// <summary>
    /// Número de diagnósticos utilizados en el período actual
    /// </summary>
    public int DiagnosticosUtilizados { get; set; } = 0;

    /// <summary>
    /// Número de búsquedas utilizadas en el día actual
    /// </summary>
    public int BusquedasUtilizadas { get; set; } = 0;

    /// <summary>
    /// Fecha del último reseteo de contadores (para límites diarios/mensuales)
    /// </summary>
    public DateTime UltimoReseteo { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Notas adicionales sobre la suscripción
    /// </summary>
    [StringLength(1000)]
    public string? Notas { get; set; }

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
    /// Verifica si la suscripción está vigente
    /// </summary>
    [NotMapped]
    public bool EsVigente
    {
        get
        {
            return Estado == EstadoSuscripcion.Activa &&
                   FechaVencimiento > DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Verifica si la suscripción está próxima a vencer (menos de 7 días)
    /// </summary>
    [NotMapped]
    public bool ProximaAVencer
    {
        get
        {
            if (!EsVigente) return false;
            var diasRestantes = (FechaVencimiento - DateTime.UtcNow).Days;
            return diasRestantes <= 7 && diasRestantes > 0;
        }
    }

    /// <summary>
    /// Calcula los días restantes de la suscripción
    /// </summary>
    [NotMapped]
    public int DiasRestantes
    {
        get
        {
            var dias = (FechaVencimiento - DateTime.UtcNow).Days;
            return dias > 0 ? dias : 0;
        }
    }

    /// <summary>
    /// Obtiene el texto del estado en español
    /// </summary>
    [NotMapped]
    public string EstadoTexto
    {
        get
        {
            return Estado switch
            {
                EstadoSuscripcion.Activa => "Activa",
                EstadoSuscripcion.Cancelada => "Cancelada",
                EstadoSuscripcion.Vencida => "Vencida",
                EstadoSuscripcion.Prueba => "En Prueba",
                EstadoSuscripcion.Suspendida => "Suspendida",
                _ => "Desconocido"
            };
        }
    }

    /// <summary>
    /// Verifica si se alcanzó el límite de diagnósticos del plan
    /// </summary>
    [NotMapped]
    public bool LimiteDiagnosticosAlcanzado
    {
        get
        {
            if (Plan == null || Plan.LimiteDiagnosticos == 0) return false;
            return DiagnosticosUtilizados >= Plan.LimiteDiagnosticos;
        }
    }

    /// <summary>
    /// Verifica si se alcanzó el límite de búsquedas del plan
    /// </summary>
    [NotMapped]
    public bool LimiteBusquedasAlcanzado
    {
        get
        {
            if (Plan == null || Plan.LimiteBusquedas == 0) return false;
            return BusquedasUtilizadas >= Plan.LimiteBusquedas;
        }
    }

    /// <summary>
    /// Calcula el porcentaje de uso del límite de diagnósticos
    /// </summary>
    [NotMapped]
    public int PorcentajeUsoDiagnosticos
    {
        get
        {
            if (Plan == null || Plan.LimiteDiagnosticos == 0) return 0;
            return Math.Min(100, (DiagnosticosUtilizados * 100) / Plan.LimiteDiagnosticos);
        }
    }
}
