using AutoGuia.Core.Entities;
using AutoGuia.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoGuia.Scraper.Services;

/// <summary>
/// Servicio para inicializar datos semilla en la base de datos del scraper.
/// Crea productos y tiendas de ejemplo para probar el sistema de scraping.
/// </summary>
public class ScraperDataSeederService
{
    private readonly AutoGuiaDbContext _context;
    private readonly ILogger<ScraperDataSeederService> _logger;

    public ScraperDataSeederService(
        AutoGuiaDbContext context,
        ILogger<ScraperDataSeederService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Inicializa datos semilla para el scraper si la base de datos está vacía.
    /// </summary>
    public async Task InicializarDatosSemilla()
    {
        _logger.LogInformation("🌱 Verificando si necesitamos inicializar datos semilla...");

        try
        {
            // Verificar si ya hay datos
            var tieneDatos = await _context.Productos.AnyAsync() || await _context.Tiendas.AnyAsync();
            
            if (tieneDatos)
            {
                _logger.LogInformation("✅ Los datos semilla ya existen");
                return;
            }

            _logger.LogInformation("🌱 Inicializando datos semilla para el scraper...");

            // Crear tiendas de ejemplo
            await CrearTiendasDeEjemplo();
            
            // Crear productos de ejemplo
            await CrearProductosDeEjemplo();

            await _context.SaveChangesAsync();
            
            _logger.LogInformation("✅ Datos semilla inicializados correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error inicializando datos semilla");
            throw;
        }
    }

    /// <summary>
    /// Crea tiendas de ejemplo para probar el scraping.
    /// </summary>
    private async Task CrearTiendasDeEjemplo()
    {
        var tiendas = new[]
        {
            new Tienda
            {
                Nombre = "RepuestosTotal",
                UrlSitioWeb = "https://www.repuestostotal.cl",
                Descripcion = "Tienda líder en repuestos automotrices en Chile",
                EsActivo = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Tienda
            {
                Nombre = "AutoZone Chile",
                UrlSitioWeb = "https://www.autozone.cl",
                Descripcion = "Cadena internacional de repuestos automotrices",
                EsActivo = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Tienda
            {
                Nombre = "Repuestos Santiago",
                UrlSitioWeb = "https://www.repuestossantiago.cl",
                Descripcion = "Distribuidor local de repuestos en Santiago",
                EsActivo = true,
                FechaCreacion = DateTime.UtcNow
            }
        };

        foreach (var tienda in tiendas)
        {
            var existe = await _context.Tiendas
                .AnyAsync(t => t.Nombre == tienda.Nombre);
            
            if (!existe)
            {
                _context.Tiendas.Add(tienda);
                _logger.LogDebug("🏪 Tienda agregada: {TiendaNombre}", tienda.Nombre);
            }
        }
    }

    /// <summary>
    /// Crea productos de ejemplo para probar el scraping.
    /// </summary>
    private async Task CrearProductosDeEjemplo()
    {
        var productos = new[]
        {
            new Producto
            {
                Nombre = "Filtro de Aceite Bosch P3274",
                NumeroDeParte = "P3274",
                Descripcion = "Filtro de aceite original Bosch para motores Toyota y Nissan",
                EsActivo = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Producto
            {
                Nombre = "Pastillas de Freno Brembo P85075",
                NumeroDeParte = "P85075",
                Descripcion = "Pastillas de freno delanteras Brembo para sedanes medianos",
                EsActivo = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Producto
            {
                Nombre = "Aceite Motor Mobil 1 5W30",
                NumeroDeParte = "M1-5W30-4L",
                Descripcion = "Aceite sintético premium Mobil 1 5W30 - 4 litros",
                EsActivo = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Producto
            {
                Nombre = "Amortiguador KYB 341457",
                NumeroDeParte = "KYB341457",
                Descripcion = "Amortiguador trasero KYB para Honda Civic 2006-2012",
                EsActivo = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Producto
            {
                Nombre = "Bujía NGK BKR6E",
                NumeroDeParte = "BKR6E",
                Descripcion = "Bujía de encendido NGK BKR6E para motores a gasolina",
                EsActivo = true,
                FechaCreacion = DateTime.UtcNow
            }
        };

        foreach (var producto in productos)
        {
            var existe = await _context.Productos
                .AnyAsync(p => p.NumeroDeParte == producto.NumeroDeParte);
            
            if (!existe)
            {
                _context.Productos.Add(producto);
                _logger.LogDebug("🔧 Producto agregado: {ProductoNombre} ({NumeroParte})", 
                    producto.Nombre, producto.NumeroDeParte);
            }
        }
    }

    /// <summary>
    /// Obtiene la tienda RepuestosTotal para usar en el scraper.
    /// </summary>
    public async Task<Tienda?> ObtenerTiendaRepuestosTotal()
    {
        return await _context.Tiendas
            .FirstOrDefaultAsync(t => t.Nombre == "RepuestosTotal" && t.EsActivo);
    }

    /// <summary>
    /// Obtiene productos activos para scrapear.
    /// </summary>
    public async Task<List<Producto>> ObtenerProductosParaScrapear(int limite = 10)
    {
        return await _context.Productos
            .Where(p => p.EsActivo)
            .Take(limite)
            .ToListAsync();
    }
}