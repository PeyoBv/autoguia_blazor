using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AutoGuia.Core.DTOs;
using AutoGuia.Infrastructure.Configuration;

namespace AutoGuia.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de información vehicular usando GetAPI.cl (patentes chilenas)
/// API: https://chile.getapi.cl/v1/vehicles/plate/{patente}
/// </summary>
public class GetApiPatenteService : IVehiculoInfoService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<GetApiPatenteService> _logger;
    private readonly VinDecoderSettings _settings;

    public GetApiPatenteService(
        IHttpClientFactory httpClientFactory,
        ILogger<GetApiPatenteService> logger,
        IOptionsSnapshot<VinDecoderSettings> settings)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _settings = settings.Value;
    }

    /// <summary>
    /// GetAPI.cl no soporta búsqueda por VIN - solo patentes
    /// </summary>
    public Task<VehiculoInfo?> GetInfoByVinAsync(string vin)
    {
        _logger.LogWarning("⚠️ [GetAPI] Búsqueda por VIN no soportada");
        return Task.FromResult<VehiculoInfo?>(new VehiculoInfo
        {
            Vin = vin,
            IsValid = false,
            ErrorMessage = "GetAPI.cl solo soporta patentes chilenas. Use NHTSA para VINs",
            Source = "GetAPI"
        });
    }

    /// <summary>
    /// Obtiene información del vehículo mediante Patente Chilena
    /// </summary>
    public async Task<VehiculoInfo?> GetInfoByPatenteAsync(string patente)
    {
        try
        {
            // 1. Validar patente
            if (string.IsNullOrWhiteSpace(patente))
            {
                _logger.LogWarning("[GetAPI] Patente vacía o nula recibida");
                return new VehiculoInfo
                {
                    IsValid = false,
                    ErrorMessage = "La patente no puede estar vacía",
                    Source = "GetAPI"
                };
            }

            // Limpiar y normalizar (quitar espacios, guiones, convertir a mayúsculas)
            patente = patente.Trim().Replace("-", "").Replace(" ", "").ToUpperInvariant();

            // Validar formato chileno: AAAA11 (4 letras + 2 números) o AA1111 (2 letras + 4 números)
            if (!EsFormatoValidoChileno(patente))
            {
                _logger.LogWarning("[GetAPI] Patente con formato inválido: {Patente}", patente);
                return new VehiculoInfo
                {
                    Patente = patente,
                    IsValid = false,
                    ErrorMessage = "Formato de patente chilena inválido. Use AAAA11 (ej: ABCD12) o AA1111 (ej: AB1234)",
                    Source = "GetAPI"
                };
            }

            // 2. Leer configuración
            var providerSettings = _settings.GetApi ?? new VinDecoderSettings.GetApiSettings();
            var apiKey = providerSettings.ApiKey;
            var baseUrl = string.IsNullOrWhiteSpace(providerSettings.BaseUrl)
                ? "https://chile.getapi.cl/v1/vehicles"
                : providerSettings.BaseUrl;

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                _logger.LogError("[GetAPI] API Key no configurada");
                return new VehiculoInfo
                {
                    Patente = patente,
                    IsValid = false,
                    ErrorMessage = "API Key de GetAPI.cl no configurada. Establécela mediante secretos de usuario o variables de entorno (VinDecoder:GetApi:ApiKey)",
                    Source = "GetAPI"
                };
            }

            // 3. Construir URL
            var url = $"{baseUrl}/plate/{Uri.EscapeDataString(patente)}";
            _logger.LogInformation("🔍 [GetAPI] Consultando patente: {Patente}", patente);
            _logger.LogDebug("📡 [GetAPI] URL: {Url}", url);

            // 4. Llamar a la API
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("X-Api-Key", apiKey);

            var response = await httpClient.GetAsync(url);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _logger.LogError("❌ [GetAPI] API Key inválida o expirada (401 Unauthorized)");
                return null; // Retornar null para que Composite pruebe fallback
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("⚠️ [GetAPI] Patente no encontrada: {Patente}", patente);
                return new VehiculoInfo
                {
                    Patente = patente,
                    IsValid = false,
                    ErrorMessage = "Patente no encontrada en el registro vehicular chileno",
                    Source = "GetAPI"
                };
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("❌ [GetAPI] Error HTTP {StatusCode}", response.StatusCode);
                return null; // Retornar null para fallback
            }

            // 5. Deserializar respuesta
            var jsonContent = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("📥 [GetAPI] Respuesta JSON: {Json}", jsonContent);

            var apiResponse = JsonSerializer.Deserialize<GetApiResponse>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (apiResponse == null || !apiResponse.Success || apiResponse.Data == null)
            {
                _logger.LogWarning("⚠️ [GetAPI] Respuesta inválida o sin datos para patente: {Patente}", patente);
                return new VehiculoInfo
                {
                    Patente = patente,
                    IsValid = false,
                    ErrorMessage = apiResponse?.Message ?? "No se encontró información para esta patente",
                    Source = "GetAPI"
                };
            }

            // 6. Mapear a VehiculoInfo
            var vehiculoInfo = MapearResultado(patente, apiResponse.Data);
            vehiculoInfo.Source = "GetAPI";

            _logger.LogInformation("✅ [GetAPI] Patente consultada: {Make} {Model} {Year}", 
                vehiculoInfo.Make, vehiculoInfo.Model, vehiculoInfo.ModelYear);

            return vehiculoInfo;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "❌ [GetAPI] Error de red consultando patente: {Patente}", patente);
            return null; // Retornar null para permitir fallback
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "❌ [GetAPI] Error deserializando respuesta para patente: {Patente}", patente);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [GetAPI] Error inesperado consultando patente: {Patente}", patente);
            return null;
        }
    }

    /// <summary>
    /// Valida formato de patente chilena
    /// Formatos válidos:
    /// - AAAA11 (4 letras + 2 números) - Antiguo
    /// - AA1111 (2 letras + 4 números) - Nuevo
    /// </summary>
    private bool EsFormatoValidoChileno(string patente)
    {
        if (patente.Length != 6)
            return false;

        // Formato antiguo: AAAA11 (4 letras + 2 números)
        if (patente.Length == 6 && 
            char.IsLetter(patente[0]) && char.IsLetter(patente[1]) &&
            char.IsLetter(patente[2]) && char.IsLetter(patente[3]) &&
            char.IsDigit(patente[4]) && char.IsDigit(patente[5]))
        {
            return true;
        }

        // Formato nuevo: AA1111 (2 letras + 4 números)
        if (patente.Length == 6 &&
            char.IsLetter(patente[0]) && char.IsLetter(patente[1]) &&
            char.IsDigit(patente[2]) && char.IsDigit(patente[3]) &&
            char.IsDigit(patente[4]) && char.IsDigit(patente[5]))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Mapea respuesta de GetAPI.cl a VehiculoInfo
    /// </summary>
    private VehiculoInfo MapearResultado(string patente, GetApiVehicleData data)
    {
        return new VehiculoInfo
        {
            Patente = patente,
            Vin = data.Vin ?? string.Empty,
            Make = data.Make ?? string.Empty,
            Model = data.Model ?? string.Empty,
            ModelYear = data.Year?.ToString() ?? string.Empty,
            ManufacturerName = data.Make ?? string.Empty, // GetAPI no separa make/manufacturer
            EngineDisplacementL = data.EngineSize ?? string.Empty,
            FuelTypePrimary = data.FuelType ?? string.Empty,
            VehicleType = data.VehicleType ?? string.Empty,
            BodyClass = data.BodyStyle ?? string.Empty,
            TransmissionStyle = data.Transmission ?? string.Empty,
            IsValid = true
        };
    }

    #region DTOs Internos para GetAPI.cl

    /// <summary>
    /// DTO para respuesta de GetAPI.cl
    /// </summary>
    private class GetApiResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("data")]
        public GetApiVehicleData? Data { get; set; }
    }

    /// <summary>
    /// DTO para datos del vehículo de GetAPI.cl
    /// </summary>
    private class GetApiVehicleData
    {
        [JsonPropertyName("plate")]
        public string? Plate { get; set; }

        [JsonPropertyName("vin")]
        public string? Vin { get; set; }

        [JsonPropertyName("make")]
        public string? Make { get; set; }

        // Model puede venir como string o como objeto
        [JsonPropertyName("model")]
        public JsonElement? ModelRaw { get; set; }

        // Propiedad helper para obtener el modelo como string
        [JsonIgnore]
        public string? Model => ModelRaw.HasValue 
            ? (ModelRaw.Value.ValueKind == JsonValueKind.String 
                ? ModelRaw.Value.GetString() 
                : ModelRaw.Value.ValueKind == JsonValueKind.Object && ModelRaw.Value.TryGetProperty("name", out var name)
                    ? name.GetString()
                    : ModelRaw.Value.ToString())
            : null;

        [JsonPropertyName("year")]
        public int? Year { get; set; }

        [JsonPropertyName("color")]
        public string? Color { get; set; }

        [JsonPropertyName("engineSize")]
        public string? EngineSize { get; set; }

        [JsonPropertyName("fuelType")]
        public string? FuelType { get; set; }

        [JsonPropertyName("transmission")]
        public string? Transmission { get; set; }

        [JsonPropertyName("vehicleType")]
        public string? VehicleType { get; set; }

        [JsonPropertyName("bodyStyle")]
        public string? BodyStyle { get; set; }

        [JsonPropertyName("doors")]
        public int? Doors { get; set; }

        [JsonPropertyName("cylinders")]
        public int? Cylinders { get; set; }

        [JsonPropertyName("registrationDate")]
        public string? RegistrationDate { get; set; }

        [JsonPropertyName("ownerName")]
        public string? OwnerName { get; set; }

        [JsonPropertyName("address")]
        public string? Address { get; set; }
    }

    #endregion
}
