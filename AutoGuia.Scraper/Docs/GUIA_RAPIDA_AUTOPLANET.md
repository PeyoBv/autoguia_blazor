# 🚀 Guía Rápida: Scraper de Autoplanet (HTML)

## 📋 Descripción

Scraper robusto para **Autoplanet.cl** que utiliza **HtmlAgilityPack** para extraer ofertas de repuestos automotrices mediante parseo HTML. Implementa múltiples selectores de respaldo y manejo específico del formato de precios chileno.

---

## ⚡ Inicio Rápido

### 1️⃣ Ejecutar Prueba Completa
```powershell
# Probar el scraper con 4 búsquedas predefinidas
dotnet run --project AutoGuia.Scraper -- --test-autoplanet
```

**Salida esperada:**
```
🧪 PRUEBA DE SCRAPER: AUTOPLANET
1️⃣ Creando instancia del scraper...
   ✅ Scraper creado correctamente
   
2️⃣ Verificando configuración...
   📋 Configuración detectada:
      • BaseUrl: https://www.autoplanet.cl
      • MaxResults: 10
      
3️⃣ Ejecutando 4 búsquedas de prueba...
   🔍 Buscando: "filtro aceite"
      ✅ Se encontraron 10 ofertas
         • Filtro de Aceite MANN W 712/75 para Toyota
           Precio: $12.990 CLP
           Stock: ✅ Disponible
```

### 2️⃣ Usar en Código
```csharp
// Inyectar el servicio
var scraper = serviceProvider.GetRequiredService<AutoplanetScraperService>();

// Scrapear productos
var ofertas = await scraper.ScrapearProductosAsync(
    numeroDeParte: "filtro aceite",
    tiendaId: 999,
    cancellationToken: CancellationToken.None
);

// Procesar resultados
foreach (var oferta in ofertas)
{
    if (!oferta.TieneErrores)
    {
        Console.WriteLine($"Producto: {oferta.Descripcion}");
        Console.WriteLine($"Precio: ${oferta.Precio:N0} CLP");
        Console.WriteLine($"URL: {oferta.UrlProducto}");
    }
}
```

---

## 🔧 Configuración

### appsettings.json
```json
{
  "ScrapingSettings": {
    "Autoplanet": {
      "MaxResults": 10,           // Máximo de resultados por búsqueda
      "TimeoutSeconds": 30        // Timeout de HTTP requests
    }
  },
  "Stores": {
    "Autoplanet": {
      "BaseUrl": "https://www.autoplanet.cl",
      "ProductSearchUrl": "/busqueda?q={0}",  // {0} = término de búsqueda
      "Enabled": true,
      "RequestDelayMs": 1500      // Delay entre requests (1.5 segundos)
    }
  }
}
```

### Parámetros Ajustables

| Parámetro | Valor por Defecto | Descripción |
|-----------|------------------|-------------|
| `MaxResults` | `10` | Límite de ofertas a extraer |
| `TimeoutSeconds` | `30` | Tiempo máximo de espera HTTP |
| `RequestDelayMs` | `1500` | Delay entre requests (ms) |
| `Enabled` | `true` | Habilitar/deshabilitar scraper |

**Recomendaciones:**
- ⚠️ No reducir `RequestDelayMs` por debajo de 1000ms (puede causar bloqueos)
- ⚡ Aumentar `TimeoutSeconds` si el sitio es lento
- 🎯 Ajustar `MaxResults` según necesidades (mínimo 5, máximo 50)

---

## 🛠️ Características Técnicas

### 1. Selectores HTML Múltiples

El scraper implementa **7 selectores de respaldo** por cada tipo de dato:

```csharp
// Ejemplo: Selectores para el título del producto
private readonly string[] _selectoresTitulo = new[]
{
    ".//h2[@class='product-title']",          // Selector principal
    ".//h3[@class='product-name']",           // Alternativa 1
    ".//div[@class='product-info']/h4",       // Alternativa 2
    ".//a[@class='product-link']",            // Alternativa 3
    ".//span[@itemprop='name']",              // Schema.org
    ".//div[contains(@class, 'producto')]//h2", // Selector genérico
    ".//div[@class='item-title']"             // Último recurso
};
```

**Ventajas:**
- ✅ Si un selector falla, intenta automáticamente el siguiente
- ✅ Resistente a cambios menores en la estructura HTML
- ✅ Mayor probabilidad de éxito en el scraping

