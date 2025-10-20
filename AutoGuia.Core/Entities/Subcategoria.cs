#nullable enable

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoGuia.Core.Entities;

/// <summary>
/// Representa una subcategoría de filtrado dentro de una categoría de consumibles
/// (ej: Viscosidad, Tipo, Marca, Tamaño)
/// </summary>
public class Subcategoria
{
    /// <summary>
    /// Identificador único de la subcategoría
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador de la categoría padre
    /// </summary>
    [Required(ErrorMessage = "El CategoriaId es requerido")]
    public int CategoriaId { get; set; }

    /// <summary>
    /// Nombre de la subcategoría (ej: "Viscosidad", "Tipo", "Marca")
    /// </summary>
    [Required(ErrorMessage = "El nombre de la subcategoría es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    // =============== NAVEGACIÓN ===============

    /// <summary>
    /// Categoría padre a la que pertenece esta subcategoría
    /// </summary>
    [ForeignKey(nameof(CategoriaId))]
    public virtual Categoria? Categoria { get; set; }

    /// <summary>
    /// Valores de filtro disponibles para esta subcategoría
    /// </summary>
    public virtual ICollection<ValorFiltro> Valores { get; set; } = new List<ValorFiltro>();
}
