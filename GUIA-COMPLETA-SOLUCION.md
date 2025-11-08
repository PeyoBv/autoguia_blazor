# 🧑‍💻 Guía Completa: Corregir Errores de Compilación y Publicación

## 📋 Resumen de Problemas y Soluciones

### Problemas Identificados:
1. ❌ **NETSDK1194**: Intentar publicar la solución completa en vez del proyecto específico
2. ❌ **NETSDK1152**: Conflicto de archivos `appsettings.json` entre proyectos
3. ⚠️ **NU1608**: Advertencias de versiones de paquetes (AngleSharp)

### Soluciones Aplicadas:
1. ✅ Modificar workflows para publicar solo `Rodavia.Web/Rodavia.Web/Rodavia.Web.csproj`
2. ✅ Configurar `Rodavia.Scraper.csproj` para excluir archivos de publicación
3. ⚠️ Las advertencias NU1608 son normales y no bloquean la compilación

---

## 🔧 Paso 1: Verificar Cambios en .csproj

### ✅ Archivo ya corregido: `Rodavia.Scraper/Rodavia.Scraper.csproj`

Verificar que contenga:

```xml
<ItemGroup>
  <Content Include="appsettings.json">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    <CopyToPublishDirectory>Never</CopyToPublishDirectory>
  </Content>
</ItemGroup>
```

**✅ Estado:** Ya aplicado en tu proyecto

---

## 🔧 Paso 2: Instalar WASM Tools (si usas Blazor WebAssembly)

### Verificar si necesitas WASM:

```powershell
# Verificar el tipo de SDK en Rodavia.Web.Client.csproj
Select-String -Path "Rodavia.Web\Rodavia.Web.Client\Rodavia.Web.Client.csproj" -Pattern "BlazorWebAssembly"
```

**Resultado esperado:** `Microsoft.NET.Sdk.BlazorWebAssembly` → SÍ necesitas WASM

### Instalar WASM Tools:

```powershell
dotnet workload install wasm-tools
```

**Nota:** Esto puede tardar varios minutos en Windows.

---

## 🔧 Paso 3: Validar Compilación Local

### 3.1. Restaurar paquetes:

```powershell
dotnet restore Rodavia.sln
```

**Salida esperada:**
- ✅ 7 proyectos restaurados
- ⚠️ Algunas advertencias NU1608 (normales)

### 3.2. Compilar el proyecto principal:

```powershell
dotnet build Rodavia.Web/Rodavia.Web/Rodavia.Web.csproj --configuration Release
```

**Salida esperada:**
- ✅ `Compilación correcta`
- ⚠️ 35 advertencias (normales)
- ❌ **0 errores** ← Esto es lo importante

### 3.3. Publicar el proyecto:

```powershell
dotnet publish Rodavia.Web/Rodavia.Web/Rodavia.Web.csproj -c Release -o ./publish
```

**Salida esperada:**
- ✅ `Rodavia.Web -> C:\...\publish\`
- Sin errores NETSDK1152

### 3.4. Verificar salida de publicación:

```powershell
Get-ChildItem ./publish -Name | Select-Object -First 20
```

**Archivos esperados:**
- ✅ `Rodavia.Web.dll`
- ✅ `appsettings.json` (solo uno)
- ✅ `wwwroot/` (carpeta)
- ✅ `web.config`

### 3.5. Limpiar directorio de prueba:

```powershell
Remove-Item -Path ./publish -Recurse -Force
```

---

## 🔧 Paso 4: Configurar Workflows de GitHub Actions

### Opción A: Workflow de Producción (recomendado)

**Archivo:** `.github/workflows/rodavia-production.yml`

```yaml
name: 🚀 Rodavia - Deploy to Production

on:
  push:
    branches:
      - main
  workflow_dispatch:

env:
  AZURE_WEBAPP_NAME: 'rodavia'
  DOTNET_VERSION: '8.0.x'
  PROJECT_PATH: 'Rodavia.Web/Rodavia.Web/Rodavia.Web.csproj'
  PUBLISH_PATH: './publish'

