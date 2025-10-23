using AutoGuia.Core.DTOs;

namespace AutoGuia.Core.Interfaces;

/// <summary>
/// Interfaz del servicio de sistemas automotrices
/// Define las operaciones para gestionar y consultar sistemas del vehículo
/// </summary>
public interface ISistemaAutomotrizService
{
    /// <summary>
    /// Obtiene todos los sistemas automotrices activos con sus síntomas
    /// Útil para mostrar el catálogo completo en la interfaz de usuario
    /// </summary>
    /// <returns>Lista completa de sistemas con síntomas anidados</returns>
    Task<List<SistemaAutomotrizDto>> ObtenerTodosLosSistemasAsync();

    /// <summary>
    /// Obtiene un sistema automotriz específico por su identificador
    /// Incluye todos sus síntomas activos asociados
    /// </summary>
    /// <param name="id">Identificador del sistema automotriz</param>
    /// <returns>Sistema con síntomas o null si no existe</returns>
    Task<SistemaAutomotrizDto?> ObtenerSistemaPorIdAsync(int id);

    /// <summary>
    /// Busca sistemas automotrices por nombre
    /// Realiza búsqueda parcial (contiene el texto especificado)
    /// </summary>
    /// <param name="nombre">Texto a buscar en el nombre del sistema</param>
    /// <returns>Lista de sistemas que coinciden con la búsqueda</returns>
    Task<List<SistemaAutomotrizDto>> BuscarSistemasPorNombreAsync(string nombre);
}
