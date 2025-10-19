using System.ComponentModel.DataAnnotations;

namespace AutoGuia.Core.DTOs;

/// <summary>
/// DTO para la búsqueda de repuestos desde el comparador
/// </summary>
public class BusquedaRepuestoQuery
{
    [Required(ErrorMessage = "El término de búsqueda es requerido")]
    [MinLength(3, ErrorMessage = "Ingrese al menos 3 caracteres")]
    public string TerminoDeBusqueda { get; set; } = string.Empty;
    
    public string? Marca { get; set; }
    
    public string? Modelo { get; set; }
    
    public string? Ano { get; set; }
    
    public string? Motor { get; set; }
    
    public string? Version { get; set; }
}
