using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rodavia.Web.Data;

namespace Rodavia.Web.Pages.Account
{
    /// <summary>
    /// Razor Page que maneja el callback de Google OAuth
    /// Crea nuevos usuarios o inicia sesión de usuarios existentes
    /// </summary>
    public class ExternalLoginCallbackModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly ILogger<ExternalLoginCallbackModel> _logger;

        public ExternalLoginCallbackModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            ILogger<ExternalLoginCallbackModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

        /// <summary>
        /// Procesa el callback de Google OAuth
        /// </summary>
        public async Task<IActionResult> OnGetAsync(string? remoteError = null)
        {
            ReturnUrl = ReturnUrl ?? Url.Content("~/");

            // Manejar errores del proveedor externo
            if (remoteError != null)
            {
                _logger.LogWarning("Error del proveedor externo: {RemoteError}", remoteError);
                return RedirectToPage("./ExternalLogin", new { ReturnUrl, remoteError });
            }

            // Obtener información del login externo
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                _logger.LogWarning("No se pudo obtener información del login externo");
                return RedirectToPage("./ExternalLogin", new 
                { 
                    ReturnUrl, 
                    remoteError = "Error al cargar información de autenticación." 
                });
            }

            try
            {
                // Intentar iniciar sesión con el proveedor externo
                var result = await _signInManager.ExternalLoginSignInAsync(
                    info.LoginProvider,
                    info.ProviderKey,
                    isPersistent: false,
                    bypassTwoFactor: true);

                if (result.Succeeded)
                {
                    // Usuario existente - login exitoso
                    var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                    _logger.LogInformation(
                        "Usuario {Email} inició sesión con {Provider}",
                        email,
                        info.LoginProvider);
                    
                    return LocalRedirect(ReturnUrl);
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("Cuenta bloqueada intentando login externo");
                    return RedirectToPage("./Lockout");
                }

                // Usuario nuevo - crear cuenta
                return await CreateNewUserAsync(info);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando callback de login externo");
                return RedirectToPage("./ExternalLogin", new 
                { 
                    ReturnUrl, 
                    remoteError = "Ocurrió un error inesperado. Por favor, intenta nuevamente." 
                });
            }
        }

        /// <summary>
        /// Crea un nuevo usuario con la información del proveedor externo (Google)
        /// </summary>
        private async Task<IActionResult> CreateNewUserAsync(ExternalLoginInfo info)
        {
            // Obtener email del claim
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("No se pudo obtener email del proveedor externo");
                return RedirectToPage("./ExternalLogin", new 
                { 
                    ReturnUrl, 
                    remoteError = "No se pudo obtener tu correo electrónico de Google." 
                });
            }

            // Crear nuevo usuario
            var user = Activator.CreateInstance<ApplicationUser>();

            await _userStore.SetUserNameAsync(user, email, CancellationToken.None);
            
            var emailStore = GetEmailStore();
            await emailStore.SetEmailAsync(user, email, CancellationToken.None);

            // Marcar email como confirmado (Google ya lo verificó)
            user.EmailConfirmed = true;

            // Obtener nombre completo del claim de Google
            var displayName = info.Principal.FindFirstValue(ClaimTypes.Name);
            if (!string.IsNullOrEmpty(displayName))
            {
                user.DisplayName = displayName;
            }

            // Obtener foto de perfil del claim de Google
            var pictureClaim = info.Principal.FindFirstValue("picture");
            if (!string.IsNullOrEmpty(pictureClaim))
            {
                user.ProfilePictureUrl = pictureClaim;
            }

            // Crear el usuario en la base de datos
            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Error al crear usuario: {Errors}", errors);
                return RedirectToPage("./ExternalLogin", new 
                { 
                    ReturnUrl, 
                    remoteError = $"Error al crear la cuenta: {errors}" 
                });
            }

            // Vincular el login externo al usuario
            result = await _userManager.AddLoginAsync(user, info);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Error al vincular login externo: {Errors}", errors);
                
                // Intentar eliminar el usuario creado
                await _userManager.DeleteAsync(user);
                
                return RedirectToPage("./ExternalLogin", new 
                { 
                    ReturnUrl, 
                    remoteError = $"Error al vincular la cuenta: {errors}" 
                });
            }

            _logger.LogInformation(
                "Usuario creado exitosamente con {Provider}: {Email}",
                info.LoginProvider,
                email);

            // Iniciar sesión automáticamente
            await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);

            _logger.LogInformation(
                "Usuario {Email} autenticado automáticamente después del registro",
                email);

            return LocalRedirect(ReturnUrl);
        }

        /// <summary>
        /// Obtiene el email store para el UserManager
        /// </summary>
        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("Se requiere un user store con soporte de email.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
