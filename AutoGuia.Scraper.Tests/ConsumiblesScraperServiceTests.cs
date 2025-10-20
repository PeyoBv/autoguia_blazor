using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using AutoGuia.Scraper.Scrapers;
using AutoGuia.Core.DTOs;
using System.Net;

namespace AutoGuia.Scraper.Tests;

/// <summary>
/// Tests unitarios para ConsumiblesScraperService
/// Valida la extracción correcta de datos desde MercadoLibre
/// </summary>
public class ConsumiblesScraperServiceTests
{
    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
    private readonly Mock<ILogger<ConsumiblesScraperService>> _mockLogger;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly ConsumiblesScraperService _scraperService;

    public ConsumiblesScraperServiceTests()
    {
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockLogger = new Mock<ILogger<ConsumiblesScraperService>>();
        _mockConfiguration = new Mock<IConfiguration>();

        _scraperService = new ConsumiblesScraperService(
            _mockHttpClientFactory.Object,
            _mockLogger.Object,
            _mockConfiguration.Object);
    }

    [Fact]
    public async Task BuscarConsumiblesAsync_ConTerminoVacio_DebeRetornarListaVacia()
    {
        // Arrange
        var termino = "";

        // Act
        var resultado = await _scraperService.BuscarConsumiblesAsync(termino);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();
        
        // Verificar que se logueó la advertencia
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("término de búsqueda está vacío")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task BuscarConsumiblesAsync_ConTerminoNulo_DebeRetornarListaVacia()
    {
        // Arrange
        string? termino = null;

        // Act
        var resultado = await _scraperService.BuscarConsumiblesAsync(termino!);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();
    }

    [Theory]
    [InlineData("Aceite 10W-40 Castrol")]
    [InlineData("Filtro aire Toyota")]
    [InlineData("Batería 12V")]
    public async Task BuscarConsumiblesAsync_ConTerminoValido_DebeLogearInicioDeBusqueda(string termino)
    {
        // Arrange
        var mockHttpClient = new HttpClient(new MockHttpMessageHandler(HttpStatusCode.OK, "<html></html>"));
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(mockHttpClient);

        // Act
        await _scraperService.BuscarConsumiblesAsync(termino);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Iniciando búsqueda") && v.ToString()!.Contains(termino)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task BuscarConsumiblesAsync_ConHTMLVacio_DebeRetornarListaVacia()
    {
        // Arrange
        var termino = "Aceite";
        var mockHttpClient = new HttpClient(new MockHttpMessageHandler(HttpStatusCode.OK, ""));
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(mockHttpClient);

        // Act
        var resultado = await _scraperService.BuscarConsumiblesAsync(termino);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task BuscarConsumiblesAsync_ConHTMLValido_DebeExtraerProductos()
    {
        // Arrange
        var termino = "Aceite";
        var htmlMercadoLibre = @"
        <html>
            <body>
                <div>
                    <li class='ui-search-layout__item'>
                        <h2 class='ui-search-item__title'>Aceite Castrol 10W-40 4L</h2>
                        <span class='andes-money-amount__fraction'>25990</span>
                        <a class='ui-search-link' href='https://mercadolibre.cl/producto1'></a>
                        <img class='ui-search-result-image__element' src='https://example.com/imagen1.jpg' />
                    </li>
                    <li class='ui-search-layout__item'>
                        <h2 class='ui-search-item__title'>Aceite Shell Helix 10W-40 1L</h2>
                        <span class='andes-money-amount__fraction'>8500</span>
                        <a class='ui-search-link' href='https://mercadolibre.cl/producto2'></a>
                        <img class='ui-search-result-image__element' src='https://example.com/imagen2.jpg' />
                    </li>
                </div>
            </body>
        </html>";

        var mockHttpClient = new HttpClient(new MockHttpMessageHandler(HttpStatusCode.OK, htmlMercadoLibre));
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(mockHttpClient);

        // Act
        var resultado = await _scraperService.BuscarConsumiblesAsync(termino);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCountGreaterThan(0);
        
        var primeraOferta = resultado.First();
        primeraOferta.Should().NotBeNull();
        primeraOferta.ProductoNombre.Should().Contain("Aceite");
        primeraOferta.TiendaNombre.Should().Be("MercadoLibre");
        primeraOferta.Precio.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task BuscarConsumiblesAsync_ConHTMLProductoCompleto_DebeMapearTodosCampos()
    {
        // Arrange
        var termino = "Aceite";
        var htmlProductoCompleto = @"
        <html>
            <body>
                <li class='ui-search-layout__item'>
                    <h2 class='ui-search-item__title'>Aceite Mobil 1 5W-30 Sintético</h2>
                    <span class='andes-money-amount__fraction'>35990</span>
                    <a class='ui-search-link' href='https://mercadolibre.cl/aceite-mobil-123'></a>
                    <img class='ui-search-result-image__element' src='https://example.com/mobil.jpg' />
                </li>
            </body>
        </html>";

        var mockHttpClient = new HttpClient(new MockHttpMessageHandler(HttpStatusCode.OK, htmlProductoCompleto));
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(mockHttpClient);

        // Act
        var resultado = await _scraperService.BuscarConsumiblesAsync(termino);

        // Assert
        resultado.Should().HaveCount(1);
        
        var oferta = resultado.First();
        oferta.ProductoNombre.Should().Be("Aceite Mobil 1 5W-30 Sintético");
        oferta.Precio.Should().Be(35990);
        oferta.UrlProductoEnTienda.Should().Be("https://mercadolibre.cl/aceite-mobil-123");
        oferta.ProductoImagen.Should().Be("https://example.com/mobil.jpg");
        oferta.TiendaNombre.Should().Be("MercadoLibre");
        oferta.EsDisponible.Should().BeTrue();
        oferta.FechaActualizacion.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Theory]
    [InlineData("25.990", 25990)]      // Formato con punto de miles
    [InlineData("8500", 8500)]         // Formato sin separador
    [InlineData("1.250.000", 1250000)] // Formato con múltiples puntos
    public async Task BuscarConsumiblesAsync_ConDiferentesFormatosPrecio_DebeParsearCorrectamente(string precioHtml, decimal precioEsperado)
    {
        // Arrange
        var termino = "Producto";
        var html = $@"
        <html>
            <body>
                <li class='ui-search-layout__item'>
                    <h2 class='ui-search-item__title'>Producto Test</h2>
                    <span class='andes-money-amount__fraction'>{precioHtml}</span>
                    <a class='ui-search-link' href='https://mercadolibre.cl/test'></a>
                    <img class='ui-search-result-image__element' src='https://example.com/test.jpg' />
                </li>
            </body>
        </html>";

        var mockHttpClient = new HttpClient(new MockHttpMessageHandler(HttpStatusCode.OK, html));
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(mockHttpClient);

        // Act
        var resultado = await _scraperService.BuscarConsumiblesAsync(termino);

        // Assert
        resultado.Should().HaveCount(1);
        resultado.First().Precio.Should().Be(precioEsperado);
    }

    [Fact]
    public async Task BuscarConsumiblesAsync_ConErrorHTTP_DebeRetornarListaVaciaYLoguearError()
    {
        // Arrange
        var termino = "Aceite";
        var mockHttpClient = new HttpClient(new MockHttpMessageHandler(HttpStatusCode.InternalServerError, ""));
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(mockHttpClient);

        // Act
        var resultado = await _scraperService.BuscarConsumiblesAsync(termino);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();
        
        // Verificar que se logueó el error
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task BuscarConsumiblesAsync_ConProductoSinPrecio_NoDebeIncluirProducto()
    {
        // Arrange
        var termino = "Aceite";
        var htmlSinPrecio = @"
        <html>
            <body>
                <li class='ui-search-layout__item'>
                    <h2 class='ui-search-item__title'>Aceite Sin Precio</h2>
                    <a class='ui-search-link' href='https://mercadolibre.cl/producto'></a>
                    <img class='ui-search-result-image__element' src='https://example.com/imagen.jpg' />
                </li>
            </body>
        </html>";

        var mockHttpClient = new HttpClient(new MockHttpMessageHandler(HttpStatusCode.OK, htmlSinPrecio));
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(mockHttpClient);

        // Act
        var resultado = await _scraperService.BuscarConsumiblesAsync(termino);

        // Assert
        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task BuscarConsumiblesAsync_ConMultiplesProductos_DebeRetornarTodosLosValidos()
    {
        // Arrange
        var termino = "Filtro";
        var htmlMultiples = @"
        <html>
            <body>
                <li class='ui-search-layout__item'>
                    <h2 class='ui-search-item__title'>Filtro Aire Mann</h2>
                    <span class='andes-money-amount__fraction'>12500</span>
                    <a class='ui-search-link' href='https://mercadolibre.cl/filtro1'></a>
                    <img class='ui-search-result-image__element' src='https://example.com/f1.jpg' />
                </li>
                <li class='ui-search-layout__item'>
                    <h2 class='ui-search-item__title'>Filtro Aceite Bosch</h2>
                    <span class='andes-money-amount__fraction'>8900</span>
                    <a class='ui-search-link' href='https://mercadolibre.cl/filtro2'></a>
                    <img class='ui-search-result-image__element' src='https://example.com/f2.jpg' />
                </li>
                <li class='ui-search-layout__item'>
                    <h2 class='ui-search-item__title'>Filtro Combustible Fram</h2>
                    <span class='andes-money-amount__fraction'>15990</span>
                    <a class='ui-search-link' href='https://mercadolibre.cl/filtro3'></a>
                    <img class='ui-search-result-image__element' src='https://example.com/f3.jpg' />
                </li>
            </body>
        </html>";

        var mockHttpClient = new HttpClient(new MockHttpMessageHandler(HttpStatusCode.OK, htmlMultiples));
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(mockHttpClient);

        // Act
        var resultado = await _scraperService.BuscarConsumiblesAsync(termino);

        // Assert
        resultado.Should().HaveCount(3);
        resultado.Should().OnlyContain(o => o.TiendaNombre == "MercadoLibre");
        resultado.Should().OnlyContain(o => o.Precio > 0);
        resultado.Select(o => o.ProductoNombre).Should().AllSatisfy(n => n.Should().Contain("Filtro"));
    }

    [Fact]
    public async Task ValidarDisponibilidadAsync_CuandoTiendaEstaDisponible_DebeRetornarTrue()
    {
        // Arrange
        var mockHttpClient = new HttpClient(new MockHttpMessageHandler(HttpStatusCode.OK, "<html></html>"));
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(mockHttpClient);

        // Act
        var disponible = await _scraperService.ValidarDisponibilidadAsync();

        // Assert
        disponible.Should().BeTrue();
    }

    [Fact]
    public async Task ValidarDisponibilidadAsync_CuandoTiendaNoEstaDisponible_DebeRetornarFalse()
    {
        // Arrange
        var mockHttpClient = new HttpClient(new MockHttpMessageHandler(HttpStatusCode.ServiceUnavailable, ""));
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(mockHttpClient);

        // Act
        var disponible = await _scraperService.ValidarDisponibilidadAsync();

        // Assert
        disponible.Should().BeFalse();
    }
}

/// <summary>
/// Mock de HttpMessageHandler para simular respuestas HTTP en tests
/// </summary>
public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly HttpStatusCode _statusCode;
    private readonly string _content;

    public MockHttpMessageHandler(HttpStatusCode statusCode, string content)
    {
        _statusCode = statusCode;
        _content = content;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = new HttpResponseMessage(_statusCode)
        {
            Content = new StringContent(_content)
        };

        return Task.FromResult(response);
    }
}
