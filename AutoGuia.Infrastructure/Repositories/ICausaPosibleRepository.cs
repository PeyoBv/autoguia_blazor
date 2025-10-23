using AutoGuia.Core.DTOs;

namespace AutoGuia.Infrastructure.Repositories;

/// <summary>
/// Interfaz para el repositorio de causas posibles
/// Define operaciones de consulta para causas con sus pasos y recomendaciones
/// </summary>
public interface ICausaPosibleRepository
{
    /// <summary>
    /// Obtiene todas las causas posibles de un síntoma específico
    /// Ordenadas por nivel de probabilidad (mayor a menor)
    /// Incluye pasos de verificación ordenados y recomendaciones preventivas
    /// </summary>
    /// <param name="sintomaId">Identificador del síntoma</param>
    /// <returns>Lista de causas con pasos y recomendaciones anidadas</returns>
    Task<List<CausaPosibleDto>> ObtenerCausasPorSintomaAsync(int sintomaId);

    /// <summary>
    /// Obtiene una causa posible específica por su identificador
    /// Incluye pasos de verificación ordenados y recomendaciones preventivas
    /// </summary>
    /// <param name="id">Identificador de la causa posible</param>
    /// <returns>Causa con pasos y recomendaciones o null si no existe</returns>
    Task<CausaPosibleDto?> ObtenerCausaPorIdAsync(int id);
}
