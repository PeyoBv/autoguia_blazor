# 🚀 Crear App Service "Rodavia" con GitHub Actions

## ✅ Paso 1: Crear App Service en Azure Portal

### 1.1 Ir a crear Web App

Abre este enlace:
```
https://portal.azure.com/#create/Microsoft.WebSite
```

O en Azure Portal:
- Click en "Crear un recurso"
- Busca "Web App"
- Click en "Crear"

### 1.2 Configuración Básica

**Pestaña "Basics":**

```
Suscripción: [Tu suscripción actual]
Resource Group: 
  - Usa existente: rodavia_group
  - O crea nuevo: rodavia_group

Name: rodavia
  (Esto creará: rodavia.azurewebsites.net)

Publish: Code
Runtime stack: .NET 8 (LTS)
Operating System: Windows
Region: Chile Central (o tu región preferida)

Pricing Plan:
  - Click "Create new"
  - Name: ASP-rodavia
  - Size: B1 (Basic - $13.14/mes)
    O Free F1 si prefieres (con limitaciones)
```

### 1.3 Configuración de Deployment

**Pestaña "Deployment":**

```
GitHub Actions settings: Enable

GitHub account: [Tu cuenta - PeyoBv]
Organization: PeyoBv
Repository: autoguia_blazor
Branch: main
```

⚠️ **IMPORTANTE**: Si pide autorización de GitHub:
- Click "Authorize Azure App Service"
- Completa la autorización en GitHub

### 1.4 Revisar y Crear

**Pestaña "Review + create":**
- Revisa que todo esté correcto
- Click "Create"
- Espera 2-3 minutos

---

## ✅ Paso 2: Azure Creará Automáticamente el Workflow

Cuando Azure termine de crear el App Service:

1. **Automáticamente agregará** un archivo a tu repo:
   ```
   .github/workflows/main_rodavia.yml
   ```

2. **Este archivo contiene** la configuración de GitHub Actions

3. **El primer deploy** se ejecutará automáticamente

---

## ✅ Paso 3: Modificar el Workflow Generado

El workflow que Azure crea necesita ajustes para tu proyecto.

### 3.1 Pull del nuevo workflow

```powershell
# Traer los cambios de GitHub (el workflow que Azure creó)
git pull origin main
```

### 3.2 Editar el workflow

Abre el archivo que Azure creó:
```
.github/workflows/main_rodavia.yml
```

Reemplaza TODO el contenido con este (optimizado para tu proyecto):

```yaml
name: Deploy Rodavia to Azure

on:
  push:
    branches:
      - main
  workflow_dispatch:

env:
  AZURE_WEBAPP_NAME: rodavia
  DOTNET_VERSION: '8.0.x'

jobs:
  build-and-deploy:
    runs-on: windows-latest

    steps:
    - name: 📥 Checkout código
      uses: actions/checkout@v4

    - name: ⚙️ Configurar .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}

    - name: 📦 Restaurar dependencias
      run: dotnet restore Rodavia.sln

    - name: 🔨 Build
      run: dotnet build Rodavia.sln --configuration Release --no-restore

    - name: 📤 Publish
      run: dotnet publish Rodavia.Web/Rodavia.Web/Rodavia.Web.csproj -c Release -o ./publish --no-build

    - name: 🚀 Deploy to Azure Web App
      uses: azure/webapps-deploy@v3
      with:
        app-name: ${{ env.AZURE_WEBAPP_NAME }}
        publish-profile: ${{ secrets.AZUREAPPSERVICE_PUBLISHPROFILE_XXXXXXXXXX }}
        package: ./publish
```

⚠️ **NOTA**: NO cambies el nombre del secret `AZUREAPPSERVICE_PUBLISHPROFILE_XXXXXXXXXX`, Azure lo genera automáticamente.

---

## ✅ Paso 4: Configurar Variables de Entorno en Azure

### 4.1 Ir a Configuration

En Azure Portal:
```
rodavia > Configuration > Application settings
```

### 4.2 Agregar variables

Click "New application setting" para cada una:

```
Authentication__Google__ClientId
  Value: [TU_GOOGLE_CLIENT_ID]

Authentication__Google__ClientSecret
  Value: [TU_GOOGLE_CLIENT_SECRET]

ASPNETCORE_ENVIRONMENT
  Value: Production

ConnectionStrings__DefaultConnection
  Value: Data Source=rodavia.db
```

### 4.3 Guardar

