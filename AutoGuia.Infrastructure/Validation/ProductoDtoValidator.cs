using AutoGuia.Core.DTOs;
using FluentValidation;

namespace AutoGuia.Infrastructure.Validation
{
    /// <summary>
    /// Validador para CrearProductoDto
    /// </summary>
    public class CrearProductoDtoValidator : AbstractValidator<CrearProductoDto>
    {
        public CrearProductoDtoValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre del producto es obligatorio")
                .Length(3, 200).WithMessage("El nombre debe tener entre 3 y 200 caracteres");

            RuleFor(x => x.Descripcion)
                .MaximumLength(1000).WithMessage("La descripción no puede exceder 1000 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Descripcion));

            RuleFor(x => x.Categoria)
                .NotEmpty().WithMessage("La categoría es obligatoria")
                .Length(2, 50).WithMessage("La categoría debe tener entre 2 y 50 caracteres");

            RuleFor(x => x.Marca)
                .MaximumLength(50).WithMessage("La marca no puede exceder 50 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Marca));
        }
    }

    /// <summary>
    /// Validador para ActualizarProductoDto
    /// </summary>
    public class ActualizarProductoDtoValidator : AbstractValidator<ActualizarProductoDto>
    {
        public ActualizarProductoDtoValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre del producto es obligatorio")
                .Length(3, 200).WithMessage("El nombre debe tener entre 3 y 200 caracteres");

            RuleFor(x => x.Descripcion)
                .MaximumLength(1000).WithMessage("La descripción no puede exceder 1000 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Descripcion));

            RuleFor(x => x.Categoria)
                .NotEmpty().WithMessage("La categoría es obligatoria");
        }
    }

    /// <summary>
    /// Validador para BusquedaProductoDto
    /// </summary>
    public class BusquedaProductoDtoValidator : AbstractValidator<BusquedaProductoDto>
    {
        public BusquedaProductoDtoValidator()
        {
            RuleFor(x => x.TerminoBusqueda)
                .MinimumLength(2).WithMessage("El término de búsqueda debe tener al menos 2 caracteres")
                .When(x => !string.IsNullOrEmpty(x.TerminoBusqueda));

            RuleFor(x => x.PrecioMinimo)
                .GreaterThanOrEqualTo(0).WithMessage("El precio mínimo no puede ser negativo")
                .When(x => x.PrecioMinimo.HasValue);

            RuleFor(x => x.PrecioMaximo)
                .GreaterThanOrEqualTo(0).WithMessage("El precio máximo no puede ser negativo")
                .GreaterThan(x => x.PrecioMinimo ?? 0)
                .WithMessage("El precio máximo debe ser mayor que el mínimo")
                .When(x => x.PrecioMaximo.HasValue && x.PrecioMinimo.HasValue);
        }
    }
}
