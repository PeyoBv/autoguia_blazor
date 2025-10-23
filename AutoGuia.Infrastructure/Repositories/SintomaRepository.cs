using AutoGuia.Core.Entities;
using AutoGuia.Core.DTOs;
using Microsoft.EntityFrameworkCore;
using AutoGuia.Infrastructure.Data;

namespace AutoGuia.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio de síntomas automotrices
/// Proporciona acceso a datos de síntomas con proyecciones optimizadas
/// </summary>
public class SintomaRepository : ISintomaRepository
{
    private readonly AutoGuiaDbContext _context;

    public SintomaRepository(AutoGuiaDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtiene todos los síntomas activos de un sistema automotriz específico
    /// </summary>
    public async Task<List<SintomaDto>> ObtenerSintomasPorSistemaAsync(int sistemaId)
    {
        return await _context.Sintomas
            .Where(s => s.SistemaAutomotrizId == sistemaId && s.EsActivo)
            .Select(s => new SintomaDto
            {
                Id = s.Id,
                Descripcion = s.Descripcion,
                DescripcionTecnica = s.DescripcionTecnica,
                NivelUrgencia = s.NivelUrgencia,
                SistemaAutomotrizId = s.SistemaAutomotrizId,
                NombreSistema = s.SistemaAutomotriz.Nombre
            })
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene un síntoma específico por su identificador
    /// </summary>
    public async Task<SintomaDto?> ObtenerSintomaPorIdAsync(int id)
    {
        return await _context.Sintomas
            .Where(s => s.Id == id && s.EsActivo)
            .Select(s => new SintomaDto
            {
                Id = s.Id,
                Descripcion = s.Descripcion,
                DescripcionTecnica = s.DescripcionTecnica,
                NivelUrgencia = s.NivelUrgencia,
                SistemaAutomotrizId = s.SistemaAutomotrizId,
                NombreSistema = s.SistemaAutomotriz.Nombre
            })
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Busca síntomas que contengan el texto especificado en descripción o descripción técnica
    /// </summary>
    public async Task<List<SintomaDto>> BuscarSintomaPorDescripcionAsync(string descripcion)
    {
        return await _context.Sintomas
            .Where(s => (s.Descripcion.Contains(descripcion) || s.DescripcionTecnica.Contains(descripcion)) && s.EsActivo)
            .Select(s => new SintomaDto
            {
                Id = s.Id,
                Descripcion = s.Descripcion,
                DescripcionTecnica = s.DescripcionTecnica,
                NivelUrgencia = s.NivelUrgencia,
                SistemaAutomotrizId = s.SistemaAutomotrizId,
                NombreSistema = s.SistemaAutomotriz.Nombre
            })
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene todos los síntomas activos del sistema
    /// </summary>
    public async Task<List<SintomaDto>> ObtenerTodosSintomasActivosAsync()
    {
        return await _context.Sintomas
            .Where(s => s.EsActivo)
            .Select(s => new SintomaDto
            {
                Id = s.Id,
                Descripcion = s.Descripcion,
                DescripcionTecnica = s.DescripcionTecnica,
                NivelUrgencia = s.NivelUrgencia,
                SistemaAutomotrizId = s.SistemaAutomotrizId,
                NombreSistema = s.SistemaAutomotriz.Nombre
            })
            .ToListAsync();
    }
}