### 2. Parseo de Precios Chilenos

**Formato chileno:**
- `$12.990` = doce mil novecientos noventa pesos
- Los **puntos son separadores de miles** (NO decimales)
- Las comas son separadores decimales (opcional)

```csharp
// Método especializado para formato chileno
private decimal LimpiarYParsearPrecio(string precioTexto)
{
    // Remover símbolo de pesos
    string precioLimpio = precioTexto.Replace("$", "").Trim();
    
    // IMPORTANTE: Puntos = miles (remover)
    precioLimpio = precioLimpio.Replace(".", "");
    
    // Coma = decimal (convertir a punto)
    precioLimpio = precioLimpio.Replace(",", ".");
    
    return decimal.Parse(precioLimpio, CultureInfo.InvariantCulture);
}
```

**Ejemplos de conversión:**
| HTML | Conversión | Resultado |
|------|------------|-----------|
| `$12.990` | `Replace(".", "")` | `12990` |
| `$1.500.000` | `Replace(".", "")` | `1500000` |
| `$19.990,50` | `Replace(".", "")` + `Replace(",", ".")` | `19990.50` |

### 3. Headers HTTP Realistas

```csharp
private void ConfigurarHttpClient(HttpClient client)
{
    // User-Agent de navegador real (Windows 10 + Chrome)
    client.DefaultRequestHeaders.Add("User-Agent", 
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
        "(KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
    
    // Tipos MIME aceptados
    client.DefaultRequestHeaders.Add("Accept", 
        "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
    
    // Idiomas preferidos (Chile)
    client.DefaultRequestHeaders.Add("Accept-Language", "es-CL,es;q=0.9,en;q=0.8");
    
    // Compresión soportada
    client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
}
```

**¿Por qué es importante?**
- 🔒 Evita ser detectado como bot
- 🌐 Se comporta como un navegador legítimo
- 🇨🇱 Headers específicos para región chilena

### 4. Detección de Disponibilidad

```csharp
private bool DeterminarDisponibilidad(string? textoStock)
{
    if (string.IsNullOrWhiteSpace(textoStock))
        return true; // Por defecto: disponible
    
    var textoLower = textoStock.ToLowerInvariant();
    
    // Palabras clave que indican NO disponible
    var palabrasNoDisponible = new[] 
    { 
        "agotado", 
        "sin stock", 
        "no disponible", 
        "fuera de stock",
        "out of stock",
        "temporalmente no disponible"
    };
    
    return !palabrasNoDisponible.Any(p => textoLower.Contains(p));
}
```

**Casos manejados:**
- ✅ "En Stock" → `true`
- ✅ "Disponible" → `true`
- ❌ "Agotado" → `false`
- ❌ "Sin stock" → `false`
- ⚠️ Texto vacío → `true` (por defecto)

---

## 📊 Datos Extraídos

### Estructura de `OfertaDto`

```csharp
public class OfertaDto
{
    public string? ProductoId { get; set; }        // ID del producto (si existe)
    public int TiendaId { get; set; }              // ID de la tienda (Autoplanet)
    public decimal Precio { get; set; }            // Precio en CLP
    public string? UrlProducto { get; set; }       // URL completa del producto
    public bool StockDisponible { get; set; }      // ¿Hay stock?
    public string? Descripcion { get; set; }       // Título/nombre del producto
    public string? ImagenUrl { get; set; }         // URL de la imagen principal
    public int? TiempoEntrega { get; set; }        // Días estimados (null si no especifica)
    public decimal? Calificacion { get; set; }     // Rating (null si no hay)
    public string? NombreTienda { get; set; }      // "Autoplanet"
    public string? CondicionProducto { get; set; } // "Nuevo", "Usado", etc.
    public bool TieneErrores { get; set; }         // ¿Ocurrió algún error?
    public string? MensajeError { get; set; }      // Descripción del error
}
```

### Ejemplo de Resultado

```json
{
  "ProductoId": null,
  "TiendaId": 999,
  "Precio": 12990,
  "UrlProducto": "https://www.autoplanet.cl/productos/filtro-aceite-mann-w712-75",
  "StockDisponible": true,
  "Descripcion": "Filtro de Aceite MANN W 712/75 para Toyota Corolla",
  "ImagenUrl": "https://www.autoplanet.cl/images/productos/filtro-mann.jpg",
  "TiempoEntrega": null,
  "Calificacion": null,
  "NombreTienda": "Autoplanet",
  "CondicionProducto": "Nuevo",
  "TieneErrores": false,
  "MensajeError": null
}
```

