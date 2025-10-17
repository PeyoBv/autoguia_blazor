using Microsoft.EntityFrameworkCore;
using AutoGuia.Core.DTOs;
using AutoGuia.Core.Entities;
using AutoGuia.Infrastructure.Data;

namespace AutoGuia.Infrastructure.Services
{
    /// <summary>
    /// Servicio para la gestión de vehículos (marcas y modelos)
    /// </summary>
    public class VehiculoService : IVehiculoService
    {
        private readonly AutoGuiaDbContext _context;

        public VehiculoService(AutoGuiaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MarcaDto>> ObtenerMarcasAsync()
        {
            var marcas = await _context.Marcas
                .Include(m => m.Modelos)
                .Select(m => new MarcaDto
                {
                    Id = m.Id,
                    Nombre = m.Nombre,
                    LogoUrl = m.LogoUrl,
                    TotalModelos = m.Modelos.Count()
                })
                .OrderBy(m => m.Nombre)
                .ToListAsync();

            return marcas;
        }

        public async Task<IEnumerable<ModeloDto>> ObtenerModelosPorMarcaAsync(int marcaId)
        {
            var modelos = await _context.Modelos
                .Where(m => m.MarcaId == marcaId)
                .Include(m => m.Marca)
                .Select(m => new ModeloDto
                {
                    Id = m.Id,
                    Nombre = m.Nombre,
                    MarcaId = m.MarcaId,
                    MarcaNombre = m.Marca.Nombre,
                    AnioInicioProduccion = m.AnioInicioProduccion ?? 0,
                    AnioFinProduccion = m.AnioFinProduccion ?? 0
                })
                .OrderBy(m => m.Nombre)
                .ToListAsync();

            return modelos;
        }

        public async Task<int> CrearMarcaAsync(CrearMarcaDto marcaDto)
        {
            var marca = new Marca
            {
                Nombre = marcaDto.Nombre,
                LogoUrl = marcaDto.LogoUrl
            };

            _context.Marcas.Add(marca);
            await _context.SaveChangesAsync();
            return marca.Id;
        }

        public async Task<int> CrearModeloAsync(CrearModeloDto modeloDto)
        {
            var modelo = new Modelo
            {
                Nombre = modeloDto.Nombre,
                MarcaId = modeloDto.MarcaId,
                AnioInicioProduccion = modeloDto.AnoInicio,
                AnioFinProduccion = modeloDto.AnoFin
            };

            _context.Modelos.Add(modelo);
            await _context.SaveChangesAsync();
            return modelo.Id;
        }
    }
}