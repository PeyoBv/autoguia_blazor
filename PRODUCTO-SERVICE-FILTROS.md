# 🔍 ProductoService - Método BuscarPorFiltrosAsync

## 📋 Descripción

Nuevo método implementado en `ProductoService` que permite buscar productos aplicando **filtros dinámicos** basados en categoría y valores de filtro personalizados.

## ✨ Características

- ✅ **Filtrado dinámico** por categoría y múltiples criterios
- ✅ **Validación robusta** de parámetros de entrada
- ✅ **Logging completo** con ILogger
- ✅ **Manejo de errores** con try-catch
- ✅ **Retorno seguro** (nunca null, siempre lista vacía)
- ✅ **Ordenamiento** automático por precio ascendente
- ✅ **Búsqueda flexible** en FiltroValor1, FiltroValor2, FiltroValor3

## 📝 Firma del Método

```csharp
Task<IEnumerable<ProductoDto>> BuscarPorFiltrosAsync(
    string categoria, 
    Dictionary<string, string> filtros
)
```

## 📥 Parámetros

### `categoria` (string, required)
- Nombre de la categoría de productos a buscar
- Ejemplos: `"Aceites"`, `"Neumáticos"`, `"Filtros"`
- Se valida que no sea null o vacío

### `filtros` (Dictionary<string, string>, optional)
- Diccionario de filtros dinámicos
- **Clave**: Nombre del filtro (opcional, solo para logging/documentación)
- **Valor**: Valor a buscar en los campos FiltroValor1, FiltroValor2, FiltroValor3

## 📤 Retorno

- **Tipo**: `IEnumerable<ProductoDto>`
- **Ordenamiento**: Por precio ascendente (PrecioMinimo)
- **Garantía**: Nunca retorna `null`, siempre una lista (puede estar vacía)

## 🎯 Casos de Uso

### Ejemplo 1: Búsqueda de Aceites por Viscosidad
```csharp
var filtros = new Dictionary<string, string>
{
    { "Viscosidad", "10W-40" }
};

var productos = await _productoService.BuscarPorFiltrosAsync("Aceites", filtros);

// Retorna todos los aceites con viscosidad 10W-40, ordenados por precio
```

### Ejemplo 2: Búsqueda Múltiple (Viscosidad + Marca)
```csharp
var filtros = new Dictionary<string, string>
{
    { "Viscosidad", "10W-40" },
    { "Marca", "Castrol" }
};

var productos = await _productoService.BuscarPorFiltrosAsync("Aceites", filtros);

// Retorna aceites 10W-40 de marca Castrol, ordenados por precio
```

### Ejemplo 3: Búsqueda de Neumáticos por Medida
```csharp
var filtros = new Dictionary<string, string>
{
    { "Medida", "195/65R15" }
};

var productos = await _productoService.BuscarPorFiltrosAsync("Neumáticos", filtros);

// Retorna neumáticos con medida 195/65R15
```

### Ejemplo 4: Sin Filtros (Solo Categoría)
```csharp
var productos = await _productoService.BuscarPorFiltrosAsync("Aceites", new Dictionary<string, string>());

// Retorna todos los aceites activos, ordenados por precio
```

## 🔧 Implementación Técnica

### Flujo de Ejecución

1. **Validación de parámetros**
   - Verifica que `categoria` no sea null/vacío
   - Retorna lista vacía si falla

2. **Logging inicial**
   - Registra categoría y cantidad de filtros

3. **Query base**
   ```csharp
   var query = _context.Productos
       .Include(p => p.Categoria)
       .Include(p => p.Ofertas)
       .Where(p => p.EsActivo && p.Categoria.Nombre == categoria);
   ```

4. **Aplicación de filtros dinámicos**
   - Itera sobre cada filtro del diccionario
   - Busca el valor en FiltroValor1, FiltroValor2 o FiltroValor3
   - Usa `Contains()` para búsqueda parcial

5. **Materialización**
   - Ejecuta query con `ToListAsync()`

6. **Proyección a DTO**
   - Mapea a `ProductoDto` con todas las propiedades
   - Calcula PrecioMinimo de las ofertas
   - Cuenta TotalOfertas

7. **Ordenamiento**
   - `OrderBy(p => p.PrecioMinimo)` ascendente

8. **Logging final**
   - Registra cantidad de productos encontrados

9. **Manejo de errores**
   - Try-catch envuelve toda la lógica
   - Logging de excepciones completo
   - Retorno de lista vacía en caso de error

## 📊 Logging

### Niveles de Log

**Information:**
```
Buscando productos en categoría 'Aceites' con 2 filtros
Búsqueda completada. Se encontraron 5 productos
```

**Debug:**
```
Aplicando filtro: Viscosidad = 10W-40
Aplicando filtro: Marca = Castrol
```

**Warning:**
```
BuscarPorFiltrosAsync: La categoría proporcionada es null o vacía
```

**Error:**
```
Error al buscar productos por filtros. Categoría: Aceites, Filtros: Viscosidad=10W-40, Marca=Castrol
```

