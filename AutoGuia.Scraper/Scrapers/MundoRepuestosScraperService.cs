using AutoGuia.Scraper.DTOs;
using AutoGuia.Scraper.Interfaces;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text.RegularExpressions;

namespace AutoGuia.Scraper.Scrapers;

/// <summary>
/// Scraper para MundoRepuestos.cl que utiliza HtmlAgilityPack para parsear HTML.
/// Implementa múltiples selectores de respaldo para mayor robustez ante cambios en la estructura del sitio.
/// </summary>
public class MundoRepuestosScraperService : IScraper
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<MundoRepuestosScraperService> _logger;
    
    private readonly string _baseUrl;
    private readonly string _productSearchUrl;
    private readonly int _maxResults;
    private readonly int _timeoutSeconds;
    private readonly int _requestDelayMs;

    // 🎯 Selectores para encontrar los contenedores de productos
    // MundoRepuestos suele usar clases como "product-item", "producto", "article-product"
    private readonly string[] _selectoresNodoProducto = new[]
    {
        "//div[contains(@class, 'product-item')]",
        "//article[contains(@class, 'product')]",
        "//div[contains(@class, 'producto-card')]",
        "//li[contains(@class, 'product')]",
        "//div[@class='grid-item']//article",
        "//div[contains(@class, 'item-product')]",
        "//div[@itemtype='http://schema.org/Product']"
    };

    // 🎯 Selectores para el título/nombre del producto
    private readonly string[] _selectoresTitulo = new[]
    {
        ".//h3[@class='product-name']",
        ".//h2[@class='product-title']",
        ".//a[@class='product-link']",
        ".//div[@class='product-info']//h4",
        ".//span[@itemprop='name']",
        ".//h3[contains(@class, 'title')]",
        ".//a[contains(@class, 'product-name')]"
    };

    // 🎯 Selectores para el precio
    private readonly string[] _selectoresPrecio = new[]
    {
        ".//span[@class='price']",
        ".//div[@class='product-price']//span",
        ".//span[@itemprop='price']",
        ".//p[@class='precio']",
        ".//span[contains(@class, 'price-current')]",
        ".//div[contains(@class, 'price-box')]//span",
        ".//span[@class='woocommerce-Price-amount amount']"
    };

    // 🎯 Selectores para la URL del producto
    private readonly string[] _selectoresUrl = new[]
    {
        ".//a[@class='product-link']/@href",
        ".//h3//a/@href",
        ".//h2//a/@href",
        ".//a[contains(@class, 'product')]/@href",
        "./@data-product-url",
        ".//a[@itemprop='url']/@href"
    };

    // 🎯 Selectores para la imagen del producto
    private readonly string[] _selectoresImagen = new[]
    {
        ".//img[@class='product-image']/@src",
        ".//img[@itemprop='image']/@src",
        ".//div[@class='product-img']//img/@src",
        ".//img[contains(@class, 'product')]/@src",
        ".//a[@class='product-link']//img/@src",
        ".//img/@data-src",
        ".//picture//img/@src"
    };

    // 🎯 Selectores para el stock/disponibilidad
    private readonly string[] _selectoresStock = new[]
    {
        ".//span[@class='stock']",
        ".//div[@class='availability']",
        ".//p[contains(@class, 'stock')]",
        ".//span[@itemprop='availability']",
        ".//div[@class='product-availability']",
        ".//span[contains(@class, 'disponibilidad')]"
    };

    // 🎯 Selectores para SKU/Código de producto
    private readonly string[] _selectoresSku = new[]
    {
        ".//span[@class='sku']",
        ".//span[@itemprop='sku']",
        "./@data-product-id",
        "./@data-sku",
        ".//p[@class='codigo']",
        ".//span[contains(@class, 'product-code')]"
    };

    public string TiendaNombre => "MundoRepuestos";
    public string TipoScraper => "HTML";
    public bool EstaHabilitado { get; private set; }

    /// <summary>
    /// Constructor que inicializa el scraper con las dependencias necesarias.
    /// </summary>
    public MundoRepuestosScraperService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<MundoRepuestosScraperService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;

        // Cargar configuración desde appsettings.json
        _baseUrl = _configuration["Stores:MundoRepuestos:BaseUrl"] ?? "https://www.mundorepuestos.cl";
        _productSearchUrl = _configuration["Stores:MundoRepuestos:ProductSearchUrl"] ?? "/search?q={0}";
        _maxResults = _configuration.GetValue<int>("ScrapingSettings:MundoRepuestos:MaxResults", 10);
        _timeoutSeconds = _configuration.GetValue<int>("ScrapingSettings:MundoRepuestos:TimeoutSeconds", 30);
        _requestDelayMs = _configuration.GetValue<int>("Stores:MundoRepuestos:RequestDelayMs", 1500);
        
        EstaHabilitado = _configuration.GetValue<bool>("Stores:MundoRepuestos:Enabled", true);

        _logger.LogInformation(
            "MundoRepuestosScraperService inicializado. BaseUrl: {BaseUrl}, MaxResults: {MaxResults}, Habilitado: {Enabled}",
            _baseUrl, _maxResults, EstaHabilitado);
    }

    /// <summary>
    /// Verifica si este scraper puede procesar la tienda especificada.
    /// </summary>
    public bool PuedeScrapearTienda(string nombreTienda)
    {
        return nombreTienda.Equals("MundoRepuestos", StringComparison.OrdinalIgnoreCase) ||
               nombreTienda.Equals("Mundo Repuestos", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Obtiene la configuración actual del scraper.
    /// </summary>
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
    /// Scrapea productos de MundoRepuestos basándose en un número de parte o término de búsqueda.
    /// </summary>
    public async Task<IEnumerable<OfertaDto>> ScrapearProductosAsync(
        string numeroDeParte,
        int tiendaId,
        CancellationToken cancellationToken = default)
    {
        if (!EstaHabilitado)
        {
            _logger.LogWarning("Scraper de MundoRepuestos está deshabilitado");
            return Enumerable.Empty<OfertaDto>();
        }

        if (string.IsNullOrWhiteSpace(numeroDeParte))
        {
            _logger.LogWarning("Número de parte vacío o nulo");
            return Enumerable.Empty<OfertaDto>();
        }

        var ofertas = new List<OfertaDto>();

        try
        {
            _logger.LogInformation(
                "Iniciando scraping en MundoRepuestos para '{NumeroDeParte}'",
                numeroDeParte);

            // 1. Construir la URL de búsqueda
            var searchUrl = ConstruirUrlBusqueda(numeroDeParte);
            _logger.LogDebug("URL de búsqueda: {SearchUrl}", searchUrl);

            // 2. Descargar el HTML
            var htmlContent = await DescargarHtmlAsync(searchUrl, cancellationToken);
            
            if (string.IsNullOrWhiteSpace(htmlContent))
            {
                _logger.LogWarning("No se pudo obtener contenido HTML de MundoRepuestos");
                return ofertas;
            }

            // 3. Parsear el HTML con HtmlAgilityPack
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(htmlContent);

            // 4. Extraer los nodos de productos
            var nodosProductos = ExtraerNodosProductos(htmlDocument);
            
            if (nodosProductos == null || !nodosProductos.Any())
            {
                _logger.LogInformation(
                    "No se encontraron productos en MundoRepuestos para '{NumeroDeParte}'",
                    numeroDeParte);
                return ofertas;
            }

            _logger.LogInformation(
                "Se encontraron {Count} nodos de productos en MundoRepuestos",
                nodosProductos.Count());

            // 5. Extraer datos de cada producto (limitado por MaxResults)
            var nodosLimitados = nodosProductos.Take(_maxResults);
            
            foreach (var nodo in nodosLimitados)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogWarning("Scraping cancelado por el usuario");
                    break;
                }

                try
                {
                    var oferta = ExtraerOfertaDeNodo(nodo, tiendaId);
                    
                    if (oferta != null)
                    {
                        ofertas.Add(oferta);
                        _logger.LogDebug(
                            "Oferta extraída: {Descripcion} - ${Precio}",
                            oferta.Descripcion,
                            oferta.Precio);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al extraer datos de un nodo de producto");
                    
                    // Agregar oferta con error para tracking
                    ofertas.Add(new OfertaDto
                    {
                        TiendaId = tiendaId,
                        TiendaNombre = TiendaNombre,
                        TieneErrores = true,
                        MensajeError = $"Error al parsear producto: {ex.Message}"
                    });
                }

                // Pequeño delay entre procesamiento de nodos
                await Task.Delay(100, cancellationToken);
            }

            _logger.LogInformation(
                "Scraping completado. Se extrajeron {Count} ofertas de MundoRepuestos",
                ofertas.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Error crítico durante el scraping de MundoRepuestos para '{NumeroDeParte}'",
                numeroDeParte);

            ofertas.Add(new OfertaDto
            {
                TiendaId = tiendaId,
                TiendaNombre = TiendaNombre,
                TieneErrores = true,
                MensajeError = $"Error crítico en scraping: {ex.Message}"
            });
        }

        return ofertas;
    }

    /// <summary>
    /// Construye la URL de búsqueda con el término especificado.
    /// </summary>
    private string ConstruirUrlBusqueda(string terminoBusqueda)
    {
        // Codificar el término de búsqueda para URL
        var terminoCodificado = Uri.EscapeDataString(terminoBusqueda);
        
        // Reemplazar el placeholder {0} con el término
        var urlRelativa = string.Format(_productSearchUrl, terminoCodificado);
        
        // Construir URL completa
        return $"{_baseUrl}{urlRelativa}";
    }

    /// <summary>
    /// Descarga el contenido HTML de la URL especificada.
    /// </summary>
    private async Task<string> DescargarHtmlAsync(string url, CancellationToken cancellationToken)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            
            // Configurar timeout y headers
            httpClient.Timeout = TimeSpan.FromSeconds(_timeoutSeconds);
            ConfigurarHttpClient(httpClient);

            _logger.LogDebug("Descargando HTML desde: {Url}", url);

            var response = await httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            
            _logger.LogDebug("HTML descargado exitosamente. Tamaño: {Size} bytes", content.Length);
            
            return content;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error HTTP al descargar HTML de {Url}", url);
            throw;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Timeout al descargar HTML de {Url}", url);
            throw;
        }
    }

    /// <summary>
    /// Configura los headers HTTP para simular un navegador real y evitar bloqueos.
    /// </summary>
    private void ConfigurarHttpClient(HttpClient client)
    {
        // User-Agent de navegador real (Chrome en Windows 10)
        client.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
            "(KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");

        // Headers adicionales para parecer más legítimo
        client.DefaultRequestHeaders.Add("Accept",
            "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
        
        client.DefaultRequestHeaders.Add("Accept-Language", "es-CL,es;q=0.9,en;q=0.8");
        client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
        client.DefaultRequestHeaders.Add("DNT", "1");
        client.DefaultRequestHeaders.Add("Connection", "keep-alive");
        client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
    }

    /// <summary>
    /// Extrae los nodos de productos del documento HTML usando múltiples selectores.
    /// </summary>
    private IEnumerable<HtmlNode>? ExtraerNodosProductos(HtmlDocument document)
    {
        foreach (var selector in _selectoresNodoProducto)
        {
            try
            {
                var nodos = document.DocumentNode.SelectNodes(selector);
                
                if (nodos != null && nodos.Any())
                {
                    _logger.LogDebug(
                        "Selector exitoso: '{Selector}' encontró {Count} nodos",
                        selector,
                        nodos.Count);
                    
                    return nodos;
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Selector '{Selector}' falló", selector);
            }
        }

        _logger.LogWarning("Ningún selector de nodos de producto fue exitoso");
        return null;
    }

    /// <summary>
    /// Extrae los datos de una oferta desde un nodo HTML de producto.
    /// </summary>
    private OfertaDto? ExtraerOfertaDeNodo(HtmlNode nodo, int tiendaId)
    {
        try
        {
            // Extraer título/descripción
            var titulo = ExtraerTexto(nodo, _selectoresTitulo);
            
            if (string.IsNullOrWhiteSpace(titulo))
            {
                _logger.LogWarning("No se pudo extraer el título del producto");
                return null;
            }

            // Extraer precio
            var precioTexto = ExtraerTexto(nodo, _selectoresPrecio);
            decimal precio = 0;
            
            if (!string.IsNullOrWhiteSpace(precioTexto))
            {
                precio = LimpiarYParsearPrecio(precioTexto);
            }
            else
            {
                _logger.LogWarning("No se pudo extraer el precio del producto: {Titulo}", titulo);
            }

            // Extraer URL del producto
            var urlRelativa = ExtraerAtributo(nodo, _selectoresUrl);
            var urlCompleta = !string.IsNullOrWhiteSpace(urlRelativa)
                ? ConstruirUrlCompleta(urlRelativa, _baseUrl)
                : null;

            // Extraer imagen
            var imagenRelativa = ExtraerAtributo(nodo, _selectoresImagen);
            var imagenCompleta = !string.IsNullOrWhiteSpace(imagenRelativa)
                ? ConstruirUrlCompleta(imagenRelativa, _baseUrl)
                : null;

            // Extraer stock/disponibilidad
            var textoStock = ExtraerTexto(nodo, _selectoresStock);
            var stockDisponible = DeterminarDisponibilidad(textoStock);

            // Extraer SKU/código de producto
            var sku = ExtraerTexto(nodo, _selectoresSku);
            int productoId = 0;
            if (!string.IsNullOrWhiteSpace(sku) && int.TryParse(sku, out var skuInt))
            {
                productoId = skuInt;
            }

            // Crear DTO de oferta
            var oferta = new OfertaDto
            {
                ProductoId = productoId,
                TiendaId = tiendaId,
                Precio = precio,
                UrlProducto = urlCompleta,
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
            _logger.LogError(ex, "Error al extraer oferta del nodo HTML");
            return null;
        }
    }

    /// <summary>
    /// Extrae texto de un nodo usando múltiples selectores de respaldo.
    /// </summary>
    private string? ExtraerTexto(HtmlNode nodo, string[] selectores)
    {
        foreach (var selector in selectores)
        {
            try
            {
                var targetNode = nodo.SelectSingleNode(selector);
                
                if (targetNode != null)
                {
                    var texto = targetNode.InnerText?.Trim();
                    
                    if (!string.IsNullOrWhiteSpace(texto))
                    {
                        // Limpiar espacios múltiples y saltos de línea
                        texto = Regex.Replace(texto, @"\s+", " ");
                        return texto;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogTrace(ex, "Selector de texto '{Selector}' falló", selector);
            }
        }

        return null;
    }

    /// <summary>
    /// Extrae el valor de un atributo usando múltiples selectores de respaldo.
    /// </summary>
    private string? ExtraerAtributo(HtmlNode nodo, string[] selectores)
    {
        foreach (var selector in selectores)
        {
            try
            {
                // Los selectores de atributos terminan en /@atributo
                if (selector.Contains("/@"))
                {
                    var partes = selector.Split(new[] { "/@" }, StringSplitOptions.None);
                    var xpathNodo = partes[0];
                    var nombreAtributo = partes[1];

                    var targetNode = nodo.SelectSingleNode(xpathNodo);
                    
                    if (targetNode != null)
                    {
                        var valorAtributo = targetNode.GetAttributeValue(nombreAtributo, null);
                        
                        if (!string.IsNullOrWhiteSpace(valorAtributo))
                        {
                            return valorAtributo.Trim();
                        }
                    }
                }
                else
                {
                    // Selector directo sin atributo
                    var targetNode = nodo.SelectSingleNode(selector);
                    
                    if (targetNode != null)
                    {
                        var texto = targetNode.InnerText?.Trim();
                        
                        if (!string.IsNullOrWhiteSpace(texto))
                        {
                            return texto;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogTrace(ex, "Selector de atributo '{Selector}' falló", selector);
            }
        }

        return null;
    }

    /// <summary>
    /// Limpia y parsea el texto del precio al formato decimal.
    /// Maneja el formato chileno: $12.990 = doce mil novecientos noventa pesos.
    /// </summary>
    private decimal LimpiarYParsearPrecio(string precioTexto)
    {
        try
        {
            // Remover símbolos de moneda, espacios y texto adicional
            string precioLimpio = precioTexto
                .Replace("$", "")
                .Replace("CLP", "")
                .Replace("UF", "")
                .Replace("USD", "")
                .Trim();

            // Remover cualquier texto que no sea número, punto o coma
            precioLimpio = Regex.Replace(precioLimpio, @"[^\d.,]", "");

            // En Chile: los puntos son separadores de miles (NO decimales)
            // Ejemplo: $12.990 = doce mil novecientos noventa
            precioLimpio = precioLimpio.Replace(".", "");

            // Las comas son separadores decimales (aunque raros en precios chilenos)
            precioLimpio = precioLimpio.Replace(",", ".");

            if (string.IsNullOrWhiteSpace(precioLimpio))
            {
                _logger.LogWarning("Precio limpio está vacío después del procesamiento");
                return 0;
            }

            return decimal.Parse(precioLimpio, CultureInfo.InvariantCulture);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Error al parsear precio '{PrecioTexto}'", 
                precioTexto);
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
            return true; // Por defecto, asumir disponible
        }

        var textoLower = textoStock.ToLowerInvariant();

        // Palabras clave que indican NO disponible
        var palabrasNoDisponible = new[]
        {
            "agotado",
            "sin stock",
            "no disponible",
            "fuera de stock",
            "out of stock",
            "temporalmente no disponible",
            "próximamente",
            "consultar disponibilidad"
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

        // Ya es una URL completa
        if (urlRelativa.StartsWith("http://") || urlRelativa.StartsWith("https://"))
        {
            return urlRelativa;
        }

        // URL protocol-relative (//ejemplo.com)
        if (urlRelativa.StartsWith("//"))
        {
            return $"https:{urlRelativa}";
        }

        // URL relativa que comienza con /
        if (urlRelativa.StartsWith("/"))
        {
            return $"{baseUrl}{urlRelativa}";
        }

        // URL relativa sin / inicial
        return $"{baseUrl}/{urlRelativa}";
    }
}
