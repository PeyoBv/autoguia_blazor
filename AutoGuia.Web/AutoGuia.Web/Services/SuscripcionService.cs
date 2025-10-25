using AutoGuia.Core.Entities;
using AutoGuia.Web.Data;
using AutoGuia.Web.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoGuia.Web.Services;

/// <summary>
/// Implementación del servicio de gestión de suscripciones
/// </summary>
public class SuscripcionService : ISuscripcionService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SuscripcionService> _logger;

    public SuscripcionService(ApplicationDbContext context, ILogger<SuscripcionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    #region Métodos de mapeo

    /// <summary>
    /// Mapea una entidad Plan a PlanDto
    /// </summary>
    private static PlanDto MapearPlanADto(Plan plan)
    {
        return new PlanDto
        {
            Id = plan.Id,
            Nombre = plan.Nombre,
            Descripcion = plan.Descripcion,
            Precio = plan.Precio,
            Duracion = plan.Duracion == TipoDuracion.Mensual ? "Mensual" : "Anual",
            LimiteDiagnosticos = plan.LimiteDiagnosticos,
            LimiteBusquedas = plan.LimiteBusquedas,
            Destacado = plan.Destacado,
            EsActivo = plan.Activo,
            Caracteristicas = plan.Caracteristicas?.ToList() ?? new List<string>()
        };
    }

    /// <summary>
    /// Mapea una entidad Suscripcion a SuscripcionDto
    /// </summary>
    private static SuscripcionDto MapearSuscripcionADto(Suscripcion suscripcion)
    {
        return new SuscripcionDto
        {
            Id = suscripcion.Id,
            UsuarioId = suscripcion.UsuarioId,
            PlanId = suscripcion.PlanId,
            Plan = suscripcion.Plan != null ? MapearPlanADto(suscripcion.Plan) : null,
            FechaInicio = suscripcion.FechaInicio,
            FechaFin = suscripcion.FechaVencimiento,
            FechaCancelacion = suscripcion.FechaCancelacion,
            EsActiva = suscripcion.Estado == EstadoSuscripcion.Activa,
            DiagnosticosUsados = suscripcion.DiagnosticosUtilizados,
            BusquedasUtilizadas = suscripcion.BusquedasUtilizadas,
            UltimoReseteo = suscripcion.UltimoReseteo,
            UpdatedAt = suscripcion.UpdatedAt
        };
    }

    #endregion

    /// <inheritdoc/>
    public async Task<SuscripcionDto?> ObtenerSuscripcionActualAsync(string usuarioId)
    {
        try
        {
            var suscripcion = await _context.Suscripciones
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(s =>
                    s.UsuarioId == usuarioId &&
                    s.Estado == EstadoSuscripcion.Activa &&
                    s.FechaVencimiento > DateTime.UtcNow);

            return suscripcion != null ? MapearSuscripcionADto(suscripcion) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener suscripción actual del usuario {UsuarioId}", usuarioId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<SuscripcionDto>> ObtenerHistorialAsync(string usuarioId)
    {
        try
        {
            var suscripciones = await _context.Suscripciones
                .Include(s => s.Plan)
                .Where(s => s.UsuarioId == usuarioId)
                .OrderByDescending(s => s.FechaInicio)
                .ToListAsync();

            return suscripciones.Select(MapearSuscripcionADto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener historial de suscripciones del usuario {UsuarioId}", usuarioId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<PlanDto>> ObtenerPlanesAsync()
    {
        try
        {
            var planes = await _context.Planes
                .Where(p => p.Activo)
                .OrderBy(p => p.Orden)
                .ToListAsync();

            return planes.Select(MapearPlanADto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener planes disponibles");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<SuscripcionDto> CambiarPlanAsync(string usuarioId, int nuevoPlanId, string? metodoPago = null, string? transaccionId = null)
    {
        try
        {
            _logger.LogInformation("Cambiando plan del usuario {UsuarioId} al plan {PlanId}", usuarioId, nuevoPlanId);

            // Verificar que el nuevo plan existe y está activo
            var nuevoPlan = await _context.Planes.FindAsync(nuevoPlanId);
            if (nuevoPlan == null || !nuevoPlan.Activo)
            {
                throw new InvalidOperationException($"El plan {nuevoPlanId} no existe o no está disponible");
            }

            // Cancelar suscripción actual si existe
            var suscripcionActualDto = await ObtenerSuscripcionActualAsync(usuarioId);
            if (suscripcionActualDto != null)
            {
                // Cargar la entidad desde la base de datos
                var suscripcionEntidad = await _context.Suscripciones.FindAsync(suscripcionActualDto.Id);
                if (suscripcionEntidad != null)
                {
                    suscripcionEntidad.Estado = EstadoSuscripcion.Cancelada;
                    suscripcionEntidad.FechaCancelacion = DateTime.UtcNow;
                    suscripcionEntidad.MotivoCancelacion = $"Cambio a plan {nuevoPlan.Nombre}";
                    suscripcionEntidad.UpdatedAt = DateTime.UtcNow;
                }
            }

            // Crear nueva suscripción
            var nuevaSuscripcion = await CrearSuscripcionAsync(usuarioId, nuevoPlanId, metodoPago, transaccionId, nuevoPlan.Precio);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Plan cambiado exitosamente para usuario {UsuarioId}", usuarioId);

            return nuevaSuscripcion;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar plan del usuario {UsuarioId}", usuarioId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> CancelarSuscripcionAsync(string usuarioId, string? motivo = null)
    {
        try
        {
            var suscripcionDto = await ObtenerSuscripcionActualAsync(usuarioId);
            if (suscripcionDto == null)
            {
                _logger.LogWarning("No se encontró suscripción activa para cancelar del usuario {UsuarioId}", usuarioId);
                return false;
            }

            // Cargar entidad desde BD
            var suscripcion = await _context.Suscripciones.FindAsync(suscripcionDto.Id);
            if (suscripcion == null)
            {
                return false;
            }

            suscripcion.Estado = EstadoSuscripcion.Cancelada;
            suscripcion.FechaCancelacion = DateTime.UtcNow;
            suscripcion.MotivoCancelacion = motivo ?? "Cancelación solicitada por el usuario";
            suscripcion.RenovacionAutomatica = false;
            suscripcion.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Suscripción cancelada exitosamente para usuario {UsuarioId}", usuarioId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cancelar suscripción del usuario {UsuarioId}", usuarioId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<SuscripcionDto> CrearSuscripcionAsync(string usuarioId, int planId, string? metodoPago = null, string? transaccionId = null, decimal? montoPagado = null)
    {
        try
        {
            var plan = await _context.Planes.FindAsync(planId);
            if (plan == null)
            {
                throw new InvalidOperationException($"Plan {planId} no encontrado");
            }

            var fechaInicio = DateTime.UtcNow;
            var fechaVencimiento = plan.Duracion == TipoDuracion.Anual
                ? fechaInicio.AddYears(1)
                : fechaInicio.AddMonths(1);

            var suscripcion = new Suscripcion
            {
                UsuarioId = usuarioId,
                PlanId = planId,
                FechaInicio = fechaInicio,
                FechaVencimiento = fechaVencimiento,
                Estado = EstadoSuscripcion.Activa,
                MetodoPago = metodoPago,
                TransaccionId = transaccionId,
                MontoPagado = montoPagado ?? plan.Precio,
                RenovacionAutomatica = true,
                DiagnosticosUtilizados = 0,
                BusquedasUtilizadas = 0,
                UltimoReseteo = fechaInicio,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Suscripciones.Add(suscripcion);
            await _context.SaveChangesAsync();

            // Recargar con navegación y mapear a DTO
            await _context.Entry(suscripcion).Reference(s => s.Plan).LoadAsync();

            _logger.LogInformation("Suscripción creada exitosamente para usuario {UsuarioId} con plan {PlanId}", usuarioId, planId);

            return MapearSuscripcionADto(suscripcion);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear suscripción para usuario {UsuarioId}", usuarioId);
            throw;
        }
    }

    /// <inheritdoc/>
    public bool ValidarVigencia(SuscripcionDto suscripcion)
    {
        return suscripcion.EsVigente;
    }

    /// <inheritdoc/>
    public async Task<bool> PuedeUsarDiagnosticoAsync(string usuarioId)
    {
        try
        {
            var suscripcion = await ObtenerSuscripcionActualAsync(usuarioId);
            if (suscripcion == null || !ValidarVigencia(suscripcion))
            {
                return false;
            }

            if (suscripcion.Plan == null)
            {
                return false;
            }

            // Si el plan tiene límite 0, es ilimitado
            if (suscripcion.Plan.LimiteDiagnosticos == 0)
            {
                return true;
            }

            return suscripcion.DiagnosticosUsados < suscripcion.Plan.LimiteDiagnosticos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar si puede usar diagnóstico el usuario {UsuarioId}", usuarioId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> PuedeUsarBusquedaAsync(string usuarioId)
    {
        try
        {
            var suscripcion = await ObtenerSuscripcionActualAsync(usuarioId);
            if (suscripcion == null || !ValidarVigencia(suscripcion))
            {
                return false;
            }

            if (suscripcion.Plan == null)
            {
                return false;
            }

            // Si el plan tiene límite 0, es ilimitado
            if (suscripcion.Plan.LimiteBusquedas == 0)
            {
                return true;
            }

            return suscripcion.BusquedasUtilizadas < suscripcion.Plan.LimiteBusquedas;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar si puede usar búsqueda el usuario {UsuarioId}", usuarioId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task IncrementarUsoDiagnosticoAsync(string usuarioId)
    {
        try
        {
            var suscripcionDto = await ObtenerSuscripcionActualAsync(usuarioId);
            if (suscripcionDto != null && ValidarVigencia(suscripcionDto))
            {
                // Cargar entidad desde BD
                var suscripcion = await _context.Suscripciones.FindAsync(suscripcionDto.Id);
                if (suscripcion != null)
                {
                    suscripcion.DiagnosticosUtilizados++;
                    suscripcion.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    _logger.LogDebug("Incrementado uso de diagnóstico para usuario {UsuarioId}. Total: {Total}", 
                        usuarioId, suscripcion.DiagnosticosUtilizados);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al incrementar uso de diagnóstico del usuario {UsuarioId}", usuarioId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task IncrementarUsoBusquedaAsync(string usuarioId)
    {
        try
        {
            var suscripcionDto = await ObtenerSuscripcionActualAsync(usuarioId);
            if (suscripcionDto != null && ValidarVigencia(suscripcionDto))
            {
                // Cargar entidad desde BD
                var suscripcion = await _context.Suscripciones.FindAsync(suscripcionDto.Id);
                if (suscripcion != null)
                {
                    suscripcion.BusquedasUtilizadas++;
                    suscripcion.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    _logger.LogDebug("Incrementado uso de búsqueda para usuario {UsuarioId}. Total: {Total}", 
                        usuarioId, suscripcion.BusquedasUtilizadas);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al incrementar uso de búsqueda del usuario {UsuarioId}", usuarioId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task ResetearContadoresAsync(string usuarioId)
    {
        try
        {
            var suscripcionDto = await ObtenerSuscripcionActualAsync(usuarioId);
            if (suscripcionDto == null || !ValidarVigencia(suscripcionDto))
            {
                return;
            }

            if (!suscripcionDto.UltimoReseteo.HasValue)
            {
                return;
            }

            var ahora = DateTime.UtcNow;
            var necesitaReseteo = false;

            // Resetear diagnósticos mensuales
            if (suscripcionDto.UltimoReseteo.Value.AddMonths(1) <= ahora)
            {
                var suscripcion = await _context.Suscripciones.FindAsync(suscripcionDto.Id);
                if (suscripcion != null)
                {
                    suscripcion.DiagnosticosUtilizados = 0;
                    necesitaReseteo = true;
                    _logger.LogInformation("Reseteando contador de diagnósticos para usuario {UsuarioId}", usuarioId);
                }
            }

            // Resetear búsquedas diarias
            if (suscripcionDto.UltimoReseteo.Value.Date < ahora.Date)
            {
                var suscripcion = await _context.Suscripciones.FindAsync(suscripcionDto.Id);
                if (suscripcion != null)
                {
                    suscripcion.BusquedasUtilizadas = 0;
                    necesitaReseteo = true;
                    _logger.LogInformation("Reseteando contador de búsquedas para usuario {UsuarioId}", usuarioId);
                }
            }

            if (necesitaReseteo)
            {
                var suscripcion = await _context.Suscripciones.FindAsync(suscripcionDto.Id);
                if (suscripcion != null)
                {
                    suscripcion.UltimoReseteo = ahora;
                    suscripcion.UpdatedAt = ahora;
                    await _context.SaveChangesAsync();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al resetear contadores del usuario {UsuarioId}", usuarioId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Dictionary<string, object>> ObtenerEstadisticasUsoAsync(string usuarioId)
    {
        try
        {
            var suscripcion = await ObtenerSuscripcionActualAsync(usuarioId);
            var estadisticas = new Dictionary<string, object>();

            if (suscripcion == null || suscripcion.Plan == null)
            {
                estadisticas["TieneSuscripcion"] = false;
                return estadisticas;
            }

            var plan = suscripcion.Plan;

            estadisticas["TieneSuscripcion"] = true;
            estadisticas["PlanNombre"] = plan.Nombre;
            estadisticas["EsVigente"] = ValidarVigencia(suscripcion);
            estadisticas["DiasRestantes"] = suscripcion.DiasRestantes;

            // Diagnósticos
            estadisticas["DiagnosticosUtilizados"] = suscripcion.DiagnosticosUsados;
            estadisticas["DiagnosticosDisponibles"] = plan.LimiteDiagnosticos == 0 
                ? "Ilimitado" 
                : (plan.LimiteDiagnosticos - suscripcion.DiagnosticosUsados).ToString();
            estadisticas["DiagnosticosLimiteAlcanzado"] = suscripcion.DiagnosticosUsados >= plan.LimiteDiagnosticos && plan.LimiteDiagnosticos > 0;
            estadisticas["DiagnosticosPorcentajeUso"] = plan.LimiteDiagnosticos > 0 
                ? Math.Round((double)suscripcion.DiagnosticosUsados / plan.LimiteDiagnosticos * 100, 2)
                : 0;

            // Búsquedas
            estadisticas["BusquedasUtilizadas"] = suscripcion.BusquedasUtilizadas;
            estadisticas["BusquedasDisponibles"] = plan.LimiteBusquedas == 0 
                ? "Ilimitado" 
                : (plan.LimiteBusquedas - suscripcion.BusquedasUtilizadas).ToString();
            estadisticas["BusquedasLimiteAlcanzado"] = suscripcion.BusquedasUtilizadas >= plan.LimiteBusquedas && plan.LimiteBusquedas > 0;
            estadisticas["BusquedasPorcentajeUso"] = plan.LimiteBusquedas > 0 
                ? Math.Round((double)suscripcion.BusquedasUtilizadas / plan.LimiteBusquedas * 100, 2)
                : 0;

            return estadisticas;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estadísticas de uso del usuario {UsuarioId}", usuarioId);
            throw;
        }
    }
}
