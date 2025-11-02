using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AutoGuia.Web.Controllers;

/// <summary>
/// Controller para manejar callbacks de Transbank (sin antiforgery)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class TransbankCallbackController : ControllerBase
{
    private readonly ILogger<TransbankCallbackController> _logger;

    public TransbankCallbackController(ILogger<TransbankCallbackController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Endpoint público para recibir callback de Transbank
    /// Redirige a la página Blazor con los parámetros
    /// Acepta parámetros desde Query String (GET) o Form Data (POST)
    /// </summary>
    [HttpGet("retorno")]
    [HttpPost("retorno")]
    [HttpGet("/pago/retorno")]
    [HttpPost("/pago/retorno")]
    [IgnoreAntiforgeryToken]
    public IActionResult Retorno()
    {
        // Leer parámetros de Query String o Form Data
        var tokenWs = Request.Query["token_ws"].FirstOrDefault() ?? Request.Form["token_ws"].FirstOrDefault();
        var token = Request.Query["token"].FirstOrDefault() ?? Request.Form["token"].FirstOrDefault();
        var tbkToken = Request.Query["TBK_TOKEN"].FirstOrDefault() ?? Request.Form["TBK_TOKEN"].FirstOrDefault();
        var tbkOrdenCompra = Request.Query["TBK_ORDEN_COMPRA"].FirstOrDefault() ?? Request.Form["TBK_ORDEN_COMPRA"].FirstOrDefault();
        var tbkIdSesion = Request.Query["TBK_ID_SESION"].FirstOrDefault() ?? Request.Form["TBK_ID_SESION"].FirstOrDefault();

        _logger.LogInformation("📥 Callback de Transbank recibido - TokenWs: {TokenWs}, Token: {Token}, TbkToken: {TbkToken}, OrdenCompra: {OrdenCompra}, IdSesion: {IdSesion}",
            tokenWs, token, tbkToken, tbkOrdenCompra, tbkIdSesion);

        // Construir URL de redirección con todos los parámetros
        var queryParams = new List<string>();
        
        if (!string.IsNullOrEmpty(tokenWs))
            queryParams.Add($"token_ws={Uri.EscapeDataString(tokenWs)}");
        
        if (!string.IsNullOrEmpty(token))
            queryParams.Add($"token={Uri.EscapeDataString(token)}");
        
        if (!string.IsNullOrEmpty(tbkToken))
            queryParams.Add($"TBK_TOKEN={Uri.EscapeDataString(tbkToken)}");
        
        if (!string.IsNullOrEmpty(tbkOrdenCompra))
            queryParams.Add($"TBK_ORDEN_COMPRA={Uri.EscapeDataString(tbkOrdenCompra)}");
        
        if (!string.IsNullOrEmpty(tbkIdSesion))
            queryParams.Add($"TBK_ID_SESION={Uri.EscapeDataString(tbkIdSesion)}");

        var redirectUrl = queryParams.Count > 0 
            ? $"/pago/procesando?{string.Join("&", queryParams)}"
            : "/pago/procesando";

        _logger.LogInformation("🔀 Redirigiendo a: {RedirectUrl}", redirectUrl);

        return Redirect(redirectUrl);
    }
}
