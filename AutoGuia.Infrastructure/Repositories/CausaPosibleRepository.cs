using AutoGuia.Core.Entities;
using AutoGuia.Core.DTOs;
using Microsoft.EntityFrameworkCore;
using AutoGuia.Infrastructure.Data;

namespace AutoGuia.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio de causas posibles
/// Proporciona acceso a causas con proyecciones anidadas optimizadas de pasos y recomendaciones
/// </summary>
public class CausaPosibleRepository : ICausaPosibleRepository
{
    private readonly AutoGuiaDbContext _context;

    public CausaPosibleRepository(AutoGuiaDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtiene todas las causas posibles de un síntoma ordenadas por probabilidad
    /// Incluye pasos ordenados secuencialmente y recomendaciones preventivas
    /// </summary>
    public async Task<List<CausaPosibleDto>> ObtenerCausasPorSintomaAsync(int sintomaId)
    {
        return await _context.CausasPosibles
            .Where(cp => cp.SintomaId == sintomaId)
            .OrderByDescending(cp => cp.NivelProbabilidad)
            .Select(cp => new CausaPosibleDto
            {
                Id = cp.Id,
                Descripcion = cp.Descripcion,
                DescripcionDetallada = cp.DescripcionDetallada,
                NivelProbabilidad = cp.NivelProbabilidad,
                RequiereServicioProfesional = cp.RequiereServicioProfesional,
                PasosVerificacion = cp.PasosVerificacion
                    .OrderBy(pv => pv.Orden)
                    .Select(pv => new PasoVerificacionDto
                    {
                        Id = pv.Id,
                        Orden = pv.Orden,
                        Descripcion = pv.Descripcion,
                        InstruccionesDetalladas = pv.InstruccionesDetalladas,
                        IndicadoresExito = pv.IndicadoresExito
                    })
                    .ToList(),
                Recomendaciones = cp.Recomendaciones
                    .Select(rp => new RecomendacionPreventivaDto
                    {
                        Id = rp.Id,
                        Descripcion = rp.Descripcion,
                        Detalle = rp.Detalle,
                        FrecuenciaKilometros = rp.FrecuenciaKilometros,
                        FrecuenciaMeses = rp.FrecuenciaMeses
                    })
                    .ToList()
            })
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene una causa posible específica con todos sus detalles
    /// Incluye pasos ordenados secuencialmente y recomendaciones preventivas
    /// </summary>
    public async Task<CausaPosibleDto?> ObtenerCausaPorIdAsync(int id)
    {
        return await _context.CausasPosibles
            .Where(cp => cp.Id == id)
            .Select(cp => new CausaPosibleDto
            {
                Id = cp.Id,
                Descripcion = cp.Descripcion,
                DescripcionDetallada = cp.DescripcionDetallada,
                NivelProbabilidad = cp.NivelProbabilidad,
                RequiereServicioProfesional = cp.RequiereServicioProfesional,
                PasosVerificacion = cp.PasosVerificacion
                    .OrderBy(pv => pv.Orden)
                    .Select(pv => new PasoVerificacionDto
                    {
                        Id = pv.Id,
                        Orden = pv.Orden,
                        Descripcion = pv.Descripcion,
                        InstruccionesDetalladas = pv.InstruccionesDetalladas,
                        IndicadoresExito = pv.IndicadoresExito
                    })
                    .ToList(),
                Recomendaciones = cp.Recomendaciones
                    .Select(rp => new RecomendacionPreventivaDto
                    {
                        Id = rp.Id,
                        Descripcion = rp.Descripcion,
                        Detalle = rp.Detalle,
                        FrecuenciaKilometros = rp.FrecuenciaKilometros,
                        FrecuenciaMeses = rp.FrecuenciaMeses
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();
    }
}