---

## 🧪 Testing

### Comando de Prueba
```powershell
dotnet run --project AutoGuia.Scraper -- --test-autoplanet
```

### Búsquedas de Prueba Incluidas
1. **"filtro aceite"** - Producto genérico común
2. **"pastillas freno"** - Componente de seguridad
3. **"amortiguador"** - Pieza de suspensión
4. **"bujia"** - Repuesto específico

### Interpretar Resultados

#### ✅ Prueba Exitosa
```
📊 REPORTE DE PRUEBA - AUTOPLANET SCRAPER (HTML)

Estado General: ✅ EXITOSO
Duración Total: 12.45 segundos

📈 ESTADÍSTICAS:
   • Total de búsquedas: 4
   • Búsquedas exitosas: 4
   • Total de ofertas encontradas: 35
   • Promedio de ofertas por búsqueda: 8.8
   • Total de errores: 0

✅ Tasa de éxito: 100.0%
```

#### ⚠️ Prueba con Advertencias
```
Estado General: ⚠️ PARCIALMENTE EXITOSO
   • Búsquedas exitosas: 2
   • Búsquedas sin resultados: 2
✅ Tasa de éxito: 50.0%
```

**Posibles causas:**
- El sitio no tiene resultados para esas búsquedas
- Los selectores HTML necesitan ajuste
- El sitio está temporalmente caído

#### ❌ Prueba Fallida
```
Estado General: ❌ FALLIDO
⚠️ Error detectado: Connection timeout
```

**Soluciones:**
1. Verificar conexión a Internet
2. Aumentar `TimeoutSeconds` en configuración
3. Revisar si el sitio está accesible

---

## 🔍 Troubleshooting

### Problema: No Se Encuentran Ofertas

#### Síntoma
```
   🔍 Buscando: "filtro aceite"
      ℹ️ No se encontraron ofertas
```

#### Soluciones
1. **Verificar selectores XPath:**
   - Abrir el sitio en navegador
   - Inspeccionar elemento (F12)
   - Copiar XPath del elemento
   - Actualizar `_selectoresNodo` en el código

2. **Ajustar configuración:**
```json
{
  "ScrapingSettings": {
    "Autoplanet": {
      "MaxResults": 20,         // Aumentar límite
      "TimeoutSeconds": 60      // Aumentar timeout
    }
  }
}
```

### Problema: Errores de Parsing de Precios

#### Síntoma
```
⚠️ Error: FormatException - El formato de entrada no es correcto
```

#### Diagnóstico
```csharp
// Agregar logging temporal
Console.WriteLine($"Precio original: {precioTexto}");
var precioLimpio = LimpiarYParsearPrecio(precioTexto);
Console.WriteLine($"Precio parseado: {precioLimpio}");
```

#### Solución
Actualizar método `LimpiarYParsearPrecio()` según formato real:
```csharp
// Si el formato es diferente:
// Ejemplo: "$12,990.00" (formato internacional)
precioLimpio = precioTexto
    .Replace("$", "")
    .Replace(",", "");  // Remover comas de miles
```

### Problema: Bloqueo por Anti-Bot

#### Síntoma
```
❌ Error: HTTP 403 Forbidden
❌ Error: Too Many Requests
```

#### Soluciones
1. **Aumentar delay entre requests:**
```json
{
  "Stores": {
    "Autoplanet": {
      "RequestDelayMs": 3000  // 3 segundos
    }
  }
}
```

2. **Usar proxy rotativo (avanzado):**
```csharp
var handler = new HttpClientHandler
{
    Proxy = new WebProxy("http://proxy-server:8080"),
    UseProxy = true
};
```

3. **Agregar más headers:**
```csharp
client.DefaultRequestHeaders.Add("Referer", "https://www.google.com");
client.DefaultRequestHeaders.Add("DNT", "1");
```

### Problema: URLs Relativas Incorrectas

#### Síntoma
```
UrlProducto: /productos/filtro-aceite  // Falta dominio
```

#### Solución
El método `ConstruirUrlCompleta()` ya maneja esto:
```csharp
var urlCompleta = ConstruirUrlCompleta(urlRelativa, _baseUrl);
// Resultado: https://www.autoplanet.cl/productos/filtro-aceite
```

