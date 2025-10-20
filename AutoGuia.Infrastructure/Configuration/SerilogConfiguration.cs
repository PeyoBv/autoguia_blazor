using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace AutoGuia.Infrastructure.Configuration
{
    /// <summary>
    /// Configuración de Serilog para logging estructurado
    /// </summary>
    public static class SerilogConfiguration
    {
        /// <summary>
        /// Configura Serilog con múltiples sinks y formato estructurado
        /// </summary>
        public static void ConfigureSerilog(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .Enrich.WithThreadId()
                .Enrich.WithEnvironmentName()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    path: "logs/autoguia-.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    path: "logs/errors/autoguia-errors-.log",
                    restrictedToMinimumLevel: LogEventLevel.Error,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 90,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            Log.Information("🚀 Serilog configurado correctamente");
        }

        /// <summary>
        /// Agrega Serilog a la aplicación con limpieza automática
        /// </summary>
        public static IServiceCollection AddSerilogLogging(this IServiceCollection services)
        {
            services.AddSingleton(Log.Logger);
            return services;
        }
    }
}
