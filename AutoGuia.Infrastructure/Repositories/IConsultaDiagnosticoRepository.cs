using AutoGuia.Core.Entities;
using AutoGuia.Core.DTOs;

namespace AutoGuia.Infrastructure.Repositories;

/// <summary>
/// Interfaz para el repositorio de consultas de diagnóstico
/// Define operaciones para gestionar el historial de consultas y feedback de usuarios
/// </summary>
public interface IConsultaDiagnosticoRepository
{
    /// <summary>
    /// Obtiene todas las consultas de un usuario específico
    /// Ordenadas por fecha de consulta (más recientes primero)
    /// </summary>
    /// <param name="usuarioId">Identificador del usuario</param>
    /// <returns>Lista de consultas con información del síntoma relacionado</returns>
    Task<List<ConsultaDiagnosticoDto>> ObtenerConsultasPorUsuarioAsync(int usuarioId);

    /// <summary>
    /// Obtiene una consulta específica por su identificador
    /// </summary>
    /// <param name="id">Identificador de la consulta</param>
    /// <returns>Consulta con información completa o null si no existe</returns>
    Task<ConsultaDiagnosticoDto?> ObtenerConsultaPorIdAsync(int id);

    /// <summary>
    /// Crea una nueva consulta de diagnóstico en la base de datos
    /// </summary>
    /// <param name="consulta">Entidad de consulta a crear</param>
    Task CrearConsultaAsync(ConsultaDiagnostico consulta);

    /// <summary>
    /// Actualiza el feedback (fue útil o no) de una consulta existente
    /// </summary>
    /// <param name="consultaId">Identificador de la consulta</param>
    /// <param name="fueUtil">Indica si la respuesta fue útil para el usuario</param>
    Task ActualizarFeedbackAsync(int consultaId, bool fueUtil);
}
