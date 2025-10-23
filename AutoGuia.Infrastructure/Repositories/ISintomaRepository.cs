using AutoGuia.Core.DTOs;

namespace AutoGuia.Infrastructure.Repositories;

/// <summary>
/// Interfaz para el repositorio de síntomas automotrices
/// Define operaciones de consulta para síntomas del sistema de diagnóstico
/// </summary>
public interface ISintomaRepository
{
    /// <summary>
    /// Obtiene todos los síntomas activos de un sistema automotriz específico
    /// </summary>
    /// <param name="sistemaId">Identificador del sistema automotriz</param>
    /// <returns>Lista de síntomas con información del sistema</returns>
    Task<List<SintomaDto>> ObtenerSintomasPorSistemaAsync(int sistemaId);

    /// <summary>
    /// Obtiene un síntoma específico por su identificador
    /// </summary>
    /// <param name="id">Identificador del síntoma</param>
    /// <returns>Síntoma con información del sistema o null si no existe</returns>
    Task<SintomaDto?> ObtenerSintomaPorIdAsync(int id);

    /// <summary>
    /// Busca síntomas que contengan el texto especificado en descripción o descripción técnica
    /// </summary>
    /// <param name="descripcion">Texto a buscar en las descripciones</param>
    /// <returns>Lista de síntomas que coinciden con la búsqueda</returns>
    Task<List<SintomaDto>> BuscarSintomaPorDescripcionAsync(string descripcion);

    /// <summary>
    /// Obtiene todos los síntomas activos del sistema
    /// </summary>
    /// <returns>Lista completa de síntomas activos</returns>
    Task<List<SintomaDto>> ObtenerTodosSintomasActivosAsync();
}
