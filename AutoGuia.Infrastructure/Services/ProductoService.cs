using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ProductoService> _logger;

        public ProductoService(AutoGuiaDbContext context, ILogger<ProductoService> logger)
        {
            _context = context;
            _logger = logger;
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

        /// <summary>
        /// Busca productos aplicando filtros dinámicos basados en categoría y valores de filtro
        /// </summary>
        /// <param name="categoria">Nombre de la categoría (ej: "Aceites", "Neumáticos")</param>
        /// <param name="filtros">Diccionario de filtros donde la clave es el nombre del filtro y el valor es el valor a buscar</param>
        /// <returns>Colección de productos que coinciden con los filtros aplicados, ordenados por precio ascendente</returns>
        public async Task<IEnumerable<ProductoDto>> BuscarPorFiltrosAsync(string categoria, Dictionary<string, string> filtros)
        {
            try
            {
                // Validar que la categoría no sea null o vacía
                if (string.IsNullOrWhiteSpace(categoria))
                {
                    _logger.LogWarning("BuscarPorFiltrosAsync: La categoría proporcionada es null o vacía");
                    return new List<ProductoDto>();
                }

                _logger.LogInformation("Buscando productos en categoría '{Categoria}' con {CantidadFiltros} filtros", 
                    categoria, filtros?.Count ?? 0);

                // Iniciar consulta base - Buscar productos de la categoría especificada
                var query = _context.Productos
                    .Include(p => p.Categoria)
                    .Include(p => p.Ofertas)
                    .Where(p => p.EsActivo && p.Categoria != null && p.Categoria.Nombre == categoria);

                // Aplicar filtros dinámicos si se proporcionaron
                if (filtros != null && filtros.Any())
                {
                    foreach (var filtro in filtros)
                    {
                        var valorFiltro = filtro.Value;
                        
                        _logger.LogDebug("Aplicando filtro: {Clave} = {Valor}", filtro.Key, valorFiltro);

                        // Filtrar por FiltroValor1, FiltroValor2, o FiltroValor3 que coincidan con el valor
                        // Usamos un enfoque que busca el valor en cualquiera de los tres campos de filtro
                        query = query.Where(p => 
                            (p.FiltroValor1 != null && p.FiltroValor1.Contains(valorFiltro)) ||
                            (p.FiltroValor2 != null && p.FiltroValor2.Contains(valorFiltro)) ||
                            (p.FiltroValor3 != null && p.FiltroValor3.Contains(valorFiltro)));
                    }
                }

                // Materializar la consulta antes de proyectar a DTO
                var productosDb = await query.ToListAsync();

                // Proyectar a ProductoDto
                var productos = productosDb
                    .Select(p => new ProductoDto
                    {
                        Id = p.Id,
                        Nombre = p.Nombre,
                        Descripcion = p.Descripcion ?? string.Empty,
                        Categoria = p.Categoria?.Nombre ?? string.Empty,
                        Subcategoria = null, // No tenemos subcategoría en la entidad Producto actual
                        Marca = p.Marca ?? string.Empty,
                        NumeroDeparte = p.NumeroDeParte ?? string.Empty,
                        ImagenUrl = p.ImagenUrl,
                        PrecioMinimo = p.Ofertas.Any() ? p.Ofertas.Min(o => o.Precio) : 0,
                        TotalOfertas = p.Ofertas.Count,
                        FechaCreacion = p.FechaCreacion
                    })
                    .OrderBy(p => p.PrecioMinimo) // Ordenar por precio ascendente
                    .ToList();

                _logger.LogInformation("Búsqueda completada. Se encontraron {CantidadProductos} productos", productos.Count);

                return productos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar productos por filtros. Categoría: {Categoria}, Filtros: {Filtros}", 
                    categoria, filtros != null ? string.Join(", ", filtros.Select(f => $"{f.Key}={f.Value}")) : "ninguno");
                
                // Retornar lista vacía en caso de error (no null)
                return new List<ProductoDto>();
            }
        }
    }
}