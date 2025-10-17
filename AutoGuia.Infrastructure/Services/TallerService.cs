using Microsoft.EntityFrameworkCore;
using AutoGuia.Core.DTOs;
using AutoGuia.Core.Entities;
using AutoGuia.Infrastructure.Data;

namespace AutoGuia.Infrastructure.Services
{
    public class TallerService : ITallerService
    {
        private readonly AutoGuiaDbContext _context;

        public TallerService(AutoGuiaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TallerDto>> ObtenerTalleresAsync()
        {
            return await _context.Talleres
                .Where(t => t.EsActivo)
                .Select(t => new TallerDto
                {
                    Id = t.Id,
                    Nombre = t.Nombre,
                    Descripcion = t.Descripcion,
                    Direccion = t.Direccion,
                    Ciudad = t.Ciudad,
                    Region = t.Region,
                    Telefono = t.Telefono,
                    Email = t.Email,
                    Latitud = t.Latitud,
                    Longitud = t.Longitud,
                    HorarioAtencion = t.HorarioAtencion,
                    CalificacionPromedio = t.CalificacionPromedio,
                    TotalResenas = t.TotalResenas,
                    Especialidades = t.Especialidades,
                    EsVerificado = t.EsVerificado
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<TallerDto>> BuscarTalleresPorCiudadAsync(string ciudad)
        {
            return await _context.Talleres
                .Where(t => t.EsActivo && t.Ciudad.Contains(ciudad))
                .Select(t => new TallerDto
                {
                    Id = t.Id,
                    Nombre = t.Nombre,
                    Descripcion = t.Descripcion,
                    Direccion = t.Direccion,
                    Ciudad = t.Ciudad,
                    Region = t.Region,
                    Telefono = t.Telefono,
                    Email = t.Email,
                    Latitud = t.Latitud,
                    Longitud = t.Longitud,
                    HorarioAtencion = t.HorarioAtencion,
                    CalificacionPromedio = t.CalificacionPromedio,
                    TotalResenas = t.TotalResenas,
                    Especialidades = t.Especialidades,
                    EsVerificado = t.EsVerificado
                })
                .ToListAsync();
        }

        public async Task<TallerDto?> ObtenerTallerPorIdAsync(int id)
        {
            var taller = await _context.Talleres
                .Where(t => t.Id == id && t.EsActivo)
                .FirstOrDefaultAsync();

            if (taller == null) return null;

            return new TallerDto
            {
                Id = taller.Id,
                Nombre = taller.Nombre,
                Descripcion = taller.Descripcion,
                Direccion = taller.Direccion,
                Ciudad = taller.Ciudad,
                Region = taller.Region,
                Telefono = taller.Telefono,
                Email = taller.Email,
                Latitud = taller.Latitud,
                Longitud = taller.Longitud,
                HorarioAtencion = taller.HorarioAtencion,
                CalificacionPromedio = taller.CalificacionPromedio,
                TotalResenas = taller.TotalResenas,
                Especialidades = taller.Especialidades,
                EsVerificado = taller.EsVerificado
            };
        }

        /// <summary>
        /// Crea un nuevo taller en la base de datos
        /// </summary>
        /// <param name="tallerDto">Datos del taller a crear</param>
        /// <returns>ID del taller creado</returns>
        public async Task<int> CrearTallerAsync(CrearTallerDto tallerDto)
        {
            var taller = new Taller
            {
                Nombre = tallerDto.Nombre,
                Descripcion = tallerDto.Descripcion,
                Direccion = tallerDto.Direccion,
                Ciudad = tallerDto.Ciudad,
                Region = tallerDto.Region,
                Telefono = tallerDto.Telefono,
                Email = tallerDto.Email,
                Latitud = tallerDto.Latitud,
                Longitud = tallerDto.Longitud,
                HorarioAtencion = tallerDto.HorarioAtencion,
                Especialidades = tallerDto.Especialidades,
                EsVerificado = tallerDto.EsVerificado,
                EsActivo = true,
                FechaRegistro = DateTime.UtcNow
            };

            _context.Talleres.Add(taller);
            await _context.SaveChangesAsync();
            
            return taller.Id;
        }

        /// <summary>
        /// Actualiza un taller existente
        /// </summary>
        /// <param name="id">ID del taller a actualizar</param>
        /// <param name="tallerDto">Datos actualizados del taller</param>
        /// <returns>True si se actualizó correctamente, False si no se encontró el taller</returns>
        public async Task<bool> ActualizarTallerAsync(int id, ActualizarTallerDto tallerDto)
        {
            var taller = await _context.Talleres
                .FirstOrDefaultAsync(t => t.Id == id && t.EsActivo);

            if (taller == null)
                return false;

            taller.Nombre = tallerDto.Nombre;
            taller.Descripcion = tallerDto.Descripcion;
            taller.Direccion = tallerDto.Direccion;
            taller.Ciudad = tallerDto.Ciudad;
            taller.Region = tallerDto.Region;
            taller.Telefono = tallerDto.Telefono;
            taller.Email = tallerDto.Email;
            taller.Latitud = tallerDto.Latitud;
            taller.Longitud = tallerDto.Longitud;
            taller.HorarioAtencion = tallerDto.HorarioAtencion;
            taller.Especialidades = tallerDto.Especialidades;
            taller.EsVerificado = tallerDto.EsVerificado;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Elimina un taller de forma lógica (marca como inactivo)
        /// </summary>
        /// <param name="id">ID del taller a eliminar</param>
        /// <returns>True si se eliminó correctamente, False si no se encontró el taller</returns>
        public async Task<bool> EliminarTallerAsync(int id)
        {
            var taller = await _context.Talleres
                .FirstOrDefaultAsync(t => t.Id == id && t.EsActivo);

            if (taller == null)
                return false;

            taller.EsActivo = false;
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Cambia el estado de verificación de un taller
        /// </summary>
        /// <param name="id">ID del taller</param>
        /// <param name="esVerificado">Nuevo estado de verificación</param>
        /// <returns>True si se actualizó correctamente, False si no se encontró el taller</returns>
        public async Task<bool> CambiarEstadoVerificacionAsync(int id, bool esVerificado)
        {
            var taller = await _context.Talleres
                .FirstOrDefaultAsync(t => t.Id == id && t.EsActivo);

            if (taller == null)
                return false;

            taller.EsVerificado = esVerificado;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}