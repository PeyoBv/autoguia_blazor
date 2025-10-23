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
        
        // Entidades de consumibles y comparador
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Subcategoria> Subcategorias { get; set; }
        public DbSet<ValorFiltro> ValoresFiltro { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Oferta> Ofertas { get; set; }
        public DbSet<Tienda> Tiendas { get; set; }
        public DbSet<ProductoVehiculoCompatible> ProductoVehiculoCompatibles { get; set; }
        
        // Entidades del módulo de diagnóstico
        public DbSet<SistemaAutomotriz> SistemasAutomotrices { get; set; }
        public DbSet<Sintoma> Sintomas { get; set; }
        public DbSet<CausaPosible> CausasPosibles { get; set; }
        public DbSet<PasoVerificacion> PasosVerificacion { get; set; }
        public DbSet<RecomendacionPreventiva> RecomendacionesPreventivas { get; set; }
        public DbSet<ConsultaDiagnostico> ConsultasDiagnostico { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de relaciones
            modelBuilder.Entity<Modelo>()
                .HasOne(m => m.Marca)
                .WithMany(ma => ma.Modelos)
                .HasForeignKey(m => m.MarcaId);

            // Configuración de relaciones para consumibles
            modelBuilder.Entity<Subcategoria>()
                .HasOne(s => s.Categoria)
                .WithMany(c => c.Subcategorias)
                .HasForeignKey(s => s.CategoriaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ValorFiltro>()
                .HasOne(v => v.Subcategoria)
                .WithMany(s => s.Valores)
                .HasForeignKey(v => v.SubcategoriaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Categoria)
                .WithMany()
                .HasForeignKey(p => p.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de clave compuesta para ProductoVehiculoCompatible
            modelBuilder.Entity<ProductoVehiculoCompatible>()
                .HasKey(pv => new { pv.ProductoId, pv.ModeloId, pv.Ano });

            modelBuilder.Entity<ProductoVehiculoCompatible>()
                .HasOne(pv => pv.Producto)
                .WithMany(p => p.VehiculosCompatibles)
                .HasForeignKey(pv => pv.ProductoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductoVehiculoCompatible>()
                .HasOne(pv => pv.Modelo)
                .WithMany()
                .HasForeignKey(pv => pv.ModeloId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de entidades existentes (talleres y foro)
            modelBuilder.Entity<RespuestaForo>()
                .HasOne(r => r.Publicacion)
                .WithMany(p => p.Respuestas)
                .HasForeignKey(r => r.PublicacionId);

            // ResenasTaller eliminado - no necesario en arquitectura de comparación de precios

            // Configuración de relaciones del módulo de diagnóstico
            
            // Relación SistemaAutomotriz - Sintoma
            modelBuilder.Entity<Sintoma>()
                .HasOne(s => s.SistemaAutomotriz)
                .WithMany(sa => sa.Sintomas)
                .HasForeignKey(s => s.SistemaAutomotrizId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación Sintoma - CausaPosible
            modelBuilder.Entity<CausaPosible>()
                .HasOne(cp => cp.Sintoma)
                .WithMany(s => s.CausasPosibles)
                .HasForeignKey(cp => cp.SintomaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación CausaPosible - PasoVerificacion
            modelBuilder.Entity<PasoVerificacion>()
                .HasOne(pv => pv.CausaPosible)
                .WithMany(cp => cp.PasosVerificacion)
                .HasForeignKey(pv => pv.CausaPosibleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación CausaPosible - RecomendacionPreventiva
            modelBuilder.Entity<RecomendacionPreventiva>()
                .HasOne(rp => rp.CausaPosible)
                .WithMany(cp => cp.Recomendaciones)
                .HasForeignKey(rp => rp.CausaPosibleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación Usuario - ConsultaDiagnostico
            modelBuilder.Entity<ConsultaDiagnostico>()
                .HasOne(cd => cd.Usuario)
                .WithMany()
                .HasForeignKey(cd => cd.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación Sintoma - ConsultaDiagnostico (opcional)
            modelBuilder.Entity<ConsultaDiagnostico>()
                .HasOne(cd => cd.SintomaRelacionado)
                .WithMany()
                .HasForeignKey(cd => cd.SintomaRelacionadoId)
                .OnDelete(DeleteBehavior.SetNull);

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

            // Categorías de consumibles
            modelBuilder.Entity<Categoria>().HasData(
                new Categoria { Id = 1, Nombre = "Aceites", Descripcion = "Aceites para motor, transmisión y diferencial", IconUrl = "/icons/aceites.svg", FechaCreacion = DateTime.UtcNow, EsActivo = true },
                new Categoria { Id = 2, Nombre = "Neumáticos", Descripcion = "Neumáticos para todo tipo de vehículos", IconUrl = "/icons/neumaticos.svg", FechaCreacion = DateTime.UtcNow, EsActivo = true },
                new Categoria { Id = 3, Nombre = "Plumillas", Descripcion = "Plumillas limpiaparabrisas", IconUrl = "/icons/plumillas.svg", FechaCreacion = DateTime.UtcNow, EsActivo = true },
                new Categoria { Id = 4, Nombre = "Filtros", Descripcion = "Filtros de aire, aceite, combustible y cabina", IconUrl = "/icons/filtros.svg", FechaCreacion = DateTime.UtcNow, EsActivo = true },
                new Categoria { Id = 5, Nombre = "Radios", Descripcion = "Radios multimedia para automóviles", IconUrl = "/icons/radios.svg", FechaCreacion = DateTime.UtcNow, EsActivo = true },
                new Categoria { Id = 6, Nombre = "Gadgets", Descripcion = "Accesorios y gadgets automotrices", IconUrl = "/icons/gadgets.svg", FechaCreacion = DateTime.UtcNow, EsActivo = true }
            );

            // Subcategorías para Aceites
            modelBuilder.Entity<Subcategoria>().HasData(
                // ACEITES
                new Subcategoria { Id = 1, CategoriaId = 1, Nombre = "Tipo" },
                new Subcategoria { Id = 2, CategoriaId = 1, Nombre = "Viscosidad" },
                new Subcategoria { Id = 3, CategoriaId = 1, Nombre = "Marca" },
                // NEUMÁTICOS
                new Subcategoria { Id = 4, CategoriaId = 2, Nombre = "Tipo" },
                new Subcategoria { Id = 5, CategoriaId = 2, Nombre = "Tamaño" },
                new Subcategoria { Id = 6, CategoriaId = 2, Nombre = "Marca" },
                // PLUMILLAS
                new Subcategoria { Id = 7, CategoriaId = 3, Nombre = "Tamaño" },
                new Subcategoria { Id = 8, CategoriaId = 3, Nombre = "Tipo" },
                new Subcategoria { Id = 9, CategoriaId = 3, Nombre = "Marca" },
                // FILTROS
                new Subcategoria { Id = 10, CategoriaId = 4, Nombre = "Tipo" },
                new Subcategoria { Id = 11, CategoriaId = 4, Nombre = "Marca" },
                // RADIOS
                new Subcategoria { Id = 12, CategoriaId = 5, Nombre = "Características" },
                new Subcategoria { Id = 13, CategoriaId = 5, Nombre = "Marca" },
                // GADGETS
                new Subcategoria { Id = 14, CategoriaId = 6, Nombre = "Tipo" },
                new Subcategoria { Id = 15, CategoriaId = 6, Nombre = "Categoría" }
            );

            // Valores de filtro
            modelBuilder.Entity<ValorFiltro>().HasData(
                // ACEITES - Tipo (SubcategoriaId = 1)
                new ValorFiltro { Id = 1, SubcategoriaId = 1, Valor = "Motor" },
                new ValorFiltro { Id = 2, SubcategoriaId = 1, Valor = "Transmisión" },
                // ACEITES - Viscosidad (SubcategoriaId = 2)
                new ValorFiltro { Id = 3, SubcategoriaId = 2, Valor = "5W-30" },
                new ValorFiltro { Id = 4, SubcategoriaId = 2, Valor = "10W-40" },
                new ValorFiltro { Id = 5, SubcategoriaId = 2, Valor = "15W-40" },
                // ACEITES - Marca (SubcategoriaId = 3)
                new ValorFiltro { Id = 6, SubcategoriaId = 3, Valor = "Castrol" },
                new ValorFiltro { Id = 7, SubcategoriaId = 3, Valor = "Mobil" },
                // NEUMÁTICOS - Tipo (SubcategoriaId = 4)
                new ValorFiltro { Id = 8, SubcategoriaId = 4, Valor = "Verano" },
                new ValorFiltro { Id = 9, SubcategoriaId = 4, Valor = "Invierno" },
                // NEUMÁTICOS - Tamaño (SubcategoriaId = 5)
                new ValorFiltro { Id = 10, SubcategoriaId = 5, Valor = "165/70R13" },
                new ValorFiltro { Id = 11, SubcategoriaId = 5, Valor = "205/55R16" },
                // NEUMÁTICOS - Marca (SubcategoriaId = 6)
                new ValorFiltro { Id = 12, SubcategoriaId = 6, Valor = "Michelin" },
                new ValorFiltro { Id = 13, SubcategoriaId = 6, Valor = "Continental" },
                // PLUMILLAS - Tamaño (SubcategoriaId = 7)
                new ValorFiltro { Id = 14, SubcategoriaId = 7, Valor = "400mm" },
                new ValorFiltro { Id = 15, SubcategoriaId = 7, Valor = "450mm" },
                new ValorFiltro { Id = 16, SubcategoriaId = 7, Valor = "500mm" },
                // PLUMILLAS - Tipo (SubcategoriaId = 8)
                new ValorFiltro { Id = 17, SubcategoriaId = 8, Valor = "Convencional" },
                new ValorFiltro { Id = 18, SubcategoriaId = 8, Valor = "Aerodinámico" },
                // PLUMILLAS - Marca (SubcategoriaId = 9)
                new ValorFiltro { Id = 19, SubcategoriaId = 9, Valor = "Bosch" },
                new ValorFiltro { Id = 20, SubcategoriaId = 9, Valor = "TRICO" },
                // FILTROS - Tipo (SubcategoriaId = 10)
                new ValorFiltro { Id = 21, SubcategoriaId = 10, Valor = "Motor" },
                new ValorFiltro { Id = 22, SubcategoriaId = 10, Valor = "Aire" },
                // FILTROS - Marca (SubcategoriaId = 11)
                new ValorFiltro { Id = 23, SubcategoriaId = 11, Valor = "Fram" },
                new ValorFiltro { Id = 24, SubcategoriaId = 11, Valor = "Bosch" },
                // RADIOS - Características (SubcategoriaId = 12)
                new ValorFiltro { Id = 25, SubcategoriaId = 12, Valor = "Bluetooth" },
                new ValorFiltro { Id = 26, SubcategoriaId = 12, Valor = "Android Auto" },
                // RADIOS - Marca (SubcategoriaId = 13)
                new ValorFiltro { Id = 27, SubcategoriaId = 13, Valor = "Pioneer" },
                new ValorFiltro { Id = 28, SubcategoriaId = 13, Valor = "Sony" },
                // GADGETS - Tipo (SubcategoriaId = 14)
                new ValorFiltro { Id = 29, SubcategoriaId = 14, Valor = "Limpieza" },
                new ValorFiltro { Id = 30, SubcategoriaId = 14, Valor = "Protección" },
                // GADGETS - Categoría (SubcategoriaId = 15)
                new ValorFiltro { Id = 31, SubcategoriaId = 15, Valor = "Ceras" },
                new ValorFiltro { Id = 32, SubcategoriaId = 15, Valor = "Cubre volante" }
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