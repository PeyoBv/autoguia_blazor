using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AutoGuia.Core.DTOs;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AutoGuia.Infrastructure.ExternalServices
{
    /// <summary>
    /// Servicio para integración con API de eBay
    /// Documentación: https://developer.ebay.com/
    /// Usa la API Buy Browse para búsquedas de productos
    /// </summary>
    public class EbayService : IExternalMarketplaceService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;
        private readonly ILogger<EbayService> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _baseUrl;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly JsonSerializerOptions _jsonOptions;

        public string NombreMarketplace => "eBay";

        public EbayService(
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache,
            IConfiguration configuration,
            ILogger<EbayService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _configuration = configuration;
            _logger = logger;
            _baseUrl = configuration["Ebay:BaseUrl"] ?? "https://api.ebay.com";
            _clientId = configuration["Ebay:ClientId"] ?? string.Empty;
            _clientSecret = configuration["Ebay:ClientSecret"] ?? string.Empty;
            
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        /// <summary>
        /// Verifica si la API de eBay está disponible
        /// </summary>
        public async Task<bool> EstaDisponibleAsync()
        {
            if (string.IsNullOrEmpty(_clientId) || string.IsNullOrEmpty(_clientSecret))
            {
                _logger.LogWarning("Credenciales de eBay no configuradas");
                return false;
            }

            try
            {
                var token = await ObtenerAccessTokenAsync();
                return !string.IsNullOrEmpty(token);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error al verificar disponibilidad de eBay");
                return false;
            }
        }

        /// <summary>
        /// Busca productos en eBay
        /// </summary>
        /// <param name="termino">Término de búsqueda</param>
        /// <param name="categoria">ID de categoría de eBay (ej: "6030" = Auto Parts)</param>
        /// <param name="limite">Cantidad máxima de resultados (1-200)</param>
        public async Task<IEnumerable<OfertaExternaDto>> BuscarProductosAsync(
            string termino, 
            string? categoria = null, 
            int limite = 20)
        {
            if (string.IsNullOrWhiteSpace(termino))
            {
                _logger.LogWarning("BuscarProductosAsync: término de búsqueda vacío");
                return Enumerable.Empty<OfertaExternaDto>();
            }

            var cacheKey = $"ebay_search_{termino}_{categoria}_{limite}";
            
            if (_cache.TryGetValue(cacheKey, out List<OfertaExternaDto>? cachedResults) 
                && cachedResults != null)
            {
                _logger.LogDebug("Resultados de eBay obtenidos desde caché: {Termino}", termino);
                return cachedResults;
            }

            try
            {
                var token = await ObtenerAccessTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogError("No se pudo obtener token de acceso de eBay");
                    return Enumerable.Empty<OfertaExternaDto>();
                }

                var client = _httpClientFactory.CreateClient("Ebay");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                client.DefaultRequestHeaders.Add("X-EBAY-C-MARKETPLACE-ID", "EBAY_US"); // Cambiar según país

                // Construir query
                var queryParams = $"q={Uri.EscapeDataString(termino)}&limit={Math.Min(limite, 200)}";
                if (!string.IsNullOrEmpty(categoria))
                {
                    queryParams += $"&category_ids={categoria}";
                }
                queryParams += "&filter=buyingOptions:{FIXED_PRICE}"; // Solo compra directa

                var url = $"{_baseUrl}/buy/browse/v1/item_summary/search?{queryParams}";
                _logger.LogInformation("Buscando en eBay: {Termino}", termino);

                var response = await client.GetAsync(url);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Error en API eBay: {StatusCode} - {Error}", 
                        response.StatusCode, errorContent);
                    return Enumerable.Empty<OfertaExternaDto>();
                }

                var content = await response.Content.ReadAsStringAsync();
                var searchResult = JsonSerializer.Deserialize<EbaySearchResponse>(content, _jsonOptions);

                if (searchResult?.ItemSummaries == null || !searchResult.ItemSummaries.Any())
                {
                    _logger.LogInformation("No se encontraron resultados en eBay para: {Termino}", termino);
                    return Enumerable.Empty<OfertaExternaDto>();
                }

                var ofertas = searchResult.ItemSummaries.Select(MapToOfertaDto).ToList();
                
                // Cachear por 15 minutos
                _cache.Set(cacheKey, ofertas, TimeSpan.FromMinutes(15));
                
                _logger.LogInformation("Encontrados {Count} productos en eBay", ofertas.Count);
                
                return ofertas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar productos en eBay: {Termino}", termino);
                return Enumerable.Empty<OfertaExternaDto>();
            }
        }

        /// <summary>
        /// Obtiene detalles de un producto específico
        /// </summary>
        public async Task<OfertaExternaDto?> ObtenerDetalleProductoAsync(string productoId)
        {
            if (string.IsNullOrWhiteSpace(productoId))
            {
                return null;
            }

            var cacheKey = $"ebay_product_{productoId}";
            
            if (_cache.TryGetValue(cacheKey, out OfertaExternaDto? cachedProduct) 
                && cachedProduct != null)
            {
                return cachedProduct;
            }

            try
            {
                var token = await ObtenerAccessTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    return null;
                }

                var client = _httpClientFactory.CreateClient("Ebay");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var url = $"{_baseUrl}/buy/browse/v1/item/{productoId}";
                
                var response = await client.GetAsync(url);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Producto eBay no encontrado: {ProductoId}", productoId);
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                var item = JsonSerializer.Deserialize<EbayItem>(content, _jsonOptions);

                if (item == null)
                {
                    return null;
                }

                var oferta = MapToOfertaDto(item);
                
                // Cachear por 30 minutos
                _cache.Set(cacheKey, oferta, TimeSpan.FromMinutes(30));
                
                return oferta;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalle de producto eBay: {ProductoId}", productoId);
                return null;
            }
        }

        /// <summary>
        /// Obtiene categorías de repuestos automotrices de eBay
        /// </summary>
        public async Task<IEnumerable<CategoriaMarketplaceDto>> ObtenerCategoriasAsync()
        {
            var cacheKey = "ebay_categorias_auto";
            
            if (_cache.TryGetValue(cacheKey, out List<CategoriaMarketplaceDto>? cachedCategorias) 
                && cachedCategorias != null)
            {
                return cachedCategorias;
            }

            try
            {
                // Categorías principales de auto parts en eBay
                var categorias = new List<CategoriaMarketplaceDto>
                {
                    new() { Id = "6030", Nombre = "Auto Parts & Accessories" },
                    new() { Id = "33556", Nombre = "Motors > Parts & Accessories" },
                    new() { Id = "131090", Nombre = "Oils & Fluids" },
                    new() { Id = "131092", Nombre = "Filters" },
                    new() { Id = "66468", Nombre = "Tires" },
                    new() { Id = "33639", Nombre = "Brakes & Brake Parts" },
                    new() { Id = "33565", Nombre = "Batteries" }
                };

                // Cachear por 24 horas
                _cache.Set(cacheKey, categorias, TimeSpan.FromHours(24));
                
                return categorias;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener categorías de eBay");
                return Enumerable.Empty<CategoriaMarketplaceDto>();
            }
        }

        /// <summary>
        /// Normaliza nombres de categorías de eBay al estándar de AutoGuía
        /// </summary>
        public string NormalizarCategoria(string categoriaNativa)
        {
            var normalizaciones = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Oils & Fluids", "Aceites" },
                { "Filters", "Filtros" },
                { "Tires", "Neumaticos" },
                { "Batteries", "Baterias" },
                { "Brakes & Brake Parts", "Frenos" }
            };

            return normalizaciones.TryGetValue(categoriaNativa, out var normalizada) 
                ? normalizada 
                : categoriaNativa;
        }

        #region OAuth 2.0 - Obtener Access Token

        /// <summary>
        /// Obtiene un access token de eBay usando Client Credentials
        /// </summary>
        private async Task<string> ObtenerAccessTokenAsync()
        {
            var cacheKey = "ebay_access_token";
            
            if (_cache.TryGetValue(cacheKey, out string? cachedToken) && !string.IsNullOrEmpty(cachedToken))
            {
                return cachedToken;
            }

            try
            {
                var client = _httpClientFactory.CreateClient("Ebay");
                
                var authUrl = $"{_baseUrl}/identity/v1/oauth2/token";
                
                var credentials = Convert.ToBase64String(
                    Encoding.ASCII.GetBytes($"{_clientId}:{_clientSecret}"));
                
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Basic {credentials}");

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("scope", "https://api.ebay.com/oauth/api_scope")
                });

                var response = await client.PostAsync(authUrl, content);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error al obtener token de eBay: {StatusCode}", response.StatusCode);
                    return string.Empty;
                }

                var tokenResponse = await response.Content.ReadFromJsonAsync<EbayTokenResponse>(_jsonOptions);
                
                if (tokenResponse?.AccessToken == null)
                {
                    return string.Empty;
                }

                // Cachear el token (expira en 2 horas típicamente)
                var expirationTime = TimeSpan.FromSeconds(tokenResponse.ExpiresIn - 300); // 5 min buffer
                _cache.Set(cacheKey, tokenResponse.AccessToken, expirationTime);
                
                _logger.LogInformation("Token de eBay obtenido exitosamente");
                
                return tokenResponse.AccessToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener access token de eBay");
                return string.Empty;
            }
        }

        #endregion

        #region Mapeo de DTOs

        private OfertaExternaDto MapToOfertaDto(EbayItem item)
        {
            return new OfertaExternaDto
            {
                Id = item.ItemId ?? string.Empty,
                Titulo = item.Title ?? string.Empty,
                Precio = item.Price?.Value ?? 0,
                Moneda = item.Price?.Currency ?? "USD",
                ImagenUrl = item.Image?.ImageUrl,
                UrlProducto = item.ItemWebUrl ?? string.Empty,
                NombreTienda = item.Seller?.Username ?? "eBay Seller",
                Marketplace = NombreMarketplace,
                Condicion = item.Condition ?? "New",
                Stock = item.EstimatedAvailabilities?.FirstOrDefault()?.EstimatedAvailableQuantity ?? 0,
                EnvioGratis = item.ShippingOptions?.Any(s => s.ShippingCost?.Value == 0) ?? false,
                Calificacion = item.Seller?.FeedbackPercentage,
                FechaActualizacion = DateTime.UtcNow
            };
        }

        #endregion

        #region Modelos de API de eBay

        private class EbayTokenResponse
        {
            public string? AccessToken { get; set; }
            public int ExpiresIn { get; set; }
            public string? TokenType { get; set; }
        }

        private class EbaySearchResponse
        {
            public List<EbayItem>? ItemSummaries { get; set; }
            public int Total { get; set; }
        }

        private class EbayItem
        {
            public string? ItemId { get; set; }
            public string? Title { get; set; }
            public Price? Price { get; set; }
            public Image? Image { get; set; }
            public string? ItemWebUrl { get; set; }
            public Seller? Seller { get; set; }
            public string? Condition { get; set; }
            public List<ShippingOption>? ShippingOptions { get; set; }
            public List<EstimatedAvailability>? EstimatedAvailabilities { get; set; }
        }

        private class Price
        {
            public decimal Value { get; set; }
            public string? Currency { get; set; }
        }

        private class Image
        {
            public string? ImageUrl { get; set; }
        }

        private class Seller
        {
            public string? Username { get; set; }
            public double? FeedbackPercentage { get; set; }
        }

        private class ShippingOption
        {
            public Price? ShippingCost { get; set; }
        }

        private class EstimatedAvailability
        {
            public int EstimatedAvailableQuantity { get; set; }
        }

        #endregion
    }
}
