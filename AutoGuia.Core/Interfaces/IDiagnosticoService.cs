using AutoGuia.Core.DTOs;

namespace AutoGuia.Core.Interfaces;

/// <summary>
/// Interfaz del servicio de diagnóstico automotriz
/// Define las operaciones principales para análisis de síntomas y gestión de diagnósticos
/// </summary>
public interface IDiagnosticoService
{
    /// <summary>
    /// Diagnostica un síntoma descrito por el usuario y genera un resultado completo
    /// Incluye identificación del síntoma, causas posibles ordenadas por probabilidad,
    /// pasos de verificación y recomendaciones preventivas
    /// </summary>
    /// <param name="descripcionSintoma">Descripción del síntoma en lenguaje natural del usuario</param>
    /// <param name="usuarioId">Identificador del usuario que realiza la consulta</param>
    /// <returns>Resultado completo del diagnóstico con causas, pasos y recomendaciones</returns>
    Task<ResultadoDiagnosticoDto> DiagnosticarSintomaAsync(string descripcionSintoma, int usuarioId);

    /// <summary>
    /// Obtiene todos los síntomas activos de un sistema automotriz específico
    /// Útil para navegación por categorías en la interfaz
    /// </summary>
    /// <param name="sistemaId">Identificador del sistema automotriz</param>
    /// <returns>Lista de síntomas del sistema</returns>
    Task<List<SintomaDto>> ObtenerSintomasPorSistemaAsync(int sistemaId);

    /// <summary>
    /// Obtiene los detalles completos de una causa posible
    /// Incluye pasos de verificación ordenados y recomendaciones preventivas
    /// </summary>
    /// <param name="causaId">Identificador de la causa posible</param>
    /// <returns>Causa con todos sus detalles o null si no existe</returns>
    Task<CausaPosibleDto?> ObtenerCausaConDetallesAsync(int causaId);

    /// <summary>
    /// Obtiene el historial completo de consultas de diagnóstico de un usuario
    /// Ordenado cronológicamente (más recientes primero)
    /// </summary>
    /// <param name="usuarioId">Identificador del usuario</param>
    /// <returns>Lista de consultas previas con respuestas y feedback</returns>
    Task<List<ConsultaDiagnosticoDto>> ObtenerHistorialAsync(int usuarioId);

    /// <summary>
    /// Registra el feedback del usuario sobre la utilidad de un diagnóstico
    /// Utilizado para análisis y mejora continua del sistema
    /// </summary>
    /// <param name="consultaId">Identificador de la consulta</param>
    /// <param name="fueUtil">Indica si la respuesta fue útil para el usuario</param>
    Task RegistrarFeedbackAsync(int consultaId, bool fueUtil);
}
