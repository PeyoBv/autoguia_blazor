using Microsoft.EntityFrameworkCore;
using AutoGuia.Core.DTOs;
using AutoGuia.Core.Entities;
using AutoGuia.Infrastructure.Data;

namespace AutoGuia.Infrastructure.Services
{
    /// <summary>
    /// Servicio para la gestión de productos
    /// </summary>
    public class ProductoService : IProductoService
    {
        private readonly AutoGuiaDbContext _context;

        public ProductoService(AutoGuiaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductoDto>> ObtenerProductosAsync()
        {
            var productos = await _context.Productos
                .Include(p => p.Ofertas)
                .Select(p => new ProductoDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    NumeroDeparte = p.NumeroDeParte,
                    ImagenUrl = p.ImagenUrl,
                    PrecioMinimo = p.Ofertas.Any() ? p.Ofertas.Min(o => o.Precio) : 0,
                    TotalOfertas = p.Ofertas.Count(),
                    FechaCreacion = p.FechaCreacion
                })
                .OrderBy(p => p.Nombre)
                .ToListAsync();

            return productos;
        }

        public async Task<ProductoDto?> ObtenerProductoPorIdAsync(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.Ofertas)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
                return null;

            return new ProductoDto
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                NumeroDeparte = producto.NumeroDeParte,
                ImagenUrl = producto.ImagenUrl,
                PrecioMinimo = producto.Ofertas.Any() ? producto.Ofertas.Min(o => o.Precio) : 0,
                TotalOfertas = producto.Ofertas.Count(),
                FechaCreacion = producto.FechaCreacion
            };
        }

        public async Task<int> CrearProductoAsync(CrearProductoDto productoDto)
        {
            var producto = new Producto
            {
                Nombre = productoDto.Nombre,
                Descripcion = productoDto.Descripcion,
                NumeroDeParte = productoDto.NumeroDeparte,
                ImagenUrl = productoDto.ImagenUrl
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
            return producto.Id;
        }

        public async Task<bool> ActualizarProductoAsync(int id, ActualizarProductoDto productoDto)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
                return false;

            producto.Nombre = productoDto.Nombre;
            producto.Descripcion = productoDto.Descripcion;
            producto.NumeroDeParte = productoDto.NumeroDeparte;
            producto.ImagenUrl = productoDto.ImagenUrl;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarProductoAsync(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
                return false;

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<string>> ObtenerMarcasProductosAsync()
        {
            // Por ahora devolvemos una lista vacía ya que los productos no tienen marcas directamente
            // Las marcas están relacionadas a través de ProductoVehiculoCompatible -> Modelo -> Marca
            return new List<string>();
        }
    }
}