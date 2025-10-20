using AutoGuia.Infrastructure.ExternalServices;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace AutoGuia.Tests.Services.ExternalServices
{
    /// <summary>
    /// Tests unitarios para MercadoLibreService
    /// </summary>
    public class MercadoLibreServiceTests : IDisposable
    {
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly Mock<ILogger<MercadoLibreService>> _mockLogger;
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        private readonly MercadoLibreService _service;

        public MercadoLibreServiceTests()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockLogger = new Mock<ILogger<MercadoLibreService>>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());

            var configDictionary = new Dictionary<string, string?>
            {
                { "MercadoLibre:BaseUrl", "https://api.mercadolibre.com" },
                { "MercadoLibre:SiteId", "MLC" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configDictionary)
                .Build();

            _service = new MercadoLibreService(
                _mockHttpClientFactory.Object,
                _memoryCache,
                _configuration,
                _mockLogger.Object);
        }

        [Fact]
        public void Constructor_InicializaPropiedadesCorrectamente()
        {
            // Assert
            _service.NombreMarketplace.Should().Be("MercadoLibre");
        }

        [Fact]
        public async Task EstaDisponibleAsync_CuandoApiRespondeOk_RetornaTrue()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            _mockHttpClientFactory
                .Setup(x => x.CreateClient("MercadoLibre"))
                .Returns(httpClient);

            // Act
            var resultado = await _service.EstaDisponibleAsync();

            // Assert
            resultado.Should().BeTrue();
        }

        [Fact]
        public async Task EstaDisponibleAsync_CuandoApiNoResponde_RetornaFalse()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException());

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            _mockHttpClientFactory
                .Setup(x => x.CreateClient("MercadoLibre"))
                .Returns(httpClient);

            // Act
            var resultado = await _service.EstaDisponibleAsync();

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public async Task BuscarProductosAsync_ConTerminoVacio_RetornaListaVacia()
        {
            // Act
            var resultado = await _service.BuscarProductosAsync(string.Empty);

            // Assert
            resultado.Should().BeEmpty();
        }

        [Fact]
        public async Task BuscarProductosAsync_ConTerminoValido_RetornaOfertas()
        {
            // Arrange
            var responseContent = new
            {
                results = new[]
                {
                    new
                    {
                        id = "MLC123456",
                        title = "Aceite Castrol 10W-40",
                        price = 25000,
                        currency_id = "CLP",
                        thumbnail = "https://example.com/image.jpg",
                        permalink = "https://articulo.mercadolibre.cl/MLC123456",
                        condition = "new",
                        available_quantity = 10,
                        sold_quantity = 5,
                        shipping = new { free_shipping = true }
                    }
                }
            };

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(responseContent))
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://api.mercadolibre.com")
            };

            _mockHttpClientFactory
                .Setup(x => x.CreateClient("MercadoLibre"))
                .Returns(httpClient);

            // Act
            var resultado = await _service.BuscarProductosAsync("aceite castrol");

            // Assert
            resultado.Should().NotBeEmpty();
            resultado.Should().HaveCount(1);
            resultado.First().Titulo.Should().Be("Aceite Castrol 10W-40");
            resultado.First().Precio.Should().Be(25000);
            resultado.First().EnvioGratis.Should().BeTrue();
        }

        [Fact]
        public async Task BuscarProductosAsync_UsaCacheEnSegundaLlamada()
        {
            // Arrange
            var responseContent = new { results = new[] { new { id = "TEST", title = "Test Product", price = 1000 } } };

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(responseContent))
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://api.mercadolibre.com")
            };

            _mockHttpClientFactory
                .Setup(x => x.CreateClient("MercadoLibre"))
                .Returns(httpClient);

            // Act
            var resultado1 = await _service.BuscarProductosAsync("test");
            var resultado2 = await _service.BuscarProductosAsync("test");

            // Assert
            resultado1.Should().BeEquivalentTo(resultado2);
            
            // Verificar que solo se hizo 1 llamada HTTP (la segunda usa caché)
            mockHttpMessageHandler
                .Protected()
                .Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>());
        }

        [Theory]
        [InlineData("Aceites y Lubricantes", "Aceites")]
        [InlineData("Filtros", "Filtros")]
        [InlineData("Neumáticos", "Neumaticos")]
        [InlineData("Categoría Desconocida", "Categoría Desconocida")]
        public void NormalizarCategoria_DebeMapejarCorrectamente(string entrada, string esperado)
        {
            // Act
            var resultado = _service.NormalizarCategoria(entrada);

            // Assert
            resultado.Should().Be(esperado);
        }

        public void Dispose()
        {
            _memoryCache?.Dispose();
        }
    }
}
