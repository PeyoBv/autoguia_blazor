#nullable enable

using System.ComponentModel.DataAnnotations;

namespace AutoGuia.Core.Entities;

/// <summary>
/// Representa una categoría de consumibles automotrices (Aceites, Neumáticos, etc.)
/// </summary>
public class Categoria
{
    /// <summary>
    /// Identificador único de la categoría
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre de la categoría (ej: "Aceites", "Neumáticos", "Plumillas")
    /// </summary>
    [Required(ErrorMessage = "El nombre de la categoría es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Descripción detallada de la categoría
    /// </summary>
    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    public string? Descripcion { get; set; }

    /// <summary>
    /// URL del icono para mostrar en la UI
    /// </summary>
    [StringLength(200, ErrorMessage = "La URL del icono no puede exceder 200 caracteres")]
    public string? IconUrl { get; set; }

    /// <summary>
    /// Fecha de creación de la categoría
    /// </summary>
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Indica si la categoría está activa
    /// </summary>
    public bool EsActivo { get; set; } = true;

    // =============== NAVEGACIÓN ===============

    /// <summary>
    /// Subcategorías asociadas a esta categoría
    /// </summary>
    public virtual ICollection<Subcategoria> Subcategorias { get; set; } = new List<Subcategoria>();
}
