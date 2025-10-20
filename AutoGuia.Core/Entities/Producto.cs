#nullable enable

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoGuia.Core.Entities;

/// <summary>
/// Representa un producto o consumible automotriz en el sistema
/// (Aceites, Neumáticos, Plumillas, Filtros, etc.)
/// </summary>
public class Producto
{
    /// <summary>
    /// Identificador único del producto
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador de la categoría a la que pertenece el producto
    /// </summary>
    [Required(ErrorMessage = "La categoría es requerida")]
    public int CategoriaId { get; set; }

    /// <summary>
    /// Nombre del producto
    /// </summary>
    [Required(ErrorMessage = "El nombre del producto es requerido")]
    [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Número de parte del fabricante o código SKU universal
    /// </summary>
    [StringLength(100, ErrorMessage = "El número de parte no puede exceder 100 caracteres")]
    public string? NumeroDeParte { get; set; }

    /// <summary>
    /// Descripción detallada del producto
    /// </summary>
    [StringLength(2000, ErrorMessage = "La descripción no puede exceder 2000 caracteres")]
    public string? Descripcion { get; set; }

    /// <summary>
    /// URL de la imagen principal del producto
    /// </summary>
    [StringLength(500, ErrorMessage = "La URL de la imagen no puede exceder 500 caracteres")]
    public string? ImagenUrl { get; set; }

    /// <summary>
    /// Marca del producto
    /// </summary>
    [StringLength(100, ErrorMessage = "La marca no puede exceder 100 caracteres")]
    public string? Marca { get; set; }

    /// <summary>
    /// Valor de filtro 1 - Primer criterio de búsqueda (ej: "10W-40" para aceites)
    /// </summary>
    [StringLength(100, ErrorMessage = "El filtro valor 1 no puede exceder 100 caracteres")]
    public string? FiltroValor1 { get; set; }

    /// <summary>
    /// Valor de filtro 2 - Segundo criterio de búsqueda (ej: "Castrol" para marca específica)
    /// </summary>
    [StringLength(100, ErrorMessage = "El filtro valor 2 no puede exceder 100 caracteres")]
    public string? FiltroValor2 { get; set; }

    /// <summary>
    /// Valor de filtro 3 - Tercer criterio de búsqueda (ej: "4L" para capacidad)
    /// </summary>
    [StringLength(100, ErrorMessage = "El filtro valor 3 no puede exceder 100 caracteres")]
    public string? FiltroValor3 { get; set; }

    /// <summary>
    /// Especificaciones técnicas adicionales en formato JSON
    /// </summary>
    [StringLength(2000, ErrorMessage = "Las especificaciones no pueden exceder 2000 caracteres")]
    public string? Especificaciones { get; set; }

    /// <summary>
    /// Calificación promedio del producto (0-5)
    /// </summary>
    [Column(TypeName = "decimal(3,2)")]
    public decimal? CalificacionPromedio { get; set; }

    /// <summary>
    /// Cantidad total de reseñas del producto
    /// </summary>
    public int TotalResenas { get; set; } = 0;

    /// <summary>
    /// Indica si el producto está activo en el sistema
    /// </summary>
    public bool EsActivo { get; set; } = true;

    /// <summary>
    /// Fecha de creación del registro
    /// </summary>
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha de última actualización del registro
    /// </summary>
    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;

    // =============== NAVEGACIÓN ===============

    /// <summary>
    /// Categoría a la que pertenece el producto
    /// </summary>
    [ForeignKey(nameof(CategoriaId))]
    public virtual Categoria? Categoria { get; set; }

    /// <summary>
    /// Ofertas disponibles para este producto en diferentes tiendas
    /// </summary>
    public virtual ICollection<Oferta> Ofertas { get; set; } = new List<Oferta>();

    /// <summary>
    /// Compatibilidad del producto con diferentes modelos de vehículos
    /// </summary>
    public virtual ICollection<ProductoVehiculoCompatible> VehiculosCompatibles { get; set; } = new List<ProductoVehiculoCompatible>();
}
