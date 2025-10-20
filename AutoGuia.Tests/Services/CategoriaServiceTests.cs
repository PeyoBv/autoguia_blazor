using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using AutoGuia.Core.DTOs;
using AutoGuia.Core.Entities;
using AutoGuia.Infrastructure.Data;
using AutoGuia.Infrastructure.Services;
using FluentAssertions;

namespace AutoGuia.Tests.Services;

/// <summary>
/// Tests unitarios para CategoriaService
/// </summary>
public class CategoriaServiceTests : IDisposable
{
    private readonly AutoGuiaDbContext _context;
    private readonly Mock<ILogger<CategoriaService>> _loggerMock;
    private readonly CategoriaService _service;

    public CategoriaServiceTests()
    {
        // Arrange - Setup común para todos los tests
        var options = new DbContextOptionsBuilder<AutoGuiaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Base de datos única por test
            .Options;

        _context = new AutoGuiaDbContext(options);
        _loggerMock = new Mock<ILogger<CategoriaService>>();
        _service = new CategoriaService(_context, _loggerMock.Object);

        // Seed inicial de datos
        SeedDatabase();
    }

    /// <summary>
    /// Método helper para poblar la base de datos con datos de prueba
    /// </summary>
    private void SeedDatabase()
    {
        var categoriaActiva = new Categoria
        {
            Id = 1,
            Nombre = "Aceites",
            Descripcion = "Aceites para motor",
            IconUrl = "/icons/aceites.png",
            EsActivo = true,
            FechaCreacion = DateTime.UtcNow
        };

        var categoriaInactiva = new Categoria
        {
            Id = 2,
            Nombre = "Neumáticos",
            Descripcion = "Neumáticos de todo tipo",
            IconUrl = "/icons/neumaticos.png",
            EsActivo = false, // ❌ Inactiva
            FechaCreacion = DateTime.UtcNow
        };

        var categoriaConSubcategorias = new Categoria
        {
            Id = 3,
            Nombre = "Filtros",
            Descripcion = "Filtros automotrices",
            IconUrl = "/icons/filtros.png",
            EsActivo = true,
            FechaCreacion = DateTime.UtcNow
        };

        var subcategoria1 = new Subcategoria
        {
            Id = 1,
            Nombre = "Viscosidad",
            CategoriaId = 1
        };

        var subcategoria2 = new Subcategoria
        {
            Id = 2,
            Nombre = "Marca",
            CategoriaId = 1
        };

        var subcategoria3 = new Subcategoria
        {
            Id = 3,
            Nombre = "Tipo de Filtro",
            CategoriaId = 3
        };

        var valorFiltro1 = new ValorFiltro
        {
            Id = 1,
            Valor = "10W-40",
            SubcategoriaId = 1
        };

        var valorFiltro2 = new ValorFiltro
        {
            Id = 2,
            Valor = "5W-30",
            SubcategoriaId = 1
        };

        var valorFiltro3 = new ValorFiltro
        {
            Id = 3,
            Valor = "Castrol",
            SubcategoriaId = 2
        };

        var valorFiltro4 = new ValorFiltro
        {
            Id = 4,
            Valor = "Aire",
            SubcategoriaId = 3
        };

        var valorFiltro5 = new ValorFiltro
        {
            Id = 5,
            Valor = "Aceite",
            SubcategoriaId = 3
        };

        _context.Categorias.AddRange(categoriaActiva, categoriaInactiva, categoriaConSubcategorias);
        _context.Subcategorias.AddRange(subcategoria1, subcategoria2, subcategoria3);
        _context.ValoresFiltro.AddRange(valorFiltro1, valorFiltro2, valorFiltro3, valorFiltro4, valorFiltro5);
        _context.SaveChanges();
    }

