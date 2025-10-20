using Xunit;
using FluentAssertions;
using AutoGuia.Core.DTOs;
using AutoGuia.Infrastructure.Validation;
using FluentValidation.TestHelper;

namespace AutoGuia.Tests.Services.Validation
{
    public class ForoDtoValidatorTests
    {
        private readonly CrearPublicacionDtoValidator _crearPublicacionValidator;
        private readonly CrearRespuestaDtoValidator _crearRespuestaValidator;

        public ForoDtoValidatorTests()
        {
            _crearPublicacionValidator = new CrearPublicacionDtoValidator();
            _crearRespuestaValidator = new CrearRespuestaDtoValidator();
        }

        [Fact]
        public void CrearPublicacion_ConDatosValidos_DebeValidarCorrectamente()
        {
            // Arrange
            var dto = new CrearPublicacionDto
            {
                Titulo = "¿Cuál es el mejor aceite sintético?",
                Contenido = "Estoy buscando recomendaciones para aceite sintético de alta calidad para mi Toyota Corolla 2020.",
                Categoria = "Consultas Técnicas",
                Etiquetas = "aceite,mantenimiento,toyota"
            };

            // Act
            var result = _crearPublicacionValidator.TestValidate(dto);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData("", "El título es obligatorio")]
        [InlineData("123", "El título debe tener entre 5 y 200 caracteres")]
        public void CrearPublicacion_ConTituloInvalido_DebeRetornarError(string titulo, string mensajeEsperado)
        {
            // Arrange
            var dto = new CrearPublicacionDto
            {
                Titulo = titulo,
                Contenido = "Contenido válido de la publicación",
                Categoria = "Consultas Técnicas"
            };

            // Act
            var result = _crearPublicacionValidator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Titulo)
                .WithErrorMessage(mensajeEsperado);
        }

        [Theory]
        [InlineData("spam")]
        [InlineData("SCAM aquí")]
        [InlineData("Fraude total")]
        public void CrearPublicacion_ConPalabrasOfensivas_DebeRetornarError(string titulo)
        {
            // Arrange
            var dto = new CrearPublicacionDto
            {
                Titulo = titulo,
                Contenido = "Contenido de prueba",
                Categoria = "Consultas Técnicas"
            };

            // Act
            var result = _crearPublicacionValidator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Titulo)
                .WithErrorMessage("El título contiene palabras no permitidas");
        }

        [Theory]
        [InlineData("", "El contenido es obligatorio")]
        [InlineData("Corto", "El contenido debe tener al menos 10 caracteres")]
        public void CrearPublicacion_ConContenidoInvalido_DebeRetornarError(
            string contenido, string mensajeEsperado)
        {
            // Arrange
            var dto = new CrearPublicacionDto
            {
                Titulo = "Título válido de prueba",
                Contenido = contenido,
                Categoria = "Consultas Técnicas"
            };

            // Act
            var result = _crearPublicacionValidator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Contenido)
                .WithErrorMessage(mensajeEsperado);
        }

        [Fact]
        public void CrearPublicacion_ConContenidoMuyLargo_DebeRetornarError()
        {
            // Arrange
            var contenidoMuyLargo = new string('a', 5001);
            var dto = new CrearPublicacionDto
            {
                Titulo = "Título válido de prueba",
                Contenido = contenidoMuyLargo,
                Categoria = "Consultas Técnicas"
            };

            // Act
            var result = _crearPublicacionValidator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Contenido)
                .WithErrorMessage("El contenido no puede exceder 5000 caracteres");
        }

        [Theory]
        [InlineData("Categoría Inválida")]
        public void CrearPublicacion_ConCategoriaInvalida_DebeRetornarError(string categoria)
        {
            // Arrange
            var dto = new CrearPublicacionDto
            {
                Titulo = "Título válido de prueba",
                Contenido = "Contenido válido de la publicación",
                Categoria = categoria
            };

            // Act
            var result = _crearPublicacionValidator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Categoria);
        }

        [Fact]
        public void CrearPublicacion_ConEtiquetasMuyLargas_DebeRetornarError()
        {
            // Arrange
            var etiquetasMuyLargas = new string('a', 201);
            var dto = new CrearPublicacionDto
            {
                Titulo = "Título válido de prueba",
                Contenido = "Contenido válido de la publicación",
                Categoria = "Consultas Técnicas",
                Etiquetas = etiquetasMuyLargas
            };

            // Act
            var result = _crearPublicacionValidator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Etiquetas)
                .WithErrorMessage("Las etiquetas no pueden exceder 200 caracteres");
        }

        [Fact]
        public void CrearRespuesta_ConDatosValidos_DebeValidarCorrectamente()
        {
            // Arrange
            var dto = new CrearRespuestaDto
            {
                PublicacionId = 1,
                Contenido = "Esta es una respuesta válida a la publicación."
            };

            // Act
            var result = _crearRespuestaValidator.TestValidate(dto);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void CrearRespuesta_ConPublicacionIdInvalido_DebeRetornarError(int publicacionId)
        {
            // Arrange
            var dto = new CrearRespuestaDto
            {
                PublicacionId = publicacionId,
                Contenido = "Contenido válido"
            };

            // Act
            var result = _crearRespuestaValidator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.PublicacionId)
                .WithErrorMessage("PublicacionId debe ser mayor a 0");
        }

        [Theory]
        [InlineData("", "El contenido de la respuesta es obligatorio")]
        [InlineData("1234", "La respuesta debe tener al menos 5 caracteres")]
        public void CrearRespuesta_ConContenidoInvalido_DebeRetornarError(
            string contenido, string mensajeEsperado)
        {
            // Arrange
            var dto = new CrearRespuestaDto
            {
                PublicacionId = 1,
                Contenido = contenido
            };

            // Act
            var result = _crearRespuestaValidator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Contenido)
                .WithErrorMessage(mensajeEsperado);
        }

        [Fact]
        public void CrearRespuesta_ConContenidoMuyLargo_DebeRetornarError()
        {
            // Arrange
            var contenidoMuyLargo = new string('a', 2001);
            var dto = new CrearRespuestaDto
            {
                PublicacionId = 1,
                Contenido = contenidoMuyLargo
            };

            // Act
            var result = _crearRespuestaValidator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Contenido)
                .WithErrorMessage("La respuesta no puede exceder 2000 caracteres");
        }
    }
}
