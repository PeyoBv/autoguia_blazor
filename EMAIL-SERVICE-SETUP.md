# 📧 Servicio de Email - Documentación Completa

## ✅ Implementación Completada

Se ha implementado exitosamente el servicio de envío de emails con MailKit para confirmación de registro y otros propósitos.

---

## 📋 Archivos Creados/Modificados

### ✨ Nuevos Archivos

1. **`Services/IEmailService.cs`** - Interfaz del servicio de email
2. **`Services/EmailService.cs`** - Implementación con MailKit
3. **`Services/IdentityEmailSender.cs`** - Adaptador para ASP.NET Identity

### 📝 Archivos Modificados

1. **`appsettings.json`** - Configuración SMTP
2. **`Program.cs`** - Registro de servicios
3. **`AutoGuia.Web.csproj`** - Paquete MailKit agregado

---

## 🔧 Configuración SMTP

### Opción 1: Gmail (Recomendado para Desarrollo)

#### Paso 1: Generar Contraseña de Aplicación

1. Ve a tu cuenta de Google: https://myaccount.google.com/security
2. Activa la **Verificación en 2 pasos** (si no está activa)
3. Ve a **Contraseñas de aplicaciones**: https://myaccount.google.com/apppasswords
4. Selecciona:
   - **Aplicación**: Correo
   - **Dispositivo**: Otro (nombre personalizado) → Escribe "AutoGuía"
5. Haz clic en **Generar**
6. Copia la contraseña de 16 caracteres (ej: `abcd efgh ijkl mnop`)

#### Paso 2: Configurar User Secrets

```bash
# Ir al proyecto Web
cd AutoGuia.Web\AutoGuia.Web

# Configurar email
dotnet user-secrets set "Smtp:Email" "tu-email@gmail.com"

# Configurar contraseña de aplicación (sin espacios)
dotnet user-secrets set "Smtp:Password" "abcdefghijklmnop"
```

### Opción 2: SendGrid (Recomendado para Producción)

#### Paso 1: Crear Cuenta en SendGrid

1. Regístrate en: https://sendgrid.com/
2. Verifica tu email
3. Ve a **Settings** → **API Keys**
4. Crea una nueva API Key con permisos de **Full Access**
5. Copia la API Key (comienza con `SG.`)

#### Paso 2: Configurar User Secrets

```bash
cd AutoGuia.Web\AutoGuia.Web

# Configurar SendGrid
dotnet user-secrets set "Smtp:Host" "smtp.sendgrid.net"
dotnet user-secrets set "Smtp:Port" "587"
dotnet user-secrets set "Smtp:Email" "apikey"  # Literal "apikey"
dotnet user-secrets set "Smtp:Password" "TU_SENDGRID_API_KEY"
```

### Opción 3: Servidor SMTP Personalizado

```bash
cd AutoGuia.Web\AutoGuia.Web

dotnet user-secrets set "Smtp:Host" "smtp.tuservidor.com"
dotnet user-secrets set "Smtp:Port" "587"
dotnet user-secrets set "Smtp:Email" "noreply@tudominio.com"
dotnet user-secrets set "Smtp:Password" "tu-contraseña-smtp"
```

---

## 🎨 Estructura del Servicio

### IEmailService - Interfaz Principal

```csharp
public interface IEmailService
{
    // Envío de email de bienvenida con plantilla predefinida
    Task<bool> EnviarEmailBienvenidaAsync(
        string destinatario, 
        string nombreUsuario, 
        string? confirmationUrl = null
    );

    // Envío de email genérico con HTML personalizado
    Task<bool> EnviarEmailAsync(
        string destinatario, 
        string asunto, 
        string htmlBody
    );
}
```

### EmailService - Implementación

**Características:**
- ✅ Usa **MailKit** (moderno, seguro, multiplataforma)
- ✅ Conexión **SMTP con TLS**
- ✅ **Logging integrado** para debugging
- ✅ **Manejo de errores** robusto
- ✅ Plantilla HTML **responsive** embebida
- ✅ **Configuración desde appsettings.json**

