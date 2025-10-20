#nullable enable

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoGuia.Core.Entities;

/// <summary>
/// Representa un valor de filtro específico para una subcategoría
/// (ej: "10W-40", "Castrol", "4L")
/// </summary>
public class ValorFiltro
{
    /// <summary>
    /// Identificador único del valor de filtro
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador de la subcategoría a la que pertenece este valor
    /// </summary>
    [Required(ErrorMessage = "El SubcategoriaId es requerido")]
    public int SubcategoriaId { get; set; }

    /// <summary>
    /// Valor específico del filtro (ej: "10W-40", "Castrol", "4L")
    /// </summary>
    [Required(ErrorMessage = "El valor es requerido")]
    [StringLength(100, ErrorMessage = "El valor no puede exceder 100 caracteres")]
    public string Valor { get; set; } = string.Empty;

    // =============== NAVEGACIÓN ===============

    /// <summary>
    /// Subcategoría a la que pertenece este valor de filtro
    /// </summary>
    [ForeignKey(nameof(SubcategoriaId))]
    public virtual Subcategoria? Subcategoria { get; set; }
}
