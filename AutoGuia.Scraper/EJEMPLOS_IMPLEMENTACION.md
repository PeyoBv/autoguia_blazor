# 🚀 Ejemplos de Implementación - Scrapers Modulares

Esta guía proporciona ejemplos completos de cómo implementar scrapers específicos usando la arquitectura modular que acabamos de crear.

---

## 📋 Tabla de Contenidos

1. [Ejemplo 1: Scraper de API (MercadoLibre)](#ejemplo-1-scraper-de-api-mercadolibre)
2. [Ejemplo 2: Scraper HTML (Autoplanet)](#ejemplo-2-scraper-html-autoplanet)
3. [Ejemplo 3: Scraper con Playwright (MundoRepuestos)](#ejemplo-3-scraper-con-playwright-mundorepuestos)
4. [Cómo ejecutar el scraper](#cómo-ejecutar-el-scraper)
5. [Testing y debugging](#testing-y-debugging)

---

## Ejemplo 1: Scraper de API (MercadoLibre)

### Paso 1: Crear el archivo del scraper

**Ubicación:** `AutoGuia.Scraper/Scrapers/MercadoLibreApiScraper.cs`

```csharp
using AutoGuia.Scraper.DTOs;
using AutoGuia.Scraper.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace AutoGuia.Scraper.Scrapers;

/// <summary>
/// Scraper para MercadoLibre usando su API pública.
/// </summary>
public class MercadoLibreApiScraper : IScraper
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<MercadoLibreApiScraper> _logger;
    private const string ApiBaseUrl = "https://api.mercadolibre.com";
    
    public string TiendaNombre => "MercadoLibre";
    public string TipoScraper => "API";
    public bool EstaHabilitado => true;

    public MercadoLibreApiScraper(
        IHttpClientFactory httpClientFactory,
        ILogger<MercadoLibreApiScraper> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<IEnumerable<OfertaDto>> ScrapearProductosAsync(
        string numeroDeParte, 
        int tiendaId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("🔍 Buscando en MercadoLibre: {NumeroDeParte}", numeroDeParte);
            
            var client = _httpClientFactory.CreateClient("ScraperClient");
            var ofertas = new List<OfertaDto>();
            
            // 1. Buscar productos usando la API de búsqueda
            var searchUrl = $"{ApiBaseUrl}/sites/MLC/search?q={Uri.EscapeDataString(numeroDeParte)}&limit=10";
            var searchResponse = await client.GetAsync(searchUrl, cancellationToken);
            
            if (!searchResponse.IsSuccessStatusCode)
            {
                _logger.LogWarning("⚠️ API de MercadoLibre devolvió: {StatusCode}", searchResponse.StatusCode);
                return ofertas;
            }
            
            var searchData = await searchResponse.Content.ReadFromJsonAsync<MercadoLibreSearchResponse>(cancellationToken: cancellationToken);
            
            if (searchData?.Results == null || !searchData.Results.Any())
            {
                _logger.LogInformation("ℹ️ No se encontraron resultados en MercadoLibre");
                return ofertas;
            }
            
            // 2. Procesar cada resultado
            foreach (var result in searchData.Results.Take(5)) // Top 5 resultados
            {
                try
                {
                    ofertas.Add(new OfertaDto
                    {
                        TiendaId = tiendaId,
                        TiendaNombre = TiendaNombre,
                        Precio = result.Price,
                        UrlProducto = result.Permalink,
                        StockDisponible = result.AvailableQuantity > 0,
                        CantidadDisponible = result.AvailableQuantity,
                        Descripcion = result.Title,
                        ImagenUrl = result.Thumbnail,
                        Calificacion = result.Seller?.ReputationLevelId != null ? 4.0m : null,
                        FechaScrapeo = DateTime.UtcNow,
                        TieneErrores = false
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error procesando resultado de MercadoLibre: {ItemId}", result.Id);
                }
            }
            
            _logger.LogInformation("✅ Se encontraron {Count} ofertas en MercadoLibre", ofertas.Count);
            return ofertas;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al scrapear MercadoLibre");
            return new List<OfertaDto>
            {
                new OfertaDto
                {
                    TiendaId = tiendaId,
                    TiendaNombre = TiendaNombre,
                    TieneErrores = true,
                    MensajeError = ex.Message,
                    FechaScrapeo = DateTime.UtcNow
                }
            };
        }
    }

    public bool PuedeScrapearTienda(string tiendaNombre)
    {
        return tiendaNombre.Equals("MercadoLibre", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<Dictionary<string, string>> ObtenerConfiguracionAsync()
    {
        return await Task.FromResult(new Dictionary<string, string>
        {
            ["ApiBaseUrl"] = ApiBaseUrl,
            ["Timeout"] = "30",
            ["MaxResults"] = "10",
            ["RetryCount"] = "3",
            ["CacheDuration"] = "300" // 5 minutos
        });
    }
}

// DTOs para deserializar la respuesta de MercadoLibre
public class MercadoLibreSearchResponse
{
    [JsonPropertyName("results")]
    public List<MercadoLibreItem>? Results { get; set; }
}

public class MercadoLibreItem
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
    
    [JsonPropertyName("available_quantity")]
    public int AvailableQuantity { get; set; }
    
    [JsonPropertyName("permalink")]
    public string Permalink { get; set; } = string.Empty;
    
    [JsonPropertyName("thumbnail")]
    public string? Thumbnail { get; set; }
    
    [JsonPropertyName("seller")]
    public MercadoLibreSeller? Seller { get; set; }
}

public class MercadoLibreSeller
{
    [JsonPropertyName("reputation_level_id")]
    public string? ReputationLevelId { get; set; }
}
```

### Ventajas del Scraper de API:
- ✅ **Confiable**: API oficial, menos propenso a cambios
- ✅ **Rápido**: Respuestas JSON estructuradas
- ✅ **Sin parsing HTML**: No depende de la estructura visual
- ⚠️ **Rate Limits**: Puede tener límites de solicitudes

---

## Ejemplo 2: Scraper HTML (Autoplanet)

### Paso 1: Instalar HtmlAgilityPack

```powershell
dotnet add AutoGuia.Scraper/AutoGuia.Scraper.csproj package HtmlAgilityPack
```

### Paso 2: Crear el scraper

**Ubicación:** `AutoGuia.Scraper/Scrapers/AutoplanetHtmlScraper.cs`

```csharp
using AutoGuia.Scraper.DTOs;
using AutoGuia.Scraper.Interfaces;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace AutoGuia.Scraper.Scrapers;

/// <summary>
/// Scraper para Autoplanet usando parsing HTML con HtmlAgilityPack.
/// </summary>
public class AutoplanetHtmlScraper : IScraper
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AutoplanetHtmlScraper> _logger;
    private const string BaseUrl = "https://www.autoplanet.cl";
    
    public string TiendaNombre => "Autoplanet";
    public string TipoScraper => "HTML";
    public bool EstaHabilitado => true;

    public AutoplanetHtmlScraper(
        IHttpClientFactory httpClientFactory,
        ILogger<AutoplanetHtmlScraper> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<IEnumerable<OfertaDto>> ScrapearProductosAsync(
        string numeroDeParte, 
        int tiendaId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("🔍 Buscando en Autoplanet: {NumeroDeParte}", numeroDeParte);
            
            var client = _httpClientFactory.CreateClient("ScraperClient");
            var ofertas = new List<OfertaDto>();
            
            // 1. Construir URL de búsqueda
            var searchUrl = $"{BaseUrl}/buscar?q={Uri.EscapeDataString(numeroDeParte)}";
            
            // 2. Descargar HTML
            var html = await client.GetStringAsync(searchUrl, cancellationToken);
            
            // 3. Parsear HTML con HtmlAgilityPack
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            
            // 4. Buscar elementos de productos (ajustar selectores según la estructura real)
            var productNodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'product-item')]");
            
            if (productNodes == null || !productNodes.Any())
            {
                _logger.LogInformation("ℹ️ No se encontraron productos en Autoplanet");
                return ofertas;
            }
            
            // 5. Extraer información de cada producto
            foreach (var productNode in productNodes.Take(10))
            {
                try
                {
                    var oferta = ExtraerOfertaDeNodo(productNode, tiendaId);
                    if (oferta != null)
                    {
                        ofertas.Add(oferta);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error al parsear producto individual");
                }
            }
            
            _logger.LogInformation("✅ Se encontraron {Count} ofertas en Autoplanet", ofertas.Count);
            return ofertas;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al scrapear Autoplanet");
            return new List<OfertaDto>
            {
                new OfertaDto
                {
                    TiendaId = tiendaId,
                    TiendaNombre = TiendaNombre,
                    TieneErrores = true,
                    MensajeError = ex.Message,
                    FechaScrapeo = DateTime.UtcNow
                }
            };
        }
    }

    private OfertaDto? ExtraerOfertaDeNodo(HtmlNode productNode, int tiendaId)
    {
        try
        {
            // Ajustar selectores CSS según la estructura real de Autoplanet
            var titulo = productNode.SelectSingleNode(".//h3[@class='product-title']")?.InnerText.Trim();
            var precioText = productNode.SelectSingleNode(".//span[@class='price']")?.InnerText.Trim();
            var urlRelativa = productNode.SelectSingleNode(".//a[@class='product-link']")?.GetAttributeValue("href", "");
            var imagenUrl = productNode.SelectSingleNode(".//img[@class='product-image']")?.GetAttributeValue("src", "");
            var stockText = productNode.SelectSingleNode(".//span[@class='stock-status']")?.InnerText.Trim();
            
            // Parsear precio (eliminar "$", ".", y convertir)
            if (string.IsNullOrEmpty(precioText))
                return null;
                
            var precioLimpio = precioText.Replace("$", "").Replace(".", "").Trim();
            if (!decimal.TryParse(precioLimpio, out var precio))
                return null;
            
            var urlCompleta = urlRelativa?.StartsWith("http") == true 
                ? urlRelativa 
                : $"{BaseUrl}{urlRelativa}";
            
            var stockDisponible = !string.IsNullOrEmpty(stockText) && 
                                 !stockText.Contains("agotado", StringComparison.OrdinalIgnoreCase);
            
            return new OfertaDto
            {
                TiendaId = tiendaId,
                TiendaNombre = TiendaNombre,
                Precio = precio,
                UrlProducto = urlCompleta ?? "",
                StockDisponible = stockDisponible,
                Descripcion = titulo,
                ImagenUrl = imagenUrl,
                FechaScrapeo = DateTime.UtcNow,
                TieneErrores = false
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error al extraer datos del nodo HTML");
            return null;
        }
    }

    public bool PuedeScrapearTienda(string tiendaNombre)
    {
        return tiendaNombre.Equals("Autoplanet", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<Dictionary<string, string>> ObtenerConfiguracionAsync()
    {
        return await Task.FromResult(new Dictionary<string, string>
        {
            ["BaseUrl"] = BaseUrl,
            ["Timeout"] = "30",
            ["MaxProducts"] = "10",
            ["UserAgent"] = "AutoGuia-Scraper/1.0",
            ["RetryCount"] = "2"
        });
    }
}
```

### Ventajas del Scraper HTML:
- ✅ **Flexible**: Puede adaptarse a cualquier sitio web
- ✅ **No requiere API**: Funciona con sitios sin API pública
- ⚠️ **Frágil**: Cambios en el HTML pueden romper el scraper
- ⚠️ **Más lento**: Requiere descargar y parsear HTML completo

---

## Ejemplo 3: Scraper con Playwright (MundoRepuestos)

### Paso 1: Instalar Playwright

```powershell
dotnet add AutoGuia.Scraper/AutoGuia.Scraper.csproj package Microsoft.Playwright
dotnet exec playwright install chromium
```

### Paso 2: Crear el scraper

**Ubicación:** `AutoGuia.Scraper/Scrapers/MundoRepuestosPlaywrightScraper.cs`

```csharp
using AutoGuia.Scraper.DTOs;
using AutoGuia.Scraper.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

namespace AutoGuia.Scraper.Scrapers;

/// <summary>
/// Scraper para MundoRepuestos usando Playwright (navegador headless).
/// Útil para sitios con JavaScript dinámico o protecciones anti-bot.
/// </summary>
public class MundoRepuestosPlaywrightScraper : IScraper
{
    private readonly ILogger<MundoRepuestosPlaywrightScraper> _logger;
    private const string BaseUrl = "https://www.mundorepuestos.cl";
    
    public string TiendaNombre => "MundoRepuestos";
    public string TipoScraper => "Playwright";
    public bool EstaHabilitado => true;

    public MundoRepuestosPlaywrightScraper(ILogger<MundoRepuestosPlaywrightScraper> logger)
    {
        _logger = logger;
    }

    public async Task<IEnumerable<OfertaDto>> ScrapearProductosAsync(
        string numeroDeParte, 
        int tiendaId,
        CancellationToken cancellationToken = default)
    {
        IPlaywright? playwright = null;
        IBrowser? browser = null;
        
        try
        {
            _logger.LogInformation("🔍 Buscando en MundoRepuestos con Playwright: {NumeroDeParte}", numeroDeParte);
            
            var ofertas = new List<OfertaDto>();
            
            // 1. Inicializar Playwright
            playwright = await Playwright.CreateAsync();
            browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true, // Sin interfaz gráfica
                Args = new[] { "--disable-blink-features=AutomationControlled" }
            });
            
            var context = await browser.NewContextAsync(new BrowserNewContextOptions
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36"
            });
            
            var page = await context.NewPageAsync();
            
            // 2. Navegar a la página de búsqueda
            var searchUrl = $"{BaseUrl}/search?q={Uri.EscapeDataString(numeroDeParte)}";
            await page.GotoAsync(searchUrl, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
            
            // 3. Esperar a que carguen los productos (ajustar selector)
            await page.WaitForSelectorAsync(".product-grid .product-item", new PageWaitForSelectorOptions
            {
                Timeout = 10000
            });
            
            // 4. Extraer información de productos
            var productos = await page.QuerySelectorAllAsync(".product-grid .product-item");
            
            if (!productos.Any())
            {
                _logger.LogInformation("ℹ️ No se encontraron productos en MundoRepuestos");
                return ofertas;
            }
            
            foreach (var producto in productos.Take(10))
            {
                try
                {
                    var titulo = await producto.QuerySelectorAsync(".product-title");
                    var precio = await producto.QuerySelectorAsync(".product-price");
                    var link = await producto.QuerySelectorAsync("a.product-link");
                    var imagen = await producto.QuerySelectorAsync("img.product-image");
                    var stock = await producto.QuerySelectorAsync(".stock-indicator");
                    
                    var tituloText = titulo != null ? await titulo.TextContentAsync() : "";
                    var precioText = precio != null ? await precio.TextContentAsync() : "";
                    var linkHref = link != null ? await link.GetAttributeAsync("href") : "";
                    var imagenSrc = imagen != null ? await imagen.GetAttributeAsync("src") : "";
                    var stockText = stock != null ? await stock.TextContentAsync() : "";
                    
                    // Parsear precio
                    var precioLimpio = precioText?.Replace("$", "").Replace(".", "").Trim();
                    if (string.IsNullOrEmpty(precioLimpio) || !decimal.TryParse(precioLimpio, out var precioDecimal))
                        continue;
                    
                    var urlCompleta = linkHref?.StartsWith("http") == true 
                        ? linkHref 
                        : $"{BaseUrl}{linkHref}";
                    
                    var stockDisponible = !string.IsNullOrEmpty(stockText) && 
                                         stockText.Contains("disponible", StringComparison.OrdinalIgnoreCase);
                    
                    ofertas.Add(new OfertaDto
                    {
                        TiendaId = tiendaId,
                        TiendaNombre = TiendaNombre,
                        Precio = precioDecimal,
                        UrlProducto = urlCompleta ?? "",
                        StockDisponible = stockDisponible,
                        Descripcion = tituloText?.Trim(),
                        ImagenUrl = imagenSrc,
                        FechaScrapeo = DateTime.UtcNow,
                        TieneErrores = false
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error al extraer producto individual con Playwright");
                }
            }
            
            _logger.LogInformation("✅ Se encontraron {Count} ofertas en MundoRepuestos", ofertas.Count);
            return ofertas;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al scrapear MundoRepuestos con Playwright");
            return new List<OfertaDto>
            {
                new OfertaDto
                {
                    TiendaId = tiendaId,
                    TiendaNombre = TiendaNombre,
                    TieneErrores = true,
                    MensajeError = ex.Message,
                    FechaScrapeo = DateTime.UtcNow
                }
            };
        }
        finally
        {
            // 5. Limpiar recursos
            if (browser != null)
                await browser.CloseAsync();
            
            if (playwright != null)
                playwright.Dispose();
        }
    }

    public bool PuedeScrapearTienda(string tiendaNombre)
    {
        return tiendaNombre.Equals("MundoRepuestos", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<Dictionary<string, string>> ObtenerConfiguracionAsync()
    {
        return await Task.FromResult(new Dictionary<string, string>
        {
            ["BaseUrl"] = BaseUrl,
            ["BrowserType"] = "Chromium",
            ["Headless"] = "true",
            ["Timeout"] = "30000",
            ["MaxProducts"] = "10"
        });
    }
}
```

### Ventajas del Scraper con Playwright:
- ✅ **JavaScript**: Ejecuta JavaScript como un navegador real
- ✅ **Anti-bot**: Puede evadir protecciones básicas
- ✅ **Capturas**: Puede tomar screenshots para debugging
- ⚠️ **Más lento**: Requiere lanzar navegador completo
- ⚠️ **Más recursos**: Consume más memoria y CPU

---

## Cómo Ejecutar el Scraper

### Opción 1: Scrapear un producto específico

```csharp
// En el Worker o en un endpoint de API
var orchestrator = serviceProvider.GetRequiredService<ScraperOrchestratorService>();

// Scrapear producto ID 1 en todas las tiendas
await orchestrator.EjecutarScrapingAsync(productoId: 1);
```

### Opción 2: Scrapear múltiples productos en paralelo

```csharp
var orchestrator = serviceProvider.GetRequiredService<ScraperOrchestratorService>();

var productosIds = new[] { 1, 2, 3, 4, 5 };

// Máximo 3 scrapers en paralelo
await orchestrator.EjecutarScrapingMasivoAsync(productosIds, maxParallelism: 3);
```

### Opción 3: Ver estadísticas de scrapers

```csharp
var orchestrator = serviceProvider.GetRequiredService<ScraperOrchestratorService>();

var stats = await orchestrator.ObtenerEstadisticasScrapersAsync();

Console.WriteLine($"Total Scrapers: {stats["TotalScrapers"]}");
Console.WriteLine($"Scrapers Habilitados: {stats["ScrapersHabilitados"]}");
```

---

## Testing y Debugging

### 1. Crear un test unitario

**Ubicación:** `AutoGuia.Scraper.Tests/Scrapers/MercadoLibreApiScraperTests.cs`

```csharp
using AutoGuia.Scraper.Scrapers;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace AutoGuia.Scraper.Tests.Scrapers;

public class MercadoLibreApiScraperTests
{
    [Fact]
    public void TiendaNombre_DebeRetornarMercadoLibre()
    {
        // Arrange
        var httpClientFactory = new Mock<IHttpClientFactory>();
        var scraper = new MercadoLibreApiScraper(httpClientFactory.Object, NullLogger<MercadoLibreApiScraper>.Instance);
        
        // Act
        var tiendaNombre = scraper.TiendaNombre;
        
        // Assert
        Assert.Equal("MercadoLibre", tiendaNombre);
    }
    
    [Fact]
    public void PuedeScrapearTienda_ConMercadoLibre_DebeRetornarTrue()
    {
        // Arrange
        var httpClientFactory = new Mock<IHttpClientFactory>();
        var scraper = new MercadoLibreApiScraper(httpClientFactory.Object, NullLogger<MercadoLibreApiScraper>.Instance);
        
        // Act
        var puede = scraper.PuedeScrapearTienda("MercadoLibre");
        
        // Assert
        Assert.True(puede);
    }
}
```

### 2. Debugging con logging detallado

En `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "AutoGuia.Scraper.Scrapers": "Debug",
      "AutoGuia.Scraper.Services.ScraperOrchestratorService": "Debug"
    }
  }
}
```

### 3. Ejecutar un solo scraper manualmente

```csharp
var scraper = serviceProvider.GetRequiredService<IEnumerable<IScraper>>()
    .FirstOrDefault(s => s.TiendaNombre == "MercadoLibre");

if (scraper != null)
{
    var ofertas = await scraper.ScrapearProductosAsync("12345", tiendaId: 1);
    
    foreach (var oferta in ofertas)
    {
        Console.WriteLine($"Precio: ${oferta.Precio}, URL: {oferta.UrlProducto}");
    }
}
```

---

## 🎯 Próximos Pasos

1. **Implementar scrapers reales**: Ajustar selectores HTML según la estructura real de cada tienda
2. **Rate limiting**: Implementar delays entre requests para evitar baneos
3. **Retry policies**: Usar Polly para reintentos automáticos
4. **Caché**: Implementar caché de resultados para reducir requests
5. **Monitoreo**: Agregar métricas y alertas cuando los scrapers fallen

---

**Generado por:** GitHub Copilot  
**Fecha:** 18 de octubre de 2025  
**Proyecto:** AutoGuía - Ejemplos de Implementación de Scrapers
