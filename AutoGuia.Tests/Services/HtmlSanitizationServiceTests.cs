using Xunit;
using Microsoft.Extensions.Logging;
using Moq;
using AutoGuia.Infrastructure.Services;

namespace AutoGuia.Tests.Services;

/// <summary>
/// Tests para validar la sanitización HTML contra ataques XSS
/// </summary>
public class HtmlSanitizationServiceTests
{
    private readonly HtmlSanitizationService _sanitizationService;
    private readonly Mock<ILogger<HtmlSanitizationService>> _mockLogger;

    public HtmlSanitizationServiceTests()
    {
        _mockLogger = new Mock<ILogger<HtmlSanitizationService>>();
        _sanitizationService = new HtmlSanitizationService(_mockLogger.Object);
    }

    [Fact]
    public void Sanitize_DebeRemoverScriptTags()
    {
        // Arrange
        var inputMalicioso = "<p>Contenido normal</p><script>alert('XSS')</script>";

        // Act
        var resultado = _sanitizationService.Sanitize(inputMalicioso);

        // Assert
        Assert.DoesNotContain("<script>", resultado);
        Assert.DoesNotContain("alert", resultado);
        Assert.DoesNotContain("XSS", resultado);
    }

    [Fact]
    public void Sanitize_DebeRemoverEventHandlers()
    {
        // Arrange
        var inputMalicioso = "<div onclick=\"alert('XSS')\">Click me</div>";

        // Act
        var resultado = _sanitizationService.Sanitize(inputMalicioso);

        // Assert
        Assert.DoesNotContain("onclick", resultado);
        Assert.DoesNotContain("alert", resultado);
    }

    [Fact]
    public void Sanitize_DebeRemoverJavaScriptProtocol()
    {
        // Arrange
        var inputMalicioso = "<a href=\"javascript:alert('XSS')\">Click</a>";

        // Act
        var resultado = _sanitizationService.Sanitize(inputMalicioso);

        // Assert
        Assert.DoesNotContain("javascript:", resultado);
        Assert.DoesNotContain("alert", resultado);
    }

    [Fact]
    public void Sanitize_DebeRemoverIframeMalicioso()
    {
        // Arrange
        var inputMalicioso = "<iframe src=\"http://malicious.com\"></iframe>";

        // Act
        var resultado = _sanitizationService.Sanitize(inputMalicioso);

        // Assert
        Assert.DoesNotContain("<iframe", resultado);
        Assert.DoesNotContain("malicious.com", resultado);
    }

    [Fact]
    public void Sanitize_DebeRemoverStyleConExpresion()
    {
        // Arrange
        var inputMalicioso = "<div style=\"background:url('javascript:alert(1)')\">Test</div>";

        // Act
        var resultado = _sanitizationService.Sanitize(inputMalicioso);

        // Assert
        Assert.DoesNotContain("javascript:", resultado);
        Assert.DoesNotContain("alert", resultado);
    }

    [Fact]
    public void Sanitize_DebePermitirTextoLimpio()
    {
        // Arrange
        var inputLimpio = "Este es un texto completamente seguro sin HTML";

        // Act
        var resultado = _sanitizationService.Sanitize(inputLimpio);

        // Assert
        Assert.Equal(inputLimpio, resultado);
    }

    [Fact]
    public void Sanitize_DebeManejarCadenaVacia()
    {
        // Act
        var resultado = _sanitizationService.Sanitize("");

        // Assert
        Assert.Equal(string.Empty, resultado);
    }

    [Fact]
    public void Sanitize_DebeManejarNull()
    {
        // Act
        var resultado = _sanitizationService.Sanitize(null);

        // Assert
        Assert.Equal(string.Empty, resultado);
    }

