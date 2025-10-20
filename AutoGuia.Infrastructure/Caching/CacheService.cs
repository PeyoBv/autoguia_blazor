using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AutoGuia.Infrastructure.Caching
{
    /// <summary>
    /// Servicio de caché unificado que soporta Memory Cache y Distributed Cache (Redis)
    /// </summary>
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);
        Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Implementación con Memory Cache (para desarrollo y single-server)
    /// </summary>
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<MemoryCacheService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public MemoryCacheService(
            IMemoryCache memoryCache,
            ILogger<MemoryCacheService> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = false
            };
        }

        public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                var value = _memoryCache.Get<T>(key);
                
                if (value != null)
                {
                    _logger.LogDebug("Cache HIT: {Key}", key);
                }
                else
                {
                    _logger.LogDebug("Cache MISS: {Key}", key);
                }
                
                return Task.FromResult(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener del caché: {Key}", key);
                return Task.FromResult<T?>(default);
            }
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(15),
                    SlidingExpiration = TimeSpan.FromMinutes(5)
                };

                _memoryCache.Set(key, value, cacheOptions);
                _logger.LogDebug("Cache SET: {Key} (expira en {Minutes} minutos)", 
                    key, expiration?.TotalMinutes ?? 15);
                
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar en caché: {Key}", key);
                return Task.CompletedTask;
            }
        }

        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                _memoryCache.Remove(key);
                _logger.LogDebug("Cache REMOVE: {Key}", key);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar del caché: {Key}", key);
                return Task.CompletedTask;
            }
        }

        public Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning("RemoveByPrefix no está implementado para MemoryCache (limitación de la API)");
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Implementación con Distributed Cache (Redis) para multi-server
    /// </summary>
    public class DistributedCacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<DistributedCacheService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public DistributedCacheService(
            IDistributedCache distributedCache,
            ILogger<DistributedCacheService> logger)
        {
            _distributedCache = distributedCache;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = false
            };
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                var bytes = await _distributedCache.GetAsync(key, cancellationToken);
                
                if (bytes == null)
                {
                    _logger.LogDebug("Distributed Cache MISS: {Key}", key);
                    return default;
                }

                var json = System.Text.Encoding.UTF8.GetString(bytes);
                var value = JsonSerializer.Deserialize<T>(json, _jsonOptions);
                
                _logger.LogDebug("Distributed Cache HIT: {Key}", key);
                return value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener del caché distribuido: {Key}", key);
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var json = JsonSerializer.Serialize(value, _jsonOptions);
                var bytes = System.Text.Encoding.UTF8.GetBytes(json);

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(15),
                    SlidingExpiration = TimeSpan.FromMinutes(5)
                };

                await _distributedCache.SetAsync(key, bytes, options, cancellationToken);
                
                _logger.LogDebug("Distributed Cache SET: {Key} (expira en {Minutes} minutos)", 
                    key, expiration?.TotalMinutes ?? 15);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar en caché distribuido: {Key}", key);
            }
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                await _distributedCache.RemoveAsync(key, cancellationToken);
                _logger.LogDebug("Distributed Cache REMOVE: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar del caché distribuido: {Key}", key);
            }
        }

        public Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning("RemoveByPrefix requiere integración con Redis directamente");
            return Task.CompletedTask;
        }
    }
}
