using AutoGuia.Scraper.DTOs;
using AutoGuia.Scraper.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AutoGuia.Scraper.Scrapers;

/// <summary>
/// Scraper para MercadoLibre usando su API pública REST.
/// No requiere autenticación para búsquedas básicas.
/// Documentación: https://developers.mercadolibre.com/es_ar/items-y-busquedas
/// </summary>
public class MercadoLibreScraperService : IScraper
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<MercadoLibreScraperService> _logger;
    
    // Configuración de la API
    private const string ApiBaseUrl = "https://api.mercadolibre.com";
    private const string SiteId = "MLC"; // Chile - Cambiar según país: MLM (México), MLA (Argentina), MLB (Brasil)
    
    public string TiendaNombre => "MercadoLibre";
    public string TipoScraper => "API";
    public bool EstaHabilitado => true;

    public MercadoLibreScraperService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<MercadoLibreScraperService> logger)
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
            _logger.LogInformation("🔍 Iniciando scraping en MercadoLibre para número de parte: {NumeroDeParte}", numeroDeParte);
            
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

            // 4. Ejecutar petición GET
            var response = await httpClient.GetAsync(searchUrl, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("⚠️ API de MercadoLibre devolvió código {StatusCode}: {ReasonPhrase}", 
                    response.StatusCode, response.ReasonPhrase);
                
                return ofertas; // Devolver lista vacía, NO ofertas con error
            }

            // 5. Parsear respuesta JSON
            var searchResponse = await response.Content.ReadFromJsonAsync<MercadoLibreSearchResponse>(
                cancellationToken: cancellationToken);

            if (searchResponse?.Results == null || searchResponse.Results.Count == 0)
            {
                _logger.LogInformation("ℹ️ No se encontraron resultados en MercadoLibre para: {NumeroDeParte}", numeroDeParte);
                return ofertas;
            }

            _logger.LogInformation("📦 Se encontraron {Count} resultados en MercadoLibre", searchResponse.Results.Count);

            // 6. Procesar cada resultado y convertir a OfertaDto
            var maxResults = _configuration.GetValue<int>("ScrapingSettings:MercadoLibre:MaxResults", 10);
            var resultadosProcesados = 0;

            foreach (var item in searchResponse.Results.Take(maxResults))
            {
                try
                {
                    var oferta = MapearItemAOferta(item, tiendaId);
                    
                    if (oferta != null)
                    {
                        ofertas.Add(oferta);
                        resultadosProcesados++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "⚠️ Error al procesar item de MercadoLibre: {ItemId}", item.Id);
                }
            }

            _logger.LogInformation("✅ Scraping completado: {Procesados}/{Total} ofertas procesadas de MercadoLibre", 
                resultadosProcesados, searchResponse.Results.Count);

            return ofertas;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "❌ Error de red al conectar con MercadoLibre");
            return ofertas; // Devolver lista vacía, NO ofertas con error
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "❌ Error al parsear JSON de MercadoLibre");
            return ofertas; // Devolver lista vacía, NO ofertas con error
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error inesperado al scrapear MercadoLibre");
            return ofertas; // Devolver lista vacía, NO ofertas con error
        }
    }

    /// <inheritdoc />
    public bool PuedeScrapearTienda(string tiendaNombre)
    {
        return tiendaNombre.Equals(TiendaNombre, StringComparison.OrdinalIgnoreCase) ||
               tiendaNombre.Equals("ML", StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public async Task<Dictionary<string, string>> ObtenerConfiguracionAsync()
    {
        return await Task.FromResult(new Dictionary<string, string>
        {
            ["ApiBaseUrl"] = ApiBaseUrl,
            ["SiteId"] = SiteId,
            ["Timeout"] = "30",
            ["MaxResults"] = _configuration.GetValue<string>("ScrapingSettings:MercadoLibre:MaxResults", "10"),
            ["RequiereAutenticacion"] = "false",
            ["TipoRespuesta"] = "JSON",
            ["RateLimitPerSecond"] = "10",
            ["CacheDuration"] = "300" // 5 minutos
        });
    }

    #region Métodos Privados

    /// <summary>
    /// Configura el HttpClient con headers y timeouts apropiados.
    /// </summary>
    private void ConfigurarHttpClient(HttpClient httpClient)
    {
        // User-Agent personalizado (recomendado por MercadoLibre)
        httpClient.DefaultRequestHeaders.Add("User-Agent", "AutoGuia-Scraper/1.0 (+https://autoguia.cl)");
        
        // Accept JSON
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        
        // Opcional: Si en el futuro necesitas autenticación, agregar aquí:
        // var accessToken = _configuration["ScrapingSettings:MercadoLibre:AccessToken"];
        // if (!string.IsNullOrEmpty(accessToken))
        // {
        //     httpClient.DefaultRequestHeaders.Authorization = 
        //         new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        // }
        
        // Timeout
        var timeoutSeconds = _configuration.GetValue<int>("ScrapingSettings:MercadoLibre:TimeoutSeconds", 30);
        httpClient.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
    }

    /// <summary>
    /// Construye la URL de búsqueda para la API de MercadoLibre.
    /// </summary>
    private string ConstruirUrlBusqueda(string numeroDeParte)
    {
        // Limpiar y escapar el número de parte
        var queryEscaped = Uri.EscapeDataString(numeroDeParte.Trim());
        
        // Límite de resultados desde configuración
        var limit = _configuration.GetValue<int>("ScrapingSettings:MercadoLibre:MaxResults", 10);
        
        // Construir URL con parámetros
        return $"{ApiBaseUrl}/sites/{SiteId}/search?q={queryEscaped}&limit={limit}&offset=0";
        
        // Parámetros adicionales disponibles:
        // &category=MLB5672 (filtrar por categoría)
        // &sort=price_asc (ordenar por precio ascendente)
        // &condition=new (solo nuevos)
        // &shipping=free (envío gratis)
    }

    /// <summary>
    /// Mapea un item de MercadoLibre a nuestro DTO de oferta.
    /// </summary>
    private OfertaDto? MapearItemAOferta(MercadoLibreItem item, int tiendaId)
    {
        if (item == null || string.IsNullOrEmpty(item.Id))
        {
            return null;
        }

        try
        {
            // Validar precio
            if (item.Price <= 0)
            {
                _logger.LogDebug("⚠️ Item {ItemId} tiene precio inválido: {Price}", item.Id, item.Price);
                return null;
            }

            // Determinar disponibilidad de stock
            var stockDisponible = !string.IsNullOrEmpty(item.Condition) && 
                                 item.AvailableQuantity > 0 &&
                                 item.Status?.Equals("active", StringComparison.OrdinalIgnoreCase) == true;

            // Calcular calificación del vendedor (0-5)
            decimal? calificacion = null;
            if (item.Seller?.SellerReputation?.LevelId != null)
            {
                // MercadoLibre usa: 5_green, 4_light_green, 3_yellow, 2_orange, 1_red
                calificacion = item.Seller.SellerReputation.LevelId.ToLower() switch
                {
                    "5_green" => 5.0m,
                    "4_light_green" => 4.0m,
                    "3_yellow" => 3.0m,
                    "2_orange" => 2.0m,
                    "1_red" => 1.0m,
                    _ => null
                };
            }

            // Determinar si tiene descuento
            decimal? precioAnterior = null;
            if (item.OriginalPrice.HasValue && item.OriginalPrice > item.Price)
            {
                precioAnterior = item.OriginalPrice.Value;
            }

            // Información de envío
            var tiempoEntrega = item.Shipping?.FreeShipping == true 
                ? "Envío gratis" 
                : null;

            return new OfertaDto
            {
                TiendaId = tiendaId,
                TiendaNombre = TiendaNombre,
                Precio = item.Price,
                UrlProducto = item.Permalink ?? $"https://articulo.mercadolibre.cl/{item.Id}",
                StockDisponible = stockDisponible,
                CantidadDisponible = item.AvailableQuantity > 0 ? item.AvailableQuantity : null,
                Descripcion = LimpiarTexto(item.Title),
                ImagenUrl = item.Thumbnail,
                TiempoEntrega = tiempoEntrega,
                Calificacion = calificacion,
                FechaScrapeo = DateTime.UtcNow,
                TieneErrores = false
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error al mapear item {ItemId}", item.Id);
            return null;
        }
    }

    /// <summary>
    /// Limpia y trunca texto para la base de datos.
    /// </summary>
    private string? LimpiarTexto(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return null;

        // Eliminar espacios extras y saltos de línea
        var limpio = System.Text.RegularExpressions.Regex.Replace(texto.Trim(), @"\s+", " ");
        
        // Truncar si es muy largo (límite de BD)
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

#region DTOs de MercadoLibre API

/// <summary>
/// Respuesta de la API de búsqueda de MercadoLibre.
/// </summary>
public class MercadoLibreSearchResponse
{
    [JsonPropertyName("site_id")]
    public string? SiteId { get; set; }

    [JsonPropertyName("query")]
    public string? Query { get; set; }

    [JsonPropertyName("paging")]
    public MercadoLibrePaging? Paging { get; set; }

    [JsonPropertyName("results")]
    public List<MercadoLibreItem> Results { get; set; } = new();
}

/// <summary>
/// Información de paginación.
/// </summary>
public class MercadoLibrePaging
{
    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    [JsonPropertyName("limit")]
    public int Limit { get; set; }
}

/// <summary>
/// Representa un producto/item de MercadoLibre.
/// </summary>
public class MercadoLibreItem
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("original_price")]
    public decimal? OriginalPrice { get; set; }

    [JsonPropertyName("currency_id")]
    public string? CurrencyId { get; set; }

    [JsonPropertyName("available_quantity")]
    public int AvailableQuantity { get; set; }

    [JsonPropertyName("sold_quantity")]
    public int SoldQuantity { get; set; }

    [JsonPropertyName("condition")]
    public string? Condition { get; set; } // "new" o "used"

    [JsonPropertyName("permalink")]
    public string? Permalink { get; set; }

    [JsonPropertyName("thumbnail")]
    public string? Thumbnail { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; } // "active", "paused", "closed"

    [JsonPropertyName("seller")]
    public MercadoLibreSeller? Seller { get; set; }

    [JsonPropertyName("shipping")]
    public MercadoLibreShipping? Shipping { get; set; }

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new();
}

/// <summary>
/// Información del vendedor.
/// </summary>
public class MercadoLibreSeller
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    [JsonPropertyName("seller_reputation")]
    public MercadoLibreSellerReputation? SellerReputation { get; set; }
}

/// <summary>
/// Reputación del vendedor.
/// </summary>
public class MercadoLibreSellerReputation
{
    [JsonPropertyName("level_id")]
    public string? LevelId { get; set; } // "5_green", "4_light_green", etc.

    [JsonPropertyName("power_seller_status")]
    public string? PowerSellerStatus { get; set; }

    [JsonPropertyName("transactions")]
    public MercadoLibreTransactions? Transactions { get; set; }
}

/// <summary>
/// Métricas de transacciones del vendedor.
/// </summary>
public class MercadoLibreTransactions
{
    [JsonPropertyName("completed")]
    public int Completed { get; set; }

    [JsonPropertyName("canceled")]
    public int Canceled { get; set; }
}

/// <summary>
/// Información de envío.
/// </summary>
public class MercadoLibreShipping
{
    [JsonPropertyName("free_shipping")]
    public bool FreeShipping { get; set; }

    [JsonPropertyName("mode")]
    public string? Mode { get; set; }

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new();
}

#endregion
