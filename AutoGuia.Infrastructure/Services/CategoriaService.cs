#nullable enable

using System.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoGuia.Core.DTOs;
using AutoGuia.Core.Entities;
using AutoGuia.Infrastructure.Data;

namespace AutoGuia.Infrastructure.Services;

/// <summary>
/// Implementación del servicio para la gestión de categorías, subcategorías y valores de filtro de consumibles automotrices
/// </summary>
public class CategoriaService : ICategoriaService
{
    private readonly AutoGuiaDbContext _context;
    private readonly ILogger<CategoriaService> _logger;

    public CategoriaService(AutoGuiaDbContext context, ILogger<CategoriaService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las categorías activas con sus subcategorías y valores de filtro
    /// </summary>
    public async Task<IEnumerable<CategoriaDto>> ObtenerCategoriasAsync()
    {
        try
        {
            _logger.LogInformation("🔍 Obteniendo todas las categorías activas con jerarquía completa");

            var categorias = await _context.Categorias
                .Where(c => c.EsActivo)
                .Include(c => c.Subcategorias)
                    .ThenInclude(s => s.Valores)
                .OrderBy(c => c.Nombre)
                .Select(c => new CategoriaDto
                {
                    Id = c.Id,
                    Nombre = HttpUtility.HtmlEncode(c.Nombre),
                    Descripcion = c.Descripcion != null ? HttpUtility.HtmlEncode(c.Descripcion) : null,
                    IconUrl = c.IconUrl,
                    Subcategorias = c.Subcategorias
                        .OrderBy(s => s.Nombre)
                        .Select(s => new SubcategoriaDto
                        {
                            Id = s.Id,
                            Nombre = HttpUtility.HtmlEncode(s.Nombre),
                            Valores = s.Valores
                                .OrderBy(v => v.Valor)
                                .Select(v => new ValorFiltroDto
                                {
                                    Id = v.Id,
                                    Valor = HttpUtility.HtmlEncode(v.Valor)
                                }).ToList()
                        }).ToList()
                })
                .ToListAsync();

            _logger.LogInformation("✅ Se obtuvieron {Count} categorías activas exitosamente", categorias.Count);
            return categorias;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener las categorías activas");
            throw;
        }
    }

    /// <summary>
    /// Obtiene las subcategorías asociadas a una categoría específica
    /// </summary>
    public async Task<IEnumerable<SubcategoriaDto>> ObtenerSubcategoriasAsync(int categoriaId)
    {
        try
        {
            _logger.LogInformation("🔍 Obteniendo subcategorías para categoría ID: {CategoriaId}", categoriaId);

            // Verificar que la categoría existe y está activa
            var categoriaExiste = await _context.Categorias
                .AnyAsync(c => c.Id == categoriaId && c.EsActivo);

            if (!categoriaExiste)
            {
                _logger.LogWarning("⚠️ La categoría con ID {CategoriaId} no existe o no está activa", categoriaId);
                return Enumerable.Empty<SubcategoriaDto>();
            }

            var subcategorias = await _context.Subcategorias
                .Where(s => s.CategoriaId == categoriaId)
                .Include(s => s.Valores)
                .OrderBy(s => s.Nombre)
                .Select(s => new SubcategoriaDto
                {
                    Id = s.Id,
                    Nombre = HttpUtility.HtmlEncode(s.Nombre),
                    Valores = s.Valores
                        .OrderBy(v => v.Valor)
                        .Select(v => new ValorFiltroDto
                        {
                            Id = v.Id,
                            Valor = HttpUtility.HtmlEncode(v.Valor)
                        }).ToList()
                })
                .ToListAsync();

            _logger.LogInformation("✅ Se obtuvieron {Count} subcategorías para categoría ID: {CategoriaId}", 
                subcategorias.Count, categoriaId);
            return subcategorias;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener subcategorías para categoría ID: {CategoriaId}", categoriaId);
            throw;
        }
    }

    /// <summary>
    /// Obtiene los valores de filtro disponibles para una subcategoría específica
    /// </summary>
    public async Task<IEnumerable<ValorFiltroDto>> ObtenerValoresFiltroAsync(int subcategoriaId)
    {
        try
        {
            _logger.LogInformation("🔍 Obteniendo valores de filtro para subcategoría ID: {SubcategoriaId}", subcategoriaId);

            // Verificar que la subcategoría existe
            var subcategoriaExiste = await _context.Subcategorias
                .AnyAsync(s => s.Id == subcategoriaId);

            if (!subcategoriaExiste)
            {
                _logger.LogWarning("⚠️ La subcategoría con ID {SubcategoriaId} no existe", subcategoriaId);
                return Enumerable.Empty<ValorFiltroDto>();
            }

            var valoresFiltro = await _context.ValoresFiltro
                .Where(v => v.SubcategoriaId == subcategoriaId)
                .OrderBy(v => v.Valor)
                .Select(v => new ValorFiltroDto
                {
                    Id = v.Id,
                    Valor = HttpUtility.HtmlEncode(v.Valor)
                })
                .ToListAsync();

            _logger.LogInformation("✅ Se obtuvieron {Count} valores de filtro para subcategoría ID: {SubcategoriaId}", 
                valoresFiltro.Count, subcategoriaId);
            return valoresFiltro;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener valores de filtro para subcategoría ID: {SubcategoriaId}", subcategoriaId);
            throw;
        }
    }

    /// <summary>
    /// Obtiene una categoría específica por su identificador, incluyendo subcategorías y valores
    /// </summary>
    public async Task<CategoriaDto?> ObtenerCategoriaPorIdAsync(int id)
    {
        try
        {
            _logger.LogInformation("🔍 Obteniendo categoría ID: {CategoriaId} con jerarquía completa", id);

            var categoria = await _context.Categorias
                .Where(c => c.Id == id && c.EsActivo)
                .Include(c => c.Subcategorias)
                    .ThenInclude(s => s.Valores)
                .FirstOrDefaultAsync();

            if (categoria == null)
            {
                _logger.LogWarning("⚠️ No se encontró categoría activa con ID: {CategoriaId}", id);
                return null;
            }

            var categoriaDto = new CategoriaDto
            {
                Id = categoria.Id,
                Nombre = HttpUtility.HtmlEncode(categoria.Nombre),
                Descripcion = categoria.Descripcion != null ? HttpUtility.HtmlEncode(categoria.Descripcion) : null,
                IconUrl = categoria.IconUrl,
                Subcategorias = categoria.Subcategorias
                    .OrderBy(s => s.Nombre)
                    .Select(s => new SubcategoriaDto
                    {
                        Id = s.Id,
                        Nombre = HttpUtility.HtmlEncode(s.Nombre),
                        Valores = s.Valores
                            .OrderBy(v => v.Valor)
                            .Select(v => new ValorFiltroDto
                            {
                                Id = v.Id,
                                Valor = HttpUtility.HtmlEncode(v.Valor)
                            }).ToList()
                    }).ToList()
            };

            _logger.LogInformation("✅ Categoría ID: {CategoriaId} obtenida exitosamente con {SubcategoriasCount} subcategorías", 
                id, categoriaDto.Subcategorias.Count);
            return categoriaDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener categoría ID: {CategoriaId}", id);
            throw;
        }
    }

    /// <summary>
    /// Crea una nueva categoría de consumibles automotrices
    /// </summary>
    public async Task<int> CrearCategoriaAsync(CrearCategoriaDto categoriaDto)
    {
        try
        {
            _logger.LogInformation("🔍 Creando nueva categoría: {NombreCategoria}", categoriaDto.Nombre);

            // Validar que no exista una categoría con el mismo nombre
            var existeCategoria = await _context.Categorias
                .AnyAsync(c => c.Nombre.ToLower() == categoriaDto.Nombre.ToLower() && c.EsActivo);

            if (existeCategoria)
            {
                _logger.LogWarning("⚠️ Ya existe una categoría activa con el nombre: {NombreCategoria}", categoriaDto.Nombre);
                throw new InvalidOperationException($"Ya existe una categoría con el nombre '{categoriaDto.Nombre}'");
            }

            var categoria = new Categoria
            {
                Nombre = HttpUtility.HtmlEncode(categoriaDto.Nombre),
                Descripcion = categoriaDto.Descripcion != null ? HttpUtility.HtmlEncode(categoriaDto.Descripcion) : null,
                IconUrl = categoriaDto.IconUrl,
                FechaCreacion = DateTime.UtcNow,
                EsActivo = true
            };

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            _logger.LogInformation("✅ Categoría creada exitosamente con ID: {CategoriaId}, Nombre: {NombreCategoria}", 
                categoria.Id, categoriaDto.Nombre);
            return categoria.Id;
        }
        catch (InvalidOperationException)
        {
            // Re-lanzar excepciones de negocio sin loguear como error
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al crear categoría: {NombreCategoria}", categoriaDto.Nombre);
            throw;
        }
    }
}
