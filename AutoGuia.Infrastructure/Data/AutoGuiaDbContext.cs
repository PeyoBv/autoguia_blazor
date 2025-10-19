using AutoGuia.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoGuia.Infrastructure.Data
{
    public partial class AutoGuiaDbContext : DbContext
    {
        public AutoGuiaDbContext(DbContextOptions<AutoGuiaDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Taller> Talleres { get; set; }
        public DbSet<Vehiculo> Vehiculos { get; set; }
        public DbSet<PublicacionForo> PublicacionesForo { get; set; }
        public DbSet<RespuestaForo> RespuestasForo { get; set; }
        public DbSet<ResenasTaller> ResenasTaller { get; set; }
        
        // Entidades de vehículos (solo Marca y Modelo)
        public DbSet<Marca> Marcas { get; set; }
        public DbSet<Modelo> Modelos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de relaciones
            modelBuilder.Entity<Modelo>()
                .HasOne(m => m.Marca)
                .WithMany(ma => ma.Modelos)
                .HasForeignKey(m => m.MarcaId);

            // Configuración de entidades existentes (talleres y foro)
            modelBuilder.Entity<RespuestaForo>()
                .HasOne(r => r.Publicacion)
                .WithMany(p => p.Respuestas)
                .HasForeignKey(r => r.PublicacionId);

            // ResenasTaller eliminado - no necesario en arquitectura de comparación de precios

            // Datos semilla (seed data)
            
            // Marcas de vehículos
            modelBuilder.Entity<Marca>().HasData(
                new Marca { Id = 1, Nombre = "Toyota", LogoUrl = "/images/marcas/toyota.png" },
                new Marca { Id = 2, Nombre = "Honda", LogoUrl = "/images/marcas/honda.png" },
                new Marca { Id = 3, Nombre = "Nissan", LogoUrl = "/images/marcas/nissan.png" },
                new Marca { Id = 4, Nombre = "Chevrolet", LogoUrl = "/images/marcas/chevrolet.png" },
                new Marca { Id = 5, Nombre = "Ford", LogoUrl = "/images/marcas/ford.png" }
            );

            // Modelos de vehículos
            modelBuilder.Entity<Modelo>().HasData(
                // Toyota
                new Modelo { Id = 1, Nombre = "Corolla", MarcaId = 1, AnioInicioProduccion = 2000, AnioFinProduccion = 2024 },
                new Modelo { Id = 2, Nombre = "Yaris", MarcaId = 1, AnioInicioProduccion = 2005, AnioFinProduccion = 2024 },
                new Modelo { Id = 3, Nombre = "RAV4", MarcaId = 1, AnioInicioProduccion = 2010, AnioFinProduccion = 2024 },
                // Honda
                new Modelo { Id = 4, Nombre = "Civic", MarcaId = 2, AnioInicioProduccion = 2000, AnioFinProduccion = 2024 },
                new Modelo { Id = 5, Nombre = "Accord", MarcaId = 2, AnioInicioProduccion = 2008, AnioFinProduccion = 2024 },
                new Modelo { Id = 6, Nombre = "CR-V", MarcaId = 2, AnioInicioProduccion = 2012, AnioFinProduccion = 2024 },
                // Nissan
                new Modelo { Id = 7, Nombre = "Sentra", MarcaId = 3, AnioInicioProduccion = 2007, AnioFinProduccion = 2024 },
                new Modelo { Id = 8, Nombre = "Versa", MarcaId = 3, AnioInicioProduccion = 2012, AnioFinProduccion = 2024 },
                new Modelo { Id = 9, Nombre = "X-Trail", MarcaId = 3, AnioInicioProduccion = 2014, AnioFinProduccion = 2024 }
            );

            // Seed data de productos/tiendas eliminado - ya no usamos web scraping

            // Seed data para talleres y foro (datos existentes)
            modelBuilder.Entity<Taller>().HasData(
                new Taller
                {
                    Id = 1,
                    Nombre = "Taller Mecánico San Miguel",
                    Direccion = "Av. San Miguel 1234, Santiago",
                    Telefono = "+56912345678",
                    Email = "contacto@tallersanmiguel.cl",
                    Especialidades = "Mecánica general, Frenos, Suspensión",
                    HorarioAtencion = "Lunes a Viernes: 8:00 - 18:00, Sábados: 8:00 - 14:00",
                    CalificacionPromedio = 4.5m
                },
                new Taller
                {
                    Id = 2,
                    Nombre = "AutoServicio Express",
                    Direccion = "Las Condes 5678, Las Condes",
                    Telefono = "+56987654321",
                    Email = "info@autoserviceexpress.cl",
                    Especialidades = "Mantención preventiva, Cambio de aceite, Afinación",
                    HorarioAtencion = "Lunes a Viernes: 7:30 - 19:00, Sábados: 8:00 - 15:00",
                    CalificacionPromedio = 4.2m
                }
            );

            /*
            // Datos de PublicacionForo y RespuestaForo movidos al DataSeeder 
            // para controlar mejor el orden de inserción con los usuarios
            modelBuilder.Entity<PublicacionForo>().HasData(
                new PublicacionForo
                {
                    Id = 1,
                    Titulo = "¿Cada cuánto cambiar el aceite del motor?",
                    Contenido = "Hola comunidad, tengo un Toyota Corolla 2018 y me gustaría saber cada cuántos kilómetros debo cambiar el aceite. He escuchado diferentes opiniones.",
                    UsuarioId = 1,
                    FechaCreacion = DateTime.UtcNow.AddDays(-5),
                    Categoria = "Mantenimiento"
                },
                new PublicacionForo
                {
                    Id = 2,
                    Titulo = "Ruido extraño en los frenos",
                    Contenido = "Mi auto hace un ruido chirriante cuando freno. ¿Será necesario cambiar las pastillas de freno? ¿Algún taller recomendado en Santiago?",
                    UsuarioId = 2,
                    FechaCreacion = DateTime.UtcNow.AddDays(-3),
                    Categoria = "Problemas Técnicos"
                }
            );

            modelBuilder.Entity<RespuestaForo>().HasData(
                new RespuestaForo
                {
                    Id = 1,
                    PublicacionId = 1,
                    Contenido = "Para un Corolla 2018, te recomiendo cambiar el aceite cada 10,000 km si usas aceite sintético, o cada 5,000 km con aceite convencional.",
                    UsuarioId = 3,
                    FechaCreacion = DateTime.UtcNow.AddDays(-4)
                },
                new RespuestaForo
                {
                    Id = 2,
                    PublicacionId = 2,
                    Contenido = "Ese ruido indica que las pastillas están gastadas. Te recomiendo el Taller San Miguel, son muy buenos con los frenos.",
                    UsuarioId = 4,
                    FechaCreacion = DateTime.UtcNow.AddDays(-2)
                }
            );
            */
        }
    }
}