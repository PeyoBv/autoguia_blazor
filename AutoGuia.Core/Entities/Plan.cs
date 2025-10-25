using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoGuia.Core.Entities;

/// <summary>
/// Plan de suscripción disponible en AutoGuía
/// </summary>
public class Plan
{
    /// <summary>
    /// Identificador único del plan
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Nombre del plan (ej: "Gratis", "Pro", "Premium")
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Descripción detallada del plan
    /// </summary>
    [Required]
    [StringLength(500)]
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>
    /// Precio del plan en CLP (pesos chilenos)
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal Precio { get; set; }

    /// <summary>
    /// Tipo de duración del plan (Mensual o Anual)
    /// </summary>
    public TipoDuracion Duracion { get; set; } = TipoDuracion.Mensual;

    /// <summary>
    /// Límite de diagnósticos con IA por mes (0 = ilimitado)
    /// </summary>
    public int LimiteDiagnosticos { get; set; }

    /// <summary>
    /// Límite de búsquedas de repuestos por día (0 = ilimitado)
    /// </summary>
    public int LimiteBusquedas { get; set; }

    /// <summary>
    /// Indica si el plan tiene acceso al foro comunitario
    /// </summary>
    public bool AccesoForo { get; set; } = true;

    /// <summary>
    /// Indica si el plan tiene acceso a mapas de talleres
    /// </summary>
    public bool AccesoMapas { get; set; } = true;

    /// <summary>
    /// Indica si el plan tiene soporte prioritario
    /// </summary>
    public bool SoportePrioritario { get; set; } = false;

    /// <summary>
    /// Indica si el plan tiene acceso a comparador de precios
    /// </summary>
    public bool AccesoComparador { get; set; } = true;

    /// <summary>
    /// Características adicionales del plan en formato JSON
    /// (ej: ["Sin publicidad", "Notificaciones push", "Estadísticas avanzadas"])
    /// </summary>
    [Column(TypeName = "jsonb")]
    public string[]? Caracteristicas { get; set; }

    /// <summary>
    /// Indica si el plan está activo y disponible para suscripción
    /// </summary>
    public bool Activo { get; set; } = true;

    /// <summary>
    /// Indica si el plan es destacado (se muestra primero)
    /// </summary>
    public bool Destacado { get; set; } = false;

    /// <summary>
    /// Color hexadecimal para identificar el plan en la UI (ej: "#4CAF50")
    /// </summary>
    [StringLength(7)]
    public string? ColorBadge { get; set; }

    /// <summary>
    /// Orden de visualización en la lista de planes
    /// </summary>
    public int Orden { get; set; }

    /// <summary>
    /// Fecha de creación del plan
    /// </summary>
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha de última actualización del plan
    /// </summary>
    public DateTime? FechaActualizacion { get; set; }

    // Relaciones

    /// <summary>
    /// Suscripciones asociadas a este plan
    /// </summary>
    public virtual ICollection<Suscripcion> Suscripciones { get; set; } = new List<Suscripcion>();

    /// <summary>
    /// Verifica si el plan es gratuito
    /// </summary>
    [NotMapped]
    public bool EsGratuito => Precio == 0;

    /// <summary>
    /// Obtiene el precio formateado con símbolo de moneda
    /// </summary>
    [NotMapped]
    public string PrecioFormateado => Precio == 0 ? "Gratis" : $"${Precio:N0} CLP";

    /// <summary>
    /// Calcula el precio mensual equivalente (útil para planes anuales)
    /// </summary>
    [NotMapped]
    public decimal PrecioMensualEquivalente
    {
        get
        {
            return Duracion == TipoDuracion.Anual
                ? Math.Round(Precio / 12, 2)
                : Precio;
        }
    }

    /// <summary>
    /// Obtiene el texto de duración en español
    /// </summary>
    [NotMapped]
    public string DuracionTexto
    {
        get
        {
            return Duracion switch
            {
                TipoDuracion.Mensual => "Mensual",
                TipoDuracion.Anual => "Anual",
                _ => "Desconocido"
            };
        }
    }
}
