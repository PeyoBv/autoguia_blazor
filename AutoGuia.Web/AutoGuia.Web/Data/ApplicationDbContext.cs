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

    /// <summary>
    /// Medios de pago (tarjetas inscritas) de usuarios
    /// </summary>
    public DbSet<PaymentMethod> PaymentMethods { get; set; }

    /// <summary>
    /// Transacciones de Transbank
    /// </summary>
    public DbSet<TransbankTransaction> TransbankTransactions { get; set; }

    /// <summary>
    /// Logs de eventos de pagos
    /// </summary>
    public DbSet<PaymentLog> PaymentLogs { get; set; }

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

        // ✅ Configuración de PaymentMethod
        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.ToTable("PaymentMethods");
            
            entity.HasKey(pm => pm.Id);
            
            entity.Property(pm => pm.UsuarioId)
                .IsRequired();
            
            entity.Property(pm => pm.TbkToken)
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(pm => pm.Last4Digits)
                .IsRequired()
                .HasMaxLength(4);
            
            entity.Property(pm => pm.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            entity.Property(pm => pm.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            // Índices
            entity.HasIndex(pm => pm.UsuarioId);
            entity.HasIndex(pm => pm.TbkToken).IsUnique();
            entity.HasIndex(pm => new { pm.UsuarioId, pm.IsDefault, pm.IsActive });
        });

        // ✅ Configuración de TransbankTransaction
        modelBuilder.Entity<TransbankTransaction>(entity =>
        {
            entity.ToTable("TransbankTransactions");
            
            entity.HasKey(t => t.Id);
            
            entity.Property(t => t.UsuarioId)
                .IsRequired();
            
            entity.Property(t => t.Type)
                .HasConversion<int>();
            
            entity.Property(t => t.Status)
                .HasConversion<int>();
            
            entity.Property(t => t.Amount)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);
            
            entity.Property(t => t.TransactionToken)
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(t => t.BuyOrder)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(t => t.Environment)
                .IsRequired()
                .HasMaxLength(20);
            
            entity.Property(t => t.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            entity.Property(t => t.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            // Relaciones
            entity.HasOne(t => t.PaymentMethod)
                .WithMany(pm => pm.Transactions)
                .HasForeignKey(t => t.PaymentMethodId)
                .OnDelete(DeleteBehavior.SetNull);
            
            entity.HasOne(t => t.Suscripcion)
                .WithMany()
                .HasForeignKey(t => t.SuscripcionId)
                .OnDelete(DeleteBehavior.SetNull);
            
            // Índices
            entity.HasIndex(t => t.UsuarioId);
            entity.HasIndex(t => t.PaymentMethodId);
            entity.HasIndex(t => t.SuscripcionId);
            entity.HasIndex(t => t.TransactionToken);
            entity.HasIndex(t => t.BuyOrder).IsUnique();
            entity.HasIndex(t => new { t.Status, t.CreatedAt });
            entity.HasIndex(t => new { t.Type, t.Status });
        });

        // ✅ Configuración de PaymentLog
        modelBuilder.Entity<PaymentLog>(entity =>
        {
            entity.ToTable("PaymentLogs");
            
            entity.HasKey(l => l.Id);
            
            entity.Property(l => l.Level)
                .HasConversion<int>();
            
            entity.Property(l => l.Event)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(l => l.Message)
                .IsRequired()
                .HasMaxLength(1000);
            
            entity.Property(l => l.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            // Relación
            entity.HasOne(l => l.Transaction)
                .WithMany()
                .HasForeignKey(l => l.TransactionId)
                .OnDelete(DeleteBehavior.SetNull);
            
            // Índices
            entity.HasIndex(l => l.TransactionId);
            entity.HasIndex(l => l.UsuarioId);
            entity.HasIndex(l => new { l.Level, l.CreatedAt });
            entity.HasIndex(l => l.Event);
        });
    }
}
