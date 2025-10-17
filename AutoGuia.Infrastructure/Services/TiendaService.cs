using Microsoft.EntityFrameworkCore;
using AutoGuia.Core.DTOs;
using AutoGuia.Core.Entities;
using AutoGuia.Infrastructure.Data;

namespace AutoGuia.Infrastructure.Services
{
    /// <summary>
    /// Servicio para la gestión de tiendas
    /// </summary>
    public class TiendaService : ITiendaService
    {
        private readonly AutoGuiaDbContext _context;

        public TiendaService(AutoGuiaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TiendaDto>> ObtenerTiendasAsync()
        {
            var tiendas = await _context.Tiendas
                .Include(t => t.Ofertas)
                .Select(t => new TiendaDto
                {
                    Id = t.Id,
                    Nombre = t.Nombre,
                    Descripcion = t.Descripcion,
                    UrlSitioWeb = t.UrlSitioWeb,
                    LogoUrl = t.LogoUrl,
                    FechaCreacion = t.FechaCreacion
                })
                .OrderBy(t => t.Nombre)
                .ToListAsync();

            return tiendas;
        }

        public async Task<TiendaDto?> ObtenerTiendaPorIdAsync(int id)
        {
            var tienda = await _context.Tiendas
                .Include(t => t.Ofertas)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tienda == null)
                return null;

            return new TiendaDto
            {
                Id = tienda.Id,
                Nombre = tienda.Nombre,
                Descripcion = tienda.Descripcion,
                UrlSitioWeb = tienda.UrlSitioWeb,
                LogoUrl = tienda.LogoUrl,
                FechaCreacion = tienda.FechaCreacion
            };
        }

        public async Task<int> CrearTiendaAsync(CrearTiendaDto tiendaDto)
        {
            var tienda = new Tienda
            {
                Nombre = tiendaDto.Nombre,
                Descripcion = tiendaDto.Descripcion,
                UrlSitioWeb = tiendaDto.SitioWeb ?? string.Empty,
                LogoUrl = tiendaDto.LogoUrl
            };

            _context.Tiendas.Add(tienda);
            await _context.SaveChangesAsync();
            return tienda.Id;
        }

        public async Task<bool> ActualizarTiendaAsync(int id, ActualizarTiendaDto tiendaDto)
        {
            var tienda = await _context.Tiendas.FindAsync(id);
            if (tienda == null)
                return false;

            tienda.Nombre = tiendaDto.Nombre;
            tienda.Descripcion = tiendaDto.Descripcion;
            tienda.UrlSitioWeb = tiendaDto.SitioWeb ?? string.Empty;
            tienda.LogoUrl = tiendaDto.LogoUrl;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarTiendaAsync(int id)
        {
            var tienda = await _context.Tiendas.FindAsync(id);
            if (tienda == null)
                return false;

            _context.Tiendas.Remove(tienda);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<OfertaDto>> ObtenerOfertasTiendaAsync(int tiendaId)
        {
            var ofertas = await _context.Ofertas
                .Include(o => o.Producto)
                .Include(o => o.Tienda)
                .Where(o => o.TiendaId == tiendaId)
                .Select(o => new OfertaDto
                {
                    Id = o.Id,
                    ProductoId = o.ProductoId,
                    ProductoNombre = o.Producto.Nombre,
                    ProductoImagen = o.Producto.ImagenUrl,
                    TiendaId = o.TiendaId,
                    TiendaNombre = o.Tienda.Nombre,
                    TiendaLogo = o.Tienda.LogoUrl,
                    Precio = o.Precio,
                    UrlProductoEnTienda = o.UrlProductoEnTienda,
                    EsDisponible = o.EsDisponible,
                    FechaActualizacion = o.FechaActualizacion
                })
                .OrderBy(o => o.ProductoNombre)
                .ToListAsync();

            return ofertas;
        }
    }
}