    /// <summary>
    /// Test 1: ObtenerCategoriasAsync debe retornar solo las categorías activas con su jerarquía completa
    /// </summary>
    [Fact]
    public async Task ObtenerCategoriasAsync_DebeRetornarSoloCategoriasActivas()
    {
        // Arrange - Ya preparado en el constructor

        // Act
        var resultado = await _service.ObtenerCategoriasAsync();
        var categorias = resultado.ToList();

        // Assert
        categorias.Should().NotBeNull();
        categorias.Should().HaveCount(2, "solo hay 2 categorías activas (Aceites y Filtros)");
        
        categorias.Should().AllSatisfy(c =>
        {
            c.Id.Should().BeGreaterThan(0);
            c.Nombre.Should().NotBeNullOrWhiteSpace();
        });

        categorias.Should().Contain(c => c.Nombre == "Aceites");
        categorias.Should().Contain(c => c.Nombre == "Filtros");
        categorias.Should().NotContain(c => c.Nombre == "Neumáticos", "porque está inactiva");

        // Verificar que incluye subcategorías
        var categoriaAceites = categorias.First(c => c.Nombre == "Aceites");
        categoriaAceites.Subcategorias.Should().HaveCount(2);
        categoriaAceites.Subcategorias.Should().Contain(s => s.Nombre == "Viscosidad");
        categoriaAceites.Subcategorias.Should().Contain(s => s.Nombre == "Marca");

        // Verificar que incluye valores de filtro
        var subcategoriaViscosidad = categoriaAceites.Subcategorias.First(s => s.Nombre == "Viscosidad");
        subcategoriaViscosidad.Valores.Should().HaveCount(2);
        subcategoriaViscosidad.Valores.Should().Contain(v => v.Valor == "10W-40");
        subcategoriaViscosidad.Valores.Should().Contain(v => v.Valor == "5W-30");

        // Verificar logging
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Obteniendo todas las categorías activas")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    /// <summary>
    /// Test 2: ObtenerSubcategoriasAsync debe retornar subcategorías filtradas por categoría ID
    /// </summary>
    [Fact]
    public async Task ObtenerSubcategoriasAsync_DebeRetornarSubcategoriasPorCategoriaId()
    {
        // Arrange
        int categoriaId = 1; // Aceites

        // Act
        var resultado = await _service.ObtenerSubcategoriasAsync(categoriaId);
        var subcategorias = resultado.ToList();

        // Assert
        subcategorias.Should().NotBeNull();
        subcategorias.Should().HaveCount(2, "la categoría Aceites tiene 2 subcategorías");

        subcategorias.Should().Contain(s => s.Nombre == "Viscosidad");
        subcategorias.Should().Contain(s => s.Nombre == "Marca");

        // Verificar que cada subcategoría tiene valores de filtro
        var viscosidad = subcategorias.First(s => s.Nombre == "Viscosidad");
        viscosidad.Valores.Should().HaveCount(2);
        viscosidad.Valores.Should().Contain(v => v.Valor == "10W-40");

        var marca = subcategorias.First(s => s.Nombre == "Marca");
        marca.Valores.Should().HaveCount(1);
        marca.Valores.Should().Contain(v => v.Valor == "Castrol");

        // Verificar logging
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Obteniendo subcategorías para categoría ID")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    /// <summary>
    /// Test 3: ObtenerValoresFiltroAsync debe retornar valores filtrados por subcategoría ID
    /// </summary>
    [Fact]
    public async Task ObtenerValoresFiltroAsync_DebeRetornarValoresPorSubcategoriaId()
    {
        // Arrange
        int subcategoriaId = 1; // Viscosidad

        // Act
        var resultado = await _service.ObtenerValoresFiltroAsync(subcategoriaId);
        var valores = resultado.ToList();

        // Assert
        valores.Should().NotBeNull();
        valores.Should().HaveCount(2, "la subcategoría Viscosidad tiene 2 valores");

        valores.Should().Contain(v => v.Valor == "10W-40");
        valores.Should().Contain(v => v.Valor == "5W-30");

        // Verificar ordenamiento alfabético
        valores[0].Valor.Should().Be("10W-40");
        valores[1].Valor.Should().Be("5W-30");

        // Verificar logging
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Obteniendo valores de filtro para subcategoría ID")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    /// <summary>
    /// Test 4: ObtenerCategoriaPorIdAsync debe retornar null cuando la categoría no existe
    /// </summary>
    [Fact]
    public async Task ObtenerCategoriaPorIdAsync_CuandoNoExiste_DebeRetornarNull()
    {
        // Arrange
        int categoriaIdInexistente = 999;

        // Act
        var resultado = await _service.ObtenerCategoriaPorIdAsync(categoriaIdInexistente);

        // Assert
        resultado.Should().BeNull("la categoría con ID 999 no existe");

        // Verificar warning log
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("No se encontró categoría activa con ID")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    /// <summary>
    /// Test 5: ObtenerCategoriaPorIdAsync debe retornar categoría con subcategorías cuando existe
    /// </summary>
    [Fact]
    public async Task ObtenerCategoriaPorIdAsync_DebeRetornarCategoriaConSubcategorias()
    {
        // Arrange
        int categoriaId = 1; // Aceites

        // Act
        var resultado = await _service.ObtenerCategoriaPorIdAsync(categoriaId);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(1);
        resultado.Nombre.Should().Be("Aceites");
        resultado.Descripcion.Should().Be("Aceites para motor");
        resultado.IconUrl.Should().Be("/icons/aceites.png");

        // Verificar subcategorías
        resultado.Subcategorias.Should().HaveCount(2);
        resultado.Subcategorias.Should().Contain(s => s.Nombre == "Viscosidad");
        resultado.Subcategorias.Should().Contain(s => s.Nombre == "Marca");

        // Verificar valores de filtro en subcategorías
        var viscosidad = resultado.Subcategorias.First(s => s.Nombre == "Viscosidad");
        viscosidad.Valores.Should().HaveCount(2);
        viscosidad.Valores.Should().Contain(v => v.Valor == "10W-40");

        // Verificar logging exitoso
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("obtenida exitosamente")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    /// <summary>
    /// Test 6: CrearCategoriaAsync debe guardar en BD y retornar el ID generado
    /// </summary>
    [Fact]
    public async Task CrearCategoriaAsync_DebeGuardarEnBDYRetornarId()
    {
        // Arrange
        var nuevaCategoria = new CrearCategoriaDto
        {
            Nombre = "Plumillas",
            Descripcion = "Plumillas limpiaparabrisas",
            IconUrl = "/icons/plumillas.png"
        };

        // Act
        var categoriaId = await _service.CrearCategoriaAsync(nuevaCategoria);

        // Assert
        categoriaId.Should().BeGreaterThan(0, "debe retornar un ID válido");

        // Verificar que se guardó en la base de datos
        var categoriaGuardada = await _context.Categorias.FindAsync(categoriaId);
        categoriaGuardada.Should().NotBeNull();
        categoriaGuardada!.Nombre.Should().Be("Plumillas");
        categoriaGuardada.Descripcion.Should().Be("Plumillas limpiaparabrisas");
        categoriaGuardada.IconUrl.Should().Be("/icons/plumillas.png");
        categoriaGuardada.EsActivo.Should().BeTrue();
        categoriaGuardada.FechaCreacion.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        // Verificar logging
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Categoría creada exitosamente")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    /// <summary>
    /// Test 7: CrearCategoriaAsync debe lanzar excepción cuando ya existe una categoría con el mismo nombre
    /// </summary>
    [Fact]
    public async Task CrearCategoriaAsync_ConDatosInvalidos_DebeLanzarExcepcion()
    {
        // Arrange
        var categoriaDuplicada = new CrearCategoriaDto
        {
            Nombre = "Aceites", // ❌ Ya existe una categoría activa con este nombre
            Descripcion = "Aceites duplicados",
            IconUrl = "/icons/aceites2.png"
        };

        // Act
        Func<Task> act = async () => await _service.CrearCategoriaAsync(categoriaDuplicada);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Ya existe una categoría con el nombre 'Aceites'*");

        // Verificar que se logueó la advertencia
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Ya existe una categoría activa con el nombre")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    /// <summary>
    /// Test adicional: ObtenerSubcategoriasAsync debe retornar lista vacía cuando la categoría no existe o está inactiva
    /// </summary>
    [Fact]
    public async Task ObtenerSubcategoriasAsync_CuandoCategoriaNoExisteOInactiva_DebeRetornarListaVacia()
    {
        // Arrange
        int categoriaIdInexistente = 999;

        // Act
        var resultado = await _service.ObtenerSubcategoriasAsync(categoriaIdInexistente);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty("la categoría no existe");

        // Verificar warning log
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("no existe o no está activa")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    /// <summary>
    /// Test adicional: ObtenerValoresFiltroAsync debe retornar lista vacía cuando la subcategoría no existe
    /// </summary>
    [Fact]
    public async Task ObtenerValoresFiltroAsync_CuandoSubcategoriaNoExiste_DebeRetornarListaVacia()
    {
        // Arrange
        int subcategoriaIdInexistente = 999;

        // Act
        var resultado = await _service.ObtenerValoresFiltroAsync(subcategoriaIdInexistente);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty("la subcategoría no existe");

        // Verificar warning log
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("no existe")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    /// <summary>
    /// Test adicional: ObtenerCategoriaPorIdAsync debe retornar null cuando la categoría está inactiva
    /// </summary>
    [Fact]
    public async Task ObtenerCategoriaPorIdAsync_CuandoCategoriaEstaInactiva_DebeRetornarNull()
    {
        // Arrange
        int categoriaIdInactiva = 2; // Neumáticos (inactiva)

        // Act
        var resultado = await _service.ObtenerCategoriaPorIdAsync(categoriaIdInactiva);

        // Assert
        resultado.Should().BeNull("la categoría está inactiva");

        // Verificar warning log
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("No se encontró categoría activa")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    /// <summary>
    /// Cleanup - Liberar recursos
    /// </summary>
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
