using Microsoft.EntityFrameworkCore;
using AutoGuia.Core.DTOs;
using AutoGuia.Core.Entities;
using AutoGuia.Infrastructure.Data;

namespace AutoGuia.Infrastructure.Services
{
    /// <summary>
    /// Implementación del servicio de repuestos
    /// </summary>
    public class RepuestoService : IRepuestoService
    {
        private readonly AutoGuiaDbContext _context;

        public RepuestoService(AutoGuiaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoriaRepuestoDto>> ObtenerCategoriasAsync()
        {
            return await _context.CategoriasRepuesto
                .Where(c => c.EsActivo)
                .Select(c => new CategoriaRepuestoDto
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Descripcion = c.Descripcion,
                    TotalRepuestos = c.Repuestos.Count(r => r.EsActivo)
                })
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }

        public async Task<IEnumerable<RepuestoDto>> ObtenerRepuestosAsync(FiltroRepuestosDto? filtros = null)
        {
            var query = _context.Repuestos
                .Where(r => r.EsActivo)
                .Include(r => r.CategoriaRepuesto)
                .AsQueryable();

            if (filtros != null)
            {
                // Filtro por término de búsqueda
                if (!string.IsNullOrEmpty(filtros.TerminoBusqueda))
                {
                    var termino = filtros.TerminoBusqueda.ToLower();
                    query = query.Where(r => 
                        r.Nombre.ToLower().Contains(termino) ||
                        r.Descripcion!.ToLower().Contains(termino) ||
                        r.NumeroDeParte!.ToLower().Contains(termino) ||
                        r.Marca!.ToLower().Contains(termino) ||
                        r.CategoriaRepuesto.Nombre.ToLower().Contains(termino));
                }

                // Filtro por categoría
                if (filtros.CategoriaId.HasValue && filtros.CategoriaId.Value > 0)
                {
                    query = query.Where(r => r.CategoriaRepuestoId == filtros.CategoriaId.Value);
                }

                // Filtro por marca
                if (!string.IsNullOrEmpty(filtros.Marca))
                {
                    query = query.Where(r => r.Marca!.ToLower().Contains(filtros.Marca.ToLower()));
                }

                // Filtro por rango de precios
                if (filtros.PrecioMinimo.HasValue)
                {
                    query = query.Where(r => r.PrecioEstimado >= filtros.PrecioMinimo.Value);
                }

                if (filtros.PrecioMaximo.HasValue)
                {
                    query = query.Where(r => r.PrecioEstimado <= filtros.PrecioMaximo.Value);
                }

                // Filtro solo disponibles
                if (filtros.SoloDisponibles == true)
                {
                    query = query.Where(r => r.EsDisponible);
                }
            }

            // Paginación
            var skip = (filtros?.Pagina ?? 1 - 1) * (filtros?.TamanoPagina ?? 20);
            var take = filtros?.TamanoPagina ?? 20;

            return await query
                .OrderBy(r => r.CategoriaRepuesto.Nombre)
                .ThenBy(r => r.Nombre)
                .Skip(skip)
                .Take(take)
                .Select(r => new RepuestoDto
                {
                    Id = r.Id,
                    Nombre = r.Nombre,
                    Descripcion = r.Descripcion,
                    NumeroDeParte = r.NumeroDeParte,
                    PrecioEstimado = r.PrecioEstimado,
                    Marca = r.Marca,
                    Modelo = r.Modelo,
                    Anio = r.Anio,
                    ImagenUrl = r.ImagenUrl,
                    EsDisponible = r.EsDisponible,
                    FechaCreacion = r.FechaCreacion,
                    CategoriaRepuestoId = r.CategoriaRepuestoId,
                    NombreCategoria = r.CategoriaRepuesto.Nombre
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<RepuestoDto>> BuscarRepuestosAsync(string terminoBusqueda)
        {
            if (string.IsNullOrEmpty(terminoBusqueda))
                return Enumerable.Empty<RepuestoDto>();

            var filtro = new FiltroRepuestosDto
            {
                TerminoBusqueda = terminoBusqueda,
                SoloDisponibles = true,
                TamanoPagina = 50
            };

            return await ObtenerRepuestosAsync(filtro);
        }

        public async Task<IEnumerable<RepuestoDto>> ObtenerRepuestosPorCategoriaAsync(int categoriaId)
        {
            var filtro = new FiltroRepuestosDto
            {
                CategoriaId = categoriaId,
                SoloDisponibles = true,
                TamanoPagina = 100
            };

            return await ObtenerRepuestosAsync(filtro);
        }

        public async Task<RepuestoDto?> ObtenerRepuestoPorIdAsync(int id)
        {
            return await _context.Repuestos
                .Where(r => r.Id == id && r.EsActivo)
                .Include(r => r.CategoriaRepuesto)
                .Select(r => new RepuestoDto
                {
                    Id = r.Id,
                    Nombre = r.Nombre,
                    Descripcion = r.Descripcion,
                    NumeroDeParte = r.NumeroDeParte,
                    PrecioEstimado = r.PrecioEstimado,
                    Marca = r.Marca,
                    Modelo = r.Modelo,
                    Anio = r.Anio,
                    ImagenUrl = r.ImagenUrl,
                    EsDisponible = r.EsDisponible,
                    FechaCreacion = r.FechaCreacion,
                    CategoriaRepuestoId = r.CategoriaRepuestoId,
                    NombreCategoria = r.CategoriaRepuesto.Nombre
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<string>> ObtenerMarcasAsync()
        {
            return await _context.Repuestos
                .Where(r => r.EsActivo && !string.IsNullOrEmpty(r.Marca))
                .Select(r => r.Marca!)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();
        }

        public async Task<int> CrearRepuestoAsync(CrearRepuestoDto repuesto)
        {
            var nuevoRepuesto = new Repuesto
            {
                Nombre = repuesto.Nombre,
                Descripcion = repuesto.Descripcion,
                NumeroDeParte = repuesto.NumeroDeParte,
                PrecioEstimado = repuesto.PrecioEstimado,
                Marca = repuesto.Marca,
                Modelo = repuesto.Modelo,
                Anio = repuesto.Anio,
                ImagenUrl = repuesto.ImagenUrl,
                CategoriaRepuestoId = repuesto.CategoriaRepuestoId,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Repuestos.Add(nuevoRepuesto);
            await _context.SaveChangesAsync();

            return nuevoRepuesto.Id;
        }
    }
}