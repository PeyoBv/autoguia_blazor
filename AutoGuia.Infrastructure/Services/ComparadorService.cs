using Microsoft.EntityFrameworkCore;
using AutoGuia.Core.DTOs;
using AutoGuia.Core.Entities;
using AutoGuia.Infrastructure.Data;

namespace AutoGuia.Infrastructure.Services
{
    /// <summary>
    /// Servicio para el sistema de comparación de precios de productos automotrices
    /// </summary>
    public class ComparadorService : IComparadorService
    {
        private readonly AutoGuiaDbContext _context;

        public ComparadorService(AutoGuiaDbContext context)
        {
            _context = context;
        }

        public async Task<ResultadoBusquedaDto> BuscarProductosAsync(BusquedaProductoDto busqueda)
        {
            var query = _context.Productos.AsQueryable();

            // Filtro por término de búsqueda
            if (!string.IsNullOrEmpty(busqueda.TerminoBusqueda))
            {
                var termino = busqueda.TerminoBusqueda.ToLower();
                query = query.Where(p => 
                    p.Nombre.ToLower().Contains(termino) ||
                    p.Descripcion.ToLower().Contains(termino) ||
                    p.NumeroDeParte.ToLower().Contains(termino));
            }

            // Filtro por categoría - temporalmente deshabilitado ya que los productos no tienen categoría directa
            /*
            if (!string.IsNullOrEmpty(busqueda.Categoria))
            {
                query = query.Where(p => p.Categoria == busqueda.Categoria);
            }
            */

            // Filtro por marca de producto - temporalmente deshabilitado ya que los productos no tienen marca directa
            /*
            if (!string.IsNullOrEmpty(busqueda.MarcaProducto))
            {
                query = query.Where(p => p.Marca == busqueda.MarcaProducto);
            }
            */

            // Filtro por compatibilidad de vehículo
            if (busqueda.MarcaVehiculoId.HasValue)
            {
                query = query.Where(p => p.VehiculosCompatibles.Any(vc => vc.Modelo.MarcaId == busqueda.MarcaVehiculoId));
            }

            if (busqueda.ModeloVehiculoId.HasValue)
            {
                query = query.Where(p => p.VehiculosCompatibles.Any(vc => vc.ModeloId == busqueda.ModeloVehiculoId));
            }

            // Contar total para paginación
            var total = await query.CountAsync();

            // Aplicar paginación
            var productos = await query
                .Include(p => p.Ofertas)
                    .ThenInclude(o => o.Tienda)
                .Skip((busqueda.Pagina - 1) * busqueda.TamanoPagina)
                .Take(busqueda.TamanoPagina)
                .Select(p => new ProductoResultadoDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    NumeroDeParte = p.NumeroDeParte,
                    ImagenUrl = p.ImagenUrl,
                    PrecioMinimo = p.Ofertas.Any() ? p.Ofertas.Min(o => o.Precio) : 0,
                    PrecioMaximo = p.Ofertas.Any() ? p.Ofertas.Max(o => o.Precio) : 0,
                    TotalOfertas = p.Ofertas.Count(),
                    MejorOferta = p.Ofertas.Any() ? new OfertaResumenDto
                    {
                        Id = p.Ofertas.OrderBy(o => o.Precio).First().Id,
                        Precio = p.Ofertas.Min(o => o.Precio),
                        TiendaNombre = p.Ofertas.OrderBy(o => o.Precio).First().Tienda.Nombre,
                        TiendaLogo = p.Ofertas.OrderBy(o => o.Precio).First().Tienda.LogoUrl,
                        UrlProductoEnTienda = p.Ofertas.OrderBy(o => o.Precio).First().UrlProductoEnTienda
                    } : null
                })
                .ToListAsync();

            return new ResultadoBusquedaDto
            {
                Productos = productos,
                Total = total,
                Pagina = busqueda.Pagina,
                TamanoPagina = busqueda.TamanoPagina,
                TotalPaginas = (int)Math.Ceiling((double)total / busqueda.TamanoPagina)
            };
        }

