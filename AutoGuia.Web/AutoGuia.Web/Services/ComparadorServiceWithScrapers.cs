using AutoGuia.Core.DTOs;
using AutoGuia.Infrastructure.Services;
using AutoGuia.Scraper.Scrapers;
using Microsoft.Extensions.Logging;

namespace AutoGuia.Web.Services;

/// <summary>
/// Servicio que extiende ComparadorService con capacidad de scraping en tiempo real
/// </summary>
public class ComparadorServiceWithScrapers : IComparadorService
{
    private readonly ComparadorService _baseService;
    private readonly ILogger<ComparadorServiceWithScrapers> _logger;
    private readonly ConsumiblesScraperService? _mercadoLibreScraper;
    private readonly AutoplanetConsumiblesScraperService? _autoplanetScraper;
    private readonly MundoRepuestosConsumiblesScraperService? _mundoRepuestosScraper;

    public ComparadorServiceWithScrapers(
        ComparadorService baseService,
        ILogger<ComparadorServiceWithScrapers> logger,
        ConsumiblesScraperService? mercadoLibreScraper = null,
        AutoplanetConsumiblesScraperService? autoplanetScraper = null,
        MundoRepuestosConsumiblesScraperService? mundoRepuestosScraper = null)
    {
        _baseService = baseService;
        _logger = logger;
        _mercadoLibreScraper = mercadoLibreScraper;
        _autoplanetScraper = autoplanetScraper;
        _mundoRepuestosScraper = mundoRepuestosScraper;
    }

    /// <summary>
    /// Busca consumibles en tiempo real usando scrapers
    /// </summary>
    public async Task<IEnumerable<ProductoConOfertasDto>> BuscarConsumiblesAsync(string termino, string? categoria = null)
    {
        _logger.LogInformation("🔍 Iniciando búsqueda con scrapers: '{Termino}'", termino);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var todasLasOfertas = new List<OfertaDto>();

        try
        {
            var tareas = new List<Task<List<OfertaDto>>>();

            // Scraper 1: MercadoLibre
            if (_mercadoLibreScraper != null)
            {
                tareas.Add(Task.Run(async () =>
                {
                    try
                    {
                        _logger.LogInformation("⏳ Buscando en MercadoLibre...");
                        var ofertas = await _mercadoLibreScraper.BuscarConsumiblesAsync(termino, categoria);
                        _logger.LogInformation("✅ MercadoLibre: {Count} ofertas", ofertas.Count);
                        return ofertas;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "⚠️ Error en MercadoLibre");
                        return new List<OfertaDto>();
                    }
                }));
            }

            // Scraper 2: Autoplanet
            if (_autoplanetScraper != null)
            {
                tareas.Add(Task.Run(async () =>
                {
                    try
                    {
                        _logger.LogInformation("⏳ Buscando en Autoplanet...");
                        var ofertas = await _autoplanetScraper.BuscarConsumiblesAsync(termino, categoria);
                        _logger.LogInformation("✅ Autoplanet: {Count} ofertas", ofertas.Count);
                        return ofertas;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "⚠️ Error en Autoplanet");
                        return new List<OfertaDto>();
                    }
                }));
            }

            // Scraper 3: MundoRepuestos
            if (_mundoRepuestosScraper != null)
            {
                tareas.Add(Task.Run(async () =>
                {
                    try
                    {
                        _logger.LogInformation("⏳ Buscando en MundoRepuestos...");
                        var ofertas = await _mundoRepuestosScraper.BuscarConsumiblesAsync(termino, categoria);
                        _logger.LogInformation("✅ MundoRepuestos: {Count} ofertas", ofertas.Count);
                        return ofertas;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "⚠️ Error en MundoRepuestos");
                        return new List<OfertaDto>();
                    }
                }));
            }

            // Esperar a que todas las tareas terminen
            var resultadosScrapers = await Task.WhenAll(tareas);
            
            // Combinar todas las ofertas
            foreach (var listaOfertas in resultadosScrapers)
            {
                todasLasOfertas.AddRange(listaOfertas);
            }

            stopwatch.Stop();

            // Agrupar ofertas por producto (nombre + número de parte)
            var productosAgrupados = todasLasOfertas
                .GroupBy(o => new { 
                    Nombre = o.ProductoNombre ?? "Producto sin nombre", 
                    NumeroDeParte = o.SKU ?? "" 
                })
                .Select(g => new ProductoConOfertasDto
                {
                    Id = 0, // No tenemos ID en scraping
                    Nombre = g.Key.Nombre,
                    NumeroDeParte = g.Key.NumeroDeParte,
                    Descripcion = g.First().ProductoNombre,
                    ImagenUrl = g.First().ProductoImagen,
                    Ofertas = g.Select(o => new OfertaComparadorDto
                    {
                        Id = o.Id,
                        TiendaId = o.TiendaId,
                        TiendaNombre = o.TiendaNombre,
                        TiendaLogoUrl = o.TiendaLogo,
                        Precio = o.Precio,
                        PrecioAnterior = o.PrecioAnterior,
                        EsDisponible = o.EsDisponible,
                        UrlProductoEnTienda = o.UrlProductoEnTienda
                    })
                    .OrderBy(o => o.Precio)
                    .ToList()
                })
                .OrderBy(p => p.PrecioMinimo)
                .ToList();

            _logger.LogInformation("✅ Búsqueda completada en {Seconds:F2}s: {TotalOfertas} ofertas, {Count} productos únicos", 
                stopwatch.Elapsed.TotalSeconds, todasLasOfertas.Count, productosAgrupados.Count);

            return productosAgrupados;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error en búsqueda con scrapers");
            return new List<ProductoConOfertasDto>();
        }
    }

    // Delegar otros métodos al servicio base
    public Task<ResultadoBusquedaDto> BuscarProductosAsync(BusquedaProductoDto busqueda) 
        => _baseService.BuscarProductosAsync(busqueda);

    public Task<ProductoDetalleDto?> ObtenerProductoDetalleAsync(int productoId) 
        => _baseService.ObtenerProductoDetalleAsync(productoId);

    public Task<IEnumerable<OfertaDestacadaDto>> ObtenerOfertasDestacadasAsync(int cantidad = 10) 
        => _baseService.ObtenerOfertasDestacadasAsync(cantidad);

    public Task<IEnumerable<ProductoDto>> ObtenerProductosPorCategoriaAsync(string categoria) 
        => _baseService.ObtenerProductosPorCategoriaAsync(categoria);

    public Task<IEnumerable<string>> ObtenerCategoriasAsync() 
        => _baseService.ObtenerCategoriasAsync();

    public Task<IEnumerable<ProductoDto>> ObtenerProductosCompatiblesAsync(int marcaId, int modeloId) 
        => _baseService.ObtenerProductosCompatiblesAsync(marcaId, modeloId);

    public Task<bool> ActualizarPrecioOfertaAsync(int ofertaId, decimal nuevoPrecio) 
        => _baseService.ActualizarPrecioOfertaAsync(ofertaId, nuevoPrecio);
}
