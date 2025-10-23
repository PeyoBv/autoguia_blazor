using AutoGuia.Core.DTOs;
using AutoGuia.Core.Interfaces;
using AutoGuia.Infrastructure.Repositories;

namespace AutoGuia.Infrastructure.Services;

/// <summary>
/// Servicio de sistemas automotrices que orquesta el repositorio
/// Implementa validaciones de parámetros y lógica de negocio
/// </summary>
public class SistemaAutomotrizService : ISistemaAutomotrizService
{
    private readonly ISistemaAutomotrizRepository _sistemaRepository;

    public SistemaAutomotrizService(ISistemaAutomotrizRepository sistemaRepository)
    {
        _sistemaRepository = sistemaRepository;
    }

    /// <summary>
    /// Obtiene todos los sistemas automotrices activos con sus síntomas
    /// </summary>
    public async Task<List<SistemaAutomotrizDto>> ObtenerTodosLosSistemasAsync()
    {
        return await _sistemaRepository.ObtenerTodosLosSistemasAsync();
    }

    /// <summary>
    /// Obtiene un sistema automotriz específico por su identificador
    /// Valida que el ID sea mayor que cero
    /// </summary>
    public async Task<SistemaAutomotrizDto?> ObtenerSistemaPorIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("El ID del sistema debe ser mayor que cero", nameof(id));

        return await _sistemaRepository.ObtenerSistemaPorIdAsync(id);
    }

    /// <summary>
    /// Busca sistemas automotrices por nombre
    /// Valida que el nombre no esté vacío y elimina espacios
    /// </summary>
    public async Task<List<SistemaAutomotrizDto>> BuscarSistemasPorNombreAsync(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre de búsqueda no puede estar vacío", nameof(nombre));

        return await _sistemaRepository.BuscarSistemasPorNombreAsync(nombre.Trim());
    }
}
