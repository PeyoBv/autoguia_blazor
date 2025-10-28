using AutoGuia.Infrastructure.Services.Payments;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AutoGuia.Web.Services;

/// <summary>
/// Servicio en segundo plano que procesa cobros recurrentes de suscripciones
/// Se ejecuta diariamente a las 2:00 AM
/// </summary>
public class SubscriptionBillingBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SubscriptionBillingBackgroundService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1); // Verificar cada hora
    private DateTime _lastExecutionDate = DateTime.MinValue;

    public SubscriptionBillingBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<SubscriptionBillingBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🚀 Servicio de facturación de suscripciones iniciado");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var now = DateTime.Now;

                // Ejecutar solo una vez al día a las 2:00 AM (±1 hora de margen)
                if (now.Hour == 2 && _lastExecutionDate.Date != now.Date)
                {
                    _logger.LogInformation("⏰ Iniciando proceso de facturación diario");
                    await EjecutarProcesoFacturacionAsync();
                    _lastExecutionDate = now;
                }

                // Esperar antes de la próxima verificación
                await Task.Delay(_checkInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en el servicio de facturación de suscripciones");
                
                // Esperar un poco más en caso de error para no saturar
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        _logger.LogInformation("🛑 Servicio de facturación de suscripciones detenido");
    }

    private async Task EjecutarProcesoFacturacionAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var billingService = scope.ServiceProvider.GetRequiredService<ISubscriptionBillingService>();

        try
        {
            _logger.LogInformation("📋 Paso 1: Actualizando estado de suscripciones vencidas");
            await billingService.ActualizarEstadoSuscripcionesVencidasAsync();

            _logger.LogInformation("💳 Paso 2: Procesando cobros recurrentes");
            var resultadoCobros = await billingService.ProcesarCobrosRecurrentesAsync();

            _logger.LogInformation(
                "✅ Cobros recurrentes completados: {Total} procesados, {Exitosos} exitosos, {Fallidos} fallidos, ${Monto} CLP cobrados",
                resultadoCobros.TotalProcesados,
                resultadoCobros.Exitosos,
                resultadoCobros.Fallidos,
                resultadoCobros.MontoTotalCobrado);

            if (resultadoCobros.Errores.Any())
            {
                _logger.LogWarning("⚠️ Errores en cobros recurrentes: {Errores}",
                    string.Join(", ", resultadoCobros.Errores));
            }

            // Reintentar cobros fallidos anteriores (solo los primeros días del mes)
            if (DateTime.Now.Day <= 5)
            {
                _logger.LogInformation("🔄 Paso 3: Reintentando cobros fallidos");
                var resultadoReintentos = await billingService.ReintentarCobrosFallidosAsync();

                _logger.LogInformation(
                    "✅ Reintentos completados: {Total} procesados, {Exitosos} exitosos, {Fallidos} fallidos",
                    resultadoReintentos.TotalProcesados,
                    resultadoReintentos.Exitosos,
                    resultadoReintentos.Fallidos);

                if (resultadoReintentos.Errores.Any())
                {
                    _logger.LogWarning("⚠️ Errores en reintentos: {Errores}",
                        string.Join(", ", resultadoReintentos.Errores));
                }
            }

            _logger.LogInformation("🎉 Proceso de facturación diario completado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al ejecutar proceso de facturación");
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("⏹️ Deteniendo servicio de facturación de suscripciones...");
        await base.StopAsync(stoppingToken);
    }
}