## 🔍 Lógica de Búsqueda en Filtros

El método busca el valor del filtro en **cualquiera** de los tres campos:

```csharp
query = query.Where(p => 
    (p.FiltroValor1 != null && p.FiltroValor1.Contains(valorFiltro)) ||
    (p.FiltroValor2 != null && p.FiltroValor2.Contains(valorFiltro)) ||
    (p.FiltroValor3 != null && p.FiltroValor3.Contains(valorFiltro))
);
```

**Ejemplos de coincidencias:**

| FiltroValor1 | FiltroValor2 | FiltroValor3 | Búsqueda: "10W-40" | ¿Coincide? |
|--------------|--------------|--------------|-------------------|------------|
| 10W-40       | Castrol      | 4L           | Sí                | ✅         |
| 5W-30        | 10W-40       | Mobil        | Sí                | ✅         |
| SAE          | Premium      | 10W-40 Plus  | Sí (Contains)     | ✅         |
| 5W-30        | Castrol      | 1L           | No                | ❌         |

## 🛡️ Validaciones

### Categoría Null/Vacía
```csharp
if (string.IsNullOrWhiteSpace(categoria))
{
    _logger.LogWarning("BuscarPorFiltrosAsync: La categoría proporcionada es null o vacía");
    return new List<ProductoDto>();
}
```

### Filtros Null/Vacíos
```csharp
if (filtros != null && filtros.Any())
{
    // Aplicar filtros
}
// Si filtros es null o vacío, solo busca por categoría
```

## 📦 Estructura de ProductoDto Retornado

```csharp
public class ProductoDto
{
    public int Id { get; set; }                    // 123
    public string Nombre { get; set; }             // "Aceite Castrol Edge 10W-40"
    public string Descripcion { get; set; }        // "Aceite sintético premium..."
    public string Categoria { get; set; }          // "Aceites"
    public string? Subcategoria { get; set; }      // null (no implementado)
    public string Marca { get; set; }              // "Castrol"
    public string NumeroDeparte { get; set; }      // "CAST-EDGE-10W40"
    public string? ImagenUrl { get; set; }         // "/images/aceite-castrol.jpg"
    public decimal PrecioMinimo { get; set; }      // 19990 (precio más bajo)
    public int TotalOfertas { get; set; }          // 3 (cantidad de tiendas)
    public DateTime FechaCreacion { get; set; }    // 2024-10-20T...
}
```

## 🔗 Registro en DI Container

El servicio está registrado en `Program.cs`:

```csharp
builder.Services.AddScoped<IProductoService, ProductoService>();
```

## 🧪 Escenarios de Prueba

### ✅ Caso Exitoso
- Categoría válida + Filtros válidos
- **Resultado**: Lista de productos que coinciden

### ⚠️ Caso Sin Resultados
- Categoría válida + Filtros que no coinciden
- **Resultado**: Lista vacía (no null)

### ❌ Caso de Error - Categoría Null
- Categoría: `null`
- **Resultado**: Lista vacía + Warning log

### ❌ Caso de Error - Excepción
- Error de base de datos
- **Resultado**: Lista vacía + Error log con stack trace

## 📈 Performance

### Optimizaciones Implementadas

1. **Include Eager Loading**
   ```csharp
   .Include(p => p.Categoria)
   .Include(p => p.Ofertas)
   ```
   - Evita N+1 queries

2. **Materialización Temprana**
   ```csharp
   var productosDb = await query.ToListAsync();
   ```
   - Ejecuta query en DB antes de proyectar

3. **Proyección en Memoria**
   ```csharp
   var productos = productosDb.Select(p => new ProductoDto { ... })
   ```
   - Mapeo eficiente sin consultas adicionales

## 🚀 Uso en Componentes Blazor

```razor
@inject IProductoService ProductoService

@code {
    private List<ProductoDto> productos = new();
    
    protected override async Task OnInitializedAsync()
    {
        var filtros = new Dictionary<string, string>
        {
            { "Viscosidad", "10W-40" },
            { "Marca", "Castrol" }
        };
        
        productos = (await ProductoService.BuscarPorFiltrosAsync("Aceites", filtros)).ToList();
    }
}
```

## 📝 Notas de Implementación

- ✅ Método completamente asíncrono
- ✅ Sigue convenciones del proyecto
- ✅ Compatible con arquitectura existente
- ✅ Documentación XML completa
- ✅ Logging estructurado
- ⚠️ Subcategoría siempre null (no implementada en entidad Producto)

## 🔄 Mejoras Futuras Sugeridas

1. **Paginación** - Agregar parámetros skip/take
2. **Ordenamiento personalizado** - Permitir ordenar por nombre, fecha, etc.
3. **Búsqueda full-text** - Integrar búsqueda de texto completo
4. **Caché** - Implementar caché de resultados frecuentes
5. **Filtros complejos** - Rangos de precio, múltiples marcas, etc.

---

**Creado**: 20 de octubre de 2025  
**Autor**: GitHub Copilot  
**Versión**: 1.0  
**Proyecto**: AutoGuía - Plataforma Automotriz
