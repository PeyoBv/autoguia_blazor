using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
using AutoGuia.Core.DTOs;
using AutoGuia.Core.Entities;
using AutoGuia.Scraper.Scrapers;
using AutoGuia.Web.Services;
using Serilog;

// ✨ Configurar Serilog ANTES de crear el builder
SerilogConfiguration.ConfigureSerilog(new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
    .Build());

var builder = WebApplication.CreateBuilder(args);

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

var identityConnectionString = builder.Configuration.GetConnectionString("IdentityConnection") ?? 
    builder.Configuration.GetConnectionString("DefaultConnection") ?? 
    throw new InvalidOperationException("Connection string 'DefaultConnection' o 'IdentityConnection' not found.");
var autoGuiaConnectionString = builder.Configuration.GetConnectionString("AutoGuiaConnection") ?? identityConnectionString;

// Configurar bases de datos separadas para Identity y AutoGuía
Console.WriteLine("✅ Configurando bases de datos separadas:");
Console.WriteLine($"   Identity DB: Puerto 5434 - identity_dev");
Console.WriteLine($"   AutoGuía DB: Puerto 5433 - autoguia_dev");

// Identity en base de datos dedicada
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(identityConnectionString));

// AutoGuía en base de datos separada
builder.Services.AddDbContext<AutoGuiaDbContext>(options =>
    options.UseNpgsql(autoGuiaConnectionString));



// Configurar Google Maps
builder.Services.Configure<GoogleMapsOptions>(builder.Configuration.GetSection(GoogleMapsOptions.SectionName));

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
