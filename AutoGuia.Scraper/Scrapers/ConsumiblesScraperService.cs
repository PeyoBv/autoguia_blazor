using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AutoGuia.Core.DTOs;
using System.Globalization;
using System.Text;

namespace AutoGuia.Scraper.Scrapers;

/// <summary>
/// Servicio de scraping para buscar consumibles automotrices en MercadoLibre
/// </summary>
public class ConsumiblesScraperService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ConsumiblesScraperService> _logger;
    private readonly IConfiguration _configuration;
    private const string BASE_URL = "https://listado.mercadolibre.cl/";
    private const string TIENDA_NOMBRE = "MercadoLibre";
    private const int RATE_LIMIT_MS = 1000; // 1 segundo entre peticiones

    public ConsumiblesScraperService(
        IHttpClientFactory httpClientFactory,
        ILogger<ConsumiblesScraperService> logger,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Busca consumibles automotrices en MercadoLibre
    /// </summary>
    /// <param name="termino">Término de búsqueda (ej: "Aceite 10W-40 Castrol")</param>
    /// <param name="categoria">Categoría del consumible (opcional)</param>
    /// <returns>Lista de ofertas encontradas</returns>
    public async Task<List<OfertaDto>> BuscarConsumiblesAsync(string termino, string? categoria = null)
    {
        if (string.IsNullOrWhiteSpace(termino))
        {
            _logger.LogWarning("⚠️ El término de búsqueda está vacío");
            return new List<OfertaDto>();
        }

        var ofertas = new List<OfertaDto>();

        try
        {
            _logger.LogInformation("🔍 Iniciando búsqueda en {Tienda}: '{Termino}' (Categoría: {Categoria})", 
                TIENDA_NOMBRE, termino, categoria ?? "Todas");

            // Construir URL de búsqueda
            var searchUrl = ConstruirUrlBusqueda(termino, categoria);
            _logger.LogDebug("📍 URL de búsqueda: {Url}", searchUrl);

            // Realizar petición HTTP
            var htmlContent = await ObtenerContenidoHtmlAsync(searchUrl);
            if (string.IsNullOrEmpty(htmlContent))
            {
                _logger.LogWarning("⚠️ No se obtuvo contenido HTML de la página");
                return ofertas;
            }

            // Parsear HTML
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            // Extraer productos
            ofertas = ExtraerProductos(htmlDoc, termino);

            _logger.LogInformation("📊 Búsqueda completada: {Count} productos encontrados en {Tienda}", 
                ofertas.Count, TIENDA_NOMBRE);

            // Rate limiting - esperar antes de permitir otra petición
            await Task.Delay(RATE_LIMIT_MS);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "❌ Error de conexión HTTP al buscar en {Tienda}: {Message}", 
                TIENDA_NOMBRE, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error inesperado al buscar consumibles en {Tienda}: {Message}", 
                TIENDA_NOMBRE, ex.Message);
        }

        return ofertas;
    }

    /// <summary>
    /// Construye la URL de búsqueda para MercadoLibre
    /// </summary>
    private string ConstruirUrlBusqueda(string termino, string? categoria)
    {
        // Sanitizar término de búsqueda
        var terminoLimpio = SanitizarTermino(termino);
        
        // Construir query string
        var queryBuilder = new StringBuilder();
        queryBuilder.Append(terminoLimpio);

        if (!string.IsNullOrWhiteSpace(categoria))
        {
            queryBuilder.Append($" {SanitizarTermino(categoria)}");
        }

        // Agregar término relacionado con automotriz
        queryBuilder.Append(" automotriz");

        // URL encode
        var query = Uri.EscapeDataString(queryBuilder.ToString());
        
        return $"{BASE_URL}{query}";
    }

    /// <summary>
    /// Sanitiza el término de búsqueda
    /// </summary>
    private string SanitizarTermino(string termino)
    {
        if (string.IsNullOrWhiteSpace(termino))
            return string.Empty;

        // Remover caracteres especiales pero mantener espacios y guiones
        var builder = new StringBuilder();
        foreach (char c in termino)
        {
            if (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '-')
            {
                builder.Append(c);
            }
        }

        return builder.ToString().Trim();
    }

    /// <summary>
    /// Obtiene el contenido HTML de la URL especificada
    /// </summary>
    private async Task<string?> ObtenerContenidoHtmlAsync(string url)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            
            // Configurar headers para simular navegador
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", 
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            client.DefaultRequestHeaders.Add("Accept", 
                "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            client.DefaultRequestHeaders.Add("Accept-Language", "es-CL,es;q=0.9,en;q=0.8");

            client.Timeout = TimeSpan.FromSeconds(30);

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "❌ Error HTTP al obtener contenido de {Url}: {StatusCode}", 
                url, ex.StatusCode);
            return null;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "❌ Timeout al obtener contenido de {Url}", url);
            return null;
        }
    }

    /// <summary>
    /// Extrae productos del documento HTML
    /// </summary>
    private List<OfertaDto> ExtraerProductos(HtmlDocument htmlDoc, string termino)
    {
        var ofertas = new List<OfertaDto>();

        try
        {
            // Buscar contenedores de productos
            // MercadoLibre usa diferentes estructuras, intentar múltiples selectores
            var productNodes = htmlDoc.DocumentNode.SelectNodes("//li[contains(@class, 'ui-search-layout__item')]");
            
            if (productNodes == null || !productNodes.Any())
            {
                _logger.LogWarning("⚠️ No se encontraron nodos de productos con el selector principal");
                // Intentar selector alternativo
                productNodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'poly-card')]");
            }

            if (productNodes == null || !productNodes.Any())
            {
                _logger.LogWarning("⚠️ No se encontraron productos en la página");
                return ofertas;
            }

            _logger.LogDebug("🔎 Procesando {Count} nodos de productos", productNodes.Count);

            foreach (var productNode in productNodes)
            {
                try
                {
                    var oferta = ExtraerProducto(productNode);
                    
                    if (oferta != null)
                    {
                        ofertas.Add(oferta);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "⚠️ Error al extraer un producto individual: {Message}", ex.Message);
                    // Continuar con el siguiente producto
                }
            }

            _logger.LogInformation("✅ Productos extraídos exitosamente: {Count}/{Total}", 
                ofertas.Count, productNodes.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al extraer productos del HTML: {Message}", ex.Message);
        }

        return ofertas;
    }

    /// <summary>
    /// Extrae información de un producto individual
    /// </summary>
    private OfertaDto? ExtraerProducto(HtmlNode productNode)
    {
        try
        {
            // Extraer título/nombre
            var tituloNode = productNode.SelectSingleNode(".//h2[contains(@class, 'poly-box__title')]") ??
                           productNode.SelectSingleNode(".//h2[contains(@class, 'ui-search-item__title')]");
            
            if (tituloNode == null)
            {
                _logger.LogDebug("⚠️ No se encontró título en el nodo del producto");
                return null;
            }

            var nombre = LimpiarTexto(tituloNode.InnerText);
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return null;
            }

            // Extraer precio
            var precioNode = productNode.SelectSingleNode(".//span[contains(@class, 'price-tag-fraction')]") ??
                           productNode.SelectSingleNode(".//span[contains(@class, 'andes-money-amount__fraction')]");
            
            var precio = ExtraerPrecio(precioNode);
            if (precio <= 0)
            {
                _logger.LogDebug("⚠️ Precio no válido para producto: {Nombre}", nombre);
                return null;
            }

            // Extraer URL
            var linkNode = productNode.SelectSingleNode(".//a[contains(@class, 'poly-box__link')]") ??
                         productNode.SelectSingleNode(".//a[contains(@class, 'ui-search-link')]");
            
            var url = linkNode?.GetAttributeValue("href", string.Empty) ?? string.Empty;
            if (string.IsNullOrWhiteSpace(url))
            {
                _logger.LogDebug("⚠️ URL no encontrada para producto: {Nombre}", nombre);
                return null;
            }

            // Extraer imagen
            var imagenNode = productNode.SelectSingleNode(".//img[contains(@class, 'poly-box__image')]") ??
                           productNode.SelectSingleNode(".//img[contains(@class, 'ui-search-result-image__element')]");
            
            var imagenUrl = imagenNode?.GetAttributeValue("src", string.Empty) ?? 
                          imagenNode?.GetAttributeValue("data-src", string.Empty) ?? 
                          string.Empty;

            // Crear DTO
            var oferta = new OfertaDto
            {
                ProductoNombre = nombre,
                Precio = precio,
                UrlProductoEnTienda = url,
                ProductoImagen = imagenUrl,
                TiendaNombre = TIENDA_NOMBRE,
                TiendaLogo = "https://http2.mlstatic.com/frontend-assets/ml-web-navigation/ui-navigation/5.21.22/mercadolibre/logo__large_plus.png",
                FechaActualizacion = DateTime.UtcNow,
                EsDisponible = true
            };

            _logger.LogDebug("✅ Producto extraído: {Nombre} - ${Precio}", nombre, precio);

            return oferta;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "⚠️ Error al extraer datos del producto: {Message}", ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Extrae y parsea el precio desde el nodo HTML
    /// </summary>
    private decimal ExtraerPrecio(HtmlNode? precioNode)
    {
        if (precioNode == null)
            return 0;

        try
        {
            var precioTexto = LimpiarTexto(precioNode.InnerText);
            
            // Remover puntos de separación de miles y reemplazar coma por punto decimal
            precioTexto = precioTexto.Replace(".", "").Replace(",", ".");
            
            // Extraer solo números y punto decimal
            var precioLimpio = new string(precioTexto.Where(c => char.IsDigit(c) || c == '.').ToArray());

            if (decimal.TryParse(precioLimpio, NumberStyles.Any, CultureInfo.InvariantCulture, out var precio))
            {
                return precio;
            }

            _logger.LogDebug("⚠️ No se pudo parsear precio: '{Texto}'", precioTexto);
            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "⚠️ Error al parsear precio: {Message}", ex.Message);
            return 0;
        }
    }

    /// <summary>
    /// Limpia el texto eliminando espacios extras y caracteres especiales
    /// </summary>
    private string LimpiarTexto(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return string.Empty;

        // Decodificar entidades HTML
        texto = HtmlEntity.DeEntitize(texto);

        // Normalizar espacios en blanco
        texto = System.Text.RegularExpressions.Regex.Replace(texto, @"\s+", " ");

        return texto.Trim();
    }

    /// <summary>
    /// Valida que MercadoLibre esté disponible
    /// </summary>
    public async Task<bool> ValidarDisponibilidadAsync()
    {
        try
        {
            _logger.LogInformation("🔍 Validando disponibilidad de {Tienda}...", TIENDA_NOMBRE);
            
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(10);
            
            var response = await client.GetAsync(BASE_URL);
            var disponible = response.IsSuccessStatusCode;

            if (disponible)
            {
                _logger.LogInformation("✅ {Tienda} está disponible", TIENDA_NOMBRE);
            }
            else
            {
                _logger.LogWarning("⚠️ {Tienda} no está disponible. StatusCode: {StatusCode}", 
                    TIENDA_NOMBRE, response.StatusCode);
            }

            return disponible;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al validar disponibilidad de {Tienda}: {Message}", 
                TIENDA_NOMBRE, ex.Message);
            return false;
        }
    }
}