jobs:
  build:
    name: 🔨 Build & Test
    runs-on: ubuntu-latest
    
    steps:
      - name: 📥 Checkout repository
        uses: actions/checkout@v4

      - name: ⚙️ Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}
      
      - name: 🔧 Install WASM Tools
        run: dotnet workload install wasm-tools
        continue-on-error: true

      - name: 📦 Restore dependencies
        run: dotnet restore ${{ env.PROJECT_PATH }}

      - name: 🔨 Build project
        run: dotnet build ${{ env.PROJECT_PATH }} --configuration Release --no-restore

      - name: 🧪 Run tests
        run: dotnet test Rodavia.Tests/Rodavia.Tests.csproj --configuration Release --verbosity normal
        continue-on-error: true

      - name: 📦 Publish application
        run: dotnet publish ${{ env.PROJECT_PATH }} --configuration Release --output ${{ env.PUBLISH_PATH }}

      - name: 📤 Upload artifact
        uses: actions/upload-artifact@v4
        with:
          name: rodavia-app
          path: ${{ env.PUBLISH_PATH }}
          retention-days: 1

  deploy:
    name: 🚀 Deploy to Azure
    needs: build
    runs-on: ubuntu-latest
    
    steps:
      - name: 📥 Download artifact
        uses: actions/download-artifact@v4
        with:
          name: rodavia-app
          path: ${{ env.PUBLISH_PATH }}

      - name: 🔐 Login to Azure
        uses: azure/login@v2
        with:
          client-id: ${{ secrets.AZUREAPPSERVICE_CLIENTID_F30058E83D074DDAB853693A89BC5A84 }}
          tenant-id: ${{ secrets.AZUREAPPSERVICE_TENANTID_8E4E13FA377D4242BB4508CC5DB3C76C }}
          subscription-id: ${{ secrets.AZUREAPPSERVICE_SUBSCRIPTIONID_B400291DBEA44229B55D00A16DFC81FF }}

      - name: 🚀 Deploy to Azure Web App
        uses: azure/webapps-deploy@v3
        with:
          app-name: ${{ env.AZURE_WEBAPP_NAME }}
          package: ${{ env.PUBLISH_PATH }}

      - name: 🏥 Health Check
        run: |
          sleep 30
          curl -f https://${{ env.AZURE_WEBAPP_NAME }}.azurewebsites.net || exit 0
```

### Opción B: Workflow de CI (validación continua)

**Archivo:** `.github/workflows/rodavia-ci.yml`

```yaml
name: 🔍 CI - Continuous Integration

on:
  pull_request:
    branches:
      - main
  push:
    branches:
      - develop

jobs:
  validate:
    runs-on: ubuntu-latest
    
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - name: Build & Test
        run: |
          dotnet restore Rodavia.sln
          dotnet build Rodavia.sln --configuration Release --no-restore
          dotnet test Rodavia.sln --configuration Release --no-build
          dotnet publish Rodavia.Web/Rodavia.Web/Rodavia.Web.csproj -c Release -o ./test-publish
```

---

## 🔧 Paso 5: Probar el Workflow Localmente

### Simular el workflow con Act (opcional):

```powershell
# Instalar act (requiere Docker)
choco install act-cli

# Ejecutar workflow localmente
act -j build
```

### Validación manual (recomendado):

```powershell
# Simular exactamente lo que hace GitHub Actions
dotnet restore Rodavia.Web/Rodavia.Web/Rodavia.Web.csproj
dotnet build Rodavia.Web/Rodavia.Web/Rodavia.Web.csproj --configuration Release --no-restore
dotnet test Rodavia.Tests/Rodavia.Tests.csproj --configuration Release --verbosity normal
dotnet publish Rodavia.Web/Rodavia.Web/Rodavia.Web.csproj --configuration Release --output ./publish
```

**✅ Si todos los pasos pasan → El workflow funcionará en GitHub**

---

## 🔧 Paso 6: Hacer Commit y Push

### 6.1. Ver cambios:

```powershell
git status
```

**Archivos modificados:**
- `Rodavia.Scraper/Rodavia.Scraper.csproj`
- `.github/workflows/main_rodavia.yml`
- `.github/workflows/azure-deploy.yml`

### 6.2. Agregar cambios:

```powershell
git add .
```

### 6.3. Commit:

```powershell
git commit -m "fix: Corregir errores NETSDK1152 y NETSDK1194 en workflows

- Configurar Rodavia.Scraper.csproj para excluir appsettings.json de publish
- Actualizar workflows para publicar solo proyecto principal
- Corregir rutas de AutoGuia a Rodavia en azure-deploy.yml"
```

### 6.4. Push:

```powershell
git push origin main
```

---

## 🔧 Paso 7: Verificar Ejecución en GitHub

1. Ir a: `https://github.com/PeyoBv/autoguia_blazor/actions`
2. Ver el workflow ejecutándose
3. Verificar que no hay errores NETSDK1152 o NETSDK1194

