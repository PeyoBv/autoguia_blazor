# 🔐 Guía de Testing - Autenticación OAuth con Google

## ✅ Estado de Implementación

La autenticación OAuth con Google ha sido completamente implementada en Rodavia. Esta guía te ayudará a configurar Google Cloud Console y probar el flujo de login.

---

## 📋 Prerequisitos

1. **Credenciales de Google OAuth** ya configuradas:
   - Client ID: `627642459253-94r5ma9u1u71bcd33ivef3j5uvv4qf7k.apps.googleusercontent.com`
   - Client Secret: `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`
   - Callback Path: `/signin-google`

2. **User Secrets** configurados en el proyecto:
   ```bash
   dotnet user-secrets set "Authentication:Google:ClientId" "627642459253-94r5ma9u1u71bcd33ivef3j5uvv4qf7k.apps.googleusercontent.com"
   dotnet user-secrets set "Authentication:Google:ClientSecret" "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"
   ```

---

## 🌐 Configuración de Google Cloud Console

### Paso 1: Acceder a Google Cloud Console
1. Ir a [Google Cloud Console](https://console.cloud.google.com/)
2. Seleccionar o crear un proyecto
3. Ir a **APIs & Services** > **Credentials**

### Paso 2: Configurar OAuth 2.0 Client ID
1. Seleccionar el Client ID existente o crear uno nuevo
2. En **Authorized JavaScript origins**, agregar:
   ```
   http://localhost:5082
   https://localhost:7082
   ```

3. En **Authorized redirect URIs**, agregar:
   ```
   http://localhost:5082/signin-google
   https://localhost:7082/signin-google
   ```

### Paso 3: Configurar la Pantalla de Consentimiento OAuth
1. Ir a **OAuth consent screen**
2. Seleccionar **External** para testing con cualquier cuenta de Google
3. Completar la información básica:
   - **App name**: Rodavia
   - **User support email**: Tu email
   - **Developer contact**: Tu email
4. Agregar los scopes:
   - `email`
   - `profile`
5. Agregar usuarios de prueba (en modo desarrollo):
   - Agregar tus cuentas de Gmail para testing

### Paso 4: Verificar URIs (IMPORTANTE)

Asegúrate que las URIs de redirección coincidan **exactamente** con las configuradas en tu aplicación:

```
Redirect URI en Google Console:
http://localhost:5082/signin-google

Configuración en Program.cs:
googleOptions.CallbackPath = "/signin-google";
```

---

## 🧪 Proceso de Testing

### 1. Compilar el Proyecto
```bash
dotnet build Rodavia.sln
```

### 2. Ejecutar la Aplicación
```bash
dotnet run --project Rodavia.Web/Rodavia.Web/Rodavia.Web.csproj
```

La aplicación se ejecutará en:
- HTTP: `http://localhost:5082`
- HTTPS: `https://localhost:7082`

### 3. Flujo de Login con Google

#### Escenario 1: Usuario Nuevo
1. Navegar a `http://localhost:5082/Account/Login`
2. Hacer clic en el botón **"Continuar con Google"**
3. Seleccionar una cuenta de Google
4. Autorizar los permisos solicitados (email, profile)
5. **Resultado esperado**:
   - Redirigido a `/Account/ExternalLogin` con email pre-llenado
   - Email del usuario viene de Google
   - Hacer clic en "Completar Registro"
   - Usuario creado en base de datos con:
     - `Email`: Email de Google
     - `UserName`: Email de Google
     - `DisplayName`: Nombre de Google
     - `ProfilePictureUrl`: Foto de perfil de Google (si está disponible)
   - Login externo vinculado a la cuenta
   - Sesión iniciada automáticamente
   - Redirigido a la página principal

#### Escenario 2: Usuario Existente
1. Navegar a `http://localhost:5082/Account/Login`
2. Hacer clic en el botón **"Continuar con Google"**
3. Seleccionar la misma cuenta de Google usada anteriormente
4. **Resultado esperado**:
   - Login automático (sin formulario de registro)
   - Sesión iniciada
   - Redirigido a la página principal

---

## 🔍 Verificación de la Base de Datos

### Verificar Usuario Creado (InMemory Database)

Como Rodavia usa **InMemory Database** para desarrollo, los usuarios no persisten entre reinicios de la aplicación.

Para verificar que el usuario se creó correctamente durante la sesión:

1. Agregar un breakpoint en `ExternalLogin.razor` después de:
   ```csharp
   var result = await UserManager.CreateAsync(user);
   ```

2. Inspeccionar el objeto `user` para confirmar:
   - `Email`: Coincide con Google
   - `UserName`: Coincide con Google
   - `DisplayName`: Nombre de Google
   - `ProfilePictureUrl`: URL de foto (si disponible)

3. Verificar que el login externo se vinculó:
   ```csharp
   result = await UserManager.AddLoginAsync(user, externalLoginInfo);
   ```

### Logs de Debugging

En modo desarrollo, los logs mostrarán:

```
✅ Google login exitoso para: usuario@gmail.com
Usuario {Email} inició sesión con Google
Usuario creado con Google: usuario@gmail.com
```

---

## 🐛 Troubleshooting

### Error: "Error from external provider: access_denied"
**Causa**: Usuario canceló el login de Google o no autorizó los permisos.  
**Solución**: Intentar nuevamente y aceptar todos los permisos.

### Error: "Error loading external login information"
**Causa**: El callback de Google no llegó correctamente.  
**Soluciones**:
1. Verificar que la Redirect URI en Google Console coincida exactamente
2. Verificar que el CallbackPath en `Program.cs` sea `/signin-google`
3. Revisar logs del navegador (F12 > Console)

### Error: "redirect_uri_mismatch"
**Causa**: La URI de redirección no está autorizada en Google Console.  
**Solución**:
1. Ir a Google Cloud Console > Credentials
2. Editar el OAuth 2.0 Client ID
3. Agregar la URI exacta mostrada en el error a "Authorized redirect URIs"

### Usuario no se crea en la base de datos
**Causa**: Error en la creación del usuario.  
**Soluciones**:
1. Revisar logs de la aplicación para ver el error específico
2. Verificar que ApplicationDbContext esté configurado correctamente
3. Verificar que las migraciones estén aplicadas (si usas SQL Server/PostgreSQL)

### El botón de Google no aparece
**Causa**: Los proveedores externos no se están cargando.  
**Soluciones**:
1. Verificar que `Program.cs` tenga `.AddGoogle(...)` configurado
2. Verificar que las credenciales estén en User Secrets
3. Revisar que `Login.razor` cargue los esquemas externos:
   ```csharp
   externalLogins = (await SignInManager.GetExternalAuthenticationSchemesAsync()).ToArray();
   ```

---

## 🔒 Seguridad - Mejores Prácticas

### Desarrollo
- ✅ Credenciales en **User Secrets** (no en código)
- ✅ Callback Path en HTTP y HTTPS
- ✅ Usuarios de prueba agregados en Google Console

### Producción
1. **Variables de Entorno**:
   ```bash
   AUTHENTICATION__GOOGLE__CLIENTID=tu-client-id
   AUTHENTICATION__GOOGLE__CLIENTSECRET=tu-client-secret
   ```

2. **Azure App Service**:
   - Ir a Configuration > Application Settings
   - Agregar las variables de entorno

3. **URIs de Redirección**:
   - Agregar el dominio de producción:
     ```
     https://tudominio.com/signin-google
     ```

4. **Pantalla de Consentimiento**:
   - Cambiar de "Testing" a "In Production"
   - Completar el proceso de verificación de Google (si es necesario)

---

## 📊 Métricas de Éxito

Al finalizar el testing exitoso, deberías ver:

- ✅ Usuario puede hacer clic en "Continuar con Google"
- ✅ Redirigido correctamente a Google OAuth
- ✅ Autorización de permisos funciona
- ✅ Callback de Google regresa correctamente
- ✅ Usuario nuevo se crea en la base de datos
- ✅ Login externo se vincula correctamente
- ✅ Usuario existente hace login automáticamente
- ✅ Sesión se mantiene después del login
- ✅ Información del perfil (nombre, email, foto) se guarda correctamente

---

## 🔗 Referencias Útiles

- [Google OAuth 2.0 Documentation](https://developers.google.com/identity/protocols/oauth2)
- [ASP.NET Core External Authentication](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/)
- [Google Cloud Console](https://console.cloud.google.com/)
- [Obtener credenciales de Google](https://console.cloud.google.com/apis/credentials)

---

## 📞 Soporte

Si encuentras problemas durante el testing:

1. Revisar los logs de la aplicación (consola)
2. Revisar la consola del navegador (F12)
3. Verificar la configuración en Google Cloud Console
4. Consultar esta documentación para soluciones comunes
5. Revisar el código de `ExternalLogin.razor` y `PerformExternalLogin.razor`

---

**Última actualización**: 12 de noviembre de 2025  
**Versión**: 1.0  
**Proyecto**: Rodavia - Plataforma Automotriz
