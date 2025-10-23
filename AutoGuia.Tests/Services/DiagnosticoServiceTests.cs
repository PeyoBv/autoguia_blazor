using Xunit;
using Moq;
using AutoGuia.Core.DTOs;
using AutoGuia.Core.Entities;
using AutoGuia.Core.Interfaces;
using AutoGuia.Infrastructure.Services;
using AutoGuia.Infrastructure.Repositories;

namespace AutoGuia.Tests.Services;

/// <summary>
/// Tests unitarios para DiagnosticoService
/// Valida la lógica de diagnóstico automotriz, búsqueda de síntomas y registro de consultas
/// </summary>
public class DiagnosticoServiceTests
{
    private readonly Mock<ISintomaRepository> _sintomaRepositoryMock;
    private readonly Mock<ICausaPosibleRepository> _causaRepositoryMock;
    private readonly Mock<IConsultaDiagnosticoRepository> _consultaRepositoryMock;
    private readonly Mock<SintomaSearchService> _searchServiceMock;
    private readonly DiagnosticoService _diagnosticoService;

    public DiagnosticoServiceTests()
    {
        _sintomaRepositoryMock = new Mock<ISintomaRepository>();
        _causaRepositoryMock = new Mock<ICausaPosibleRepository>();
        _consultaRepositoryMock = new Mock<IConsultaDiagnosticoRepository>();
        _searchServiceMock = new Mock<SintomaSearchService>(_sintomaRepositoryMock.Object);
        
        _diagnosticoService = new DiagnosticoService(
            _sintomaRepositoryMock.Object,
            _causaRepositoryMock.Object,
            _consultaRepositoryMock.Object,
            _searchServiceMock.Object);
    }

    [Fact]
    public async Task DiagnosticarSintomaAsync_ConSintomaEncontrado_DebeRetornarResultadoConCausas()
    {
        // Arrange
        var sintomaDto = new SintomaDto
        {
            Id = 1,
            Descripcion = "El motor no enciende",
            DescripcionTecnica = "Falla en el arranque del motor de combustión",
            NivelUrgencia = 4,
            SistemaAutomotrizId = 1
        };

        var causasDto = new List<CausaPosibleDto>
        {
            new CausaPosibleDto
            {
                Id = 1,
                Descripcion = "Batería descargada",
                DescripcionDetallada = "La batería no tiene suficiente carga para arrancar el motor",
                NivelProbabilidad = 4,
                RequiereServicioProfesional = false,
                PasosVerificacion = new List<PasoVerificacionDto>(),
                Recomendaciones = new List<RecomendacionPreventivaDto>()
            }
        };

        _searchServiceMock
            .Setup(x => x.BuscarSintomasAvanzadoAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<SintomaDto> { sintomaDto });

        _causaRepositoryMock
            .Setup(x => x.ObtenerCausasPorSintomaAsync(1))
            .ReturnsAsync(causasDto);

        _consultaRepositoryMock
            .Setup(x => x.CrearConsultaAsync(It.IsAny<ConsultaDiagnostico>()))
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _diagnosticoService.DiagnosticarSintomaAsync("motor no enciende", 1);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("El motor no enciende", resultado.SintomaIdentificado);
        Assert.Equal(4, resultado.NivelUrgencia);
        Assert.Single(resultado.CausasPosibles);
        Assert.False(resultado.SugerirServicioProfesional);
        Assert.Equal(1, resultado.SintomaRelacionadoId);
    }

