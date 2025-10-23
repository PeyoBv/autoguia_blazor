using AutoGuia.Core.Entities;
using AutoGuia.Core.DTOs;
using Microsoft.EntityFrameworkCore;
using AutoGuia.Infrastructure.Data;

namespace AutoGuia.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio de consultas de diagnóstico
/// Gestiona el historial de consultas, feedback y auditoría del sistema
/// </summary>
public class ConsultaDiagnosticoRepository : IConsultaDiagnosticoRepository
{
    private readonly AutoGuiaDbContext _context;

    public ConsultaDiagnosticoRepository(AutoGuiaDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtiene todas las consultas de un usuario ordenadas cronológicamente
    /// Maneja correctamente síntomas relacionados nullables
    /// </summary>
    public async Task<List<ConsultaDiagnosticoDto>> ObtenerConsultasPorUsuarioAsync(int usuarioId)
    {
        return await _context.ConsultasDiagnostico
            .Where(cd => cd.UsuarioId == usuarioId)
            .OrderByDescending(cd => cd.FechaConsulta)
            .Select(cd => new ConsultaDiagnosticoDto
            {
                Id = cd.Id,
                SintomaDescrito = cd.SintomaDescrito,
                FechaConsulta = cd.FechaConsulta,
                RespuestaAsistente = cd.RespuestaAsistente,
                FueUtil = cd.FueUtil,
                SintomaRelacionadoId = cd.SintomaRelacionadoId,
                NombreSintomaRelacionado = cd.SintomaRelacionado != null ? cd.SintomaRelacionado.Descripcion : null
            })
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene una consulta específica con todos sus detalles
    /// Maneja correctamente síntomas relacionados nullables
    /// </summary>
    public async Task<ConsultaDiagnosticoDto?> ObtenerConsultaPorIdAsync(int id)
    {
        return await _context.ConsultasDiagnostico
            .Where(cd => cd.Id == id)
            .Select(cd => new ConsultaDiagnosticoDto
            {
                Id = cd.Id,
                SintomaDescrito = cd.SintomaDescrito,
                FechaConsulta = cd.FechaConsulta,
                RespuestaAsistente = cd.RespuestaAsistente,
                FueUtil = cd.FueUtil,
                SintomaRelacionadoId = cd.SintomaRelacionadoId,
                NombreSintomaRelacionado = cd.SintomaRelacionado != null ? cd.SintomaRelacionado.Descripcion : null
            })
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Crea una nueva consulta de diagnóstico
    /// Persiste inmediatamente en la base de datos
    /// </summary>
    public async Task CrearConsultaAsync(ConsultaDiagnostico consulta)
    {
        _context.ConsultasDiagnostico.Add(consulta);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Actualiza el feedback del usuario sobre la utilidad de la respuesta
    /// Utilizado para análisis y mejora del sistema
    /// </summary>
    public async Task ActualizarFeedbackAsync(int consultaId, bool fueUtil)
    {
        var consulta = await _context.ConsultasDiagnostico.FindAsync(consultaId);
        if (consulta != null)
        {
            consulta.FueUtil = fueUtil;
            await _context.SaveChangesAsync();
        }
    }
}
