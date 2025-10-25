using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace AutoGuia.Web.Services;

/// <summary>
/// Servicio de envío de emails usando MailKit
/// </summary>
public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _smtpEmail;
    private readonly string _smtpPassword;
    private readonly string _fromName;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        // Cargar configuración SMTP
        _smtpHost = _configuration["Smtp:Host"] ?? "smtp.gmail.com";
        _smtpPort = int.Parse(_configuration["Smtp:Port"] ?? "587");
        _smtpEmail = _configuration["Smtp:Email"] ?? throw new InvalidOperationException("Smtp:Email no está configurado");
        _smtpPassword = _configuration["Smtp:Password"] ?? throw new InvalidOperationException("Smtp:Password no está configurado");
        _fromName = _configuration["Smtp:FromName"] ?? "AutoGuía";
    }

    /// <summary>
    /// Envía un email de bienvenida personalizado
    /// </summary>
    public async Task<bool> EnviarEmailBienvenidaAsync(string destinatario, string nombreUsuario, string? confirmationUrl = null)
    {
        try
        {
            var htmlBody = GenerarPlantillaBienvenida(nombreUsuario, confirmationUrl);
            return await EnviarEmailAsync(destinatario, "¡Bienvenido a AutoGuía! 🚗", htmlBody);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar email de bienvenida a {Email}", destinatario);
            return false;
        }
    }

    /// <summary>
    /// Envía un email genérico con contenido HTML
    /// </summary>
    public async Task<bool> EnviarEmailAsync(string destinatario, string asunto, string htmlBody)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_fromName, _smtpEmail));
            message.To.Add(new MailboxAddress("", destinatario));
            message.Subject = asunto;

            var builder = new BodyBuilder
            {
                HtmlBody = htmlBody
            };

            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            
            // Conectar al servidor SMTP
            await client.ConnectAsync(_smtpHost, _smtpPort, SecureSocketOptions.StartTls);
            
            // Autenticar
            await client.AuthenticateAsync(_smtpEmail, _smtpPassword);
            
            // Enviar email
            await client.SendAsync(message);
            
            // Desconectar
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email enviado exitosamente a {Email} con asunto: {Subject}", destinatario, asunto);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar email a {Email}", destinatario);
            return false;
        }
    }

    /// <summary>
    /// Genera la plantilla HTML de bienvenida
    /// </summary>
    private string GenerarPlantillaBienvenida(string nombreUsuario, string? confirmationUrl)
    {
        var confirmButton = string.IsNullOrEmpty(confirmationUrl)
            ? ""
            : $@"
                <div style='text-align: center; margin: 30px 0;'>
                    <a href='{confirmationUrl}' style='
                        background-color: #007bff;
                        color: white;
                        padding: 12px 30px;
                        text-decoration: none;
                        border-radius: 5px;
                        display: inline-block;
                        font-weight: bold;'>
                        Confirmar mi cuenta
                    </a>
                </div>";

        return $@"
<!DOCTYPE html>
<html lang='es'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Bienvenido a AutoGuía</title>
</head>
<body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;'>
    <table width='100%' cellpadding='0' cellspacing='0' style='background-color: #f4f4f4; padding: 20px;'>
        <tr>
            <td align='center'>
                <table width='600' cellpadding='0' cellspacing='0' style='background-color: white; border-radius: 10px; overflow: hidden; box-shadow: 0 2px 10px rgba(0,0,0,0.1);'>
                    <!-- Header -->
                    <tr>
                        <td style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px; text-align: center;'>
                            <h1 style='color: white; margin: 0; font-size: 28px;'>
                                🚗 AutoGuía
                            </h1>
                            <p style='color: rgba(255,255,255,0.9); margin: 10px 0 0 0; font-size: 16px;'>
                                Tu guía automotriz en Chile
                            </p>
                        </td>
                    </tr>
                    
                    <!-- Body -->
                    <tr>
                        <td style='padding: 40px;'>
                            <h2 style='color: #333; margin-top: 0;'>
                                ¡Hola, {nombreUsuario}! 👋
                            </h2>
                            
                            <p style='color: #666; line-height: 1.6; font-size: 16px;'>
                                ¡Bienvenido a <strong>AutoGuía</strong>! Estamos emocionados de tenerte con nosotros.
                            </p>
                            
                            <p style='color: #666; line-height: 1.6; font-size: 16px;'>
                                Tu cuenta ha sido creada exitosamente. Ahora puedes disfrutar de:
                            </p>
                            
                            <ul style='color: #666; line-height: 1.8; font-size: 15px;'>
                                <li>🔧 <strong>Encuentra talleres mecánicos</strong> cerca de ti</li>
                                <li>💬 <strong>Participa en el foro</strong> de la comunidad automotriz</li>
                                <li>🛒 <strong>Compara precios</strong> de repuestos y consumibles</li>
                                <li>🤖 <strong>Diagnóstico con IA</strong> para problemas vehiculares</li>
                                <li>🗺️ <strong>Mapas interactivos</strong> de talleres</li>
                            </ul>
                            
                            {confirmButton}
                            
                            <p style='color: #666; line-height: 1.6; font-size: 16px;'>
                                Si tienes alguna pregunta o necesitas ayuda, no dudes en contactarnos.
                            </p>
                            
                            <p style='color: #666; line-height: 1.6; font-size: 16px;'>
                                ¡Gracias por unirte a nuestra comunidad! 🎉
                            </p>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style='background-color: #f8f9fa; padding: 30px; text-align: center; border-top: 1px solid #dee2e6;'>
                            <p style='color: #6c757d; margin: 0; font-size: 14px;'>
                                <strong>AutoGuía</strong> - Tu aliado en el mundo automotriz
                            </p>
                            <p style='color: #6c757d; margin: 10px 0 0 0; font-size: 12px;'>
                                Este es un email automático, por favor no respondas a este mensaje.
                            </p>
                            <p style='color: #6c757d; margin: 10px 0 0 0; font-size: 12px;'>
                                © 2025 AutoGuía. Todos los derechos reservados.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }
}
