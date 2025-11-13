using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rodavia.Web.Data;

namespace Rodavia.Web.Pages.Account
{
    /// <summary>
    /// Razor Page para iniciar el flujo de autenticación externa con Google
    /// </summary>
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<ExternalLoginModel> _logger;

        public ExternalLoginModel(
            SignInManager<ApplicationUser> signInManager,
            ILogger<ExternalLoginModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Maneja el GET - Muestra la página o maneja errores del proveedor externo
        /// </summary>
        public void OnGet(string? remoteError = null)
        {
            ReturnUrl = ReturnUrl ?? Url.Content("~/");

            if (!string.IsNullOrEmpty(remoteError))
            {
                ErrorMessage = $"Error del proveedor externo: {remoteError}";
                _logger.LogWarning("Error del proveedor externo: {RemoteError}", remoteError);
            }
        }

        /// <summary>
        /// PageHandler que inicia el flujo OAuth con Google
        /// </summary>
        public IActionResult OnPostGoogle()
        {
            // Configurar la URL de redirección después de la autenticación
            var redirectUrl = Url.Page("./ExternalLoginCallback", 
                pageHandler: null,
                values: new { ReturnUrl },
                protocol: Request.Scheme);

            _logger.LogInformation(
                "Iniciando autenticación con Google. Redirect URL: {RedirectUrl}",
                redirectUrl);

            // Configurar las propiedades de autenticación
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(
                "Google",
                redirectUrl);

            // Redirigir a Google para autenticación
            return new ChallengeResult("Google", properties);
        }
    }
}
