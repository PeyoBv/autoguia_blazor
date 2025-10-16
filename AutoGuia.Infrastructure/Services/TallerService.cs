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
    }
}