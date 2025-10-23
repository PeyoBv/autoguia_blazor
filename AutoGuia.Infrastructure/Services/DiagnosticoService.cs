using AutoGuia.Core.DTOs;
using AutoGuia.Core.Entities;
using AutoGuia.Core.Interfaces;
using AutoGuia.Infrastructure.Repositories;

namespace AutoGuia.Infrastructure.Services;

public class DiagnosticoService : IDiagnosticoService
{
    private readonly ISintomaRepository _sintomaRepository;
    private readonly ICausaPosibleRepository _causaRepository;
    private readonly IConsultaDiagnosticoRepository _consultaRepository;
    private readonly SintomaSearchService _searchService;

    public DiagnosticoService(
        ISintomaRepository sintomaRepository,
        ICausaPosibleRepository causaRepository,
        IConsultaDiagnosticoRepository consultaRepository,
        SintomaSearchService searchService)
    {
        _sintomaRepository = sintomaRepository;
        _causaRepository = causaRepository;
        _consultaRepository = consultaRepository;
        _searchService = searchService;
    }

    public async Task<ResultadoDiagnosticoDto> DiagnosticarSintomaAsync(string descripcionSintoma, int usuarioId)
    {
        // ✅ NUEVO: Usar búsqueda avanzada fuzzy matching
        var sintomasCoincidentes = await _searchService.BuscarSintomasAvanzadoAsync(descripcionSintoma);
        
        var resultado = new ResultadoDiagnosticoDto();

        if (sintomasCoincidentes.Any())
        {
            var sintomaIdentificado = sintomasCoincidentes.First();
            resultado.SintomaIdentificado = sintomaIdentificado.Descripcion;
            resultado.SintomaRelacionadoId = sintomaIdentificado.Id;
            resultado.NivelUrgencia = sintomaIdentificado.NivelUrgencia;

            var causas = await _causaRepository.ObtenerCausasPorSintomaAsync(sintomaIdentificado.Id);
            resultado.CausasPosibles = causas;

            resultado.SugerirServicioProfesional = causas.Any(c => c.RequiereServicioProfesional);
            resultado.Recomendacion = GenerarRecomendacion(sintomaIdentificado.NivelUrgencia, resultado.SugerirServicioProfesional);
        }
        else
        {
            resultado.Recomendacion = "No se encontraron síntomas coincidentes. Por favor, describa el problema con más detalle.";
        }

        var consulta = new ConsultaDiagnostico
        {
            UsuarioId = usuarioId,
            SintomaDescrito = descripcionSintoma,
            RespuestaAsistente = resultado.Recomendacion,
            SintomaRelacionadoId = resultado.SintomaRelacionadoId,
            FechaConsulta = DateTime.UtcNow
        };

        await _consultaRepository.CrearConsultaAsync(consulta);

        return resultado;
    }

    public async Task<List<SintomaDto>> ObtenerSintomasPorSistemaAsync(int sistemaId)
    {
        return await _sintomaRepository.ObtenerSintomasPorSistemaAsync(sistemaId);
    }

    public async Task<CausaPosibleDto?> ObtenerCausaConDetallesAsync(int causaId)
    {
        return await _causaRepository.ObtenerCausaPorIdAsync(causaId);
    }

    public async Task<List<ConsultaDiagnosticoDto>> ObtenerHistorialAsync(int usuarioId)
    {
        return await _consultaRepository.ObtenerConsultasPorUsuarioAsync(usuarioId);
    }

    public async Task RegistrarFeedbackAsync(int consultaId, bool fueUtil)
    {
        await _consultaRepository.ActualizarFeedbackAsync(consultaId, fueUtil);
    }

    private string GenerarRecomendacion(int nivelUrgencia, bool requiereServicio)
    {
        return nivelUrgencia switch
        {
            1 => requiereServicio ? "Problema leve, pero le recomendamos llevar a servicio profesional." : "Problema leve, puede intentar las recomendaciones preventivas.",
            2 => requiereServicio ? "Problema moderado, le recomendamos llevar a servicio profesional pronto." : "Problema moderado, intente los pasos de verificación.",
            3 => requiereServicio ? "Problema importante, DEBE llevar a servicio profesional." : "Problema importante, intente los pasos pero si persiste lleve a servicio.",
            4 => "⚠️ PROBLEMA CRÍTICO: Detenga el vehículo y lleve a servicio profesional INMEDIATAMENTE.",
            _ => "Consulte a un profesional."
        };
    }
}