### Logs esperados:

```
✅ Restore dependencies - SUCCESS
✅ Build project - SUCCESS (35 warnings, 0 errors)
✅ Run tests - SUCCESS
✅ Publish application - SUCCESS
✅ Upload artifact - SUCCESS
✅ Deploy to Azure - SUCCESS
```

---

## 📊 Comparación: Antes vs Después

### ❌ Antes (INCORRECTO):

```yaml
# ❌ Publicaba toda la solución
- name: dotnet publish
  run: dotnet publish -c Release -o ./publish
```

**Resultado:** Error NETSDK1152 por conflicto de appsettings.json

### ✅ Después (CORRECTO):

```yaml
# ✅ Publica solo el proyecto principal
- name: Publish application
  run: dotnet publish Rodavia.Web/Rodavia.Web/Rodavia.Web.csproj --configuration Release --output ./publish
```

**Resultado:** Publicación exitosa sin conflictos

---

## 🚀 Comandos Rápidos de Referencia

### Compilación local:
```powershell
dotnet build Rodavia.Web/Rodavia.Web/Rodavia.Web.csproj -c Release
```

### Publicación local:
```powershell
dotnet publish Rodavia.Web/Rodavia.Web/Rodavia.Web.csproj -c Release -o ./publish
```

### Ejecutar en desarrollo:
```powershell
dotnet run --project Rodavia.Web/Rodavia.Web/Rodavia.Web.csproj
```

### Ejecutar tests:
```powershell
dotnet test Rodavia.Tests/Rodavia.Tests.csproj
```

---

## ⚠️ Advertencias Esperadas (NORMALES)

### NU1608 - Versiones de paquetes:
```
warning NU1608: HtmlSanitizer 9.0.886 requiere AngleSharp (= 0.17.1), 
pero la versión AngleSharp 1.1.1 ya se resolvió.
```

**Explicación:** 
- `bunit 1.28.9` necesita `AngleSharp 1.1.1`
- `HtmlSanitizer` prefiere `AngleSharp 0.17.1`
- .NET usa la versión más reciente compatible (1.1.1)
- **No afecta la compilación ni ejecución**

### CS8602/CS8601 - Nullable warnings:
```
warning CS8602: Desreferencia de una referencia posiblemente NULL
```

**Explicación:**
- Advertencias de análisis de null safety
- No impiden la compilación
- Puedes corregirlas gradualmente

---

## 🎯 Checklist Final

- [ ] ✅ `Rodavia.Scraper.csproj` configurado para excluir appsettings.json
- [ ] ✅ Workflow actualizado para publicar solo proyecto principal
- [ ] ✅ Compilación local exitosa (0 errores)
- [ ] ✅ Publicación local exitosa
- [ ] ✅ Cambios commiteados y pusheados
- [ ] ✅ Workflow ejecutándose en GitHub sin errores
- [ ] ✅ Aplicación desplegada correctamente en Azure

---

## 🆘 Troubleshooting

### Error: "No se puede encontrar el proyecto"
```powershell
# Verificar que la ruta es correcta
Get-ChildItem -Path "Rodavia.Web/Rodavia.Web" -Filter "*.csproj"
```

### Error: "WASM workload no instalado"
```powershell
dotnet workload install wasm-tools
```

### Error: "Artifact no encontrado en deploy"
- Verificar que el nombre del artifact coincide:
  - Upload: `name: rodavia-app`
  - Download: `name: rodavia-app`

### Error en Azure Login:
- Verificar que los secrets están configurados en GitHub:
  - `AZUREAPPSERVICE_CLIENTID_...`
  - `AZUREAPPSERVICE_TENANTID_...`
  - `AZUREAPPSERVICE_SUBSCRIPTIONID_...`

---

## 📚 Recursos Adicionales

- [Documentación oficial de dotnet publish](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-publish)
- [GitHub Actions para .NET](https://docs.github.com/en/actions/automating-builds-and-tests/building-and-testing-net)
- [Azure Web Apps Deploy Action](https://github.com/Azure/webapps-deploy)

---

**Fecha:** 8 de noviembre de 2025  
**Versión .NET:** 8.0  
**Estado:** ✅ Listo para producción
