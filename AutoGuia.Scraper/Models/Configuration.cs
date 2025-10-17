namespace AutoGuia.Scraper.Models;

/// <summary>
/// Configuración general del sistema de scraping.
/// Mapeada desde la sección "ScrapingSettings" del appsettings.json.
/// </summary>
public class ScrapingSettings
{
    /// <summary>
    /// Intervalo en minutos entre ejecuciones del scraper.
    /// </summary>
    public int IntervalInMinutes { get; set; } = 60;

    /// <summary>
    /// Número máximo de reintentos en caso de error.
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Timeout en segundos para las requests HTTP.
    /// </summary>
    public int TimeoutInSeconds { get; set; } = 30;

    /// <summary>
    /// User-Agent que se enviará en las requests HTTP.
    /// </summary>
    public string UserAgent { get; set; } = "AutoGuia-Scraper/1.0";

    /// <summary>
    /// Indica si el scraping está habilitado globalmente.
    /// </summary>
    public bool Enabled { get; set; } = true;
}

/// <summary>
/// Configuración de todas las tiendas disponibles para scraping.
/// Mapeada desde la sección "Stores" del appsettings.json.
/// </summary>
public class StoreSettings
{
    /// <summary>
    /// Configuración específica para RepuestosTotal.
    /// </summary>
    public StoreConfig RepuestosTotal { get; set; } = new();

    /// <summary>
    /// Diccionario para tiendas adicionales que se puedan agregar dinámicamente.
    /// </summary>
    public Dictionary<string, StoreConfig> Additional { get; set; } = new();
}

/// <summary>
/// Configuración específica para una tienda individual.
/// </summary>
public class StoreConfig
{
    /// <summary>
    /// URL base de la tienda.
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// URL para búsqueda de productos. {0} será reemplazado por el término de búsqueda.
    /// </summary>
    public string ProductSearchUrl { get; set; } = string.Empty;

    /// <summary>
    /// Indica si el scraping está habilitado para esta tienda específica.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Delay en milisegundos entre requests para evitar ser bloqueado.
    /// </summary>
    public int RequestDelayMs { get; set; } = 1000;

    /// <summary>
    /// Configuraciones adicionales específicas de la tienda.
    /// </summary>
    public Dictionary<string, string> CustomSettings { get; set; } = new();
}