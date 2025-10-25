using AutoGuia.Core.Entities;
using AutoGuia.Web.DTOs;

namespace AutoGuia.Web.Services;

/// <summary>
/// Servicio para gestión de suscripciones de usuarios
/// </summary>
public interface ISuscripcionService
{
    /// <summary>
    /// Obtiene la suscripción activa del usuario
    /// </summary>
    /// <param name="usuarioId">ID del usuario</param>
    /// <returns>Suscripción activa o null si no tiene</returns>
    Task<SuscripcionDto?> ObtenerSuscripcionActualAsync(string usuarioId);

    /// <summary>
    /// Obtiene el historial completo de suscripciones del usuario
    /// </summary>
    /// <param name="usuarioId">ID del usuario</param>
    /// <returns>Lista de suscripciones ordenadas por fecha</returns>
    Task<IEnumerable<SuscripcionDto>> ObtenerHistorialAsync(string usuarioId);

    /// <summary>
    /// Obtiene todos los planes disponibles para suscripción
    /// </summary>
    /// <returns>Lista de planes activos ordenados por orden</returns>
    Task<IEnumerable<PlanDto>> ObtenerPlanesAsync();

    /// <summary>
    /// Cambia el plan de suscripción del usuario
    /// Cancela la suscripción actual y crea una nueva con el nuevo plan
    /// </summary>
    /// <param name="usuarioId">ID del usuario</param>
    /// <param name="nuevoPlanId">ID del nuevo plan</param>
    /// <param name="metodoPago">Método de pago utilizado</param>
    /// <param name="transaccionId">ID de la transacción de pago</param>
    /// <returns>Nueva suscripción creada</returns>
    Task<SuscripcionDto> CambiarPlanAsync(string usuarioId, int nuevoPlanId, string? metodoPago = null, string? transaccionId = null);

    /// <summary>
    /// Cancela la suscripción activa del usuario
    /// </summary>
    /// <param name="usuarioId">ID del usuario</param>
    /// <param name="motivo">Motivo de cancelación</param>
    /// <returns>True si se canceló exitosamente</returns>
    Task<bool> CancelarSuscripcionAsync(string usuarioId, string? motivo = null);

    /// <summary>
    /// Crea una nueva suscripción para el usuario
    /// </summary>
    /// <param name="usuarioId">ID del usuario</param>
    /// <param name="planId">ID del plan</param>
    /// <param name="metodoPago">Método de pago</param>
    /// <param name="transaccionId">ID de transacción</param>
    /// <param name="montoPagado">Monto pagado</param>
    /// <returns>Suscripción creada</returns>
    Task<SuscripcionDto> CrearSuscripcionAsync(string usuarioId, int planId, string? metodoPago = null, string? transaccionId = null, decimal? montoPagado = null);

    /// <summary>
    /// Valida si la suscripción está vigente
    /// </summary>
    /// <param name="suscripcion">Suscripción a validar</param>
    /// <returns>True si está vigente</returns>
    bool ValidarVigencia(SuscripcionDto suscripcion);

    /// <summary>
    /// Verifica si el usuario puede usar diagnósticos de IA
    /// </summary>
    /// <param name="usuarioId">ID del usuario</param>
    /// <returns>True si puede usar, false si alcanzó el límite</returns>
    Task<bool> PuedeUsarDiagnosticoAsync(string usuarioId);

    /// <summary>
    /// Verifica si el usuario puede realizar búsquedas
    /// </summary>
    /// <param name="usuarioId">ID del usuario</param>
    /// <returns>True si puede buscar, false si alcanzó el límite</returns>
    Task<bool> PuedeUsarBusquedaAsync(string usuarioId);

    /// <summary>
    /// Incrementa el contador de diagnósticos utilizados
    /// </summary>
    /// <param name="usuarioId">ID del usuario</param>
    Task IncrementarUsoDiagnosticoAsync(string usuarioId);

    /// <summary>
    /// Incrementa el contador de búsquedas utilizadas
    /// </summary>
    /// <param name="usuarioId">ID del usuario</param>
    Task IncrementarUsoBusquedaAsync(string usuarioId);

    /// <summary>
    /// Resetea los contadores mensuales/diarios según corresponda
    /// </summary>
    /// <param name="usuarioId">ID del usuario</param>
    Task ResetearContadoresAsync(string usuarioId);

    /// <summary>
    /// Obtiene estadísticas de uso de la suscripción
    /// </summary>
    /// <param name="usuarioId">ID del usuario</param>
    /// <returns>Diccionario con estadísticas</returns>
    Task<Dictionary<string, object>> ObtenerEstadisticasUsoAsync(string usuarioId);
}
