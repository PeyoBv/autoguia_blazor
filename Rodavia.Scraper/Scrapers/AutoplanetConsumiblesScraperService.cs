using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Rodavia.Core.DTOs;
using System.Globalization;
using System.Net;

namespace Rodavia.Scraper.Scrapers;

/// <summary>
/// Servicio de scraping para buscar consumibles automotrices en Autoplanet.cl
/// Implementa extracción de productos, precios e imágenes desde el sitio web.
/// </summary>
public class AutoplanetConsumiblesScraperService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AutoplanetConsumiblesScraperService> _logger;
    private readonly IConfiguration _configuration;
    
    private const string BASE_URL = "https://www.autoplanet.cl/";
    private const string TIENDA_NOMBRE = "Autoplanet";
    private const int RATE_LIMIT_MS = 1500; // 1.5 segundos entre peticiones
    
    /// <summary>
    /// Constructor del servicio de scraping para Autoplanet
    /// </summary>
    /// <param name="httpClientFactory">Factory para crear clientes HTTP</param>
    /// <param name="logger">Logger para registrar eventos</param>
    /// <param name="configuration">Configuración de la aplicación</param>
    public AutoplanetConsumiblesScraperService(
        IHttpClientFactory httpClientFactory,
        ILogger<AutoplanetConsumiblesScraperService> logger,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Busca consumibles en Autoplanet.cl según término de búsqueda
    /// </summary>
    /// <param name="termino">Término de búsqueda (ej: "aceite 10w40", "filtro aire")</param>
    /// <param name="categoria">Categoría opcional para filtrar resultados</param>
    /// <returns>Lista de ofertas encontradas</returns>
    public async Task<List<OfertaDto>> BuscarConsumiblesAsync(string termino, string? categoria = null)
    {
        if (string.IsNullOrWhiteSpace(termino))
        {
            _logger.LogWarning("⚠️ Término de búsqueda vacío o nulo");
            return new List<OfertaDto>();
        }

        _logger.LogInformation("🔍 Iniciando búsqueda en Autoplanet: '{Termino}'", termino);

        try
        {
            var ofertas = new List<OfertaDto>();
            var searchUrl = $"{BASE_URL}?q={Uri.EscapeDataString(termino)}";

            if (!string.IsNullOrWhiteSpace(categoria))
            {
                searchUrl += $"&categoria={Uri.EscapeDataString(categoria)}";
                _logger.LogInformation("📂 Búsqueda filtrada por categoría: '{Categoria}'", categoria);
            }

            _logger.LogDebug("🌐 URL de búsqueda: {Url}", searchUrl);

            // Aplicar rate limiting antes de la petición
            await Task.Delay(RATE_LIMIT_MS);

            var httpClient = _httpClientFactory.CreateClient();
            
            // Configurar cliente para manejar SSL y headers realistas
            ConfigurarHttpClient(httpClient);

            var response = await httpClient.GetAsync(searchUrl);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("⚠️ Respuesta HTTP no exitosa: {StatusCode}", response.StatusCode);
                return new List<OfertaDto>();
            }

            var htmlContent = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(htmlContent))
            {
                _logger.LogWarning("⚠️ Contenido HTML vacío");
                return new List<OfertaDto>();
            }

            ofertas = ExtraerProductos(htmlContent);

            _logger.LogInformation("📊 Búsqueda completada. {Count} ofertas encontradas en Autoplanet", ofertas.Count);

            return ofertas;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "❌ Error HTTP al buscar en Autoplanet: {Message}", ex.Message);
            return new List<OfertaDto>();
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "❌ Timeout al buscar en Autoplanet: {Message}", ex.Message);
            return new List<OfertaDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error inesperado al buscar en Autoplanet: {Message}", ex.Message);
            return new List<OfertaDto>();
        }
    }

    /// <summary>
    /// Extrae productos del HTML de Autoplanet
    /// </summary>
    /// <param name="htmlContent">Contenido HTML de la página</param>
    /// <returns>Lista de ofertas extraídas</returns>
    private List<OfertaDto> ExtraerProductos(string htmlContent)
    {
        var ofertas = new List<OfertaDto>();

        try
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            // Selectores CSS para diferentes estructuras HTML de Autoplanet
            var selectoresProductos = new[]
            {
                "//div[contains(@class, 'product-item')]",
                "//div[contains(@class, 'product-card')]",
                "//article[contains(@class, 'product')]",
                "//li[contains(@class, 'product')]"
            };

            HtmlNodeCollection? productos = null;

            // Intentar diferentes selectores hasta encontrar productos
            foreach (var selector in selectoresProductos)
            {
                productos = htmlDoc.DocumentNode.SelectNodes(selector);
                if (productos != null && productos.Count > 0)
                {
                    _logger.LogDebug("✅ Productos encontrados con selector: {Selector}", selector);
                    break;
                }
            }

            if (productos == null || productos.Count == 0)
            {
                _logger.LogWarning("⚠️ No se encontraron productos con ningún selector");
                return ofertas;
            }

            _logger.LogDebug("📦 Procesando {Count} productos encontrados", productos.Count);

            foreach (var producto in productos)
            {
                try
                {
                    var oferta = ExtraerOferta(producto);
                    
                    if (oferta != null && oferta.Precio > 0)
                    {
                        ofertas.Add(oferta);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "⚠️ Error al extraer producto individual: {Message}", ex.Message);
                    // Continuar con el siguiente producto
                }
            }

            _logger.LogInformation("✅ Extracción completada: {Count} ofertas válidas", ofertas.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al parsear HTML: {Message}", ex.Message);
        }

        return ofertas;
    }

    /// <summary>
    /// Extrae una oferta individual desde un nodo HTML de producto
    /// </summary>
    /// <param name="productoNode">Nodo HTML del producto</param>
    /// <returns>OfertaDto o null si no se pudo extraer</returns>
    private OfertaDto? ExtraerOferta(HtmlNode productoNode)
    {
        // Selectores para nombre del producto
        var nombreSelectores = new[]
        {
            ".//div[contains(@class, 'product-name')]",
            ".//h2[contains(@class, 'product-title')]",
            ".//h3[contains(@class, 'product-title')]",
            ".//a[contains(@class, 'product-link')]",
            ".//span[contains(@class, 'product-name')]"
        };

        var nombre = ExtraerTexto(productoNode, nombreSelectores);
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return null;
        }

        // Selectores para precio
        var precioSelectores = new[]
        {
            ".//span[contains(@class, 'product-price')]",
            ".//div[contains(@class, 'price')]",
            ".//span[contains(@class, 'precio')]",
            ".//p[contains(@class, 'price')]"
        };

        var precioTexto = ExtraerTexto(productoNode, precioSelectores);
        var precio = ExtraerPrecio(precioTexto);

        if (precio <= 0)
        {
            _logger.LogDebug("⚠️ Producto sin precio válido: {Nombre}", nombre);
            return null;
        }

        // Selectores para URL del producto
        var urlSelectores = new[]
        {
            ".//a[contains(@class, 'product-link')]/@href",
            ".//a/@href"
        };

        var url = ExtraerAtributo(productoNode, urlSelectores);
        
        // Normalizar URL relativa a absoluta
        if (!string.IsNullOrWhiteSpace(url) && !url.StartsWith("http"))
        {
            url = new Uri(new Uri(BASE_URL), url).ToString();
        }

        // Selectores para imagen del producto
        var imagenSelectores = new[]
        {
            ".//img[contains(@class, 'product-image')]/@src",
            ".//img[contains(@class, 'product-img')]/@src",
            ".//img/@src"
        };

        var imagen = ExtraerAtributo(productoNode, imagenSelectores);

        // Normalizar URL de imagen relativa a absoluta
        if (!string.IsNullOrWhiteSpace(imagen) && !imagen.StartsWith("http"))
        {
            imagen = new Uri(new Uri(BASE_URL), imagen).ToString();
        }

        return new OfertaDto
        {
            ProductoNombre = nombre.Trim(),
            Precio = precio,
            UrlProductoEnTienda = url ?? BASE_URL,
            ProductoImagen = imagen,
            TiendaNombre = TIENDA_NOMBRE,
            TiendaLogo = $"{BASE_URL}logo.png",
            FechaActualizacion = DateTime.UtcNow,
            EsDisponible = true
        };
    }

    /// <summary>
    /// Extrae texto de un nodo usando múltiples selectores CSS
    /// </summary>
    /// <param name="nodo">Nodo HTML padre</param>
    /// <param name="selectores">Array de selectores CSS a intentar</param>
    /// <returns>Texto extraído o string vacío</returns>
    private string ExtraerTexto(HtmlNode nodo, string[] selectores)
    {
        foreach (var selector in selectores)
        {
            var elemento = nodo.SelectSingleNode(selector);
            if (elemento != null)
            {
                var texto = HtmlEntity.DeEntitize(elemento.InnerText).Trim();
                if (!string.IsNullOrWhiteSpace(texto))
                {
                    return texto;
                }
            }
        }
        return string.Empty;
    }

    /// <summary>
    /// Extrae atributo de un nodo usando múltiples selectores XPath
    /// </summary>
    /// <param name="nodo">Nodo HTML padre</param>
    /// <param name="selectores">Array de selectores XPath a intentar</param>
    /// <returns>Valor del atributo o null</returns>
    private string? ExtraerAtributo(HtmlNode nodo, string[] selectores)
    {
        foreach (var selector in selectores)
        {
            var atributo = nodo.SelectSingleNode(selector);
            if (atributo != null)
            {
                var valor = atributo.GetAttributeValue("href", string.Empty) ?? atributo.GetAttributeValue("src", string.Empty);
                if (!string.IsNullOrWhiteSpace(valor))
                {
                    return valor.Trim();
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Extrae precio desde un string con formato chileno
    /// Ejemplos: "$25.990", "25990", "$1.250.000"
    /// </summary>
    /// <param name="precioTexto">Texto con el precio</param>
    /// <returns>Precio como decimal</returns>
    private decimal ExtraerPrecio(string? precioTexto)
    {
        if (string.IsNullOrWhiteSpace(precioTexto))
        {
            return 0;
        }

        try
        {
            // Remover símbolos de moneda y espacios
            var precioLimpio = precioTexto
                .Replace("$", "")
                .Replace("CLP", "")
                .Replace(" ", "")
                .Replace(".", "") // Remover separadores de miles
                .Replace(",", ".") // Convertir coma decimal a punto
                .Trim();

            if (decimal.TryParse(precioLimpio, NumberStyles.Any, CultureInfo.InvariantCulture, out var precio))
            {
                return precio;
            }

            _logger.LogDebug("⚠️ No se pudo parsear precio: '{PrecioTexto}'", precioTexto);
            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "⚠️ Error al extraer precio de: '{PrecioTexto}'", precioTexto);
            return 0;
        }
    }

    /// <summary>
    /// Configura el HttpClient con headers y configuración SSL
    /// </summary>
    /// <param name="httpClient">Cliente HTTP a configurar</param>
    private void ConfigurarHttpClient(HttpClient httpClient)
    {
        // Headers para simular navegador real
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add("User-Agent", 
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        httpClient.DefaultRequestHeaders.Add("Accept", 
            "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
        httpClient.DefaultRequestHeaders.Add("Accept-Language", "es-CL,es;q=0.9,en;q=0.8");
        httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
        httpClient.DefaultRequestHeaders.Add("DNT", "1");
        httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
        httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
        httpClient.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");

        // Timeout de 30 segundos
        httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    /// <summary>
    /// Valida si Autoplanet.cl está disponible
    /// </summary>
    /// <returns>True si el sitio responde correctamente</returns>
    public async Task<bool> ValidarDisponibilidadAsync()
    {
        try
        {
            _logger.LogInformation("🔍 Validando disponibilidad de Autoplanet...");

            var httpClient = _httpClientFactory.CreateClient();
            ConfigurarHttpClient(httpClient);

            var response = await httpClient.GetAsync(BASE_URL);
            var disponible = response.IsSuccessStatusCode;

            if (disponible)
            {
                _logger.LogInformation("✅ Autoplanet está disponible");
            }
            else
            {
                _logger.LogWarning("⚠️ Autoplanet no está disponible. Status: {StatusCode}", response.StatusCode);
            }

            return disponible;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al validar disponibilidad de Autoplanet: {Message}", ex.Message);
            return false;
        }
    }
}
