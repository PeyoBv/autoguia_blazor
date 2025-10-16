namespace AutoGuia.Core.DTOs
{
    public class TallerDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string Direccion { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }
        public string? HorarioAtencion { get; set; }
        public decimal? CalificacionPromedio { get; set; }
        public int TotalResenas { get; set; }
        public string? Especialidades { get; set; }
        public bool EsVerificado { get; set; }
    }
}