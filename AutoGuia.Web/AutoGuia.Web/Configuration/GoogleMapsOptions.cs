namespace AutoGuia.Web.Configuration
{
    /// <summary>
    /// Configuración para la integración con Google Maps Platform
    /// </summary>
    public class GoogleMapsOptions
    {
        public const string SectionName = "GoogleMaps";

        /// <summary>
        /// Clave de API de Google Maps Platform
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// Región por defecto para el mapa (ISO 3166-1 alpha-2)
        /// </summary>
        public string Region { get; set; } = "CL"; // Chile por defecto

        /// <summary>
        /// Idioma por defecto para el mapa
        /// </summary>
        public string Language { get; set; } = "es"; // Español por defecto

        /// <summary>
        /// Centro por defecto del mapa (Santiago, Chile)
        /// </summary>
        public MapCenter DefaultCenter { get; set; } = new()
        {
            Latitude = -33.4489,
            Longitude = -70.6693
        };

        /// <summary>
        /// Zoom por defecto del mapa
        /// </summary>
        public int DefaultZoom { get; set; } = 12;
    }

    /// <summary>
    /// Configuración del centro del mapa
    /// </summary>
    public class MapCenter
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}