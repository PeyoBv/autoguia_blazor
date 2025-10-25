using System.ComponentModel.DataAnnotations;

namespace AutoGuia.Web.DTOs;

/// <summary>
/// DTO para representar un plan de suscripción
/// </summary>
public class PlanDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del plan es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    public string? Descripcion { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser mayor o igual a 0")]
    public decimal Precio { get; set; }

    // Propiedades calculadas para compatibilidad con Suscripciones.razor
    public decimal PrecioMensual => Duracion == "Mensual" ? Precio : Math.Round(Precio / 12, 2);
    
    public decimal PrecioAnual => Duracion == "Anual" ? Precio : Precio * 12;

    public string Duracion { get; set; } = "Mensual";

    [Range(0, int.MaxValue, ErrorMessage = "El límite de diagnósticos debe ser mayor o igual a 0")]
    public int LimiteDiagnosticos { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "El límite de diagnósticos debe ser mayor o igual a 0")]
    public int LimiteDiagnosticosMes => LimiteDiagnosticos;

    public int LimiteBusquedas { get; set; }

    public bool EsPopular => Destacado;
    
    public bool Destacado { get; set; }

    public bool EsActivo { get; set; }

    public List<string> Caracteristicas { get; set; } = new();

    /// <summary>
    /// Precio formateado con símbolo de moneda
    /// </summary>
    public string PrecioFormateado => Precio == 0 ? "Gratis" : $"${Precio:N0} CLP";

    /// <summary>
    /// Indica si el plan es gratuito
    /// </summary>
    public bool EsGratuito => Precio == 0;
}
