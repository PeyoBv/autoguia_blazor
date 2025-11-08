using Microsoft.AspNetCore.Identity;
using Rodavia.Web.Data;

namespace Rodavia.Web.Services;

/// <summary>
/// Implementación de IEmailSender para ASP.NET Identity que usa nuestro EmailService
/// </summary>
public class IdentityEmailSender : IEmailSender<ApplicationUser>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<IdentityEmailSender> _logger;

    public IdentityEmailSender(IEmailService emailService, ILogger<IdentityEmailSender> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    /// <summary>
    /// Envía email de confirmación con link de activación
    /// </summary>
    public async Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        try
        {
            var userName = user.UserName ?? email.Split('@')[0];
            var success = await _emailService.EnviarEmailBienvenidaAsync(email, userName, confirmationLink);
            
            if (success)
            {
                _logger.LogInformation("Email de confirmación enviado a {Email}", email);
            }
            else
            {
                _logger.LogWarning("No se pudo enviar email de confirmación a {Email}", email);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar email de confirmación a {Email}", email);
        }
    }

    /// <summary>
    /// Envía email de recuperación de contraseña
    /// </summary>
    public async Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
        try
        {
            var htmlBody = $@"
<!DOCTYPE html>
<html lang='es'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Restablecer Contraseña - AutoGuía</title>
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
                                🔐 Restablecer Contraseña
                            </h1>
                        </td>
                    </tr>
                    
                    <!-- Body -->
                    <tr>
                        <td style='padding: 40px;'>
                            <h2 style='color: #333; margin-top: 0;'>
                                Hola, {user.UserName ?? email.Split('@')[0]}
                            </h2>
                            
                            <p style='color: #666; line-height: 1.6; font-size: 16px;'>
                                Recibimos una solicitud para restablecer la contraseña de tu cuenta en <strong>AutoGuía</strong>.
                            </p>
                            
                            <p style='color: #666; line-height: 1.6; font-size: 16px;'>
                                Para restablecer tu contraseña, haz clic en el siguiente botón:
                            </p>
                            
                            <div style='text-align: center; margin: 30px 0;'>
                                <a href='{resetLink}' style='
                                    background-color: #dc3545;
                                    color: white;
                                    padding: 12px 30px;
                                    text-decoration: none;
                                    border-radius: 5px;
                                    display: inline-block;
                                    font-weight: bold;'>
                                    Restablecer mi contraseña
                                </a>
                            </div>
                            
                            <p style='color: #666; line-height: 1.6; font-size: 14px; background-color: #fff3cd; padding: 15px; border-left: 4px solid #ffc107;'>
                                ⚠️ <strong>Importante:</strong> Si no solicitaste restablecer tu contraseña, ignora este email. Tu contraseña permanecerá sin cambios.
                            </p>
                            
                            <p style='color: #999; line-height: 1.6; font-size: 12px; margin-top: 30px;'>
                                Este enlace expirará en 24 horas por motivos de seguridad.
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

            var success = await _emailService.EnviarEmailAsync(email, "Restablecer tu contraseña - AutoGuía", htmlBody);
            
            if (success)
            {
                _logger.LogInformation("Email de recuperación de contraseña enviado a {Email}", email);
            }
            else
            {
                _logger.LogWarning("No se pudo enviar email de recuperación a {Email}", email);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar email de recuperación de contraseña a {Email}", email);
        }
    }

    /// <summary>
    /// Envía código de recuperación de contraseña
    /// </summary>
    public async Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {
        try
        {
            var htmlBody = $@"
<!DOCTYPE html>
<html lang='es'>
<head>
    <meta charset='UTF-8'>
    <title>Código de Verificación</title>
</head>
<body style='font-family: Arial, sans-serif; padding: 20px;'>
    <h2>Tu código de verificación</h2>
    <p>Hola, {user.UserName ?? email.Split('@')[0]}</p>
    <p>Tu código de verificación es:</p>
    <h1 style='background-color: #f0f0f0; padding: 20px; text-align: center; letter-spacing: 5px;'>{resetCode}</h1>
    <p>Este código expirará en 15 minutos.</p>
    <p>Si no solicitaste este código, ignora este mensaje.</p>
    <hr>
    <p style='color: #666; font-size: 12px;'>AutoGuía - Tu aliado en el mundo automotriz</p>
</body>
</html>";

            await _emailService.EnviarEmailAsync(email, "Tu código de verificación - AutoGuía", htmlBody);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar código de verificación a {Email}", email);
        }
    }
}
