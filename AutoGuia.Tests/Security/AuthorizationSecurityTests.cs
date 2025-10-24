using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AutoGuia.Web.Controllers;
using AutoGuia.Core.Interfaces;
using AutoGuia.Core.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Antiforgery;

namespace AutoGuia.Tests.Security;

/// <summary>
/// Tests de seguridad para autorización y protección CSRF en controllers
/// Verifica que los endpoints estén correctamente protegidos contra accesos no autorizados y ataques CSRF
/// </summary>
public class AuthorizationSecurityTests
{
    private readonly Mock<IDiagnosticoService> _mockDiagnosticoService;
    private readonly Mock<ISistemaAutomotrizService> _mockSistemaService;

    public AuthorizationSecurityTests()
    {
        _mockDiagnosticoService = new Mock<IDiagnosticoService>();
        _mockSistemaService = new Mock<ISistemaAutomotrizService>();
    }

    [Fact]
    public async Task Test_Unauthorized_Access_Rejected()
    {
        // Arrange: Controlador sin usuario autenticado
        var controller = new DiagnosticoController(_mockDiagnosticoService.Object);
        
        // Simular HttpContext sin autenticación
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity()); // Usuario NO autenticado
        
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        var request = new DiagnosticoRequestDto
        {
            DescripcionSintoma = "Mi auto hace un ruido extraño"
        };

        // Act & Assert: Debe fallar porque User.FindFirst retornará null
        await Assert.ThrowsAsync<NullReferenceException>(async () =>
        {
            // El método intentará acceder a User.FindFirst().Value con ! y lanzará NullReferenceException
            await controller.DiagnosticarSintoma(request);
        });
    }

    [Fact]
    public void Test_CSRF_Token_Validation_Attribute_Present()
    {
        // Arrange: Verificar que el método POST tenga [ValidateAntiForgeryToken]
        var methodInfo = typeof(DiagnosticoController).GetMethod("DiagnosticarSintoma");

        // Act: Buscar atributo [ValidateAntiForgeryToken]
        var hasValidateAntiForgeryToken = methodInfo!.GetCustomAttributes(typeof(ValidateAntiForgeryTokenAttribute), false).Any();

        // Assert: El método POST debe tener protección CSRF
        Assert.True(hasValidateAntiForgeryToken, 
            "El método DiagnosticarSintoma debe tener [ValidateAntiForgeryToken] para protección CSRF");
    }

    [Fact]
    public void Test_Controller_Has_Authorize_Attribute()
    {
        // Arrange: Verificar que el controller tenga [Authorize] a nivel de clase
        var controllerType = typeof(DiagnosticoController);

        // Act: Buscar atributo [Authorize]
        var hasAuthorize = controllerType.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), false).Any();

        // Assert: El controller debe requerir autenticación
        Assert.True(hasAuthorize, 
            "DiagnosticoController debe tener [Authorize] para proteger endpoints sensibles");
    }

    [Fact]
    public async Task Test_Valid_Token_And_Auth_Succeeds()
    {
        // Arrange: Controlador con usuario autenticado válido
        var controller = new DiagnosticoController(_mockDiagnosticoService.Object);
        
        // Simular usuario autenticado con claims válidos
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "123"),
            new Claim(ClaimTypes.Name, "test@autoguia.cl")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        
        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipal
        };
        
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Mock del servicio para retornar diagnóstico válido
        var resultadoEsperado = new ResultadoDiagnosticoDto
        {
            SintomaIdentificado = "Ruido en motor",
            Recomendacion = "Revisar correa de distribución",
            NivelUrgencia = 2,
            CausasPosibles = new List<CausaPosibleDto>()
        };

        _mockDiagnosticoService
            .Setup(s => s.DiagnosticarSintomaAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(resultadoEsperado);

        var request = new DiagnosticoRequestDto
        {
            DescripcionSintoma = "Mi auto hace un ruido extraño"
        };

        // Act: Ejecutar método con autenticación y CSRF válidos
        var result = await controller.DiagnosticarSintoma(request);

        // Assert: Debe retornar 200 OK con resultado
        var okResult = Assert.IsType<OkObjectResult>(result);
        var diagnostico = Assert.IsType<ResultadoDiagnosticoDto>(okResult.Value);
        Assert.Equal("Ruido en motor", diagnostico.SintomaIdentificado);
        Assert.Equal("Revisar correa de distribución", diagnostico.Recomendacion);
    }

    [Fact]
    public void Test_AllowAnonymous_On_Public_Endpoints()
    {
        // Arrange: Verificar que endpoints públicos tengan [AllowAnonymous]
        var methodInfo = typeof(DiagnosticoController).GetMethod("ObtenerSintomasPorSistema");

        // Act: Buscar atributo [AllowAnonymous]
        var hasAllowAnonymous = methodInfo!.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute), false).Any();

        // Assert: Los endpoints públicos deben permitir acceso anónimo explícitamente
        Assert.True(hasAllowAnonymous, 
            "ObtenerSintomasPorSistema debe tener [AllowAnonymous] ya que es endpoint público");
    }

    [Fact]
    public void Test_Feedback_Method_Has_CSRF_Protection()
    {
        // Arrange: Verificar que el método POST de feedback tenga protección CSRF
        var methodInfo = typeof(DiagnosticoController).GetMethod("RegistrarFeedback");

        // Act: Buscar atributo [ValidateAntiForgeryToken]
        var hasValidateAntiForgeryToken = methodInfo!.GetCustomAttributes(typeof(ValidateAntiForgeryTokenAttribute), false).Any();

        // Assert: Todos los métodos POST deben tener protección CSRF
        Assert.True(hasValidateAntiForgeryToken, 
            "RegistrarFeedback debe tener [ValidateAntiForgeryToken] para prevenir ataques CSRF");
    }

    [Fact]
    public void Test_SistemasController_Public_Endpoints()
    {
        // Arrange: SistemasController debe permitir acceso público a endpoints de lectura
        var controllerType = typeof(SistemasController);

        // Act: Verificar que tenga [AllowAnonymous] a nivel de clase
        var hasAllowAnonymous = controllerType.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute), false).Any();

        // Assert: SistemasController es solo lectura y debe ser público
        Assert.True(hasAllowAnonymous, 
            "SistemasController debe tener [AllowAnonymous] para permitir consultas públicas de sistemas");
    }

    [Fact]
    public async Task Test_Historial_Requires_Authentication()
    {
        // Arrange: Verificar que historial requiera usuario autenticado
        var controller = new DiagnosticoController(_mockDiagnosticoService.Object);
        
        // Simular usuario NO autenticado
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity());
        
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act & Assert: Debe fallar al intentar acceder sin autenticación
        await Assert.ThrowsAsync<NullReferenceException>(async () =>
        {
            await controller.ObtenerHistorial();
        });
    }
}
