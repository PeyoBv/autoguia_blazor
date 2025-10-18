using AutoGuia.Scraper.DTOs;
using AutoGuia.Scraper.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using System.Globalization;
using System.Text.RegularExpressions;

namespace AutoGuia.Scraper.Scrapers;

/// <summary>
/// Scraper para Autoplanet.cl que utiliza Playwright para manejar sitios SPA con JavaScript.
/// Esta versión es ideal para sitios que cargan contenido dinámicamente con JS.
/// 
/// VENTAJAS sobre HttpClient + HtmlAgilityPack:
/// - ✅ Ejecuta JavaScript (renderizado completo)
/// - ✅ Espera a que elementos sean visibles
/// - ✅ Maneja sitios SPA (React, Vue, Angular)
/// - ✅ Puede interactuar con elementos (clicks, scroll)
/// - ✅ Screenshots para debugging
/// 
/// DESVENTAJAS:
/// - ⚠️ Más lento (~10-15s vs ~3-5s)
/// - ⚠️ Consume más recursos (navegador headless)
/// - ⚠️ Requiere instalación de navegadores (pwsh bin/Debug/net8.0/playwright.ps1 install)
/// </summary>
public class AutoplanetPlaywrightScraperService : IScraper
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AutoplanetPlaywrightScraperService> _logger;
    
    private readonly string _baseUrl;
    private readonly string _productSearchUrl;
    private readonly int _maxResults;
    private readonly int _timeoutSeconds;
    private readonly int _requestDelayMs;

    // 🎯 Selectores CSS para Playwright (más modernos que XPath)
    // Playwright recomienda CSS selectors sobre XPath para mejor performance
    private readonly string[] _selectoresNodoProducto = new[]
    {
        ".product-card",
        ".producto",
        ".product",
        ".item-product",
        "article[data-product]",
        ".grid-item .product",
        "[itemtype='http://schema.org/Product']"
    };

    private readonly string[] _selectoresTitulo = new[]
    {
        ".product-title",
        ".product-name",
        "h3.product-name",
        "h2.title",
        "[itemprop='name']",
        ".product-info h4"
    };

    private readonly string[] _selectoresPrecio = new[]
    {
        ".price",
        ".product-price",
        "[itemprop='price']",
        ".precio",
        ".price-current",
        ".woocommerce-Price-amount"
    };

    private readonly string[] _selectoresImagen = new[]
    {
        ".product-image",
        "[itemprop='image']",
        ".product-img img",
        ".product-thumbnail img"
    };

    private readonly string[] _selectoresUrl = new[]
    {
        ".product-link",
        "h3 a",
        "h2 a",
        "a.product-name"
    };

    private readonly string[] _selectoresStock = new[]
    {
        ".stock",
        ".availability",
        "[itemprop='availability']",
        ".product-availability"
    };

    public string TiendaNombre => "Autoplanet";
    public string TipoScraper => "Playwright"; // Indicamos que usa Playwright
    public bool EstaHabilitado { get; private set; }

    public AutoplanetPlaywrightScraperService(
        IConfiguration configuration,
        ILogger<AutoplanetPlaywrightScraperService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        // Cargar configuración
        _baseUrl = _configuration["Stores:Autoplanet:BaseUrl"] ?? "https://www.autoplanet.cl";
        _productSearchUrl = _configuration["Stores:Autoplanet:ProductSearchUrl"] ?? "/busqueda?q={0}";
        _maxResults = _configuration.GetValue<int>("ScrapingSettings:Autoplanet:MaxResults", 10);
        _timeoutSeconds = _configuration.GetValue<int>("ScrapingSettings:Autoplanet:TimeoutSeconds", 60); // Más tiempo para Playwright
        _requestDelayMs = _configuration.GetValue<int>("Stores:Autoplanet:RequestDelayMs", 2000);
        
        EstaHabilitado = _configuration.GetValue<bool>("Stores:Autoplanet:Enabled", true);

        _logger.LogInformation(
            "AutoplanetPlaywrightScraperService inicializado. BaseUrl: {BaseUrl}, MaxResults: {MaxResults}, Habilitado: {Enabled}",
            _baseUrl, _maxResults, EstaHabilitado);
    }

    public bool PuedeScrapearTienda(string nombreTienda)
    {
        return nombreTienda.Equals("Autoplanet", StringComparison.OrdinalIgnoreCase);
    }

    public Task<Dictionary<string, string>> ObtenerConfiguracionAsync()
    {
        var config = new Dictionary<string, string>
        {
            { "TiendaNombre", TiendaNombre },
            { "TipoScraper", TipoScraper },
            { "BaseUrl", _baseUrl },
            { "MaxResults", _maxResults.ToString() },
            { "TimeoutSeconds", _timeoutSeconds.ToString() },
            { "RequestDelayMs", _requestDelayMs.ToString() },
            { "Habilitado", EstaHabilitado.ToString() }
        };

        return Task.FromResult(config);
    }

    /// <summary>
    /// Scrapea productos usando Playwright para sitios con JavaScript.
    /// </summary>
    public async Task<IEnumerable<OfertaDto>> ScrapearProductosAsync(
        string numeroDeParte,
        int tiendaId,
        CancellationToken cancellationToken = default)
    {
        if (!EstaHabilitado)
        {
            _logger.LogWarning("Scraper de Autoplanet (Playwright) está deshabilitado");
            return Enumerable.Empty<OfertaDto>();
        }

        if (string.IsNullOrWhiteSpace(numeroDeParte))
        {
            _logger.LogWarning("Número de parte vacío o nulo");
            return Enumerable.Empty<OfertaDto>();
        }

        var ofertas = new List<OfertaDto>();
        IPlaywright? playwright = null;
        IBrowser? browser = null;

        try
        {
            _logger.LogInformation(
                "🎭 Iniciando scraping con Playwright en Autoplanet para '{NumeroDeParte}'",
                numeroDeParte);

            // 🎭 PASO 1: Crear instancia de Playwright
            playwright = await Playwright.CreateAsync();
            
            _logger.LogDebug("Playwright creado exitosamente");

            // 🎭 PASO 2: Configurar opciones del navegador
            var browserOptions = new BrowserTypeLaunchOptions
            {
                Headless = true, // Sin interfaz gráfica (más rápido)
                // Headless = false, // Descomentar para ver el navegador (debugging)
                Timeout = _timeoutSeconds * 1000, // Convertir a ms
                Args = new[]
                {
                    "--disable-blink-features=AutomationControlled", // Evitar detección
                    "--disable-dev-shm-usage", // Para ambientes con poca memoria
                    "--no-sandbox" // Para ambientes Docker/Linux
                }
            };

            // 🎭 PASO 3: Lanzar navegador (Chromium por defecto)
            _logger.LogDebug("Lanzando navegador Chromium...");
            browser = await playwright.Chromium.LaunchAsync(browserOptions);
            
            // 🎭 PASO 4: Crear contexto de navegador (simula sesión)
            var contextOptions = new BrowserNewContextOptions
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
                           "(KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
                Locale = "es-CL",
                TimezoneId = "America/Santiago",
                ViewportSize = new ViewportSize { Width = 1920, Height = 1080 }
            };
            
            await using var context = await browser.NewContextAsync(contextOptions);
            
            // 🎭 PASO 5: Crear nueva página
            var page = await context.NewPageAsync();
            
            // Opcional: Configurar timeout para todas las operaciones
            page.SetDefaultTimeout(_timeoutSeconds * 1000);

            // 🎭 PASO 6: Construir URL y navegar
            var searchUrl = ConstruirUrlBusqueda(numeroDeParte);
            _logger.LogInformation("Navegando a: {SearchUrl}", searchUrl);

            var navigationOptions = new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle, // Esperar a que no haya más requests de red
                Timeout = _timeoutSeconds * 1000
            };

            await page.GotoAsync(searchUrl, navigationOptions);
            _logger.LogDebug("Página cargada exitosamente");

            // 🎭 PASO 7: Esperar a que los productos sean visibles
            // Intentar con cada selector hasta encontrar uno que funcione
            ILocator? productosLocator = null;
            string? selectorExitoso = null;

            foreach (var selector in _selectoresNodoProducto)
            {
                try
                {
                    // Esperar máximo 5 segundos por selector
                    await page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions
                    {
                        State = WaitForSelectorState.Visible,
                        Timeout = 5000
                    });

                    productosLocator = page.Locator(selector);
                    var count = await productosLocator.CountAsync();

                    if (count > 0)
                    {
                        selectorExitoso = selector;
                        _logger.LogDebug("✅ Selector exitoso: '{Selector}' encontró {Count} productos", selector, count);
                        break;
                    }
                }
                catch (TimeoutException)
                {
                    _logger.LogTrace("Selector '{Selector}' no encontró elementos", selector);
                }
            }

            if (productosLocator == null || selectorExitoso == null)
            {
                _logger.LogWarning("No se encontraron productos en la página");
                
                // Opcional: Tomar screenshot para debugging
                await page.ScreenshotAsync(new PageScreenshotOptions
                {
                    Path = $"debug_autoplanet_{DateTime.UtcNow:yyyyMMddHHmmss}.png"
                });
                
                return ofertas;
            }

            // 🎭 PASO 8: Extraer datos de cada producto
            var totalProductos = await productosLocator.CountAsync();
            var productosAProcesar = Math.Min(totalProductos, _maxResults);

            _logger.LogInformation("Procesando {Count} de {Total} productos encontrados", 
                productosAProcesar, totalProductos);

            for (int i = 0; i < productosAProcesar; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogWarning("Scraping cancelado por el usuario");
                    break;
                }

                try
                {
                    var productoElement = productosLocator.Nth(i);
                    var oferta = await ExtraerOfertaDeElemento(productoElement, tiendaId);

                    if (oferta != null)
                    {
                        ofertas.Add(oferta);
                        _logger.LogDebug("✅ Oferta {Index}/{Total}: {Descripcion} - ${Precio}", 
                            i + 1, productosAProcesar, oferta.Descripcion, oferta.Precio);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al extraer producto {Index}", i);
                    
                    ofertas.Add(new OfertaDto
                    {
                        TiendaId = tiendaId,
                        TiendaNombre = TiendaNombre,
                        TieneErrores = true,
                        MensajeError = $"Error al parsear producto {i}: {ex.Message}"
                    });
                }

                // Pequeño delay entre productos
                await Task.Delay(100, cancellationToken);
            }

            _logger.LogInformation("✅ Scraping completado. Extraídas {Count} ofertas", ofertas.Count);
        }
        catch (PlaywrightException ex)
        {
            _logger.LogError(ex, "Error de Playwright durante el scraping");
            ofertas.Add(new OfertaDto
            {
                TiendaId = tiendaId,
                TiendaNombre = TiendaNombre,
                TieneErrores = true,
                MensajeError = $"Error de Playwright: {ex.Message}"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error crítico durante scraping con Playwright");
            ofertas.Add(new OfertaDto
            {
                TiendaId = tiendaId,
                TiendaNombre = TiendaNombre,
                TieneErrores = true,
                MensajeError = $"Error crítico: {ex.Message}"
            });
        }
        finally
        {
            // 🎭 Liberar recursos de Playwright
            if (browser != null)
            {
                await browser.CloseAsync();
                await browser.DisposeAsync();
                _logger.LogDebug("Navegador cerrado correctamente");
            }
            playwright.Dispose();
        }

        return ofertas;
    }

    /// <summary>
    /// Extrae datos de una oferta desde un ILocator de Playwright.
    /// </summary>
    private async Task<OfertaDto?> ExtraerOfertaDeElemento(ILocator elemento, int tiendaId)
    {
        try
        {
            // Extraer título
            var titulo = await ExtraerTextoAsync(elemento, _selectoresTitulo);
            
            if (string.IsNullOrWhiteSpace(titulo))
            {
                _logger.LogWarning("No se pudo extraer el título del producto");
                return null;
            }

            // Extraer precio
            var precioTexto = await ExtraerTextoAsync(elemento, _selectoresPrecio);
            decimal precio = 0;
            
            if (!string.IsNullOrWhiteSpace(precioTexto))
            {
                precio = LimpiarYParsearPrecio(precioTexto);
            }

            // Extraer URL
            var urlRelativa = await ExtraerAtributoAsync(elemento, _selectoresUrl, "href");
            var urlCompleta = !string.IsNullOrWhiteSpace(urlRelativa)
                ? ConstruirUrlCompleta(urlRelativa, _baseUrl)
                : null;

            // Extraer imagen
            var imagenRelativa = await ExtraerAtributoAsync(elemento, _selectoresImagen, "src");
            var imagenCompleta = !string.IsNullOrWhiteSpace(imagenRelativa)
                ? ConstruirUrlCompleta(imagenRelativa, _baseUrl)
                : null;

            // Extraer stock
            var textoStock = await ExtraerTextoAsync(elemento, _selectoresStock);
            var stockDisponible = DeterminarDisponibilidad(textoStock);

            var oferta = new OfertaDto
            {
                ProductoId = 0, // No tenemos SKU en este caso
                TiendaId = tiendaId,
                Precio = precio,
                UrlProducto = urlCompleta ?? string.Empty,
                StockDisponible = stockDisponible,
                Descripcion = titulo?.Trim(),
                ImagenUrl = imagenCompleta,
                TiendaNombre = TiendaNombre,
                TieneErrores = false
            };

            return oferta;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al extraer datos del elemento");
            return null;
        }
    }

    /// <summary>
    /// Extrae texto de un elemento usando múltiples selectores.
    /// </summary>
    private async Task<string?> ExtraerTextoAsync(ILocator elemento, string[] selectores)
    {
        foreach (var selector in selectores)
        {
            try
            {
                var targetElement = elemento.Locator(selector).First;
                
                // Verificar si el elemento existe
                var count = await targetElement.CountAsync();
                if (count > 0)
                {
                    var texto = await targetElement.TextContentAsync();
                    
                    if (!string.IsNullOrWhiteSpace(texto))
                    {
                        // Limpiar espacios múltiples y saltos de línea
                        texto = Regex.Replace(texto.Trim(), @"\s+", " ");
                        return texto;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogTrace(ex, "Selector '{Selector}' falló", selector);
            }
        }

        return null;
    }

    /// <summary>
    /// Extrae un atributo de un elemento usando múltiples selectores.
    /// </summary>
    private async Task<string?> ExtraerAtributoAsync(ILocator elemento, string[] selectores, string atributo)
    {
        foreach (var selector in selectores)
        {
            try
            {
                var targetElement = elemento.Locator(selector).First;
                
                var count = await targetElement.CountAsync();
                if (count > 0)
                {
                    var valor = await targetElement.GetAttributeAsync(atributo);
                    
                    if (!string.IsNullOrWhiteSpace(valor))
                    {
                        return valor.Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogTrace(ex, "Selector '{Selector}' falló para atributo '{Atributo}'", selector, atributo);
            }
        }

        return null;
    }

    /// <summary>
    /// Construye la URL de búsqueda con el término especificado.
    /// </summary>
    private string ConstruirUrlBusqueda(string terminoBusqueda)
    {
        var terminoCodificado = Uri.EscapeDataString(terminoBusqueda);
        var urlRelativa = string.Format(_productSearchUrl, terminoCodificado);
        return $"{_baseUrl}{urlRelativa}";
    }

    /// <summary>
    /// Limpia y parsea el texto del precio al formato decimal.
    /// Maneja el formato chileno: $12.990 = doce mil novecientos noventa pesos.
    /// </summary>
    private decimal LimpiarYParsearPrecio(string precioTexto)
    {
        try
        {
            string precioLimpio = precioTexto
                .Replace("$", "")
                .Replace("CLP", "")
                .Replace("UF", "")
                .Trim();

            precioLimpio = Regex.Replace(precioLimpio, @"[^\d.,]", "");
            precioLimpio = precioLimpio.Replace(".", ""); // Puntos = miles
            precioLimpio = precioLimpio.Replace(",", "."); // Coma = decimal

            if (string.IsNullOrWhiteSpace(precioLimpio))
            {
                return 0;
            }

            return decimal.Parse(precioLimpio, CultureInfo.InvariantCulture);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al parsear precio '{PrecioTexto}'", precioTexto);
            return 0;
        }
    }

    /// <summary>
    /// Determina si un producto está disponible basándose en el texto de stock.
    /// </summary>
    private bool DeterminarDisponibilidad(string? textoStock)
    {
        if (string.IsNullOrWhiteSpace(textoStock))
        {
            return true;
        }

        var textoLower = textoStock.ToLowerInvariant();

        var palabrasNoDisponible = new[]
        {
            "agotado", "sin stock", "no disponible",
            "fuera de stock", "out of stock"
        };

        return !palabrasNoDisponible.Any(p => textoLower.Contains(p));
    }

    /// <summary>
    /// Construye una URL completa desde una URL relativa.
    /// </summary>
    private string ConstruirUrlCompleta(string urlRelativa, string baseUrl)
    {
        if (string.IsNullOrWhiteSpace(urlRelativa))
        {
            return baseUrl;
        }

        if (urlRelativa.StartsWith("http://") || urlRelativa.StartsWith("https://"))
        {
            return urlRelativa;
        }

        if (urlRelativa.StartsWith("//"))
        {
            return $"https:{urlRelativa}";
        }

        if (urlRelativa.StartsWith("/"))
        {
            return $"{baseUrl}{urlRelativa}";
        }

        return $"{baseUrl}/{urlRelativa}";
    }
}
