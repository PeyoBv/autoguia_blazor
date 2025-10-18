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
    /// <param name="serviceProvider">Proveedor de servicios para obtener dependencias</param>
    public static async Task SeedData(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            // Obtener contextos y managers necesarios
            var identityContext = services.GetRequiredService<ApplicationDbContext>();
            var autoguiaContext = services.GetRequiredService<AutoGuiaDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            // Asegurar que las bases de datos estén creadas
            await identityContext.Database.EnsureCreatedAsync();
            await autoguiaContext.Database.EnsureCreatedAsync();

            // Sembrar datos de Identity
            await SeedIdentityData(userManager, roleManager);

            // Sembrar datos de la aplicación
            await SeedApplicationData(autoguiaContext);

            Console.WriteLine("✅ Datos semilla aplicados correctamente");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error al aplicar datos semilla: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Siembra los datos de Identity (roles y usuarios)
    /// </summary>
    private static async Task SeedIdentityData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Crear roles si no existen
        await CreateRoleIfNotExists(roleManager, "Admin");
        await CreateRoleIfNotExists(roleManager, "Usuario");

        // Crear usuario administrador si no existe
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
            else
            {
                Console.WriteLine($"❌ Error al crear usuario administrador: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }

    /// <summary>
    /// Crea un rol si no existe
    /// </summary>
    private static async Task CreateRoleIfNotExists(RoleManager<IdentityRole> roleManager, string roleName)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
            Console.WriteLine($"✅ Rol creado: {roleName}");
        }
    }

    /// <summary>
    /// Siembra los datos de la aplicación AutoGuía
    /// </summary>
    private static async Task SeedApplicationData(AutoGuiaDbContext context)
    {
        // Verificar si ya existen datos
        if (await context.Marcas.AnyAsync())
        {
            Console.WriteLine("ℹ️ Los datos de la aplicación ya existen, omitiendo seeding");
            return;
        }

        // Sembrar Marcas
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

        // Sembrar Productos
        var productos = new List<Producto>
        {
            new() { 
                Nombre = "Pastillas de Freno Delanteras", 
                NumeroDeParte = "BP-1234",
                Descripcion = "Pastillas de freno cerámicas para mayor durabilidad y menor ruido",
                ImagenUrl = "/images/productos/pastillas-freno-bosch.jpg",
                EsActivo = true, 
                FechaCreacion = DateTime.UtcNow 
            },
            new() { 
                Nombre = "Filtro de Aceite", 
                NumeroDeParte = "FO-9012",
                Descripcion = "Filtro de aceite de alta calidad para motor",
                ImagenUrl = "/images/productos/filtro-aceite-mann.jpg",
                EsActivo = true, 
                FechaCreacion = DateTime.UtcNow 
            },
            new() { 
                Nombre = "Amortiguador Delantero", 
                NumeroDeParte = "AD-7890",
                Descripcion = "Amortiguador de gas presurizado para mejor confort y control",
                ImagenUrl = "/images/productos/amortiguador-monroe.jpg",
                EsActivo = true, 
                FechaCreacion = DateTime.UtcNow 
            },
            new() { 
                Nombre = "Batería 12V 65Ah", 
                NumeroDeParte = "BT-9753",
                Descripcion = "Batería de arranque libre de mantenimiento",
                ImagenUrl = "/images/productos/bateria-bosch.jpg",
                EsActivo = true, 
                FechaCreacion = DateTime.UtcNow 
            },
            new() { 
                Nombre = "Aceite Motor 5W-30 Sintético", 
                NumeroDeParte = "AM-2468",
                Descripcion = "Aceite sintético premium para motores de alta performance",
                ImagenUrl = "/images/productos/aceite-castrol.jpg",
                EsActivo = true, 
                FechaCreacion = DateTime.UtcNow 
            }
        };
        
        await context.Productos.AddRangeAsync(productos);
        await context.SaveChangesAsync();

        // Sembrar Tiendas
        var tiendas = new List<Tienda>
        {
            new() { 
                Nombre = "Repuestos Total",
                UrlSitioWeb = "https://www.repuestostotal.cl",
                LogoUrl = "/images/tiendas/repuestos-total.png",
                Descripcion = "Tienda líder en repuestos automotrices en Chile",
                EsActivo = true, 
                FechaCreacion = DateTime.UtcNow 
            },
            new() { 
                Nombre = "Automotriz Chile",
                UrlSitioWeb = "https://www.automotrizchile.cl",
                LogoUrl = "/images/tiendas/automotriz-chile.png",
                Descripcion = "Especialistas en repuestos y accesorios para vehículos",
                EsActivo = true, 
                FechaCreacion = DateTime.UtcNow 
            },
            new() { 
                Nombre = "AutoPartes Online",
                UrlSitioWeb = "https://www.autopartesonline.cl",
                LogoUrl = "/images/tiendas/autopartes-online.png",
                Descripcion = "Venta online de repuestos con entrega a domicilio",
                EsActivo = true, 
                FechaCreacion = DateTime.UtcNow 
            }
        };
        
        await context.Tiendas.AddRangeAsync(tiendas);
        await context.SaveChangesAsync();

        // Sembrar Talleres
        var talleres = new List<Taller>
        {
            new() { 
                Nombre = "Taller Mecánico Santiago Centro",
                Descripcion = "Taller especializado en mantenimiento preventivo y correctivo",
                Direccion = "Av. Libertador Bernardo O'Higgins 1234",
                Ciudad = "Santiago",
                Region = "Región Metropolitana",
                CodigoPostal = "8320000",
                Telefono = "+56 2 2123 4567",
                Email = "contacto@tallersantiago.cl",
                SitioWeb = "https://www.tallersantiago.cl",
                HorarioAtencion = "Lunes a Viernes 8:00 - 18:00, Sábados 9:00 - 13:00",
                Especialidades = "Mecánica General, Frenos, Suspensión",
                ServiciosOfrecidos = "Cambio de aceite, Alineación, Balanceado, Reparación de frenos, Mantención preventiva",
                EsVerificado = true,
                EsActivo = true, 
                FechaRegistro = DateTime.UtcNow,
                FechaVerificacion = DateTime.UtcNow,
                CalificacionPromedio = 4.5m,
                TotalResenas = 25
            },
            new() { 
                Nombre = "AutoServicio Las Condes",
                Descripcion = "Taller premium con tecnología de última generación",
                Direccion = "Av. Apoquindo 3456",
                Ciudad = "Las Condes",
                Region = "Región Metropolitana",
                CodigoPostal = "7550000",
                Telefono = "+56 2 2987 6543",
                Email = "info@autoserviciolc.cl",
                SitioWeb = "https://www.autoserviciolc.cl",
                HorarioAtencion = "Lunes a Viernes 8:30 - 19:00, Sábados 9:00 - 14:00",
                Especialidades = "Mecánica Premium, Diagnóstico Computarizado, Aire Acondicionado",
                ServiciosOfrecidos = "Diagnóstico computarizado, Reparación de motor, Sistema eléctrico, Aire acondicionado",
                EsVerificado = true,
                EsActivo = true, 
                FechaRegistro = DateTime.UtcNow,
                FechaVerificacion = DateTime.UtcNow,
                CalificacionPromedio = 4.8m,
                TotalResenas = 42
            },
            new() { 
                Nombre = "Mecánica Express Maipú",
                Descripcion = "Servicio rápido y económico para el sector poniente",
                Direccion = "Av. Pajaritos 2789",
                Ciudad = "Maipú",
                Region = "Región Metropolitana",
                CodigoPostal = "9250000",
                Telefono = "+56 2 2543 2198",
                Email = "mecanicoexpress@gmail.com",
                HorarioAtencion = "Lunes a Sábados 8:00 - 19:00",
                Especialidades = "Mecánica Rápida, Neumáticos, Batería",
                ServiciosOfrecidos = "Cambio de aceite express, Revisión técnica, Cambio de neumáticos, Carga de batería",
                EsVerificado = false,
                EsActivo = true, 
                FechaRegistro = DateTime.UtcNow,
                CalificacionPromedio = 4.2m,
                TotalResenas = 18
            }
        };
        
        await context.Talleres.AddRangeAsync(talleres);
        await context.SaveChangesAsync();

        // Sembrar algunos usuarios de ejemplo para AutoGuía
        var usuarios = new List<Usuario>
        {
            new() {
                Nombre = "Carlos",
                Apellido = "González",
                Email = "carlos.gonzalez@email.com",
                Telefono = "+56 9 8765 4321",
                Biografia = "Entusiasta automotriz con 10 años de experiencia en mecánica",
                EspecialidadAutomotriz = "Mecánica General",
                AnosExperiencia = 10,
                FechaRegistro = DateTime.UtcNow,
                EsActivo = true
            },
            new() {
                Nombre = "María",
                Apellido = "Silva",
                Email = "maria.silva@email.com",
                Telefono = "+56 9 1234 5678",
                Biografia = "Mecánica especializada en sistemas eléctricos automotrices",
                EspecialidadAutomotriz = "Sistemas Eléctricos",
                AnosExperiencia = 7,
                FechaRegistro = DateTime.UtcNow,
                EsActivo = true
            }
        };
        
        await context.Usuarios.AddRangeAsync(usuarios);
        await context.SaveChangesAsync();

        // Sembrar publicaciones del foro después de tener usuarios disponibles
        var publicaciones = new List<PublicacionForo>
        {
            new() {
                Id = 1,
                Titulo = "¿Cada cuánto cambiar el aceite del motor?",
                Contenido = "Hola comunidad, tengo un Toyota Corolla 2018 y me gustaría saber cada cuántos kilómetros debo cambiar el aceite. He escuchado diferentes opiniones.",
                UsuarioId = 1,
                FechaCreacion = DateTime.UtcNow.AddDays(-5),
                Categoria = "Mantenimiento",
                EsActivo = true,
                EsCerrado = false,
                EsDestacado = false,
                Likes = 0,
                Vistas = 0
            },
            new() {
                Id = 2,
                Titulo = "Ruido extraño en los frenos",
                Contenido = "Mi auto hace un ruido chirriante cuando freno. ¿Será necesario cambiar las pastillas de freno? ¿Algún taller recomendado en Santiago?",
                UsuarioId = 2,
                FechaCreacion = DateTime.UtcNow.AddDays(-3),
                Categoria = "Problemas Técnicos",
                EsActivo = true,
                EsCerrado = false,
                EsDestacado = false,
                Likes = 0,
                Vistas = 0
            }
        };
        
        await context.PublicacionesForo.AddRangeAsync(publicaciones);
        await context.SaveChangesAsync();

        // Sembrar respuestas del foro después de tener publicaciones
        var respuestas = new List<RespuestaForo>
        {
            new() {
                Id = 1,
                PublicacionId = 1,
                Contenido = "Para un Corolla 2018, te recomiendo cambiar el aceite cada 10,000 km si usas aceite sintético, o cada 5,000 km con aceite convencional.",
                UsuarioId = 1,
                FechaCreacion = DateTime.UtcNow.AddDays(-4),
                EsActivo = true,
                Likes = 0,
                EsRespuestaAceptada = false
            },
            new() {
                Id = 2,
                PublicacionId = 2,
                Contenido = "Ese ruido indica que las pastillas están gastadas. Te recomiendo el Taller San Miguel, son muy buenos con los frenos.",
                UsuarioId = 2,
                FechaCreacion = DateTime.UtcNow.AddDays(-2),
                EsActivo = true,
                Likes = 0,
                EsRespuestaAceptada = false
            }
        };
        
        await context.RespuestasForo.AddRangeAsync(respuestas);
        await context.SaveChangesAsync();

        Console.WriteLine("✅ Datos de la aplicación sembrados correctamente");
    }
}