        public async Task<ProductoDetalleDto?> ObtenerProductoDetalleAsync(int productoId)
        {
            var producto = await _context.Productos
                .Include(p => p.Ofertas)
                    .ThenInclude(o => o.Tienda)
                .Include(p => p.VehiculosCompatibles)
                    .ThenInclude(vc => vc.Modelo)
                        .ThenInclude(m => m.Marca)
                .FirstOrDefaultAsync(p => p.Id == productoId);

            if (producto == null)
                return null;

            return new ProductoDetalleDto
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                NumeroDeParte = producto.NumeroDeParte,
                ImagenUrl = producto.ImagenUrl,
                Ofertas = producto.Ofertas.Select(o => new OfertaDto
                {
                    Id = o.Id,
                    ProductoId = o.ProductoId,
                    TiendaId = o.TiendaId,
                    TiendaNombre = o.Tienda.Nombre,
                    TiendaLogo = o.Tienda.LogoUrl,
                    Precio = o.Precio,
                    UrlProductoEnTienda = o.UrlProductoEnTienda,
                    EsDisponible = o.EsDisponible,
                    FechaActualizacion = o.FechaActualizacion
                }).OrderBy(o => o.Precio).ToList(),
                VehiculosCompatibles = producto.VehiculosCompatibles.Select(vc => new VehiculoCompatibleDto
                {
                    MarcaId = vc.Modelo.MarcaId,
                    MarcaNombre = vc.Modelo.Marca.Nombre,
                    ModeloId = vc.ModeloId,
                    ModeloNombre = vc.Modelo.Nombre,
                    AnioInicioProduccion = vc.Modelo.AnioInicioProduccion ?? 0,
                    AnioFinProduccion = vc.Modelo.AnioFinProduccion ?? 0
                }).ToList(),
                FechaCreacion = producto.FechaCreacion
            };
        }

        public async Task<IEnumerable<OfertaDestacadaDto>> ObtenerOfertasDestacadasAsync(int cantidad = 10)
        {
            var ofertas = await _context.Ofertas
                .Include(o => o.Producto)
                .Include(o => o.Tienda)
                .Where(o => o.EsDisponible)
                .OrderBy(o => o.Precio)
                .Take(cantidad)
                .Select(o => new OfertaDestacadaDto
                {
                    ProductoId = o.ProductoId,
                    ProductoNombre = o.Producto.Nombre,
                    ProductoImagen = o.Producto.ImagenUrl,
                    ProductoMarca = "Sin marca", // Producto no tiene Marca directa
                    TiendaNombre = o.Tienda.Nombre,
                    TiendaLogo = o.Tienda.LogoUrl,
                    Precio = o.Precio,
                    PrecioAnterior = o.PrecioAnterior,
                    PorcentajeDescuento = o.PrecioAnterior.HasValue ? 
                        (int)Math.Round(((o.PrecioAnterior.Value - o.Precio) / o.PrecioAnterior.Value) * 100) : 0,
                    Stock = 0, // Stock no existe en Oferta
                    FechaFin = null, // FechaFin no existe en Oferta
                    UrlTienda = o.UrlProductoEnTienda
                })
                .ToListAsync();

            return ofertas;
        }

        public async Task<IEnumerable<ProductoDto>> ObtenerProductosPorCategoriaAsync(string categoria)
        {
            // Por ahora devolvemos todos los productos ya que no tienen categoría directa
            var productos = await _context.Productos
                .Include(p => p.Ofertas)
                .Select(p => new ProductoDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Categoria = "General", // Categoria no existe en entidad
                    Marca = "Sin marca", // Marca no existe en entidad
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

        public async Task<IEnumerable<string>> ObtenerCategoriasAsync()
        {
            // Por ahora devolvemos categorías hardcoded ya que los productos no tienen categoría directa
            return new List<string> { "Motor", "Frenos", "Suspensión", "Eléctrico", "Carrocería" };
        }

        public async Task<IEnumerable<ProductoDto>> ObtenerProductosCompatiblesAsync(int marcaId, int modeloId)
        {
            var productos = await _context.Productos
                .Where(p => p.VehiculosCompatibles.Any(vc => vc.Modelo.MarcaId == marcaId && vc.ModeloId == modeloId))
                .Include(p => p.Ofertas)
                .Select(p => new ProductoDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Categoria = "General", // Categoria no existe en entidad
                    Marca = "Sin marca", // Marca no existe en entidad
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

        public async Task<bool> ActualizarPrecioOfertaAsync(int ofertaId, decimal nuevoPrecio)
        {
            var oferta = await _context.Ofertas.FindAsync(ofertaId);
            if (oferta == null)
                return false;

            oferta.Precio = nuevoPrecio;
            oferta.FechaActualizacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}