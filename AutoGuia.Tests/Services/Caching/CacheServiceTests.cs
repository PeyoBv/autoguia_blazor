using Xunit;
using FluentAssertions;
using AutoGuia.Infrastructure.Caching;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace AutoGuia.Tests.Services.Caching
{
    public class CacheServiceTests
    {
        private readonly IMemoryCache _memoryCache;
        private readonly Mock<ILogger<MemoryCacheService>> _loggerMock;
        private readonly MemoryCacheService _cacheService;

        public CacheServiceTests()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _loggerMock = new Mock<ILogger<MemoryCacheService>>();
            _cacheService = new MemoryCacheService(_memoryCache, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAsync_ConClaveNoExistente_DebeRetornarNull()
        {
            // Act
            var result = await _cacheService.GetAsync<string>("clave_inexistente");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task SetAsync_Y_GetAsync_DebenFuncionar()
        {
            // Arrange
            const string clave = "test_key";
            const string valor = "test_value";

            // Act
            await _cacheService.SetAsync(clave, valor);
            var result = await _cacheService.GetAsync<string>(clave);

            // Assert
            result.Should().Be(valor);
        }

        [Fact]
        public async Task SetAsync_ConObjeto_DebeAlmacenarCorrectamente()
        {
            // Arrange
            const string clave = "test_object";
            var objeto = new TestObject { Id = 1, Nombre = "Test" };

            // Act
            await _cacheService.SetAsync(clave, objeto);
            var result = await _cacheService.GetAsync<TestObject>(clave);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.Nombre.Should().Be("Test");
        }

        private class TestObject
        {
            public int Id { get; set; }
            public string Nombre { get; set; } = string.Empty;
        }

        [Fact]
        public async Task RemoveAsync_DebeEliminarClave()
        {
            // Arrange
            const string clave = "test_remove";
            await _cacheService.SetAsync(clave, "valor");

            // Act
            await _cacheService.RemoveAsync(clave);
            var result = await _cacheService.GetAsync<string>(clave);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task SetAsync_ConExpiracion_DebeConfigurarTTL()
        {
            // Arrange
            const string clave = "test_ttl";
            const string valor = "valor_temporal";
            var expiracion = TimeSpan.FromMilliseconds(100);

            // Act
            await _cacheService.SetAsync(clave, valor, expiracion);
            var resultadoInmediato = await _cacheService.GetAsync<string>(clave);
            
            await Task.Delay(150); // Esperar a que expire
            
            var resultadoDespuesDeExpiracion = await _cacheService.GetAsync<string>(clave);

            // Assert
            resultadoInmediato.Should().Be(valor);
            resultadoDespuesDeExpiracion.Should().BeNull();
        }

        [Fact]
        public void CacheKeys_Format_DebeGenerarClaveCorrectamente()
        {
            // Arrange
            const string template = CacheKeys.TalleresPorCiudad;
            const string ciudad = "Santiago";

            // Act
            var clave = CacheKeys.Format(template, ciudad);

            // Assert
            clave.Should().Be("talleres_ciudad_Santiago");
        }

        [Fact]
        public void CacheKeys_TodosLosTalleres_DebeSerConstante()
        {
            // Act & Assert
            CacheKeys.TodosLosTalleres.Should().Be("talleres_all");
        }

        [Fact]
        public void CacheKeys_MercadoLibreBusqueda_DebeTenerFormatoEsperado()
        {
            // Arrange
            const string query = "frenos";
            const string categoria = "auto";
            const int limite = 10;

            // Act
            var clave = CacheKeys.Format(CacheKeys.MercadoLibreBusqueda, query, categoria, limite);

            // Assert
            clave.Should().Be("ml_search_frenos_auto_10");
        }
    }
}
