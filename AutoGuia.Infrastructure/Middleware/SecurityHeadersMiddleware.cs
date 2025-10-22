using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

namespace AutoGuia.Infrastructure.Middleware;

/// <summary>
/// Middleware para agregar headers de seguridad HTTP que protegen contra varios vectores de ataque.
/// Implementa protecciones contra XSS, Clickjacking, MIME sniffing, y otras vulnerabilidades comunes.
/// </summary>
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SecurityHeadersMiddleware> _logger;

    public SecurityHeadersMiddleware(RequestDelegate next, ILogger<SecurityHeadersMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Content Security Policy (CSP) - Protección principal contra XSS
        // Política estricta que solo permite recursos del mismo origen
        context.Response.Headers.Append("Content-Security-Policy",
            "default-src 'self'; " +
            "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com; " +
            "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com https://fonts.googleapis.com; " +
            "font-src 'self' https://fonts.gstatic.com https://cdnjs.cloudflare.com; " +
            "img-src 'self' data: https: blob:; " +
            "connect-src 'self' https://api.autoguia.cl; " +
            "frame-ancestors 'self'; " +
            "base-uri 'self'; " +
            "form-action 'self'; " +
            "upgrade-insecure-requests;");

        // X-Content-Type-Options - Previene MIME sniffing
        // Fuerza al navegador a respetar el Content-Type declarado
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

        // X-Frame-Options - Protección contra Clickjacking
        // Previene que la página sea embebida en un iframe externo
        context.Response.Headers.Append("X-Frame-Options", "SAMEORIGIN");

        // X-XSS-Protection - Protección XSS legacy para navegadores antiguos
        // Nota: Obsoleto en navegadores modernos, pero útil para compatibilidad
        context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

        // Referrer-Policy - Controla cuánta información del referrer se envía
        // Balancea privacidad con funcionalidad de analytics
        context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

        // Permissions-Policy - Controla acceso a APIs del navegador
        // Deshabilita features no utilizadas para reducir superficie de ataque
        context.Response.Headers.Append("Permissions-Policy",
            "accelerometer=(), " +
            "camera=(), " +
            "geolocation=(self), " +
            "gyroscope=(), " +
            "magnetometer=(), " +
            "microphone=(), " +
            "payment=(), " +
            "usb=()");

        // Strict-Transport-Security (HSTS) - Fuerza HTTPS
        // Solo para producción con HTTPS habilitado
        if (context.Request.IsHttps)
        {
            context.Response.Headers.Append("Strict-Transport-Security",
                "max-age=31536000; includeSubDomains; preload");
        }

        // Remover headers que revelan información del servidor
        context.Response.Headers.Remove("Server");
        context.Response.Headers.Remove("X-Powered-By");
        context.Response.Headers.Remove("X-AspNet-Version");

        _logger.LogDebug("Security headers aplicados a la respuesta para {Path}", context.Request.Path);

        await _next(context);
    }
}

/// <summary>
/// Métodos de extensión para registrar el middleware de seguridad.
/// </summary>
public static class SecurityHeadersMiddlewareExtensions
{
    /// <summary>
    /// Agrega el middleware de security headers al pipeline de la aplicación.
    /// Debe llamarse temprano en el pipeline para asegurar que todos los responses tengan los headers.
    /// </summary>
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SecurityHeadersMiddleware>();
    }
}