    [Fact]
    public async Task DiagnosticarSintomaAsync_ConCausaQueRequiereServicio_DebeSugerirServicioProfesional()
    {
        // Arrange
        var sintomaDto = new SintomaDto
        {
            Id = 2,
            Descripcion = "Ruido metálico en el motor",
            DescripcionTecnica = "Sonido anormal metálico proveniente del bloque del motor",
            NivelUrgencia = 3,
            SistemaAutomotrizId = 1
        };

        var causasDto = new List<CausaPosibleDto>
        {
            new CausaPosibleDto
            {
                Id = 2,
                Descripcion = "Biela dañada",
                DescripcionDetallada = "Posible rotura o desgaste severo de biela",
                NivelProbabilidad = 3,
                RequiereServicioProfesional = true,
                PasosVerificacion = new List<PasoVerificacionDto>(),
                Recomendaciones = new List<RecomendacionPreventivaDto>()
            }
        };

        _searchServiceMock
            .Setup(x => x.BuscarSintomasAvanzadoAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<SintomaDto> { sintomaDto });

        _causaRepositoryMock
            .Setup(x => x.ObtenerCausasPorSintomaAsync(2))
            .ReturnsAsync(causasDto);

        _consultaRepositoryMock
            .Setup(x => x.CrearConsultaAsync(It.IsAny<ConsultaDiagnostico>()))
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _diagnosticoService.DiagnosticarSintomaAsync("ruido metálico motor", 1);

        // Assert
        Assert.NotNull(resultado);
        Assert.True(resultado.SugerirServicioProfesional);
        Assert.Contains("DEBE llevar a servicio profesional", resultado.Recomendacion);
    }

    [Fact]
    public async Task DiagnosticarSintomaAsync_SinSintomaEncontrado_DebeRetornarMensajeDeError()
    {
        // Arrange
        _searchServiceMock
            .Setup(x => x.BuscarSintomasAvanzadoAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<SintomaDto>());

        _consultaRepositoryMock
            .Setup(x => x.CrearConsultaAsync(It.IsAny<ConsultaDiagnostico>()))
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _diagnosticoService.DiagnosticarSintomaAsync("síntoma inexistente", 1);

        // Assert
        Assert.NotNull(resultado);
        Assert.Null(resultado.SintomaRelacionadoId);
        Assert.Empty(resultado.CausasPosibles);
        Assert.Contains("No se encontraron síntomas", resultado.Recomendacion);
    }

    [Fact]
    public async Task DiagnosticarSintomaAsync_ConDescripcionVacia_DebeRetornarMensajeDeError()
    {
        // Arrange
        _searchServiceMock
            .Setup(x => x.BuscarSintomasAvanzadoAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<SintomaDto>());

        // Act
        var resultado = await _diagnosticoService.DiagnosticarSintomaAsync("", 1);

        // Assert
        Assert.NotNull(resultado);
        Assert.Contains("No se encontraron síntomas", resultado.Recomendacion);
    }

    [Fact]
    public async Task DiagnosticarSintomaAsync_ConUsuarioIdInvalido_DebeRegistrarConsultaCorrectamente()
    {
        // Arrange
        _searchServiceMock
            .Setup(x => x.BuscarSintomasAvanzadoAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<SintomaDto>());

        // Act
        var resultado = await _diagnosticoService.DiagnosticarSintomaAsync("motor no enciende", 0);

        // Assert
        _consultaRepositoryMock.Verify(x => x.CrearConsultaAsync(It.Is<ConsultaDiagnostico>(
            c => c.UsuarioId == 0)), Times.Once);
    }

