# 🏗️ Arquitectura Modular del Scraper - Patrón Strategy

## 📋 Resumen de Implementación

Se ha implementado exitosamente una **arquitectura modular y escalable** para el sistema de scraping usando el **Patrón Strategy**. Esta implementación permite agregar, eliminar o modificar scrapers sin afectar el resto de la aplicación.

---

## 🎯 Componentes Implementados

### 1. **IScraper** - Interfaz Base
**Ubicación:** `AutoGuia.Scraper/Interfaces/IScraper.cs`

```csharp
public interface IScraper
{
    string TiendaNombre { get; }
    string TipoScraper { get; }
    bool EstaHabilitado { get; }
    
    Task<IEnumerable<OfertaDto>> ScrapearProductosAsync(
        string numeroDeParte, 
        int tiendaId,
        CancellationToken cancellationToken = default);
    
    bool PuedeScrapearTienda(string tiendaNombre);
    Task<Dictionary<string, string>> ObtenerConfiguracionAsync();
}
```

**Responsabilidades:**
- Define el contrato que deben cumplir todos los scrapers
- Proporciona métodos para identificación y configuración
- Permite validación de compatibilidad con tiendas

---

### 2. **OfertaDto** - Objeto de Transferencia de Datos
**Ubicación:** `AutoGuia.Scraper/DTOs/OfertaDto.cs`

**Propiedades principales:**
- `ProductoId`, `TiendaId`: Identificadores de relación
- `Precio`, `StockDisponible`: Información de disponibilidad
- `UrlProducto`, `ImagenUrl`: Referencias web
- `TiempoEntrega`, `Calificacion`: Metadatos adicionales
- `TieneErrores`, `MensajeError`: Manejo de errores

**Características:**
- ✅ Documentación XML completa
- ✅ Propiedades opcionales con nullable reference types
- ✅ Timestamp de scraping automático
- ✅ Manejo de errores integrado

---

### 3. **ScraperOrchestratorService** - Orquestador
**Ubicación:** `AutoGuia.Scraper/Services/ScraperOrchestratorService.cs`

**Responsabilidades:**
1. **Coordinación de Scrapers**: Selecciona el scraper apropiado para cada tienda
2. **Gestión de Flujo**: Ejecuta scraping secuencial por producto
3. **Manejo de Errores**: Captura y registra errores sin detener el proceso
4. **Actualización de BD**: Usa `OfertaUpdateService` para persistir datos
5. **Logging Detallado**: Emojis y mensajes descriptivos para trazabilidad

**Métodos principales:**
```csharp
// Scrapear un solo producto en todas las tiendas
Task EjecutarScrapingAsync(int productoId, CancellationToken cancellationToken)

// Scrapear múltiples productos en paralelo
Task EjecutarScrapingMasivoAsync(IEnumerable<int> productosIds, int maxParallelism = 3)

// Obtener estadísticas de scrapers disponibles
Task<Dictionary<string, object>> ObtenerEstadisticasScrapersAsync()
```

**Flujo de Ejecución:**
```
1. Buscar producto en BD (obtener NumeroDeParte)
   ↓
2. Obtener todas las tiendas activas
   ↓
3. Para cada tienda:
   a. Buscar scraper compatible (IScraper)
   b. Ejecutar ScrapearProductosAsync()
   c. Acumular ofertas
   ↓
4. Actualizar BD con todas las ofertas (OfertaUpdateService)
```

---

### 4. **Registro Automático de Scrapers**
**Ubicación:** `AutoGuia.Scraper/Program.cs` → `RegistrarScrapersAutomaticamente()`

**Características:**
- 🔍 **Reflexión (Reflection)**: Busca automáticamente todas las clases que implementan `IScraper`
- ✅ **Filtrado Inteligente**: Solo registra clases concretas (no abstractas ni interfaces)
- 📝 **Logging Detallado**: Muestra cada scraper registrado en consola
- 🔄 **Sin Modificaciones**: Agregar un nuevo scraper no requiere cambiar `Program.cs`

**Código implementado:**
```csharp
private static void RegistrarScrapersAutomaticamente(IServiceCollection services)
{
    var assembly = Assembly.GetExecutingAssembly();
    
    var scraperTypes = assembly.GetTypes()
        .Where(type => 
            type.IsClass &&
            !type.IsAbstract &&
            typeof(IScraper).IsAssignableFrom(type))
        .ToList();

    foreach (var scraperType in scraperTypes)
    {
        services.AddTransient(typeof(IScraper), scraperType);
        Console.WriteLine($"   ✅ Registrado: {scraperType.Name}");
    }
}
```

---

## 🎨 Ventajas de la Arquitectura

### ✅ Modularidad
- Cada scraper es **independiente** y puede ser desarrollado/probado por separado
- Agregar un nuevo scraper = crear una clase que implemente `IScraper`
- Eliminar un scraper = borrar su archivo (registro automático lo detectará)

### ✅ Mantenibilidad
- Cambios en un scraper **no afectan** a otros
- El orquestador maneja errores sin detener el proceso completo
- Logging detallado facilita debugging

