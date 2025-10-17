using Microsoft.EntityFrameworkCore;
using AutoGuia.Core.DTOs;
using AutoGuia.Core.Entities;
using AutoGuia.Infrastructure.Data;

namespace AutoGuia.Infrastructure.Services
{
    /// <summary>
    /// Servicio para gestionar las reseñas de talleres
    /// </summary>
    public class ResenaService : IResenaService
    {
        private readonly AutoGuiaDbContext _context;

        public ResenaService(AutoGuiaDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene todas las reseñas de un taller específico
        /// </summary>
        public async Task<IEnumerable<ResenaDto>> ObtenerResenasPorTallerAsync(int tallerId)
        {
            return await _context.Resenas
                .Where(r => r.TallerId == tallerId)
                .OrderByDescending(r => r.FechaPublicacion)
                .Select(r => new ResenaDto
                {
                    Id = r.Id,
                    Calificacion = r.Calificacion,
                    Comentario = r.Comentario,
                    FechaPublicacion = r.FechaPublicacion,
                    TallerId = r.TallerId,
                    UsuarioId = r.UsuarioId,
                    NombreUsuario = r.NombreUsuario
                })
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene estadísticas de reseñas para un taller
        /// </summary>
        public async Task<EstadisticasResenaDto> ObtenerEstadisticasTallerAsync(int tallerId)
        {
            var resenas = await _context.Resenas
                .Where(r => r.TallerId == tallerId)
                .ToListAsync();

            if (!resenas.Any())
            {
                return new EstadisticasResenaDto
                {
                    CalificacionPromedio = 0,
                    TotalResenas = 0,
                    DistribucionCalificaciones = new Dictionary<int, int>
                    {
                        {1, 0}, {2, 0}, {3, 0}, {4, 0}, {5, 0}
                    }
                };
            }

            var distribucion = new Dictionary<int, int>();
            for (int i = 1; i <= 5; i++)
            {
                distribucion[i] = resenas.Count(r => r.Calificacion == i);
            }

            return new EstadisticasResenaDto
            {
                CalificacionPromedio = (decimal)resenas.Average(r => r.Calificacion),
                TotalResenas = resenas.Count,
                DistribucionCalificaciones = distribucion
            };
        }

        /// <summary>
        /// Crea una nueva reseña
        /// </summary>
        public async Task<int> CrearResenaAsync(CrearResenaDto resena, string usuarioId, string nombreUsuario)
        {
            // Verificar si el usuario ya reseñó este taller
            var reseñaExistente = await _context.Resenas
                .AnyAsync(r => r.TallerId == resena.TallerId && r.UsuarioId == usuarioId);

            if (reseñaExistente)
            {
                throw new InvalidOperationException("Ya has reseñado este taller anteriormente.");
            }

            var nuevaResena = new Resena
            {
                Calificacion = resena.Calificacion,
                Comentario = resena.Comentario,
                TallerId = resena.TallerId,
                UsuarioId = usuarioId,
                NombreUsuario = nombreUsuario,
                FechaPublicacion = DateTime.UtcNow
            };

            _context.Resenas.Add(nuevaResena);
            await _context.SaveChangesAsync();

            // Actualizar estadísticas del taller
            await ActualizarEstadisticasTallerAsync(resena.TallerId);

            return nuevaResena.Id;
        }

        /// <summary>
        /// Verifica si un usuario ya reseñó un taller
        /// </summary>
        public async Task<bool> UsuarioYaResenoTallerAsync(int tallerId, string usuarioId)
        {
            return await _context.Resenas
                .AnyAsync(r => r.TallerId == tallerId && r.UsuarioId == usuarioId);
        }

        /// <summary>
        /// Elimina una reseña (solo el autor puede eliminar su propia reseña)
        /// </summary>
        public async Task<bool> EliminarResenaAsync(int resenaId, string usuarioId)
        {
            var resena = await _context.Resenas
                .FirstOrDefaultAsync(r => r.Id == resenaId && r.UsuarioId == usuarioId);

            if (resena == null)
                return false;

            var tallerId = resena.TallerId;
            _context.Resenas.Remove(resena);
            await _context.SaveChangesAsync();

            // Actualizar estadísticas del taller
            await ActualizarEstadisticasTallerAsync(tallerId);

            return true;
        }

        /// <summary>
        /// Obtiene una reseña por su ID
        /// </summary>
        public async Task<ResenaDto?> ObtenerResenaPorIdAsync(int resenaId)
        {
            return await _context.Resenas
                .Where(r => r.Id == resenaId)
                .Select(r => new ResenaDto
                {
                    Id = r.Id,
                    Calificacion = r.Calificacion,
                    Comentario = r.Comentario,
                    FechaPublicacion = r.FechaPublicacion,
                    TallerId = r.TallerId,
                    UsuarioId = r.UsuarioId,
                    NombreUsuario = r.NombreUsuario
                })
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Actualiza las estadísticas de calificación de un taller
        /// </summary>
        private async Task ActualizarEstadisticasTallerAsync(int tallerId)
        {
            var taller = await _context.Talleres.FindAsync(tallerId);
            if (taller == null) return;

            var estadisticas = await ObtenerEstadisticasTallerAsync(tallerId);
            
            taller.CalificacionPromedio = estadisticas.CalificacionPromedio;
            taller.TotalResenas = estadisticas.TotalResenas;

            await _context.SaveChangesAsync();
        }
    }
}