- Click "Save" (arriba)
- Click "Continue" en el diálogo de confirmación
- Espera que Azure reinicie el App Service

---

## ✅ Paso 5: Commit y Push

```powershell
# Ver cambios
git status

# Agregar el workflow modificado
git add .github/workflows/

# Commit
git commit -m "feat: Configurar GitHub Actions para deploy automático a Azure"

# Push (esto activará el deploy automático)
git push origin main
```

---

## ✅ Paso 6: Monitorear el Deploy

### 6.1 Ver el workflow en GitHub

```
https://github.com/PeyoBv/autoguia_blazor/actions
```

Verás:
- 📥 Checkout código
- ⚙️ Configurar .NET
- 📦 Restaurar dependencias
- 🔨 Build
- 📤 Publish
- 🚀 Deploy to Azure

⏱️ Tiempo: 5-8 minutos

### 6.2 Ver logs en tiempo real

Click en el workflow que está corriendo > Click en "build-and-deploy"

---

## ✅ Paso 7: Verificar la Aplicación

### 7.1 Obtener la URL

En Azure Portal:
```
rodavia > Overview
```

La URL será algo como:
```
https://rodavia-[hash].azurewebsites.net
```

### 7.2 Abrir la app

- Espera 2-3 minutos después del deploy
- Abre la URL
- Verifica que cargue correctamente

---

## ✅ Paso 8: Actualizar Google OAuth

### 8.1 Copiar la URL exacta de tu app

Ejemplo: `https://rodavia-abc123xyz.azurewebsites.net`

### 8.2 Ir a Google Console

```
https://console.cloud.google.com/apis/credentials
```

### 8.3 Editar OAuth Client

- Click en tu Client ID
- En "Authorized redirect URIs"
- Agregar estas dos (con TU URL):

```
https://[TU-URL].azurewebsites.net/signin-google
https://[TU-URL].azurewebsites.net/Account/ExternalLoginCallback
```

Ejemplo con URL real:
```
https://rodavia-abc123xyz.azurewebsites.net/signin-google
https://rodavia-abc123xyz.azurewebsites.net/Account/ExternalLoginCallback
```

- Click "Save"
- Espera 1-2 minutos

### 8.4 Probar Google Login

- Abre tu app
- Click en "Iniciar sesión con Google"
- Debe funcionar correctamente

---

## 🎉 ¡Listo! Workflow Automático Configurado

De ahora en adelante, cada vez que hagas `git push`:

```powershell
# 1. Haces cambios en el código

# 2. Commit
git add .
git commit -m "feat: nueva funcionalidad"

# 3. Push (deploy automático)
git push origin main

# 4. GitHub Actions despliega automáticamente
# 5. En 5-8 minutos, cambios en producción ✅
```

---

## 📊 Verificación Final

Checklist:
- [ ] App Service "rodavia" creado en Azure
- [ ] GitHub Actions configurado
- [ ] Variables de entorno configuradas
- [ ] Workflow corriendo correctamente
- [ ] App carga en la URL de Azure
- [ ] Google OAuth URIs actualizadas
- [ ] Login con Google funciona

---

## 🔧 Solución de Problemas

### Si el workflow falla:

1. **Click en el workflow fallido** en GitHub Actions
2. **Lee el error** en los logs
3. **Errores comunes:**

   **"dotnet: command not found"**
   - El runner necesita .NET instalado
   - Verifica que uses `runs-on: windows-latest`

   **"Path does not exist: ./publish"**
   - Verifica la ruta en el comando publish
   - Debe ser: `-o ./publish`

   **"App Service not found"**
   - Verifica que el nombre sea exactamente "rodavia"
   - Verifica el secret del publish profile

### Ver logs de Azure:

```
Azure Portal > rodavia > Log stream
```

---

## 💡 URLs de Referencia

- **GitHub Repo**: https://github.com/PeyoBv/autoguia_blazor
- **GitHub Actions**: https://github.com/PeyoBv/autoguia_blazor/actions
- **Azure Portal**: https://portal.azure.com
- **Google Console**: https://console.cloud.google.com/apis/credentials

---

## 🚀 Próximos Pasos Opcionales

### Agregar ambiente de staging:

```yaml
# Deploy a staging en cada PR
on:
  pull_request:
    branches: [ main ]
```

### Agregar tests automáticos:

```yaml
- name: 🧪 Run Tests
  run: dotnet test Rodavia.sln --no-build
```

### Notificaciones de deploy:

- Configura notificaciones en GitHub
- Settings > Notifications > Actions