### IdentityEmailSender - Adaptador ASP.NET Identity

**Métodos implementados:**
- `SendConfirmationLinkAsync()` - Email de confirmación de cuenta
- `SendPasswordResetLinkAsync()` - Email de recuperación de contraseña
- `SendPasswordResetCodeAsync()` - Email con código de verificación

---

## 🚀 Uso del Servicio

### En Componentes Blazor

```csharp
@inject IEmailService EmailService

// Enviar email de bienvenida
var exito = await EmailService.EnviarEmailBienvenidaAsync(
    "usuario@example.com",
    "Juan Pérez",
    "https://autoguia.cl/confirmar?token=abc123"
);

if (exito)
{
    // Email enviado exitosamente
}
```

### En Servicios/Controladores

```csharp
public class MiServicio
{
    private readonly IEmailService _emailService;

    public MiServicio(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task NotificarUsuarioAsync(string email)
    {
        var html = "<h1>Hola!</h1><p>Este es un email personalizado.</p>";
        await _emailService.EnviarEmailAsync(
            email, 
            "Notificación Importante", 
            html
        );
    }
}
```

### Flujo Actual en Register.razor

El componente `Register.razor` ya está integrado automáticamente porque usa `IEmailSender<ApplicationUser>`, que ahora está conectado a nuestro `EmailService`:

```csharp
// En Register.razor - Línea ~90
await EmailSender.SendConfirmationLinkAsync(user, Input.Email, callbackUrl);

// Esto internamente llama a:
// EmailService.EnviarEmailBienvenidaAsync(email, userName, callbackUrl)
```

---

## 📧 Plantillas de Email

### Email de Bienvenida

**Características:**
- 🎨 Diseño responsive (se ve bien en móvil y desktop)
- 🌈 Gradiente morado/azul corporativo
- 🔘 Botón de confirmación destacado
- 📱 Compatible con todos los clientes de email
- ✨ Personalización con nombre del usuario

**Vista previa:**
```
┌─────────────────────────────────────┐
│  🚗 AutoGuía                        │
│  Tu guía automotriz en Chile        │
├─────────────────────────────────────┤
│  ¡Hola, Juan! 👋                    │
│                                     │
│  ¡Bienvenido a AutoGuía!            │
│  Estamos emocionados...             │
│                                     │
│  Ahora puedes disfrutar de:         │
│  • 🔧 Encuentra talleres            │
│  • 💬 Participa en el foro          │
│  • 🛒 Compara precios               │
│  • 🤖 Diagnóstico con IA            │
│                                     │
│  [Confirmar mi cuenta]              │
│                                     │
│  Si tienes alguna pregunta...       │
└─────────────────────────────────────┘
```

### Email de Recuperación de Contraseña

**Características:**
- 🔐 Tema de seguridad (colores rojos)
- ⚠️ Advertencia destacada si no fue solicitado
- ⏰ Indicación de expiración del link
- 🎯 Call-to-action claro

---

## 🧪 Probar el Servicio

### 1. Configurar Credenciales

Sigue los pasos de configuración SMTP (Gmail, SendGrid o personalizado)

### 2. Ejecutar la Aplicación

```bash
dotnet run --project AutoGuia.Web/AutoGuia.Web/AutoGuia.Web.csproj
```

### 3. Registrar un Usuario

1. Navega a: https://localhost:5001/Account/Register
2. Completa el formulario con:
   - **Email**: Tu email real (para recibir el mensaje)
   - **Password**: Contraseña segura
3. Haz clic en **Register**

### 4. Verificar Email

1. Revisa tu bandeja de entrada
2. Busca email de **AutoGuía**
3. Haz clic en **Confirmar mi cuenta**
4. Serás redirigido a la aplicación

---

## 🔒 Seguridad

### ✅ Buenas Prácticas Implementadas

