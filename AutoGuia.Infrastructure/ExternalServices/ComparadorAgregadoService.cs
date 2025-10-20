using AutoGuia.Core.DTOs;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace AutoGuia.Infrastructure.ExternalServices
{
    /// <summary>
    /// Servicio agregador que busca productos en múltiples marketplaces simultáneamente
    /// y consolida los resultados en una lista unificada ordenada por precio
    /// </summary>
    public class ComparadorAgregadoService
    {
        private readonly IEnumerable<IExternalMarketplaceService> _marketplaceServices;
        private readonly ILogger<ComparadorAgregadoService> _logger;

        public ComparadorAgregadoService(
            IEnumerable<IExternalMarketplaceService> marketplaceServices,
            ILogger<ComparadorAgregadoService> logger)
        {
            _marketplaceServices = marketplaceServices;
            _logger = logger;
        }

        /// <summary>
        /// Busca productos en todos los marketplaces disponibles en paralelo
        /// </summary>
        /// <param name="termino">Término de búsqueda</param>
        /// <param name="categoria">Categoría opcional</param>
        /// <param name="limiteResultados">Límite total de resultados a devolver</param>
        /// <returns>Lista consolidada de ofertas ordenadas por precio</returns>
        public async Task<ResultadoBusquedaAgregadaDto> BuscarEnTodosLosMarketplacesAsync(
            string termino,
            string? categoria = null,
            int limiteResultados = 50)
        {
            if (string.IsNullOrWhiteSpace(termino))
            {
                _logger.LogWarning("Término de búsqueda vacío");
                return new ResultadoBusquedaAgregadaDto();
            }

            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation(
                "Iniciando búsqueda agregada: Término={Termino}, Categoría={Categoria}", 
                termino, categoria);

            var resultado = new ResultadoBusquedaAgregadaDto
            {
                TerminoBusqueda = termino,
                Categoria = categoria
            };

            try
            {
                // Ejecutar búsquedas en paralelo en todos los marketplaces
                var tareasBusqueda = _marketplaceServices
                    .Select(async service =>
                    {
                        var servicioStopwatch = Stopwatch.StartNew();
                        try
                        {
                            _logger.LogDebug("Buscando en {Marketplace}...", service.NombreMarketplace);
                            
                            var ofertas = await service.BuscarProductosAsync(termino, categoria, 20);
                            var ofertasList = ofertas.ToList();
                            
                            servicioStopwatch.Stop();
                            
                            resultado.MarketplacesConsultados.Add(new MarketplaceResultadoDto
                            {
                                NombreMarketplace = service.NombreMarketplace,
                                CantidadResultados = ofertasList.Count,
                                TiempoRespuestaMs = (int)servicioStopwatch.ElapsedMilliseconds,
                                ExitosoBusqueda = true
                            });

                            _logger.LogInformation(
                                "✅ {Marketplace}: {Count} resultados en {Ms}ms",
                                service.NombreMarketplace,
                                ofertasList.Count,
                                servicioStopwatch.ElapsedMilliseconds);

                            return ofertasList;
                        }
                        catch (Exception ex)
                        {
                            servicioStopwatch.Stop();
                            
                            _logger.LogError(ex, 
                                "❌ Error al buscar en {Marketplace}", 
                                service.NombreMarketplace);

                            resultado.MarketplacesConsultados.Add(new MarketplaceResultadoDto
                            {
                                NombreMarketplace = service.NombreMarketplace,
                                CantidadResultados = 0,
                                TiempoRespuestaMs = (int)servicioStopwatch.ElapsedMilliseconds,
                                ExitosoBusqueda = false,
                                MensajeError = ex.Message
                            });

                            return Enumerable.Empty<OfertaExternaDto>();
                        }
                    })
                    .ToList();

                // Esperar a que todas las búsquedas terminen
                var resultadosBusqueda = await Task.WhenAll(tareasBusqueda);

                // Consolidar y ordenar todos los resultados
                var todasLasOfertas = resultadosBusqueda
                    .SelectMany(ofertas => ofertas)
                    .OrderBy(o => o.Precio)
                    .Take(limiteResultados)
                    .ToList();

                resultado.Ofertas = todasLasOfertas;
                resultado.TotalResultados = todasLasOfertas.Count;

                // Calcular estadísticas
                if (todasLasOfertas.Any())
                {
                    resultado.PrecioMinimo = todasLasOfertas.Min(o => o.Precio);
                    resultado.PrecioMaximo = todasLasOfertas.Max(o => o.Precio);
                    resultado.PrecioPromedio = todasLasOfertas.Average(o => o.Precio);
                }

                stopwatch.Stop();
                resultado.TiempoTotalMs = (int)stopwatch.ElapsedMilliseconds;

                _logger.LogInformation(
                    "✅ Búsqueda agregada completada: {Total} resultados de {Marketplaces} marketplaces en {Ms}ms",
                    resultado.TotalResultados,
                    resultado.MarketplacesConsultados.Count,
                    resultado.TiempoTotalMs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico en búsqueda agregada");
                resultado.TiempoTotalMs = (int)stopwatch.ElapsedMilliseconds;
            }

            return resultado;
        }

        /// <summary>
        /// Verifica la disponibilidad de todos los marketplaces
        /// </summary>
        public async Task<Dictionary<string, bool>> VerificarDisponibilidadMarketplacesAsync()
        {
            _logger.LogInformation("Verificando disponibilidad de marketplaces...");

            var resultados = new Dictionary<string, bool>();

            var tareas = _marketplaceServices.Select(async service =>
            {
                try
                {
                    var disponible = await service.EstaDisponibleAsync();
                    _logger.LogInformation(
                        "{Marketplace}: {Status}",
                        service.NombreMarketplace,
                        disponible ? "✅ Disponible" : "❌ No disponible");
                    
                    return (service.NombreMarketplace, disponible);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al verificar {Marketplace}", service.NombreMarketplace);
                    return (service.NombreMarketplace, false);
                }
            });

            var resultadosArray = await Task.WhenAll(tareas);

            foreach (var (marketplace, disponible) in resultadosArray)
            {
                resultados[marketplace] = disponible;
            }

            return resultados;
        }

        /// <summary>
        /// Obtiene todas las categorías disponibles de todos los marketplaces
        /// </summary>
        public async Task<Dictionary<string, IEnumerable<CategoriaMarketplaceDto>>> ObtenerTodasLasCategoriasAsync()
        {
            _logger.LogInformation("Obteniendo categorías de todos los marketplaces...");

            var resultados = new Dictionary<string, IEnumerable<CategoriaMarketplaceDto>>();

            var tareas = _marketplaceServices.Select(async service =>
            {
                try
                {
                    var categorias = await service.ObtenerCategoriasAsync();
                    return (service.NombreMarketplace, categorias);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al obtener categorías de {Marketplace}", service.NombreMarketplace);
                    return (service.NombreMarketplace, Enumerable.Empty<CategoriaMarketplaceDto>());
                }
            });

            var resultadosArray = await Task.WhenAll(tareas);

            foreach (var (marketplace, categorias) in resultadosArray)
            {
                resultados[marketplace] = categorias;
            }

            return resultados;
        }
    }

    /// <summary>
    /// DTO para resultados de búsqueda agregada
    /// </summary>
    public class ResultadoBusquedaAgregadaDto
    {
        public string TerminoBusqueda { get; set; } = string.Empty;
        public string? Categoria { get; set; }
        public List<OfertaExternaDto> Ofertas { get; set; } = new();
        public int TotalResultados { get; set; }
        public decimal PrecioMinimo { get; set; }
        public decimal PrecioMaximo { get; set; }
        public decimal PrecioPromedio { get; set; }
        public List<MarketplaceResultadoDto> MarketplacesConsultados { get; set; } = new();
        public int TiempoTotalMs { get; set; }
        public DateTime FechaBusqueda { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// DTO para resultados individuales de cada marketplace
    /// </summary>
    public class MarketplaceResultadoDto
    {
        public string NombreMarketplace { get; set; } = string.Empty;
        public int CantidadResultados { get; set; }
        public int TiempoRespuestaMs { get; set; }
        public bool ExitosoBusqueda { get; set; }
        public string? MensajeError { get; set; }
    }
}
