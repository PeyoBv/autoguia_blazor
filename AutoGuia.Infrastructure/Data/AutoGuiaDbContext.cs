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
        
        // Nuevas entidades para comparación de precios
        public DbSet<Marca> Marcas { get; set; }
        public DbSet<Modelo> Modelos { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Tienda> Tiendas { get; set; }
        public DbSet<Oferta> Ofertas { get; set; }
        public DbSet<ProductoVehiculoCompatible> ProductoVehiculoCompatibles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de relaciones
            modelBuilder.Entity<Modelo>()
                .HasOne(m => m.Marca)
                .WithMany(ma => ma.Modelos)
                .HasForeignKey(m => m.MarcaId);

            modelBuilder.Entity<Oferta>()
                .HasOne(o => o.Producto)
                .WithMany(p => p.Ofertas)
                .HasForeignKey(o => o.ProductoId);

            modelBuilder.Entity<Oferta>()
                .HasOne(o => o.Tienda)
                .WithMany(t => t.Ofertas)
                .HasForeignKey(o => o.TiendaId);

            modelBuilder.Entity<ProductoVehiculoCompatible>()
                .HasKey(pvc => new { pvc.ProductoId, pvc.ModeloId });

            modelBuilder.Entity<ProductoVehiculoCompatible>()
                .HasOne(pvc => pvc.Producto)
                .WithMany(p => p.VehiculosCompatibles)
                .HasForeignKey(pvc => pvc.ProductoId);

            modelBuilder.Entity<ProductoVehiculoCompatible>()
                .HasOne(pvc => pvc.Modelo)
                .WithMany()
                .HasForeignKey(pvc => pvc.ModeloId);

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

            // Tiendas
            modelBuilder.Entity<Tienda>().HasData(
                new Tienda
                {
                    Id = 1,
                    Nombre = "Repuestos Santiago",
                    Descripcion = "Tu tienda de confianza para repuestos automotrices",
                    UrlSitioWeb = "https://repuestossantiago.cl",
                    LogoUrl = "/images/tiendas/repuestos-santiago.png"
                },
                new Tienda
                {
                    Id = 2,
                    Nombre = "AutoPartes Chile",
                    Descripcion = "Especialistas en repuestos importados y nacionales",
                    UrlSitioWeb = "https://autoparteschile.cl",
                    LogoUrl = "/images/tiendas/autopartes-chile.png"
                },
                new Tienda
                {
                    Id = 3,
                    Nombre = "MegaRepuestos",
                    Descripcion = "Los mejores precios en repuestos automotrices",
                    UrlSitioWeb = "https://megarepuestos.cl",
                    LogoUrl = "/images/tiendas/mega-repuestos.png"
                }
            );

            // Productos
            modelBuilder.Entity<Producto>().HasData(
                new Producto
                {
                    Id = 1,
                    Nombre = "Pastillas de Freno Delanteras",
                    Descripcion = "Pastillas de freno cerámicas para mayor durabilidad y menor ruido",
                    NumeroDeParte = "BP-1234",
                    ImagenUrl = "/images/productos/pastillas-freno-bosch.jpg"
                },
                new Producto
                {
                    Id = 2,
                    Nombre = "Filtro de Aceite",
                    Descripcion = "Filtro de aceite de alta calidad para motor",
                    NumeroDeParte = "FO-9012",
                    ImagenUrl = "/images/productos/filtro-aceite-mann.jpg"
                },
                new Producto
                {
                    Id = 3,
                    Nombre = "Amortiguador Delantero",
                    Descripcion = "Amortiguador de gas presurizado para mejor confort y control",
                    NumeroDeParte = "AD-7890",
                    ImagenUrl = "/images/productos/amortiguador-monroe.jpg"
                },
                new Producto
                {
                    Id = 4,
                    Nombre = "Batería 12V 65Ah",
                    Descripcion = "Batería de arranque libre de mantenimiento",
                    NumeroDeParte = "BT-9753",
                    ImagenUrl = "/images/productos/bateria-bosch.jpg"
                },
                new Producto
                {
                    Id = 5,
                    Nombre = "Aceite Motor 5W-30 Sintético",
                    Descripcion = "Aceite sintético premium para motores de alta performance",
                    NumeroDeParte = "AM-2468",
                    ImagenUrl = "/images/productos/aceite-castrol.jpg"
                }
            );

            // Ofertas
            modelBuilder.Entity<Oferta>().HasData(
                new Oferta
                {
                    Id = 1,
                    ProductoId = 1,
                    TiendaId = 1,
                    Precio = 35000m,
                    PrecioAnterior = 42000m,
                    EsOferta = true,
                    UrlProductoEnTienda = "https://repuestossantiago.cl/productos/pastillas-freno-bp1234",
                    SKU = "BP-1234-RS"
                },
                new Oferta
                {
                    Id = 2,
                    ProductoId = 1,
                    TiendaId = 2,
                    Precio = 38000m,
                    EsOferta = false,
                    UrlProductoEnTienda = "https://autoparteschile.cl/pastillas-bosch-bp1234",
                    SKU = "BP-1234-AC"
                },
                new Oferta
                {
                    Id = 3,
                    ProductoId = 1,
                    TiendaId = 3,
                    Precio = 33000m,
                    EsOferta = false,
                    UrlProductoEnTienda = "https://megarepuestos.cl/frenos/pastillas-bp1234",
                    SKU = "BP-1234-MR"
                },
                new Oferta
                {
                    Id = 4,
                    ProductoId = 2,
                    TiendaId = 1,
                    Precio = 8500m,
                    EsOferta = false,
                    UrlProductoEnTienda = "https://repuestossantiago.cl/productos/filtro-aceite-fo9012",
                    SKU = "FO-9012-RS"
                },
                new Oferta
                {
                    Id = 5,
                    ProductoId = 2,
                    TiendaId = 2,
                    Precio = 9200m,
                    EsOferta = false,
                    UrlProductoEnTienda = "https://autoparteschile.cl/filtros/aceite-mann-fo9012",
                    SKU = "FO-9012-AC"
                },
                new Oferta
                {
                    Id = 6,
                    ProductoId = 3,
                    TiendaId = 2,
                    Precio = 85000m,
                    EsOferta = false,
                    UrlProductoEnTienda = "https://autoparteschile.cl/suspension/amortiguador-monroe-ad7890",
                    SKU = "AD-7890-AC"
                },
                new Oferta
                {
                    Id = 7,
                    ProductoId = 4,
                    TiendaId = 1,
                    Precio = 89000m,
                    EsOferta = false,
                    UrlProductoEnTienda = "https://repuestossantiago.cl/productos/bateria-bosch-bt9753",
                    SKU = "BT-9753-RS"
                },
                new Oferta
                {
                    Id = 8,
                    ProductoId = 5,
                    TiendaId = 3,
                    Precio = 43500m,
                    PrecioAnterior = 48000m,
                    EsOferta = true,
                    UrlProductoEnTienda = "https://megarepuestos.cl/lubricantes/aceite-am2468",
                    SKU = "AM-2468-MR"
                }
            );

            // Compatibilidad de productos con vehículos
            modelBuilder.Entity<ProductoVehiculoCompatible>().HasData(
                // Pastillas de Freno - Compatible con Toyota Corolla y Yaris
                new ProductoVehiculoCompatible { ProductoId = 1, ModeloId = 1 }, // Toyota Corolla
                new ProductoVehiculoCompatible { ProductoId = 1, ModeloId = 2 }, // Toyota Yaris
                
                // Filtro de Aceite - Compatible con Toyota Corolla, Yaris y Honda Civic
                new ProductoVehiculoCompatible { ProductoId = 2, ModeloId = 1 }, // Toyota Corolla
                new ProductoVehiculoCompatible { ProductoId = 2, ModeloId = 2 }, // Toyota Yaris
                new ProductoVehiculoCompatible { ProductoId = 2, ModeloId = 4 }, // Honda Civic
                
                // Amortiguador - Compatible con Nissan Sentra y Versa
                new ProductoVehiculoCompatible { ProductoId = 3, ModeloId = 7 }, // Nissan Sentra
                new ProductoVehiculoCompatible { ProductoId = 3, ModeloId = 8 }, // Nissan Versa
                
                // Batería - Universal (compatible con múltiples modelos)
                new ProductoVehiculoCompatible { ProductoId = 4, ModeloId = 1 }, // Toyota Corolla
                new ProductoVehiculoCompatible { ProductoId = 4, ModeloId = 2 }, // Toyota Yaris
                new ProductoVehiculoCompatible { ProductoId = 4, ModeloId = 4 }, // Honda Civic
                new ProductoVehiculoCompatible { ProductoId = 4, ModeloId = 5 }, // Honda Accord
                new ProductoVehiculoCompatible { ProductoId = 4, ModeloId = 7 }, // Nissan Sentra
                
                // Aceite Motor - Compatible con varios modelos
                new ProductoVehiculoCompatible { ProductoId = 5, ModeloId = 1 }, // Toyota Corolla
                new ProductoVehiculoCompatible { ProductoId = 5, ModeloId = 4 }, // Honda Civic
                new ProductoVehiculoCompatible { ProductoId = 5, ModeloId = 5 }, // Honda Accord
                new ProductoVehiculoCompatible { ProductoId = 5, ModeloId = 7 }  // Nissan Sentra
            );

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