using AutoGuia.Core.DTOs;
using AutoGuia.Core.Entities;

namespace AutoGuia.Infrastructure.Services
{
    public interface ITallerService
    {
        Task<IEnumerable<TallerDto>> ObtenerTalleresAsync();
        Task<IEnumerable<TallerDto>> BuscarTalleresPorCiudadAsync(string ciudad);
        Task<TallerDto?> ObtenerTallerPorIdAsync(int id);
    }

    public interface IForoService
    {
        Task<IEnumerable<PublicacionForoDto>> ObtenerPublicacionesAsync(int pagina = 1, int tamanoPagina = 10);
        Task<PublicacionForoDto?> ObtenerPublicacionPorIdAsync(int id);
        Task<IEnumerable<RespuestaForoDto>> ObtenerRespuestasAsync(int publicacionId);
        Task<int> CrearPublicacionAsync(CrearPublicacionDto publicacion, int usuarioId);
        Task<int> CrearRespuestaAsync(CrearRespuestaDto respuesta, int usuarioId);
    }

    public interface IResenaService
    {
        Task<IEnumerable<ResenaDto>> ObtenerResenasPorTallerAsync(int tallerId);
        Task<EstadisticasResenaDto> ObtenerEstadisticasTallerAsync(int tallerId);
        Task<int> CrearResenaAsync(CrearResenaDto resena, string usuarioId, string nombreUsuario);
        Task<bool> UsuarioYaResenoTallerAsync(int tallerId, string usuarioId);
        Task<bool> EliminarResenaAsync(int resenaId, string usuarioId);
        Task<ResenaDto?> ObtenerResenaPorIdAsync(int resenaId);
    }

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
}