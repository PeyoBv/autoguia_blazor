using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AutoGuia.Infrastructure.Data;
using AutoGuia.Core.Entities;
using AutoGuia.Web.Data;

namespace AutoGuia.Web.Data;

/// <summary>
/// Clase estática responsable de poblar la base de datos con datos iniciales
/// </summary>
public static class DataSeeder
{
    /// <summary>
    /// Método principal para sembrar datos en la base de datos
    /// </summary>
    public static async Task SeedData(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var identityContext = services.GetRequiredService<ApplicationDbContext>();
            var autoguiaContext = services.GetRequiredService<AutoGuiaDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            await identityContext.Database.EnsureCreatedAsync();
            await autoguiaContext.Database.EnsureCreatedAsync();

            await SeedIdentityData(userManager, roleManager);
            await SeedApplicationData(autoguiaContext);

            Console.WriteLine("✅ Datos semilla aplicados correctamente");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error al aplicar datos semilla: {ex.Message}");
            throw;
        }
    }

    private static async Task SeedIdentityData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        await CreateRoleIfNotExists(roleManager, "Admin");
        await CreateRoleIfNotExists(roleManager, "Usuario");

        var adminEmail = "admin@autoguia.cl";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                Console.WriteLine($"✅ Usuario administrador creado: {adminEmail}");
            }
        }
    }

    private static async Task CreateRoleIfNotExists(RoleManager<IdentityRole> roleManager, string roleName)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
            Console.WriteLine($"✅ Rol creado: {roleName}");
        }
    }

    private static async Task SeedApplicationData(AutoGuiaDbContext context)
    {
        Console.WriteLine("🌱 Iniciando seeding de datos de AutoGuía...");

        // Sembrar Marcas
        if (!await context.Marcas.AnyAsync())
        {
            Console.WriteLine("📦 Sembrando Marcas...");
            var marcas = new List<Marca>
            {
                new() { Nombre = "Toyota", LogoUrl = "/images/marcas/toyota.png", EsActivo = true, FechaCreacion = DateTime.UtcNow },
                new() { Nombre = "Honda", LogoUrl = "/images/marcas/honda.png", EsActivo = true, FechaCreacion = DateTime.UtcNow },
                new() { Nombre = "Nissan", LogoUrl = "/images/marcas/nissan.png", EsActivo = true, FechaCreacion = DateTime.UtcNow },
                new() { Nombre = "Chevrolet", LogoUrl = "/images/marcas/chevrolet.png", EsActivo = true, FechaCreacion = DateTime.UtcNow },
                new() { Nombre = "Ford", LogoUrl = "/images/marcas/ford.png", EsActivo = true, FechaCreacion = DateTime.UtcNow }
            };
            
            await context.Marcas.AddRangeAsync(marcas);
            await context.SaveChangesAsync();
            Console.WriteLine($"   ✅ {marcas.Count} marcas creadas");
        }

        // Sembrar Talleres
        if (!await context.Talleres.AnyAsync())
        {
            Console.WriteLine("🔧 Sembrando Talleres...");
            var talleres = new List<Taller>
            {
                new() {
                    Nombre = "Taller Mecánico Central",
                    Ciudad = "Santiago",
                    Region = "Metropolitana",
                    Direccion = "Av. Providencia 1234, Santiago",
                    Telefono = "+56 2 2345 6789",
                    Email = "contacto@tallercentral.cl",
                    Descripcion = "Taller mecánico con más de 20 años de experiencia",
                    HorarioAtencion = "Lun-Vie: 9:00-19:00, Sáb: 9:00-14:00",
                    Latitud = -33.4372,
                    Longitud = -70.6506,
                    CalificacionPromedio = 4.5m,
                    EsActivo = true,
                    FechaRegistro = DateTime.UtcNow
                },
                new() {
                    Nombre = "AutoService Express",
                    Ciudad = "Las Condes",
                    Region = "Metropolitana",
                    Direccion = "Av. Las Condes 9876, Las Condes",
                    Telefono = "+56 2 3456 7890",
                    Email = "info@autoserviceexpress.cl",
                    Descripcion = "Servicio rápido y de calidad para tu vehículo",
                    HorarioAtencion = "Lun-Vie: 8:00-20:00, Sáb: 9:00-15:00",
                    Latitud = -33.4167,
                    Longitud = -70.6,
                    CalificacionPromedio = 4.8m,
                    EsActivo = true,
                    FechaRegistro = DateTime.UtcNow
                }
            };

            await context.Talleres.AddRangeAsync(talleres);
            await context.SaveChangesAsync();
            Console.WriteLine($"   ✅ {talleres.Count} talleres creados");
        }

        Console.WriteLine("✅ Seeding de AutoGuía completado");
    }
}
