using AutoGuia.Core.DTOs;
using FluentValidation;

namespace AutoGuia.Infrastructure.Validation
{
    /// <summary>
    /// Validador para CrearTallerDto usando FluentValidation
    /// </summary>
    public class CrearTallerDtoValidator : AbstractValidator<CrearTallerDto>
    {
        public CrearTallerDtoValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre del taller es obligatorio")
                .Length(3, 100).WithMessage("El nombre debe tener entre 3 y 100 caracteres")
                .Matches(@"^[a-zA-Z0-9\sáéíóúÁÉÍÓÚñÑ\-\.]+$")
                .WithMessage("El nombre contiene caracteres no válidos");

            RuleFor(x => x.Direccion)
                .NotEmpty().WithMessage("La dirección es obligatoria")
                .Length(5, 200).WithMessage("La dirección debe tener entre 5 y 200 caracteres");

            RuleFor(x => x.Ciudad)
                .NotEmpty().WithMessage("La ciudad es obligatoria")
                .Length(2, 50).WithMessage("La ciudad debe tener entre 2 y 50 caracteres");

            RuleFor(x => x.Region)
                .NotEmpty().WithMessage("La región es obligatoria")
                .Length(2, 50).WithMessage("La región debe tener entre 2 y 50 caracteres");

            RuleFor(x => x.Telefono)
                .NotEmpty().WithMessage("El teléfono es obligatorio")
                .Matches(@"^\+?[0-9\s\-\(\)]{7,20}$")
                .WithMessage("Formato de teléfono no válido");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Email no válido")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.Latitud)
                .InclusiveBetween(-90, 90).WithMessage("Latitud debe estar entre -90 y 90")
                .When(x => x.Latitud.HasValue);

            RuleFor(x => x.Longitud)
                .InclusiveBetween(-180, 180).WithMessage("Longitud debe estar entre -180 y 180")
                .When(x => x.Longitud.HasValue);
        }
    }

    /// <summary>
    /// Validador para ActualizarTallerDto
    /// </summary>
    public class ActualizarTallerDtoValidator : AbstractValidator<ActualizarTallerDto>
    {
        public ActualizarTallerDtoValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre del taller es obligatorio")
                .Length(3, 100).WithMessage("El nombre debe tener entre 3 y 100 caracteres");

            RuleFor(x => x.Direccion)
                .NotEmpty().WithMessage("La dirección es obligatoria")
                .Length(5, 200).WithMessage("La dirección debe tener entre 5 y 200 caracteres");

            RuleFor(x => x.Telefono)
                .Matches(@"^\+?[0-9\s\-\(\)]{7,20}$")
                .WithMessage("Formato de teléfono no válido")
                .When(x => !string.IsNullOrEmpty(x.Telefono));

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Email no válido")
                .When(x => !string.IsNullOrEmpty(x.Email));
        }
    }
}
