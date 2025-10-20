using System.Net.Http.Json;
using System.Text.Json;
using AutoGuia.Core.DTOs;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AutoGuia.Infrastructure.ExternalServices
{
    /// <summary>
    /// Servicio para integración con API de MercadoLibre
    /// Documentación: https://developers.mercadolibre.com/es_ar/api-docs
    /// </summary>
    public class MercadoLibreService : IExternalMarketplaceService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;
        private readonly ILogger<MercadoLibreService> _logger;
        private readonly string _baseUrl;
        private readonly string _siteId; // CL = Chile, AR = Argentina, MX = México
        private readonly JsonSerializerOptions _jsonOptions;

        public string NombreMarketplace => "MercadoLibre";

        public MercadoLibreService(
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache,
            IConfiguration configuration,
            ILogger<MercadoLibreService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _logger = logger;
            _baseUrl = configuration["MercadoLibre:BaseUrl"] ?? "https://api.mercadolibre.com";
            _siteId = configuration["MercadoLibre:SiteId"] ?? "MLC"; // MLC = Chile
            
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        /// <summary>
        /// Verifica si la API de MercadoLibre está disponible
        /// </summary>
        public async Task<bool> EstaDisponibleAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("MercadoLibre");
                var response = await client.GetAsync($"{_baseUrl}/sites/{_siteId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error al verificar disponibilidad de MercadoLibre");
                return false;
            }
        }

        /// <summary>
        /// Busca productos en MercadoLibre Chile
        /// </summary>
        /// <param name="termino">Término de búsqueda (ej: "aceite motor castrol")</param>
        /// <param name="categoria">Categoría de MercadoLibre (ej: "MLC1747" = Repuestos)</param>
        /// <param name="limite">Cantidad máxima de resultados (1-50)</param>
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

            var cacheKey = $"ml_search_{termino}_{categoria}_{limite}";
            
            if (_cache.TryGetValue(cacheKey, out List<OfertaExternaDto>? cachedResults) 
                && cachedResults != null)
            {
                _logger.LogDebug("Resultados de búsqueda obtenidos desde caché: {Termino}", termino);
                return cachedResults;
            }

            try
            {
                var client = _httpClientFactory.CreateClient("MercadoLibre");
                
                // Construir query parameters
                var queryParams = $"q={Uri.EscapeDataString(termino)}&limit={Math.Min(limite, 50)}";
                if (!string.IsNullOrEmpty(categoria))
                {
                    queryParams += $"&category={categoria}";
                }

                var url = $"{_baseUrl}/sites/{_siteId}/search?{queryParams}";
                _logger.LogInformation("Buscando en MercadoLibre: {Url}", url);

                var response = await client.GetAsync(url);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error en API MercadoLibre: {StatusCode}", response.StatusCode);
                    return Enumerable.Empty<OfertaExternaDto>();
                }

                var content = await response.Content.ReadAsStringAsync();
                var searchResult = JsonSerializer.Deserialize<MercadoLibreSearchResponse>(content, _jsonOptions);

                if (searchResult?.Results == null || !searchResult.Results.Any())
                {
                    _logger.LogInformation("No se encontraron resultados para: {Termino}", termino);
                    return Enumerable.Empty<OfertaExternaDto>();
                }

                var ofertas = searchResult.Results.Select(MapToOfertaDto).ToList();
                
                // Cachear por 15 minutos
                _cache.Set(cacheKey, ofertas, TimeSpan.FromMinutes(15));
                
                _logger.LogInformation("Encontrados {Count} productos en MercadoLibre", ofertas.Count);
                
                return ofertas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar productos en MercadoLibre: {Termino}", termino);
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

            var cacheKey = $"ml_product_{productoId}";
            
            if (_cache.TryGetValue(cacheKey, out OfertaExternaDto? cachedProduct) 
                && cachedProduct != null)
            {
                return cachedProduct;
            }

            try
            {
                var client = _httpClientFactory.CreateClient("MercadoLibre");
                var url = $"{_baseUrl}/items/{productoId}";
                
                var response = await client.GetAsync(url);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Producto no encontrado: {ProductoId}", productoId);
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                var producto = JsonSerializer.Deserialize<MercadoLibreItem>(content, _jsonOptions);

                if (producto == null)
                {
                    return null;
                }

                var oferta = MapToOfertaDto(producto);
                
                // Cachear por 30 minutos
                _cache.Set(cacheKey, oferta, TimeSpan.FromMinutes(30));
                
                return oferta;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalle de producto: {ProductoId}", productoId);
                return null;
            }
        }

        /// <summary>
        /// Obtiene categorías de repuestos automotrices
        /// </summary>
        public async Task<IEnumerable<CategoriaMarketplaceDto>> ObtenerCategoriasAsync()
        {
            var cacheKey = "ml_categorias_auto";
            
            if (_cache.TryGetValue(cacheKey, out List<CategoriaMarketplaceDto>? cachedCategorias) 
                && cachedCategorias != null)
            {
                return cachedCategorias;
            }

            try
            {
                var client = _httpClientFactory.CreateClient("MercadoLibre");
                
                // Categoría raíz de repuestos automotrices en Chile
                var url = $"{_baseUrl}/categories/MLC1747/children";
                
                var response = await client.GetAsync(url);
                
                if (!response.IsSuccessStatusCode)
                {
                    return Enumerable.Empty<CategoriaMarketplaceDto>();
                }

                var categorias = await response.Content.ReadFromJsonAsync<List<MercadoLibreCategory>>(_jsonOptions);
                
                if (categorias == null)
                {
                    return Enumerable.Empty<CategoriaMarketplaceDto>();
                }

                var categoriasDto = categorias.Select(c => new CategoriaMarketplaceDto
                {
                    Id = c.Id ?? string.Empty,
                    Nombre = c.Name ?? string.Empty,
                    TotalProductos = c.TotalItemsInThisCategory
                }).ToList();

                // Cachear por 24 horas (las categorías no cambian frecuentemente)
                _cache.Set(cacheKey, categoriasDto, TimeSpan.FromHours(24));
                
                return categoriasDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener categorías de MercadoLibre");
                return Enumerable.Empty<CategoriaMarketplaceDto>();
            }
        }

        /// <summary>
        /// Normaliza nombres de categorías de MercadoLibre al estándar de AutoGuía
        /// </summary>
        public string NormalizarCategoria(string categoriaNativa)
        {
            var normalizaciones = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Aceites y Lubricantes", "Aceites" },
                { "Filtros", "Filtros" },
                { "Neumáticos", "Neumaticos" },
                { "Baterías", "Baterias" },
                { "Frenos", "Frenos" },
                { "Suspensión", "Suspension" },
                { "Iluminación", "Iluminacion" }
            };

            return normalizaciones.TryGetValue(categoriaNativa, out var normalizada) 
                ? normalizada 
                : categoriaNativa;
        }

        #region Mapeo de DTOs

        private OfertaExternaDto MapToOfertaDto(MercadoLibreItem item)
        {
            return new OfertaExternaDto
            {
                Id = item.Id ?? string.Empty,
                Titulo = item.Title ?? string.Empty,
                Precio = item.Price,
                Moneda = item.CurrencyId ?? "CLP",
                ImagenUrl = item.Thumbnail ?? item.Pictures?.FirstOrDefault()?.Url,
                UrlProducto = item.Permalink ?? $"https://articulo.mercadolibre.cl/{item.Id}",
                NombreTienda = item.SellerAddress?.City?.Name ?? "MercadoLibre",
                Marketplace = NombreMarketplace,
                Condicion = item.Condition == "new" ? "Nuevo" : "Usado",
                Stock = item.AvailableQuantity,
                EnvioGratis = item.Shipping?.FreeShipping ?? false,
                CantidadVendidos = item.SoldQuantity,
                FechaActualizacion = DateTime.UtcNow
            };
        }

        #endregion

        #region Modelos de API de MercadoLibre

        private class MercadoLibreSearchResponse
        {
            public List<MercadoLibreItem>? Results { get; set; }
        }

        private class MercadoLibreItem
        {
            public string? Id { get; set; }
            public string? Title { get; set; }
            public decimal Price { get; set; }
            public string? CurrencyId { get; set; }
            public int AvailableQuantity { get; set; }
            public int SoldQuantity { get; set; }
            public string? Condition { get; set; }
            public string? Thumbnail { get; set; }
            public string? Permalink { get; set; }
            public List<Picture>? Pictures { get; set; }
            public Shipping? Shipping { get; set; }
            public SellerAddress? SellerAddress { get; set; }
        }

        private class Picture
        {
            public string? Url { get; set; }
        }

        private class Shipping
        {
            public bool FreeShipping { get; set; }
        }

        private class SellerAddress
        {
            public City? City { get; set; }
        }

        private class City
        {
            public string? Name { get; set; }
        }

        private class MercadoLibreCategory
        {
            public string? Id { get; set; }
            public string? Name { get; set; }
            public int TotalItemsInThisCategory { get; set; }
        }

        #endregion
    }
}
