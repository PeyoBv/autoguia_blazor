namespace AutoGuia.Web.Services;

/// <summary>
/// Interfaz para el servicio de envío de emails
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Envía un email de bienvenida al usuario registrado
    /// </summary>
    /// <param name="destinatario">Email del destinatario</param>
    /// <param name="nombreUsuario">Nombre del usuario para personalizar el mensaje</param>
    /// <param name="confirmationUrl">URL de confirmación de cuenta (opcional)</param>
    /// <returns>True si el email se envió correctamente, False en caso contrario</returns>
    Task<bool> EnviarEmailBienvenidaAsync(string destinatario, string nombreUsuario, string? confirmationUrl = null);

    /// <summary>
    /// Envía un email genérico con HTML personalizado
    /// </summary>
    /// <param name="destinatario">Email del destinatario</param>
    /// <param name="asunto">Asunto del email</param>
    /// <param name="htmlBody">Contenido HTML del email</param>
    /// <returns>True si el email se envió correctamente, False en caso contrario</returns>
    Task<bool> EnviarEmailAsync(string destinatario, string asunto, string htmlBody);
}
