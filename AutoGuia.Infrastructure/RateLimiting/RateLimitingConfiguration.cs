using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.RateLimiting;

namespace AutoGuia.Infrastructure.RateLimiting
{
    /// <summary>
    /// Configuración de Rate Limiting para proteger APIs
    /// </summary>
    public static class RateLimitingConfiguration
    {
        public const string FixedWindowPolicy = "fixed";
        public const string SlidingWindowPolicy = "sliding";
        public const string TokenBucketPolicy = "token";
        public const string ConcurrencyPolicy = "concurrency";

        /// <summary>
        /// Configura políticas de Rate Limiting
        /// </summary>
        public static IServiceCollection AddCustomRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                // Política por defecto - Ventana fija
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.User?.Identity?.Name ?? context.Request.Headers.Host.ToString(),
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 100, // 100 requests
                            Window = TimeSpan.FromMinutes(1) // por minuto
                        }));

                // Política de ventana fija
                options.AddFixedWindowLimiter(FixedWindowPolicy, options =>
                {
                    options.PermitLimit = 100;
                    options.Window = TimeSpan.FromMinutes(1);
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = 10;
                });

                // Política de ventana deslizante (más precisa)
                options.AddSlidingWindowLimiter(SlidingWindowPolicy, options =>
                {
                    options.PermitLimit = 100;
                    options.Window = TimeSpan.FromMinutes(1);
                    options.SegmentsPerWindow = 6; // Divide la ventana en 6 segmentos de 10 segundos
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = 10;
                });

                // Política de token bucket (para ráfagas)
                options.AddTokenBucketLimiter(TokenBucketPolicy, options =>
                {
                    options.TokenLimit = 100;
                    options.ReplenishmentPeriod = TimeSpan.FromMinutes(1);
                    options.TokensPerPeriod = 100;
                    options.AutoReplenishment = true;
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = 10;
                });

                // Política de concurrencia (límite de requests simultáneos)
                options.AddConcurrencyLimiter(ConcurrencyPolicy, options =>
                {
                    options.PermitLimit = 20; // Máximo 20 requests concurrentes
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = 50;
                });

                // Manejador de rechazo personalizado
                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    
                    await context.HttpContext.Response.WriteAsJsonAsync(new
                    {
                        error = "Too Many Requests",
                        message = "Has excedido el límite de solicitudes. Por favor, intenta más tarde.",
                        retryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter) 
                            ? retryAfter.TotalSeconds 
                            : 60
                    }, cancellationToken: token);
                };
            });

            return services;
        }

        /// <summary>
        /// Usa Rate Limiting en la aplicación
        /// </summary>
        public static IApplicationBuilder UseCustomRateLimiting(this IApplicationBuilder app)
        {
            app.UseRateLimiter();
            return app;
        }
    }

    /// <summary>
    /// Atributo para aplicar Rate Limiting a endpoints específicos
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class RateLimitAttribute : Attribute
    {
        public string PolicyName { get; }

        public RateLimitAttribute(string policyName = RateLimitingConfiguration.FixedWindowPolicy)
        {
            PolicyName = policyName;
        }
    }
}
