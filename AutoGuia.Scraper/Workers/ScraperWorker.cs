using AutoGuia.Core.Entities;
using AutoGuia.Infrastructure.Data;
using AutoGuia.Scraper.Models;
using AutoGuia.Scraper.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AutoGuia.Scraper.Workers;

/// <summary>
/// Servicio en segundo plano que ejecuta el proceso de scraping de manera periódica.
/// Orquesta todo el flujo: obtiene productos, ejecuta scraping y actualiza la base de datos.
/// Implementa rate limiting y manejo robusto de errores según especificaciones del Paso 5.
/// </summary>
public class ScraperWorker : BackgroundService
{
    private readonly ILogger<ScraperWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ScrapingSettings _scrapingSettings;
    private readonly StoreSettings _storeSettings;

    public ScraperWorker(
        ILogger<ScraperWorker> logger,
        IServiceScopeFactory scopeFactory,
        IOptions<ScrapingSettings> scrapingSettings,
        IOptions<StoreSettings> storeSettings)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        _scrapingSettings = scrapingSettings?.Value ?? throw new ArgumentNullException(nameof(scrapingSettings));
        _storeSettings = storeSettings?.Value ?? throw new ArgumentNullException(nameof(storeSettings));
    }

    /// <summary>
    /// Método principal del servicio en segundo plano.
    /// Se ejecuta continuamente hasta que se cancele la aplicación.
    /// Implementa el corazón del scraper con manejo robusto de errores.
    /// </summary>
    /// <param name="stoppingToken">Token de cancelación</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🚀 ScraperWorker iniciado - Intervalo configurado: {IntervalMinutes} minutos", 
            _scrapingSettings.IntervalInMinutes);

        // Verificar si el scraping está habilitado globalmente
        if (!_scrapingSettings.IsEnabled)
        {
            _logger.LogWarning("⚠️ Scraping deshabilitado globalmente en configuración");
            return;
        }

        // Inicialización única
        await InicializarDatosSiEsNecesario(stoppingToken);

        // Ciclo principal del worker
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var cicloInicio = DateTime.UtcNow;
                _logger.LogInformation("🕷️ Iniciando ciclo de scraping - {FechaHora}", cicloInicio);

                // Ejecutar un ciclo completo de scraping
                var resultados = await EjecutarCicloCompleto(stoppingToken);

                var cicloDuracion = DateTime.UtcNow - cicloInicio;
                _logger.LogInformation("✅ Ciclo de scraping completado en {Duracion} - " +
                    "Productos procesados: {TotalProductos}, Exitosos: {Exitosos}, Fallidos: {Fallidos}",
                    cicloDuracion, resultados.Total, resultados.Exitosos, resultados.Fallidos);

            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("🛑 Scraping cancelado por solicitud del usuario");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error no controlado en el ciclo de scraping");
            }

            // Pausa larga entre ciclos completos (configurada en appsettings.json)
            var intervaloPausa = TimeSpan.FromMinutes(_scrapingSettings.IntervalInMinutes);
            var proximaEjecucion = DateTime.Now.Add(intervaloPausa);
            
            _logger.LogInformation("⏰ Próximo ciclo de scraping en {IntervalMinutos} minutos ({ProximaEjecucion})", 
                intervaloPausa.TotalMinutes, proximaEjecucion.ToString("yyyy-MM-dd HH:mm:ss"));

            await Task.Delay(intervaloPausa, stoppingToken);
        }
    }

    /// <summary>
    /// Inicializa datos semilla si es la primera ejecución.
    /// </summary>
    private async Task InicializarDatosSiEsNecesario(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var dataSeeder = scope.ServiceProvider.GetRequiredService<ScraperDataSeederService>();
            await dataSeeder.InicializarDatosSemilla();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error inicializando datos semilla");
        }
    }

    /// <summary>
    /// Ejecuta un ciclo completo de scraping: obtiene productos, ejecuta scraping y actualiza base de datos.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Resultados del ciclo</returns>
    private async Task<CicloResultado> EjecutarCicloCompleto(CancellationToken cancellationToken)
    {
        var resultado = new CicloResultado();
        
        // Crear scope para resolver servicios con DI (Importante: BackgroundService es Singleton)
        using var scope = _scopeFactory.CreateScope();
        
        try
        {
            // Obtener servicios del scope
            var scraperService = scope.ServiceProvider.GetRequiredService<IScraperService>();
            var ofertaUpdateService = scope.ServiceProvider.GetRequiredService<IOfertaUpdateService>();
            var dbContext = scope.ServiceProvider.GetRequiredService<AutoGuiaDbContext>();

            // Paso 1: Obtener o crear la tienda RepuestosTotal
            var tienda = await ObtenerOCrearTiendaRepuestosTotal(dbContext, cancellationToken);
            if (tienda == null)
            {
                _logger.LogError("❌ No se pudo obtener la tienda RepuestosTotal");
                return resultado;
            }

            _logger.LogInformation("🏪 Tienda RepuestosTotal encontrada/creada - ID: {TiendaId}", tienda.Id);

            // Paso 2: Obtener productos a scrapear
            var productos = await dbContext.Productos
                .Where(p => p.EsActivo)
                .ToListAsync(cancellationToken);

            if (!productos.Any())
            {
                _logger.LogWarning("⚠️ No se encontraron productos activos para scrapear");
                return resultado;
            }

            _logger.LogInformation("📦 Productos encontrados para scrapear: {CantidadProductos}", productos.Count);

            // Paso 3: Iterar y scrapear cada producto
            foreach (var producto in productos)
            {
                // Verificar cancelación antes de cada producto
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogInformation("🛑 Scraping cancelado durante procesamiento de productos");
                    break;
                }

                resultado.Total++;

                // Loggear inicio del scraping del producto
                _logger.LogInformation("🔍 Scrapeando {ProductoNombre} ({NumeroParte})...", 
                    producto.Nombre, producto.NumeroDeParte);

                try
                {
                    // Llamar al scraper
                    var resultadoScraping = await scraperService.ScrapearProducto(producto, cancellationToken);

                    // Procesar resultado
                    if (resultadoScraping.Exitoso)
                    {
                        // Actualizar oferta en la base de datos
                        var actualizado = await ofertaUpdateService.ActualizarOferta(
                            producto, tienda, resultadoScraping, cancellationToken);

                        if (actualizado)
                        {
                            resultado.Exitosos++;
                            _logger.LogInformation("✅ Éxito - Producto: {ProductoNombre}, Precio: ${Precio:F0}", 
                                producto.Nombre, resultadoScraping.Precio);
                        }
                        else
                        {
                            resultado.ErroresBD++;
                            _logger.LogWarning("⚠️ Scraping exitoso pero falló actualización BD - Producto: {ProductoNombre}", 
                                producto.Nombre);
                        }
                    }
                    else
                    {
                        resultado.Fallidos++;
                        _logger.LogWarning("❌ Fallo al scrapear {ProductoNombre}: {Error}", 
                            producto.Nombre, resultadoScraping.MensajeError);

                        // Marcar oferta como no disponible
                        await ofertaUpdateService.MarcarOfertaComoNoDisponible(
                            producto.Id, tienda.Id, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    resultado.Errores++;
                    _logger.LogError(ex, "❌ Error inesperado procesando producto {ProductoNombre} ({ProductoId})", 
                        producto.Nombre, producto.Id);
                }

                // IMPORTANTE: Rate Limiting - Pausa entre productos para no saturar el servidor
                var delaySegundos = 2; // 2 segundos por defecto
                _logger.LogDebug("⏸️ Pausa de {DelaySegundos}s antes del próximo producto (Rate Limiting)", delaySegundos);
                await Task.Delay(TimeSpan.FromSeconds(delaySegundos), cancellationToken);
            }

            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error durante el ciclo completo de scraping");
            resultado.Errores++;
            return resultado;
        }
    }

    /// <summary>
    /// Obtiene o crea la tienda RepuestosTotal en la base de datos.
    /// </summary>
    /// <param name="dbContext">Contexto de la base de datos</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>La tienda RepuestosTotal</returns>
    private async Task<Tienda?> ObtenerOCrearTiendaRepuestosTotal(AutoGuiaDbContext dbContext, CancellationToken cancellationToken)
    {
        try
        {
            // Buscar tienda existente
            var tiendaExistente = await dbContext.Tiendas
                .FirstOrDefaultAsync(t => t.Nombre == "RepuestosTotal" && t.EsActivo, cancellationToken);

            if (tiendaExistente != null)
            {
                _logger.LogDebug("🏪 Tienda RepuestosTotal encontrada - ID: {TiendaId}", tiendaExistente.Id);
                return tiendaExistente;
            }

            // Crear nueva tienda si no existe
            var nuevaTienda = new Tienda
            {
                Nombre = "RepuestosTotal",
                UrlSitioWeb = _storeSettings.RepuestosTotal.BaseUrl,
                Descripcion = "Tienda líder en repuestos automotrices en Chile",
                EsActivo = true,
                FechaCreacion = DateTime.UtcNow
            };

            dbContext.Tiendas.Add(nuevaTienda);
            await dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("🏪 Tienda RepuestosTotal creada exitosamente - ID: {TiendaId}", nuevaTienda.Id);
            return nuevaTienda;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error obteniendo/creando tienda RepuestosTotal");
            return null;
        }
    }

    /// <summary>
    /// Se ejecuta cuando el servicio se está deteniendo.
    /// </summary>
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("🛑 Deteniendo ScraperWorker...");
        await base.StopAsync(cancellationToken);
        _logger.LogInformation("✅ ScraperWorker detenido exitosamente");
    }
}

/// <summary>
/// Resultado de un ciclo completo de scraping.
/// </summary>
public class CicloResultado
{
    public int Total { get; set; } = 0;
    public int Exitosos { get; set; } = 0;
    public int Fallidos { get; set; } = 0;
    public int ErroresBD { get; set; } = 0;
    public int Errores { get; set; } = 0;
}