    [Fact]
    public void SanitizeWithBasicFormatting_DebePermitirTagsBasicos()
    {
        // Arrange
        var inputConFormato = "<p>Este es un <strong>texto en negrita</strong> y <em>cursiva</em></p>";

        // Act
        var resultado = _sanitizationService.SanitizeWithBasicFormatting(inputConFormato);

        // Assert
        Assert.Contains("<strong>", resultado);
        Assert.Contains("<em>", resultado);
        Assert.Contains("texto en negrita", resultado);
    }

    [Fact]
    public void SanitizeWithBasicFormatting_DebeRemoverScriptsPeroPermitirFormato()
    {
        // Arrange
        var inputMixto = "<p>Texto normal</p><script>alert('XSS')</script><strong>Negrita</strong>";

        // Act
        var resultado = _sanitizationService.SanitizeWithBasicFormatting(inputMixto);

        // Assert
        Assert.DoesNotContain("<script>", resultado);
        Assert.DoesNotContain("</script>", resultado);
        Assert.Contains("<strong>", resultado);
        Assert.Contains("Negrita", resultado);
        // Nota: El contenido del script puede permanecer como texto, pero sin las etiquetas
    }

    [Fact]
    public void SanitizeWithBasicFormatting_DebePermitirEnlacesHTTPS()
    {
        // Arrange
        var inputConEnlace = "<p>Visita <a href=\"https://example.com\">este enlace</a></p>";

        // Act
        var resultado = _sanitizationService.SanitizeWithBasicFormatting(inputConEnlace);

        // Assert
        Assert.Contains("<a href=\"https://example.com\">", resultado);
        Assert.Contains("este enlace", resultado);
    }

    [Fact]
    public void SanitizeWithBasicFormatting_DebeRemoverEnlacesJavaScript()
    {
        // Arrange
        var inputMalicioso = "<a href=\"javascript:alert('XSS')\">Click</a>";

        // Act
        var resultado = _sanitizationService.SanitizeWithBasicFormatting(inputMalicioso);

        // Assert
        Assert.DoesNotContain("javascript:", resultado);
        Assert.DoesNotContain("alert", resultado);
    }

    [Fact]
    public void SanitizeWithBasicFormatting_DebePermitirListas()
    {
        // Arrange
        var inputConLista = "<ul><li>Item 1</li><li>Item 2</li></ul>";

        // Act
        var resultado = _sanitizationService.SanitizeWithBasicFormatting(inputConLista);

        // Assert
        Assert.Contains("<ul>", resultado);
        Assert.Contains("<li>", resultado);
        Assert.Contains("Item 1", resultado);
    }

    [Theory]
    [InlineData("<img src=x onerror=alert('XSS')>")]
    [InlineData("<svg onload=alert('XSS')>")]
    [InlineData("<body onload=alert('XSS')>")]
    [InlineData("<input onfocus=alert('XSS') autofocus>")]
    [InlineData("<select onfocus=alert('XSS') autofocus>")]
    [InlineData("<textarea onfocus=alert('XSS') autofocus>")]
    [InlineData("<object data='javascript:alert(1)'>")]
    [InlineData("<embed src='javascript:alert(1)'>")]
    public void Sanitize_DebeBloquearMultiplesVectoresDeAtaqueXSS(string payloadXSS)
    {
        // Act
        var resultado = _sanitizationService.Sanitize(payloadXSS);

        // Assert
        Assert.DoesNotContain("alert", resultado);
        Assert.DoesNotContain("javascript:", resultado);
        Assert.DoesNotContain("onerror", resultado);
        Assert.DoesNotContain("onload", resultado);
        Assert.DoesNotContain("onfocus", resultado);
    }

    [Fact]
    public void Sanitize_DebeLoggearCuandoSeRemuevContenidoPeligroso()
    {
        // Arrange
        var inputMalicioso = "<script>alert('XSS')</script>";

        // Act
        var resultado = _sanitizationService.Sanitize(inputMalicioso);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("potencialmente peligroso")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.AtLeastOnce);
    }
}