    [Fact]
    public async Task ObtenerSintomasPorSistemaAsync_DebeRetornarListaDeSintomas()
    {
        // Arrange
        var sintomas = new List<SintomaDto>
        {
            new SintomaDto
            {
                Id = 1,
                Descripcion = "Motor no arranca",
                DescripcionTecnica = "Falla en arranque",
                NivelUrgencia = 4,
                SistemaAutomotrizId = 1
            },
            new SintomaDto
            {
                Id = 2,
                Descripcion = "Motor hace ruido",
                DescripcionTecnica = "Ruido anormal",
                NivelUrgencia = 2,
                SistemaAutomotrizId = 1
            }
        };

        _sintomaRepositoryMock
            .Setup(x => x.ObtenerSintomasPorSistemaAsync(1))
            .ReturnsAsync(sintomas);

        // Act
        var resultado = await _diagnosticoService.ObtenerSintomasPorSistemaAsync(1);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count);
        Assert.All(resultado, s => Assert.Equal(1, s.SistemaAutomotrizId));
    }

    [Fact]
    public async Task ObtenerCausaConDetallesAsync_ConIdValido_DebeRetornarCausa()
    {
        // Arrange
        var causaDto = new CausaPosibleDto
        {
            Id = 1,
            Descripcion = "Batería descargada",
            DescripcionDetallada = "La batería no tiene carga suficiente",
            NivelProbabilidad = 4,
            RequiereServicioProfesional = false,
            PasosVerificacion = new List<PasoVerificacionDto>
            {
                new PasoVerificacionDto
                {
                    Id = 1,
                    Descripcion = "Verificar voltaje de batería",
                    InstruccionesDetalladas = "Usar multímetro para medir voltaje",
                    IndicadoresExito = "Voltaje debe ser 12.6V o superior",
                    Orden = 1
                }
            },
            Recomendaciones = new List<RecomendacionPreventivaDto>()
        };

        _causaRepositoryMock
            .Setup(x => x.ObtenerCausaPorIdAsync(1))
            .ReturnsAsync(causaDto);

        // Act
        var resultado = await _diagnosticoService.ObtenerCausaConDetallesAsync(1);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Batería descargada", resultado.Descripcion);
        Assert.Single(resultado.PasosVerificacion);
    }

    [Fact]
    public async Task ObtenerCausaConDetallesAsync_ConIdInexistente_DebeRetornarNull()
    {
        // Arrange
        _causaRepositoryMock
            .Setup(x => x.ObtenerCausaPorIdAsync(999))
            .ReturnsAsync((CausaPosibleDto?)null);

        // Act
        var resultado = await _diagnosticoService.ObtenerCausaConDetallesAsync(999);

        // Assert
        Assert.Null(resultado);
    }

    [Fact]
    public async Task ObtenerHistorialAsync_DebeRetornarListaDeConsultas()
    {
        // Arrange
        var consultas = new List<ConsultaDiagnosticoDto>
        {
            new ConsultaDiagnosticoDto
            {
                Id = 1,
                SintomaDescrito = "Motor ruidoso",
                RespuestaAsistente = "Posible problema en rodamientos",
                FechaConsulta = DateTime.UtcNow.AddDays(-1),
                FueUtil = true,
                SintomaRelacionadoId = 1
            },
            new ConsultaDiagnosticoDto
            {
                Id = 2,
                SintomaDescrito = "Frenos chirrian",
                RespuestaAsistente = "Pastillas desgastadas",
                FechaConsulta = DateTime.UtcNow,
                FueUtil = false,
                SintomaRelacionadoId = 2
            }
        };

        _consultaRepositoryMock
            .Setup(x => x.ObtenerConsultasPorUsuarioAsync(1))
            .ReturnsAsync(consultas);

        // Act
        var resultado = await _diagnosticoService.ObtenerHistorialAsync(1);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count);
        Assert.Equal("Motor ruidoso", resultado[0].SintomaDescrito);
        Assert.True(resultado[0].FueUtil);
        Assert.False(resultado[1].FueUtil);
    }

    [Fact]
    public async Task ObtenerHistorialAsync_ConUsuarioSinConsultas_DebeRetornarListaVacia()
    {
        // Arrange
        _consultaRepositoryMock
            .Setup(x => x.ObtenerConsultasPorUsuarioAsync(2))
            .ReturnsAsync(new List<ConsultaDiagnosticoDto>());

        // Act
        var resultado = await _diagnosticoService.ObtenerHistorialAsync(2);

        // Assert
        Assert.NotNull(resultado);
        Assert.Empty(resultado);
    }

    [Fact]
    public async Task RegistrarFeedbackAsync_ConParametrosValidos_DebeActualizarFeedback()
    {
        // Arrange
        _consultaRepositoryMock
            .Setup(x => x.ActualizarFeedbackAsync(1, true))
            .Returns(Task.CompletedTask);

        // Act
        await _diagnosticoService.RegistrarFeedbackAsync(1, true);

        // Assert
        _consultaRepositoryMock.Verify(x => x.ActualizarFeedbackAsync(1, true), Times.Once);
    }

    [Fact]
    public async Task RegistrarFeedbackAsync_ConFeedbackNegativo_DebeActualizarCorrectamente()
    {
        // Arrange
        _consultaRepositoryMock
            .Setup(x => x.ActualizarFeedbackAsync(2, false))
            .Returns(Task.CompletedTask);

        // Act
        await _diagnosticoService.RegistrarFeedbackAsync(2, false);

        // Assert
        _consultaRepositoryMock.Verify(x => x.ActualizarFeedbackAsync(2, false), Times.Once);
    }

    [Theory]
    [InlineData(1, "leve")]
    [InlineData(2, "moderado")]
    [InlineData(3, "importante")]
    [InlineData(4, "CRÍTICO")]
    public async Task DiagnosticarSintomaAsync_DebeGenerarRecomendacionSegunNivelUrgencia(int nivelUrgencia, string palabraClave)
    {
        // Arrange
        var sintomaDto = new SintomaDto
        {
            Id = 1,
            Descripcion = "Test síntoma",
            DescripcionTecnica = "Test técnico",
            NivelUrgencia = nivelUrgencia,
            SistemaAutomotrizId = 1
        };

        var causasDto = new List<CausaPosibleDto>
        {
            new CausaPosibleDto
            {
                Id = 1,
                Descripcion = "Test causa",
                DescripcionDetallada = "Test detalle",
                NivelProbabilidad = 3,
                RequiereServicioProfesional = false,
                PasosVerificacion = new List<PasoVerificacionDto>(),
                Recomendaciones = new List<RecomendacionPreventivaDto>()
            }
        };

        _searchServiceMock
            .Setup(x => x.BuscarSintomasAvanzadoAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<SintomaDto> { sintomaDto });

        _causaRepositoryMock
            .Setup(x => x.ObtenerCausasPorSintomaAsync(1))
            .ReturnsAsync(causasDto);

        _consultaRepositoryMock
            .Setup(x => x.CrearConsultaAsync(It.IsAny<ConsultaDiagnostico>()))
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _diagnosticoService.DiagnosticarSintomaAsync("test", 1);

        // Assert
        Assert.NotNull(resultado);
        Assert.Contains(palabraClave.ToLower(), resultado.Recomendacion.ToLower());
    }

    [Fact]
    public async Task DiagnosticarSintomaAsync_DebeRegistrarConsultaEnRepositorio()
    {
        // Arrange
        var sintomaDto = new SintomaDto
        {
            Id = 1,
            Descripcion = "Motor sobrecalentado",
            DescripcionTecnica = "Temperatura excesiva",
            NivelUrgencia = 3,
            SistemaAutomotrizId = 1
        };

        _searchServiceMock
            .Setup(x => x.BuscarSintomasAvanzadoAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<SintomaDto> { sintomaDto });

        _causaRepositoryMock
            .Setup(x => x.ObtenerCausasPorSintomaAsync(1))
            .ReturnsAsync(new List<CausaPosibleDto>());

        ConsultaDiagnostico? consultaCapturada = null;
        _consultaRepositoryMock
            .Setup(x => x.CrearConsultaAsync(It.IsAny<ConsultaDiagnostico>()))
            .Callback<ConsultaDiagnostico>(c => consultaCapturada = c)
            .Returns(Task.CompletedTask);

        // Act
        await _diagnosticoService.DiagnosticarSintomaAsync("motor sobrecalentado", 5);

        // Assert
        Assert.NotNull(consultaCapturada);
        Assert.Equal("motor sobrecalentado", consultaCapturada.SintomaDescrito);
        Assert.Equal(5, consultaCapturada.UsuarioId);
        Assert.Equal(1, consultaCapturada.SintomaRelacionadoId);
        _consultaRepositoryMock.Verify(x => x.CrearConsultaAsync(It.IsAny<ConsultaDiagnostico>()), Times.Once);
    }
}
