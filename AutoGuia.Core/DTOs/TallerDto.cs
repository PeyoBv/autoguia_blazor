using System.ComponentModel.DataAnnotations;

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

    public class CrearTallerDto
    {
        [Required(ErrorMessage = "El nombre del taller es obligatorio")]
        [StringLength(200, ErrorMessage = "El nombre no puede exceder los 200 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "La descripción no puede exceder los 1000 caracteres")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria")]
        [StringLength(300, ErrorMessage = "La dirección no puede exceder los 300 caracteres")]
        public string Direccion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La ciudad es obligatoria")]
        [StringLength(100, ErrorMessage = "La ciudad no puede exceder los 100 caracteres")]
        public string Ciudad { get; set; } = string.Empty;

        [Required(ErrorMessage = "La región es obligatoria")]
        [StringLength(100, ErrorMessage = "La región no puede exceder los 100 caracteres")]
        public string Region { get; set; } = string.Empty;

        [Phone(ErrorMessage = "El formato del teléfono no es válido")]
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres")]
        public string? Telefono { get; set; }

        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [StringLength(100, ErrorMessage = "El email no puede exceder los 100 caracteres")]
        public string? Email { get; set; }

        [Range(-90, 90, ErrorMessage = "La latitud debe estar entre -90 y 90 grados")]
        public double? Latitud { get; set; }

        [Range(-180, 180, ErrorMessage = "La longitud debe estar entre -180 y 180 grados")]
        public double? Longitud { get; set; }

        [StringLength(200, ErrorMessage = "El horario de atención no puede exceder los 200 caracteres")]
        public string? HorarioAtencion { get; set; }

        [StringLength(500, ErrorMessage = "Las especialidades no pueden exceder los 500 caracteres")]
        public string? Especialidades { get; set; }

        public bool EsVerificado { get; set; } = false;
    }

    public class ActualizarTallerDto : CrearTallerDto
    {
        // Hereda todas las propiedades de CrearTallerDto
        // Se puede extender con propiedades específicas para actualización si es necesario
    }
}