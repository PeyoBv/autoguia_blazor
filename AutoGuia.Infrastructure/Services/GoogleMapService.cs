using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using AutoGuia.Core.Entities;
using AutoGuia.Core.Services;
using AutoGuia.Core.DTOs;

namespace AutoGuia.Infrastructure.Services
{
    /// <summary>
    /// Implementación del servicio de mapas usando Google Maps Platform
    /// </summary>
    public class GoogleMapService : IMapService
    {
        private readonly IJSRuntime _jsRuntime;

        public GoogleMapService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        /// <summary>
        /// Inicializa el mapa de Google Maps con los talleres especificados
        /// </summary>
        public async Task InicializarMapaAsync(object mapElement, IEnumerable<Taller> talleres, string apiKey)
        {
            try
            {
                // Convertir talleres a DTOs para JS
                var talleresData = talleres.Select(ConvertirTallerAMarcador).ToArray();

                // Invocar función JavaScript para inicializar el mapa
                await _jsRuntime.InvokeAsync<string>("autoguiaMap.initMap", mapElement, talleresData, apiKey);
            }
            catch (Exception ex)
            {
                // Log del error (aquí puedes usar tu sistema de logging preferido)
                Console.WriteLine($"Error al inicializar mapa: {ex.Message}");
                throw new InvalidOperationException("No se pudo inicializar el mapa de Google Maps", ex);
            }
        }

        /// <summary>
        /// Agrega un marcador individual al mapa
        /// </summary>
        public async Task AgregarMarcadorAsync(Taller taller)
        {
            try
            {
                var marcadorData = ConvertirTallerAMarcador(taller);
                await _jsRuntime.InvokeVoidAsync("autoguiaMap.addSingleMarker", marcadorData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al agregar marcador: {ex.Message}");
                throw new InvalidOperationException("No se pudo agregar el marcador al mapa", ex);
            }
        }

        /// <summary>
        /// Centra el mapa en las coordenadas especificadas
        /// </summary>
        public async Task CentrarMapaAsync(double latitud, double longitud, int zoom = 12)
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("autoguiaMap.centerMap", latitud, longitud, zoom);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al centrar mapa: {ex.Message}");
                throw new InvalidOperationException("No se pudo centrar el mapa", ex);
            }
        }

        /// <summary>
        /// Limpia todos los marcadores del mapa
        /// </summary>
        public async Task LimpiarMarcadoresAsync()
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("autoguiaMap.clearMarkers");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al limpiar marcadores: {ex.Message}");
                throw new InvalidOperationException("No se pudieron limpiar los marcadores", ex);
            }
        }

        /// <summary>
        /// Convierte una entidad Taller a un DTO MarcadorMapaDto para JavaScript
        /// </summary>
        private static MarcadorMapaDto ConvertirTallerAMarcador(Taller taller)
        {
            return new MarcadorMapaDto
            {
                Id = taller.Id,
                Titulo = taller.Nombre,
                Descripcion = taller.Descripcion ?? string.Empty,
                Latitud = taller.Latitud ?? 0,
                Longitud = taller.Longitud ?? 0,
                Direccion = taller.Direccion,
                Telefono = taller.Telefono ?? string.Empty,
                Email = taller.Email ?? string.Empty,
                EsVerificado = taller.EsVerificado,
                CalificacionPromedio = (double)(taller.CalificacionPromedio ?? 0),
                IconoUrl = string.Empty // Se maneja en JS según el estado de verificación
            };
        }
    }
}