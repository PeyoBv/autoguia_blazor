using AutoGuia.Core.Entities;
using AutoGuia.Scraper.Models;
using Microsoft.Extensions.Logging;

namespace AutoGuia.Scraper.Services;

/// <summary>
/// Servicio de prueba completo que demuestra la integración del scraping con la actualización de la base de datos.
/// Combina el scraping de productos con la persistencia de ofertas.
/// </summary>
public class IntegratedScrapingTestService
{
    private readonly IScraperService _scraperService;
    private readonly IOfertaUpdateService _ofertaUpdateService;
    private readonly ScraperDataSeederService _dataSeederService;
    private readonly ILogger<IntegratedScrapingTestService> _logger;

    public IntegratedScrapingTestService(
        IScraperService scraperService,
        IOfertaUpdateService ofertaUpdateService,
        ScraperDataSeederService dataSeederService,
        ILogger<IntegratedScrapingTestService> logger)
    {
        _scraperService = scraperService ?? throw new ArgumentNullException(nameof(scraperService));
        _ofertaUpdateService = ofertaUpdateService ?? throw new ArgumentNullException(nameof(ofertaUpdateService));
        _dataSeederService = dataSeederService ?? throw new ArgumentNullException(nameof(dataSeederService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Ejecuta una prueba completa del sistema de scraping con actualización de base de datos.
    /// </summary>
    public async Task<TestResult> EjecutarPruebaCompleta(CancellationToken cancellationToken = default)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        _logger.LogInformation("🚀 Iniciando prueba completa del sistema de scraping integrado...");

        var resultado = new TestResult();

        try
        {
            // 1. Inicializar datos semilla
            await _dataSeederService.InicializarDatosSemilla();
            resultado.DatosInicializados = true;

            // 2. Obtener tienda y productos para probar
            var tienda = await _dataSeederService.ObtenerTiendaRepuestosTotal();
            if (tienda == null)
            {
                resultado.MensajeError = "No se encontró la tienda RepuestosTotal";
                return resultado;
            }
            resultado.TiendaEncontrada = true;

            var productos = await _dataSeederService.ObtenerProductosParaScrapear(3);
            if (!productos.Any())
            {
                resultado.MensajeError = "No se encontraron productos para probar";
                return resultado;
            }
            resultado.ProductosEncontrados = productos.Count;

            // 3. Verificar disponibilidad del scraper
            var scraperDisponible = await _scraperService.VerificarDisponibilidad(cancellationToken);
            resultado.ScraperDisponible = scraperDisponible;

            // 4. Ejecutar scraping y actualización para cada producto
            foreach (var producto in productos)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                _logger.LogInformation("🔍 Probando producto: {ProductoNombre} ({NumeroParte})",
                    producto.Nombre, producto.NumeroDeParte);

                try
                {
                    // Scrapear producto
                    var resultadoScraping = await _scraperService.ScrapearProducto(producto, cancellationToken);
                    resultado.IntentosDeScrapingTotal++;

                    if (resultadoScraping.Exitoso)
                    {
                        resultado.ScrapingExitosos++;
                        _logger.LogInformation("✅ Scraping exitoso - Precio: ${Precio}", resultadoScraping.Precio);

                        // Actualizar oferta en base de datos
                        var actualizacionExitosa = await _ofertaUpdateService.ActualizarOferta(
                            producto, tienda, resultadoScraping, cancellationToken);

                        if (actualizacionExitosa)
                        {
                            resultado.OfertasActualizadas++;
                            _logger.LogInformation("💾 Oferta actualizada exitosamente");
                        }
                        else
                        {
                            resultado.ErroresDeActualizacion++;
                            _logger.LogWarning("⚠️ Error actualizando oferta");
                        }
                    }
                    else
                    {
                        resultado.ScrapingFallidos++;
                        _logger.LogWarning("❌ Scraping falló: {Error}", resultadoScraping.MensajeError);

                        // Marcar oferta como no disponible
                        await _ofertaUpdateService.MarcarOfertaComoNoDisponible(
                            producto.Id, tienda.Id, cancellationToken);
                    }

                    // Pequeña pausa entre scraping para ser respetuosos
                    await Task.Delay(1000, cancellationToken);
                }
                catch (Exception ex)
                {
                    resultado.ErroresGenerales++;
                    _logger.LogError(ex, "❌ Error procesando producto {ProductoId}", producto.Id);
                }
            }

            // 5. Obtener estadísticas finales
            var estadisticas = await _ofertaUpdateService.ObtenerEstadisticas(tienda.Id, cancellationToken);
            resultado.EstadisticasFinales = estadisticas;

            stopwatch.Stop();
            resultado.TiempoTranscurrido = stopwatch.Elapsed;
            resultado.Exitoso = true;

            _logger.LogInformation("✅ Prueba completa finalizada exitosamente en {TiempoMs}ms. " +
                "Scraping exitosos: {Exitosos}/{Total}, Ofertas actualizadas: {OfertasActualizadas}",
                stopwatch.ElapsedMilliseconds, resultado.ScrapingExitosos, resultado.IntentosDeScrapingTotal,
                resultado.OfertasActualizadas);

            return resultado;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            resultado.MensajeError = ex.Message;
            _logger.LogError(ex, "❌ Error en la prueba completa del sistema");
            return resultado;
        }
    }

    /// <summary>
    /// Genera un reporte detallado del test.
    /// </summary>
    public string GenerarReporte(TestResult resultado)
    {
        var reporte = new System.Text.StringBuilder();
        
        reporte.AppendLine("=== REPORTE DE PRUEBA COMPLETA DEL SISTEMA DE SCRAPING ===");
        reporte.AppendLine($"Fecha: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        reporte.AppendLine($"Duración: {resultado.TiempoTranscurrido.TotalSeconds:F2} segundos");
        reporte.AppendLine($"Estado: {(resultado.Exitoso ? "✅ EXITOSO" : "❌ FALLIDO")}");
        reporte.AppendLine();
        
        reporte.AppendLine("--- INICIALIZACIÓN ---");
        reporte.AppendLine($"Datos semilla inicializados: {(resultado.DatosInicializados ? "✅" : "❌")}");
        reporte.AppendLine($"Tienda RepuestosTotal encontrada: {(resultado.TiendaEncontrada ? "✅" : "❌")}");
        reporte.AppendLine($"Productos encontrados: {resultado.ProductosEncontrados}");
        reporte.AppendLine($"Scraper disponible: {(resultado.ScraperDisponible ? "✅" : "❌")}");
        reporte.AppendLine();
        
        reporte.AppendLine("--- RESULTADOS DE SCRAPING ---");
        reporte.AppendLine($"Total de intentos: {resultado.IntentosDeScrapingTotal}");
        reporte.AppendLine($"Scraping exitosos: {resultado.ScrapingExitosos}");
        reporte.AppendLine($"Scraping fallidos: {resultado.ScrapingFallidos}");
        
        if (resultado.IntentosDeScrapingTotal > 0)
        {
            var tasa = (double)resultado.ScrapingExitosos / resultado.IntentosDeScrapingTotal * 100;
            reporte.AppendLine($"Tasa de éxito: {tasa:F1}%");
        }
        reporte.AppendLine();
        
        reporte.AppendLine("--- ACTUALIZACIÓN DE BASE DE DATOS ---");
        reporte.AppendLine($"Ofertas actualizadas: {resultado.OfertasActualizadas}");
        reporte.AppendLine($"Errores de actualización: {resultado.ErroresDeActualizacion}");
        reporte.AppendLine($"Errores generales: {resultado.ErroresGenerales}");
        reporte.AppendLine();
        
        if (resultado.EstadisticasFinales != null)
        {
            reporte.AppendLine("--- ESTADÍSTICAS DE LA BASE DE DATOS ---");
            reporte.AppendLine($"Total ofertas: {resultado.EstadisticasFinales.TotalOfertas}");
            reporte.AppendLine($"Ofertas activas: {resultado.EstadisticasFinales.OfertasActivas}");
            reporte.AppendLine($"Ofertas actualizadas hoy: {resultado.EstadisticasFinales.OfertasActualizadasHoy}");
            reporte.AppendLine($"Ofertas no disponibles: {resultado.EstadisticasFinales.OfertasNoDisponibles}");
            reporte.AppendLine($"Última actualización: {resultado.EstadisticasFinales.UltimaActualizacion:yyyy-MM-dd HH:mm:ss}");
        }
        
        if (!string.IsNullOrEmpty(resultado.MensajeError))
        {
            reporte.AppendLine();
            reporte.AppendLine("--- ERRORES ---");
            reporte.AppendLine(resultado.MensajeError);
        }
        
        reporte.AppendLine();
        reporte.AppendLine("=== FIN DEL REPORTE ===");
        
        return reporte.ToString();
    }
}

/// <summary>
/// Resultado de la prueba completa del sistema.
/// </summary>
public class TestResult
{
    public bool Exitoso { get; set; } = false;
    public string MensajeError { get; set; } = string.Empty;
    public TimeSpan TiempoTranscurrido { get; set; }
    
    // Inicialización
    public bool DatosInicializados { get; set; } = false;
    public bool TiendaEncontrada { get; set; } = false;
    public int ProductosEncontrados { get; set; } = 0;
    public bool ScraperDisponible { get; set; } = false;
    
    // Scraping
    public int IntentosDeScrapingTotal { get; set; } = 0;
    public int ScrapingExitosos { get; set; } = 0;
    public int ScrapingFallidos { get; set; } = 0;
    
    // Base de datos
    public int OfertasActualizadas { get; set; } = 0;
    public int ErroresDeActualizacion { get; set; } = 0;
    public int ErroresGenerales { get; set; } = 0;
    
    // Estadísticas
    public OfertaStats? EstadisticasFinales { get; set; }
}