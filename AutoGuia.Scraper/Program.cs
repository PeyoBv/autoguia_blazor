using AutoGuia.Infrastructure.Data;
using AutoGuia.Scraper.Interfaces;
using AutoGuia.Scraper.Models;
using AutoGuia.Scraper.Services;
using AutoGuia.Scraper.Workers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

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
            else if (args.Length > 0 && args[0].Equals("--test-ml", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("🧪 Ejecutando prueba de MercadoLibre...");
                await EjecutarPruebaMercadoLibre(host);
            }
            else if (args.Length > 0 && args[0].Equals("--test-autoplanet", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("🧪 Ejecutando prueba de Autoplanet...");
                await EjecutarPruebaAutoplanet(host);
            }
            else if (args.Length > 0 && args[0].Equals("--test-mundorepuestos", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("🧪 Ejecutando prueba de MundoRepuestos...");
                await EjecutarPruebaMundoRepuestos(host);
            }
            else if (args.Length > 0 && args[0].Equals("--test-playwright", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("🎭 Ejecutando PRUEBA DE FUEGO con Playwright...");
                await EjecutarPruebaPlaywright(host);
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

        // 🎯 Registrar configuraciones desde appsettings.json
        Console.WriteLine("📋 Registrando configuraciones de scraping...");
        services.Configure<ScrapingSettings>(configuration.GetSection("ScrapingSettings"));
        services.Configure<List<ScrapingTarget>>(configuration.GetSection("ScrapingTargets"));
        
        // Mostrar configuraciones cargadas
        var scrapingTargets = configuration.GetSection("ScrapingTargets").Get<List<ScrapingTarget>>();
        if (scrapingTargets != null && scrapingTargets.Any())
        {
            Console.WriteLine($"📊 Configuradas {scrapingTargets.Count} páginas objetivo:");
            foreach (var target in scrapingTargets.Where(t => t.EsActivo))
            {
                Console.WriteLine($"   ✅ {target.TiendaNombre}: {target.Url} ({target.Categoria})");
            }
        }
        else
        {
            Console.WriteLine("⚠️  No se encontraron targets de scraping configurados");
        }

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
        
        // 🎯 REGISTRO AUTOMÁTICO: Buscar y registrar todos los scrapers que implementen IScraper
        RegistrarScrapersAutomaticamente(services);
        
        // Servicio principal de scraping para RepuestosTotal (nueva implementación con parsing real)
        services.AddTransient<IScraperService, RepuestosTotalScraperService>();
        
        // Servicio para actualizar ofertas en la base de datos
        services.AddScoped<IOfertaUpdateService, OfertaUpdateService>();
        services.AddScoped<OfertaUpdateService>(); // Registro adicional sin interfaz
        
        // Servicio para procesar y validar datos scrapeados (temporalmente deshabilitado)
        // services.AddTransient<IDataProcessingService, DataProcessingService>();

        // Servicio de pruebas del scraper
        services.AddTransient<ScraperTestService>();
        
        // Servicio para inicializar datos semilla
        services.AddScoped<ScraperDataSeederService>();
        
        // Servicio de prueba completa integrada
        services.AddTransient<IntegratedScrapingTestService>();
        
        // Servicio de prueba específico para MercadoLibre
        services.AddTransient<MercadoLibreTestService>();
        
        // Servicio de prueba específico para Autoplanet
        services.AddTransient<AutoplanetTestService>();
        
        // Servicio de prueba específico para MundoRepuestos
        services.AddTransient<MundoRepuestosTestService>();
        
        // Servicio de prueba para Playwright (Prueba de Fuego)
        services.AddTransient<AutoplanetPlaywrightTestService>();
        
        // 🎯 Servicio orquestador para coordinar múltiples scrapers
        services.AddScoped<ScraperOrchestratorService>();

        // 4️⃣ Registrar el Worker principal como servicio hospedado
        Console.WriteLine("⚙️  Registrando worker de segundo plano...");
        services.AddHostedService<ScraperWorker>();

        // 5️⃣ Registrar configuraciones tipadas
        services.Configure<ScrapingSettings>(configuration.GetSection("ScrapingSettings"));
        services.Configure<StoreSettings>(configuration.GetSection("Stores"));

        Console.WriteLine("✅ Todos los servicios configurados correctamente");
    }

    /// <summary>
    /// Registra automáticamente todos los scrapers que implementan IScraper usando reflexión.
    /// Esto permite agregar nuevos scrapers sin modificar Program.cs.
    /// </summary>
    /// <param name="services">Colección de servicios</param>
    private static void RegistrarScrapersAutomaticamente(IServiceCollection services)
    {
        Console.WriteLine("🔍 Buscando implementaciones de IScraper...");
        
        try
        {
            // Obtener el ensamblado actual (AutoGuia.Scraper)
            var assembly = Assembly.GetExecutingAssembly();
            
            // Buscar todos los tipos que implementan IScraper
            var scraperTypes = assembly.GetTypes()
                .Where(type => 
                    type.IsClass &&                          // Es una clase
                    !type.IsAbstract &&                      // No es abstracta
                    typeof(IScraper).IsAssignableFrom(type)) // Implementa IScraper
                .ToList();

            if (!scraperTypes.Any())
            {
                Console.WriteLine("⚠️  No se encontraron implementaciones de IScraper");
                return;
            }

            // Registrar cada scraper encontrado
            foreach (var scraperType in scraperTypes)
            {
                services.AddTransient(typeof(IScraper), scraperType);
                Console.WriteLine($"   ✅ Registrado: {scraperType.Name}");
            }

            Console.WriteLine($"✅ Se registraron {scraperTypes.Count} scrapers automáticamente");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error al registrar scrapers automáticamente: {ex.Message}");
            throw;
        }
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

    /// <summary>
    /// Ejecuta una prueba específica del scraper de MercadoLibre.
    /// </summary>
    private static async Task EjecutarPruebaMercadoLibre(IHost host)
    {
        using var scope = host.Services.CreateScope();
        var testService = scope.ServiceProvider.GetRequiredService<MercadoLibreTestService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            var resultado = await testService.EjecutarPruebaCompleta();

            if (resultado.Exitoso)
            {
                Console.WriteLine("\n🎉 ¡Prueba de MercadoLibre completada exitosamente!");
                Console.WriteLine($"📊 Total de ofertas encontradas: {resultado.TotalOfertas}");
            }
            else
            {
                Console.WriteLine("\n❌ La prueba de MercadoLibre falló.");
                if (!string.IsNullOrEmpty(resultado.MensajeError))
                {
                    Console.WriteLine($"   Error: {resultado.MensajeError}");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Error ejecutando la prueba de MercadoLibre");
            Console.WriteLine($"❌ Error inesperado: {ex.Message}");
        }

        Console.WriteLine("\nPresione cualquier tecla para salir...");
        Console.ReadKey();
    }

    /// <summary>
    /// Ejecuta una prueba específica del scraper de Autoplanet.
    /// </summary>
    private static async Task EjecutarPruebaAutoplanet(IHost host)
    {
        using var scope = host.Services.CreateScope();
        var testService = scope.ServiceProvider.GetRequiredService<AutoplanetTestService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            var resultado = await testService.EjecutarPruebaCompleta();

            if (resultado.Exitoso)
            {
                Console.WriteLine("\n🎉 ¡Prueba de Autoplanet completada exitosamente!");
                Console.WriteLine($"📊 Total de ofertas encontradas: {resultado.TotalOfertas}");
            }
            else
            {
                Console.WriteLine("\n❌ La prueba de Autoplanet falló.");
                if (!string.IsNullOrEmpty(resultado.MensajeError))
                {
                    Console.WriteLine($"   Error: {resultado.MensajeError}");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Error ejecutando la prueba de Autoplanet");
            Console.WriteLine($"❌ Error inesperado: {ex.Message}");
        }

        Console.WriteLine("\nPresione cualquier tecla para salir...");
        Console.ReadKey();
    }

    /// <summary>
    /// Ejecuta una prueba específica del scraper de MundoRepuestos.
    /// </summary>
    private static async Task EjecutarPruebaMundoRepuestos(IHost host)
    {
        using var scope = host.Services.CreateScope();
        var testService = scope.ServiceProvider.GetRequiredService<MundoRepuestosTestService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            var resultado = await testService.EjecutarPruebaCompleta();

            if (resultado.Exitoso)
            {
                Console.WriteLine("\n🎉 ¡Prueba de MundoRepuestos completada exitosamente!");
                Console.WriteLine($"📊 Total de ofertas encontradas: {resultado.TotalOfertas}");
            }
            else
            {
                Console.WriteLine("\n❌ La prueba de MundoRepuestos falló.");
                if (!string.IsNullOrEmpty(resultado.MensajeError))
                {
                    Console.WriteLine($"   Error: {resultado.MensajeError}");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Error ejecutando la prueba de MundoRepuestos");
            Console.WriteLine($"❌ Error inesperado: {ex.Message}");
        }

        Console.WriteLine("\nPresione cualquier tecla para salir...");
        Console.ReadKey();
    }

    /// <summary>
    /// Ejecuta la prueba de fuego con Playwright para sitios SPA.
    /// </summary>
    private static async Task EjecutarPruebaPlaywright(IHost host)
    {
        using var scope = host.Services.CreateScope();
        var testService = scope.ServiceProvider.GetRequiredService<AutoplanetPlaywrightTestService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            var resultado = await testService.EjecutarPruebaCompleta();

            if (resultado.Exitoso)
            {
                Console.WriteLine("\n🎉 ¡Prueba de Playwright completada exitosamente!");
                Console.WriteLine($"📊 Total de ofertas encontradas: {resultado.TotalOfertas}");
                Console.WriteLine($"⏱️  Tiempo de scraping: {resultado.DuracionScraping.TotalSeconds:F2}s");
            }
            else
            {
                Console.WriteLine("\n❌ La prueba de Playwright falló.");
                if (!string.IsNullOrEmpty(resultado.MensajeError))
                {
                    Console.WriteLine($"   Error: {resultado.MensajeError}");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Error ejecutando la prueba de Playwright");
            Console.WriteLine($"❌ Error inesperado: {ex.Message}");
        }

        Console.WriteLine("\nPresione cualquier tecla para salir...");
        Console.ReadKey();
    }
}
