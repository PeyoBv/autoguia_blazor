using Bunit;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Rodavia.Web.Components.Shared;

namespace Rodavia.Tests.Components;

/// <summary>
/// Tests para el componente ProductCard.razor
/// Verifica renderizado, accesibilidad y eventos
/// </summary>
public class ProductCardTests : TestContext
{
    [Fact]
    public void ProductCard_Renderiza_Titulo_Y_Precio_Correctos()
    {
        // Arrange
        var expectedTitle = "Aceite Castrol 5W-30";
        var expectedPrice = 25990m;

        // Act
        var cut = RenderComponent<ProductCard>(parameters => parameters
            .Add(p => p.Title, expectedTitle)
            .Add(p => p.Price, expectedPrice)
        );

        // Assert
        var titleElement = cut.Find("h3.product-card-title");
        titleElement.TextContent.Trim().Should().Be(expectedTitle);

        var priceElement = cut.Find("span.product-card-amount");
        priceElement.TextContent.Should().Contain("25.990"); // Formato con separador de miles
    }

    [Fact]
    public void ProductCard_Imagen_Tiene_Loading_Lazy_Y_Alt_Correcto()
    {
        // Arrange
        var expectedTitle = "Filtro de Aceite";
        var expectedImageUrl = "/images/filtro-aceite.jpg";
        var expectedAlt = "Filtro de aceite para motor";

        // Act
        var cut = RenderComponent<ProductCard>(parameters => parameters
            .Add(p => p.Title, expectedTitle)
            .Add(p => p.ImageUrl, expectedImageUrl)
            .Add(p => p.ImageAlt, expectedAlt)
            .Add(p => p.Price, 8990m)
        );

        // Assert
        var imageElement = cut.Find("img.product-card-image");
        
        // Verificar atributo loading="lazy"
        imageElement.GetAttribute("loading").Should().Be("lazy");
        
        // Verificar atributo alt
        imageElement.GetAttribute("alt").Should().Be(expectedAlt);
        
        // Verificar src
        imageElement.GetAttribute("src").Should().Be(expectedImageUrl);
    }

    [Fact]
    public void ProductCard_Click_Boton_Agregar_Invoca_EventCallback_OnAdd()
    {
        // Arrange
        var onAddInvoked = false;
        var expectedTitle = "Bujías NGK";
        var expectedPrice = 12990m;

        // Act
        var cut = RenderComponent<ProductCard>(parameters => parameters
            .Add(p => p.Title, expectedTitle)
            .Add(p => p.Price, expectedPrice)
            .Add(p => p.ButtonText, "Agregar")
            .Add(p => p.OnAdd, EventCallback.Factory.Create(this, () => onAddInvoked = true))
        );

        var button = cut.Find("button.product-card-btn");
        button.Click();

        // Assert
        onAddInvoked.Should().BeTrue();
    }

    [Fact]
    public void ProductCard_Muestra_Descripcion_Cuando_Se_Proporciona()
    {
        // Arrange
        var expectedDescription = "Aceite sintético de alta calidad para motores modernos";

        // Act
        var cut = RenderComponent<ProductCard>(parameters => parameters
            .Add(p => p.Title, "Aceite Mobil 1")
            .Add(p => p.Price, 35990m)
            .Add(p => p.ShortDescription, expectedDescription)
        );

        // Assert
        var descriptionElement = cut.Find("p.product-card-description");
        descriptionElement.TextContent.Trim().Should().Be(expectedDescription);
    }

    [Fact]
    public void ProductCard_No_Muestra_Descripcion_Cuando_Esta_Vacia()
    {
        // Arrange & Act
        var cut = RenderComponent<ProductCard>(parameters => parameters
            .Add(p => p.Title, "Filtro de Aire")
            .Add(p => p.Price, 9990m)
            .Add(p => p.ShortDescription, "")
        );

        // Assert
        var descriptionElements = cut.FindAll("p.product-card-description");
        descriptionElements.Should().BeEmpty();
    }

    [Fact]
    public void ProductCard_Boton_Muestra_Spinner_Durante_Carga()
    {
        // Arrange
        var cut = RenderComponent<ProductCard>(parameters => parameters
            .Add(p => p.Title, "Batería Bosch")
            .Add(p => p.Price, 89990m)
            .Add(p => p.ShowLoadingState, true)
            .Add(p => p.OnAdd, EventCallback.Factory.Create(this, async () => 
            {
                await Task.Delay(100); // Simular operación asíncrona
            }))
        );

        // Act - Click en el botón (inicia carga)
        var button = cut.Find("button.product-card-btn");
        button.Click();

        // Assert - Inmediatamente después del click, debería mostrar spinner
        var spinner = cut.FindAll("span.spinner-border");
        spinner.Should().NotBeEmpty();
    }

