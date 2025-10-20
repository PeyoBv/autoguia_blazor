# 🎯 GUÍA DE ACTIVACIÓN - FASE 1 PROFESIONALIZACIÓN

**Fecha**: 20 de Octubre de 2025  
**Versión**: 1.0  
**Proyecto**: AutoGuía - Profesionalización V2.0

---

## 🚀 ESTADO ACTUAL

✅ **FASE 1 COMPLETADA CON ÉXITO**

- ✅ CI/CD Pipeline implementado
- ✅ APIs de MercadoLibre y eBay integradas
- ✅ Comparador agregado funcional
- ✅ Logging estructurado con Serilog
- ✅ Políticas de resiliencia con Polly
- ✅ Tests unitarios implementados
- ✅ **Compilación exitosa** (0 errores, 25 warnings no críticos)

---

## ⚠️ ADVERTENCIA IMPORTANTE

El archivo **`Program.cs`** tiene código nuevo que requiere **credenciales de eBay** para funcionar completamente.

Tienes **DOS OPCIONES**:

### 🔷 OPCIÓN 1: Activación Parcial (Solo MercadoLibre)

**Recomendado si quieres probar rápidamente**

Comenta las líneas relacionadas con eBay en `Program.cs`:

```csharp
// ✨ Configurar HttpClients con políticas de resiliencia (Polly)
// builder.Services.AddResilientHttpClients(builder.Configuration); // ⬅️ COMENTAR

// ✨ Agregar Memory Cache para optimización
builder.Services.AddMemoryCache(); // ⬅️ DEJAR

// ✨ Registrar servicios de APIs externas (MercadoLibre, eBay)
// builder.Services.AddScoped<IExternalMarketplaceService, MercadoLibreService>(); // ⬅️ COMENTAR
// builder.Services.AddScoped<IExternalMarketplaceService, EbayService>(); // ⬅️ COMENTAR
// builder.Services.AddScoped<ComparadorAgregadoService>(); // ⬅️ COMENTAR
```

**Y también comentar las líneas de Serilog:**

```csharp
// ✨ Configurar Serilog ANTES de crear el builder
// SerilogConfiguration.ConfigureSerilog(new ConfigurationBuilder()...); // ⬅️ COMENTAR

// ✨ Usar Serilog como proveedor de logging
// builder.Host.UseSerilog(); // ⬅️ COMENTAR
```

**Resultado:**
- ✅ Aplicación funcionará normalmente
- ✅ Logging tradicional activo
- ❌ Sin APIs externas de marketplaces
- ❌ Sin logging estructurado Serilog

### 🔷 OPCIÓN 2: Activación Completa (MercadoLibre + eBay)

**Recomendado para funcionalidad completa**

#### Paso 1: Obtener Credenciales de eBay

1. **Ir a**: https://developer.ebay.com/
2. **Crear cuenta** de desarrollador (gratis)
3. **Crear una aplicación**:
   - Nombre: AutoGuía  
   - Tipo: Application (no Production)
   - APIs: Buy → Browse API
4. **Obtener credenciales**:
   - Client ID (App ID)
   - Client Secret (Cert ID)

#### Paso 2: Configurar Credenciales

**Método A: User Secrets (Desarrollo - Recomendado)**

```bash
cd AutoGuia.Web/AutoGuia.Web

# Inicializar user secrets
dotnet user-secrets init

# Agregar credenciales
dotnet user-secrets set "Ebay:ClientId" "YOUR_CLIENT_ID"
dotnet user-secrets set "Ebay:ClientSecret" "YOUR_CLIENT_SECRET"

# Verificar
dotnet user-secrets list
```

**Método B: appsettings.Development.json (No recomendado por seguridad)**

```json
{
  "Ebay": {
    "BaseUrl": "https://api.ebay.com",
    "ClientId": "YOUR_CLIENT_ID",
    "ClientSecret": "YOUR_CLIENT_SECRET"
  }
}
```

⚠️ **IMPORTANTE**: NO commitear credenciales al repositorio

#### Paso 3: Verificar Program.cs

Asegúrate de que estas líneas **NO estén comentadas**:

```csharp
// ✨ Configurar Serilog
SerilogConfiguration.ConfigureSerilog(new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
    .Build());

var builder = WebApplication.CreateBuilder(args);

// ✨ Usar Serilog
builder.Host.UseSerilog();

// ... otros servicios ...

// ✨ APIs Externas
builder.Services.AddResilientHttpClients(builder.Configuration);
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IExternalMarketplaceService, MercadoLibreService>();
builder.Services.AddScoped<IExternalMarketplaceService, EbayService>();
builder.Services.AddScoped<ComparadorAgregadoService>();
```

#### Paso 4: Compilar y Ejecutar

```bash
dotnet build AutoGuia.sln
dotnet run --project AutoGuia.Web/AutoGuia.Web/AutoGuia.Web.csproj
```

