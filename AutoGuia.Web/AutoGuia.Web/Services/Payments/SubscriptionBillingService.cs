using AutoGuia.Core.Entities;
using AutoGuia.Infrastructure.Services.Payments;
using AutoGuia.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoGuia.Web.Services.Payments;

/// <summary>
/// Servicio para gestionar la facturación y cobros de suscripciones
/// </summary>
public class SubscriptionBillingService : ISubscriptionBillingService
{
    private readonly ApplicationDbContext _context;
    private readonly ITransbankGateway _transbankGateway;
    private readonly ILogger<SubscriptionBillingService> _logger;

    public SubscriptionBillingService(
        ApplicationDbContext context,
        ITransbankGateway transbankGateway,
        ILogger<SubscriptionBillingService> logger)
    {
        _context = context;
        _transbankGateway = transbankGateway;
        _logger = logger;
    }

    /// <summary>
    /// Procesa el cobro inicial cuando el usuario activa una suscripción
    /// </summary>
    public async Task<(bool Success, string Message, int? TransactionId)> ProcesarCobroInicialAsync(int suscripcionId)
    {
        try
        {
            _logger.LogInformation("Procesando cobro inicial para suscripción {SuscripcionId}", suscripcionId);

            var suscripcion = await _context.Suscripciones
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(s => s.Id == suscripcionId);

            if (suscripcion == null)
            {
                return (false, "Suscripción no encontrada", null);
            }

            if (suscripcion.Plan == null)
            {
                return (false, "Plan no encontrado", null);
            }

            // Si el plan es gratuito, no se cobra
            if (suscripcion.Plan.EsGratuito)
            {
                suscripcion.Estado = EstadoSuscripcion.Activa;
                suscripcion.FechaInicio = DateTime.UtcNow;
                suscripcion.FechaVencimiento = CalcularFechaVencimiento(DateTime.UtcNow, suscripcion.Plan.Duracion);
                suscripcion.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return (true, "Suscripción gratuita activada", null);
            }

            // Verificar que exista un medio de pago
            var medioPago = await _transbankGateway.ObtenerMedioPagoPredeterminadoAsync(suscripcion.UsuarioId);

            if (medioPago == null)
            {
                return (false, "No se encontró un medio de pago válido. Por favor, inscriba una tarjeta.", null);
            }

            // Realizar cobro
            var resultado = await _transbankGateway.CobrarSuscripcionAsync(suscripcionId, suscripcion.Plan.Precio);

            if (resultado.Success)
            {
                // Actualizar suscripción
                suscripcion.Estado = EstadoSuscripcion.Activa;
                suscripcion.FechaInicio = DateTime.UtcNow;
                suscripcion.FechaVencimiento = CalcularFechaVencimiento(DateTime.UtcNow, suscripcion.Plan.Duracion);
                suscripcion.MontoPagado = suscripcion.Plan.Precio;
                suscripcion.TransaccionId = resultado.AuthorizationCode;
                suscripcion.MetodoPago = $"Transbank - {medioPago.Description}";
                suscripcion.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Cobro inicial exitoso para suscripción {SuscripcionId}. Transacción: {TransactionId}",
                    suscripcionId, resultado.TransactionId);

                return (true, "Suscripción activada y cobro realizado exitosamente", resultado.TransactionId);
            }
            else
            {
                // Marcar suscripción como suspendida
                suscripcion.Estado = EstadoSuscripcion.Suspendida;
                suscripcion.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogWarning("Cobro inicial fallido para suscripción {SuscripcionId}. Error: {Error}",
                    suscripcionId, resultado.ErrorMessage);

                return (false, $"Error al procesar el pago: {resultado.ErrorMessage}", null);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar cobro inicial para suscripción {SuscripcionId}", suscripcionId);
            return (false, $"Error interno: {ex.Message}", null);
        }
    }

