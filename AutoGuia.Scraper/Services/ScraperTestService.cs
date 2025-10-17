using AutoGuia.Core.Entities;
using AutoGuia.Scraper.Models;
using AutoGuia.Scraper.Services;
using Microsoft.Extensions.Logging;

namespace AutoGuia.Scraper.Services;

/// <summary>
/// Servicio de prueba para validar el funcionamiento del scraper.
/// </summary>
public class ScraperTestService
{
    private readonly ILogger<ScraperTestService> _logger;
    private readonly IScraperService _scraperService;

    public ScraperTestService(
        ILogger<ScraperTestService> logger,
        IScraperService scraperService)
    {
        _logger = logger;
        _scraperService = scraperService;
    }

    /// <summary>
    /// Ejecuta pruebas del scraper con productos de ejemplo.
    /// </summary>
    public async Task<List<ScrapeResult>> EjecutarPruebas(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("🧪 Iniciando pruebas del scraper para {TiendaNombre}", _scraperService.TiendaNombre);

        var productosDeEjemplo = CrearProductosDeEjemplo();
        var resultados = new List<ScrapeResult>();

        foreach (var producto in productosDeEjemplo)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            _logger.LogInformation("🔍 Probando scraping para: {ProductoNombre} ({NumeroParte})", 
                producto.Nombre, producto.NumeroDeParte);

            try
            {
                var resultado = await _scraperService.ScrapearProducto(producto, cancellationToken);
                resultados.Add(resultado);

                if (resultado.Exitoso)
                {
                    _logger.LogInformation("✅ Prueba exitosa - Precio: ${Precio} - URL: {Url}", 
                        resultado.Precio, resultado.UrlProducto);
                }
                else
                {
                    _logger.LogWarning("⚠️ Prueba fallida - Error: {Error}", resultado.MensajeError);
                }

                // Delay entre pruebas para no sobrecargar la tienda
                await Task.Delay(2000, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error durante prueba del producto {ProductoId}", producto.Id);
                resultados.Add(ScrapeResult.CrearFallido($"Error de prueba: {ex.Message}"));
            }
        }

        // Generar reporte de pruebas
        GenerarReportePruebas(resultados);

        return resultados;
    }

    /// <summary>
    /// Verifica la disponibilidad de la tienda.
    /// </summary>
    public async Task<bool> VerificarDisponibilidadTienda(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("🌐 Verificando disponibilidad de {TiendaNombre}", _scraperService.TiendaNombre);

        try
        {
            var disponible = await _scraperService.VerificarDisponibilidad(cancellationToken);
            
            if (disponible)
            {
                _logger.LogInformation("✅ Tienda {TiendaNombre} está disponible", _scraperService.TiendaNombre);
                
                // Mostrar información del scraper
                var info = _scraperService.ObtenerInformacion();
                _logger.LogInformation("📋 Información del scraper - Versión: {Version}, Delay: {Delay}ms", 
                    info.Version, info.DelayEntreRequests);
            }
            else
            {
                _logger.LogWarning("⚠️ Tienda {TiendaNombre} no está disponible", _scraperService.TiendaNombre);
            }

            return disponible;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error verificando disponibilidad de {TiendaNombre}", _scraperService.TiendaNombre);
            return false;
        }
    }

    /// <summary>
    /// Crea productos de ejemplo para las pruebas.
    /// </summary>
    private static List<Producto> CrearProductosDeEjemplo()
    {
        return new List<Producto>
        {
            new Producto
            {
                Id = 1,
                Nombre = "Filtro de Aceite Bosch",
                NumeroDeParte = "P3274",
                Descripcion = "Filtro de aceite original Bosch para motores Toyota"
            },
            new Producto
            {
                Id = 2,
                Nombre = "Pastillas de Freno Brembo",
                NumeroDeParte = "P85075",
                Descripcion = "Pastillas de freno delanteras Brembo"
            },
            new Producto
            {
                Id = 3,
                Nombre = "Aceite Motor 5W30",
                NumeroDeParte = "5W30-4L",
                Descripcion = "Aceite sintético para motor 5W30 - 4 litros"
            }
        };
    }

    /// <summary>
    /// Genera un reporte consolidado de las pruebas ejecutadas.
    /// </summary>
    private void GenerarReportePruebas(List<ScrapeResult> resultados)
    {
        var exitosos = resultados.Count(r => r.Exitoso);
        var fallidos = resultados.Count - exitosos;
        var porcentajeExito = resultados.Count > 0 ? (exitosos * 100.0 / resultados.Count) : 0;

        _logger.LogInformation("📊 Reporte de Pruebas del Scraper:");
        _logger.LogInformation("   🔢 Total de pruebas: {Total}", resultados.Count);
        _logger.LogInformation("   ✅ Pruebas exitosas: {Exitosos}", exitosos);
        _logger.LogInformation("   ❌ Pruebas fallidas: {Fallidos}", fallidos);
        _logger.LogInformation("   📈 Porcentaje de éxito: {Porcentaje:F1}%", porcentajeExito);

        if (exitosos > 0)
        {
            var precioPromedio = resultados.Where(r => r.Exitoso).Average(r => r.Precio);
            _logger.LogInformation("   💰 Precio promedio encontrado: ${PrecioPromedio:F0}", precioPromedio);
        }

        if (fallidos > 0)
        {
            _logger.LogWarning("⚠️ Errores encontrados durante las pruebas:");
            var erroresAgrupados = resultados
                .Where(r => !r.Exitoso)
                .GroupBy(r => r.MensajeError)
                .OrderByDescending(g => g.Count());

            foreach (var grupo in erroresAgrupados)
            {
                _logger.LogWarning("   📋 '{Error}': {Cantidad} ocurrencias", grupo.Key, grupo.Count());
            }
        }
    }
}