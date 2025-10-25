using AutoGuia.Core.Entities;
using AutoGuia.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace AutoGuia.Web.Data;

/// <summary>
/// Seeder de datos iniciales para planes de suscripción
/// </summary>
public static class PlanesSeeder
{
    /// <summary>
    /// Pobla la base de datos con los planes iniciales de AutoGuía
    /// </summary>
    public static async Task SeedPlanesAsync(ApplicationDbContext context)
    {
        // Verificar si ya existen planes
        if (await context.Planes.AnyAsync())
        {
            Console.WriteLine("ℹ️ Los planes ya están poblados en la base de datos");
            return;
        }

        Console.WriteLine("📦 Poblando planes de suscripción...");

        var planes = new List<Plan>
        {
            // ✅ Plan Gratuito
            new Plan
            {
                Nombre = "Gratis",
                Descripcion = "Plan básico para usuarios casuales. Perfecto para comenzar a explorar AutoGuía.",
                Precio = 0,
                Duracion = TipoDuracion.Mensual,
                LimiteDiagnosticos = 5, // 5 diagnósticos con IA por mes
                LimiteBusquedas = 10, // 10 búsquedas de repuestos por día
                AccesoForo = true,
                AccesoMapas = true,
                SoportePrioritario = false,
                AccesoComparador = true,
                Caracteristicas = new[]
                {
                    "5 diagnósticos con IA al mes",
                    "10 búsquedas de repuestos por día",
                    "Acceso al foro comunitario",
                    "Mapa de talleres",
                    "Comparador de precios básico",
                    "Con publicidad"
                },
                Activo = true,
                Destacado = false,
                ColorBadge = "#9E9E9E", // Gris
                Orden = 3,
                FechaCreacion = DateTime.UtcNow
            },

            // 🌟 Plan Pro (Recomendado)
            new Plan
            {
                Nombre = "Pro",
                Descripcion = "Plan profesional para usuarios frecuentes. El más popular entre mecánicos y entusiastas.",
                Precio = 5990, // $5.990 CLP/mes
                Duracion = TipoDuracion.Mensual,
                LimiteDiagnosticos = 50, // 50 diagnósticos con IA por mes
                LimiteBusquedas = 0, // Ilimitado
                AccesoForo = true,
                AccesoMapas = true,
                SoportePrioritario = false,
                AccesoComparador = true,
                Caracteristicas = new[]
                {
                    "50 diagnósticos con IA al mes",
                    "Búsquedas ilimitadas",
                    "Sin publicidad",
                    "Acceso prioritario al foro",
                    "Comparador avanzado",
                    "Notificaciones push",
                    "Historial completo"
                },
                Activo = true,
                Destacado = true, // Plan destacado/recomendado
                ColorBadge = "#2196F3", // Azul
                Orden = 2,
                FechaCreacion = DateTime.UtcNow
            },

            // 💎 Plan Premium
            new Plan
            {
                Nombre = "Premium",
                Descripcion = "Plan ilimitado para talleres mecánicos y profesionales del sector automotriz.",
                Precio = 9990, // $9.990 CLP/mes
                Duracion = TipoDuracion.Mensual,
                LimiteDiagnosticos = 0, // Ilimitado
                LimiteBusquedas = 0, // Ilimitado
                AccesoForo = true,
                AccesoMapas = true,
                SoportePrioritario = true,
                AccesoComparador = true,
                Caracteristicas = new[]
                {
                    "Diagnósticos ilimitados con IA",
                    "Búsquedas ilimitadas",
                    "Sin publicidad",
                    "Soporte prioritario 24/7",
                    "Comparador profesional",
                    "Estadísticas avanzadas",
                    "API de integración",
                    "Reportes personalizados",
                    "Prioridad en nuevas features"
                },
                Activo = true,
                Destacado = false,
                ColorBadge = "#FFD700", // Dorado
                Orden = 1,
                FechaCreacion = DateTime.UtcNow
            },

            // 📅 Plan Pro Anual (Ahorro del 20%)
            new Plan
            {
                Nombre = "Pro Anual",
                Descripcion = "Plan profesional con pago anual. Ahorra un 20% comparado con el plan mensual.",
                Precio = 57500, // $57.500 CLP/año (~$4.792/mes)
                Duracion = TipoDuracion.Anual,
                LimiteDiagnosticos = 50, // 50 diagnósticos con IA por mes
                LimiteBusquedas = 0, // Ilimitado
                AccesoForo = true,
                AccesoMapas = true,
                SoportePrioritario = false,
                AccesoComparador = true,
                Caracteristicas = new[]
                {
                    "50 diagnósticos con IA al mes",
                    "Búsquedas ilimitadas",
                    "Sin publicidad",
                    "Acceso prioritario al foro",
                    "Comparador avanzado",
                    "Notificaciones push",
                    "Historial completo",
                    "🎁 20% de descuento (ahorra $14.380 al año)"
                },
                Activo = true,
                Destacado = false,
                ColorBadge = "#2196F3", // Azul
                Orden = 4,
                FechaCreacion = DateTime.UtcNow
            },

            // 💎 Plan Premium Anual (Ahorro del 25%)
            new Plan
            {
                Nombre = "Premium Anual",
                Descripcion = "Plan premium con pago anual. Ahorra un 25% y obtén acceso ilimitado durante todo el año.",
                Precio = 89900, // $89.900 CLP/año (~$7.492/mes)
                Duracion = TipoDuracion.Anual,
                LimiteDiagnosticos = 0, // Ilimitado
                LimiteBusquedas = 0, // Ilimitado
                AccesoForo = true,
                AccesoMapas = true,
                SoportePrioritario = true,
                AccesoComparador = true,
                Caracteristicas = new[]
                {
                    "Diagnósticos ilimitados con IA",
                    "Búsquedas ilimitadas",
                    "Sin publicidad",
                    "Soporte prioritario 24/7",
                    "Comparador profesional",
                    "Estadísticas avanzadas",
                    "API de integración",
                    "Reportes personalizados",
                    "Prioridad en nuevas features",
                    "🎁 25% de descuento (ahorra $29.980 al año)"
                },
                Activo = true,
                Destacado = false,
                ColorBadge = "#FFD700", // Dorado
                Orden = 5,
                FechaCreacion = DateTime.UtcNow
            }
        };

        await context.Planes.AddRangeAsync(planes);
        await context.SaveChangesAsync();

        Console.WriteLine($"✅ {planes.Count} planes de suscripción creados exitosamente:");
        foreach (var plan in planes.OrderBy(p => p.Orden))
        {
            Console.WriteLine($"   • {plan.Nombre} - {plan.PrecioFormateado}/{plan.DuracionTexto}");
        }
    }