    /// <summary>
    /// Procesa cobros recurrentes para suscripciones próximas a vencer
    /// </summary>
    public async Task<BillingBatchResult> ProcesarCobrosRecurrentesAsync()
    {
        var resultado = new BillingBatchResult();

        try
        {
            _logger.LogInformation("Iniciando procesamiento de cobros recurrentes");

            // Obtener suscripciones que vencen en los próximos 3 días y tienen renovación automática
            var fechaLimite = DateTime.UtcNow.AddDays(3);

            var suscripcionesPorRenovar = await _context.Suscripciones
                .Include(s => s.Plan)
                .Where(s =>
                    s.Estado == EstadoSuscripcion.Activa &&
                    s.RenovacionAutomatica &&
                    s.FechaVencimiento <= fechaLimite &&
                    s.FechaVencimiento > DateTime.UtcNow)
                .ToListAsync();

            resultado.TotalProcesados = suscripcionesPorRenovar.Count;

            _logger.LogInformation("Encontradas {Count} suscripciones para renovar", suscripcionesPorRenovar.Count);

            foreach (var suscripcion in suscripcionesPorRenovar)
            {
                try
                {
                    if (suscripcion.Plan == null || suscripcion.Plan.EsGratuito)
                    {
                        // Renovar automáticamente planes gratuitos
                        suscripcion.FechaVencimiento = CalcularFechaVencimiento(
                            suscripcion.FechaVencimiento,
                            suscripcion.Plan?.Duracion ?? TipoDuracion.Mensual);
                        suscripcion.UpdatedAt = DateTime.UtcNow;
                        await _context.SaveChangesAsync();

                        resultado.Exitosos++;
                        continue;
                    }

                    // Intentar cobro
                    var resultadoCobro = await _transbankGateway.CobrarSuscripcionAsync(
                        suscripcion.Id,
                        suscripcion.Plan.Precio);

                    if (resultadoCobro.Success)
                    {
                        // Extender fecha de vencimiento
                        suscripcion.FechaVencimiento = CalcularFechaVencimiento(
                            suscripcion.FechaVencimiento,
                            suscripcion.Plan.Duracion);
                        suscripcion.MontoPagado = suscripcion.Plan.Precio;
                        suscripcion.TransaccionId = resultadoCobro.AuthorizationCode;
                        suscripcion.UpdatedAt = DateTime.UtcNow;

                        await _context.SaveChangesAsync();

                        resultado.Exitosos++;
                        resultado.MontoTotalCobrado += suscripcion.Plan.Precio;

                        _logger.LogInformation("Suscripción {Id} renovada exitosamente", suscripcion.Id);
                    }
                    else
                    {
                        // Suspender suscripción si el cobro falla
                        suscripcion.Estado = EstadoSuscripcion.Suspendida;
                        suscripcion.UpdatedAt = DateTime.UtcNow;
                        await _context.SaveChangesAsync();

                        resultado.Fallidos++;
                        resultado.Errores.Add($"Suscripción {suscripcion.Id}: {resultadoCobro.ErrorMessage}");

                        _logger.LogWarning("Fallo al renovar suscripción {Id}: {Error}",
                            suscripcion.Id, resultadoCobro.ErrorMessage);
                    }
                }
                catch (Exception ex)
                {
                    resultado.Fallidos++;
                    resultado.Errores.Add($"Suscripción {suscripcion.Id}: {ex.Message}");

                    _logger.LogError(ex, "Error al procesar renovación de suscripción {Id}", suscripcion.Id);
                }
            }

            _logger.LogInformation(
                "Cobros recurrentes finalizados. Procesados: {Total}, Exitosos: {Success}, Fallidos: {Failed}",
                resultado.TotalProcesados, resultado.Exitosos, resultado.Fallidos);

            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar cobros recurrentes");
            resultado.Errores.Add($"Error general: {ex.Message}");
            return resultado;
        }
    }

