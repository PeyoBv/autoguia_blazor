using Mscc.GenerativeAI;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AutoGuia.Infrastructure.Services
{
    /// <summary>
    /// Servicio de diagnóstico automotriz usando IA de Gemini
    /// </summary>
    public class GeminiService : IGeminiService
    {
        private readonly string _apiKey;
        private readonly string _model;
        private readonly ILogger<GeminiService> _logger;

        private const string PROMPT_BASE = @"Eres un mecánico automotriz experto certificado. Tu rol es ayudar a identificar problemas en vehículos.

INSTRUCCIONES:
1. Analiza la descripción de falla proporcionada por el usuario.
2. Entrega EXACTAMENTE en este orden:
   - Posibles causas ordenadas por probabilidad (de mayor a menor)
   - Pasos básicos para revisar cada causa
   - Nivel de urgencia (Bajo, Medio, Alto, Crítico)

FORMATO DE RESPUESTA OBLIGATORIO:
## 🔍 Posibles Causas
1. [Causa 1] - Probabilidad: XX%
2. [Causa 2] - Probabilidad: XX%
3. [Causa 3] - Probabilidad: XX%

## 🔧 Pasos para Revisar
### Causa 1: [Nombre]
- Paso 1
- Paso 2
- Paso 3

### Causa 2: [Nombre]
- Paso 1
- Paso 2

### Causa 3: [Nombre]
- Paso 1
- Paso 2

## 🚨 Nivel de Urgencia
**URGENCIA: [BAJO/MEDIO/ALTO/CRÍTICO]**

Explicación breve de por qué este nivel.

---

DESCRIPCIÓN DE LA FALLA DEL USUARIO:
{0}

Responde de manera profesional pero comprensible para usuarios sin conocimiento técnico avanzado.";

        public GeminiService(IConfiguration configuration, ILogger<GeminiService> logger)
        {
            _apiKey = configuration["GeminiApi:ApiKey"]
                ?? throw new InvalidOperationException("GeminiApi:ApiKey no configurada");
            // Para el paquete Mscc.GenerativeAI, usar "gemini-1.5-pro-latest" o simplemente omitir el modelo
            _model = configuration["GeminiApi:Model"] ?? "gemini-1.5-pro-latest";
            _logger = logger;
        }

        public async Task<string> ObtenerDiagnosticoAsync(string descripcionFalla)
        {
            try
            {
                // Validación de entrada
                if (string.IsNullOrWhiteSpace(descripcionFalla))
                {
                    _logger.LogWarning("Descripción de falla vacía");
                    throw new ArgumentException("La descripción de la falla no puede estar vacía");
                }

                if (descripcionFalla.Length < 10)
                {
                    _logger.LogWarning("Descripción muy corta");
                    return "⚠️ Por favor, proporciona una descripción más detallada de la falla (mínimo 10 caracteres).";
                }

                if (descripcionFalla.Length > 2000)
                {
                    _logger.LogWarning("Descripción muy larga");
                    return "⚠️ La descripción es demasiado larga. Usa máximo 2000 caracteres.";
                }

                _logger.LogInformation($"Enviando consulta a Gemini API (caracteres: {descripcionFalla.Length})");

                // Crear cliente y obtener modelo
                var googleAI = new GoogleAI(apiKey: _apiKey);
                // No especificar modelo, dejar que use el default
                var model = googleAI.GenerativeModel();

                // Construir prompt completo
                string promptCompleto = string.Format(PROMPT_BASE, SanitizarEntrada(descripcionFalla));

                // Llamar a API con timeout
                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30)))
                {
                    var response = await model.GenerateContent(promptCompleto);

                    if (response == null || string.IsNullOrEmpty(response.Text))
                    {
                        _logger.LogWarning("Respuesta vacía de Gemini API");
                        return "⚠️ No se pudo obtener una respuesta. Intenta nuevamente.";
                    }

                    string resultado = response.Text.Trim();
                    _logger.LogInformation($"Respuesta recibida: {resultado.Length} caracteres");

                    return resultado;
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogError("Timeout en solicitud a Gemini API");
                return "❌ La solicitud tardó demasiado. Intenta nuevamente.";
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"Error HTTP: {ex.Message}");
                return "❌ Error de conexión con Gemini API. Verifica tu conexión a internet.";
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError($"Error de autenticación: {ex.Message}");
                return "❌ Error de autenticación. Verifica que tu API Key sea válida.";
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Error de validación: {ex.Message}");
                return $"⚠️ {ex.Message}";
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error inesperado: {ex.GetType().Name} - {ex.Message}");
                return $"❌ Error inesperado: {ex.Message}";
            }
        }

        /// <summary>
        /// Método auxiliar para sanitizar entrada del usuario
        /// </summary>
        private string SanitizarEntrada(string entrada)
        {
            return System.Text.RegularExpressions.Regex.Replace(entrada, @"\s+", " ").Trim();
        }
    }
}
