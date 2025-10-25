using System.ComponentModel.DataAnnotations;

namespace AutoGuia.Web.DTOs;

/// <summary>
/// DTO para representar una suscripción de usuario
/// </summary>
public class SuscripcionDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El ID del usuario es requerido")]
    public string UsuarioId { get; set; } = string.Empty;

    [Required(ErrorMessage = "El ID del plan es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID del plan debe ser mayor a 0")]
    public int PlanId { get; set; }

    public PlanDto? Plan { get; set; }

    [Required(ErrorMessage = "La fecha de inicio es requerida")]
    public DateTime FechaInicio { get; set; }

    [Required(ErrorMessage = "La fecha de fin es requerida")]
    public DateTime FechaFin { get; set; }
    
    // Alias para compatibilidad con la entidad
    public DateTime FechaVencimiento => FechaFin;

    public DateTime? FechaCancelacion { get; set; }

    public bool EsActiva { get; set; }

    public int DiagnosticosUsados { get; set; }
    
    // Alias para compatibilidad con la entidad
    public int DiagnosticosUtilizados => DiagnosticosUsados;

    public int BusquedasUtilizadas { get; set; }
    
    public DateTime? UltimoReseteo { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public bool LimiteDiagnosticosAlcanzado { get; set; }
    
    public bool LimiteBusquedasAlcanzado { get; set; }

    /// <summary>
    /// Indica si la suscripción está vigente (no cancelada y dentro del período)
    /// </summary>
    public bool EsVigente => EsActiva && FechaCancelacion == null && DateTime.UtcNow >= FechaInicio && DateTime.UtcNow <= FechaFin;

    /// <summary>
    /// Indica si la suscripción ha vencido
    /// </summary>
    public bool EsVencida => DateTime.UtcNow > FechaFin;

    /// <summary>
    /// Días restantes de la suscripción
    /// </summary>
    public int DiasRestantes => EsVigente ? (FechaFin - DateTime.UtcNow).Days : 0;
}
