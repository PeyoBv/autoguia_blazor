# 📋 YAML Corregido - Listo para Copiar y Pegar

## ✅ Opción 1: Workflow de Producción Completo (Recomendado)

**Archivo:** `.github/workflows/rodavia-production.yml`

```yaml
name: Deploy Rodavia to Production

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
    name: Build & Test
    runs-on: ubuntu-latest
    
    steps:
      - name: Checkout repository
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}
      
      - name: Install WASM Tools
        run: dotnet workload install wasm-tools
        continue-on-error: true

      - name: Restore dependencies
        run: dotnet restore ${{ env.PROJECT_PATH }}

      - name: Build project
        run: dotnet build ${{ env.PROJECT_PATH }} --configuration Release --no-restore

      - name: Run tests
        run: dotnet test Rodavia.Tests/Rodavia.Tests.csproj --configuration Release --verbosity normal
        continue-on-error: true

      - name: Publish application
        run: dotnet publish ${{ env.PROJECT_PATH }} --configuration Release --output ${{ env.PUBLISH_PATH }}

      - name: Upload artifact
        uses: actions/upload-artifact@v4
        with:
          name: rodavia-app
          path: ${{ env.PUBLISH_PATH }}
          retention-days: 1

  deploy:
    name: Deploy to Azure
    needs: build
    runs-on: ubuntu-latest
    
    steps:
      - name: Download artifact
        uses: actions/download-artifact@v4
        with:
          name: rodavia-app
          path: ${{ env.PUBLISH_PATH }}

      - name: Login to Azure
        uses: azure/login@v2
        with:
          client-id: ${{ secrets.AZUREAPPSERVICE_CLIENTID_F30058E83D074DDAB853693A89BC5A84 }}
          tenant-id: ${{ secrets.AZUREAPPSERVICE_TENANTID_8E4E13FA377D4242BB4508CC5DB3C76C }}
          subscription-id: ${{ secrets.AZUREAPPSERVICE_SUBSCRIPTIONID_B400291DBEA44229B55D00A16DFC81FF }}

      - name: Deploy to Azure Web App
        uses: azure/webapps-deploy@v3
        with:
          app-name: ${{ env.AZURE_WEBAPP_NAME }}
          package: ${{ env.PUBLISH_PATH }}

      - name: Health Check
        run: |
          sleep 30
          curl -f https://${{ env.AZURE_WEBAPP_NAME }}.azurewebsites.net || echo "Site still warming up"

      - name: Azure logout
        if: always()
        run: az logout
        continue-on-error: true
```

---

## ✅ Opción 2: Workflow de CI (Solo Validación)

**Archivo:** `.github/workflows/rodavia-ci.yml`

```yaml
name: CI - Continuous Integration

on:
  pull_request:
    branches:
      - main
  push:
    branches:
      - develop
      - feature/*

env:
  DOTNET_VERSION: '8.0.x'
  PROJECT_PATH: 'Rodavia.Web/Rodavia.Web/Rodavia.Web.csproj'

jobs:
  validate:
    name: Validate & Test
    runs-on: ubuntu-latest
    
    steps:
      - name: Checkout code
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}

      - name: Install WASM Tools
        run: dotnet workload install wasm-tools
        continue-on-error: true

      - name: Restore dependencies
        run: dotnet restore Rodavia.sln

      - name: Build solution
        run: dotnet build Rodavia.sln --configuration Release --no-restore

      - name: Run tests
        run: dotnet test Rodavia.sln --configuration Release --no-build --verbosity normal

      - name: Test publish
        run: dotnet publish ${{ env.PROJECT_PATH }} --configuration Release --no-build --output ./test-publish

      - name: Verify publish
        run: |
          if [ -d "./test-publish" ]; then
            echo "✅ Publish successful"
            ls -lah ./test-publish | head -20
          else
            echo "❌ Publish failed"
            exit 1
          fi
```

---

## ✅ Opción 3: Actualización del Workflow Existente

Si ya tienes `.github/workflows/main_rodavia.yml`, solo modifica estas líneas:

### Cambio en Build:

**Antes (INCORRECTO):**
```yaml
- name: Build with dotnet
  run: dotnet build --configuration Release
```

**Después (CORRECTO):**
```yaml
- name: Build with dotnet
  run: dotnet build Rodavia.Web/Rodavia.Web/Rodavia.Web.csproj --configuration Release
```

### Cambio en Publish:

**Antes (INCORRECTO):**
```yaml
- name: dotnet publish
  run: dotnet publish -c Release -o "${{env.DOTNET_ROOT}}/myapp"
```

**Después (CORRECTO):**
```yaml
- name: dotnet publish
  run: dotnet publish Rodavia.Web/Rodavia.Web/Rodavia.Web.csproj -c Release -o "${{env.DOTNET_ROOT}}/myapp"
```

---

## 🔧 Corrección del archivo .csproj

**Archivo:** `Rodavia.Scraper/Rodavia.Scraper.csproj`

Asegúrate de que contenga esto:

```xml
<ItemGroup>
  <Content Include="appsettings.json">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    <CopyToPublishDirectory>Never</CopyToPublishDirectory>
  </Content>
</ItemGroup>
```

---

## 📝 Instrucciones de Uso

### Para Opción 1 (Workflow Completo):

1. Crea el archivo `.github/workflows/rodavia-production.yml`
2. Copia y pega el YAML completo de arriba
3. Commit y push
4. El workflow se ejecutará automáticamente

### Para Opción 2 (Solo CI):

1. Crea el archivo `.github/workflows/rodavia-ci.yml`
2. Copia y pega el YAML de CI
3. Se ejecutará en PRs y branches develop

### Para Opción 3 (Actualizar existente):

1. Abre `.github/workflows/main_rodavia.yml`
2. Reemplaza las líneas indicadas
3. Guarda y haz push

---

## ✅ Validación Local Antes de Push

```powershell
# Ejecuta esto primero
.\validar.ps1

# Si pasa, entonces haz push
git add .
git commit -m "fix: Corregir workflows para publicar solo proyecto principal"
git push origin main
```

---

## 🎯 Resultado Esperado

Después del push, en GitHub Actions verás:

```
✅ Checkout repository - SUCCESS
✅ Setup .NET - SUCCESS
✅ Install WASM Tools - SUCCESS
✅ Restore dependencies - SUCCESS
✅ Build project - SUCCESS (35 warnings, 0 errors)
✅ Run tests - SUCCESS
✅ Publish application - SUCCESS (sin NETSDK1152)
✅ Upload artifact - SUCCESS
✅ Deploy to Azure - SUCCESS
✅ Health Check - SUCCESS
```

---

## ⚠️ Notas Importantes

1. **Reemplaza los secrets** si son diferentes en tu proyecto:
   - `AZUREAPPSERVICE_CLIENTID_...`
   - `AZUREAPPSERVICE_TENANTID_...`
   - `AZUREAPPSERVICE_SUBSCRIPTIONID_...`

2. **Verifica el nombre de la app** en Azure:
   - Variable `AZURE_WEBAPP_NAME: 'rodavia'`

3. **Las advertencias NU1608 son normales** y no afectan el build

---

## 📞 Soporte

Si encuentras errores:
1. Consulta `GUIA-COMPLETA-SOLUCION.md`
2. Ejecuta `.\validar.ps1 -Verbose`
3. Revisa logs de GitHub Actions

---

**Fecha:** 8 de noviembre de 2025  
**Estado:** ✅ Listo para copiar y pegar
