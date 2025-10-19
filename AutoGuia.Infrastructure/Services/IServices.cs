using AutoGuia.Core.DTOs;
using AutoGuia.Core.Entities;

namespace AutoGuia.Infrastructure.Services
{
    public interface ITallerService
    {
        Task<IEnumerable<TallerDto>> ObtenerTalleresAsync();
        Task<IEnumerable<TallerDto>> BuscarTalleresPorCiudadAsync(string ciudad);
        Task<TallerDto?> ObtenerTallerPorIdAsync(int id);
        
        // Operaciones CRUD para administración
        Task<int> CrearTallerAsync(CrearTallerDto taller);
        Task<bool> ActualizarTallerAsync(int id, ActualizarTallerDto taller);
        Task<bool> EliminarTallerAsync(int id);
        Task<bool> CambiarEstadoVerificacionAsync(int id, bool esVerificado);
    }

    public interface IForoService
    {
        Task<IEnumerable<PublicacionForoDto>> ObtenerPublicacionesAsync(int pagina = 1, int tamanoPagina = 10);
        Task<PublicacionForoDto?> ObtenerPublicacionPorIdAsync(int id);
        Task<IEnumerable<RespuestaForoDto>> ObtenerRespuestasAsync(int publicacionId);
        Task<int> CrearPublicacionAsync(CrearPublicacionDto publicacion, int usuarioId);
        Task<int> CrearRespuestaAsync(CrearRespuestaDto respuesta, int usuarioId);
    }

    // IResenaService eliminado - no necesario en arquitectura de comparación de precios

    /// <summary>
    /// Servicio para la gestión e interacción con mapas interactivos
    /// </summary>
    public interface IMapService
    {
        /// <summary>
        /// Inicializa un mapa interactivo en el elemento HTML especificado
        /// </summary>
        /// <param name="mapElementId">ID del elemento HTML donde se renderizará el mapa</param>
        /// <param name="talleres">Colección de talleres para mostrar como marcadores</param>
        /// <param name="apiKey">Clave de API para el servicio de mapas</param>
        /// <returns>Task que representa la operación asíncrona</returns>
        Task InicializarMapaAsync(string mapElementId, IEnumerable<Taller> talleres, string apiKey);

        /// <summary>
        /// Agrega un marcador individual al mapa existente
        /// </summary>
        /// <param name="taller">Taller a agregar como marcador</param>
        /// <returns>Task que representa la operación asíncrona</returns>
        Task AgregarMarcadorAsync(Taller taller);

        /// <summary>
        /// Centra el mapa en las coordenadas especificadas
        /// </summary>
        /// <param name="latitud">Latitud del centro</param>
        /// <param name="longitud">Longitud del centro</param>
        /// <param name="zoom">Nivel de zoom</param>
        /// <returns>Task que representa la operación asíncrona</returns>
        Task CentrarMapaAsync(double latitud, double longitud, int zoom = 12);

        /// <summary>
        /// Limpia todos los marcadores del mapa
        /// </summary>
        /// <returns>Task que representa la operación asíncrona</returns>
        Task LimpiarMarcadoresAsync();
    }

    /// <summary>
    /// Servicio para la gestión del sistema de comparación de precios de productos automotrices
    /// </summary>
    public interface IComparadorService
    {
        /// <summary>
        /// Busca productos por término de búsqueda con comparación de precios
        /// </summary>
        Task<ResultadoBusquedaDto> BuscarProductosAsync(BusquedaProductoDto busqueda);

        /// <summary>
        /// Obtiene los detalles de un producto específico con todas sus ofertas
        /// </summary>
        Task<ProductoDetalleDto?> ObtenerProductoDetalleAsync(int productoId);

        /// <summary>
        /// Obtiene las mejores ofertas del día
        /// </summary>
        Task<IEnumerable<OfertaDestacadaDto>> ObtenerOfertasDestacadasAsync(int cantidad = 10);

        /// <summary>
        /// Obtiene productos filtrados por categoría
        /// </summary>
        Task<IEnumerable<ProductoDto>> ObtenerProductosPorCategoriaAsync(string categoria);

        /// <summary>
        /// Obtiene todas las categorías disponibles
        /// </summary>
        Task<IEnumerable<string>> ObtenerCategoriasAsync();

        /// <summary>
        /// Obtiene productos compatibles con un vehículo específico
        /// </summary>
        Task<IEnumerable<ProductoDto>> ObtenerProductosCompatiblesAsync(int marcaId, int modeloId);

        /// <summary>
        /// Actualiza el precio de una oferta específica
        /// </summary>
        Task<bool> ActualizarPrecioOfertaAsync(int ofertaId, decimal nuevoPrecio);

