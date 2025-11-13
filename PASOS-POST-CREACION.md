# 🎯 Pasos Post-Creación del App Service

## ✅ Paso 1: Esperar que Azure termine (2-3 minutos)

Verás un mensaje: **"La implementación se completó"**

---

## ✅ Paso 2: Obtener el nombre del App Service

Cuando termine, Azure te mostrará:
- **Nombre del recurso**: Anótalo (ejemplo: rodavia-abc123)
- **URL**: La URL de tu aplicación

---

## ✅ Paso 3: Traer el Workflow de GitHub

Azure habrá creado automáticamente un archivo en tu repositorio.

```powershell
# Traer los cambios de GitHub
git pull origin main
```

Verás algo como:
```
remote: Enumerating objects: 5, done.
remote: Counting objects: 100% (5/5), done.
From https://github.com/PeyoBv/autoguia_blazor
   abc1234..def5678  main       -> origin/main
Updating abc1234..def5678
 .github/workflows/main_rodavia-XXXXX.yml | 75 +++++++++++++++++++++++
```

---

## ✅ Paso 4: Verificar el Workflow Creado

```powershell
# Ver archivos en .github/workflows
ls .github\workflows\
```

Deberías ver:
- `azure-deploy.yml` (el que creamos antes)
- `main_rodavia-XXXXX.yml` (el que Azure creó)

---

## ✅ Paso 5: Modificar el Workflow de Azure

El workflow que Azure crea necesita ajustes para tu proyecto Blazor.

### 5.1 Abrir el archivo

```powershell
# Listar archivos
ls .github\workflows\

# Copiar el nombre exacto y abrirlo
code .github\workflows\main_rodavia-XXXXX.yml
```

### 5.2 Buscar la sección de Publish

Busca esta línea (aproximadamente línea 30-40):
```yaml
- name: dotnet publish
  run: dotnet publish -c Release -o ${{env.DOTNET_ROOT}}/myapp
```

### 5.3 Reemplazar SOLO esa línea

Cambia a:
```yaml
- name: dotnet publish
  run: dotnet publish Rodavia.Web/Rodavia.Web/Rodavia.Web.csproj -c Release -o ${{env.DOTNET_ROOT}}/myapp
```

**Importante**: Solo cambia la línea `run:`, lo demás déjalo igual.

---

## ✅ Paso 6: Configurar Variables de Entorno en Azure

### 6.1 Ir al App Service

```
Azure Portal > [Tu App Service] > Configuration > Application settings
```

### 6.2 Agregar cada variable

Click "New application setting" y agrega:

```
Name: Authentication__Google__ClientId
Value: [TU_GOOGLE_CLIENT_ID]

Name: Authentication__Google__ClientSecret
Value: [TU_GOOGLE_CLIENT_SECRET]

Name: ASPNETCORE_ENVIRONMENT
Value: Production

Name: ConnectionStrings__DefaultConnection
Value: Data Source=rodavia.db
```

### 6.3 Guardar

- Click "Save" (arriba)
- Click "Continue" en el diálogo

---

## ✅ Paso 7: Commit y Push

```powershell
# Ver cambios
git status

# Agregar el workflow modificado
git add .github/workflows/

# Commit
git commit -m "fix: Configurar workflow de Azure para proyecto Blazor"

# Push (esto activará el deploy)
git push origin main
```

---

## ✅ Paso 8: Monitorear el Deploy

### En GitHub:
```
https://github.com/PeyoBv/autoguia_blazor/actions
```

Verás el workflow corriendo. Tiempo: 5-8 minutos.

### En Azure (opcional):
```
App Service > Deployment Center > Logs
```

---

## ✅ Paso 9: Verificar la Aplicación

### 9.1 Obtener la URL

En Azure Portal:
```
App Service > Overview > URL
```

Ejemplo: `https://rodavia-abc123.azurewebsites.net`

### 9.2 Abrir y probar

- Abre la URL
- Espera 1-2 minutos (primera carga)
- Verifica que la app cargue

---

## ✅ Paso 10: Actualizar Google OAuth

### 10.1 Google Cloud Console

```
https://console.cloud.google.com/apis/credentials
```

### 10.2 Editar OAuth Client

Agrega las Redirect URIs (con tu URL real):

```
https://[TU-URL].azurewebsites.net/signin-google
https://[TU-URL].azurewebsites.net/Account/ExternalLoginCallback
```

### 10.3 Probar login

- Ve a tu app
- Click "Iniciar sesión con Google"
- Debe funcionar ✅

---

## 🎉 ¡Listo!

Ahora tienes:
- ✅ App Service en Azure
- ✅ GitHub Actions configurado
- ✅ Deploy automático con cada push
- ✅ Google OAuth funcionando

**Para futuros cambios:**
```powershell
git add .
git commit -m "descripción"
git push origin main
# Deploy automático en 5-8 minutos ✨
```