Si no funciona, verificar `baseUrl` en configuración.

---

## 📈 Optimización de Performance

### 1. Paralelizar Extracción (Avanzado)
```csharp
var tareas = nodosProductos.Select(async nodo => 
    await ExtraerOfertaDeNodo(nodo, tiendaId)
);
var ofertas = await Task.WhenAll(tareas);
```

⚠️ **Precaución:** Puede disparar anti-bot si se hacen muchas requests paralelas.

### 2. Cachear Resultados
```csharp
// Usar MemoryCache para evitar scraping repetido
_cache.Set($"autoplanet_{numeroDeParte}", ofertas, TimeSpan.FromMinutes(15));
```

### 3. Reducir Selectores
Si sabes la estructura exacta del HTML:
```csharp
// Usar solo el selector más común
private readonly string[] _selectoresTitulo = new[]
{
    ".//h2[@class='product-title']"  // Solo 1 selector
};
```

**Trade-off:** Más rápido pero menos robusto.

---

## 📚 Recursos Adicionales

### Documentación
- 📖 [PASO_3_COMPLETADO.md](PASO_3_COMPLETADO.md) - Reporte completo del paso
- 📖 [ARQUITECTURA_MODULAR.md](ARQUITECTURA_MODULAR.md) - Diseño del sistema
- 📖 [HtmlAgilityPack Docs](https://html-agility-pack.net/) - Documentación oficial

### Comandos Útiles
```powershell
# Compilar proyecto
dotnet build AutoGuia.Scraper/AutoGuia.Scraper.csproj

# Ejecutar prueba de Autoplanet
dotnet run --project AutoGuia.Scraper -- --test-autoplanet

# Ejecutar en modo normal (worker continuo)
dotnet run --project AutoGuia.Scraper

# Ver logs detallados
$env:ASPNETCORE_ENVIRONMENT="Development"; dotnet run --project AutoGuia.Scraper
```

### XPath Cheat Sheet
```xpath
// Seleccionar por clase
.//div[@class='product']

// Seleccionar por atributo que contiene
.//div[contains(@class, 'product')]

// Seleccionar hijo directo
./h2

// Seleccionar descendiente
.//h2

// Seleccionar por texto
.//span[text()='En Stock']

// Seleccionar siguiente hermano
.//following-sibling::span
```

---

## ✅ Checklist de Uso

- [ ] HtmlAgilityPack instalado (`dotnet add package HtmlAgilityPack`)
- [ ] Configuración en `appsettings.json` completa
- [ ] Servicio registrado en `Program.cs`
- [ ] Prueba ejecutada exitosamente (`--test-autoplanet`)
- [ ] Selectores XPath ajustados según sitio real
- [ ] `RequestDelayMs` configurado apropiadamente (> 1000ms)
- [ ] Headers HTTP configurados para evitar bloqueos

---

## 🎯 Mejores Prácticas

### ✅ DO (Hacer)
- ✅ Usar múltiples selectores de respaldo
- ✅ Manejar errores gracefully con try-catch
- ✅ Respetar rate limits con delays
- ✅ Validar datos antes de retornarlos
- ✅ Loggear errores para debugging
- ✅ Testear con datos reales del sitio

### ❌ DON'T (No Hacer)
- ❌ Scrapear sin delays (sobrecarga el sitio)
- ❌ Asumir estructura HTML constante
- ❌ Ignorar errores de parsing
- ❌ Usar un solo selector XPath
- ❌ Hardcodear URLs en el código
- ❌ Omitir headers HTTP realistas

---

## 🤝 Contribuir

### Reportar Problemas
Si los selectores dejan de funcionar:
1. Verificar estructura HTML actual del sitio
2. Actualizar selectores en `AutoplanetScraperService.cs`
3. Ejecutar pruebas para validar
4. Documentar cambios

### Agregar Nuevos Selectores
```csharp
// En AutoplanetScraperService.cs
private readonly string[] _selectoresTitulo = new[]
{
    // Agregar nuevo selector al final
    ".//div[@class='nuevo-selector']"
};
```

---

**Última actualización:** ${new Date().toLocaleDateString('es-CL')}  
**Versión:** 1.0.0  
**Mantenedor:** AutoGuía Development Team
