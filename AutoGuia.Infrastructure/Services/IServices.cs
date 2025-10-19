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
}