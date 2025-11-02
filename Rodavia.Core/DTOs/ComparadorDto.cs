using System.ComponentModel.DataAnnotations;

namespace Rodavia.Core.DTOs
{
    public class ResultadoBusquedaDto
    {
        public List<ProductoResultadoDto> Productos { get; set; } = new();
        public int Total { get; set; }
        public int Pagina { get; set; }
        public int TamanoPagina { get; set; }
        public int TotalPaginas { get; set; }
    }

    public class ProductoResultadoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string NumeroDeParte { get; set; } = string.Empty;
        public string? ImagenUrl { get; set; }
        public decimal PrecioMinimo { get; set; }
        public decimal PrecioMaximo { get; set; }
        public int TotalOfertas { get; set; }
        public OfertaResumenDto? MejorOferta { get; set; }
    }

    public class OfertaResumenDto
    {
        public int Id { get; set; }
        public decimal Precio { get; set; }
        public string TiendaNombre { get; set; } = string.Empty;
        public string? TiendaLogo { get; set; }
        public string? UrlProductoEnTienda { get; set; }
        public bool EsDisponible { get; set; } = true;
    }

    public class ProductoDetalleDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string NumeroDeParte { get; set; } = string.Empty;
        public string? ImagenUrl { get; set; }
        public List<OfertaDto> Ofertas { get; set; } = new();
        public List<VehiculoCompatibleDto> VehiculosCompatibles { get; set; } = new();
        public DateTime FechaCreacion { get; set; }
    }

    public class OfertaDestacadaDto
    {
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        public string? ProductoImagen { get; set; }
        public string ProductoMarca { get; set; } = string.Empty;
        public string TiendaNombre { get; set; } = string.Empty;
        public string? TiendaLogo { get; set; }
        public decimal Precio { get; set; }
        public decimal? PrecioAnterior { get; set; }
        public int PorcentajeDescuento { get; set; }
        public int Stock { get; set; }
        public DateTime? FechaFin { get; set; }
        public string? UrlTienda { get; set; }
    }

    public class ProductoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string? Subcategoria { get; set; }
        public string Marca { get; set; } = string.Empty;
        public string NumeroDeparte { get; set; } = string.Empty;
        public string? ImagenUrl { get; set; }
        public decimal PrecioMinimo { get; set; }
        public int TotalOfertas { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    public class OfertaDto
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string? ProductoNombre { get; set; }
        public string? ProductoImagen { get; set; }
        public int TiendaId { get; set; }
        public string TiendaNombre { get; set; } = string.Empty;
        public string? TiendaLogo { get; set; }
        public decimal Precio { get; set; }
        public decimal? PrecioAnterior { get; set; }
        public bool EsOferta { get; set; }
        public bool EsDisponible { get; set; } = true;
        public string UrlProductoEnTienda { get; set; } = string.Empty;
        public string? SKU { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }

    public class TiendaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string UrlSitioWeb { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public IEnumerable<OfertaDto>? Ofertas { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    public class MarcaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public int TotalModelos { get; set; }
    }

    public class ModeloDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int MarcaId { get; set; }
        public string MarcaNombre { get; set; } = string.Empty;
        public int AnioInicioProduccion { get; set; }
        public int AnioFinProduccion { get; set; }
    }

    public class VehiculoCompatibleDto
    {
        public int MarcaId { get; set; }
        public string MarcaNombre { get; set; } = string.Empty;
        public int ModeloId { get; set; }
        public string ModeloNombre { get; set; } = string.Empty;
        public int AnioInicioProduccion { get; set; }
        public int AnioFinProduccion { get; set; }
    }

    public class BusquedaProductoDto
    {
        public string? TerminoBusqueda { get; set; }
        public string? Categoria { get; set; }
        public string? MarcaProducto { get; set; }
        public int? MarcaVehiculoId { get; set; }
        public int? ModeloVehiculoId { get; set; }
        public decimal? PrecioMinimo { get; set; }
        public decimal? PrecioMaximo { get; set; }
        public bool SoloConStock { get; set; } = true;
        public string OrdenarPor { get; set; } = "precio";
        public bool OrdenAscendente { get; set; } = true;
        public int Pagina { get; set; } = 1;
        public int TamanoPagina { get; set; } = 20;
    }

    public class CrearProductoDto
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        [Required]
        public string Categoria { get; set; } = string.Empty;
        public string? Subcategoria { get; set; }
        [Required]
        public string Marca { get; set; } = string.Empty;
        [Required]
        public string NumeroDeparte { get; set; } = string.Empty;
        public string? ImagenUrl { get; set; }
    }

    public class ActualizarProductoDto
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        [Required]
        public string Categoria { get; set; } = string.Empty;
        public string? Subcategoria { get; set; }
        [Required]
        public string Marca { get; set; } = string.Empty;
        [Required]
        public string NumeroDeparte { get; set; } = string.Empty;
        public string? ImagenUrl { get; set; }
    }

    public class CrearTiendaDto
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? SitioWeb { get; set; }
        public string? LogoUrl { get; set; }
    }

    public class ActualizarTiendaDto
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? SitioWeb { get; set; }
        public string? LogoUrl { get; set; }
        public bool EsConfiable { get; set; }
    }

    public class CrearMarcaDto
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
    }

    public class CrearModeloDto
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        public int MarcaId { get; set; }
        public int? AnoInicio { get; set; }
        public int? AnoFin { get; set; }
    }

    // DTO para experiencias de usuarios en talleres
    public class ExperienciaUsuarioDto
    {
        public int Id { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string UsuarioInicial { get; set; } = string.Empty;
        public string Comentario { get; set; } = string.Empty;
        public bool EsPositiva { get; set; } = true;
        public int Calificacion { get; set; } = 5;
        public string? ServicioTipo { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public int Likes { get; set; } = 0;
        public int TallerId { get; set; }
    }
}
