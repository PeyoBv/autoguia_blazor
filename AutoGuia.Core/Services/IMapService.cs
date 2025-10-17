using AutoGuia.Core.Entities;

namespace AutoGuia.Core.Services
{
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