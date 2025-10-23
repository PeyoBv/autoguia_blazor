using System.Text.RegularExpressions;
using AutoGuia.Core.DTOs;
using AutoGuia.Infrastructure.Repositories;

namespace AutoGuia.Infrastructure.Services;

/// <summary>
/// Servicio de búsqueda avanzada con fuzzy matching para síntomas
/// </summary>
public class SintomaSearchService
{
    private readonly ISintomaRepository _sintomaRepository;

    public SintomaSearchService(ISintomaRepository sintomaRepository)
    {
        _sintomaRepository = sintomaRepository;
    }

    /// <summary>
    /// Busca síntomas usando fuzzy matching con palabras clave
    /// </summary>
    public virtual async Task<List<SintomaDto>> BuscarSintomasAvanzadoAsync(string descripcion)
    {
        if (string.IsNullOrWhiteSpace(descripcion))
            return new();

        // Obtener todos los síntomas activos
        var todosLosSintomas = await _sintomaRepository.ObtenerTodosSintomasActivosAsync();

        // Normalizar entrada del usuario
        var palabrasClaveUsuario = NormalizarYExtraerPalabrasClaves(descripcion);

        // Calcular puntuación de similitud para cada síntoma
        var resultadosConPuntuacion = todosLosSintomas
            .Select(sintoma => new
            {
                Sintoma = sintoma,
                Puntuacion = CalcularPuntuacionSimilitud(
                    sintoma.Descripcion,
                    sintoma.DescripcionTecnica,
                    palabrasClaveUsuario)
            })
            .Where(x => x.Puntuacion > 0) // Solo síntomas con coincidencia
            .OrderByDescending(x => x.Puntuacion) // Ordenar por relevancia
            .Select(x => x.Sintoma)
            .ToList();

        return resultadosConPuntuacion;
    }

    /// <summary>
    /// Normaliza texto: minúsculas, quita tildes, extrae palabras clave
    /// </summary>
    private List<string> NormalizarYExtraerPalabrasClaves(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return new();

        // Minúsculas + quitar tildes
        var normalizado = RemoverTildes(texto.ToLowerInvariant());

        // Palabras clave (excluir preposiciones y artículos comunes)
        var stopWords = new[] { "el", "la", "un", "una", "los", "las", "unos", "unas", 
                               "de", "del", "a", "al", "en", "por", "para", "con", "sin",
                               "que", "y", "o", "es", "está", "están", "se", "mi", "tu", "su" };

        var palabras = normalizado
            .Split(new[] { ' ', ',', '.', ';', ':', '?', '!' }, StringSplitOptions.RemoveEmptyEntries)
            .Where(p => p.Length > 2 && !stopWords.Contains(p))
            .Distinct()
            .ToList();

        return palabras;
    }

    /// <summary>
    /// Quita tildes de texto (á → a, é → e, etc)
    /// </summary>
    private string RemoverTildes(string texto)
    {
        var conTildes = "áàäâãéèëêíìïîóòöôõúùüûñç";
        var sinTildes = "aaaaaeeeeiiiiooooouuuunc";

        foreach (var i in Enumerable.Range(0, conTildes.Length))
        {
            texto = texto.Replace(conTildes[i], sinTildes[i]);
        }

        return texto;
    }

    /// <summary>
    /// Calcula puntuación de similitud entre entrada del usuario y síntoma
    /// Basado en: coincidencias de palabras, Levenshtein distance, longitud
    /// </summary>
    private double CalcularPuntuacionSimilitud(
        string descripcionSintoma,
        string descripcionTecnica,
        List<string> palabrasClaveUsuario)
    {
        var descripcionNormalizada = RemoverTildes(descripcionSintoma.ToLowerInvariant());
        var tecnicaNormalizada = RemoverTildes(descripcionTecnica.ToLowerInvariant());
        var textoCompleto = $"{descripcionNormalizada} {tecnicaNormalizada}";

        double puntuacion = 0;

        // 1. Puntuación por palabras clave coincidentes (peso: 40%)
        var coincidenciasDirectas = palabrasClaveUsuario.Count(palabra => textoCompleto.Contains(palabra));
        var porcentajeCoincidencias = palabrasClaveUsuario.Count > 0 
            ? (double)coincidenciasDirectas / palabrasClaveUsuario.Count 
            : 0;
        puntuacion += porcentajeCoincidencias * 40;

        // 2. Puntuación por similitud Levenshtein (peso: 35%)
        var distanciaLevenshtein = CalcularLevenshteinDistance(
            string.Join(" ", palabrasClaveUsuario),
            descripcionNormalizada);
        var maxDistancia = Math.Max(
            string.Join(" ", palabrasClaveUsuario).Length,
            descripcionNormalizada.Length);
        var similitudLevenshtein = maxDistancia > 0 
            ? 1 - ((double)distanciaLevenshtein / maxDistancia)
            : 0;
        puntuacion += similitudLevenshtein * 35;

        // 3. Bonificación si coincide en descripción principal (peso: 25%)
        if (descripcionNormalizada.Contains(string.Join(" ", palabrasClaveUsuario)))
            puntuacion += 25;

        return puntuacion;
    }

    /// <summary>
    /// Algoritmo Levenshtein: distancia mínima de ediciones entre dos strings
    /// </summary>
    private int CalcularLevenshteinDistance(string texto1, string texto2)
    {
        var longitud1 = texto1.Length;
        var longitud2 = texto2.Length;
        var matriz = new int[longitud1 + 1, longitud2 + 1];

        for (int i = 0; i <= longitud1; i++)
            matriz[i, 0] = i;

        for (int j = 0; j <= longitud2; j++)
            matriz[0, j] = j;

        for (int i = 1; i <= longitud1; i++)
        {
            for (int j = 1; j <= longitud2; j++)
            {
                int costo = (texto1[i - 1] == texto2[j - 1]) ? 0 : 1;
                matriz[i, j] = Math.Min(
                    Math.Min(
                        matriz[i - 1, j] + 1,      // Eliminar
                        matriz[i, j - 1] + 1),    // Insertar
                    matriz[i - 1, j - 1] + costo); // Reemplazar
            }
        }

        return matriz[longitud1, longitud2];
    }
}
