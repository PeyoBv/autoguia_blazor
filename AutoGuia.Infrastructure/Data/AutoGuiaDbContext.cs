using Microsoft.EntityFrameworkCore;
using AutoGuia.Core.Entities;

namespace AutoGuia.Infrastructure.Data
{
    public class AutoGuiaDbContext : DbContext
    {
        public AutoGuiaDbContext(DbContextOptions<AutoGuiaDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Vehiculo> Vehiculos { get; set; }
        public DbSet<Taller> Talleres { get; set; }
        public DbSet<ResenasTaller> ResenasTalleres { get; set; }
        public DbSet<Resena> Resenas { get; set; }
        public DbSet<PublicacionForo> PublicacionesForo { get; set; }
        public DbSet<RespuestaForo> RespuestasForo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de relaciones
            modelBuilder.Entity<Vehiculo>()
                .HasOne(v => v.Usuario)
                .WithMany(u => u.Vehiculos)
                .HasForeignKey(v => v.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración ResenasTaller (legacy - mantenido para compatibilidad)
            modelBuilder.Entity<ResenasTaller>()
                .HasOne(r => r.Taller)
                .WithMany() // Sin relación de navegación para evitar conflictos
                .HasForeignKey(r => r.TallerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ResenasTaller>()
                .HasOne(r => r.Usuario)
                .WithMany()
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PublicacionForo>()
                .HasOne(p => p.Usuario)
                .WithMany(u => u.PublicacionesForo)
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RespuestaForo>()
                .HasOne(r => r.Publicacion)
                .WithMany(p => p.Respuestas)
                .HasForeignKey(r => r.PublicacionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RespuestaForo>()
                .HasOne(r => r.Usuario)
                .WithMany(u => u.RespuestasForo)
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RespuestaForo>()
                .HasOne(r => r.RespuestaPadre)
                .WithMany(r => r.RespuestasHijas)
                .HasForeignKey(r => r.RespuestaPadreId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración para nueva entidad Resena
            modelBuilder.Entity<Resena>()
                .HasOne(r => r.Taller)
                .WithMany(t => t.Resenas)
                .HasForeignKey(r => r.TallerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración de índices para mejor rendimiento
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Taller>()
                .HasIndex(t => new { t.Ciudad, t.Region });

            modelBuilder.Entity<PublicacionForo>()
                .HasIndex(p => p.Categoria);

            modelBuilder.Entity<PublicacionForo>()
                .HasIndex(p => p.FechaCreacion);

            modelBuilder.Entity<Resena>()
                .HasIndex(r => r.TallerId);

            modelBuilder.Entity<Resena>()
                .HasIndex(r => r.FechaPublicacion);

            modelBuilder.Entity<Resena>()
                .HasIndex(r => new { r.TallerId, r.UsuarioId })
                .IsUnique(); // Un usuario solo puede reseñar un taller una vez

            // Datos semilla para el MVP
            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            // Usuarios de ejemplo
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    Nombre = "Juan",
                    Apellido = "Pérez",
                    Email = "juan.perez@example.com",
                    Telefono = "+56912345678",
                    Biografia = "Mecánico con 15 años de experiencia en motores diésel",
                    EspecialidadAutomotriz = "Motores Diésel",
                    AnosExperiencia = 15,
                    FechaRegistro = DateTime.UtcNow.AddDays(-30)
                },
                new Usuario
                {
                    Id = 2,
                    Nombre = "María",
                    Apellido = "González",
                    Email = "maria.gonzalez@example.com",
                    Telefono = "+56987654321",
                    Biografia = "Especialista en sistemas eléctricos automotrices",
                    EspecialidadAutomotriz = "Sistemas Eléctricos",
                    AnosExperiencia = 8,
                    FechaRegistro = DateTime.UtcNow.AddDays(-20)
                },
                new Usuario
                {
                    Id = 3,
                    Nombre = "Carlos",
                    Apellido = "Silva",
                    Email = "carlos.silva@example.com",
                    Biografia = "Aficionado a los autos clásicos",
                    FechaRegistro = DateTime.UtcNow.AddDays(-10)
                }
            );

            // Talleres de ejemplo
            modelBuilder.Entity<Taller>().HasData(
                new Taller
                {
                    Id = 1,
                    Nombre = "Taller Mecánico Central",
                    Descripcion = "Taller especializado en reparaciones generales y mantención preventiva",
                    Direccion = "Av. Libertador Bernardo O'Higgins 1234",
                    Ciudad = "Santiago",
                    Region = "Región Metropolitana",
                    Telefono = "+56222345678",
                    Email = "contacto@tallercentral.cl",
                    Latitud = -33.4489,
                    Longitud = -70.6693,
                    HorarioAtencion = "Lunes a Viernes 8:00-18:00, Sábados 8:00-14:00",
                    CalificacionPromedio = 4.5m,
                    TotalResenas = 23,
                    Especialidades = "Frenos, Suspensión, Motor",
                    EsVerificado = true
                },
                new Taller
                {
                    Id = 2,
                    Nombre = "AutoService Las Condes",
                    Descripcion = "Centro de servicio premium para vehículos de gama alta",
                    Direccion = "Av. Apoquindo 4567",
                    Ciudad = "Las Condes",
                    Region = "Región Metropolitana",
                    Telefono = "+56233456789",
                    Email = "info@autoserviceslc.cl",
                    Latitud = -33.4167,
                    Longitud = -70.6000,
                    HorarioAtencion = "Lunes a Viernes 7:30-19:00, Sábados 8:00-16:00",
                    CalificacionPromedio = 4.8m,
                    TotalResenas = 45,
                    Especialidades = "Diagnóstico computarizado, Aire acondicionado, Cambio de aceite",
                    EsVerificado = true
                },
                new Taller
                {
                    Id = 3,
                    Nombre = "Taller Rodriguez - Valparaíso",
                    Descripcion = "Taller familiar con más de 20 años de experiencia",
                    Direccion = "Calle Errázuriz 789",
                    Ciudad = "Valparaíso",
                    Region = "Región de Valparaíso",
                    Telefono = "+56324567890",
                    Latitud = -33.0458,
                    Longitud = -71.6197,
                    HorarioAtencion = "Lunes a Sábado 8:30-18:30",
                    CalificacionPromedio = 4.2m,
                    TotalResenas = 12,
                    Especialidades = "Soldadura, Pintura, Enderezado",
                    EsVerificado = false
                }
            );

            // Publicaciones del foro de ejemplo
            modelBuilder.Entity<PublicacionForo>().HasData(
                new PublicacionForo
                {
                    Id = 1,
                    Titulo = "¿Cada cuánto cambiar el aceite del motor?",
                    Contenido = "Hola comunidad, tengo un Toyota Corolla 2018 y quería saber con qué frecuencia debería cambiar el aceite. He escuchado diferentes opiniones, algunos dicen cada 5.000 km, otros cada 10.000 km. ¿Cuál es su experiencia?",
                    Categoria = "Mantenimiento",
                    Etiquetas = "aceite,motor,mantenimiento,toyota",
                    FechaCreacion = DateTime.UtcNow.AddDays(-5),
                    UsuarioId = 3,
                    Vistas = 45,
                    Likes = 8,
                    EsDestacado = true
                },
                new PublicacionForo
                {
                    Id = 2,
                    Titulo = "Problema con frenos que rechinan",
                    Contenido = "Mi auto ha comenzado a hacer un ruido extraño cuando freno, como un rechinar. ¿Es normal? ¿Debería preocuparme? El vehículo tiene 60.000 km.",
                    Categoria = "Problemas Mecánicos",
                    Etiquetas = "frenos,ruido,seguridad",
                    FechaCreacion = DateTime.UtcNow.AddDays(-2),
                    UsuarioId = 3,
                    Vistas = 23,
                    Likes = 3
                },
                new PublicacionForo
                {
                    Id = 3,
                    Titulo = "Recomendaciones de talleres en Santiago Centro",
                    Contenido = "Necesito un taller confiable en Santiago Centro para hacerle el service a mi auto. ¿Alguna recomendación? Preferiblemente que no sea muy caro pero que hagan buen trabajo.",
                    Categoria = "Recomendaciones",
                    Etiquetas = "taller,santiago,recomendacion,service",
                    FechaCreacion = DateTime.UtcNow.AddDays(-1),
                    UsuarioId = 3,
                    Vistas = 12,
                    Likes = 2
                }
            );

            // Respuestas del foro de ejemplo
            modelBuilder.Entity<RespuestaForo>().HasData(
                new RespuestaForo
                {
                    Id = 1,
                    Contenido = "Para un Toyota Corolla 2018, te recomiendo cambiar el aceite cada 10.000 km o cada 6 meses, lo que ocurra primero. Usa aceite 5W-30 sintético para mejor protección.",
                    FechaCreacion = DateTime.UtcNow.AddDays(-4),
                    PublicacionId = 1,
                    UsuarioId = 1,
                    Likes = 5,
                    EsRespuestaAceptada = true
                },
                new RespuestaForo
                {
                    Id = 2,
                    Contenido = "El rechinar en los frenos generalmente indica que las pastillas están gastadas. Te recomiendo revisarlas pronto, es un tema de seguridad. No esperes mucho.",
                    FechaCreacion = DateTime.UtcNow.AddDays(-1),
                    PublicacionId = 2,
                    UsuarioId = 1,
                    Likes = 2
                },
                new RespuestaForo
                {
                    Id = 3,
                    Contenido = "Yo he tenido buena experiencia con el Taller Mecánico Central en O'Higgins. Son honestos y tienen buenos precios.",
                    FechaCreacion = DateTime.UtcNow.AddHours(-12),
                    PublicacionId = 3,
                    UsuarioId = 2,
                    Likes = 1
                }
            );

            // Datos semilla para Reseñas
            modelBuilder.Entity<Resena>().HasData(
                new Resena
                {
                    Id = 1,
                    Calificacion = 5,
                    Comentario = "Excelente servicio, muy profesionales y precios justos. Recomendado al 100%.",
                    FechaPublicacion = DateTime.UtcNow.AddDays(-30),
                    TallerId = 1, // Taller Mecánico Central
                    UsuarioId = "user1@example.com",
                    NombreUsuario = "María González"
                },
                new Resena
                {
                    Id = 2,
                    Calificacion = 4,
                    Comentario = "Buen trabajo, aunque tuve que esperar un poco más de lo esperado.",
                    FechaPublicacion = DateTime.UtcNow.AddDays(-25),
                    TallerId = 1,
                    UsuarioId = "user2@example.com",
                    NombreUsuario = "Carlos Rodríguez"
                },
                new Resena
                {
                    Id = 3,
                    Calificacion = 5,
                    Comentario = "Atención de primera, solucionaron mi problema rápidamente.",
                    FechaPublicacion = DateTime.UtcNow.AddDays(-20),
                    TallerId = 2, // AutoService Las Condes
                    UsuarioId = "user3@example.com",
                    NombreUsuario = "Ana López"
                },
                new Resena
                {
                    Id = 4,
                    Calificacion = 3,
                    Comentario = "Servicio regular, cumplieron pero nada excepcional.",
                    FechaPublicacion = DateTime.UtcNow.AddDays(-18),
                    TallerId = 3, // Taller Rodríguez - Valparaíso
                    UsuarioId = "user4@example.com",
                    NombreUsuario = "Luis Hernández"
                },
                new Resena
                {
                    Id = 5,
                    Calificacion = 4,
                    Comentario = "Buenos mecánicos, precios competitivos. Volveré sin duda.",
                    FechaPublicacion = DateTime.UtcNow.AddDays(-15),
                    TallerId = 3,
                    UsuarioId = "user5@example.com",
                    NombreUsuario = "Patricia Silva"
                },
                new Resena
                {
                    Id = 6,
                    Calificacion = 5,
                    Comentario = "Perfecta atención, muy recomendable para trabajos de carrocería.",
                    FechaPublicacion = DateTime.UtcNow.AddDays(-10),
                    TallerId = 4, // AutoMaster Concepción
                    UsuarioId = "user6@example.com",
                    NombreUsuario = "Roberto Muñoz"
                },
                new Resena
                {
                    Id = 7,
                    Calificacion = 2,
                    Comentario = "Tuve algunos inconvenientes con los tiempos de entrega.",
                    FechaPublicacion = DateTime.UtcNow.AddDays(-8),
                    TallerId = 5, // Mecánica Express Viña del Mar
                    UsuarioId = "user7@example.com",
                    NombreUsuario = "Sandra Torres"
                },
                new Resena
                {
                    Id = 8,
                    Calificacion = 4,
                    Comentario = "Muy buena experiencia, personal capacitado y instalaciones limpias.",
                    FechaPublicacion = DateTime.UtcNow.AddDays(-5),
                    TallerId = 6, // TallerPro La Serena
                    UsuarioId = "user8@example.com",
                    NombreUsuario = "Fernando Díaz"
                }
            );
        }
    }
}