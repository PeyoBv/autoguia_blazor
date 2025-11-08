# 🎯 Rodavia - Guía Completa de Refactorización
## Transición de "Comparador de Precios" a "Centro de Información Vehicular"

---

## ✅ **YA COMPLETADO:**

### 1. Proyecto Rodavia.Scraper
- ✅ Eliminado de `Rodavia.sln`
- ✅ Eliminada referencia en `Rodavia.Web.csproj`
- ⏳ Pendiente: Eliminar carpeta física `Rodavia.Scraper/` (hacerlo manualmente o con `Remove-Item`)

---

## 📋 **FASE 1: LIMPIEZA MANUAL (Pasos Restantes)**

### **Paso 1-A: Eliminar Entidades Obsoletas (Rodavia.Core/Entities)**

Eliminar estos archivos:
```
❌ Rodavia.Core/Entities/Producto.cs
❌ Rodavia.Core/Entities/Tienda.cs
❌ Rodavia.Core/Entities/Oferta.cs
❌ Rodavia.Core/Entities/ProductoVehiculoCompatible.cs
```

✅ **MANTENER:**
- `Marca.cs`
- `Modelo.cs`
- `Usuario.cs`
- `Vehiculo.cs`
- `Taller.cs`
- `PublicacionForo.cs`
- `RespuestaForo.cs`
- `ResenasTaller.cs`

### **Paso 1-B: Eliminar DTOs Obsoletos (Rodavia.Core/DTOs)**

Eliminar estos archivos (si existen):
```
❌ ProductoDto.cs
❌ TiendaDto.cs
❌ OfertaDto.cs
```

✅ **MANTENER:**
- `TallerDto.cs`
- `ForoDto.cs`
- `VinInfo.cs`

### **Paso 1-C: Limpiar RodaviaDbContext.cs**

Archivo: `Rodavia.Infrastructure/Data/RodaviaDbContext.cs`

**Eliminar estos DbSets:**
```csharp
❌ public DbSet<Producto> Productos { get; set; }
❌ public DbSet<Tienda> Tiendas { get; set; }
❌ public DbSet<Oferta> Ofertas { get; set; }
❌ public DbSet<ProductoVehiculoCompatible> ProductoVehiculoCompatibles { get; set; }
```

✅ **MANTENER:**
```csharp
public DbSet<Marca> Marcas { get; set; }
public DbSet<Modelo> Modelos { get; set; }
public DbSet<Usuario> Usuarios { get; set; }
public DbSet<Vehiculo> Vehiculos { get; set; }
public DbSet<Taller> Talleres { get; set; }
public DbSet<PublicacionForo> PublicacionesForo { get; set; }
public DbSet<RespuestaForo> RespuestasForo { get; set; }
public DbSet<ResenasTaller> ReseniasTaller { get; set; }
```

### **Paso 1-D: Limpiar Interfaces de Servicios**

Archivo: `Rodavia.Infrastructure/Services/IServices.cs`

**Eliminar estas interfaces:**
```csharp
❌ IProductoService
❌ ITiendaService
❌ IComparadorService
```

✅ **MANTENER:**
- `ITallerService`
- `IForoService`
- `IVehiculoService`
- `IVinDecoderService` (lo refactorizaremos en Fase 2)

### **Paso 1-E: Eliminar Servicios Obsoletos (Rodavia.Infrastructure/Services)**

Eliminar estos archivos:
```
❌ ProductoService.cs
❌ TiendaService.cs
❌ ComparadorService.cs
```

### **Paso 1-F: Limpiar Program.cs**

Archivo: `Rodavia.Web/Rodavia.Web/Program.cs`

**Eliminar estos registros de servicios:**
```csharp
❌ builder.Services.AddScoped<IProductoService, ProductoService>();
❌ builder.Services.AddScoped<ITiendaService, TiendaService>();
❌ builder.Services.AddScoped<IComparadorService, ComparadorService>();
❌ // Cualquier registro relacionado con Scrapers
```

**Eliminar la sección de "ScrapingSettings":**
```csharp
❌ // Configuración de servicios de scraping
❌ var scrapingSettings = builder.Configuration.GetSection("ScrapingSettings");
❌ // ... todo lo relacionado
```

### **Paso 1-G: Eliminar Páginas UI Obsoletas**

Eliminar estos archivos (si existen):
```
❌ Rodavia.Web/Components/Pages/Productos.razor
❌ Rodavia.Web/Components/Pages/ComparadorPrecios.razor
❌ Rodavia.Web/Components/Pages/DetalleProducto.razor
❌ Rodavia.Web/Components/Pages/Repuestos.razor
❌ Rodavia.Web/Services/ScraperIntegrationService.cs
```

### **Paso 1-H: Limpiar NavMenu.razor**

Archivo: `Rodavia.Web/Components/Layout/NavMenu.razor`

**Eliminar el enlace a Repuestos:**
```razor
❌ <div class="nav-item px-3">
❌     <NavLink class="nav-link" href="repuestos">
❌         <span class="fas fa-shopping-cart" aria-hidden="true"></span> Repuestos
❌     </NavLink>
❌ </div>
```

### **Paso 1-I: Limpiar appsettings.json**

