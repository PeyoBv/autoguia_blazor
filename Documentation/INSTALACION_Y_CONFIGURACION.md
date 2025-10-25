# Instalación y Configuración - AutoGuía

## Índice
- [Requisitos Previos](#requisitos-previos)
- [Instalación Local](#instalación-local)
- [Configuración de Variables de Entorno](#configuración-de-variables-de-entorno)
- [Configuración de Base de Datos](#configuración-de-base-de-datos)
- [Configuración de Autenticación](#configuración-de-autenticación)
- [Configuración de Email](#configuración-de-email)
- [Configuración de APIs Externas](#configuración-de-apis-externas)
- [User Secrets (Desarrollo)](#user-secrets-desarrollo)
- [Verificación de Instalación](#verificación-de-instalación)
- [Troubleshooting](#troubleshooting)

## Requisitos Previos

### Software Requerido

| Software | Versión Mínima | Propósito |
|----------|----------------|-----------|
| **.NET SDK** | 8.0+ | Framework principal |
| **Visual Studio 2022** | 17.8+ | IDE (opcional) |
| **VS Code** | Latest | Editor alternativo |
| **Git** | 2.40+ | Control de versiones |
| **SQL Server** | 2019+ | Base de datos (producción) |
| **Node.js** | 18+ | Herramientas frontend (opcional) |

### Verificar Instalaciones

```powershell
# Verificar .NET SDK
dotnet --version
# Salida esperada: 8.0.x o superior

# Verificar Git
git --version
# Salida esperada: git version 2.x.x

# Verificar SQL Server (si está instalado localmente)
sqlcmd -S localhost -E -Q "SELECT @@VERSION"
```

## Instalación Local

### Paso 1: Clonar el Repositorio

```bash
# Clonar desde GitHub
git clone https://github.com/PeyoBv/autoguia_blazor.git

# Navegar al directorio del proyecto
cd autoguia_blazor
```

### Paso 2: Restaurar Dependencias

```powershell
# Restaurar todos los paquetes NuGet
dotnet restore AutoGuia.sln

# Verificar que no hay errores
dotnet build AutoGuia.sln
```

**Salida esperada:**
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

### Paso 3: Configurar Estructura de Proyectos

El proyecto tiene la siguiente estructura modular:

```
AutoGuia/
├── AutoGuia.Core/              # Entidades y DTOs
├── AutoGuia.Infrastructure/    # Datos y servicios
├── AutoGuia.Web/              
│   ├── AutoGuia.Web/          # Aplicación Blazor (Server)
│   └── AutoGuia.Web.Client/   # Cliente WebAssembly
├── AutoGuia.Tests/            # Pruebas unitarias
└── AutoGuia.sln               # Solución principal
```

## Configuración de Variables de Entorno

### appsettings.json (Desarrollo)

Archivo: `AutoGuia.Web/AutoGuia.Web/appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  },
  "AllowedHosts": "*",
  
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AutoGuiaDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  
  "Authentication": {
    "Google": {
      "ClientId": "TU_GOOGLE_CLIENT_ID",
      "ClientSecret": "TU_GOOGLE_CLIENT_SECRET"
    }
  },
  
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderName": "AutoGuía",
    "SenderEmail": "noreply@autoguia.cl",
    "Username": "tu-email@gmail.com",
    "Password": "tu-app-password"
  },
  
  "ExternalAPIs": {
    "MercadoLibre": {
      "BaseUrl": "https://api.mercadolibre.com",
      "AppId": "TU_ML_APP_ID"
    },
    "AutoPlanet": {
      "BaseUrl": "https://www.autoplanet.cl",
      "ApiKey": "TU_AUTOPLANET_KEY"
    }
  }
}
```

### appsettings.Development.json

Archivo específico para desarrollo (ya incluido):

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  },
  
  "DetailedErrors": true,
  "UseInMemoryDatabase": false
}
```

### appsettings.Production.json

Ver sección [Crear appsettings.Production.json](#crear-appsettingsproductionjson) más adelante.

## Configuración de Base de Datos

### Opción 1: InMemory Database (Rápido para MVP)

Por defecto, el proyecto usa InMemory Database para desarrollo rápido.

**Configuración en Program.cs:**

```csharp
// Ya configurado por defecto
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("AutoGuiaDb"));
```

**Ventajas:**
- ✅ No requiere instalación de SQL Server
- ✅ Datos se recrean en cada ejecución
- ✅ Ideal para desarrollo y pruebas

**Desventajas:**
- ❌ Datos se pierden al cerrar la aplicación
- ❌ No es para producción

### Opción 2: SQL Server LocalDB (Recomendado para Desarrollo)

**Configuración:**

1. Actualizar `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AutoGuiaDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

2. Modificar `Program.cs`:

```csharp
// Cambiar de UseInMemoryDatabase a UseSqlServer
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
```

3. Crear migraciones:

```powershell
# Crear migración inicial
dotnet ef migrations add InitialCreate --project AutoGuia.Web/AutoGuia.Web

# Aplicar migración a la base de datos
dotnet ef database update --project AutoGuia.Web/AutoGuia.Web
```

**Verificar que la base de datos se creó:**

```powershell
sqlcmd -S "(localdb)\mssqllocaldb" -d AutoGuiaDb -Q "SELECT name FROM sys.tables"
```

### Opción 3: SQL Server en Azure (Producción)

Ver archivo [DEPLOYMENT_AZURE.md](./DEPLOYMENT_AZURE.md) para configuración completa.

## Configuración de Autenticación

### Google OAuth 2.0

**Paso 1: Crear Proyecto en Google Cloud Console**

1. Ve a [Google Cloud Console](https://console.cloud.google.com/)
2. Crea un nuevo proyecto: "AutoGuia"
3. Habilita **Google+ API**

**Paso 2: Configurar Pantalla de Consentimiento**

1. APIs & Services → OAuth consent screen
2. Tipo: **External**
3. Información de la aplicación:
   - Nombre: **AutoGuía**
   - Email de soporte: tu-email@gmail.com
   - Logo: (opcional)
4. Scopes: `email`, `profile`, `openid`
5. Test users: Agrega tu email para testing

**Paso 3: Crear Credenciales OAuth 2.0**

1. APIs & Services → Credentials → Create Credentials → OAuth 2.0 Client ID
2. Tipo: **Web application**
3. Nombre: **AutoGuia Web**
4. Authorized redirect URIs:
   ```
   https://localhost:7001/signin-google
   https://tu-app.azurewebsites.net/signin-google
   ```
5. Guardar y copiar:
   - **Client ID**: `123456789-abcdefg.apps.googleusercontent.com`
   - **Client Secret**: `GOCSPX-xxxxxxxxxxxxxxx`

**Paso 4: Configurar en AutoGuía**

**Opción A: User Secrets (Recomendado para desarrollo)**

```powershell
cd AutoGuia.Web/AutoGuia.Web

# Inicializar User Secrets
dotnet user-secrets init

# Agregar Google Client ID
dotnet user-secrets set "Authentication:Google:ClientId" "TU_GOOGLE_CLIENT_ID"

# Agregar Google Client Secret
dotnet user-secrets set "Authentication:Google:ClientSecret" "TU_GOOGLE_CLIENT_SECRET"

# Verificar
dotnet user-secrets list
```

**Opción B: appsettings.json (Solo para testing local)**

⚠️ **NUNCA COMMITEAR SECRETS EN GIT**

```json
{
  "Authentication": {
    "Google": {
      "ClientId": "123456789-abcdefg.apps.googleusercontent.com",
      "ClientSecret": "GOCSPX-xxxxxxxxxxxxxxx"
    }
  }
}
```

**Paso 5: Verificar Configuración en Program.cs**

```csharp
// Ya configurado en el proyecto
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
        options.CallbackPath = "/signin-google";
        
        options.Scope.Add("email");
        options.Scope.Add("profile");
        
        options.SaveTokens = true;
    });
```

## Configuración de Email

### Gmail SMTP (Recomendado para Desarrollo)

**Paso 1: Crear App Password en Gmail**

1. Ve a tu [Cuenta de Google](https://myaccount.google.com/)
2. Seguridad → Verificación en 2 pasos (activar si no está)
3. Contraseñas de aplicaciones → Crear nueva
4. Seleccionar: **Correo** y **Otro (nombre personalizado)**
5. Nombre: "AutoGuia SMTP"
6. Copiar contraseña generada (16 caracteres): `abcd efgh ijkl mnop`

**Paso 2: Configurar User Secrets**

```powershell
cd AutoGuia.Web/AutoGuia.Web

# Configurar SMTP
dotnet user-secrets set "EmailSettings:SmtpServer" "smtp.gmail.com"
dotnet user-secrets set "EmailSettings:SmtpPort" "587"
dotnet user-secrets set "EmailSettings:SenderEmail" "tu-email@gmail.com"
dotnet user-secrets set "EmailSettings:SenderName" "AutoGuía"
dotnet user-secrets set "EmailSettings:Username" "tu-email@gmail.com"
dotnet user-secrets set "EmailSettings:Password" "abcd efgh ijkl mnop"

# Verificar
dotnet user-secrets list
```

**Paso 3: Verificar Servicio de Email**

El servicio `EmailService` ya está implementado. Verifica que esté registrado en `Program.cs`:

```csharp
builder.Services.AddScoped<IEmailService, EmailService>();
```

### Alternativa: SendGrid (Producción)

Para producción, se recomienda usar SendGrid:

```json
{
  "EmailSettings": {
    "SendGridApiKey": "SG.xxxxxxxxxxxxxxxxxxxxxxx",
    "SenderEmail": "noreply@autoguia.cl",
    "SenderName": "AutoGuía"
  }
}
```

## Configuración de APIs Externas

### Mercado Libre API

**Paso 1: Registrar Aplicación**

1. Ve a [Mercado Libre Developers](https://developers.mercadolibre.cl/)
2. Crear aplicación → AutoGuia
3. Redirect URI: `https://localhost:7001/ml-callback`
4. Copiar **App ID**: `1234567890123456`

**Paso 2: Configurar en AutoGuía**

```powershell
dotnet user-secrets set "ExternalAPIs:MercadoLibre:AppId" "1234567890123456"
dotnet user-secrets set "ExternalAPIs:MercadoLibre:BaseUrl" "https://api.mercadolibre.com"
```

### AutoPlanet Scraper

**Configuración:**

```powershell
dotnet user-secrets set "ExternalAPIs:AutoPlanet:BaseUrl" "https://www.autoplanet.cl"
dotnet user-secrets set "ExternalAPIs:AutoPlanet:ApiKey" "tu-api-key-si-la-tienes"
```

## User Secrets (Desarrollo)

### ¿Qué son User Secrets?

Los **User Secrets** permiten almacenar datos sensibles fuera del código fuente, evitando commits accidentales a Git.

### Estructura de Secrets

Archivo: `%APPDATA%\Microsoft\UserSecrets\[user-secrets-id]\secrets.json`

```json
{
  "Authentication:Google:ClientId": "123456789-abcdefg.apps.googleusercontent.com",
  "Authentication:Google:ClientSecret": "GOCSPX-xxxxxxxxxxxxxxx",
  "EmailSettings:SmtpServer": "smtp.gmail.com",
  "EmailSettings:SmtpPort": "587",
  "EmailSettings:Username": "tu-email@gmail.com",
  "EmailSettings:Password": "abcd efgh ijkl mnop",
  "ExternalAPIs:MercadoLibre:AppId": "1234567890123456"
}
```

### Comandos Útiles

```powershell
# Ver todos los secrets
dotnet user-secrets list

# Eliminar un secret
dotnet user-secrets remove "Authentication:Google:ClientId"

# Limpiar todos los secrets
dotnet user-secrets clear

# Ver ubicación del archivo
dotnet user-secrets list --project AutoGuia.Web/AutoGuia.Web
```

### Prioridad de Configuración

.NET lee configuración en este orden (último gana):

1. `appsettings.json`
2. `appsettings.{Environment}.json`
3. **User Secrets** (solo Development)
4. Variables de entorno
5. Argumentos de línea de comandos

## Verificación de Instalación

### Checklist Completo

#### 1. Compilación Exitosa

```powershell
dotnet build AutoGuia.sln
```

✅ **Esperado:** `Build succeeded. 0 Error(s)`

#### 2. Ejecutar Aplicación

```powershell
cd AutoGuia.Web/AutoGuia.Web
dotnet run
```

✅ **Esperado:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7001
      Now listening on: http://localhost:5000
```

#### 3. Verificar Navegación

Abre en navegador: `https://localhost:7001`

✅ **Esperado:**
- Página de inicio carga correctamente
- Menú de navegación visible
- Sin errores en consola del navegador

#### 4. Verificar Autenticación Google

1. Clic en "Iniciar Sesión"
2. Clic en "Iniciar con Google"

✅ **Esperado:**
- Redirecciona a pantalla de Google
- Permite seleccionar cuenta
- Vuelve a AutoGuía autenticado

#### 5. Verificar Base de Datos

```powershell
# Si usas InMemory: Verifica que no haya errores en consola

# Si usas SQL Server:
sqlcmd -S "(localdb)\mssqllocaldb" -d AutoGuiaDb -Q "SELECT COUNT(*) FROM Planes"
```

✅ **Esperado:** `3` (planes seeded)

#### 6. Verificar Email

1. Registrar nuevo usuario con email real
2. Revisar bandeja de entrada

✅ **Esperado:** Email de confirmación recibido

### Script de Verificación Automatizado

Crea archivo `verify-setup.ps1`:

```powershell
Write-Host "🔍 Verificando instalación de AutoGuía..." -ForegroundColor Cyan

# Verificar .NET SDK
Write-Host "`n1. Verificando .NET SDK..." -ForegroundColor Yellow
$dotnetVersion = dotnet --version
if ($dotnetVersion -like "8.*") {
    Write-Host "   ✅ .NET $dotnetVersion instalado" -ForegroundColor Green
} else {
    Write-Host "   ❌ Se requiere .NET 8.0+" -ForegroundColor Red
    exit 1
}

# Verificar compilación
Write-Host "`n2. Compilando solución..." -ForegroundColor Yellow
$buildResult = dotnet build AutoGuia.sln --nologo --verbosity quiet
if ($LASTEXITCODE -eq 0) {
    Write-Host "   ✅ Compilación exitosa" -ForegroundColor Green
} else {
    Write-Host "   ❌ Error en compilación" -ForegroundColor Red
    exit 1
}

# Verificar User Secrets
Write-Host "`n3. Verificando User Secrets..." -ForegroundColor Yellow
$secrets = dotnet user-secrets list --project AutoGuia.Web/AutoGuia.Web
if ($secrets -like "*Google:ClientId*") {
    Write-Host "   ✅ Google OAuth configurado" -ForegroundColor Green
} else {
    Write-Host "   ⚠️  Google OAuth no configurado" -ForegroundColor Yellow
}

Write-Host "`n✅ Verificación completada!" -ForegroundColor Green
Write-Host "Ejecuta 'dotnet run --project AutoGuia.Web/AutoGuia.Web' para iniciar" -ForegroundColor Cyan
```

Ejecutar:

```powershell
.\verify-setup.ps1
```

## Troubleshooting

### Error: "Unable to resolve service for type ApplicationDbContext"

**Causa:** DbContext no registrado correctamente

**Solución:**

```csharp
// Verificar en Program.cs
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("AutoGuiaDb"));
```

### Error: "Invalid redirect_uri" (Google OAuth)

**Causa:** URI de redirección no coincide con la configurada en Google Cloud Console

**Solución:**

1. Ir a Google Cloud Console → Credentials
2. Editar OAuth 2.0 Client
3. Agregar URI exacta:
   ```
   https://localhost:7001/signin-google
   ```
4. **Importante:** Debe ser HTTPS en producción

### Error: "Authentication failed" (SMTP)

**Causa:** Contraseña incorrecta o 2FA no habilitado

**Solución:**

1. Verificar que Gmail tiene 2FA activado
2. Generar nueva **App Password** (16 caracteres)
3. Actualizar User Secrets:
   ```powershell
   dotnet user-secrets set "EmailSettings:Password" "abcd efgh ijkl mnop"
   ```

### Error: "Port 7001 already in use"

**Causa:** Otra instancia de la aplicación ejecutándose

**Solución:**

```powershell
# Ver procesos usando el puerto
netstat -ano | findstr :7001

# Matar proceso (reemplazar PID)
taskkill /PID [PID] /F

# O cambiar puerto en launchSettings.json
```

### Error: "Unable to apply migrations"

**Causa:** Base de datos SQL Server no accesible

**Solución:**

```powershell
# Verificar que SQL Server está ejecutándose
sqllocaldb info mssqllocaldb

# Iniciar si está detenido
sqllocaldb start mssqllocaldb

# Recrear base de datos
dotnet ef database drop --project AutoGuia.Web/AutoGuia.Web
dotnet ef database update --project AutoGuia.Web/AutoGuia.Web
```

### Error: "User Secrets not found"

**Causa:** User Secrets no inicializado

**Solución:**

```powershell
cd AutoGuia.Web/AutoGuia.Web
dotnet user-secrets init
dotnet user-secrets set "Authentication:Google:ClientId" "TU_CLIENT_ID"
```

---

## Próximos Pasos

Una vez completada la instalación:

1. ✅ **Leer documentación:**
   - [AUTENTICACION.md](./AUTENTICACION.md)
   - [SUSCRIPCIONES.md](./SUSCRIPCIONES.md)
   
2. ✅ **Configurar deployment:**
   - [DEPLOYMENT_AZURE.md](./DEPLOYMENT_AZURE.md)
   
3. ✅ **Explorar la aplicación:**
   - Registrar usuario
   - Probar diagnóstico con IA
   - Buscar vehículos
   - Participar en foro

---

**Última actualización:** Octubre 2024  
**Versión:** 1.0  
**Soporte:** GitHub Issues
