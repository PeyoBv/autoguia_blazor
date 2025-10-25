# 🤖 Configuración de Gemini API - AutoGuía

## ✅ Instalación Completada

Se ha instalado y configurado exitosamente **Gemini API** en el proyecto AutoGuía.

### 📦 Paquete Instalado

```
Mscc.GenerativeAI v2.8.19
```

Este paquete proporciona acceso a los modelos de IA generativa de Google, incluyendo Gemini.

---

## 🔑 Obtener tu API Key

### Paso 1: Accede a Google AI Studio
Visita: **https://aistudio.google.com**

### Paso 2: Inicia Sesión
Usa tu cuenta de Google para acceder a la plataforma.

### Paso 3: Crear API Key
1. En el menú lateral izquierdo, selecciona **"Get API Key"**
2. Haz clic en **"Create API Key"**
3. Selecciona un proyecto de Google Cloud (o crea uno nuevo)
4. Copia la API Key generada

### Paso 4: Configurar en AutoGuía
Abre los archivos de configuración y reemplaza `TU_API_KEY_AQUI` con tu API Key:

#### `appsettings.json`
```json
{
  "GeminiApi": {
    "ApiKey": "AIzaSy...", // 👈 Pega tu API Key aquí
    "Model": "gemini-1.5-flash"
  }
}
```

#### `appsettings.Development.json`
```json
{
  "GeminiApi": {
    "ApiKey": "AIzaSy...", // 👈 Pega tu API Key aquí
    "Model": "gemini-1.5-flash"
  }
}
```

---

## 🎯 Modelo Configurado

### Gemini Pro
- **Modelo**: `gemini-pro`
- **Costo**: ✅ **GRATUITO** (con límites de tasa)
- **Características**:
  - Respuestas precisas y detalladas
  - Procesamiento de texto avanzado
  - Análisis de contexto profundo
  - Ideal para diagnósticos automotrices complejos
  - Modelo estable y confiable

### Límites del Nivel Gratuito
- **15 solicitudes por minuto (RPM)**
- **1 millón de tokens por minuto (TPM)**
- **1,500 solicitudes por día (RPD)**

Más que suficiente para una aplicación de diagnóstico automotriz.

---

## 🔒 Seguridad

### ⚠️ IMPORTANTE: NO subas la API Key al repositorio

#### Opción 1: Usar Secrets de Usuario (Recomendado para desarrollo)
```bash
dotnet user-secrets init --project AutoGuia.Web/AutoGuia.Web
dotnet user-secrets set "GeminiApi:ApiKey" "TU_API_KEY_AQUI" --project AutoGuia.Web/AutoGuia.Web
```

#### Opción 2: Variables de Entorno
```bash
# PowerShell
$env:GeminiApi__ApiKey = "TU_API_KEY_AQUI"

# Linux/Mac
export GeminiApi__ApiKey="TU_API_KEY_AQUI"
```

#### Opción 3: Azure Key Vault (Producción)
Para entornos de producción, almacena la API Key en Azure Key Vault.

---

## 📝 Archivos Modificados

### 1. AutoGuia.Infrastructure.csproj
```xml
<PackageReference Include="Mscc.GenerativeAI" Version="2.8.19" />
```

### 2. appsettings.json
```json
{
  "GeminiApi": {
    "ApiKey": "TU_API_KEY_AQUI",
    "Model": "gemini-1.5-flash"
  }
}
```

### 3. appsettings.Development.json
```json
{
  "GeminiApi": {
    "ApiKey": "TU_API_KEY_AQUI",
    "Model": "gemini-1.5-flash"
  }
}
```

---

## 🧪 Próximos Pasos

1. ✅ **Completado**: Instalación del paquete
2. ✅ **Completado**: Configuración en appsettings
3. ⏭️ **Siguiente**: Obtener API Key de Google AI Studio
4. ⏭️ **Siguiente**: Crear servicio de diagnóstico con Gemini
5. ⏭️ **Siguiente**: Implementar interfaz de usuario para diagnóstico

---

## 🔗 Referencias

- **Google AI Studio**: https://aistudio.google.com
- **Documentación Gemini**: https://ai.google.dev/docs
- **Paquete NuGet**: https://www.nuget.org/packages/Mscc.GenerativeAI
- **Documentación del paquete**: https://github.com/mscraftsman/generative-ai

---

## 🎨 Contexto AutoGuía

Esta configuración reemplaza cualquier sistema de diagnóstico previo y prepara el proyecto para:
- Diagnóstico inteligente de problemas automotrices
- Recomendaciones personalizadas de mantenimiento
- Análisis de síntomas reportados por usuarios
- Sugerencias de talleres especializados

La API de Gemini se integrará en la capa de **Infrastructure** y será consumida desde componentes Blazor en la capa **Web**.
