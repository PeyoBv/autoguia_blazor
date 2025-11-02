using Rodavia.Core.Entities;

namespace Rodavia.Infrastructure.Services.Payments;

/// <summary>
/// Interfaz del servicio de facturación de suscripciones
/// </summary>
public interface ISubscriptionBillingService
{
    /// <summary>
    /// Procesa el cobro inicial de una suscripción (primera cuota)
    /// </summary>
    Task<(bool Success, string Message, int? TransactionId)> ProcesarCobroInicialAsync(int suscripcionId);

    /// <summary>
    /// Procesa cobros recurrentes de todas las suscripciones que deben renovarse
    /// </summary>
    Task<BillingBatchResult> ProcesarCobrosRecurrentesAsync();

    /// <summary>
    /// Renueva manualmente una suscripción específica
    /// </summary>
    Task<(bool Success, string Message)> RenovarSuscripcionAsync(int suscripcionId);

    /// <summary>
    /// Verifica y actualiza el estado de suscripciones vencidas
    /// </summary>
    Task ActualizarEstadoSuscripcionesVencidasAsync();

    /// <summary>
    /// Intenta recuperar cobros fallidos
    /// </summary>
    Task<BillingBatchResult> ReintentarCobrosFallidosAsync();
}

/// <summary>
/// Resultado de un lote de facturación
/// </summary>
public class BillingBatchResult
{
    public int TotalProcesados { get; set; }
    public int Exitosos { get; set; }
    public int Fallidos { get; set; }
    public List<string> Errores { get; set; } = new();
    public decimal MontoTotalCobrado { get; set; }
}