Archivo: `Rodavia.Web/appsettings.json`

**Eliminar la sección "ScrapingSettings":**
```json
❌ "ScrapingSettings": {
❌     "Autoplanet": { ... },
❌     "MercadoLibre": { ... },
❌     "MundoRepuestos": { ... }
❌ }
```

### **Paso 1-J: Crear Migración de Limpieza**

```powershell
# Navegar al proyecto Web
cd Rodavia.Web\Rodavia.Web

# Crear migración que eliminará las tablas obsoletas
dotnet ef migrations add RemoveScrapingModel --context RodaviaDbContext

# Aplicar la migración
dotnet ef database update --context RodaviaDbContext

# Volver al directorio raíz
cd ..\..
```

---

## 🚀 **FASE 2: IMPLEMENTACIÓN (Nueva Funcionalidad)**

### **Cambios Arquitectónicos:**

1. **Renombrar `IVinDecoderService` → `IVehiculoInfoService`**
2. **Renombrar `NhtsaVinDecoderService` → `NhtsaVinService`**
3. **Crear `GetApiPatenteService`** (nuevo)
4. **Crear `CompositeVehiculoInfoService`** (reemplazo de `CompositeVinDecoderService`)
5. **Refactorizar `VinDecoder.razor` → `ConsultaVehiculo.razor`**

### **Nueva Interfaz: `IVehiculoInfoService`**

```csharp
public interface IVehiculoInfoService
{
    /// <summary>
    /// Obtiene información del vehículo por VIN (17 caracteres)
    /// Usa NHTSA API (EE.UU.)
    /// </summary>
    Task<VehiculoInfo?> GetInfoByVinAsync(string vin);
    
    /// <summary>
    /// Obtiene información del vehículo por Patente Chilena (6 caracteres)
    /// Usa GetAPI.cl (Chile y Latinoamérica)
    /// </summary>
    Task<VehiculoInfo?> GetInfoByPatenteAsync(string patente);
}
```

### **Nuevo DTO: `VehiculoInfo`**

```csharp
public class VehiculoInfo
{
    // Identificación
    public string? Vin { get; set; }
    public string? Patente { get; set; }
    
    // Información del vehículo
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string ModelYear { get; set; } = string.Empty;
    public string ManufacturerName { get; set; } = string.Empty;
    
    // Motor
    public string EngineDisplacementL { get; set; } = string.Empty;
    public string EngineCylinders { get; set; } = string.Empty;
    
    // Tipo de vehículo
    public string VehicleType { get; set; } = string.Empty;
    public string BodyClass { get; set; } = string.Empty;
    public string Doors { get; set; } = string.Empty;
    
    // Propulsión
    public string FuelTypePrimary { get; set; } = string.Empty;
    public string TransmissionStyle { get; set; } = string.Empty;
    public string DriveType { get; set; } = string.Empty;
    
    // Otros sistemas
    public string BrakeSystemType { get; set; } = string.Empty;
    
    // Manufactura
    public string PlantCountry { get; set; } = string.Empty;
    public string PlantCity { get; set; } = string.Empty;
    
    // Metadatos
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
    public string Source { get; set; } = string.Empty; // "NHTSA", "GetAPI", etc.
}
```

### **Nuevo Servicio: `GetApiPatenteService`**

**Endpoint:** `https://chile.getapi.cl/v1/vehicles/plate/{patente}`

**Header:** `X-Api-Key: TU_API_KEY`

**Ejemplo de respuesta:**
```json
{
  "success": true,
  "data": {
    "plate": "ABCD12",
    "vin": "1HGBH41JXMN109186",
    "make": "Honda",
    "model": "Accord",
    "year": 2021,
    "color": "Negro",
    "engineSize": "2.0L",
    "fuelType": "Gasolina"
  }
}
```

### **Registro de Servicios en Program.cs:**

```csharp
// Servicios de información vehicular
builder.Services.AddScoped<NhtsaVinService>();
builder.Services.AddScoped<GetApiPatenteService>();
builder.Services.AddScoped<IVehiculoInfoService, CompositeVehiculoInfoService>();
```

---

## 📊 **RESULTADO FINAL:**

### **Antes (Comparador de Precios):**
```
🛒 Scraper → Productos/Tiendas/Ofertas → Comparador UI
```

### **Después (Centro de Información Vehicular):**
```
🚗 VIN/Patente → APIs (NHTSA/GetAPI) → Información Completa
```

### **Funcionalidades Finales:**
1. ✅ **Búsqueda por Patente Chilena** (GetAPI.cl)
2. ✅ **Búsqueda por VIN** (NHTSA)
3. ✅ **Directorio de Talleres**
4. ✅ **Foro Comunitario**
5. ❌ **Comparador de Precios** (ELIMINADO)

---

## 🎯 **Próximos Pasos Recomendados:**

1. Completar limpieza manual (Fase 1)
2. Ejecutar migraciones de base de datos
3. Implementar servicios de Fase 2
4. Probar con patentes chilenas reales
5. Desplegar a producción

---

**Fecha de Pivote:** 19 de Octubre de 2025  
**Razón:** Web scraping inestable → APIs estables y confiables  
**Inspiración:** Autofact.cl (líder en información vehicular Chile)

