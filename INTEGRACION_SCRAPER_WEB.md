# 🔗 INTEGRACIÓN DEL SCRAPER CON LA APLICACIÓN WEB

**Fecha:** 18 de octubre de 2025  
**Estado:** ✅ EN PROGRESO

---

## 📋 RESUMEN DE CAMBIOS REALIZADOS

### 1. ✅ Referencia de Proyecto Agregada

**Archivo:** `AutoGuia.Web/AutoGuia.Web/AutoGuia.Web.csproj`

```xml
<ItemGroup>
    <ProjectReference Include="..\..\AutoGuia.Core\AutoGuia.Core.csproj" />
    <ProjectReference Include="..\..\AutoGuia.Infrastructure\AutoGuia.Infrastructure.csproj" />
    <ProjectReference Include="..\..\AutoGuia.Scraper\AutoGuia.Scraper.csproj" /> <!-- ✅ NUEVO -->
</ItemGroup>

<ItemGroup>
    <PackageReference Include="HtmlAgilityPack" Version="1.12.4" /> <!-- ✅ NUEVO -->
</ItemGroup>
```

---

### 2. ✅ Servicio de Integración Creado

**Archivo:** `AutoGuia.Infrastructure/Services/ScraperIntegrationService.cs` (NUEVO)

**Características:**
- Ejecuta múltiples scrapers en paralelo
- Implementa caché con `IMemoryCache` (duración: 24h)
- Manejo robusto de errores por scraper
- Logging detallado de operaciones
- Método para limpiar caché manualmente

**Interfaz:**
```csharp
public interface IScraperIntegrationService
{
    Task<List<OfertaDto>> ObtenerOfertasEnTiempoRealAsync(
        string numeroDeParte, 
        CancellationToken cancellationToken = default);
        
    Task<bool> LimpiarCacheAsync(string numeroDeParte);
}
```

**Flujo de Ejecución:**
1. Verificar caché → Si existe, devolver
2. Ejecutar scrapers en paralelo (`Task.WhenAll`)
3. Consolidar resultados
4. Guardar en caché (24h)
5. Devolver ofertas

---

### 3. ✅ ComparadorService Extendido

**Archivos:**
- `AutoGuia.Infrastructure/Services/ComparadorService.cs` - Modificado a `partial class`
- `AutoGuia.Infrastructure/Services/ComparadorService.ScraperIntegration.cs` - NUEVO

**Nuevos Métodos:**
```csharp
// Obtiene ofertas en tiempo real con scrapers
Task<IEnumerable<OfertaDto>> ObtenerOfertasEnTiempoRealAsync(
    string numeroDeParte, 
    CancellationToken cancellationToken = default);

// Limpia caché para forzar nueva búsqueda
Task<bool> LimpiarCacheOfertasAsync(string numeroDeParte);
```

**Fallback Implementado:**
- Si `IScraperIntegrationService` no está disponible → busca en BD
- Garantiza que la app funcione incluso sin scrapers

---

### 4. ✅ IComparadorService Actualizado

**Archivo:** `AutoGuia.Infrastructure/Services/IServices.cs`

```csharp
public interface IComparadorService
{
    // ... métodos existentes ...
    
    /// <summary>
    /// Obtiene ofertas en tiempo real ejecutando scrapers
    /// </summary>
    Task<IEnumerable<OfertaDto>> ObtenerOfertasEnTiempoRealAsync(
        string numeroDeParte, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Limpia el caché de ofertas para un producto específico
    /// </summary>
    Task<bool> LimpiarCacheOfertasAsync(string numeroDeParte);
}
```

---

### 5. 🔧 Program.cs - Registro de Servicios

**Archivo:** `AutoGuia.Web/AutoGuia.Web/Program.cs`