    [Fact]
    public void ProductCard_Tiene_Role_Article_Para_Accesibilidad()
    {
        // Arrange & Act
        var cut = RenderComponent<ProductCard>(parameters => parameters
            .Add(p => p.Title, "Neumático Michelin")
            .Add(p => p.Price, 65990m)
        );

        // Assert
        var cardElement = cut.Find("div.product-card");
        cardElement.GetAttribute("role").Should().Be("article");
    }

    [Fact]
    public void ProductCard_Boton_Tiene_AriaLabel_Descriptivo()
    {
        // Arrange
        var expectedTitle = "Amortiguador Monroe";

        // Act
        var cut = RenderComponent<ProductCard>(parameters => parameters
            .Add(p => p.Title, expectedTitle)
            .Add(p => p.Price, 45990m)
            .Add(p => p.ButtonText, "Agregar")
        );

        // Assert
        var button = cut.Find("button.product-card-btn");
        var ariaLabel = button.GetAttribute("aria-label");
        ariaLabel.Should().Contain("Agregar");
        ariaLabel.Should().Contain(expectedTitle);
    }

    [Fact]
    public void ProductCard_Muestra_Fallback_SVG_Cuando_Imagen_Falla()
    {
        // Arrange
        var cut = RenderComponent<ProductCard>(parameters => parameters
            .Add(p => p.Title, "Producto Sin Imagen")
            .Add(p => p.Price, 19990m)
            .Add(p => p.ImageUrl, "/images/non-existent.jpg")
        );

        // Act - Simular error de imagen
        var imageElement = cut.Find("img.product-card-image");
        imageElement.TriggerEvent("onerror", new Microsoft.AspNetCore.Components.Web.ErrorEventArgs());

        // Assert
        var fallbackContainer = cut.Find("div.product-card-image-fallback");
        fallbackContainer.Should().NotBeNull();

        var svgElement = fallbackContainer.QuerySelector("svg");
        svgElement.Should().NotBeNull();
    }

    [Fact]
    public void ProductCard_Formatea_Precio_Con_Separador_De_Miles()
    {
        // Arrange
        var priceWithThousands = 1250000m; // 1.250.000

        // Act
        var cut = RenderComponent<ProductCard>(parameters => parameters
            .Add(p => p.Title, "Auto Usado")
            .Add(p => p.Price, priceWithThousands)
        );

        // Assert
        var priceElement = cut.Find("span.product-card-amount");
        // Formato N0 en español de Chile usa punto como separador de miles
        priceElement.TextContent.Should().Contain("1.250.000");
    }

    [Fact]
    public void ProductCard_Boton_Deshabilitado_Durante_Carga()
    {
        // Arrange
        var cut = RenderComponent<ProductCard>(parameters => parameters
            .Add(p => p.Title, "Llanta de Aleación")
            .Add(p => p.Price, 125990m)
            .Add(p => p.ShowLoadingState, true)
            .Add(p => p.OnAdd, EventCallback.Factory.Create(this, async () => 
            {
                await Task.Delay(50);
            }))
        );

        // Act
        var button = cut.Find("button.product-card-btn");
        button.Click();

        // Assert
        button.HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void ProductCard_Puede_Personalizar_Texto_Boton()
    {
        // Arrange
        var customButtonText = "Ver Detalles";

        // Act
        var cut = RenderComponent<ProductCard>(parameters => parameters
            .Add(p => p.Title, "Kit de Herramientas")
            .Add(p => p.Price, 79990m)
            .Add(p => p.ButtonText, customButtonText)
        );

        // Assert
        var button = cut.Find("button.product-card-btn");
        button.TextContent.Should().Contain(customButtonText);
    }

    [Fact]
    public void ProductCard_Tiene_IDs_Unicos_Para_Accesibilidad()
    {
        // Arrange & Act
        var cut1 = RenderComponent<ProductCard>(parameters => parameters
            .Add(p => p.Title, "Producto 1")
            .Add(p => p.Price, 10000m)
        );

        var cut2 = RenderComponent<ProductCard>(parameters => parameters
            .Add(p => p.Title, "Producto 2")
            .Add(p => p.Price, 20000m)
        );

        // Assert
        var title1 = cut1.Find("h3.product-card-title");
        var title2 = cut2.Find("h3.product-card-title");

        var id1 = title1.GetAttribute("id");
        var id2 = title2.GetAttribute("id");

        id1.Should().NotBe(id2);
        id1.Should().StartWith("product-title-");
        id2.Should().StartWith("product-title-");
    }

    [Fact]
    public void ProductCard_Precio_Tiene_AriaLabel_Descriptivo()
    {
        // Arrange
        var expectedPrice = 45990m;

        // Act
        var cut = RenderComponent<ProductCard>(parameters => parameters
            .Add(p => p.Title, "Radiador")
            .Add(p => p.Price, expectedPrice)
        );

        // Assert
        var priceContainer = cut.Find("div.product-card-price");
        var ariaLabel = priceContainer.GetAttribute("aria-label");
        ariaLabel.Should().Contain("Precio");
        ariaLabel.Should().Contain("45.990"); // Formato con separador
    }
}
