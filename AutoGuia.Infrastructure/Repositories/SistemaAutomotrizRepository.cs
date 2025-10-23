using AutoGuia.Core.Entities;
using AutoGuia.Core.DTOs;
using Microsoft.EntityFrameworkCore;
using AutoGuia.Infrastructure.Data;

namespace AutoGuia.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio de sistemas automotrices
/// Proporciona acceso a sistemas con proyecciones optimizadas de síntomas anidados
/// </summary>
public class SistemaAutomotrizRepository : ISistemaAutomotrizRepository
{
    private readonly AutoGuiaDbContext _context;

    public SistemaAutomotrizRepository(AutoGuiaDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtiene todos los sistemas automotrices activos con sus síntomas activos
    /// Filtra por EsActivo en ambas entidades para mostrar solo contenido vigente
    /// </summary>
    public async Task<List<SistemaAutomotrizDto>> ObtenerTodosLosSistemasAsync()
    {
        return await _context.SistemasAutomotrices
            .Where(sa => sa.EsActivo)
            .Select(sa => new SistemaAutomotrizDto
            {
                Id = sa.Id,
                Nombre = sa.Nombre,
                Descripcion = sa.Descripcion,
                Sintomas = sa.Sintomas
                    .Where(s => s.EsActivo)
                    .Select(s => new SintomaDto
                    {
                        Id = s.Id,
                        Descripcion = s.Descripcion,
                        DescripcionTecnica = s.DescripcionTecnica,
                        NivelUrgencia = s.NivelUrgencia,
                        SistemaAutomotrizId = s.SistemaAutomotrizId,
                        NombreSistema = sa.Nombre
                    })
                    .ToList()
            })
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene un sistema automotriz específico con sus síntomas activos
    /// Filtra por EsActivo en ambas entidades
    /// </summary>
    public async Task<SistemaAutomotrizDto?> ObtenerSistemaPorIdAsync(int id)
    {
        return await _context.SistemasAutomotrices
            .Where(sa => sa.Id == id && sa.EsActivo)
            .Select(sa => new SistemaAutomotrizDto
            {
                Id = sa.Id,
                Nombre = sa.Nombre,
                Descripcion = sa.Descripcion,
                Sintomas = sa.Sintomas
                    .Where(s => s.EsActivo)
                    .Select(s => new SintomaDto
                    {
                        Id = s.Id,
                        Descripcion = s.Descripcion,
                        DescripcionTecnica = s.DescripcionTecnica,
                        NivelUrgencia = s.NivelUrgencia,
                        SistemaAutomotrizId = s.SistemaAutomotrizId,
                        NombreSistema = sa.Nombre
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Busca sistemas automotrices por nombre (búsqueda parcial)
    /// Filtra por EsActivo en sistemas y síntomas
    /// </summary>
    public async Task<List<SistemaAutomotrizDto>> BuscarSistemasPorNombreAsync(string nombre)
    {
        return await _context.SistemasAutomotrices
            .Where(sa => sa.Nombre.Contains(nombre) && sa.EsActivo)
            .Select(sa => new SistemaAutomotrizDto
            {
                Id = sa.Id,
                Nombre = sa.Nombre,
                Descripcion = sa.Descripcion,
                Sintomas = sa.Sintomas
                    .Where(s => s.EsActivo)
                    .Select(s => new SintomaDto
                    {
                        Id = s.Id,
                        Descripcion = s.Descripcion,
                        DescripcionTecnica = s.DescripcionTecnica,
                        NivelUrgencia = s.NivelUrgencia,
                        SistemaAutomotrizId = s.SistemaAutomotrizId,
                        NombreSistema = sa.Nombre
                    })
                    .ToList()
            })
            .ToListAsync();
    }
}
