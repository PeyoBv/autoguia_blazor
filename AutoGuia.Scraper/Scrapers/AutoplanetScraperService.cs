using AutoGuia.Scraper.DTOs;
using AutoGuia.Scraper.Interfaces;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;

namespace AutoGuia.Scraper.Scrapers;

/// <summary>
/// Scraper para Autoplanet.cl usando parsing HTML con HtmlAgilityPack.
/// Extrae información de productos desde el HTML de la página de búsqueda.
/// </summary>
public class AutoplanetScraperService : IScraper
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AutoplanetScraperService> _logger;
    
    // Configuración de URLs
    private const string BaseUrl = "https://www.autoplanet.cl";
    private const string SearchUrlTemplate = "/busqueda?q={0}";
    
    public string TiendaNombre => "Autoplanet";
    public string TipoScraper => "HTML";
    public bool EstaHabilitado => true;

    public AutoplanetScraperService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<AutoplanetScraperService> logger)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<IEnumerable<OfertaDto>> ScrapearProductosAsync(
        string numeroDeParte, 
        int tiendaId,
        CancellationToken cancellationToken = default)
    {
        var ofertas = new List<OfertaDto>();
        
        try
        {
            _logger.LogInformation("🔍 Iniciando scraping en Autoplanet para número de parte: {NumeroDeParte}", numeroDeParte);
            
            // 1. Validar entrada
            if (string.IsNullOrWhiteSpace(numeroDeParte))
            {
                _logger.LogWarning("⚠️ Número de parte vacío o nulo");
                return ofertas;
            }

            // 2. Crear HttpClient con configuración
            var httpClient = _httpClientFactory.CreateClient("ScraperClient");
            ConfigurarHttpClient(httpClient);

            // 3. Construir URL de búsqueda
            var searchUrl = ConstruirUrlBusqueda(numeroDeParte);
            _logger.LogDebug("📡 URL de búsqueda: {Url}", searchUrl);

            // 4. Descargar HTML de la página
            var html = await DescargarHtmlAsync(httpClient, searchUrl, cancellationToken);
            
            if (string.IsNullOrEmpty(html))
            {
                _logger.LogWarning("⚠️ No se pudo descargar el HTML de Autoplanet");
                return new List<OfertaDto>
                {
                    CrearOfertaConError(tiendaId, "No se pudo descargar la página")
                };
            }

            // 5. Parsear HTML con HtmlAgilityPack
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            // 6. Extraer nodos de productos usando XPath
            var productNodes = ExtraerNodosProductos(htmlDoc);

            if (productNodes == null || productNodes.Count == 0)
            {
                _logger.LogInformation("ℹ️ No se encontraron productos en Autoplanet para: {NumeroDeParte}", numeroDeParte);
                return ofertas;
            }

            _logger.LogInformation("📦 Se encontraron {Count} productos en el HTML", productNodes.Count);

            // 7. Procesar cada nodo de producto
            var maxResults = _configuration.GetValue<int>("ScrapingSettings:Autoplanet:MaxResults", 10);
            var resultadosProcesados = 0;

            foreach (var productNode in productNodes.Take(maxResults))
            {
                try
                {
                    var oferta = ExtraerOfertaDeNodo(productNode, tiendaId);
                    
                    if (oferta != null)
                    {
                        ofertas.Add(oferta);
                        resultadosProcesados++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "⚠️ Error al procesar nodo de producto en Autoplanet");
                }
            }

            _logger.LogInformation("✅ Scraping completado: {Procesados}/{Total} ofertas procesadas de Autoplanet", 
                resultadosProcesados, productNodes.Count);

            return ofertas;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "❌ Error de red al conectar con Autoplanet");
            return new List<OfertaDto>
            {
                CrearOfertaConError(tiendaId, $"Error de conexión: {ex.Message}")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error inesperado al scrapear Autoplanet");
            return new List<OfertaDto>
            {
                CrearOfertaConError(tiendaId, $"Error inesperado: {ex.Message}")
            };
        }
    }

    /// <inheritdoc />
    public bool PuedeScrapearTienda(string tiendaNombre)
    {
        return tiendaNombre.Equals(TiendaNombre, StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public async Task<Dictionary<string, string>> ObtenerConfiguracionAsync()
    {
        return await Task.FromResult(new Dictionary<string, string>
        {
            ["BaseUrl"] = BaseUrl,
            ["Timeout"] = "30",
            ["MaxResults"] = _configuration.GetValue<string>("ScrapingSettings:Autoplanet:MaxResults", "10"),
            ["UserAgent"] = "AutoGuia-Scraper/1.0",
            ["TipoRespuesta"] = "HTML",
            ["RequiereJavaScript"] = "false"
        });
    }

    #region Métodos Privados

    /// <summary>
    /// Configura el HttpClient con headers apropiados para simular un navegador.
    /// </summary>
    private void ConfigurarHttpClient(HttpClient httpClient)
    {
        // User-Agent de navegador real para evitar bloqueos
        httpClient.DefaultRequestHeaders.Add("User-Agent", 
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        
        // Accept headers para simular navegador
        httpClient.DefaultRequestHeaders.Add("Accept", 
            "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
        httpClient.DefaultRequestHeaders.Add("Accept-Language", "es-CL,es;q=0.9,en;q=0.8");
        httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
        
        // Timeout
        var timeoutSeconds = _configuration.GetValue<int>("ScrapingSettings:Autoplanet:TimeoutSeconds", 30);
        httpClient.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
    }

    /// <summary>
    /// Construye la URL de búsqueda para Autoplanet.
    /// </summary>
    private string ConstruirUrlBusqueda(string numeroDeParte)
    {
        // Limpiar y codificar el número de parte
        var queryEncoded = HttpUtility.UrlEncode(numeroDeParte.Trim());
        
        // Construir URL completa
        var searchPath = string.Format(SearchUrlTemplate, queryEncoded);
        return $"{BaseUrl}{searchPath}";
    }

    /// <summary>
    /// Descarga el HTML de una URL con manejo de errores.
    /// </summary>
    private async Task<string?> DescargarHtmlAsync(
        HttpClient httpClient, 
        string url, 
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await httpClient.GetAsync(url, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("⚠️ Autoplanet devolvió código {StatusCode}: {ReasonPhrase}", 
                    response.StatusCode, response.ReasonPhrase);
                return null;
            }

            var html = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogDebug("📄 HTML descargado: {Length} caracteres", html.Length);
            
            return html;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al descargar HTML de: {Url}", url);
            return null;
        }
    }

    /// <summary>
    /// Extrae los nodos de productos del documento HTML usando selectores XPath.
    /// </summary>
    private HtmlNodeCollection? ExtraerNodosProductos(HtmlDocument htmlDoc)
    {
        // Intentar múltiples selectores ya que la estructura puede variar
        var selectores = new[]
        {
            "//div[contains(@class, 'product-card')]",           // Selector común
            "//div[contains(@class, 'producto')]",               // Variación 1
            "//article[contains(@class, 'product')]",            // Variación 2
            "//div[contains(@class, 'item-product')]",           // Variación 3
            "//div[@class='product']",                           // Selector específico
            "//div[contains(@class, 'product-item')]",           // Variación 4
            "//li[contains(@class, 'product')]"                  // Si usa lista
        };

        foreach (var selector in selectores)
        {
            var nodes = htmlDoc.DocumentNode.SelectNodes(selector);
            if (nodes != null && nodes.Count > 0)
            {
                _logger.LogDebug("✅ Encontrados nodos con selector: {Selector}", selector);
                return nodes;
            }
        }

        _logger.LogWarning("⚠️ No se encontraron productos con ningún selector conocido");
        return null;
    }

    /// <summary>
    /// Extrae información de oferta de un nodo HTML de producto.
    /// </summary>
    private OfertaDto? ExtraerOfertaDeNodo(HtmlNode productNode, int tiendaId)
    {
        try
        {
            // 1. Extraer título/nombre del producto
            var titulo = ExtraerTexto(productNode, new[]
            {
                ".//h2[contains(@class, 'product-title')]",
                ".//h3[contains(@class, 'product-name')]",
                ".//a[contains(@class, 'product-link')]",
                ".//div[contains(@class, 'product-title')]",
                ".//span[contains(@class, 'title')]"
            });

            // 2. Extraer precio
            var precioTexto = ExtraerTexto(productNode, new[]
            {
                ".//span[contains(@class, 'price')]",
                ".//div[contains(@class, 'product-price')]",
                ".//p[contains(@class, 'price')]",
                ".//span[contains(@class, 'precio')]",
                ".//strong[contains(@class, 'price')]"
            });

            // 3. Extraer URL del producto
            var urlRelativa = ExtraerAtributo(productNode, new[]
            {
                ".//a[contains(@class, 'product-link')]",
                ".//a[contains(@href, '/producto')]",
                ".//a"
            }, "href");

            // 4. Extraer imagen
            var imagenUrl = ExtraerAtributo(productNode, new[]
            {
                ".//img[contains(@class, 'product-image')]",
                ".//img[@class='img-fluid']",
                ".//img"
            }, "src");

            // 5. Extraer información de stock
            var stockTexto = ExtraerTexto(productNode, new[]
            {
                ".//span[contains(@class, 'stock')]",
                ".//div[contains(@class, 'availability')]",
                ".//p[contains(@class, 'stock')]"
            });

            // Validar que tenemos datos mínimos necesarios
            if (string.IsNullOrEmpty(titulo) || string.IsNullOrEmpty(precioTexto))
            {
                _logger.LogDebug("⚠️ Producto sin título o precio, saltando...");
                return null;
            }

            // Limpiar y parsear precio
            var precio = LimpiarYParsearPrecio(precioTexto);
            if (precio <= 0)
            {
                _logger.LogDebug("⚠️ Precio inválido: {PrecioTexto}", precioTexto);
                return null;
            }

            // Construir URL completa
            var urlCompleta = ConstruirUrlCompleta(urlRelativa);

            // Determinar disponibilidad de stock
            var stockDisponible = DeterminarDisponibilidad(stockTexto);

            // Construir URL completa de imagen
            var imagenCompleta = ConstruirUrlCompleta(imagenUrl);

            return new OfertaDto
            {
                TiendaId = tiendaId,
                TiendaNombre = TiendaNombre,
                Precio = precio,
                UrlProducto = urlCompleta ?? $"{BaseUrl}/productos",
                StockDisponible = stockDisponible,
                Descripcion = LimpiarTexto(titulo),
                ImagenUrl = imagenCompleta,
                FechaScrapeo = DateTime.UtcNow,
                TieneErrores = false
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error al extraer oferta de nodo");
            return null;
        }
    }

    /// <summary>
    /// Extrae texto de un nodo usando múltiples selectores XPath.
    /// </summary>
    private string? ExtraerTexto(HtmlNode node, string[] selectores)
    {
        foreach (var selector in selectores)
        {
            try
            {
                var targetNode = node.SelectSingleNode(selector);
                if (targetNode != null)
                {
                    var texto = HttpUtility.HtmlDecode(targetNode.InnerText).Trim();
                    if (!string.IsNullOrWhiteSpace(texto))
                    {
                        return texto;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug("Error con selector {Selector}: {Error}", selector, ex.Message);
            }
        }
        return null;
    }

    /// <summary>
    /// Extrae un atributo de un nodo usando múltiples selectores XPath.
    /// </summary>
    private string? ExtraerAtributo(HtmlNode node, string[] selectores, string atributo)
    {
        foreach (var selector in selectores)
        {
            try
            {
                var targetNode = node.SelectSingleNode(selector);
                if (targetNode != null)
                {
                    var valor = targetNode.GetAttributeValue(atributo, null);
                    if (!string.IsNullOrWhiteSpace(valor))
                    {
                        return valor;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug("Error extrayendo atributo {Atributo} con selector {Selector}: {Error}", 
                    atributo, selector, ex.Message);
            }
        }
        return null;
    }

    /// <summary>
    /// Limpia y parsea un precio en formato chileno (ej: "$12.990", "$1.234.567").
    /// </summary>
    private decimal LimpiarYParsearPrecio(string precioTexto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(precioTexto))
                return 0;

            // 1. Eliminar símbolos de moneda y espacios
            var precioLimpio = precioTexto
                .Replace("$", "")
                .Replace("CLP", "")
                .Replace("clp", "")
                .Replace(" ", "")
                .Trim();

            // 2. En Chile, el punto (.) es separador de miles, NO decimal
            // Eliminar todos los puntos
            precioLimpio = precioLimpio.Replace(".", "");

            // 3. Si tiene coma, es separador decimal (raro en Chile pero posible)
            precioLimpio = precioLimpio.Replace(",", ".");

            // 4. Intentar parsear
            if (decimal.TryParse(precioLimpio, NumberStyles.Any, CultureInfo.InvariantCulture, out var precio))
            {
                return precio;
            }

            _logger.LogDebug("⚠️ No se pudo parsear precio: {PrecioTexto} -> {PrecioLimpio}", 
                precioTexto, precioLimpio);
            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error al limpiar precio: {PrecioTexto}", precioTexto);
            return 0;
        }
    }

    /// <summary>
    /// Determina si un producto está disponible basado en texto de stock.
    /// </summary>
    private bool DeterminarDisponibilidad(string? stockTexto)
    {
        if (string.IsNullOrWhiteSpace(stockTexto))
            return true; // Asumimos disponible si no hay info

        var textoLower = stockTexto.ToLower();

        // Palabras que indican NO disponible
        var palabrasNoDisponible = new[] 
        { 
            "agotado", "sin stock", "no disponible", "out of stock", 
            "sin existencias", "sin disponibilidad" 
        };

        foreach (var palabra in palabrasNoDisponible)
        {
            if (textoLower.Contains(palabra))
                return false;
        }

        // Palabras que indican disponible
        var palabrasDisponible = new[] 
        { 
            "disponible", "en stock", "available", "en existencia" 
        };

        foreach (var palabra in palabrasDisponible)
        {
            if (textoLower.Contains(palabra))
                return true;
        }

        // Por defecto, asumimos disponible
        return true;
    }

    /// <summary>
    /// Construye una URL completa desde una URL relativa.
    /// </summary>
    private string? ConstruirUrlCompleta(string? urlRelativa)
    {
        if (string.IsNullOrWhiteSpace(urlRelativa))
            return null;

        // Si ya es URL completa, retornar tal cual
        if (urlRelativa.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            urlRelativa.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return urlRelativa;
        }

        // Si empieza con //, agregar https:
        if (urlRelativa.StartsWith("//"))
        {
            return $"https:{urlRelativa}";
        }

        // Si empieza con /, es relativa a la raíz
        if (urlRelativa.StartsWith("/"))
        {
            return $"{BaseUrl}{urlRelativa}";
        }

        // Caso general: agregar base URL
        return $"{BaseUrl}/{urlRelativa}";
    }

    /// <summary>
    /// Limpia y trunca texto para la base de datos.
    /// </summary>
    private string? LimpiarTexto(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return null;

        // Eliminar espacios extras y saltos de línea
        var limpio = Regex.Replace(texto.Trim(), @"\s+", " ");

        // Decodificar entidades HTML
        limpio = HttpUtility.HtmlDecode(limpio);

        // Truncar si es muy largo
        const int maxLength = 500;
        if (limpio.Length > maxLength)
        {
            limpio = limpio.Substring(0, maxLength - 3) + "...";
        }

        return limpio;
    }

    /// <summary>
    /// Crea una oferta con error para registro.
    /// </summary>
    private OfertaDto CrearOfertaConError(int tiendaId, string mensajeError)
    {
        return new OfertaDto
        {
            TiendaId = tiendaId,
            TiendaNombre = TiendaNombre,
            TieneErrores = true,
            MensajeError = mensajeError,
            FechaScrapeo = DateTime.UtcNow
        };
    }

    #endregion
}