    /// <summary>
    /// Crea una suscripción gratuita para un usuario nuevo
    /// </summary>
    public static async Task CrearSuscripcionGratuitaAsync(ApplicationDbContext context, string usuarioId)
    {
        // Verificar si el usuario ya tiene una suscripción activa
        var suscripcionExistente = await context.Suscripciones
            .FirstOrDefaultAsync(s => s.UsuarioId == usuarioId && s.Estado == EstadoSuscripcion.Activa);

        if (suscripcionExistente != null)
        {
            Console.WriteLine($"ℹ️ El usuario {usuarioId} ya tiene una suscripción activa");
            return;
        }

        // Obtener el plan gratuito
        var planGratuito = await context.Planes
            .FirstOrDefaultAsync(p => p.Nombre == "Gratis" && p.Activo);

        if (planGratuito == null)
        {
            Console.WriteLine("⚠️ No se encontró el plan gratuito");
            return;
        }

        // Crear suscripción gratuita (vigencia indefinida)
        var suscripcion = new Suscripcion
        {
            UsuarioId = usuarioId,
            PlanId = planGratuito.Id,
            FechaInicio = DateTime.UtcNow,
            FechaVencimiento = DateTime.UtcNow.AddYears(100), // Prácticamente indefinida
            Estado = EstadoSuscripcion.Activa,
            MetodoPago = "Gratuito",
            MontoPagado = 0,
            RenovacionAutomatica = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await context.Suscripciones.AddAsync(suscripcion);
        await context.SaveChangesAsync();

        Console.WriteLine($"✅ Suscripción gratuita creada para usuario {usuarioId}");
    }
}
