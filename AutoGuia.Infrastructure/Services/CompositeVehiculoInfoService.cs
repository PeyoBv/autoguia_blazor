using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AutoGuia.Core.DTOs;

namespace AutoGuia.Infrastructure.Services;

/// <summary>
/// Servicio compuesto que orquesta múltiples proveedores de información vehicular
/// - Para VINs: Usa NHTSA (proveedor principal)
/// - Para Patentes Chilenas: Usa GetAPI.cl (proveedor premium)
/// </summary>
public class CompositeVehiculoInfoService : IVehiculoInfoService
{
    private readonly NhtsaVinService _nhtsaService;
    private readonly GetApiPatenteService _getApiService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CompositeVehiculoInfoService> _logger;

    public CompositeVehiculoInfoService(
        NhtsaVinService nhtsaService,
        GetApiPatenteService getApiService,
        IConfiguration configuration,
        ILogger<CompositeVehiculoInfoService> logger)
    {
        _nhtsaService = nhtsaService;
        _getApiService = getApiService;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene información mediante VIN usando NHTSA
    /// </summary>
    public async Task<VehiculoInfo?> GetInfoByVinAsync(string vin)
    {
        _logger.LogInformation("🔍 [Composite] Iniciando búsqueda por VIN: {VIN}", vin);

        try
        {
            // Para VINs, NHTSA es el proveedor principal (gratuito y confiable)
            _logger.LogInformation("📡 [Composite] Consultando NHTSA...");
            var resultado = await _nhtsaService.GetInfoByVinAsync(vin);

            if (resultado != null && resultado.IsValid)
            {
                _logger.LogInformation("✅ [Composite] Éxito con NHTSA: {Make} {Model} {Year}", 
                    resultado.Make, resultado.Model, resultado.ModelYear);
                resultado.Source = "NHTSA";
                return resultado;
            }

            // Si NHTSA falla, no hay fallback para VINs
            _logger.LogWarning("⚠️ [Composite] NHTSA no pudo decodificar el VIN");
            return new VehiculoInfo
            {
                Vin = vin,
                IsValid = false,
                ErrorMessage = "No se pudo obtener información del VIN. Verifique que sea un VIN válido de 17 caracteres.",
                Source = "Composite"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [Composite] Error obteniendo información por VIN: {VIN}", vin);
            return new VehiculoInfo
            {
                Vin = vin,
                IsValid = false,
                ErrorMessage = "Error inesperado al procesar el VIN",
                Source = "Composite"
            };
        }
    }

    /// <summary>
    /// Obtiene información mediante Patente Chilena usando GetAPI.cl
    /// </summary>
    public async Task<VehiculoInfo?> GetInfoByPatenteAsync(string patente)
    {
        _logger.LogInformation("🔍 [Composite] Iniciando búsqueda por Patente: {Patente}", patente);

        try
        {
            // Verificar si GetAPI está habilitado
            var enableGetApi = bool.Parse(_configuration["VinDecoder:EnableGetApi"] ?? "false");

            if (!enableGetApi)
            {
                _logger.LogWarning("⚠️ [Composite] GetAPI.cl está deshabilitado en configuración");
                return new VehiculoInfo
                {
                    Patente = patente,
                    IsValid = false,
                    ErrorMessage = "Servicio de consulta de patentes deshabilitado. Configure 'VinDecoder:EnableGetApi' en appsettings.json",
                    Source = "Composite"
                };
            }

            // Para patentes chilenas, GetAPI.cl es el único proveedor
            _logger.LogInformation("📡 [Composite] Consultando GetAPI.cl...");
            var resultado = await _getApiService.GetInfoByPatenteAsync(patente);

            if (resultado != null && resultado.IsValid)
            {
                _logger.LogInformation("✅ [Composite] Éxito con GetAPI.cl: {Make} {Model} {Year}", 
                    resultado.Make, resultado.Model, resultado.ModelYear);
                resultado.Source = "GetAPI";
                return resultado;
            }

            // Si GetAPI falla o retorna null (401/error), informar al usuario
            if (resultado == null)
            {
                _logger.LogWarning("⚠️ [Composite] GetAPI.cl no disponible (API Key inválida o error de conexión)");
                return new VehiculoInfo
                {
                    Patente = patente,
                    IsValid = false,
                    ErrorMessage = "Servicio de consulta de patentes no disponible. Verifique su API Key de GetAPI.cl o intente más tarde.",
                    Source = "Composite"
                };
            }

            // GetAPI retornó respuesta pero no es válida
            _logger.LogWarning("⚠️ [Composite] GetAPI.cl no encontró información para patente: {Patente}", patente);
            return resultado; // Ya tiene ErrorMessage asignado
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [Composite] Error obteniendo información por Patente: {Patente}", patente);
            return new VehiculoInfo
            {
                Patente = patente,
                IsValid = false,
                ErrorMessage = "Error inesperado al procesar la patente",
                Source = "Composite"
            };
        }
    }
}
