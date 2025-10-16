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
}