        /// <summary>
        /// Obtiene ofertas en tiempo real ejecutando scrapers
        /// </summary>
        Task<IEnumerable<OfertaDto>> ObtenerOfertasEnTiempoRealAsync(string numeroDeParte, CancellationToken cancellationToken = default);

        /// <summary>
        /// Limpia el caché de ofertas para un producto específico
        /// </summary>
        Task<bool> LimpiarCacheOfertasAsync(string numeroDeParte);
    }

    /// <summary>
    /// Servicio para la gestión de productos
    /// </summary>
    public interface IProductoService
    {
        /// <summary>
        /// Obtiene todos los productos
        /// </summary>
        Task<IEnumerable<ProductoDto>> ObtenerProductosAsync();

        /// <summary>
        /// Obtiene un producto por su ID
        /// </summary>
        Task<ProductoDto?> ObtenerProductoPorIdAsync(int id);

        /// <summary>
        /// Crea un nuevo producto
        /// </summary>
        Task<int> CrearProductoAsync(CrearProductoDto producto);

        /// <summary>
        /// Actualiza un producto existente
        /// </summary>
        Task<bool> ActualizarProductoAsync(int id, ActualizarProductoDto producto);

        /// <summary>
        /// Elimina un producto
        /// </summary>
        Task<bool> EliminarProductoAsync(int id);

        /// <summary>
        /// Obtiene las marcas de productos disponibles
        /// </summary>
        Task<IEnumerable<string>> ObtenerMarcasProductosAsync();
    }

    /// <summary>
    /// Servicio para la gestión de tiendas
    /// </summary>
    public interface ITiendaService
    {
        /// <summary>
        /// Obtiene todas las tiendas
        /// </summary>
        Task<IEnumerable<TiendaDto>> ObtenerTiendasAsync();

        /// <summary>
        /// Obtiene una tienda por su ID
        /// </summary>
        Task<TiendaDto?> ObtenerTiendaPorIdAsync(int id);

        /// <summary>
        /// Crea una nueva tienda
        /// </summary>
        Task<int> CrearTiendaAsync(CrearTiendaDto tienda);

        /// <summary>
        /// Actualiza una tienda existente
        /// </summary>
        Task<bool> ActualizarTiendaAsync(int id, ActualizarTiendaDto tienda);

        /// <summary>
        /// Elimina una tienda
        /// </summary>
        Task<bool> EliminarTiendaAsync(int id);

        /// <summary>
        /// Obtiene ofertas de una tienda específica
        /// </summary>
        Task<IEnumerable<OfertaDto>> ObtenerOfertasTiendaAsync(int tiendaId);
    }

    /// <summary>
    /// Servicio para integrar scrapers con la aplicación web
    /// </summary>
    public interface IScraperIntegrationService
    {
        /// <summary>
        /// Obtiene ofertas en tiempo real ejecutando scrapers
        /// </summary>
        Task<List<OfertaDto>> ObtenerOfertasEnTiempoRealAsync(string numeroDeParte, CancellationToken cancellationToken = default);

        /// <summary>
        /// Limpia el caché de ofertas para un producto específico
        /// </summary>
        Task<bool> LimpiarCacheAsync(string numeroDeParte);

        /// <summary>
        /// Ejecuta el scraping para un producto y actualiza/crea ofertas en la base de datos.
        /// Traduce entre Scraper.DTOs.OfertaDto y Core.Entities.Oferta.
        /// </summary>
        /// <param name="productoId">ID del producto a scrapear</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Número de ofertas procesadas (creadas + actualizadas)</returns>
        Task<int> EjecutarYActualizarPrecios(int productoId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Busca y descubre productos nuevos ejecutando scraping en todas las tiendas.
        /// Agrupa ofertas por producto y devuelve resultados para mostrar en UI.
        /// </summary>
        /// <param name="query">Criterios de búsqueda</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de productos con sus ofertas</returns>
        Task<List<ProductoConOfertasDto>> BuscarYDescubrirRepuestos(BusquedaRepuestoQuery query, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Servicio para la gestión de vehículos (marcas y modelos)
    /// </summary>
    public interface IVehiculoService
    {
        /// <summary>
        /// Obtiene todas las marcas de vehículos
        /// </summary>
        Task<IEnumerable<MarcaDto>> ObtenerMarcasAsync();

        /// <summary>
        /// Obtiene modelos por marca
        /// </summary>
        Task<IEnumerable<ModeloDto>> ObtenerModelosPorMarcaAsync(int marcaId);

        /// <summary>
        /// Crea una nueva marca
        /// </summary>
        Task<int> CrearMarcaAsync(CrearMarcaDto marca);

        /// <summary>
        /// Crea un nuevo modelo
        /// </summary>
        Task<int> CrearModeloAsync(CrearModeloDto modelo);
    }
}