using Microsoft.AspNetCore.Mvc;
using AutoGuia.Core.DTOs;
using AutoGuia.Core.Interfaces;

namespace AutoGuia.Web.Controllers;

/// <summary>
/// Controlador API para sistemas automotrices
/// Proporciona endpoints REST para consulta y búsqueda de sistemas del vehículo
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SistemasController : ControllerBase
{
    private readonly ISistemaAutomotrizService _sistemaService;

    public SistemasController(ISistemaAutomotrizService sistemaService)
    {
        _sistemaService = sistemaService;
    }

    /// <summary>
    /// Obtiene todos los sistemas automotrices activos con sus síntomas
    /// </summary>
    /// <returns>Lista completa de sistemas con síntomas anidados</returns>
    [HttpGet]
    public async Task<IActionResult> ObtenerTodosSistemas()
    {
        try
        {
            var sistemas = await _sistemaService.ObtenerTodosLosSistemasAsync();
            return Ok(sistemas);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error al obtener sistemas", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene un sistema automotriz específico por su identificador
    /// </summary>
    /// <param name="id">Identificador del sistema automotriz</param>
    /// <returns>Sistema con síntomas o NotFound si no existe</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerSistemaPorId(int id)
    {
        try
        {
            var sistema = await _sistemaService.ObtenerSistemaPorIdAsync(id);
            if (sistema == null)
                return NotFound(new { mensaje = "Sistema no encontrado" });

            return Ok(sistema);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error al obtener sistema", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Busca sistemas automotrices por nombre (búsqueda parcial)
    /// </summary>
    /// <param name="nombre">Texto a buscar en el nombre del sistema</param>
    /// <returns>Lista de sistemas que coinciden con la búsqueda</returns>
    [HttpGet("buscar/{nombre}")]
    public async Task<IActionResult> BuscarSistemasPorNombre(string nombre)
    {
        try
        {
            var sistemas = await _sistemaService.BuscarSistemasPorNombreAsync(nombre);
            return Ok(sistemas);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error al buscar sistemas", detalle = ex.Message });
        }
    }
}
