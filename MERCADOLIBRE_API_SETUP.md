# 🔧 Configuración de la API de MercadoLibre

## ⚠️ Problema Actual: Error 403 Forbidden

El scraper de MercadoLibre está recibiendo errores **403 Forbidden** porque su API tiene **límites de rate limiting** muy estrictos para peticiones sin autenticación:

- **Sin autenticación**: 10 peticiones/segundo (compartido entre TODOS los usuarios desde la misma IP)
- **Con autenticación**: 2,000 peticiones/día por aplicación

## 📋 Soluciones Disponibles

### Opción 1: ✅ Obtener Access Token (RECOMENDADO)

Esta es la solución oficial y correcta. Te da acceso a 2,000 búsquedas por día.

#### Paso 1: Crear una Aplicación en MercadoLibre

1. Ve a: https://developers.mercadolibre.com/
2. Haz clic en **"Mis aplicaciones"** (esquina superior derecha)
3. Inicia sesión con tu cuenta de MercadoLibre
4. Haz clic en **"Crear nueva aplicación"**
5. Completa el formulario:
   - **Nombre**: AutoGuía Scraper
   - **Short Name**: autoguia
   - **Descripción**: Comparador de precios de repuestos automotrices
   - **Categoría**: Automotriz
   - **URL de Redirect**: `http://localhost:5070/auth/mercadolibre/callback` (para desarrollo local)

6. **IMPORTANTE**: Guarda los siguientes valores:
   - **Client ID** (App ID)
   - **Client Secret** (Secret Key)

#### Paso 2: Obtener el Access Token

**Opción A: Flujo de Autenticación Simple (Sin Usuario)**

Para búsquedas públicas, puedes usar el flujo **Client Credentials**:

```bash
curl -X POST \
  'https://api.mercadolibre.com/oauth/token' \
  -H 'Accept: application/json' \
  -H 'Content-Type: application/x-www-form-urlencoded' \
  -d 'grant_type=client_credentials&client_id=TU_CLIENT_ID&client_secret=TU_CLIENT_SECRET'
```

Respuesta:
```json
{
  "access_token": "APP_USR-123456789-012345-abc123def456...",
  "token_type": "bearer",
  "expires_in": 21600,
  "scope": "offline_access read",
  "user_id": null
}
```

**Opción B: Usar Postman o Thunder Client**

1. Abre Postman/Thunder Client
2. Crea una petición POST a: `https://api.mercadolibre.com/oauth/token`
3. En Body (x-www-form-urlencoded), agrega:
   - `grant_type`: `client_credentials`
   - `client_id`: TU_CLIENT_ID
   - `client_secret`: TU_CLIENT_SECRET
4. Envía la petición
5. Copia el `access_token` de la respuesta

#### Paso 3: Configurar el Token en AutoGuía

Abre `appsettings.json` y agrega tu token:

```json
{
  "ScrapingSettings": {
    "MercadoLibre": {
      "MaxResults": 10,
      "TimeoutSeconds": 30,
      "AccessToken": "APP_USR-123456789-012345-abc123def456..."
    }
  }
}
```

#### Paso 4: El Código Ya Está Preparado

El scraper `MercadoLibreScraperService.cs` ya tiene código comentado listo para usar el token:

```csharp
// Líneas 136-142 en MercadoLibreScraperService.cs
var accessToken = _configuration["ScrapingSettings:MercadoLibre:AccessToken"];
if (!string.IsNullOrEmpty(accessToken))
{
    httpClient.DefaultRequestHeaders.Authorization = 
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
}
```

**Descomenta este bloque** después de agregar el token en `appsettings.json`.

---

### Opción 2: ⚠️ Implementar Cache y Retry Logic

Si no quieres usar autenticación, puedes:

1. **Agregar cache** para evitar peticiones repetidas
2. **Implementar retry con exponential backoff**
3. **Limitar las búsquedas** a 1 cada 2-3 segundos

Esta opción es **menos confiable** y puede seguir dando errores 403 en horas pico.

---

### Opción 3: 🔄 Usar Web Scraping HTML en lugar de API

**NO RECOMENDADO**: MercadoLibre detecta y bloquea scrapers muy agresivamente. Necesitarías:
- Proxies rotativos
- User-Agent aleatorio
- JavaScript rendering (Puppeteer/Playwright)
- Captcha solving

Es mucho más complejo y viola sus términos de servicio.

---

## 🎯 Recomendación Final

**Usa la Opción 1 (Access Token)**:
- ✅ Es gratis (2,000 búsquedas/día es más que suficiente)
- ✅ Es oficial y permitido
- ✅ Es confiable
- ✅ Solo toma 5 minutos configurarlo
- ✅ El código ya está preparado

---

## 📚 Recursos Adicionales

- **Documentación oficial**: https://developers.mercadolibre.com/es_ar/autenticacion-y-autorizacion
- **API de búsqueda**: https://developers.mercadolibre.com/es_ar/items-y-busquedas
- **Límites de Rate**: https://developers.mercadolibre.com/es_ar/api-docs-es
- **SDKs oficiales**: https://github.com/mercadolibre (tienen SDK para .NET)

---

## ❓ Solución de Problemas

### Error: "invalid_grant"
- Verifica que tu `client_id` y `client_secret` sean correctos
- Asegúrate de usar `grant_type=client_credentials`

### Error: "Token expirado"
- Los tokens expiran en 6 horas (`expires_in: 21600`)
- Implementa lógica para renovar el token automáticamente
- O genera un nuevo token manualmente cuando sea necesario

### Error: Sigue dando 403 con token
- Verifica que el header sea: `Authorization: Bearer TU_TOKEN`
- Confirma que el token no tenga espacios extra al copiarlo
- Revisa los logs para ver si el token se está enviando correctamente

---

## 🚀 Próximos Pasos

1. **Ahora**: Obtén tu Access Token siguiendo los pasos arriba
2. **Después**: Considera implementar auto-renovación del token
3. **Futuro**: Migrar a usar el SDK oficial de MercadoLibre para .NET
