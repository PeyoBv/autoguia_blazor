using AutoGuia.Core.DTOs;

namespace AutoGuia.Infrastructure.ExternalServices
{
    /// <summary>
    /// Contrato para servicios de marketplaces externos (MercadoLibre, eBay, etc.)
    /// </summary>
    public interface IExternalMarketplaceService
    {
        /// <summary>
        /// Obtiene el nombre del marketplace
        /// </summary>
        string NombreMarketplace { get; }

        /// <summary>
        /// Indica si el servicio está disponible
        /// </summary>
        Task<bool> EstaDisponibleAsync();

        /// <summary>
        /// Busca productos en el marketplace
        /// </summary>
        /// <param name="termino">Término de búsqueda</param>
        /// <param name="categoria">Categoría opcional</param>
        /// <param name="limite">Cantidad máxima de resultados</param>
        /// <returns>Lista de ofertas encontradas</returns>
        Task<IEnumerable<OfertaExternaDto>> BuscarProductosAsync(
            string termino, 
            string? categoria = null, 
            int limite = 20);

        /// <summary>
        /// Obtiene detalles de un producto específico
        /// </summary>
        /// <param name="productoId">ID del producto en el marketplace</param>
        /// <returns>Detalle de la oferta</returns>
        Task<OfertaExternaDto?> ObtenerDetalleProductoAsync(string productoId);

        /// <summary>
        /// Obtiene las categorías disponibles en el marketplace
        /// </summary>
        /// <returns>Lista de categorías</returns>
        Task<IEnumerable<CategoriaMarketplaceDto>> ObtenerCategoriasAsync();

        /// <summary>
        /// Normaliza el nombre de una categoría al estándar de AutoGuía
        /// </summary>
        /// <param name="categoriaNativa">Nombre de la categoría en el marketplace</param>
        /// <returns>Nombre normalizado</returns>
        string NormalizarCategoria(string categoriaNativa);
    }

    /// <summary>
    /// DTO para ofertas de marketplaces externos
    /// </summary>
    public class OfertaExternaDto
    {
        public string Id { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public string Moneda { get; set; } = "CLP";
        public string? ImagenUrl { get; set; }
        public string UrlProducto { get; set; } = string.Empty;
        public string NombreTienda { get; set; } = string.Empty;
        public string Marketplace { get; set; } = string.Empty;
        public string Condicion { get; set; } = "Nuevo";
        public int Stock { get; set; }
        public bool EnvioGratis { get; set; }
        public double? Calificacion { get; set; }
        public int CantidadVendidos { get; set; }
        public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// DTO para categorías de marketplaces
    /// </summary>
    public class CategoriaMarketplaceDto
    {
        public string Id { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Icono { get; set; }
        public int TotalProductos { get; set; }
    }
}
