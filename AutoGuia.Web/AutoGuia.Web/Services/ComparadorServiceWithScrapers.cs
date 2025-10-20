using AutoGuia.Core.DTOs;
using AutoGuia.Core.Entities;
using AutoGuia.Infrastructure.Data;
using AutoGuia.Infrastructure.Services;
using AutoGuia.Scraper.Scrapers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoGuia.Web.Services;

/// <summary>
/// Servicio que extiende ComparadorService con capacidad de scraping en tiempo real
/// </summary>
public class ComparadorServiceWithScrapers : IComparadorService
{
    private readonly ComparadorService _baseService;
    private readonly AutoGuiaDbContext _context;
    private readonly ILogger<ComparadorServiceWithScrapers> _logger;
    private readonly ConsumiblesScraperService? _mercadoLibreScraper;
    private readonly AutoplanetConsumiblesScraperService? _autoplanetScraper;
    private readonly MundoRepuestosConsumiblesScraperService? _mundoRepuestosScraper;

    public ComparadorServiceWithScrapers(
        ComparadorService baseService,
        AutoGuiaDbContext context,
        ILogger<ComparadorServiceWithScrapers> logger,
        ConsumiblesScraperService? mercadoLibreScraper = null,
        AutoplanetConsumiblesScraperService? autoplanetScraper = null,
        MundoRepuestosConsumiblesScraperService? mundoRepuestosScraper = null)
    {
        _baseService = baseService;
        _context = context;
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

            _logger.LogInformation("📊 Total de ofertas encontradas: {Count}", todasLasOfertas.Count);

            // 🏪 CREAR O ACTUALIZAR TIENDAS EN BD
            var tiendas = new Dictionary<string, Tienda>();
            var tiendasNombres = todasLasOfertas.Select(o => o.TiendaNombre).Distinct().ToList();

            foreach (var tiendaNombre in tiendasNombres)
            {
                if (string.IsNullOrWhiteSpace(tiendaNombre)) continue;

                var tiendaExistente = await _context.Tiendas
                    .FirstOrDefaultAsync(t => t.Nombre == tiendaNombre);

                if (tiendaExistente == null)
                {
                    _logger.LogInformation("🏪 Creando tienda: {Tienda}", tiendaNombre);
                    var nuevaTienda = new Tienda
                    {
                        Nombre = tiendaNombre,
                        UrlSitioWeb = ObtenerURLBase(tiendaNombre),
                        LogoUrl = ObtenerLogoUrl(tiendaNombre),
                        Descripcion = $"Tienda de consumibles automotrices",
                        EsActivo = true,
                        FechaCreacion = DateTime.UtcNow
                    };
                    _context.Tiendas.Add(nuevaTienda);
                    await _context.SaveChangesAsync();
                    tiendas[tiendaNombre] = nuevaTienda;
                }
                else
                {
                    tiendas[tiendaNombre] = tiendaExistente;
                }
            }

            // Agrupar ofertas por producto (nombre normalizado)
            var productosAgrupados = todasLasOfertas
                .GroupBy(o => NormalizarNombreProducto(o.ProductoNombre ?? "Producto sin nombre"))
                .Select(g => new ProductoConOfertasDto
                {
                    Id = 0, // No tenemos ID en scraping
                    Nombre = g.First().ProductoNombre ?? "Producto sin nombre",
                    NumeroDeParte = g.First().SKU,
                    Descripcion = g.First().ProductoNombre,
                    ImagenUrl = g.First().ProductoImagen,
                    Ofertas = g.Select(o => new OfertaComparadorDto
                    {
                        Id = 0,
                        TiendaId = tiendas.ContainsKey(o.TiendaNombre) ? tiendas[o.TiendaNombre].Id : 0,
                        TiendaNombre = o.TiendaNombre,
                        TiendaLogoUrl = tiendas.ContainsKey(o.TiendaNombre) ? tiendas[o.TiendaNombre].LogoUrl : null,
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

    // ==================== MÉTODOS AUXILIARES ====================

    private string ObtenerURLBase(string tiendaNombre)
    {
        return tiendaNombre switch
        {
            "MercadoLibre" => "https://www.mercadolibre.cl/",
            "Autoplanet" => "https://www.autoplanet.cl/",
            "MundoRepuestos" => "https://www.mundorepuestos.cl/",
            _ => "https://example.com"
        };
    }

    private string ObtenerLogoUrl(string tiendaNombre)
    {
        return tiendaNombre switch
        {
            "MercadoLibre" => "/images/tiendas/mercadolibre.png",
            "Autoplanet" => "/images/tiendas/autoplanet.png",
            "MundoRepuestos" => "/images/tiendas/mundorepuestos.png",
            _ => "/images/tiendas/default.png"
        };
    }

    private string NormalizarNombreProducto(string nombre)
    {
        if (string.IsNullOrEmpty(nombre)) return "";
        return nombre.Trim().ToLower();
    }
}
