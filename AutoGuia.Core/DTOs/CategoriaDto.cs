#nullable enable

using System.ComponentModel.DataAnnotations;

namespace AutoGuia.Core.DTOs;

/// <summary>
/// DTO para representar una categoría de consumibles automotrices con sus subcategorías
/// </summary>
public record CategoriaDto
{
    /// <summary>
    /// Identificador único de la categoría
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Nombre de la categoría (ej: "Aceites", "Neumáticos", "Plumillas")
    /// </summary>
    public string Nombre { get; init; } = string.Empty;

    /// <summary>
    /// Descripción detallada de la categoría
    /// </summary>
    public string? Descripcion { get; init; }

    /// <summary>
    /// URL del icono para mostrar en la interfaz de usuario
    /// </summary>
    public string? IconUrl { get; init; }

    /// <summary>
    /// Lista de subcategorías asociadas a esta categoría
    /// </summary>
    public List<SubcategoriaDto> Subcategorias { get; init; } = new();
}

/// <summary>
/// DTO para representar una subcategoría con sus valores de filtro
/// </summary>
public record SubcategoriaDto
{
    /// <summary>
    /// Identificador único de la subcategoría
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Nombre de la subcategoría (ej: "Viscosidad", "Tipo", "Marca")
    /// </summary>
    public string Nombre { get; init; } = string.Empty;

    /// <summary>
    /// Lista de valores de filtro disponibles para esta subcategoría
    /// </summary>
    public List<ValorFiltroDto> Valores { get; init; } = new();
}

/// <summary>
/// DTO para representar un valor de filtro específico
/// </summary>
public record ValorFiltroDto
{
    /// <summary>
    /// Identificador único del valor de filtro
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Valor específico del filtro (ej: "10W-40", "Castrol", "4L")
    /// </summary>
    public string Valor { get; init; } = string.Empty;
}

/// <summary>
/// DTO para crear una nueva categoría de consumibles automotrices
/// </summary>
public record CrearCategoriaDto
{
    /// <summary>
    /// Nombre de la categoría
    /// </summary>
    [Required(ErrorMessage = "El nombre de la categoría es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string Nombre { get; init; } = string.Empty;

    /// <summary>
    /// Descripción detallada de la categoría
    /// </summary>
    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    public string? Descripcion { get; init; }

    /// <summary>
    /// URL del icono para mostrar en la interfaz de usuario
    /// </summary>
    [StringLength(200, ErrorMessage = "La URL del icono no puede exceder 200 caracteres")]
    [Url(ErrorMessage = "La URL del icono no es válida")]
    public string? IconUrl { get; init; }
}
