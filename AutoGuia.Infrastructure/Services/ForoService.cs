using Microsoft.EntityFrameworkCore;
using AutoGuia.Core.DTOs;
using AutoGuia.Core.Entities;
using AutoGuia.Infrastructure.Data;

namespace AutoGuia.Infrastructure.Services
{
    public class ForoService : IForoService
    {
        private readonly AutoGuiaDbContext _context;
        private readonly IHtmlSanitizationService _sanitizationService;

        public ForoService(AutoGuiaDbContext context, IHtmlSanitizationService sanitizationService)
        {
            _context = context;
            _sanitizationService = sanitizationService;
        }

        public async Task<IEnumerable<PublicacionForoDto>> ObtenerPublicacionesAsync(int pagina = 1, int tamanoPagina = 10)
        {
            // ✅ OPTIMIZACIÓN N+1: Eager loading de Usuario y Respuestas con ThenInclude
            return await _context.PublicacionesForo
                .Where(p => p.EsActivo)
                .Include(p => p.Usuario)
                .Include(p => p.Respuestas.Where(r => r.EsActivo))
                    .ThenInclude(r => r.Usuario) // ⚡ Evita N+1 al cargar usuarios de respuestas
                .OrderByDescending(p => p.FechaCreacion)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .Select(p => new PublicacionForoDto
                {
                    Id = p.Id,
                    Titulo = p.Titulo,
                    Contenido = _sanitizationService.Sanitize(p.Contenido), // Sanitizar contenido
                    Categoria = p.Categoria,
                    Etiquetas = p.Etiquetas,
                    FechaCreacion = p.FechaCreacion,
                    NombreUsuario = $"{p.Usuario.Nombre} {p.Usuario.Apellido}",
                    Vistas = p.Vistas,
                    Likes = p.Likes,
                    TotalRespuestas = p.Respuestas.Count(r => r.EsActivo),
                    EsDestacado = p.EsDestacado,
                    EsCerrado = p.EsCerrado
                })
                .ToListAsync();
        }

        public async Task<PublicacionForoDto?> ObtenerPublicacionPorIdAsync(int id)
        {
            // ✅ OPTIMIZACIÓN N+1: Eager loading de Usuario y Respuestas con ThenInclude
            var publicacion = await _context.PublicacionesForo
                .Where(p => p.Id == id && p.EsActivo)
                .Include(p => p.Usuario)
                .Include(p => p.Respuestas.Where(r => r.EsActivo))
                    .ThenInclude(r => r.Usuario) // ⚡ Evita N+1 al cargar usuarios de respuestas
                .FirstOrDefaultAsync();

            if (publicacion == null) return null;

            // Incrementar vistas
            publicacion.Vistas++;
            await _context.SaveChangesAsync();

            return new PublicacionForoDto
            {
                Id = publicacion.Id,
                Titulo = publicacion.Titulo,
                Contenido = _sanitizationService.Sanitize(publicacion.Contenido), // Sanitizar contenido
                Categoria = publicacion.Categoria,
                Etiquetas = publicacion.Etiquetas,
                FechaCreacion = publicacion.FechaCreacion,
                NombreUsuario = $"{publicacion.Usuario.Nombre} {publicacion.Usuario.Apellido}",
                Vistas = publicacion.Vistas,
                Likes = publicacion.Likes,
                TotalRespuestas = publicacion.Respuestas.Count(r => r.EsActivo),
                EsDestacado = publicacion.EsDestacado,
                EsCerrado = publicacion.EsCerrado
            };
        }

        public async Task<IEnumerable<RespuestaForoDto>> ObtenerRespuestasAsync(int publicacionId)
        {
            return await _context.RespuestasForo
                .Where(r => r.PublicacionId == publicacionId && r.EsActivo)
                .Include(r => r.Usuario)
                .OrderBy(r => r.FechaCreacion)
                .Select(r => new RespuestaForoDto
                {
                    Id = r.Id,
                    Contenido = _sanitizationService.Sanitize(r.Contenido), // Sanitizar contenido
                    FechaCreacion = r.FechaCreacion,
                    NombreUsuario = $"{r.Usuario.Nombre} {r.Usuario.Apellido}",
                    Likes = r.Likes,
                    EsRespuestaAceptada = r.EsRespuestaAceptada
                })
                .ToListAsync();
        }

        public async Task<int> CrearPublicacionAsync(CrearPublicacionDto publicacion, int usuarioId)
        {
            // Sanitizar contenido antes de guardarlo en la base de datos
            var contenidoSanitizado = _sanitizationService.Sanitize(publicacion.Contenido);
            
            var nuevaPublicacion = new PublicacionForo
            {
                Titulo = publicacion.Titulo,
                Contenido = contenidoSanitizado,
                Categoria = publicacion.Categoria,
                Etiquetas = publicacion.Etiquetas,
                UsuarioId = usuarioId,
                FechaCreacion = DateTime.UtcNow
            };

            _context.PublicacionesForo.Add(nuevaPublicacion);
            await _context.SaveChangesAsync();

            return nuevaPublicacion.Id;
        }

        public async Task<int> CrearRespuestaAsync(CrearRespuestaDto respuesta, int usuarioId)
        {
            // Sanitizar contenido antes de guardarlo en la base de datos
            var contenidoSanitizado = _sanitizationService.Sanitize(respuesta.Contenido);
            
            var nuevaRespuesta = new RespuestaForo
            {
                Contenido = contenidoSanitizado,
                PublicacionId = respuesta.PublicacionId,
                RespuestaPadreId = respuesta.RespuestaPadreId,
                UsuarioId = usuarioId,
                FechaCreacion = DateTime.UtcNow
            };

            _context.RespuestasForo.Add(nuevaRespuesta);
            await _context.SaveChangesAsync();

            return nuevaRespuesta.Id;
        }
    }
}