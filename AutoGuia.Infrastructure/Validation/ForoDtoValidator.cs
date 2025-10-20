using AutoGuia.Core.DTOs;
using FluentValidation;

namespace AutoGuia.Infrastructure.Validation
{
    /// <summary>
    /// Validador para CrearPublicacionDto
    /// </summary>
    public class CrearPublicacionDtoValidator : AbstractValidator<CrearPublicacionDto>
    {
        private static readonly string[] CategoriasPermitidas = 
        {
            "Valoraciones y Reseñas",
            "Consultas Técnicas",
            "Rendimiento y Tuning",
            "Productos Alternativos"
        };

        public CrearPublicacionDtoValidator()
        {
            RuleFor(x => x.Titulo)
                .NotEmpty().WithMessage("El título es obligatorio")
                .Length(5, 200).WithMessage("El título debe tener entre 5 y 200 caracteres")
                .Must(NoContenerPalabrasOfensivas)
                .WithMessage("El título contiene palabras no permitidas");

            RuleFor(x => x.Contenido)
                .NotEmpty().WithMessage("El contenido es obligatorio")
                .MinimumLength(10).WithMessage("El contenido debe tener al menos 10 caracteres")
                .MaximumLength(5000).WithMessage("El contenido no puede exceder 5000 caracteres");

            RuleFor(x => x.Categoria)
                .Must(categoria => string.IsNullOrEmpty(categoria) || CategoriasPermitidas.Contains(categoria))
                .WithMessage($"Categoría debe ser una de: {string.Join(", ", CategoriasPermitidas)}")
                .When(x => !string.IsNullOrEmpty(x.Categoria));

            RuleFor(x => x.Etiquetas)
                .MaximumLength(200).WithMessage("Las etiquetas no pueden exceder 200 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Etiquetas));
        }

        private bool NoContenerPalabrasOfensivas(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return true;

            var palabrasOfensivas = new[] { "spam", "scam", "fraude" }; // Lista básica
            return !palabrasOfensivas.Any(p => 
                texto.Contains(p, StringComparison.OrdinalIgnoreCase));
        }
    }

    /// <summary>
    /// Validador para CrearRespuestaDto
    /// </summary>
    public class CrearRespuestaDtoValidator : AbstractValidator<CrearRespuestaDto>
    {
        public CrearRespuestaDtoValidator()
        {
            RuleFor(x => x.PublicacionId)
                .GreaterThan(0).WithMessage("PublicacionId debe ser mayor a 0");

            RuleFor(x => x.Contenido)
                .NotEmpty().WithMessage("El contenido de la respuesta es obligatorio")
                .MinimumLength(5).WithMessage("La respuesta debe tener al menos 5 caracteres")
                .MaximumLength(2000).WithMessage("La respuesta no puede exceder 2000 caracteres");
        }
    }
}