    /// <summary>
    /// Renueva manualmente una suscripción específica
    /// </summary>
    public async Task<(bool Success, string Message)> RenovarSuscripcionAsync(int suscripcionId)
    {
        try
        {
            var suscripcion = await _context.Suscripciones
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(s => s.Id == suscripcionId);

            if (suscripcion == null)
            {
                return (false, "Suscripción no encontrada");
            }

            if (suscripcion.Estado != EstadoSuscripcion.Activa && suscripcion.Estado != EstadoSuscripcion.Vencida)
            {
                return (false, "La suscripción no está en un estado renovable");
            }

            if (suscripcion.Plan == null)
            {
                return (false, "Plan no encontrado");
            }

            // Si es gratuito, solo extender fecha
            if (suscripcion.Plan.EsGratuito)
            {
                var nuevaFecha = suscripcion.FechaVencimiento > DateTime.UtcNow
                    ? suscripcion.FechaVencimiento
                    : DateTime.UtcNow;

                suscripcion.FechaVencimiento = CalcularFechaVencimiento(nuevaFecha, suscripcion.Plan.Duracion);
                suscripcion.Estado = EstadoSuscripcion.Activa;
                suscripcion.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return (true, "Suscripción renovada exitosamente");
            }

            // Realizar cobro
            var resultado = await _transbankGateway.CobrarSuscripcionAsync(suscripcionId, suscripcion.Plan.Precio);

            if (resultado.Success)
            {
                var nuevaFecha = suscripcion.FechaVencimiento > DateTime.UtcNow
                    ? suscripcion.FechaVencimiento
                    : DateTime.UtcNow;

                suscripcion.FechaVencimiento = CalcularFechaVencimiento(nuevaFecha, suscripcion.Plan.Duracion);
                suscripcion.Estado = EstadoSuscripcion.Activa;
                suscripcion.MontoPagado = suscripcion.Plan.Precio;
                suscripcion.TransaccionId = resultado.AuthorizationCode;
                suscripcion.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return (true, "Suscripción renovada y cobro realizado exitosamente");
            }
            else
            {
                return (false, $"Error al procesar el pago: {resultado.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al renovar suscripción {SuscripcionId}", suscripcionId);
            return (false, $"Error interno: {ex.Message}");
        }
    }

    /// <summary>
    /// Actualiza el estado de suscripciones vencidas
    /// </summary>
    public async Task ActualizarEstadoSuscripcionesVencidasAsync()
    {
        try
        {
            _logger.LogInformation("Actualizando estado de suscripciones vencidas");

            var suscripcionesVencidas = await _context.Suscripciones
                .Where(s =>
                    s.Estado == EstadoSuscripcion.Activa &&
                    s.FechaVencimiento < DateTime.UtcNow)
                .ToListAsync();

            foreach (var suscripcion in suscripcionesVencidas)
            {
                suscripcion.Estado = EstadoSuscripcion.Vencida;
                suscripcion.UpdatedAt = DateTime.UtcNow;
            }

            if (suscripcionesVencidas.Any())
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Se actualizaron {Count} suscripciones a estado vencido",
                    suscripcionesVencidas.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar suscripciones vencidas");
        }
    }

    /// <summary>
    /// Reintenta cobros fallidos
    /// </summary>
    public async Task<BillingBatchResult> ReintentarCobrosFallidosAsync()
    {
        var resultado = new BillingBatchResult();

        try
        {
            _logger.LogInformation("Reintentando cobros fallidos");

            // Obtener suscripciones suspendidas que tienen renovación automática
            var suscripcionesSuspendidas = await _context.Suscripciones
                .Include(s => s.Plan)
                .Where(s =>
                    s.Estado == EstadoSuscripcion.Suspendida &&
                    s.RenovacionAutomatica &&
                    s.FechaVencimiento >= DateTime.UtcNow.AddDays(-30)) // Dentro de los últimos 30 días
                .ToListAsync();

            resultado.TotalProcesados = suscripcionesSuspendidas.Count;

            foreach (var suscripcion in suscripcionesSuspendidas)
            {
                try
                {
                    if (suscripcion.Plan == null || suscripcion.Plan.EsGratuito)
                    {
                        continue;
                    }

                    var resultadoCobro = await _transbankGateway.CobrarSuscripcionAsync(
                        suscripcion.Id,
                        suscripcion.Plan.Precio);

                    if (resultadoCobro.Success)
                    {
                        suscripcion.Estado = EstadoSuscripcion.Activa;
                        suscripcion.FechaVencimiento = CalcularFechaVencimiento(
                            DateTime.UtcNow,
                            suscripcion.Plan.Duracion);
                        suscripcion.MontoPagado = suscripcion.Plan.Precio;
                        suscripcion.TransaccionId = resultadoCobro.AuthorizationCode;
                        suscripcion.UpdatedAt = DateTime.UtcNow;

                        await _context.SaveChangesAsync();

                        resultado.Exitosos++;
                        resultado.MontoTotalCobrado += suscripcion.Plan.Precio;

                        _logger.LogInformation("Cobro recuperado para suscripción {Id}", suscripcion.Id);
                    }
                    else
                    {
                        resultado.Fallidos++;
                        resultado.Errores.Add($"Suscripción {suscripcion.Id}: {resultadoCobro.ErrorMessage}");
                    }
                }
                catch (Exception ex)
                {
                    resultado.Fallidos++;
                    resultado.Errores.Add($"Suscripción {suscripcion.Id}: {ex.Message}");
                    _logger.LogError(ex, "Error al reintentar cobro de suscripción {Id}", suscripcion.Id);
                }
            }

            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al reintentar cobros fallidos");
            resultado.Errores.Add($"Error general: {ex.Message}");
            return resultado;
        }
    }

    // ==================== MÉTODOS AUXILIARES ====================

    private DateTime CalcularFechaVencimiento(DateTime fechaInicio, TipoDuracion duracion)
    {
        return duracion switch
        {
            TipoDuracion.Mensual => fechaInicio.AddMonths(1),
            TipoDuracion.Anual => fechaInicio.AddYears(1),
            _ => fechaInicio.AddMonths(1)
        };
    }
}