**Servicios Agregados:**
```csharp
// HttpClientFactory para scrapers
builder.Services.AddHttpClient();

// Memory Cache para caché de ofertas
builder.Services.AddMemoryCache();

// Auto-registro de scrapers mediante Reflection
var scraperTypes = typeof(IScraper).Assembly.GetTypes()
    .Where(t => t.IsClass && !t.IsAbstract && typeof(IScraper).IsAssignableFrom(t))
    .Where(t => !t.Name.Contains("Playwright")) // Excluir Playwright
    .ToList();

foreach (var scraperType in scraperTypes)
{
    builder.Services.AddScoped(typeof(IScraper), scraperType);
}

// Servicio de integración
builder.Services.AddScoped<IScraperIntegrationService, ScraperIntegrationService>();
```

**Scrapers Registrados:**
1. ✅ MercadoLibreScraperService
2. ✅ AutoplanetScraperService  
3. ✅ MundoRepuestosScraperService
4. ⏸️ AutoplanetPlaywrightScraperService (excluido por rendimiento)

---

## 🎯 PRÓXIMOS PASOS

### Paso 1: Verificar Compilación ✅ EN CURSO

```bash
dotnet build AutoGuia.sln
```

**Estado:** Compilando...

### Paso 2: Actualizar Página de Productos

Agregar botón "Buscar con Scrapers" en `Productos.razor`:

```razor
<button @onclick="BuscarConScrapers" class="btn btn-success ms-2">
    <i class="fas fa-sync-alt me-2"></i>Actualizar Precios
</button>
```

```csharp
private async Task BuscarConScrapers()
{
    actualizandoScrapers = true;
    StateHasChanged();
    
    var ofertas = await ComparadorService.ObtenerOfertasEnTiempoRealAsync(
        terminoBusqueda, 
        CancellationToken.None);
    
    // Mostrar ofertas en UI
    actualizandoScrapers = false;
    StateHasChanged();
}
```

### Paso 3: Agregar Indicador de Carga

```razor
@if (actualizandoScrapers)
{
    <div class="alert alert-info">
        <div class="spinner-border spinner-border-sm me-2"></div>
        🔄 Buscando precios en tiempo real... 
        <small>(Esto puede tomar 10-30 segundos)</small>
    </div>
}
```

### Paso 4: Probar Integración

1. Ejecutar aplicación
2. Navegar a `/productos`
3. Buscar: "filtro aceite"
4. Click "Actualizar Precios"
5. Verificar logs en consola
6. Confirmar que aparecen ofertas de múltiples tiendas

### Paso 5: Optimizaciones

**Caché:**
- ✅ Memory Cache (24h) ya implementado
- 🔜 Redis Cache (para múltiples instancias)

**Performance:**
- ✅ Ejecución paralela de scrapers
- ✅ Timeout configurado (30s por scraper)
- 🔜 Rate limiting para evitar bloqueos

**UX:**
- 🔜 Barra de progreso por scraper
- 🔜 Notificación toast al completar
- 🔜 Botón "Limpiar Caché" manual

---

## 📊 ARQUITECTURA DE INTEGRACIÓN

