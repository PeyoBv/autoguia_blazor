using AutoGuia.Core.DTOs;
using AutoGuia.Core.Entities;

namespace AutoGuia.Infrastructure.Services
{
    /// <summary>
    /// Servicio de diagnóstico automotriz usando IA de Gemini
    /// </summary>
    public interface IGeminiService
    {
        /// <summary>
        /// Obtiene un diagnóstico inteligente basado en la descripción de la falla del vehículo
        /// </summary>
        /// <param name="descripcionFalla">Descripción detallada del problema del vehículo</param>
        /// <returns>Diagnóstico con posibles causas, pasos de revisión y nivel de urgencia</returns>
        Task<string> ObtenerDiagnosticoAsync(string descripcionFalla);
    }

    /// <summary>
    /// Servicio para sanitizar contenido HTML y proteger contra ataques XSS
    /// </summary>
    public interface IHtmlSanitizationService
    {
        /// <summary>
        /// Sanitiza una cadena HTML, removiendo scripts y contenido potencialmente peligroso
        /// </summary>
        string Sanitize(string? unsafeHtml);
        
        /// <summary>
        /// Sanitiza contenido HTML preservando formato básico (negrita, cursiva, enlaces, listas)
        /// </summary>
        string SanitizeWithBasicFormatting(string? unsafeHtml);
    }
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

    // Interfaces de servicios de scraping eliminadas (IComparadorService, IProductoService, ITiendaService, IScraperIntegrationService)
    // Ya no usamos web scraping - ahora somos un centro de información vehicular basado en APIs

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

    // IVinDecoderService eliminada - reemplazada por IVehiculoInfoService en archivo separado

    /// <summary>
    /// Servicio para el sistema de comparación de precios de productos automotrices
    /// </summary>
    public interface IComparadorService
    {
        Task<ResultadoBusquedaDto> BuscarProductosAsync(BusquedaProductoDto busqueda);
        Task<ProductoDetalleDto?> ObtenerProductoDetalleAsync(int productoId);
        Task<IEnumerable<OfertaDestacadaDto>> ObtenerOfertasDestacadasAsync(int cantidad = 10);
        Task<IEnumerable<ProductoDto>> ObtenerProductosPorCategoriaAsync(string categoria);
        Task<IEnumerable<string>> ObtenerCategoriasAsync();
        Task<IEnumerable<ProductoDto>> ObtenerProductosCompatiblesAsync(int marcaId, int modeloId);
        Task<bool> ActualizarPrecioOfertaAsync(int ofertaId, decimal nuevoPrecio);
        
        /// <summary>
        /// Busca consumibles automotrices en tiempo real usando web scraping
        /// </summary>
        /// <param name="termino">Término de búsqueda (ej: "Aceite 10W-40 Castrol")</param>
        /// <param name="categoria">Categoría del consumible (opcional)</param>
        /// <returns>Lista de productos con ofertas encontradas en diferentes tiendas</returns>
        Task<IEnumerable<ProductoConOfertasDto>> BuscarConsumiblesAsync(string termino, string? categoria = null);
    }

    /// <summary>
    /// Servicio para la gestión de productos automotrices
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
        Task<int> CrearProductoAsync(CrearProductoDto productoDto);

        /// <summary>
        /// Actualiza un producto existente
        /// </summary>
        Task<bool> ActualizarProductoAsync(int id, ActualizarProductoDto productoDto);

        /// <summary>
        /// Elimina un producto
        /// </summary>
        Task<bool> EliminarProductoAsync(int id);

        /// <summary>
        /// Obtiene las marcas de productos disponibles
        /// </summary>
        Task<IEnumerable<string>> ObtenerMarcasProductosAsync();

        /// <summary>
        /// Busca productos aplicando filtros dinámicos basados en categoría y valores de filtro
        /// </summary>
        /// <param name="categoria">Nombre de la categoría (ej: "Aceites", "Neumáticos")</param>
        /// <param name="filtros">Diccionario de filtros donde la clave es el nombre del filtro y el valor es el valor a buscar (ej: {"Viscosidad": "10W-40", "Marca": "Castrol"})</param>
        /// <returns>Colección de productos que coinciden con los filtros aplicados, ordenados por precio ascendente</returns>
        Task<IEnumerable<ProductoDto>> BuscarPorFiltrosAsync(string categoria, Dictionary<string, string> filtros);
    }

    /// <summary>
    /// Servicio para la gestión de categorías, subcategorías y valores de filtro de consumibles automotrices
    /// </summary>
    public interface ICategoriaService
    {
        /// <summary>
        /// Obtiene todas las categorías activas con sus subcategorías y valores de filtro
        /// </summary>
        /// <returns>Colección de categorías activas con su jerarquía completa</returns>
        Task<IEnumerable<CategoriaDto>> ObtenerCategoriasAsync();

        /// <summary>
        /// Obtiene las subcategorías asociadas a una categoría específica
        /// </summary>
        /// <param name="categoriaId">Identificador único de la categoría</param>
        /// <returns>Colección de subcategorías con sus valores de filtro</returns>
        Task<IEnumerable<SubcategoriaDto>> ObtenerSubcategoriasAsync(int categoriaId);

        /// <summary>
        /// Obtiene los valores de filtro disponibles para una subcategoría específica
        /// </summary>
        /// <param name="subcategoriaId">Identificador único de la subcategoría</param>
        /// <returns>Colección de valores de filtro disponibles</returns>
        Task<IEnumerable<ValorFiltroDto>> ObtenerValoresFiltroAsync(int subcategoriaId);

        /// <summary>
        /// Obtiene una categoría específica por su identificador, incluyendo subcategorías y valores
        /// </summary>
        /// <param name="id">Identificador único de la categoría</param>
        /// <returns>Categoría con su jerarquía completa, o null si no existe</returns>
        Task<CategoriaDto?> ObtenerCategoriaPorIdAsync(int id);

        /// <summary>
        /// Crea una nueva categoría de consumibles automotrices
        /// </summary>
        /// <param name="categoria">Datos de la categoría a crear</param>
        /// <returns>Identificador de la categoría creada</returns>
        Task<int> CrearCategoriaAsync(CrearCategoriaDto categoria);
    }
}