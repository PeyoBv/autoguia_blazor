namespace AutoGuia.Core.DTOs
{
    public class PublicacionForoDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Contenido { get; set; } = string.Empty;
        public string? Categoria { get; set; }
        public string? Etiquetas { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public int Vistas { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; } = 0; // Nueva propiedad para sistema de puntuación
        public int TotalRespuestas { get; set; }
        public bool EsDestacado { get; set; }
        public bool EsCerrado { get; set; }
    }

    public class RespuestaForoDto
    {
        public int Id { get; set; }
        public string Contenido { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public int Likes { get; set; }
        public bool EsRespuestaAceptada { get; set; }
    }

    public class CrearPublicacionDto
    {
        public string Titulo { get; set; } = string.Empty;
        public string Contenido { get; set; } = string.Empty;
        public string? Categoria { get; set; }
        public string? Etiquetas { get; set; }
    }

    public class CrearRespuestaDto
    {
        public string Contenido { get; set; } = string.Empty;
        public int PublicacionId { get; set; }
        public int? RespuestaPadreId { get; set; }
    }
}