using Bunit;
using Xunit;
using FluentAssertions;
using AutoGuia.Web.Components.Layout;
using Bunit.TestDoubles;

namespace AutoGuia.Tests.Components;

public class NavMenuTests : Bunit.TestContext
{
    public NavMenuTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        
        // Mock de autorización para AuthorizeView
        this.AddTestAuthorization();
        
        // Stub para NavLink
        ComponentFactories.AddStub<Microsoft.AspNetCore.Components.Routing.NavLink>();
    }

    [Fact]
    public void NavMenu_Renderiza_Enlaces_Principales()
    {
        var cut = RenderComponent<NavMenu>();
        
        // Verificar que el componente se renderiza sin errores
        cut.Should().NotBeNull();
        
        // Verificar que existen múltiples nav-items
        var navItems = cut.FindAll(".nav-item");
        navItems.Count.Should().BeGreaterThan(0, "debe haber al menos un elemento de navegación");
    }

    [Fact]
    public void NavMenu_Muestra_Logo()
    {
        var cut = RenderComponent<NavMenu>();
        var navbarBrand = cut.Find("a.navbar-brand");
        navbarBrand.TextContent.Should().Contain("AutoGuía");
    }

    [Fact]
    public void NavMenu_Tiene_Toggler()
    {
        var cut = RenderComponent<NavMenu>();
        var toggler = cut.Find("input.navbar-toggler");
        toggler.Should().NotBeNull();
    }
}