### ✅ Escalabilidad
- `EjecutarScrapingMasivoAsync` permite procesar múltiples productos en paralelo
- Control de paralelismo con `SemaphoreSlim`
- Fácil agregar nuevas tiendas sin modificar código existente

### ✅ Testabilidad
- Cada scraper puede ser mockeado fácilmente (interfaz `IScraper`)
- El orquestador puede ser probado con scrapers de prueba
- `ObtenerEstadisticasScrapersAsync()` ayuda a validar el estado del sistema

---

## 🚀 Próximos Pasos

### Paso 2: Implementar Scrapers Específicos
Ahora que la arquitectura está lista, podemos crear:

1. **MercadoLibreApiScraper** (API)
   - Implementa `IScraper`
   - Usa `HttpClient` para llamadas a la API de MercadoLibre
   - `TipoScraper = "API"`

2. **AutoplanetHtmlScraper** (HTML)
   - Implementa `IScraper`
   - Usa `HtmlAgilityPack` para parsing HTML
   - `TipoScraper = "HTML"`

3. **MundoRepuestosHtmlScraper** (HTML)
   - Similar a Autoplanet
   - Parseo específico para la estructura de MundoRepuestos

### Ejemplo de Implementación
```csharp
public class MercadoLibreApiScraper : IScraper
{
    public string TiendaNombre => "MercadoLibre";
    public string TipoScraper => "API";
    public bool EstaHabilitado => true;

    private readonly IHttpClientFactory _httpClientFactory;
    
    public MercadoLibreApiScraper(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IEnumerable<OfertaDto>> ScrapearProductosAsync(
        string numeroDeParte, 
        int tiendaId,
        CancellationToken cancellationToken)
    {
        // Implementar llamada a API de MercadoLibre
        var client = _httpClientFactory.CreateClient("ScraperClient");
        // ... lógica de API
        
        return ofertas;
    }

    public bool PuedeScrapearTienda(string tiendaNombre) 
        => tiendaNombre.Equals("MercadoLibre", StringComparison.OrdinalIgnoreCase);

    public async Task<Dictionary<string, string>> ObtenerConfiguracionAsync()
    {
        return new Dictionary<string, string>
        {
            ["ApiUrl"] = "https://api.mercadolibre.com",
            ["Timeout"] = "30",
            ["RetryCount"] = "3"
        };
    }
}
```

---

## 📊 Diagrama de Flujo

```
┌─────────────────────────────────────────────────────────┐
│          ScraperOrchestratorService                     │
│  (Coordina todos los scrapers)                          │
└────────────────┬────────────────────────────────────────┘
                 │
                 │ IEnumerable<IScraper>
                 │
        ┌────────┴────────┬────────────────┬───────────────┐
        ▼                 ▼                ▼               ▼
┌───────────────┐  ┌──────────────┐  ┌─────────────┐  ┌─────────────┐
│MercadoLibre   │  │Autoplanet    │  │MundoRepuestos│ │Scraper N    │
│ApiScraper     │  │HtmlScraper   │  │HtmlScraper   │ │(futuro)     │
└───────────────┘  └──────────────┘  └─────────────┘  └─────────────┘
     │ API              │ HTML           │ HTML             │
     ▼                  ▼                ▼                  ▼
┌─────────────────────────────────────────────────────────┐
│              OfertaUpdateService                        │
│        (Persiste en la base de datos)                   │
└─────────────────────────────────────────────────────────┘
```

---

## 🎯 Comandos Útiles

### Compilar el proyecto
```powershell
dotnet build AutoGuia.Scraper/AutoGuia.Scraper.csproj
```

### Ejecutar el scraper
```powershell
dotnet run --project AutoGuia.Scraper/AutoGuia.Scraper.csproj
```

### Ejecutar en modo test
```powershell
dotnet run --project AutoGuia.Scraper/AutoGuia.Scraper.csproj -- --test
```

---

## ✅ Checklist de Implementación

- [x] Crear interfaz `IScraper`
- [x] Crear `OfertaDto` con todas las propiedades necesarias
- [x] Implementar `ScraperOrchestratorService`
- [x] Registrar scrapers automáticamente en `Program.cs`
- [x] Documentar arquitectura completa
- [ ] Implementar `MercadoLibreApiScraper` (Paso 2)
- [ ] Implementar `AutoplanetHtmlScraper` (Paso 2)
- [ ] Implementar `MundoRepuestosHtmlScraper` (Paso 2)
- [ ] Crear tests unitarios para cada scraper
- [ ] Configurar rate limiting y retry policies

---

## 📚 Referencias Técnicas

- **Patrón Strategy**: [Microsoft Docs](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/)
- **Inyección de Dependencias**: [ASP.NET Core DI](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection)
- **HttpClientFactory**: [Best Practices](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests)
- **HtmlAgilityPack**: [GitHub Repository](https://github.com/zzzprojects/html-agility-pack)
- **Playwright**: [.NET Documentation](https://playwright.dev/dotnet/)

---

**Generado por:** GitHub Copilot  
**Fecha:** 18 de octubre de 2025  
**Proyecto:** AutoGuía - Sistema de Scraping Modular