```
┌─────────────────────────────────────────────────────────────┐
│                     AutoGuia.Web.Client                     │
│                     (Blazor Component)                      │
│                                                             │
│  ┌─────────────────────────────────────────────────────┐  │
│  │           Productos.razor                           │  │
│  │  @inject IComparadorService ComparadorService       │  │
│  └──────────────────────┬──────────────────────────────┘  │
└────────────────────────│────────────────────────────────────┘
                          │
                          │ HTTP (SignalR)
                          ▼
┌─────────────────────────────────────────────────────────────┐
│                   AutoGuia.Web (Server)                     │
│                                                             │
│  ┌──────────────────────────────────────────────────────┐ │
│  │          ComparadorService                           │ │
│  │  implements IComparadorService                       │ │
│  │                                                      │ │
│  │  + ObtenerOfertasEnTiempoRealAsync()                │ │
│  │  + LimpiarCacheOfertasAsync()                       │ │
│  └────────────────────┬─────────────────────────────────┘ │
└─────────────────────────│───────────────────────────────────┘
                          │
                          │ DI
                          ▼
┌─────────────────────────────────────────────────────────────┐
│            AutoGuia.Infrastructure.Services                 │
│                                                             │
│  ┌──────────────────────────────────────────────────────┐ │
│  │     ScraperIntegrationService                        │ │
│  │  implements IScraperIntegrationService               │ │
│  │                                                      │ │
│  │  + ObtenerOfertasEnTiempoRealAsync()                │ │
│  │  + LimpiarCacheAsync()                              │ │
│  │                                                      │ │
│  │  Dependencies:                                       │ │
│  │  - IEnumerable<IScraper> _scrapers                   │ │
│  │  - IMemoryCache _cache                              │ │
│  │  - ILogger _logger                                   │ │
│  └────────────────────┬─────────────────────────────────┘ │
└─────────────────────────│───────────────────────────────────┘
                          │
                          │ Task.WhenAll()
                          ▼
┌─────────────────────────────────────────────────────────────┐
│                     AutoGuia.Scraper                        │
│                                                             │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐    │
│  │ MercadoLibre │  │  Autoplanet  │  │MundoRepuestos│    │
│  │   Scraper    │  │   Scraper    │  │   Scraper    │    │
│  │              │  │              │  │              │    │
│  │  API REST    │  │   HTML       │  │   HTML       │    │
│  │  (JSON)      │  │ (HtmlAgility)│  │ (HtmlAgility)│    │
│  └──────────────┘  └──────────────┘  └──────────────┘    │
└─────────────────────────────────────────────────────────────┘
         │                  │                  │
         │                  │                  │
         ▼                  ▼                  ▼
   ┌─────────┐       ┌──────────┐      ┌──────────┐
   │MercadoLi│       │ Autoplanet│      │  Mundo   │
   │  bre    │       │    .cl    │      │Repuestos │
   │ .cl API │       │  (HTML)   │      │  .cl     │
   └─────────┘       └───────────┘      └──────────┘
```

---

## 🔍 FLUJO DE DATOS

### 1. Usuario Busca Producto

```
Usuario en /productos 
  → Ingresa "filtro aceite"
  → Click "Actualizar Precios"
```

### 2. Blazor Component → Service

```csharp
// Productos.razor
await ComparadorService.ObtenerOfertasEnTiempoRealAsync("filtro aceite");
```

### 3. ComparadorService → ScraperIntegrationService

```csharp
// ComparadorService.cs
if (_scraperService != null)
{
    return await _scraperService.ObtenerOfertasEnTiempoRealAsync(
        numeroDeParte, 
        cancellationToken);
}
```

### 4. ScraperIntegrationService → Caché

```csharp
// ScraperIntegrationService.cs
var cacheKey = $"ofertas_{numeroDeParte.ToLower().Trim()}";

if (_cache.TryGetValue<List<OfertaDto>>(cacheKey, out var ofertasCache))
{
    return ofertasCache; // ⚡ Respuesta inmediata desde caché
}
```

### 5. Ejecución Paralela de Scrapers

```csharp
var tareas = _scrapers.Select(async scraper =>
{
    try
    {
        return await scraper.ObtenerOfertasAsync(numeroDeParte, cancellationToken);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error en scraper {TiendaNombre}", scraper.TiendaNombre);
        return Enumerable.Empty<OfertaDto>();
    }
}).ToList();

var resultados = await Task.WhenAll(tareas); // ⏱️ 10-30 segundos
```

### 6. Consolidación y Caché

```csharp
var todasLasOfertas = resultados.SelectMany(r => r).ToList();

_cache.Set(cacheKey, todasLasOfertas, new MemoryCacheEntryOptions
{
    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24),
    SlidingExpiration = TimeSpan.FromHours(6)
});

return todasLasOfertas; // 🎯 Ofertas consolidadas
```

### 7. Renderizado en UI

```razor
@foreach (var oferta in ofertas)
{
    <div class="card">
        <h5>@oferta.TiendaNombre</h5>
        <p class="price">$@oferta.Precio.ToString("N0")</p>
        <a href="@oferta.UrlProductoEnTienda" target="_blank">
            Ver en tienda
        </a>
    </div>
}
```

