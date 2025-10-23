using AutoGuia.Core.DTOs;

namespace AutoGuia.Infrastructure.Repositories;

/// <summary>
/// Interfaz para el repositorio de sistemas automotrices
/// Define operaciones de consulta para sistemas con sus síntomas asociados
/// </summary>
public interface ISistemaAutomotrizRepository
{
    /// <summary>
    /// Obtiene todos los sistemas automotrices activos con sus síntomas activos
    /// Útil para mostrar el catálogo completo de sistemas disponibles
    /// </summary>
    /// <returns>Lista de sistemas con síntomas anidados</returns>
    Task<List<SistemaAutomotrizDto>> ObtenerTodosLosSistemasAsync();

    /// <summary>
    /// Obtiene un sistema automotriz específico por su identificador
    /// Incluye sus síntomas activos asociados
    /// </summary>
    /// <param name="id">Identificador del sistema automotriz</param>
    /// <returns>Sistema con síntomas anidados o null si no existe</returns>
    Task<SistemaAutomotrizDto?> ObtenerSistemaPorIdAsync(int id);

    /// <summary>
    /// Busca sistemas automotrices cuyo nombre contenga el texto especificado
    /// Incluye síntomas activos de los sistemas encontrados
    /// </summary>
    /// <param name="nombre">Texto a buscar en el nombre del sistema</param>
    /// <returns>Lista de sistemas que coinciden con la búsqueda</returns>
    Task<List<SistemaAutomotrizDto>> BuscarSistemasPorNombreAsync(string nombre);
}
