# 🔐 Configuración de Autenticación Google OAuth2

## ✅ Implementación Completada

Se ha implementado exitosamente la autenticación OAuth2 con Google en el proyecto AutoGuía.

---

## 📋 Pasos para Obtener Credenciales de Google

### 1. Acceder a Google Cloud Console

Navega a: **[https://console.cloud.google.com](https://console.cloud.google.com)**

### 2. Crear o Seleccionar un Proyecto

- Si no tienes un proyecto, crea uno nuevo:
  - Haz clic en el selector de proyectos (parte superior)
  - Clic en **"Proyecto nuevo"**
  - Nombre: `AutoGuia` (o el que prefieras)
  - Clic en **"Crear"**

### 3. Habilitar Google+ API (Opcional pero recomendado)

- En el menú lateral, ve a **"APIs y servicios"** → **"Biblioteca"**
- Busca: `Google+ API`
- Clic en **"Habilitar"**

### 4. Configurar Pantalla de Consentimiento OAuth

- Ve a **"APIs y servicios"** → **"Pantalla de consentimiento de OAuth"**
- Selecciona tipo de usuario:
  - **Interno**: Solo usuarios de tu organización Google Workspace
  - **Externo**: Cualquier usuario con cuenta Google (recomendado para AutoGuía)
- Completa el formulario:
  - **Nombre de la aplicación**: `AutoGuía`
  - **Correo electrónico de asistencia**: Tu email
  - **Logo de la aplicación**: (opcional)
  - **Dominios autorizados**: Tu dominio de producción (ej: `autoguia.cl`)
  - **Correo de contacto del desarrollador**: Tu email
- Clic en **"Guardar y continuar"**

### 5. Crear Credenciales OAuth 2.0

- Ve a **"APIs y servicios"** → **"Credenciales"**
- Clic en **"+ CREAR CREDENCIALES"** → **"ID de cliente de OAuth"**
- Selecciona tipo de aplicación: **"Aplicación web"**
- Configura:
  - **Nombre**: `AutoGuía Web Client`
  - **Orígenes de JavaScript autorizados**:
    ```
    http://localhost:5000
    https://localhost:5001
    https://tudominio.com (tu URL de producción)
    ```
  - **URIs de redireccionamiento autorizados**:
    ```
    http://localhost:5000/signin-google
    https://localhost:5001/signin-google
    https://tudominio.com/signin-google
    ```
- Clic en **"Crear"**

### 6. Obtener Client ID y Client Secret

Una vez creado, verás una ventana emergente con:

```
Client ID: 123456789-abcdefghijklmnop.apps.googleusercontent.com
Client Secret: GOCSPX-AbCdEfGhIjKlMnOpQrStUvWxYz
```

⚠️ **IMPORTANTE**: Copia estos valores inmediatamente.

---

## 🔧 Configuración en el Proyecto

### Opción 1: User Secrets (Desarrollo - **RECOMENDADO**)

Ejecuta en la terminal desde la raíz del proyecto:

```bash
# Ir al proyecto Web
cd AutoGuia.Web\AutoGuia.Web

# Configurar Client ID
dotnet user-secrets set "Authentication:Google:ClientId" "TU_CLIENT_ID.apps.googleusercontent.com"

# Configurar Client Secret
dotnet user-secrets set "Authentication:Google:ClientSecret" "TU_CLIENT_SECRET"
```

### Opción 2: appsettings.json (NO RECOMENDADO para repositorios públicos)

Edita `AutoGuia.Web/AutoGuia.Web/appsettings.json`:

```json
{
  "Authentication": {
    "Google": {
      "ClientId": "123456789-abcdefghijklmnop.apps.googleusercontent.com",
      "ClientSecret": "GOCSPX-AbCdEfGhIjKlMnOpQrStUvWxYz"
    }
  }
}
```

⚠️ **NUNCA subas estos valores a Git**. Agrega esto a `.gitignore` si es necesario.

### Opción 3: Variables de Entorno (Producción)

```bash
# Windows PowerShell
$env:Authentication__Google__ClientId = "TU_CLIENT_ID"
$env:Authentication__Google__ClientSecret = "TU_CLIENT_SECRET"

# Linux/macOS
export Authentication__Google__ClientId="TU_CLIENT_ID"
export Authentication__Google__ClientSecret="TU_CLIENT_SECRET"
```

---

## 🎨 Interfaz de Usuario

El botón **"Google"** se mostrará automáticamente en la página de Login gracias al componente `ExternalLoginPicker.razor`.

No necesitas modificar nada en la UI - el sistema detecta automáticamente los proveedores configurados.

---

## 🧪 Probar la Autenticación

1. Ejecuta la aplicación:
   ```bash
   dotnet run --project AutoGuia.Web/AutoGuia.Web/AutoGuia.Web.csproj
   ```

2. Navega a: **https://localhost:5001/Account/Login**

3. Verás el botón **"Google"** en la sección **"Use another service to log in"**

4. Haz clic en el botón y completa el flujo de autenticación

5. Después de autenticarte con Google:
   - **Si es usuario nuevo**: Se crea automáticamente en la base de datos
   - **Si ya existe**: Inicia sesión directamente
   - **Redirección**: A la página de inicio o a la URL de retorno configurada

---

## 🔒 Seguridad y Mejores Prácticas

### ✅ Implementadas

- **User Secrets**: Credenciales no están en el código fuente
- **HTTPS**: OAuth requiere conexiones seguras
- **CallbackPath**: Ruta específica `/signin-google`
- **Scopes**: Solo `profile` y `email` (información básica)
- **SaveTokens**: Tokens guardados para uso futuro (opcional)

### 🔐 Recomendaciones Adicionales

1. **Nunca commitear** `appsettings.json` con credenciales reales
2. **Usar User Secrets** en desarrollo
3. **Usar Variables de Entorno** en producción
4. **Configurar dominios autorizados** correctamente en Google Cloud
5. **Revisar periódicamente** los usuarios OAuth en Google Cloud Console

---

## 🚀 Flujo de Autenticación

```
Usuario → Clic "Google"
    ↓
Redirección a Google
    ↓
Usuario aprueba permisos
    ↓
Google redirige a /signin-google
    ↓
ASP.NET Identity procesa el callback
    ↓
Usuario autenticado ✅
    ↓
Redirección a página de inicio
```

---

## 🐛 Troubleshooting

### Error: "redirect_uri_mismatch"

**Causa**: La URI de callback no está autorizada en Google Cloud Console.

**Solución**: 
1. Ve a Google Cloud Console → Credenciales
2. Edita tu cliente OAuth
3. Agrega la URI exacta: `https://localhost:5001/signin-google`

### Error: "invalid_client"

**Causa**: Client ID o Client Secret incorrectos.

**Solución**: 
1. Verifica las credenciales en Google Cloud Console
2. Actualiza los valores en User Secrets o appsettings.json
3. Reinicia la aplicación

### El botón de Google no aparece

**Causa**: Credenciales no configuradas o incorrectas.

**Solución**: 
1. Verifica que los valores estén en User Secrets:
   ```bash
   dotnet user-secrets list --project AutoGuia.Web/AutoGuia.Web
   ```
2. Asegúrate que los valores no estén vacíos
3. Reinicia la aplicación

### Error: "access_denied"

**Causa**: Usuario canceló el flujo de autenticación.

**Solución**: Esto es normal, el usuario simplemente no completó el login.

---

## 📚 Referencias

- [Documentación oficial de Google OAuth2](https://developers.google.com/identity/protocols/oauth2)
- [ASP.NET Core Authentication](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/)
- [Google Cloud Console](https://console.cloud.google.com)

---

## ✨ Próximos Pasos Opcionales

1. **Personalizar el botón de Google** con estilos corporativos
2. **Agregar más proveedores** (Microsoft, Facebook, GitHub)
3. **Implementar Claims personalizados** para roles/permisos
4. **Configurar ExternalLoginInfo** para capturar datos adicionales
5. **Implementar página de perfil** que muestre datos de Google

---

**Implementado por**: GitHub Copilot  
**Fecha**: 24 de octubre de 2025  
**Versión**: 1.0