---

## ⚙️ CONFIGURACIÓN DE CACHÉ

| Parámetro | Valor | Descripción |
|-----------|-------|-------------|
| **AbsoluteExpiration** | 24 horas | Tiempo máximo en caché |
| **SlidingExpiration** | 6 horas | Se renueva si se accede |
| **Key Pattern** | `ofertas_{numeroDeParte}` | Clave de caché |
| **Storage** | Memory | En memoria del servidor |

**Ejemplo de Clave:**
```
ofertas_filtro_aceite
ofertas_pastillas_freno
ofertas_bujia_ngk
```

---

## 🚀 BENEFICIOS DE LA INTEGRACIÓN

### Para Usuarios:
✅ **Precios en tiempo real** de múltiples tiendas  
✅ **Comparación automática** sin navegar entre sitios  
✅ **Ahorro de tiempo** en búsquedas  
✅ **Mejores decisiones** de compra  

### Para Desarrollo:
✅ **Código modular** y mantenible  
✅ **Caché inteligente** reduce carga  
✅ **Ejecución paralela** optimiza tiempos  
✅ **Logging detallado** para debugging  
✅ **Fallback a BD** garantiza disponibilidad  

### Performance:
- **Primera búsqueda**: 10-30 segundos (3 scrapers en paralelo)
- **Búsquedas siguientes**: <100ms (desde caché)
- **Caché válido**: 24 horas
- **Renovación**: Cada 6 horas si se usa

---

## 🐛 TROUBLESHOOTING

### Error: "ScraperIntegrationService no está disponible"

**Causa:** Servicio no registrado en DI

**Solución:**
```csharp
// Program.cs
builder.Services.AddScoped<IScraperIntegrationService, ScraperIntegrationService>();
```

### Error: "No scrapers registered"

**Causa:** Auto-registro fallido

**Solución:** Verificar namespace y que implementen `IScraper`

### Scrapers muy lentos

**Causa:** Ejecutándose secuencialmente

**Solución:** Ya implementado con `Task.WhenAll()` ✅

### Caché no funciona

**Causa:** `AddMemoryCache()` no registrado

**Solución:**
```csharp
// Program.cs
builder.Services.AddMemoryCache();
```

---

## 📈 MÉTRICAS ESPERADAS

| Métrica | Sin Scrapers | Con Scrapers |
|---------|--------------|--------------|
| Ofertas por producto | 2-5 (BD) | 10-30 (tiempo real) |
| Tiendas consultadas | 1-2 | 3+ |
| Tiempo primera búsqueda | <1s | 10-30s |
| Tiempo búsquedas siguientes | <1s | <1s (caché) |
| Actualización precios | Manual | Automática (24h) |

---

## ✅ CHECKLIST DE VERIFICACIÓN

- [x] ✅ Referencia de AutoGuia.Scraper agregada
- [x] ✅ ScraperIntegrationService creado
- [x] ✅ ComparadorService extendido
- [x] ✅ IComparadorService actualizado
- [ ] ⏳ Program.cs registra servicios correctamente
- [ ] ⏳ Compilación exitosa
- [ ] ⏳ Productos.razor actualizado con botón
- [ ] ⏳ Testing de integración
- [ ] ⏳ Documentación de uso

---

## 🎓 PRÓXIMAS MEJORAS

### Corto Plazo:
1. Agregar UI en Productos.razor
2. Implementar barra de progreso
3. Agregar botón "Limpiar Caché"
4. Toast notifications

### Mediano Plazo:
1. Redis Cache para producción
2. Rate limiting por IP
3. Webhooks para actualizaciones
4. Panel admin de scrapers

### Largo Plazo:
1. Machine Learning para predicción de precios
2. Alertas de precio por email
3. API REST pública
4. App móvil nativa

---

**Estado Actual:** 🔧 EN DESARROLLO  
**Próximo Paso:** Compilar y probar integración  
**Tiempo Estimado Restante:** 2-3 horas
