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
using AutoGuia.Core.DTOs;
using AutoGuia.Core.Entities;

var builder = WebApplication.CreateBuilder(args);

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

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
var autoGuiaConnectionString = builder.Configuration.GetConnectionString("AutoGuiaConnection") ?? connectionString;

// Configurar ApplicationDbContext (Identity) - SQLite para desarrollo, PostgreSQL para producción
if (connectionString.Contains("Host=") || connectionString.Contains("Server="))
{
    // PostgreSQL para producción
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));
}
else
{
    // SQLite para desarrollo local
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(connectionString));
}

// Configurar AutoGuía DbContext - PostgreSQL para producción, InMemory para desarrollo
if (autoGuiaConnectionString.Contains("Host=") || autoGuiaConnectionString.Contains("Server="))
{
    // PostgreSQL para producción
    builder.Services.AddDbContext<AutoGuiaDbContext>(options =>
        options.UseNpgsql(autoGuiaConnectionString));
}
else
{
    // InMemory para desarrollo local
    builder.Services.AddDbContext<AutoGuiaDbContext>(options =>
        options.UseInMemoryDatabase("AutoGuiaDb"));
}

// Configurar Google Maps
builder.Services.Configure<GoogleMapsOptions>(builder.Configuration.GetSection(GoogleMapsOptions.SectionName));

// Registrar servicios de AutoGuía
builder.Services.AddScoped<ITallerService, TallerService>();
builder.Services.AddScoped<IForoService, ForoService>();
builder.Services.AddScoped<IMapService, GoogleMapService>();

// Servicios del sistema de comparación de precios
builder.Services.AddScoped<IComparadorService, ComparadorService>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<ITiendaService, TiendaService>();
builder.Services.AddScoped<IVehiculoService, VehiculoService>();

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

// Inicializar la base de datos de AutoGuía con datos semilla
// Esto crea automáticamente las tablas y carga los datos de prueba
using (var scope = app.Services.CreateScope())
{
    var autoGuiaContext = scope.ServiceProvider.GetRequiredService<AutoGuiaDbContext>();
    autoGuiaContext.Database.EnsureCreated();
    
    // Inicializar roles y usuario administrador
    await InicializarRolesYAdminAsync(scope.ServiceProvider);
}

app.Run();

/// <summary>
/// Inicializa los roles del sistema y crea el usuario administrador inicial
/// </summary>
static async Task InicializarRolesYAdminAsync(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    
    // Crear rol Admin si no existe
    const string adminRole = "Admin";
    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        await roleManager.CreateAsync(new IdentityRole(adminRole));
    }
    
    // Crear usuario administrador si no existe
    const string adminEmail = "admin@autoguia.cl";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true // Para que pueda iniciar sesión sin confirmación
        };
        
        const string adminPassword = "Admin123!"; // En producción, usar variables de entorno
        var result = await userManager.CreateAsync(adminUser, adminPassword);
        
        if (result.Succeeded)
        {
            // Asignar rol Admin al usuario
            await userManager.AddToRoleAsync(adminUser, adminRole);
            Console.WriteLine($"Usuario administrador creado: {adminEmail} / {adminPassword}");
        }
        else
        {
            Console.WriteLine($"Error creando usuario administrador: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
    else
    {
        // Asegurar que el usuario existente tenga el rol Admin
        if (!await userManager.IsInRoleAsync(adminUser, adminRole))
        {
            await userManager.AddToRoleAsync(adminUser, adminRole);
        }
    }
}
