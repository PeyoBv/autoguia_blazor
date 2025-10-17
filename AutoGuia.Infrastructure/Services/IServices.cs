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
}