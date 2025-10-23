using Xunit;
using Moq;
using AutoGuia.Core.DTOs;
using AutoGuia.Infrastructure.Services;
using AutoGuia.Infrastructure.Repositories;

namespace AutoGuia.Tests.Services;

public class SintomaSearchServiceTests
{
    private readonly Mock<ISintomaRepository> _repositoryMock;
    private readonly SintomaSearchService _searchService;

    public SintomaSearchServiceTests()
    {
        _repositoryMock = new Mock<ISintomaRepository>();
        _searchService = new SintomaSearchService(_repositoryMock.Object);
    }

    /// <summary>
    /// Test 1: Búsqueda con variación de palabras (arranca vs enciende)
    /// </summary>
    [Fact]
    public async Task BuscarSintomasAvanzadoAsync_ConVariacionDePalabras_DebeEncontrarCoincidencia()
    {
        // Arrange
        var sintomas = new List<SintomaDto>
        {
            new SintomaDto
            {
                Id = 1,
                Descripcion = "El motor no enciende",
                DescripcionTecnica = "Falla de ignición o combustible",
                NivelUrgencia = 4,
                SistemaAutomotrizId = 1,
                NombreSistema = "Sistema de Motor"
            }
        };

        _repositoryMock
            .Setup(x => x.ObtenerTodosSintomasActivosAsync())
            .ReturnsAsync(sintomas);

        // Act
        var resultado = await _searchService.BuscarSintomasAvanzadoAsync("El motor no arranca");

        // Assert
        Assert.NotEmpty(resultado);
        Assert.Single(resultado);
        Assert.Equal(1, resultado[0].Id);
        Assert.Equal("El motor no enciende", resultado[0].Descripcion);
    }

    /// <summary>
    /// Test 2: Búsqueda con tildes y minúsculas
    /// </summary>
    [Fact]
    public async Task BuscarSintomasAvanzadoAsync_ConTildesYMinusculas_DebeEncontrarCoincidencia()
    {
        // Arrange
        var sintomas = new List<SintomaDto>
        {
            new SintomaDto
            {
                Id = 2,
                Descripcion = "Auto se sobrecalienta",
                DescripcionTecnica = "Temperatura del motor elevada",
                NivelUrgencia = 3,
                SistemaAutomotrizId = 1,
                NombreSistema = "Sistema de Motor"
            }
        };

        _repositoryMock
            .Setup(x => x.ObtenerTodosSintomasActivosAsync())
            .ReturnsAsync(sintomas);

        // Act
        var resultado = await _searchService.BuscarSintomasAvanzadoAsync("mi auto se sobrecalienta");

        // Assert
        Assert.NotEmpty(resultado);
        Assert.Contains(resultado, s => s.Id == 2);
    }

    /// <summary>
    /// Test 3: Búsqueda con palabras clave dispersas
    /// </summary>
    [Fact]
    public async Task BuscarSintomasAvanzadoAsync_ConPalabrasDispersas_DebeEncontrarCoincidencia()
    {
        // Arrange
        var sintomas = new List<SintomaDto>
        {
            new SintomaDto
            {
                Id = 3,
                Descripcion = "Pedal de freno suave o hundido",
                DescripcionTecnica = "Posible fuga de fluido o aire en el sistema",
                NivelUrgencia = 4,
                SistemaAutomotrizId = 3,
                NombreSistema = "Sistema de Frenos"
            }
        };

        _repositoryMock
            .Setup(x => x.ObtenerTodosSintomasActivosAsync())
            .ReturnsAsync(sintomas);

        // Act
        var resultado = await _searchService.BuscarSintomasAvanzadoAsync("freno suave pedal");

        // Assert
        Assert.NotEmpty(resultado);
        Assert.Contains(resultado, s => s.Id == 3);
    }

    /// <summary>
    /// Test 4: Búsqueda sin coincidencias debe retornar lista vacía
    /// </summary>
    [Fact]
    public async Task BuscarSintomasAvanzadoAsync_SinCoincidencias_DebeRetornarVacio()
    {
        // Arrange
        var sintomas = new List<SintomaDto>
        {
            new SintomaDto
            {
                Id = 1,
                Descripcion = "El motor no enciende",
                DescripcionTecnica = "Falla de ignición",
                NivelUrgencia = 4,
                SistemaAutomotrizId = 1,
                NombreSistema = "Sistema de Motor"
            }
        };

        _repositoryMock
            .Setup(x => x.ObtenerTodosSintomasActivosAsync())
            .ReturnsAsync(sintomas);

        // Act - Palabras completamente aleatorias sin relación semántica
        var resultado = await _searchService.BuscarSintomasAvanzadoAsync("zxcvbn qwerty asdfgh");

        // Assert
        Assert.Empty(resultado);
    }

