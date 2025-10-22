namespace AutoGuia.Infrastructure.Configuration;

/// <summary>
/// Configuración tipada para los servicios de decodificación VIN y consulta de patentes.
/// </summary>
public sealed class VinDecoderSettings
{
    public const string SectionName = "VinDecoder";

    /// <summary>
    /// Configuración específica del proveedor GetAPI.cl.
    /// </summary>
    public GetApiSettings GetApi { get; init; } = new();

    /// <summary>
    /// Permite habilitar o deshabilitar las consultas a GetAPI.cl sin cambiar código.
    /// </summary>
    public bool EnableGetApi { get; init; }

    public sealed class GetApiSettings
    {
        /// <summary>
        /// Clave API utilizada para autenticarse ante GetAPI.cl. Debe inyectarse vía secretos o variables de entorno.
        /// </summary>
        public string? ApiKey { get; init; }

        /// <summary>
        /// Punto base de la API de consulta por patente.
        /// </summary>
        public string BaseUrl { get; init; } = "https://chile.getapi.cl/v1/vehicles";
    }
}
