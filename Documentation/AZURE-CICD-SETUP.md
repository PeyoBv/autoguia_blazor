# 🚀 Configuración de CI/CD con GitHub Actions y Azure App Service

Este documento explica cómo configurar el despliegue continuo de Rodavia a Azure App Service usando GitHub Actions.

## 📋 Requisitos Previos

1. **Azure App Service creado** con el nombre `rodavia`
2. **Repositorio en GitHub** (PeyoBv/rodavia_blazor)
3. **Publish Profile** de Azure App Service

---

## 🔐 Paso 1: Obtener el Publish Profile de Azure

### Opción A: Desde Azure Portal (Recomendado)

1. Ve a [Azure Portal](https://portal.azure.com)
2. Navega a tu **App Service** llamado `rodavia`
3. En el menú izquierdo, haz clic en **"Get publish profile"** (Obtener perfil de publicación)
4. Se descargará un archivo XML llamado `rodavia.PublishSettings`
5. Abre el archivo con un editor de texto y **copia TODO el contenido**

### Opción B: Desde Azure CLI

```bash
az webapp deployment list-publishing-profiles \
  --name rodavia \
  --resource-group tu-resource-group \
  --xml
```

---

## 🔑 Paso 2: Configurar el Secreto en GitHub

1. Ve a tu repositorio en GitHub: https://github.com/PeyoBv/rodavia_blazor
2. Haz clic en **Settings** (Configuración)
3. En el menú lateral izquierdo, ve a **Secrets and variables** → **Actions**
4. Haz clic en **"New repository secret"**
5. Configura el secreto:
   - **Name**: `AZUREAPPSERVICE_PUBLISHPROFILE`
   - **Secret**: Pega el contenido completo del archivo XML del Publish Profile
6. Haz clic en **"Add secret"**

---

## ✅ Paso 3: Verificar el Workflow

El workflow ya está configurado en `.github/workflows/azure-webapps.yml` y se ejecutará automáticamente cuando:

- **Se haga push a la rama `main`**
- **Se ejecute manualmente** desde la pestaña "Actions" en GitHub

### Estructura del Workflow

```yaml
on:
  push:
    branches:
      - main
  workflow_dispatch: # Ejecución manual
```

### Pasos del Workflow

1. 📥 **Checkout repository**: Descarga el código fuente
2. 🔧 **Setup .NET 8.x**: Instala .NET SDK
3. 📦 **Restore dependencies**: Restaura paquetes NuGet
4. 🏗️ **Build solution**: Compila en modo Release
5. 📤 **Publish application**: Genera los archivos de publicación
6. 🚀 **Deploy to Azure**: Despliega a Azure App Service

---

## 🧪 Paso 4: Probar el Despliegue

### Opción A: Push a la rama main

```bash
git add .
git commit -m "🚀 Configurar CI/CD para Azure"
git push origin main
```

### Opción B: Ejecución Manual

1. Ve a tu repositorio en GitHub
2. Haz clic en la pestaña **"Actions"**
3. Selecciona el workflow **"Build and Deploy to Azure App Service"**
4. Haz clic en **"Run workflow"**
5. Selecciona la rama `main`
6. Haz clic en **"Run workflow"**

---

## 📊 Monitorear el Despliegue

1. Ve a la pestaña **"Actions"** en tu repositorio
2. Verás el workflow en ejecución con un indicador amarillo 🟡
3. Haz clic en el workflow para ver los detalles de cada paso
4. Cuando termine exitosamente, verás un check verde ✅

### Verificar la Aplicación Desplegada

Una vez completado el despliegue, tu aplicación estará disponible en:

**🌐 https://rodavia.azurewebsites.net**

---

## 🔧 Configuración Adicional

### Variables de Entorno en Azure

Si necesitas configurar variables de entorno (como API keys), agrégalas en Azure Portal:

1. Ve a tu App Service `rodavia`
2. En el menú izquierdo, selecciona **Configuration**
3. Agrega las siguientes Application Settings:

```
Authentication__Google__ClientId = TU_GOOGLE_CLIENT_ID
Authentication__Google__ClientSecret = TU_GOOGLE_CLIENT_SECRET
GoogleMaps__ApiKey = AIzaSyDDrpPXn4n09DqNBwtuzVzwuHAsRnlB23M
```

> **Nota**: Azure App Service reemplaza los `:` por `__` (doble guion bajo) en las claves de configuración.

### Habilitar HTTPS Permanente

1. En tu App Service, ve a **Settings** → **Configuration**
2. En **General settings**, activa **HTTPS Only**
3. Guarda los cambios

---

## 🐛 Solución de Problemas

### Error: "Publish profile is not valid"

- Verifica que copiaste **TODO** el contenido del archivo XML
- Asegúrate de que el secreto se llama exactamente `AZUREAPPSERVICE_PUBLISHPROFILE`
- Regenera el Publish Profile desde Azure Portal

### Error: "App Service not found"

- Verifica que el nombre del App Service es exactamente `rodavia`
- Confirma que el App Service existe en tu suscripción de Azure

### Error en la compilación

- Revisa los logs en la pestaña "Actions"
- Verifica que todas las dependencias están en el repositorio
- Asegúrate de que el proyecto compila localmente con `dotnet build`

### Despliegue exitoso pero la app no funciona

1. Revisa los logs del App Service:
   - Azure Portal → App Service → **Log stream**
2. Verifica la configuración de Application Settings
3. Asegúrate de que la conexión a base de datos está configurada

---

## 📚 Recursos Adicionales

- [Documentación de GitHub Actions](https://docs.github.com/en/actions)
- [Azure App Service Documentation](https://docs.microsoft.com/en-us/azure/app-service/)
- [azure/webapps-deploy Action](https://github.com/Azure/webapps-deploy)
- [.NET en Azure App Service](https://docs.microsoft.com/en-us/azure/app-service/quickstart-dotnetcore)

---

## 🎯 Próximos Pasos Recomendados

1. **Configurar entornos separados** (Staging/Production)
2. **Agregar tests automatizados** al workflow
3. **Configurar notificaciones** de despliegue (Slack, Teams, email)
4. **Implementar health checks** para verificar el estado de la app
5. **Configurar Application Insights** para monitoreo

---

## 📝 Checklist de Configuración

- [ ] Azure App Service `rodavia` creado
- [ ] Publish Profile descargado
- [ ] Secreto `AZUREAPPSERVICE_PUBLISHPROFILE` configurado en GitHub
- [ ] Workflow `azure-webapps.yml` en el repositorio
- [ ] Push a la rama `main` realizado
- [ ] Workflow ejecutado exitosamente
- [ ] Aplicación accesible en https://rodavia.azurewebsites.net
- [ ] Variables de entorno configuradas en Azure
- [ ] HTTPS habilitado

---

**¡Tu pipeline de CI/CD está listo!** 🎉

Cada vez que hagas push a `main`, GitHub Actions compilará y desplegará automáticamente tu aplicación a Azure.
