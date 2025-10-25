using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using AutoGuia.Core.Entities;

namespace AutoGuia.Web.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    /// <summary>
    /// Planes de suscripción disponibles
    /// </summary>
    public DbSet<Plan> Planes { get; set; }

    /// <summary>
    /// Suscripciones de usuarios
    /// </summary>
    public DbSet<Suscripcion> Suscripciones { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ✅ Configuración de Plan
        modelBuilder.Entity<Plan>(entity =>
        {
            entity.ToTable("Planes");
            
            entity.HasKey(p => p.Id);
            
            entity.Property(p => p.Nombre)
                .IsRequired()
                .HasMaxLength(50);
            
            entity.Property(p => p.Descripcion)
                .IsRequired()
                .HasMaxLength(500);
            
            entity.Property(p => p.Precio)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);
            
            entity.Property(p => p.Duracion)
                .HasConversion<int>();
            
            entity.Property(p => p.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            // Configurar JSON para PostgreSQL
            entity.Property(p => p.Caracteristicas)
                .HasColumnType("jsonb");
            
            // Índices
            entity.HasIndex(p => p.Nombre);
            entity.HasIndex(p => new { p.Activo, p.Orden });
        });

        // ✅ Configuración de Suscripcion
        modelBuilder.Entity<Suscripcion>(entity =>
        {
            entity.ToTable("Suscripciones");
            
            entity.HasKey(s => s.Id);
            
            entity.Property(s => s.UsuarioId)
                .IsRequired();
            
            entity.Property(s => s.Estado)
                .HasConversion<int>();
            
            entity.Property(s => s.MontoPagado)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);
            
            entity.Property(s => s.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            entity.Property(s => s.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            // Relación con ApplicationUser
            entity.HasOne<ApplicationUser>()
                .WithMany(u => u.Suscripciones)
                .HasForeignKey(s => s.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Relación con Plan
            entity.HasOne(s => s.Plan)
                .WithMany(p => p.Suscripciones)
                .HasForeignKey(s => s.PlanId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Índices
            entity.HasIndex(s => s.UsuarioId);
            entity.HasIndex(s => s.PlanId);
            entity.HasIndex(s => new { s.Estado, s.FechaVencimiento });
            entity.HasIndex(s => s.TransaccionId);
        });
    }
}
