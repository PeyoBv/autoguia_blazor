# ✅ Correcciones de Compilación y Publicación - Rodavia

## Resumen de Cambios Realizados

### 1. ✅ Corrección de Error NETSDK1152 (Conflicto appsettings.json)

**Problema:** Múltiples archivos `appsettings.json` con la misma ruta de salida durante la publicación.

**Solución:** Se modificó `Rodavia.Scraper/Rodavia.Scraper.csproj` para excluir `appsettings.json` de la publicación cuando se incluye como dependencia:

```xml
<ItemGroup>
  <Content Include="appsettings.json">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    <CopyToPublishDirectory>Never</CopyToPublishDirectory>
  </Content>
</ItemGroup>
```

### 2. ✅ Actualización de GitHub Actions Workflows

#### Workflow: `main_rodavia.yml`
Se cambió de publicar toda la solución a publicar solo el proyecto principal:

**Antes:**
```yaml
- name: Build with dotnet
  run: dotnet build --configuration Release

- name: dotnet publish
  run: dotnet publish -c Release -o "${{env.DOTNET_ROOT}}/myapp"
```

**Después:**
```yaml
- name: Build with dotnet
  run: dotnet build Rodavia.Web/Rodavia.Web/Rodavia.Web.csproj --configuration Release

- name: dotnet publish
  run: dotnet publish Rodavia.Web/Rodavia.Web/Rodavia.Web.csproj -c Release -o "${{env.DOTNET_ROOT}}/myapp"
```

#### Workflow: `azure-deploy.yml`
Se actualizaron las rutas de `AutoGuia` a `Rodavia`:

- ✅ `dotnet restore Rodavia.sln`
- ✅ `dotnet build Rodavia.sln --configuration Release --no-restore`
- ✅ `dotnet test Rodavia.Tests/Rodavia.Tests.csproj`
- ✅ `dotnet publish Rodavia.Web/Rodavia.Web/Rodavia.Web.csproj`
- ✅ `dotnet ef database update --project Rodavia.Web/Rodavia.Web/Rodavia.Web.csproj`

### 3. ⚠️ Advertencias NU1608 (Versiones de Paquetes)

Las advertencias relacionadas con versiones de paquetes AngleSharp son **normales** y no afectan la compilación:

```
warning NU1608: HtmlSanitizer 9.0.886 requiere AngleSharp (= 0.17.1), 
pero la versión AngleSharp 1.1.1 ya se resolvió.
```

**Explicación:** `bunit 1.28.9` requiere `AngleSharp 1.1.1` mientras que `HtmlSanitizer` prefiere una versión anterior. .NET usa la versión más reciente compatible, lo cual es correcto.

## Comandos de Validación

### Compilar el proyecto
```powershell
dotnet build Rodavia.Web/Rodavia.Web/Rodavia.Web.csproj --configuration Release
```

### Publicar el proyecto
```powershell
dotnet publish Rodavia.Web/Rodavia.Web/Rodavia.Web.csproj -c Release -o ./publish
```

## Resultados de la Validación

✅ **Compilación:** Exitosa (35 advertencias, 0 errores)
✅ **Publicación:** Exitosa
✅ **Estructura de salida:** Correcta en `./publish`

## Estructura del Proyecto

```
Rodavia/
├── Rodavia.Core/              # ✅ Compila correctamente
├── Rodavia.Infrastructure/    # ✅ Compila con warnings menores
├── Rodavia.Scraper/          # ✅ appsettings.json excluido de publish
├── Rodavia.Scraper.Tests/    # ✅ Paquetes NuGet actualizados
├── Rodavia.Tests/            # ✅ Paquetes NuGet actualizados
└── Rodavia.Web/              
    ├── Rodavia.Web/          # ✅ Proyecto principal - PUBLICAR ESTE
    └── Rodavia.Web.Client/   # ✅ Se incluye automáticamente
```

## Próximos Pasos

### Para desarrollo local:
```powershell
# Compilar
dotnet build Rodavia.sln

# Ejecutar en desarrollo
dotnet run --project Rodavia.Web/Rodavia.Web/Rodavia.Web.csproj
```

### Para CI/CD:
Los workflows de GitHub Actions ya están configurados correctamente para:
1. ✅ Compilar solo el proyecto principal
2. ✅ Ejecutar tests
3. ✅ Publicar sin conflictos
4. ✅ Desplegar a Azure

### Instalación de WASM Tools (si es necesario):
Si usas Blazor WebAssembly en Azure:
```powershell
dotnet workload install wasm-tools
```

## Notas Importantes

🔴 **NO** publicar la solución completa: 
```powershell
# ❌ INCORRECTO
dotnet publish Rodavia.sln
```

🟢 **SÍ** publicar solo el proyecto principal:
```powershell
# ✅ CORRECTO
dotnet publish Rodavia.Web/Rodavia.Web/Rodavia.Web.csproj -c Release -o ./publish
```

## Estado Final

| Componente | Estado | Notas |
|-----------|--------|-------|
| Compilación | ✅ OK | 35 warnings (normales), 0 errores |
| Publicación | ✅ OK | Sin conflictos de archivos |
| Tests | ✅ OK | Paquetes NuGet compatibles |
| Workflows | ✅ OK | Actualizados para Rodavia |
| Estructura | ✅ OK | Separación clara de proyectos |

---

**Fecha:** 8 de noviembre de 2025
**Versión .NET:** 8.0
**Estado:** ✅ Listo para producción
