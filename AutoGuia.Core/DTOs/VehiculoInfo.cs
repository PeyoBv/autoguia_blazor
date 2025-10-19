using System.ComponentModel.DataAnnotations;

namespace AutoGuia.Core.DTOs;

/// <summary>
/// DTO para información completa de un vehículo
/// Puede ser obtenida mediante VIN (17 caracteres) o Patente (6 caracteres chilena)
/// </summary>
public class VehiculoInfo
{
    // ================== IDENTIFICADORES ==================
    
    /// <summary>
    /// VIN (Vehicle Identification Number) - 17 caracteres alfanuméricos
    /// </summary>
    public string? Vin { get; set; }
    
    /// <summary>
    /// Patente (Matrícula) chilena - Formato: AAAA11 o AA1111
    /// </summary>
    public string? Patente { get; set; }
    
    // ================== INFORMACIÓN BÁSICA ==================
    
    /// <summary>
    /// Marca del vehículo (ej: "Toyota", "Ford", "BMW")
    /// </summary>
    [Required(ErrorMessage = "La marca es requerida")]
    public string Make { get; set; } = string.Empty;
    
    /// <summary>
    /// Modelo del vehículo (ej: "Corolla", "F-150", "Serie 3")
    /// </summary>
    [Required(ErrorMessage = "El modelo es requerido")]
    public string Model { get; set; } = string.Empty;
    
    /// <summary>
    /// Año del modelo (ej: "2021", "2020")
    /// </summary>
    [Required(ErrorMessage = "El año es requerido")]
    public string ModelYear { get; set; } = string.Empty;
    
    /// <summary>
    /// Nombre del fabricante (ej: "Toyota Motor Corporation")
    /// </summary>
    public string ManufacturerName { get; set; } = string.Empty;
    
    // ================== MOTOR ==================
    
    /// <summary>
    /// Cilindrada del motor en litros (ej: "2.0", "3.5")
    /// </summary>
    public string EngineDisplacementL { get; set; } = string.Empty;
    
    /// <summary>
    /// Número de cilindros (ej: "4", "6", "8")
    /// </summary>
    public string EngineCylinders { get; set; } = string.Empty;
    
    // ================== TIPO DE VEHÍCULO ==================
    
    /// <summary>
    /// Tipo de vehículo (ej: "Passenger Car", "Truck", "Multipurpose Passenger Vehicle (MPV)")
    /// </summary>
    public string VehicleType { get; set; } = string.Empty;
    
    /// <summary>
    /// Clase de carrocería (ej: "Sedan", "SUV", "Pickup")
    /// </summary>
    public string BodyClass { get; set; } = string.Empty;
    
    /// <summary>
    /// Número de puertas (ej: "4", "2")
    /// </summary>
    public string Doors { get; set; } = string.Empty;
    
    // ================== PROPULSIÓN ==================
    
    /// <summary>
    /// Tipo de combustible primario (ej: "Gasoline", "Diesel", "Electric")
    /// </summary>
    public string FuelTypePrimary { get; set; } = string.Empty;
    
    /// <summary>
    /// Tipo de transmisión (ej: "Automatic", "Manual")
    /// </summary>
    public string TransmissionStyle { get; set; } = string.Empty;
    
    /// <summary>
    /// Tipo de tracción (ej: "Front-Wheel Drive (FWD)", "4x4")
    /// </summary>
    public string DriveType { get; set; } = string.Empty;
    
    // ================== OTROS SISTEMAS ==================
    
    /// <summary>
    /// Tipo de sistema de frenos (ej: "Hydraulic", "Disc")
    /// </summary>
    public string BrakeSystemType { get; set; } = string.Empty;
    
    // ================== MANUFACTURA ==================
    
    /// <summary>
    /// País de manufactura (ej: "United States", "Japan", "Germany")
    /// </summary>
    public string PlantCountry { get; set; } = string.Empty;
    
    /// <summary>
    /// Ciudad de manufactura (ej: "Detroit", "Tokyo")
    /// </summary>
    public string PlantCity { get; set; } = string.Empty;
    
    // ================== METADATOS ==================
    
    /// <summary>
    /// Indica si la información es válida y completa
    /// </summary>
    public bool IsValid { get; set; }
    
    /// <summary>
    /// Mensaje de error si IsValid = false
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// Fuente de la información (ej: "NHTSA", "GetAPI", "Composite")
    /// </summary>
    public string Source { get; set; } = string.Empty;
}
