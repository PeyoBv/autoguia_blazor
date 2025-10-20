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
            await SeedCategoriesAsync(autoguiaContext);

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

    /// <summary>
    /// Siembra las categorías de consumibles automotrices con sus subcategorías y valores de filtro
    /// </summary>
    private static async Task SeedCategoriesAsync(AutoGuiaDbContext context)
    {
        try
        {
            Console.WriteLine("🏷️  Iniciando seeding de Categorías...");

            // Validar que no existan datos
            if (await context.Categorias.AnyAsync())
            {
                Console.WriteLine("   ⚠️  Las categorías ya existen, omitiendo seeding");
                return;
            }

            // 1. ACEITES
            Console.WriteLine("   📦 Creando categoría: ACEITES");
            var aceites = new Categoria
            {
                Nombre = "Aceites",
                Descripcion = "Aceites para motor, transmisión y diferencial",
                IconUrl = "fas fa-oil-can",
                EsActivo = true,
                FechaCreacion = DateTime.UtcNow
            };
            await context.Categorias.AddAsync(aceites);
            await context.SaveChangesAsync();

            var aceitesSubcategorias = new List<Subcategoria>
            {
                new() { CategoriaId = aceites.Id, Nombre = "Tipo" },
                new() { CategoriaId = aceites.Id, Nombre = "Viscosidad" },
                new() { CategoriaId = aceites.Id, Nombre = "Marca" },
                new() { CategoriaId = aceites.Id, Nombre = "Tamaño" }
            };
            await context.Subcategorias.AddRangeAsync(aceitesSubcategorias);
            await context.SaveChangesAsync();

            var aceitesValores = new List<ValorFiltro>
            {
                new() { SubcategoriaId = aceitesSubcategorias[0].Id, Valor = "Motor" },
                new() { SubcategoriaId = aceitesSubcategorias[0].Id, Valor = "Transmisión" },
                new() { SubcategoriaId = aceitesSubcategorias[0].Id, Valor = "Diferencial" },
                new() { SubcategoriaId = aceitesSubcategorias[1].Id, Valor = "5W-30" },
                new() { SubcategoriaId = aceitesSubcategorias[1].Id, Valor = "10W-40" },
                new() { SubcategoriaId = aceitesSubcategorias[1].Id, Valor = "15W-40" },
                new() { SubcategoriaId = aceitesSubcategorias[1].Id, Valor = "20W-50" },
                new() { SubcategoriaId = aceitesSubcategorias[2].Id, Valor = "Castrol" },
                new() { SubcategoriaId = aceitesSubcategorias[2].Id, Valor = "Mobil" },
                new() { SubcategoriaId = aceitesSubcategorias[2].Id, Valor = "Shell" },
                new() { SubcategoriaId = aceitesSubcategorias[3].Id, Valor = "1L" },
                new() { SubcategoriaId = aceitesSubcategorias[3].Id, Valor = "4L" },
                new() { SubcategoriaId = aceitesSubcategorias[3].Id, Valor = "5L" }
            };
            await context.ValoresFiltro.AddRangeAsync(aceitesValores);
            await context.SaveChangesAsync();
            Console.WriteLine($"      ✅ ACEITES: {aceitesSubcategorias.Count} subcategorías, {aceitesValores.Count} valores");

            // 2. NEUMÁTICOS
            Console.WriteLine("   📦 Creando categoría: NEUMÁTICOS");
            var neumaticos = new Categoria
            {
                Nombre = "Neumáticos",
                Descripcion = "Neumáticos para todo tipo de vehículos",
                IconUrl = "fas fa-tire",
                EsActivo = true,
                FechaCreacion = DateTime.UtcNow
            };
            await context.Categorias.AddAsync(neumaticos);
            await context.SaveChangesAsync();

            var neumaticosSubcategorias = new List<Subcategoria>
            {
                new() { CategoriaId = neumaticos.Id, Nombre = "Tipo" },
                new() { CategoriaId = neumaticos.Id, Nombre = "Tamaño" },
                new() { CategoriaId = neumaticos.Id, Nombre = "Marca" }
            };
            await context.Subcategorias.AddRangeAsync(neumaticosSubcategorias);
            await context.SaveChangesAsync();

            var neumaticosValores = new List<ValorFiltro>
            {
                new() { SubcategoriaId = neumaticosSubcategorias[0].Id, Valor = "Verano" },
                new() { SubcategoriaId = neumaticosSubcategorias[0].Id, Valor = "Invierno" },
                new() { SubcategoriaId = neumaticosSubcategorias[0].Id, Valor = "All Season" },
                new() { SubcategoriaId = neumaticosSubcategorias[1].Id, Valor = "165/70R13" },
                new() { SubcategoriaId = neumaticosSubcategorias[1].Id, Valor = "185/65R14" },
                new() { SubcategoriaId = neumaticosSubcategorias[1].Id, Valor = "195/65R15" },
                new() { SubcategoriaId = neumaticosSubcategorias[1].Id, Valor = "205/55R16" },
                new() { SubcategoriaId = neumaticosSubcategorias[2].Id, Valor = "Michelin" },
                new() { SubcategoriaId = neumaticosSubcategorias[2].Id, Valor = "Bridgestone" },
                new() { SubcategoriaId = neumaticosSubcategorias[2].Id, Valor = "Goodyear" }
            };
            await context.ValoresFiltro.AddRangeAsync(neumaticosValores);
            await context.SaveChangesAsync();
            Console.WriteLine($"      ✅ NEUMÁTICOS: {neumaticosSubcategorias.Count} subcategorías, {neumaticosValores.Count} valores");

            // 3. PLUMILLAS
            Console.WriteLine("   📦 Creando categoría: PLUMILLAS");
            var plumillas = new Categoria
            {
                Nombre = "Plumillas",
                Descripcion = "Plumillas limpiaparabrisas",
                IconUrl = "fas fa-wind",
                EsActivo = true,
                FechaCreacion = DateTime.UtcNow
            };
            await context.Categorias.AddAsync(plumillas);
            await context.SaveChangesAsync();

            var plumillasSubcategorias = new List<Subcategoria>
            {
                new() { CategoriaId = plumillas.Id, Nombre = "Tamaño" },
                new() { CategoriaId = plumillas.Id, Nombre = "Tipo" },
                new() { CategoriaId = plumillas.Id, Nombre = "Marca" }
            };
            await context.Subcategorias.AddRangeAsync(plumillasSubcategorias);
            await context.SaveChangesAsync();

            var plumillasValores = new List<ValorFiltro>
            {
                new() { SubcategoriaId = plumillasSubcategorias[0].Id, Valor = "400mm" },
                new() { SubcategoriaId = plumillasSubcategorias[0].Id, Valor = "450mm" },
                new() { SubcategoriaId = plumillasSubcategorias[0].Id, Valor = "500mm" },
                new() { SubcategoriaId = plumillasSubcategorias[0].Id, Valor = "550mm" },
                new() { SubcategoriaId = plumillasSubcategorias[1].Id, Valor = "Convencional" },
                new() { SubcategoriaId = plumillasSubcategorias[1].Id, Valor = "Aerodinámico" },
                new() { SubcategoriaId = plumillasSubcategorias[1].Id, Valor = "Híbrido" },
                new() { SubcategoriaId = plumillasSubcategorias[2].Id, Valor = "Bosch" },
                new() { SubcategoriaId = plumillasSubcategorias[2].Id, Valor = "TRICO" },
                new() { SubcategoriaId = plumillasSubcategorias[2].Id, Valor = "Rain-X" }
            };
            await context.ValoresFiltro.AddRangeAsync(plumillasValores);
            await context.SaveChangesAsync();
            Console.WriteLine($"      ✅ PLUMILLAS: {plumillasSubcategorias.Count} subcategorías, {plumillasValores.Count} valores");

            // 4. FILTROS
            Console.WriteLine("   📦 Creando categoría: FILTROS");
            var filtros = new Categoria
            {
                Nombre = "Filtros",
                Descripcion = "Filtros de aire, aceite, combustible y cabina",
                IconUrl = "fas fa-filter",
                EsActivo = true,
                FechaCreacion = DateTime.UtcNow
            };
            await context.Categorias.AddAsync(filtros);
            await context.SaveChangesAsync();

            var filtrosSubcategorias = new List<Subcategoria>
            {
                new() { CategoriaId = filtros.Id, Nombre = "Tipo" },
                new() { CategoriaId = filtros.Id, Nombre = "Marca" }
            };
            await context.Subcategorias.AddRangeAsync(filtrosSubcategorias);
            await context.SaveChangesAsync();

            var filtrosValores = new List<ValorFiltro>
            {
                new() { SubcategoriaId = filtrosSubcategorias[0].Id, Valor = "Motor" },
                new() { SubcategoriaId = filtrosSubcategorias[0].Id, Valor = "Aire" },
                new() { SubcategoriaId = filtrosSubcategorias[0].Id, Valor = "Combustible" },
                new() { SubcategoriaId = filtrosSubcategorias[0].Id, Valor = "Cabina" },
                new() { SubcategoriaId = filtrosSubcategorias[1].Id, Valor = "Fram" },
                new() { SubcategoriaId = filtrosSubcategorias[1].Id, Valor = "Bosch" },
                new() { SubcategoriaId = filtrosSubcategorias[1].Id, Valor = "Mann" }
            };
            await context.ValoresFiltro.AddRangeAsync(filtrosValores);
            await context.SaveChangesAsync();
            Console.WriteLine($"      ✅ FILTROS: {filtrosSubcategorias.Count} subcategorías, {filtrosValores.Count} valores");

            // 5. RADIOS
            Console.WriteLine("   📦 Creando categoría: RADIOS");
            var radios = new Categoria
            {
                Nombre = "Radios",
                Descripcion = "Radios multimedia para automóviles",
                IconUrl = "fas fa-radio",
                EsActivo = true,
                FechaCreacion = DateTime.UtcNow
            };
            await context.Categorias.AddAsync(radios);
            await context.SaveChangesAsync();

            var radiosSubcategorias = new List<Subcategoria>
            {
                new() { CategoriaId = radios.Id, Nombre = "Características" },
                new() { CategoriaId = radios.Id, Nombre = "Marca" }
            };
            await context.Subcategorias.AddRangeAsync(radiosSubcategorias);
            await context.SaveChangesAsync();

            var radiosValores = new List<ValorFiltro>
            {
                new() { SubcategoriaId = radiosSubcategorias[0].Id, Valor = "Bluetooth" },
                new() { SubcategoriaId = radiosSubcategorias[0].Id, Valor = "Android Auto" },
                new() { SubcategoriaId = radiosSubcategorias[0].Id, Valor = "Apple CarPlay" },
                new() { SubcategoriaId = radiosSubcategorias[0].Id, Valor = "Pantalla Táctil" },
                new() { SubcategoriaId = radiosSubcategorias[1].Id, Valor = "Pioneer" },
                new() { SubcategoriaId = radiosSubcategorias[1].Id, Valor = "Sony" },
                new() { SubcategoriaId = radiosSubcategorias[1].Id, Valor = "JVC" }
            };
            await context.ValoresFiltro.AddRangeAsync(radiosValores);
            await context.SaveChangesAsync();
            Console.WriteLine($"      ✅ RADIOS: {radiosSubcategorias.Count} subcategorías, {radiosValores.Count} valores");

            // 6. GADGETS
            Console.WriteLine("   📦 Creando categoría: GADGETS");
            var gadgets = new Categoria
            {
                Nombre = "Gadgets",
                Descripcion = "Accesorios y gadgets automotrices",
                IconUrl = "fas fa-tools",
                EsActivo = true,
                FechaCreacion = DateTime.UtcNow
            };
            await context.Categorias.AddAsync(gadgets);
            await context.SaveChangesAsync();

            var gadgetsSubcategorias = new List<Subcategoria>
            {
                new() { CategoriaId = gadgets.Id, Nombre = "Tipo" },
                new() { CategoriaId = gadgets.Id, Nombre = "Producto" }
            };
            await context.Subcategorias.AddRangeAsync(gadgetsSubcategorias);
            await context.SaveChangesAsync();

            var gadgetsValores = new List<ValorFiltro>
            {
                new() { SubcategoriaId = gadgetsSubcategorias[0].Id, Valor = "Limpieza" },
                new() { SubcategoriaId = gadgetsSubcategorias[0].Id, Valor = "Protección" },
                new() { SubcategoriaId = gadgetsSubcategorias[0].Id, Valor = "Organización" },
                new() { SubcategoriaId = gadgetsSubcategorias[1].Id, Valor = "Cera protectora" },
                new() { SubcategoriaId = gadgetsSubcategorias[1].Id, Valor = "Cubre volante" },
                new() { SubcategoriaId = gadgetsSubcategorias[1].Id, Valor = "Organizador" },
                new() { SubcategoriaId = gadgetsSubcategorias[1].Id, Valor = "Ambientador" }
            };
            await context.ValoresFiltro.AddRangeAsync(gadgetsValores);
            await context.SaveChangesAsync();
            Console.WriteLine($"      ✅ GADGETS: {gadgetsSubcategorias.Count} subcategorías, {gadgetsValores.Count} valores");

            // Resumen final
            var totalCategorias = await context.Categorias.CountAsync();
            var totalSubcategorias = await context.Subcategorias.CountAsync();
            var totalValores = await context.ValoresFiltro.CountAsync();
            
            Console.WriteLine("   ═══════════════════════════════════════════════");
            Console.WriteLine($"   ✅ Seeding de Categorías completado:");
            Console.WriteLine($"      📊 {totalCategorias} Categorías");
            Console.WriteLine($"      📊 {totalSubcategorias} Subcategorías");
            Console.WriteLine($"      📊 {totalValores} Valores de Filtro");
            Console.WriteLine("   ═══════════════════════════════════════════════");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   ❌ Error al sembrar categorías: {ex.Message}");
            Console.WriteLine($"   📍 StackTrace: {ex.StackTrace}");
            throw;
        }
    }
}
