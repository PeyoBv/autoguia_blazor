using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using AutoGuia.Web.Client.Pages;
using AutoGuia.Web.Components;
using AutoGuia.Web.Components.Account;
using AutoGuia.Web.Data;
using AutoGuia.Web.Configuration;
using AutoGuia.Infrastructure.Data;
using AutoGuia.Infrastructure.Services;
using AutoGuia.Infrastructure.ExternalServices;
using AutoGuia.Infrastructure.Configuration;
using AutoGuia.Infrastructure.Validation;
using AutoGuia.Infrastructure.Caching;
using AutoGuia.Infrastructure.RateLimiting;
using AutoGuia.Infrastructure.Middleware;
using AspNetCoreRateLimit;
using AutoGuia.Infrastructure.Repositories;
using AutoGuia.Infrastructure.Data.Seeders;
using AutoGuia.Infrastructure.Services;
using AutoGuia.Core.Interfaces;
using AutoGuia.Core.DTOs;
using AutoGuia.Core.Entities;
using AutoGuia.Scraper.Scrapers;
using AutoGuia.Web.Services;
using FluentValidation;
using Serilog;

// ✨ Configurar Serilog ANTES de crear el builder
SerilogConfiguration.ConfigureSerilog(new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
    .Build());

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>(optional: true);
}

builder.Configuration.AddEnvironmentVariables();

// ✨ Usar Serilog como proveedor de logging
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

// ✅ Configurar protección CSRF
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.Cookie.Name = "X-CSRF-TOKEN-COOKIE";
    options.Cookie.HttpOnly = true;
    // En desarrollo, permitir HTTP para testing
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment() 
        ? CookieSecurePolicy.SameAsRequest 
        : CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

var identityConnectionString = builder.Configuration.GetConnectionString("IdentityConnection") ?? 
    builder.Configuration.GetConnectionString("DefaultConnection") ?? 
    throw new InvalidOperationException("Connection string 'DefaultConnection' o 'IdentityConnection' not found.");
var autoGuiaConnectionString = builder.Configuration.GetConnectionString("AutoGuiaConnection") ?? identityConnectionString;

// Configurar bases de datos separadas para Identity y AutoGuía
Console.WriteLine("✅ Configurando bases de datos separadas:");
Console.WriteLine($"   Identity DB: Puerto 5434 - identity_dev");
Console.WriteLine($"   AutoGuía DB: Puerto 5433 - autoguia_dev");

// Identity en base de datos dedicada con pooling
builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
{
    options.UseNpgsql(identityConnectionString);
}, poolSize: 128); // Pool size optimizado para Identity (autenticación concurrente)

// AutoGuía en base de datos separada con pooling optimizado
builder.Services.AddDbContextPool<AutoGuiaDbContext>(options =>
{
    options.UseNpgsql(autoGuiaConnectionString);
    // Configuraciones adicionales para producción
    if (!builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging(false);
        options.EnableDetailedErrors(false);
    }
}, poolSize: 256); // Pool size mayor para operaciones de negocio



// Configurar Google Maps
builder.Services.Configure<GoogleMapsOptions>(builder.Configuration.GetSection(GoogleMapsOptions.SectionName));
builder.Services.Configure<VinDecoderSettings>(builder.Configuration.GetSection(VinDecoderSettings.SectionName));

