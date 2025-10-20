using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace AutoGuia.Infrastructure.Configuration
{
    /// <summary>
    /// Configuración de políticas de resiliencia con Polly para HttpClient
    /// </summary>
    public static class ResiliencePoliciesConfiguration
    {
        /// <summary>
        /// Configura HttpClient con políticas de reintentos, circuit breaker y timeout
        /// </summary>
        public static IServiceCollection AddResilientHttpClients(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            var timeoutSeconds = configuration.GetValue<int>("ExternalServices:TimeoutSeconds", 30);
            var maxRetries = configuration.GetValue<int>("ExternalServices:MaxRetries", 3);
            var retryDelaySeconds = configuration.GetValue<int>("ExternalServices:RetryDelaySeconds", 2);

            // HttpClient para MercadoLibre
            services.AddHttpClient("MercadoLibre", client =>
            {
                client.BaseAddress = new Uri(configuration["MercadoLibre:BaseUrl"] ?? "https://api.mercadolibre.com");
                client.DefaultRequestHeaders.Add("User-Agent", "AutoGuia/1.0");
                client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
            })
            .AddPolicyHandler(GetRetryPolicy(maxRetries, retryDelaySeconds))
            .AddPolicyHandler(GetCircuitBreakerPolicy())
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(timeoutSeconds));

            // HttpClient para eBay
            services.AddHttpClient("Ebay", client =>
            {
                client.BaseAddress = new Uri(configuration["Ebay:BaseUrl"] ?? "https://api.ebay.com");
                client.DefaultRequestHeaders.Add("User-Agent", "AutoGuia/1.0");
                client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
            })
            .AddPolicyHandler(GetRetryPolicy(maxRetries, retryDelaySeconds))
            .AddPolicyHandler(GetCircuitBreakerPolicy())
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(timeoutSeconds));

            // HttpClient genérico para otros servicios
            services.AddHttpClient("Default", client =>
            {
                client.DefaultRequestHeaders.Add("User-Agent", "AutoGuia/1.0");
                client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
            })
            .AddPolicyHandler(GetRetryPolicy(maxRetries, retryDelaySeconds))
            .AddPolicyHandler(GetCircuitBreakerPolicy());

            return services;
        }

        /// <summary>
        /// Política de reintentos con backoff exponencial
        /// </summary>
        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int maxRetries, int retryDelaySeconds)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError() // Maneja 5xx y 408
                .Or<TimeoutRejectedException>() // Maneja timeouts
                .WaitAndRetryAsync(
                    retryCount: maxRetries,
                    sleepDurationProvider: retryAttempt => 
                        TimeSpan.FromSeconds(Math.Pow(retryDelaySeconds, retryAttempt)),
                    onRetry: (outcome, timespan, retryCount, context) =>
                    {
                        Console.WriteLine(
                            $"⚠️ Retry {retryCount} after {timespan.TotalSeconds}s due to: {outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}");
                    });
        }

        /// <summary>
        /// Política de circuit breaker para evitar llamadas a servicios caídos
        /// </summary>
        private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 5, // Abre el circuito después de 5 fallos
                    durationOfBreak: TimeSpan.FromSeconds(30), // Permanece abierto 30 segundos
                    onBreak: (outcome, duration) =>
                    {
                        Console.WriteLine($"🚨 Circuit breaker abierto por {duration.TotalSeconds}s");
                    },
                    onReset: () =>
                    {
                        Console.WriteLine("✅ Circuit breaker cerrado - servicio restaurado");
                    },
                    onHalfOpen: () =>
                    {
                        Console.WriteLine("🔄 Circuit breaker en half-open - probando servicio");
                    });
        }
    }
}
