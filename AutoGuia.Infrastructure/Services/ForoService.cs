using Microsoft.EntityFrameworkCore;
using AutoGuia.Core.DTOs;
using AutoGuia.Core.Entities;
using AutoGuia.Infrastructure.Data;

namespace AutoGuia.Infrastructure.Services
{
    public class ForoService : IForoService
    {
        private readonly AutoGuiaDbContext _context;

        public ForoService(AutoGuiaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PublicacionForoDto>> ObtenerPublicacionesAsync(int pagina = 1, int tamanoPagina = 10)
        {
            return await _context.PublicacionesForo
                .Where(p => p.EsActivo)
                .Include(p => p.Usuario)
                .Include(p => p.Respuestas)
                .OrderByDescending(p => p.FechaCreacion)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .Select(p => new PublicacionForoDto
                {
                    Id = p.Id,
                    Titulo = p.Titulo,
                    Contenido = p.Contenido,
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
            var publicacion = await _context.PublicacionesForo
                .Where(p => p.Id == id && p.EsActivo)
                .Include(p => p.Usuario)
                .Include(p => p.Respuestas)
                .FirstOrDefaultAsync();

            if (publicacion == null) return null;

            // Incrementar vistas
            publicacion.Vistas++;
            await _context.SaveChangesAsync();

            return new PublicacionForoDto
            {
                Id = publicacion.Id,
                Titulo = publicacion.Titulo,
                Contenido = publicacion.Contenido,
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
                    Contenido = r.Contenido,
                    FechaCreacion = r.FechaCreacion,
                    NombreUsuario = $"{r.Usuario.Nombre} {r.Usuario.Apellido}",
                    Likes = r.Likes,
                    EsRespuestaAceptada = r.EsRespuestaAceptada
                })
                .ToListAsync();
        }

        public async Task<int> CrearPublicacionAsync(CrearPublicacionDto publicacion, int usuarioId)
        {
            var nuevaPublicacion = new PublicacionForo
            {
                Titulo = publicacion.Titulo,
                Contenido = publicacion.Contenido,
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
            var nuevaRespuesta = new RespuestaForo
            {
                Contenido = respuesta.Contenido,
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