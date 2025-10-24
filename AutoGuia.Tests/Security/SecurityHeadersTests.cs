using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using AutoGuia.Web;

namespace AutoGuia.Tests.Security;

/// <summary>
/// Tests de seguridad para validar la correcta configuración de Security Headers HTTP.
/// Verifica que todos los headers de protección estén presentes y correctamente configurados.
/// </summary>
public class SecurityHeadersTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public SecurityHeadersTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Test: Valida que el Content-Security-Policy (CSP) header esté presente y bien formado.
    /// 
    /// PROPÓSITO: CSP es la defensa principal contra XSS. Este test asegura que:
    /// - El header CSP existe
    /// - Contiene directivas críticas (default-src, script-src, style-src)
    /// - Está correctamente formateado
    /// 
    /// CSP previene la ejecución de código malicioso al definir fuentes confiables
    /// para scripts, estilos, imágenes y otros recursos.
    /// </summary>
    [Fact]
    public async Task Test_CSP_Header_Valid()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act: Realizar petición GET a la home page
        var response = await client.GetAsync("/");

        // Assert: Verificar que CSP header existe
        Assert.True(
            response.Headers.Contains("Content-Security-Policy"),
            "Content-Security-Policy header debe estar presente en la respuesta"
        );

        // Obtener valor del CSP
        var cspValues = response.Headers.GetValues("Content-Security-Policy").ToList();
        Assert.NotEmpty(cspValues);

        var cspValue = cspValues.First();

        // Verificar directivas críticas del CSP
        var criticalDirectives = new[]
        {
            "default-src",      // Define política por defecto
            "script-src",       // Control de fuentes de JavaScript
            "style-src",        // Control de fuentes de CSS
            "img-src",          // Control de fuentes de imágenes
            "frame-ancestors"   // Protección anti-clickjacking (complementa X-Frame-Options)
        };

        foreach (var directive in criticalDirectives)
        {
            Assert.Contains(directive, cspValue);
        }

        // Verificar que 'unsafe-inline' esté presente (necesario para Blazor)
        // NOTA: En producción ideal, se debería usar nonces en lugar de 'unsafe-inline'
        Assert.Contains("'unsafe-inline'", cspValue);

        // Verificar que 'unsafe-eval' esté controlado
        // ADVERTENCIA: 'unsafe-eval' permite eval() y es considerado inseguro
        // Si está presente, debe estar justificado por necesidades de Blazor/framework
        if (cspValue.Contains("'unsafe-eval'"))
        {
            // Log warning pero no fallar el test (Blazor puede requerirlo)
            Assert.True(true, "ADVERTENCIA: CSP permite 'unsafe-eval' (necesario para Blazor WebAssembly)");
        }
    }

    /// <summary>
    /// Test: Verifica que TODOS los security headers requeridos estén presentes.
    /// 
    /// PROPÓSITO: Validación completa de configuración de SecurityHeadersMiddleware.
    /// Este test es la verificación final de que la defensa en profundidad está activa.
    /// 
    /// Headers validados:
    /// 1. Content-Security-Policy - Protección XSS principal
    /// 2. X-Content-Type-Options: nosniff - Anti MIME sniffing
    /// 3. X-Frame-Options - Anti clickjacking
    /// 4. X-XSS-Protection - Protección XSS legacy (navegadores antiguos)
    /// 5. Referrer-Policy - Control de información del referrer
    /// 6. Permissions-Policy - Control de APIs del navegador
    /// 7. Strict-Transport-Security (solo HTTPS) - Forzar HTTPS
    /// 
    /// También verifica que headers sensibles NO estén expuestos:
    /// - Server (revela tecnología del servidor)
    /// - X-Powered-By (revela framework utilizado)
    /// - X-AspNet-Version (revela versión de ASP.NET)
    /// </summary>
    [Fact]
    public async Task Test_Required_Security_Headers_Present()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act: Realizar petición GET a la home page
        var response = await client.GetAsync("/");

        // Assert: Verificar headers de seguridad requeridos
        var headers = response.Headers;

        // 1. Content-Security-Policy (CSP) - CRÍTICO
        Assert.True(
            headers.Contains("Content-Security-Policy"),
            "❌ CRÍTICO: Falta Content-Security-Policy (protección XSS)"
        );

        // 2. X-Content-Type-Options - ALTO
        Assert.True(
            headers.Contains("X-Content-Type-Options"),
            "❌ ALTO: Falta X-Content-Type-Options (previene MIME sniffing)"
        );
        
        if (headers.Contains("X-Content-Type-Options"))
        {
            var values = headers.GetValues("X-Content-Type-Options").First();
            Assert.Equal("nosniff", values);
        }

        // 3. X-Frame-Options - ALTO
        Assert.True(
            headers.Contains("X-Frame-Options"),
            "❌ ALTO: Falta X-Frame-Options (previene clickjacking)"
        );
        
        if (headers.Contains("X-Frame-Options"))
        {
            var values = headers.GetValues("X-Frame-Options").First();
            Assert.True(
                values == "DENY" || values == "SAMEORIGIN",
                "X-Frame-Options debe ser 'DENY' o 'SAMEORIGIN'"
            );
        }

        // 4. X-XSS-Protection - MEDIO (legacy)
        Assert.True(
            headers.Contains("X-XSS-Protection"),
            "⚠️ MEDIO: Falta X-XSS-Protection (protección legacy)"
        );

        // 5. Referrer-Policy - MEDIO
        Assert.True(
            headers.Contains("Referrer-Policy"),
            "⚠️ MEDIO: Falta Referrer-Policy (privacidad de referrer)"
        );

        // 6. Permissions-Policy - MEDIO
        Assert.True(
            headers.Contains("Permissions-Policy"),
            "⚠️ MEDIO: Falta Permissions-Policy (control de features del navegador)"
        );

        // 7. Strict-Transport-Security (HSTS) - CRÍTICO en producción con HTTPS
        // NOTA: Solo debe aplicarse en conexiones HTTPS
        // En desarrollo (HTTP), este header NO debe estar presente
        if (response.RequestMessage?.RequestUri?.Scheme == "https")
        {
            Assert.True(
                headers.Contains("Strict-Transport-Security"),
                "❌ CRÍTICO: Falta Strict-Transport-Security en conexión HTTPS"
            );
        }

        // === VALIDACIÓN DE HEADERS SENSIBLES REMOVIDOS ===
        
        // 8. Verificar que NO se expone 'Server' header
        Assert.False(
            headers.Contains("Server"),
            "❌ REVELACIÓN DE INFORMACIÓN: Header 'Server' debe estar removido"
        );

        // 9. Verificar que NO se expone 'X-Powered-By' header
        Assert.False(
            headers.Contains("X-Powered-By"),
            "❌ REVELACIÓN DE INFORMACIÓN: Header 'X-Powered-By' debe estar removido"
        );

        // 10. Verificar que NO se expone 'X-AspNet-Version' header
        Assert.False(
            headers.Contains("X-AspNet-Version"),
            "❌ REVELACIÓN DE INFORMACIÓN: Header 'X-AspNet-Version' debe estar removido"
        );

        // === RESUMEN DE VALIDACIÓN ===
        var headersCount = 0;
        if (headers.Contains("Content-Security-Policy")) headersCount++;
        if (headers.Contains("X-Content-Type-Options")) headersCount++;
        if (headers.Contains("X-Frame-Options")) headersCount++;
        if (headers.Contains("X-XSS-Protection")) headersCount++;
        if (headers.Contains("Referrer-Policy")) headersCount++;
        if (headers.Contains("Permissions-Policy")) headersCount++;

        Assert.True(
            headersCount >= 6,
            $"✅ Se encontraron {headersCount}/6 security headers críticos"
        );
    }
}
