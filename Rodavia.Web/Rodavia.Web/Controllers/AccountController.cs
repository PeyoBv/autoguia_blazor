using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Rodavia.Web.Data;
using System.Security.Claims;

namespace Rodavia.Web.Controllers;

[AllowAnonymous]
[Route("[controller]")]
public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ILogger<AccountController> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
    }

    [HttpGet("ExternalLogin")]
    [HttpPost("ExternalLogin")]
    [IgnoreAntiforgeryToken]
    public IActionResult ExternalLogin(string? returnUrl = null, string? handler = null)
    {
        _logger.LogInformation("🚀 [Controller] ExternalLogin ejecutado. ReturnUrl: {Url}, Handler: {Handler}", 
            returnUrl ?? "/", handler ?? "N/A");
        
        var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
        _logger.LogInformation("🔗 [Controller] Redirect URL: {Url}", redirectUrl);
        
        var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
        
        _logger.LogInformation("✅ [Controller] Retornando Challenge para Google");
        return Challenge(properties, "Google");
    }

    [HttpGet("ExternalLoginCallback")]
    public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null)
    {
        _logger.LogInformation("📥 [Controller] Callback recibido. ReturnUrl: {Url}", returnUrl ?? "/");
        
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            _logger.LogError("❌ [Controller] GetExternalLoginInfoAsync retornó null");
            return Redirect("/Account/Login?error=external-login-failed");
        }

        _logger.LogInformation("✅ [Controller] Info obtenida de {Provider}", info.LoginProvider);

        // Intentar login
        var result = await _signInManager.ExternalLoginSignInAsync(
            info.LoginProvider, 
            info.ProviderKey, 
            isPersistent: false);

        if (result.Succeeded)
        {
            _logger.LogInformation("✅ [Controller] Login exitoso para usuario existente");
            return LocalRedirect(returnUrl ?? "/");
        }

        if (result.IsLockedOut)
        {
            _logger.LogWarning("⚠️ [Controller] Usuario bloqueado");
            return Redirect("/Account/Login?error=locked-out");
        }

        // Extraer email
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email))
        {
            _logger.LogError("❌ [Controller] No se pudo obtener email");
            return Redirect("/Account/Login?error=no-email");
        }

        _logger.LogInformation("📧 [Controller] Email: {Email}", email);

        // Buscar usuario existente
        var user = await _userManager.FindByEmailAsync(email);
        
        if (user != null)
        {
            _logger.LogInformation("👤 [Controller] Usuario existe, vinculando login");
            var addLoginResult = await _userManager.AddLoginAsync(user, info);
            
            if (addLoginResult.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                _logger.LogInformation("✅ [Controller] Login vinculado y usuario autenticado");
                return LocalRedirect(returnUrl ?? "/");
            }
            else
            {
                _logger.LogError("❌ [Controller] Error vinculando login: {Errors}", 
                    string.Join(", ", addLoginResult.Errors.Select(e => e.Description)));
                return Redirect("/Account/Login?error=link-failed");
            }
        }

        // Crear nuevo usuario
        _logger.LogInformation("👤 [Controller] Creando nuevo usuario con email: {Email}", email);
        
        var userName = email;
        var fullName = info.Principal.FindFirstValue(ClaimTypes.Name) ?? email;
        
        var newUser = new ApplicationUser
        {
            UserName = userName,
            Email = email,
            EmailConfirmed = true,
            DisplayName = fullName
        };

        var createResult = await _userManager.CreateAsync(newUser);
        if (!createResult.Succeeded)
        {
            _logger.LogError("❌ [Controller] Error creando usuario: {Errors}", 
                string.Join(", ", createResult.Errors.Select(e => e.Description)));
            return Redirect("/Account/Login?error=create-failed");
        }

        _logger.LogInformation("✅ [Controller] Usuario creado exitosamente");

        // Agregar login externo
        var addLoginResultNew = await _userManager.AddLoginAsync(newUser, info);
        if (!addLoginResultNew.Succeeded)
        {
            _logger.LogError("❌ [Controller] Error agregando login externo: {Errors}", 
                string.Join(", ", addLoginResultNew.Errors.Select(e => e.Description)));
            return Redirect("/Account/Login?error=link-failed");
        }

        await _signInManager.SignInAsync(newUser, isPersistent: false);
        
        _logger.LogInformation("🎉 [Controller] Usuario creado y logueado exitosamente");
        return LocalRedirect(returnUrl ?? "/");
    }
}