// ✨ Registrar HttpClient para NHTSA VIN Decoder API
builder.Services.AddHttpClient("NHTSA_API", client =>
{
    client.BaseAddress = new Uri("https://vpic.nhtsa.dot.gov/api/");
    client.DefaultRequestHeaders.Add("User-Agent", "AutoGuia-VinDecoder/1.0");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// ✨ Configurar HttpClients con políticas de resiliencia (Polly)
builder.Services.AddResilientHttpClients(builder.Configuration);

// ✨ Agregar Memory Cache para optimización
builder.Services.AddMemoryCache();

// ✨ Configurar AspNetCoreRateLimit
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// ✨ Configurar Distributed Cache (Redis) - Comentado para desarrollo
// builder.Services.AddStackExchangeRedisCache(options =>
// {
//     options.Configuration = builder.Configuration.GetConnectionString("Redis");
//     options.InstanceName = "AutoGuia:";
// });

// ✨ Registrar Servicio de Caché Unificado
builder.Services.AddScoped<ICacheService, MemoryCacheService>();
// Para producción con Redis:
// builder.Services.AddScoped<ICacheService, DistributedCacheService>();

// ✨ Configurar FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CrearTallerDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CrearPublicacionDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CrearProductoDtoValidator>();

// ✨ Configurar Rate Limiting
builder.Services.AddCustomRateLimiting();

// ✨ Registrar servicios de APIs externas (MercadoLibre, eBay)
builder.Services.AddScoped<IExternalMarketplaceService, MercadoLibreService>();
builder.Services.AddScoped<IExternalMarketplaceService, EbayService>();
builder.Services.AddScoped<ComparadorAgregadoService>();

// Registrar servicios de AutoGuía
builder.Services.AddScoped<ITallerService, TallerService>();
builder.Services.AddScoped<IForoService, ForoService>();
builder.Services.AddScoped<IMapService, GoogleMapService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IProductoService, ProductoService>();

// ✨ Servicio de sanitización HTML para protección XSS
builder.Services.AddScoped<IHtmlSanitizationService, HtmlSanitizationService>();

// 🔧 Registrar repositorios del módulo de diagnóstico
builder.Services.AddScoped<ISintomaRepository, SintomaRepository>();
builder.Services.AddScoped<ICausaPosibleRepository, CausaPosibleRepository>();
builder.Services.AddScoped<IConsultaDiagnosticoRepository, ConsultaDiagnosticoRepository>();
builder.Services.AddScoped<ISistemaAutomotrizRepository, SistemaAutomotrizRepository>();

// 🩺 Registrar servicios del módulo de diagnóstico
builder.Services.AddScoped<IDiagnosticoService, DiagnosticoService>();
builder.Services.AddScoped<ISistemaAutomotrizService, SistemaAutomotrizService>();
builder.Services.AddScoped<SintomaSearchService>();

// Registrar ComparadorService base y luego el wrapper con scrapers
builder.Services.AddScoped<ComparadorService>();
builder.Services.AddScoped<IComparadorService, AutoGuia.Web.Services.ComparadorServiceWithScrapers>();

// 🛒 Servicios de Scraping de Consumibles Automotrices
builder.Services.AddScoped<ConsumiblesScraperService>();              // MercadoLibre
builder.Services.AddScoped<AutoplanetConsumiblesScraperService>();    // Autoplanet
builder.Services.AddScoped<MundoRepuestosConsumiblesScraperService>(); // MundoRepuestos

// 🌐 HttpClient para scrapers de consumibles
builder.Services.AddHttpClient("ConsumiblesScraperClient", client =>
{
    client.DefaultRequestHeaders.Add("User-Agent", "AutoGuia-Scraper/2.0");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Servicio de vehículos (solo Marca y Modelo)
// builder.Services.AddScoped<IVehiculoService, VehiculoService>();

// ✨ Servicios de información vehicular (VIN y Patente) con arquitectura compuesta
// Registramos las implementaciones concretas primero
builder.Services.AddScoped<NhtsaVinService>();         // VIN → NHTSA (gratuito)
builder.Services.AddScoped<GetApiPatenteService>();    // Patente → GetAPI.cl (premium)

// Luego registramos el servicio compuesto como la implementación de la interfaz
// Este servicio orquesta:
//   - Para VINs: NHTSA (proveedor principal)
//   - Para Patentes Chilenas: GetAPI.cl (proveedor único)
builder.Services.AddScoped<IVehiculoInfoService, CompositeVehiculoInfoService>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() // Habilitar roles
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

// Agregar RoleManager para gestión de roles
builder.Services.AddScoped<RoleManager<IdentityRole>>();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// ✨ Aplicar headers de seguridad (XSS, Clickjacking, MIME sniffing, etc.)
app.UseSecurityHeaders();

app.UseAuthentication();
app.UseAuthorization();

// ✨ Usar AspNetCoreRateLimit (DEBE ir después de UseAuthentication)
app.UseIpRateLimiting();

// ✨ Usar Rate Limiting personalizado
app.UseCustomRateLimiting();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(AutoGuia.Web.Client._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

// ✅ PLAN DE ACCIÓN: Inicialización completa de base de datos
// 1. Aplicar migraciones automáticamente
// 2. Poblar datos iniciales de Identity
using (var scope = app.Services.CreateScope())
{
    try
    {
        // Paso 1: Aplicar migraciones de Identity
        var identityContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await identityContext.Database.MigrateAsync();
        
        // Paso 2: Aplicar migraciones de AutoGuía
        var autoGuiaContext = scope.ServiceProvider.GetRequiredService<AutoGuiaDbContext>();
        await autoGuiaContext.Database.MigrateAsync();
        
        // Paso 3: Ejecutar seeding de datos (Identity + Aplicación)
        await DataSeeder.SeedData(app.Services);
        
        // Paso 4: Ejecutar seeding del módulo de diagnóstico
        DiagnosticoSeeder.SeedDiagnosticoData(autoGuiaContext);
        
        Console.WriteLine("✅ Base de datos inicializada correctamente con datos de prueba");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️ Error en inicialización de BD: {ex.Message}");
        Console.WriteLine("✅ La aplicación continuará ejecutándose...");
    }
}

try
{
    Log.Information("🚀 Iniciando AutoGuía aplicación web");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "❌ Aplicación terminó inesperadamente");
}
finally
{
    Log.CloseAndFlush();
}

// ✅ Hacer Program accesible para tests de integración
public partial class Program { }