1. **User Secrets**: Credenciales NO en código fuente
2. **TLS/SSL**: Conexión cifrada con servidor SMTP
3. **Logging**: Registro de envíos sin exponer passwords
4. **Try-Catch**: Manejo robusto de errores
5. **Async/Await**: No bloquea el hilo principal
6. **Validación**: Verifica emails antes de enviar

### 🔐 Variables de Entorno para Producción

```bash
# Azure App Service / Docker
Smtp__Host=smtp.sendgrid.net
Smtp__Port=587
Smtp__Email=apikey
Smtp__Password=SG.xxxxxxxxxxxxx
Smtp__FromName=AutoGuía
```

---

## 🐛 Troubleshooting

### Error: "Smtp:Email no está configurado"

**Causa**: Falta configuración de credenciales SMTP.

**Solución**:
```bash
cd AutoGuia.Web\AutoGuia.Web
dotnet user-secrets set "Smtp:Email" "tu-email@gmail.com"
dotnet user-secrets set "Smtp:Password" "tu-contraseña-app"
```

### Error: "Authentication failed"

**Causa**: Contraseña incorrecta o autenticación en 2 pasos no configurada.

**Solución Gmail**:
1. Verifica que la verificación en 2 pasos esté activa
2. Genera una nueva contraseña de aplicación
3. Usa la contraseña sin espacios

### Error: "SMTP connection timeout"

**Causa**: Firewall bloqueando puerto 587 o host incorrecto.

**Solución**:
1. Verifica el host SMTP: `smtp.gmail.com` para Gmail
2. Prueba puerto alternativo: 465 (SSL directo)
3. Verifica firewall corporativo

### Email no llega a la bandeja

**Causa**: Email marcado como spam o configuración DNS incorrecta.

**Solución**:
1. Revisa la carpeta de **Spam/Correo no deseado**
2. Para producción, configura **SPF**, **DKIM** y **DMARC**
3. Usa un servicio profesional como SendGrid

### Error: "Email enviado pero no llega"

**Causa**: Límites de Gmail (500 emails/día para cuentas gratuitas).

**Solución**:
1. Verifica logs de la aplicación
2. Revisa límites de tu proveedor SMTP
3. Considera migrar a SendGrid (100 emails/día gratis)

---

## 📊 Límites y Recomendaciones

### Gmail

- **Límite diario**: 500 emails (cuenta gratuita)
- **Límite por minuto**: ~20 emails
- **Recomendado para**: Desarrollo y testing
- **Costo**: Gratis

### SendGrid

- **Plan gratuito**: 100 emails/día
- **Plan básico**: 40,000 emails/mes por $19.95
- **Recomendado para**: Producción
- **Ventajas**: Analytics, templates, APIs avanzadas

### Amazon SES

- **Plan gratuito**: 62,000 emails/mes (primer año)
- **Precio**: $0.10 por cada 1,000 emails
- **Recomendado para**: Alta escalabilidad
- **Requiere**: Configuración DNS avanzada

---

## 🚀 Próximas Mejoras Opcionales

1. **Templates externos**: Cargar HTML desde archivos
2. **Queue system**: Encolar emails para envío batch
3. **Retry logic**: Reintentar envíos fallidos
4. **Email tracking**: Saber si el usuario abrió el email
5. **Plantillas dinámicas**: Motor de templates (Razor, Liquid)
6. **Attachments**: Enviar archivos adjuntos
7. **Multi-idioma**: Plantillas en español/inglés
8. **Analytics**: Dashboard de emails enviados/abiertos

---

## 📚 Referencias

- [MailKit Documentation](http://www.mimekit.net/docs/html/Introduction.htm)
- [Gmail App Passwords](https://support.google.com/accounts/answer/185833)
- [SendGrid Quick Start](https://docs.sendgrid.com/for-developers/sending-email/v3-csharp-code-example)
- [ASP.NET Core Email Best Practices](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/accconfirm)

---

**✅ Implementado por**: GitHub Copilot  
**📅 Fecha**: 24 de octubre de 2025  
**📦 Versión**: 1.0  
**🔧 Framework**: .NET 8 + Blazor Server  
**📧 Librería**: MailKit 4.14.1