**Resultado:**
- ✅ MercadoLibre API funcional (sin credenciales)
- ✅ eBay API funcional (con credenciales)
- ✅ Comparador agregado activo
- ✅ Logging estructurado Serilog
- ✅ Políticas de resiliencia Polly
- ✅ Memory Cache optimizado

---

## 🧪 PROBAR LAS APIS

### 1. Verificar Disponibilidad

Crear un endpoint de test o ejecutar en consola:

```csharp
@inject ComparadorAgregadoService Comparador

var disponibilidad = await Comparador.VerificarDisponibilidadMarketplacesAsync();

foreach (var (marketplace, disponible) in disponibilidad)
{
    Console.WriteLine($"{marketplace}: {(disponible ? "✅ Disponible" : "❌ No disponible")}");
}
```

**Resultado esperado:**
```
MercadoLibre: ✅ Disponible
eBay: ✅ Disponible (si configuraste credenciales)
```

### 2. Búsqueda en MercadoLibre

```csharp
@inject MercadoLibreService MercadoLibre

var ofertas = await MercadoLibre.BuscarProductosAsync("filtro aceite", null, 10);

<h3>Encontrados {ofertas.Count()} productos en MercadoLibre</h3>

@foreach (var oferta in ofertas)
{
    <div class="card mb-2">
        <div class="card-body">
            <h5>@oferta.Titulo</h5>
            <p><strong>${oferta.Precio} @oferta.Moneda</strong></p>
            <p>@(oferta.EnvioGratis ? "🚚 Envío gratis" : "")</p>
            <a href="@oferta.UrlProducto" target="_blank" class="btn btn-primary">Ver en MercadoLibre</a>
        </div>
    </div>
}
```

### 3. Comparador Agregado

```csharp
@inject ComparadorAgregadoService Comparador

var resultado = await Comparador.BuscarEnTodosLosMarketplacesAsync("pastillas freno", null, 30);

<div class="alert alert-info">
    <h4>📊 Resultados de Búsqueda Agregada</h4>
    <p><strong>Total:</strong> {resultado.TotalResultados} productos</p>
    <p><strong>Precio mínimo:</strong> ${resultado.PrecioMinimo}</p>
    <p><strong>Precio máximo:</strong> ${resultado.PrecioMaximo}</p>
    <p><strong>Precio promedio:</strong> ${resultado.PrecioPromedio:F2}</p>
    <p><strong>Tiempo:</strong> {resultado.TiempoTotalMs}ms</p>
</div>

<h5>Marketplaces consultados:</h5>
@foreach (var mp in resultado.MarketplacesConsultados)
{
    <div class="badge bg-@(mp.ExitosoBusqueda ? "success" : "danger") me-2">
        {mp.NombreMarketplace}: {mp.CantidadResultados} resultados ({mp.TiempoRespuestaMs}ms)
    </div>
}

<hr/>

<div class="row">
    @foreach (var oferta in resultado.Ofertas.Take(20))
    {
        <div class="col-md-6 col-lg-4 mb-3">
            <div class="card">
                @if (!string.IsNullOrEmpty(oferta.ImagenUrl))
                {
                    <img src="@oferta.ImagenUrl" class="card-img-top" alt="@oferta.Titulo">
                }
                <div class="card-body">
                    <h6 class="card-title">@oferta.Titulo</h6>
                    <p class="card-text">
                        <strong class="text-success">${oferta.Precio} @oferta.Moneda</strong><br/>
                        <small>@oferta.NombreTienda · @oferta.Marketplace</small><br/>
                        @if (oferta.EnvioGratis)
                        {
                            <span class="badge bg-success">🚚 Envío gratis</span>
                        }
                    </p>
                    <a href="@oferta.UrlProducto" target="_blank" class="btn btn-sm btn-primary">Ver producto</a>
                </div>
            </div>
        </div>
    }
</div>
```

---

## 📋 VERIFICACIÓN DE LOGS

### Logs en Consola (Serilog)

Si activaste Serilog, verás logs estructurados:

```
[10:30:15 INF] 🚀 Serilog configurado correctamente
[10:30:16 INF] 🚀 Iniciando AutoGuía aplicación web
[10:30:17 INF] Buscando en MercadoLibre: https://api.mercadolibre.com/sites/MLC/search?...
[10:30:18 INF] Encontrados 20 productos en MercadoLibre
[10:30:19 INF] ✅ 50 resultados de 2 marketplaces en 1850ms
```

### Logs en Archivos

Archivos generados automáticamente:

```
AutoGuía/
└── logs/
    ├── autoguia-20251020.log          # Todos los logs
    └── errors/
        └── autoguia-errors-20251020.log # Solo errores
```

**Abrir logs:**

```bash
# Windows
notepad logs\autoguia-20251020.log

# Linux/macOS
tail -f logs/autoguia-20251020.log
```

