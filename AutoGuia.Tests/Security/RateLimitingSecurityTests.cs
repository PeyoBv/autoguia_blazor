using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using AutoGuia.Web;

namespace AutoGuia.Tests.Security;

/// <summary>
/// Tests de seguridad para validar la implementación de Rate Limiting.
/// Verifica que el sistema previene abusos mediante límites de peticiones por tiempo.
/// </summary>
public class RateLimitingSecurityTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public RateLimitingSecurityTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Test: Verifica que después de exceder el límite de rate (10 req/min), 
    /// el servidor retorna HTTP 429 (Too Many Requests).
    /// 
    /// PROPÓSITO: Validar protección contra DoS en endpoint de diagnóstico.
    /// </summary>
    [Fact]
    public async Task Test_Rate_Limit_Exceeded_Returns_429()
    {
        // Arrange
        var client = _factory.CreateClient();
        const int maxRequests = 10;
        const string endpoint = "/api/diagnostico/diagnosticar";
        
        var responses = new List<HttpResponseMessage>();

        // Act: Realizar 15 peticiones (5 más del límite de 10/min)
        for (int i = 0; i < maxRequests + 5; i++)
        {
            var response = await client.GetAsync(endpoint);
            responses.Add(response);
        }

        // Assert: Al menos una petición debe retornar 429
        var tooManyRequestsResponses = responses.Where(r => r.StatusCode == HttpStatusCode.TooManyRequests).ToList();
        
        Assert.True(
            tooManyRequestsResponses.Any(), 
            $"Se esperaba al menos un HTTP 429 después de {maxRequests} peticiones, pero todas retornaron: {string.Join(", ", responses.Select(r => (int)r.StatusCode))}"
        );

        // Cleanup
        foreach (var response in responses)
        {
            response.Dispose();
        }
    }

    /// <summary>
    /// Test: Verifica que después de pasar el período de rate limiting (1 minuto),
    /// el contador se reinicia y permite nuevas peticiones.
    /// 
    /// PROPÓSITO: Asegurar que el sistema no bloquea permanentemente a usuarios legítimos.
    /// NOTA: Test marcado como Skip debido a tiempo de ejecución (61 segundos).
    /// </summary>
    [Fact(Skip = "Test requiere esperar 61 segundos para validar reset del rate limit")]
    public async Task Test_Rate_Limit_Reset_After_Period()
    {
        // Arrange
        var client = _factory.CreateClient();
        const string endpoint = "/api/diagnostico/diagnosticar";

        // Act 1: Hacer 10 peticiones para alcanzar el límite
        for (int i = 0; i < 10; i++)
        {
            await client.GetAsync(endpoint);
        }

        // Act 2: Esperar 61 segundos (período de reset + margen)
        await Task.Delay(TimeSpan.FromSeconds(61));

        // Act 3: Hacer nueva petición después del reset
        var responseAfterReset = await client.GetAsync(endpoint);

        // Assert: La petición después del reset NO debe ser 429
        Assert.NotEqual(HttpStatusCode.TooManyRequests, responseAfterReset.StatusCode);
    }

    /// <summary>
    /// Test: Verifica que todos los security headers críticos están presentes en la respuesta.
    /// 
    /// PROPÓSITO: Validar configuración completa de SecurityHeadersMiddleware.
    /// Headers validados:
    /// - Content-Security-Policy (CSP) - Protección XSS
    /// - X-Content-Type-Options - Anti MIME sniffing
    /// - X-Frame-Options - Anti Clickjacking
    /// - X-XSS-Protection - Protección legacy XSS
    /// - Referrer-Policy - Control de referrer info
    /// </summary>
    [Fact]
    public async Task Test_Security_Headers_Present_In_Response()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act: Realizar petición GET a la home page
        var response = await client.GetAsync("/");

        // Assert: Verificar presencia de headers críticos
        var headers = response.Headers;
        var contentHeaders = response.Content.Headers;

        // 1. Content-Security-Policy (CSP)
        Assert.True(
            headers.Contains("Content-Security-Policy"),
            "Falta header Content-Security-Policy (protección XSS)"
        );

        // 2. X-Content-Type-Options
        Assert.True(
            headers.Contains("X-Content-Type-Options"),
            "Falta header X-Content-Type-Options (anti MIME sniffing)"
        );
        
        if (headers.Contains("X-Content-Type-Options"))
        {
            var values = headers.GetValues("X-Content-Type-Options");
            Assert.Contains("nosniff", values);
        }

        // 3. X-Frame-Options
        Assert.True(
            headers.Contains("X-Frame-Options"),
            "Falta header X-Frame-Options (anti clickjacking)"
        );

        // 4. X-XSS-Protection
        Assert.True(
            headers.Contains("X-XSS-Protection"),
            "Falta header X-XSS-Protection (protección XSS legacy)"
        );

        // 5. Referrer-Policy
        Assert.True(
            headers.Contains("Referrer-Policy"),
            "Falta header Referrer-Policy (control de referrer)"
        );

        // 6. Verificar que NO se exponen headers sensibles
        Assert.False(
            headers.Contains("Server"),
            "Header 'Server' debe estar removido (revelación de información)"
        );
        
        Assert.False(
            headers.Contains("X-Powered-By"),
            "Header 'X-Powered-By' debe estar removido (revelación de información)"
        );
    }
}
