namespace AutoGuia.Infrastructure.Services;

/// <summary>
/// Servicio para obtener información completa de vehículos
/// mediante VIN (17 caracteres) o Patente chilena (6 caracteres)
/// </summary>
public interface IVehiculoInfoService
{
    /// <summary>
    /// Obtiene información del vehículo mediante VIN (Vehicle Identification Number)
    /// </summary>
    /// <param name="vin">VIN de 17 caracteres alfanuméricos (ej: "1HGBH41JXMN109186")</param>
    /// <returns>Información del vehículo o null si no se encuentra</returns>
    /// <remarks>
    /// Usa NHTSA API (Estados Unidos) como proveedor principal.
    /// Funciona con VINs internacionales.
    /// </remarks>
    Task<AutoGuia.Core.DTOs.VehiculoInfo?> GetInfoByVinAsync(string vin);
    
    /// <summary>
    /// Obtiene información del vehículo mediante Patente Chilena
    /// </summary>
    /// <param name="patente">Patente chilena en formato AAAA11 o AA1111 (ej: "ABCD12", "AB1234")</param>
    /// <returns>Información del vehículo o null si no se encuentra</returns>
    /// <remarks>
    /// Usa GetAPI.cl (Chile y Latinoamérica) como proveedor principal.
    /// Solo funciona con patentes chilenas registradas.
    /// Requiere API Key de GetAPI.cl
    /// </remarks>
    Task<AutoGuia.Core.DTOs.VehiculoInfo?> GetInfoByPatenteAsync(string patente);
}
