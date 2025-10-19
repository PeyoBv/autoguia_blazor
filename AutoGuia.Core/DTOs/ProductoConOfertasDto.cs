namespace AutoGuia.Core.DTOs;

/// <summary>
/// DTO para mostrar producto con sus ofertas en el comparador en tiempo real
/// </summary>
public class ProductoConOfertasDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? NumeroDeParte { get; set; }
    public string? Descripcion { get; set; }
    public string? ImagenUrl { get; set; }
    public List<OfertaComparadorDto> Ofertas { get; set; } = new();
    
    public decimal PrecioMinimo => Ofertas.Any() ? Ofertas.Min(o => o.Precio) : 0;
    public decimal PrecioMaximo => Ofertas.Any() ? Ofertas.Max(o => o.Precio) : 0;
    public int CantidadOfertas => Ofertas.Count;
}

/// <summary>
/// DTO para mostrar oferta individual en el comparador en tiempo real
/// </summary>
public class OfertaComparadorDto
{
    public int Id { get; set; }
    public int TiendaId { get; set; }
    public string TiendaNombre { get; set; } = string.Empty;
    public string? TiendaLogoUrl { get; set; }
    public decimal Precio { get; set; }
    public decimal? PrecioAnterior { get; set; }
    public bool EsDisponible { get; set; }
    public string? UrlProductoEnTienda { get; set; }
    
    public int PorcentajeDescuento
    {
        get
        {
            if (PrecioAnterior.HasValue && PrecioAnterior.Value > Precio)
            {
                return (int)Math.Round(((PrecioAnterior.Value - Precio) / PrecioAnterior.Value) * 100);
            }
            return 0;
        }
    }
}
