using Ganss.Xss;
using Microsoft.Extensions.Logging;

namespace AutoGuia.Infrastructure.Services;

/// <summary>
/// Servicio para sanitizar contenido HTML y proteger contra ataques XSS.
/// Utiliza HtmlSanitizer con whitelist de tags y atributos seguros.
/// </summary>
public class HtmlSanitizationService : IHtmlSanitizationService
{
    private readonly HtmlSanitizer _basicSanitizer;
    private readonly HtmlSanitizer _formattingSanitizer;
    private readonly ILogger<HtmlSanitizationService> _logger;

    public HtmlSanitizationService(ILogger<HtmlSanitizationService> logger)
    {
        _logger = logger;
        
        // Sanitizador básico - solo texto sin formato
        _basicSanitizer = new HtmlSanitizer();
        _basicSanitizer.AllowedTags.Clear();
        _basicSanitizer.AllowedAttributes.Clear();
        _basicSanitizer.AllowedCssProperties.Clear();
        _basicSanitizer.AllowedSchemes.Clear();
        
        // Sanitizador con formato básico permitido
        _formattingSanitizer = new HtmlSanitizer();
        
        // Whitelist de tags seguros para formato básico
        _formattingSanitizer.AllowedTags.Clear();
        _formattingSanitizer.AllowedTags.Add("b");
        _formattingSanitizer.AllowedTags.Add("strong");
        _formattingSanitizer.AllowedTags.Add("i");
        _formattingSanitizer.AllowedTags.Add("em");
        _formattingSanitizer.AllowedTags.Add("u");
        _formattingSanitizer.AllowedTags.Add("p");
        _formattingSanitizer.AllowedTags.Add("br");
        _formattingSanitizer.AllowedTags.Add("ul");
        _formattingSanitizer.AllowedTags.Add("ol");
        _formattingSanitizer.AllowedTags.Add("li");
        _formattingSanitizer.AllowedTags.Add("a");
        _formattingSanitizer.AllowedTags.Add("blockquote");
        _formattingSanitizer.AllowedTags.Add("code");
        _formattingSanitizer.AllowedTags.Add("pre");
        
        // Whitelist de atributos seguros
        _formattingSanitizer.AllowedAttributes.Clear();
        _formattingSanitizer.AllowedAttributes.Add("href");
        _formattingSanitizer.AllowedAttributes.Add("title");
        _formattingSanitizer.AllowedAttributes.Add("rel");
        _formattingSanitizer.AllowedAttributes.Add("target");
        
        // Whitelist de esquemas de URL seguros
        _formattingSanitizer.AllowedSchemes.Clear();
        _formattingSanitizer.AllowedSchemes.Add("http");
        _formattingSanitizer.AllowedSchemes.Add("https");
        _formattingSanitizer.AllowedSchemes.Add("mailto");
        
        // No permitir CSS inline
        _formattingSanitizer.AllowedCssProperties.Clear();
        
        // Configuraciones de seguridad adicionales
        _formattingSanitizer.AllowDataAttributes = false;
        _formattingSanitizer.KeepChildNodes = true;
    }

    public string Sanitize(string? unsafeHtml)
    {
        if (string.IsNullOrWhiteSpace(unsafeHtml))
        {
            return string.Empty;
        }

        try
        {
            var sanitized = _basicSanitizer.Sanitize(unsafeHtml);
            
            // Log si se removió contenido peligroso
            if (sanitized != unsafeHtml)
            {
                _logger.LogWarning("Contenido HTML potencialmente peligroso fue sanitizado. Longitud original: {Original}, Longitud sanitizada: {Sanitized}", 
                    unsafeHtml.Length, sanitized.Length);
            }
            
            return sanitized;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al sanitizar HTML. Se retornará cadena vacía.");
            return string.Empty;
        }
    }

    public string SanitizeWithBasicFormatting(string? unsafeHtml)
    {
        if (string.IsNullOrWhiteSpace(unsafeHtml))
        {
            return string.Empty;
        }

        try
        {
            var sanitized = _formattingSanitizer.Sanitize(unsafeHtml);
            
            // Log si se removió contenido peligroso
            if (sanitized != unsafeHtml)
            {
                _logger.LogWarning("Contenido HTML con formato fue sanitizado. Longitud original: {Original}, Longitud sanitizada: {Sanitized}", 
                    unsafeHtml.Length, sanitized.Length);
            }
            
            return sanitized;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al sanitizar HTML con formato. Se retornará cadena vacía.");
            return string.Empty;
        }
    }
}
