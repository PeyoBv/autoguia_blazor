namespace AutoGuia.Core.DTOs
{
    /// <summary>
    /// DTO para representar un marcador en el mapa
    /// </summary>
    public class MarcadorMapaDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public string Direccion { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool EsVerificado { get; set; }
        public double CalificacionPromedio { get; set; }
        public string IconoUrl { get; set; } = string.Empty;
    }
}