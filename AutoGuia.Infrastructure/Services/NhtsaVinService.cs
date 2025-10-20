using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Http;
using AutoGuia.Core.DTOs;

namespace AutoGuia.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de información vehicular usando NHTSA (solo VINs)
/// API: https://vpic.nhtsa.dot.gov/api/
/// </summary>
public class NhtsaVinService : IVehiculoInfoService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<NhtsaVinService> _logger;
    
    private const string API_BASE_URL = "https://vpic.nhtsa.dot.gov/api/vehicles";

    public NhtsaVinService(
        IHttpClientFactory httpClientFactory, 
        ILogger<NhtsaVinService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene información del vehículo mediante VIN
    /// </summary>
    public async Task<VehiculoInfo?> GetInfoByVinAsync(string vin)
    {
        try
        {
            // 1. Validar VIN
            if (string.IsNullOrWhiteSpace(vin))
            {
                _logger.LogWarning("[NHTSA] VIN vacío o nulo recibido");
                return new VehiculoInfo 
                { 
                    IsValid = false, 
                    ErrorMessage = "El VIN no puede estar vacío",
                    Source = "NHTSA"
                };
            }

            vin = vin.Trim().ToUpperInvariant();

            if (vin.Length != 17)
            {
                _logger.LogWarning("[NHTSA] VIN con longitud inválida: {Length} caracteres", vin.Length);
                return new VehiculoInfo 
                { 
                    Vin = vin,
                    IsValid = false, 
                    ErrorMessage = $"El VIN debe tener exactamente 17 caracteres (tiene {vin.Length})",
                    Source = "NHTSA"
                };
            }

            // 2. Construir URL
            var url = $"{API_BASE_URL}/DecodeVin/{Uri.EscapeDataString(vin)}?format=json";
            _logger.LogInformation("🔍 [NHTSA] Decodificando VIN: {VIN}", vin);

            // 3. Llamar a la API
            var httpClient = _httpClientFactory.CreateClient("NHTSA_API");
            
            HttpResponseMessage response;
            try
            {
                response = await httpClient.GetAsync(url);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "❌ [NHTSA] Error de conexión al consultar API");
                return new VehiculoInfo 
                { 
                    Vin = vin,
                    IsValid = false, 
                    ErrorMessage = "⚠️ El servicio de decodificación de VIN (NHTSA) no está disponible temporalmente. Por favor, intenta más tarde.",
                    Source = "NHTSA"
                };
            }
            catch (TaskCanceledException)
            {
                _logger.LogError("❌ [NHTSA] Timeout al consultar API");
                return new VehiculoInfo 
                { 
                    Vin = vin,
                    IsValid = false, 
                    ErrorMessage = "⚠️ La consulta tardó demasiado tiempo. Por favor, intenta nuevamente.",
                    Source = "NHTSA"
                };
            }

            if (!response.IsSuccessStatusCode)
            {
                var statusMessage = response.StatusCode switch
                {
                    System.Net.HttpStatusCode.ServiceUnavailable => "⚠️ El servicio de decodificación de VIN (NHTSA) está temporalmente fuera de servicio. Por favor, intenta más tarde.",
                    System.Net.HttpStatusCode.TooManyRequests => "⚠️ Se han realizado demasiadas consultas. Por favor, espera unos minutos e intenta nuevamente.",
                    System.Net.HttpStatusCode.InternalServerError => "⚠️ Error en el servidor de NHTSA. Por favor, intenta más tarde.",
                    _ => $"⚠️ Error al consultar la API de NHTSA ({(int)response.StatusCode}: {response.StatusCode})"
                };
                
                _logger.LogError("❌ [NHTSA] Error HTTP {StatusCode}", response.StatusCode);
                return new VehiculoInfo 
                { 
                    Vin = vin,
                    IsValid = false, 
                    ErrorMessage = statusMessage,
                    Source = "NHTSA"
                };
            }

            // 4. Deserializar
            var jsonContent = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<NhtsaApiResponse>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (apiResponse?.Results == null || apiResponse.Results.Count == 0)
            {
                _logger.LogWarning("⚠️ [NHTSA] Respuesta vacía para VIN: {VIN}", vin);
                return new VehiculoInfo 
                { 
                    Vin = vin,
                    IsValid = false, 
                    ErrorMessage = "No se encontró información para este VIN",
                    Source = "NHTSA"
                };
            }

            // 5. Mapear resultados
            var vehiculoInfo = MapearResultados(vin, apiResponse.Results);
            vehiculoInfo.Source = "NHTSA";

            _logger.LogInformation("✅ [NHTSA] VIN decodificado: {Make} {Model} {Year}", 
                vehiculoInfo.Make, vehiculoInfo.Model, vehiculoInfo.ModelYear);

            return vehiculoInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [NHTSA] Error decodificando VIN: {VIN}", vin);
            return new VehiculoInfo 
            { 
                Vin = vin,
                IsValid = false, 
                ErrorMessage = "Error inesperado al decodificar el VIN",
                Source = "NHTSA"
            };
        }
    }

    /// <summary>
    /// NHTSA no soporta búsqueda por patente - solo VINs
    /// </summary>
    public Task<VehiculoInfo?> GetInfoByPatenteAsync(string patente)
    {
        _logger.LogWarning("⚠️ [NHTSA] Búsqueda por patente no soportada");
        return Task.FromResult<VehiculoInfo?>(new VehiculoInfo
        {
            Patente = patente,
            IsValid = false,
            ErrorMessage = "NHTSA no soporta búsqueda por patente chilena. Use GetAPI.cl",
            Source = "NHTSA"
        });
    }

    /// <summary>
    /// Mapea resultados de NHTSA a VehiculoInfo
    /// </summary>
    private VehiculoInfo MapearResultados(string vin, List<NhtsaResultItem> results)
    {
        var vehiculoInfo = new VehiculoInfo
        {
            Vin = vin,
            IsValid = true
        };

        var resultsDict = results.ToDictionary(
            r => r.Variable ?? string.Empty,
            r => r.Value ?? string.Empty,
            StringComparer.OrdinalIgnoreCase
        );

        string GetValue(params string[] possibleKeys)
        {
            foreach (var key in possibleKeys)
            {
                if (resultsDict.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value))
                    return value;
            }
            return string.Empty;
        }

        // Mapear campos
        vehiculoInfo.Make = GetValue("Make");
        vehiculoInfo.Model = GetValue("Model");
        vehiculoInfo.ModelYear = GetValue("Model Year", "ModelYear");
        vehiculoInfo.ManufacturerName = GetValue("Manufacturer Name", "ManufacturerName");
        vehiculoInfo.EngineDisplacementL = GetValue("Displacement (L)", "DisplacementL");
        vehiculoInfo.EngineCylinders = GetValue("Engine Number of Cylinders", "EngineCylinders");
        vehiculoInfo.VehicleType = GetValue("Vehicle Type");
        vehiculoInfo.BodyClass = GetValue("Body Class", "BodyClass");
        vehiculoInfo.Doors = GetValue("Doors");
        vehiculoInfo.FuelTypePrimary = GetValue("Fuel Type - Primary", "FuelType");
        vehiculoInfo.TransmissionStyle = GetValue("Transmission Style", "TransmissionStyle");
        vehiculoInfo.DriveType = GetValue("Drive Type", "DriveType");
        vehiculoInfo.BrakeSystemType = GetValue("Brake System Type", "BrakeSystemType");
        vehiculoInfo.PlantCity = GetValue("Plant City");
        vehiculoInfo.PlantCountry = GetValue("Plant Country");

        // Verificar errores
        var errorCode = GetValue("Error Code", "ErrorCode");
        var errorText = GetValue("Error Text", "ErrorText");
        
        if (!string.IsNullOrWhiteSpace(errorCode) && errorCode != "0")
        {
            if (errorCode == "1")
            {
                _logger.LogWarning("[NHTSA] VIN {VIN} tiene Error Code 1 (Check Digit): {ErrorText}", vin, errorText);
            }
            else if (errorCode == "6" || errorCode == "8")
            {
                vehiculoInfo.IsValid = false;
                vehiculoInfo.ErrorMessage = errorCode switch
                {
                    "8" => "⚠️ VIN sin información detallada en NHTSA (vehículo muy antiguo/nuevo o no registrado en EE.UU.)",
                    "6" => "❌ VIN no válido o no existe",
                    _ => $"❌ Error NHTSA: {errorText}"
                };
                
                _logger.LogWarning("[NHTSA] VIN {VIN} rechazado - Error Code {ErrorCode}", vin, errorCode);
                return vehiculoInfo;
            }
        }

        // Validar datos mínimos
        if (string.IsNullOrWhiteSpace(vehiculoInfo.Make) && 
            string.IsNullOrWhiteSpace(vehiculoInfo.Model) && 
            string.IsNullOrWhiteSpace(vehiculoInfo.ModelYear))
        {
            vehiculoInfo.IsValid = false;
            if (string.IsNullOrEmpty(vehiculoInfo.ErrorMessage))
            {
                vehiculoInfo.ErrorMessage = "⚠️ No se pudo obtener información del vehículo";
            }
        }

        return vehiculoInfo;
    }

    #region DTOs Internos

    private class NhtsaApiResponse
    {
        public int Count { get; set; }
        public string Message { get; set; } = string.Empty;
        public string SearchCriteria { get; set; } = string.Empty;
        public List<NhtsaResultItem> Results { get; set; } = new();
    }

    private class NhtsaResultItem
    {
        public string? Value { get; set; }
        public string? ValueId { get; set; }
        public string? Variable { get; set; }
        public int VariableId { get; set; }
    }

    #endregion
}