---

## 🚨 TROUBLESHOOTING

### Error: "Cannot resolve service IExternalMarketplaceService"

**Causa**: Servicios no registrados en Program.cs

**Solución**: Verifica que estas líneas estén presentes y NO comentadas:

```csharp
builder.Services.AddScoped<IExternalMarketplaceService, MercadoLibreService>();
builder.Services.AddScoped<IExternalMarketplaceService, EbayService>();
builder.Services.AddScoped<ComparadorAgregadoService>();
```

### Error: "Ebay credentials not found"

**Causa**: eBay ClientId/ClientSecret no configurados

**Solución (Opción 1)**: Comentar línea de eBay:

```csharp
// builder.Services.AddScoped<IExternalMarketplaceService, EbayService>();
```

**Solución (Opción 2)**: Configurar credenciales con user-secrets (ver arriba)

### Error: "Serilog not configured"

**Causa**: Configuración de Serilog incompleta

**Solución**: Comentar líneas de Serilog:

```csharp
// SerilogConfiguration.ConfigureSerilog(...);
// builder.Host.UseSerilog();
```

Y también comentar al final:

```csharp
// try
// {
//     Log.Information("...");
//     app.Run();
// }
// catch (Exception ex)
// {
//     Log.Fatal(ex, "...");
// }
// finally
// {
//     Log.CloseAndFlush();
// }

// Reemplazar con:
app.Run();
```

### Warnings de compilación (No críticos)

Los 25 warnings son de código existente (nullable references, async sin await).

**No afectan funcionalidad** y se resolverán en Fase 2.

---

## 📊 MÉTRICAS DE ÉXITO

### ✅ Compilación

```bash
dotnet build AutoGuia.sln -c Release
```

**Resultado esperado:**
```
Compilación correcta.
    25 Advertencia(s)
    0 Errores
```

### ✅ Tests

```bash
dotnet test AutoGuia.Tests/AutoGuia.Tests.csproj
```

**Resultado esperado:**
```
Total tests: 7
Passed: 7
Failed: 0
```

### ✅ Aplicación Ejecutándose

```bash
dotnet run --project AutoGuia.Web/AutoGuia.Web/AutoGuia.Web.csproj
```

**Acceso:**
- https://localhost:7001
- http://localhost:5070

**Sin errores en consola** ✅

---

## 🎯 PRÓXIMOS PASOS

### Inmediatos (Hoy)

1. ✅ Elegir OPCIÓN 1 o OPCIÓN 2
2. ✅ Configurar credenciales si eliges OPCIÓN 2
3. ✅ Compilar y ejecutar
4. ✅ Probar búsqueda en MercadoLibre
5. ✅ Verificar logs

### Corto Plazo (Esta Semana)

1. **Crear página de comparador** con UI completa
2. **Implementar filtros** por categoría
3. **Agregar favoritos** de productos
4. **Expandir tests** a eBay y Comparador

### Medio Plazo (Próximas 2 Semanas)

1. **FASE 2**: Validación con FluentValidation
2. **DTOs completos** con AutoMapper
3. **Paginación** de resultados
4. **Rate limiting** para APIs

---

## 📚 RECURSOS

### Documentación

- **FASE-1-PROFESIONALIZACION-COMPLETA.md** - Documentación completa (500+ líneas)
- **RESUMEN-EJECUTIVO-FASE-1.md** - Resumen ejecutivo
- **Este archivo** - Guía de activación

### APIs

- **MercadoLibre**: https://developers.mercadolibre.com/
- **eBay**: https://developer.ebay.com/

### Frameworks

- **Polly**: https://github.com/App-vNext/Polly
- **Serilog**: https://serilog.net/

---

## ✅ CHECKLIST FINAL

Antes de continuar, verifica:

- [ ] Elegiste OPCIÓN 1 o OPCIÓN 2
- [ ] Configuraste credenciales (si OPCIÓN 2)
- [ ] Compilación exitosa (0 errores)
- [ ] Aplicación ejecutándose
- [ ] MercadoLibre funcional
- [ ] Logs visibles (si activaste Serilog)
- [ ] Tests pasando (7/7)

---

## 🎉 ¡FELICIDADES!

Si todos los checks están ✅, has completado exitosamente la **FASE 1** de profesionalización de AutoGuía.

**Tu aplicación ahora tiene:**
- ✅ CI/CD con GitHub Actions
- ✅ APIs de marketplaces (MercadoLibre + eBay)
- ✅ Comparador agregado inteligente
- ✅ Logging estructurado
- ✅ Políticas de resiliencia
- ✅ Tests unitarios

**Próximo hito**: FASE 2 - Validación y Componentes Blazor 🚀

---

**Última actualización**: 20 de Octubre de 2025  
**Versión**: 1.0  
**Estado**: 🟢 LISTO PARA ACTIVACIÓN
