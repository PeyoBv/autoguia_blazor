using Rodavia.Core.DTOs;
using Rodavia.Core.Entities;

namespace Rodavia.Infrastructure.Services.Payments;

/// <summary>
/// Interfaz para el gateway de pagos con Transbank Webpay OneClick
/// </summary>
public interface ITransbankGateway
{
    // ==================== INSCRIPCIÓN DE TARJETA ====================

    /// <summary>
    /// Inicia el proceso de inscripción de una tarjeta para pagos recurrentes
    /// </summary>
    /// <param name="request">Datos de la inscripción</param>
    /// <param name="usuarioId">ID del usuario que inscribe la tarjeta</param>
    /// <returns>Información para redirigir al usuario a Webpay</returns>
    Task<IniciarInscripcionResponseDto> IniciarInscripcionAsync(
        IniciarInscripcionRequestDto request, 
        string usuarioId);

    /// <summary>
    /// Confirma la inscripción de una tarjeta después del redirect de Transbank
    /// </summary>
    /// <param name="token">Token de la transacción de Transbank</param>
    /// <returns>Resultado de la inscripción con token TBK para cobros futuros</returns>
    Task<ConfirmarInscripcionResponseDto> ConfirmarInscripcionAsync(string token);

    /// <summary>
    /// Elimina (desactiva) un medio de pago inscrito
    /// </summary>
    /// <param name="paymentMethodId">ID del medio de pago a eliminar</param>
    /// <param name="usuarioId">ID del usuario propietario</param>
    /// <returns>True si se eliminó exitosamente</returns>
    Task<bool> EliminarMedioPagoAsync(int paymentMethodId, string usuarioId);

    // ==================== COBROS ====================

    /// <summary>
    /// Realiza un cobro utilizando una tarjeta inscrita (token TBK)
    /// </summary>
    /// <param name="request">Datos del cobro</param>
    /// <returns>Resultado del cobro</returns>
    Task<CobrarConTokenResponseDto> CobrarConTokenAsync(CobrarConTokenRequestDto request);

    /// <summary>
    /// Realiza un cobro de suscripción (wrapper con lógica de negocio)
    /// </summary>
    /// <param name="suscripcionId">ID de la suscripción a cobrar</param>
    /// <param name="monto">Monto a cobrar</param>
    /// <returns>Resultado del cobro</returns>
    Task<CobrarConTokenResponseDto> CobrarSuscripcionAsync(int suscripcionId, decimal monto);

    // ==================== WEBHOOK ====================

    /// <summary>
    /// Procesa la notificación webhook de Transbank
    /// </summary>
    /// <param name="webhook">Datos del webhook</param>
    /// <returns>Resultado del procesamiento</returns>
    Task<WebhookResponseDto> ProcesarWebhookAsync(TransbankWebhookDto webhook);

    // ==================== CONSULTAS ====================

    /// <summary>
    /// Obtiene el estado de una transacción desde Transbank
    /// </summary>
    /// <param name="token">Token de la transacción</param>
    /// <returns>Estado actual de la transacción</returns>
    Task<TransbankTransactionDto> ObtenerEstadoTransaccionAsync(string token);

    /// <summary>
    /// Obtiene todos los medios de pago activos de un usuario
    /// </summary>
    /// <param name="usuarioId">ID del usuario</param>
    /// <returns>Lista de medios de pago</returns>
    Task<List<PaymentMethodDto>> ObtenerMediosPagoAsync(string usuarioId);

    /// <summary>
    /// Obtiene el medio de pago predeterminado de un usuario
    /// </summary>
    /// <param name="usuarioId">ID del usuario</param>
    /// <returns>Medio de pago predeterminado o null</returns>
    Task<PaymentMethodDto?> ObtenerMedioPagoPredeterminadoAsync(string usuarioId);

    /// <summary>
    /// Establece un medio de pago como predeterminado
    /// </summary>
    /// <param name="paymentMethodId">ID del medio de pago</param>
    /// <param name="usuarioId">ID del usuario propietario</param>
    /// <returns>True si se estableció exitosamente</returns>
    Task<bool> EstablecerMedioPagoPredeterminadoAsync(int paymentMethodId, string usuarioId);

    // ==================== AUDITORÍA ====================

    /// <summary>
    /// Obtiene el historial de transacciones de un usuario
    /// </summary>
    /// <param name="usuarioId">ID del usuario</param>
    /// <param name="limit">Límite de registros (por defecto 50)</param>
    /// <returns>Lista de transacciones</returns>
    Task<List<TransbankTransactionDto>> ObtenerHistorialTransaccionesAsync(
        string usuarioId, 
        int limit = 50);

    /// <summary>
    /// Obtiene detalles completos de una transacción
    /// </summary>
    /// <param name="transactionId">ID de la transacción</param>
    /// <returns>Detalles de la transacción</returns>
    Task<TransbankTransaction?> ObtenerDetalleTransaccionAsync(int transactionId);

    // ==================== UTILIDADES ====================

    /// <summary>
    /// Verifica si el gateway está en modo sandbox
    /// </summary>
    bool IsSandbox { get; }

    /// <summary>
    /// Obtiene la configuración actual del gateway
    /// </summary>
    string GetEnvironmentInfo();

    /// <summary>
    /// Valida que un medio de pago esté activo y disponible
    /// </summary>
    /// <param name="paymentMethodId">ID del medio de pago</param>
    /// <returns>True si el medio de pago es válido</returns>
    Task<bool> ValidarMedioPagoAsync(int paymentMethodId);
}
