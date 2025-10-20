using Xunit;
using FluentAssertions;
using AutoGuia.Core.DTOs;
using AutoGuia.Infrastructure.Validation;
using FluentValidation.TestHelper;

namespace AutoGuia.Tests.Services.Validation
{
    public class TallerDtoValidatorTests
    {
        private readonly CrearTallerDtoValidator _crearValidator;
        private readonly ActualizarTallerDtoValidator _actualizarValidator;

        public TallerDtoValidatorTests()
        {
            _crearValidator = new CrearTallerDtoValidator();
            _actualizarValidator = new ActualizarTallerDtoValidator();
        }

        [Fact]
        public void CrearTaller_ConDatosValidos_DebeValidarCorrectamente()
        {
            // Arrange
            var dto = new CrearTallerDto
            {
                Nombre = "Taller Mecánico Central",
                Direccion = "Av. Providencia 1234",
                Ciudad = "Santiago",
                Region = "Metropolitana",
                Telefono = "+56912345678",
                Email = "contacto@tallercentral.cl",
                Latitud = -33.4489,
                Longitud = -70.6693
            };

            // Act
            var result = _crearValidator.TestValidate(dto);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData("", "El nombre del taller es obligatorio")]
        [InlineData("Ab", "El nombre debe tener entre 3 y 100 caracteres")]
        public void CrearTaller_ConNombreInvalido_DebeRetornarError(string nombre, string mensajeEsperado)
        {
            // Arrange
            var dto = new CrearTallerDto
            {
                Nombre = nombre,
                Direccion = "Av. Providencia 1234",
                Ciudad = "Santiago",
                Region = "Metropolitana",
                Telefono = "+56912345678"
            };

            // Act
            var result = _crearValidator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Nombre)
                .WithErrorMessage(mensajeEsperado);
        }

        [Theory]
        [InlineData("telefono_invalido")]
        [InlineData("123")]
        [InlineData("++++++")]
        public void CrearTaller_ConTelefonoInvalido_DebeRetornarError(string telefono)
        {
            // Arrange
            var dto = new CrearTallerDto
            {
                Nombre = "Taller Test",
                Direccion = "Direccion 123",
                Ciudad = "Santiago",
                Region = "Metropolitana",
                Telefono = telefono
            };

            // Act
            var result = _crearValidator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Telefono)
                .WithErrorMessage("Formato de teléfono no válido");
        }

        [Theory]
        [InlineData("correo_invalido")]
        [InlineData("@@@")]
        [InlineData("sin-arroba.com")]
        public void CrearTaller_ConEmailInvalido_DebeRetornarError(string email)
        {
            // Arrange
            var dto = new CrearTallerDto
            {
                Nombre = "Taller Test",
                Direccion = "Direccion 123",
                Ciudad = "Santiago",
                Region = "Metropolitana",
                Telefono = "+56912345678",
                Email = email
            };

            // Act
            var result = _crearValidator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Theory]
        [InlineData(-100, "Latitud debe estar entre -90 y 90")]
        [InlineData(100, "Latitud debe estar entre -90 y 90")]
        public void CrearTaller_ConLatitudFueraDeRango_DebeRetornarError(double latitud, string mensajeEsperado)
        {
            // Arrange
            var dto = new CrearTallerDto
            {
                Nombre = "Taller Test",
                Direccion = "Direccion 123",
                Ciudad = "Santiago",
                Region = "Metropolitana",
                Telefono = "+56912345678",
                Latitud = latitud
            };

            // Act
            var result = _crearValidator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Latitud)
                .WithErrorMessage(mensajeEsperado);
        }

        [Theory]
        [InlineData(-200, "Longitud debe estar entre -180 y 180")]
        [InlineData(200, "Longitud debe estar entre -180 y 180")]
        public void CrearTaller_ConLongitudFueraDeRango_DebeRetornarError(double longitud, string mensajeEsperado)
        {
            // Arrange
            var dto = new CrearTallerDto
            {
                Nombre = "Taller Test",
                Direccion = "Direccion 123",
                Ciudad = "Santiago",
                Region = "Metropolitana",
                Telefono = "+56912345678",
                Longitud = longitud
            };

            // Act
            var result = _crearValidator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Longitud)
                .WithErrorMessage(mensajeEsperado);
        }
    }
}
