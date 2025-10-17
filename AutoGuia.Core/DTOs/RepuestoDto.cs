namespace AutoGuia.Core.DTOs
{
    /// <summary>
    /// DTO para mostrar información de una categoría de repuestos
    /// </summary>
    public class CategoriaRepuestoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int TotalRepuestos { get; set; }
    }

    /// <summary>
    /// DTO para mostrar información de un repuesto en el catálogo
    /// </summary>
    public class RepuestoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? NumeroDeParte { get; set; }
        public decimal? PrecioEstimado { get; set; }
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public string? Anio { get; set; }
        public string? ImagenUrl { get; set; }
        public bool EsDisponible { get; set; }
        public DateTime FechaCreacion { get; set; }
        
        // Información de la categoría
        public int CategoriaRepuestoId { get; set; }
        public string NombreCategoria { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para crear un nuevo repuesto
    /// </summary>
    public class CrearRepuestoDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? NumeroDeParte { get; set; }
        public decimal? PrecioEstimado { get; set; }
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public string? Anio { get; set; }
        public string? ImagenUrl { get; set; }
        public int CategoriaRepuestoId { get; set; }
    }

    /// <summary>
    /// DTO para filtros de búsqueda de repuestos
    /// </summary>
    public class FiltroRepuestosDto
    {
        public string? TerminoBusqueda { get; set; }
        public int? CategoriaId { get; set; }
        public string? Marca { get; set; }
        public decimal? PrecioMinimo { get; set; }
        public decimal? PrecioMaximo { get; set; }
        public bool? SoloDisponibles { get; set; } = true;
        public int Pagina { get; set; } = 1;
        public int TamanoPagina { get; set; } = 20;
    }
}