    /// <summary>
    /// Test 5: Búsqueda ordena por relevancia (mejores coincidencias primero)
    /// </summary>
    [Fact]
    public async Task BuscarSintomasAvanzadoAsync_DebeOrdenarPorRelevancia()
    {
        // Arrange
        var sintomas = new List<SintomaDto>
        {
            new SintomaDto
            {
                Id = 1,
                Descripcion = "Motor ruidoso",
                DescripcionTecnica = "Ruidos de motor",
                NivelUrgencia = 2,
                SistemaAutomotrizId = 1,
                NombreSistema = "Sistema de Motor"
            },
            new SintomaDto
            {
                Id = 2,
                Descripcion = "Ruidos extraños en el motor",
                DescripcionTecnica = "Golpeteo o chirridos en motor",
                NivelUrgencia = 2,
                SistemaAutomotrizId = 1,
                NombreSistema = "Sistema de Motor"
            }
        };

        _repositoryMock
            .Setup(x => x.ObtenerTodosSintomasActivosAsync())
            .ReturnsAsync(sintomas);

        // Act
        var resultado = await _searchService.BuscarSintomasAvanzadoAsync("ruidos extraños motor");

        // Assert
        Assert.NotEmpty(resultado);
        // El segundo síntoma tiene mayor coincidencia (3 palabras: ruidos, extraños, motor)
        Assert.Equal(2, resultado[0].Id);
    }

    /// <summary>
    /// Test 6: Descripción vacía debe retornar lista vacía
    /// </summary>
    [Fact]
    public async Task BuscarSintomasAvanzadoAsync_ConDescripcionVacia_DebeRetornarVacio()
    {
        // Arrange
        _repositoryMock
            .Setup(x => x.ObtenerTodosSintomasActivosAsync())
            .ReturnsAsync(new List<SintomaDto>());

        // Act
        var resultado = await _searchService.BuscarSintomasAvanzadoAsync("");

        // Assert
        Assert.Empty(resultado);
    }

    /// <summary>
    /// Test 7: Búsqueda case-insensitive
    /// </summary>
    [Fact]
    public async Task BuscarSintomasAvanzadoAsync_DebeSerCaseInsensitive()
    {
        // Arrange
        var sintomas = new List<SintomaDto>
        {
            new SintomaDto
            {
                Id = 1,
                Descripcion = "Batería descargada",
                DescripcionTecnica = "Batería sin carga",
                NivelUrgencia = 3,
                SistemaAutomotrizId = 5,
                NombreSistema = "Sistema Eléctrico"
            }
        };

        _repositoryMock
            .Setup(x => x.ObtenerTodosSintomasActivosAsync())
            .ReturnsAsync(sintomas);

        // Act
        var resultado1 = await _searchService.BuscarSintomasAvanzadoAsync("BATERIA DESCARGADA");
        var resultado2 = await _searchService.BuscarSintomasAvanzadoAsync("bateria descargada");
        var resultado3 = await _searchService.BuscarSintomasAvanzadoAsync("BaTErÍa DeScArGaDa");

        // Assert
        Assert.NotEmpty(resultado1);
        Assert.NotEmpty(resultado2);
        Assert.NotEmpty(resultado3);
        Assert.Equal(1, resultado1[0].Id);
    }

    /// <summary>
    /// Test 8: Múltiples síntomas, cada uno con diferentes niveles de coincidencia
    /// </summary>
    [Fact]
    public async Task BuscarSintomasAvanzadoAsync_ConMultiplesSintomas_OrdenaPorMejorCoincidencia()
    {
        // Arrange
        var sintomas = new List<SintomaDto>
        {
            new SintomaDto
            {
                Id = 1,
                Descripcion = "Transmisión",
                DescripcionTecnica = "Problemas de transmisión",
                NivelUrgencia = 2,
                SistemaAutomotrizId = 2,
                NombreSistema = "Sistema de Transmisión"
            },
            new SintomaDto
            {
                Id = 2,
                Descripcion = "Cambios de marcha bruscos",
                DescripcionTecnica = "Falla en embrague o fluido de transmisión",
                NivelUrgencia = 2,
                SistemaAutomotrizId = 2,
                NombreSistema = "Sistema de Transmisión"
            },
            new SintomaDto
            {
                Id = 3,
                Descripcion = "Auto no acelera",
                DescripcionTecnica = "Problemas de motor",
                NivelUrgencia = 3,
                SistemaAutomotrizId = 1,
                NombreSistema = "Sistema de Motor"
            }
        };

        _repositoryMock
            .Setup(x => x.ObtenerTodosSintomasActivosAsync())
            .ReturnsAsync(sintomas);

        // Act
        var resultado = await _searchService.BuscarSintomasAvanzadoAsync("cambios marcha bruscos");

        // Assert
        Assert.NotEmpty(resultado);
        // El primer resultado debe ser el síntoma 2 (mejor coincidencia)
        Assert.Equal(2, resultado[0].Id);
        Assert.Equal("Cambios de marcha bruscos", resultado[0].Descripcion);
    }
}
