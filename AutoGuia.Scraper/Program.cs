using AutoGuia.Infrastructure.Data;
using AutoGuia.Scraper.Models;
using AutoGuia.Scraper.Services;
using AutoGuia.Scraper.Workers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AutoGuia.Scraper;

/// <summary>
/// Punto de entrada principal para el sistema de web scraping de AutoGuía.
/// Configura el host genérico con inyección de dependencias y servicios en segundo plano.
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            Console.WriteLine("🚀 Iniciando AutoGuía Scraper...");
            
            // Crear y configurar el host genérico
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // 📋 Configurar servicios de la aplicación
                    ConfigureApplicationServices(services, context.Configuration);
                })
                .ConfigureLogging((context, logging) =>
                {
                    // 📝 Configurar logging específico para el scraper
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddDebug();
                    
                    // Configurar niveles de log desde appsettings.json
                    logging.AddConfiguration(context.Configuration.GetSection("Logging"));
                })
                .UseConsoleLifetime() // Permitir cancelación con Ctrl+C
                .Build();

            Console.WriteLine("✅ Host configurado exitosamente");
            
            // Verificar si estamos en modo de prueba
            if (args.Length > 0 && args[0].Equals("--test", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("🧪 Ejecutando en modo de prueba...");
                await EjecutarPruebaCompleta(host);
            }
            else
            {
                // Ejecutar el host de forma asíncrona (modo normal)
                await host.RunAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error crítico al iniciar el scraper: {ex.Message}");
            Console.WriteLine($"📋 Detalles: {ex}");
            Environment.Exit(1);
        }
    }

    /// <summary>
    /// Configura todos los servicios de la aplicación mediante inyección de dependencias.
    /// </summary>
    /// <param name="services">Colección de servicios</param>
    /// <param name="configuration">Configuración de la aplicación</param>
    private static void ConfigureApplicationServices(IServiceCollection services, IConfiguration configuration)
    {
        Console.WriteLine("🔧 Configurando servicios de la aplicación...");

        // 1️⃣ Registrar DbContext con base de datos InMemory
        Console.WriteLine("🗄️  Configurando base de datos InMemory para scraper");
        services.AddDbContext<AutoGuiaDbContext>(options =>
        {
            options.UseInMemoryDatabase("AutoGuiaScraperDb");
            options.EnableSensitiveDataLogging(); // Solo para desarrollo
        });

        // 2️⃣ Registrar HttpClientFactory para realizar requests HTTP
        services.AddHttpClient();
        
        // Configurar HttpClient específico para scraping con configuraciones por defecto
        services.AddHttpClient("ScraperClient", client =>
        {
            var scrapingSettings = configuration.GetSection("ScrapingSettings");
            client.Timeout = TimeSpan.FromSeconds(scrapingSettings.GetValue<int>("TimeoutInSeconds", 30));
            client.DefaultRequestHeaders.Add("User-Agent", 
                scrapingSettings.GetValue<string>("UserAgent", "AutoGuia-Scraper/1.0"));
        });

        // 3️⃣ Registrar servicios de scraping (interfaces y implementaciones)
        Console.WriteLine("🕷️  Registrando servicios de scraping...");
        
        // Servicio principal de scraping para RepuestosTotal (nueva implementación con parsing real)
        services.AddTransient<IScraperService, RepuestosTotalScraperService>();
        
        // Servicio para actualizar ofertas en la base de datos
        services.AddScoped<IOfertaUpdateService, OfertaUpdateService>();
        
        // Servicio para procesar y validar datos scrapeados (temporalmente deshabilitado)
        // services.AddTransient<IDataProcessingService, DataProcessingService>();

        // Servicio de pruebas del scraper
        services.AddTransient<ScraperTestService>();
        
        // Servicio para inicializar datos semilla
        services.AddScoped<ScraperDataSeederService>();
        
        // Servicio de prueba completa integrada
        services.AddTransient<IntegratedScrapingTestService>();

        // 4️⃣ Registrar el Worker principal como servicio hospedado
        Console.WriteLine("⚙️  Registrando worker de segundo plano...");
        services.AddHostedService<ScraperWorker>();

        // 5️⃣ Registrar configuraciones tipadas
        services.Configure<ScrapingSettings>(configuration.GetSection("ScrapingSettings"));
        services.Configure<StoreSettings>(configuration.GetSection("Stores"));

        Console.WriteLine("✅ Todos los servicios configurados correctamente");
    }

    /// <summary>
    /// Ejecuta una prueba completa del sistema de scraping integrado.
    /// </summary>
    private static async Task EjecutarPruebaCompleta(IHost host)
    {
        using var scope = host.Services.CreateScope();
        var testService = scope.ServiceProvider.GetRequiredService<IntegratedScrapingTestService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("🧪 INICIANDO PRUEBA COMPLETA DEL SISTEMA");
            Console.WriteLine(new string('=', 60));

            var resultado = await testService.EjecutarPruebaCompleta();
            
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("📊 REPORTE DE LA PRUEBA");
            Console.WriteLine(new string('=', 60));

            var reporte = testService.GenerarReporte(resultado);
            Console.WriteLine(reporte);

            if (resultado.Exitoso)
            {
                Console.WriteLine("🎉 ¡Prueba completada exitosamente!");
            }
            else
            {
                Console.WriteLine("❌ La prueba falló. Revise los detalles arriba.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Error ejecutando la prueba completa");
            Console.WriteLine($"❌ Error inesperado: {ex.Message}");
        }

        Console.WriteLine("\nPresione cualquier tecla para salir...");
        Console.ReadKey();
    }
}
