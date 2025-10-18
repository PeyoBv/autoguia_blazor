using Microsoft.Extensions.DependencyInjection;
using AutoGuia.Scraper.Interfaces;
using AutoGuia.Scraper.Services;

namespace AutoGuia.Scraper.Extensions;

/// <summary>
/// Métodos de extensión para registrar servicios del proyecto Scraper en el contenedor DI
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra todos los servicios necesarios del proyecto AutoGuia.Scraper
    /// </summary>
    /// <param name="services">Colección de servicios</param>
    /// <param name="excludePlaywright">Si es true, excluye los scrapers de Playwright (requieren instalación adicional)</param>
    /// <returns>La misma colección de servicios para encadenamiento</returns>
    public static IServiceCollection AddScraperServices(
        this IServiceCollection services, 
        bool excludePlaywright = true)
    {
        // 1. Registrar HttpClient y MemoryCache (requeridos por scrapers)
        services.AddHttpClient();
        services.AddMemoryCache();

        // 2. Registrar servicios core del proyecto Scraper
        // IMPORTANTE: ScraperOrchestratorService espera el tipo CONCRETO, no la interfaz
        services.AddScoped<OfertaUpdateService>(); // Tipo concreto primero
        services.AddScoped<IOfertaUpdateService>(sp => sp.GetRequiredService<OfertaUpdateService>()); // Interfaz apunta al concreto
        services.AddScoped<ScraperOrchestratorService>();

        // 3. Auto-registrar todas las implementaciones de IScraper
        var scraperTypes = typeof(IScraper).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IScraper).IsAssignableFrom(t))
            .Where(t => !excludePlaywright || !t.Name.Contains("Playwright"))
            .ToList();

        Console.WriteLine($"🔧 [ScraperServices] Registrando {scraperTypes.Count} scrapers:");
        foreach (var scraperType in scraperTypes)
        {
            services.AddScoped(typeof(IScraper), scraperType);
            Console.WriteLine($"   ✅ {scraperType.Name}");
        }

        Console.WriteLine($"✅ [ScraperServices] Todos los servicios de scraping registrados correctamente");

        return services;
    }
}
