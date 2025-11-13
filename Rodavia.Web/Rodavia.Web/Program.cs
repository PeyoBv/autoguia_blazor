using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Rodavia.Web.Client.Pages;
using Rodavia.Web.Components;
using Rodavia.Web.Components.Account;
using Rodavia.Web.Data;
using Rodavia.Web.Configuration;
using Rodavia.Infrastructure.Data;
using Rodavia.Infrastructure.Services;
using Rodavia.Infrastructure.ExternalServices;
using Rodavia.Infrastructure.Configuration;
using Rodavia.Infrastructure.Validation;
using Rodavia.Infrastructure.Caching;
using Rodavia.Infrastructure.RateLimiting;
using Rodavia.Infrastructure.Middleware;
using AspNetCoreRateLimit;
using Rodavia.Infrastructure.Repositories;
using Rodavia.Infrastructure.Data.Seeders;
using Rodavia.Core.Interfaces;
using Rodavia.Core.DTOs;
using Rodavia.Core.Entities;
using Rodavia.Scraper.Scrapers;
using Rodavia.Web.Services;
using Rodavia.Web.Services.Payments;
using Rodavia.Infrastructure.Services.Payments;
using FluentValidation;
using Serilog;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

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

// ✅ Configurar API Controllers con JSON case-insensitive para Transbank
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? string.Empty;
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? string.Empty;
        googleOptions.CallbackPath = "/signin-google";
        
        // ✨ Configurar scopes para obtener información del perfil
        googleOptions.Scope.Add("profile");
        googleOptions.Scope.Add("email");
        
        // ✨ Guardar tokens para uso posterior
        googleOptions.SaveTokens = true;
        
        // ✨ Configurar eventos para debugging (solo desarrollo)
        if (builder.Environment.IsDevelopment())
        {
            googleOptions.Events.OnCreatingTicket = context =>
            {
                Console.WriteLine($"✅ Google login exitoso para: {context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value}");
                return Task.CompletedTask;
            };
        }
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


// ⚙️ Configuración de Base de Datos - InMemory para Desarrollo Rápido
Console.WriteLine("✅ Configurando base de datos InMemory:");
Console.WriteLine($"   Identity DB: RodaviaIdentityDb (InMemory)");
Console.WriteLine($"   Rodavia DB: RodaviaDb (InMemory)");

// ✅ Configurar DbContexts con InMemory Database para desarrollo rápido
// Identity en base de datos InMemory dedicada con pooling
builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
{
    options.UseInMemoryDatabase("RodaviaIdentityDb");
}, poolSize: 128); // Pool size optimizado para Identity (autenticación concurrente)

// Rodavia en base de datos separada con pooling optimizado
builder.Services.AddDbContextPool<RodaviaDbContext>(options =>
{
    options.UseInMemoryDatabase("RodaviaDb");
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
    client.DefaultRequestHeaders.Add("User-Agent", "Rodavia-VinDecoder/1.0");
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
//     options.InstanceName = "Rodavia:";
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

// Registrar servicios de Rodavia
builder.Services.AddScoped<ITallerService, TallerService>();
builder.Services.AddScoped<IForoService, ForoService>();
builder.Services.AddScoped<IMapService, GoogleMapService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IProductoService, ProductoService>();

// ✨ Servicio de sanitización HTML para protección XSS
builder.Services.AddScoped<IHtmlSanitizationService, HtmlSanitizationService>();

// 💳 Servicio de gestión de suscripciones
builder.Services.AddScoped<ISuscripcionService, SuscripcionService>();

// 💰 Servicios de Pagos con Transbank
builder.Services.AddHttpClient(); // Requerido para TransbankGateway
builder.Services.AddScoped<global::Rodavia.Infrastructure.Services.Payments.ITransbankGateway, global::Rodavia.Web.Services.Payments.TransbankGateway>();
builder.Services.AddScoped<global::Rodavia.Infrastructure.Services.Payments.ISubscriptionBillingService, global::Rodavia.Web.Services.Payments.SubscriptionBillingService>();

// 🔄 Background Service para facturación automática de suscripciones
builder.Services.AddHostedService<global::Rodavia.Web.Services.SubscriptionBillingBackgroundService>();

// 🤖 Servicio de diagnóstico con IA de Gemini
builder.Services.AddScoped<IGeminiService, GeminiService>();

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
builder.Services.AddScoped<IComparadorService, Rodavia.Web.Services.ComparadorServiceWithScrapers>();

// 🛒 Servicios de Scraping de Consumibles Automotrices
builder.Services.AddScoped<ConsumiblesScraperService>();              // MercadoLibre (único activo)
// builder.Services.AddScoped<AutoplanetConsumiblesScraperService>();    // Autoplanet (deshabilitado)
// builder.Services.AddScoped<MundoRepuestosConsumiblesScraperService>(); // MundoRepuestos (deshabilitado)

// 🌐 HttpClient para scrapers de consumibles
builder.Services.AddHttpClient("ConsumiblesScraperClient", client =>
{
    client.DefaultRequestHeaders.Add("User-Agent", "Rodavia-Scraper/2.0");
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

builder.Services.AddIdentityCore<ApplicationUser>(options => 
{
    // En desarrollo, permitir login sin confirmar email
    options.SignIn.RequireConfirmedAccount = !builder.Environment.IsDevelopment();
    options.SignIn.RequireConfirmedEmail = !builder.Environment.IsDevelopment();
})
    .AddRoles<IdentityRole>() // Habilitar roles
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

// Agregar RoleManager para gestión de roles
builder.Services.AddScoped<RoleManager<IdentityRole>>();

// ✅ Servicios de Email
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEmailSender<ApplicationUser>, IdentityEmailSender>();

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
    .AddAdditionalAssemblies(typeof(Rodavia.Web.Client._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

// ✅ Mapear API Controllers (incluye AccountController para OAuth)
app.MapControllers();

// =============================================================================

// ✅ PLAN DE ACCIÓN: Inicialización completa de base de datos
// 1. Aplicar migraciones automáticamente
// 2. Poblar datos iniciales de Identity
using (var scope = app.Services.CreateScope())
{
    try
    {
        // Paso 1: Crear base de datos de Identity (InMemory)
        var identityContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await identityContext.Database.EnsureCreatedAsync();

        // Paso 2: Crear base de datos de Rodavia (InMemory)
        var rodaviaContext = scope.ServiceProvider.GetRequiredService<RodaviaDbContext>();
        await rodaviaContext.Database.EnsureCreatedAsync();

        // Paso 3: Ejecutar seeding de datos (Identity + Aplicación)
        await DataSeeder.SeedData(app.Services);

        // Paso 4: Ejecutar seeding de planes de suscripción
        await PlanesSeeder.SeedPlanesAsync(identityContext);

        // Paso 5: Ejecutar seeding del módulo de diagnóstico
        DiagnosticoSeeder.SeedDiagnosticoData(rodaviaContext);

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
    Log.Information("🚀 Iniciando Rodavia aplicación web");
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
