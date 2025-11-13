# Guía de Solución de Problemas - Google Maps API

## Resumen de Cambios Aplicados

Se han realizado las siguientes correcciones para resolver problemas con la visualización del mapa de Google:

### ✅ Cambios Realizados

1. **Eliminada clave hardcodeada en App.razor**
   - La API Key ya no está expuesta en el código HTML
   - Ahora se carga dinámicamente desde la configuración de usuario

2. **Mejorado manejo de errores en JavaScript**
   - Validación de API Key antes de cargar el script
   - Mensajes de error más descriptivos
   - Mejor logging en consola del navegador

3. **Mejorado manejo de errores en C#**
   - Validaciones detalladas de la API Key
   - Distinción entre errores de JavaScript y errores .NET
   - Mensajes informativos para el usuario

## Verificación de la Configuración

### 1. Verificar que la API Key está configurada

```powershell
cd Rodavia.Web\Rodavia.Web
dotnet user-secrets list
```

Deberías ver:
```
GoogleMaps:ApiKey = AIzaSyDDrpPXn4n09DqNBwtuzVzwuHAsRnlB23M
```

### 2. Verificar permisos en Google Cloud Console

**IMPORTANTE**: Debes verificar que tu API Key tenga los permisos correctos:

1. Ve a: https://console.cloud.google.com/
2. Selecciona tu proyecto
3. Ve a **API y servicios** > **Credenciales**
4. Encuentra tu API Key: `AIzaSyDDrpPXn4n09DqNBwtuzVzwuHAsRnlB23M`
5. Verifica lo siguiente:

#### APIs que DEBEN estar habilitadas:
- ✅ **Maps JavaScript API** (REQUERIDA)
- ✅ **Places API** (opcional pero recomendada)
- ✅ **Geocoding API** (opcional)

#### Restricciones de la API Key:

**Restricciones de aplicación:**
- Tipo: **Restricciones de HTTP (sitios web)**
- Referentes de sitios web permitidos:
  ```
  http://localhost:*
  https://localhost:*
  https://rodavia.azurewebsites.net/*
  ```

**Restricciones de API:**
- Seleccionar: **Restringir la clave**
- APIs permitidas:
  - Maps JavaScript API
  - Places API
  - Geocoding API

### 3. Verificar que la API está habilitada

1. Ve a **API y servicios** > **Biblioteca**
2. Busca "Maps JavaScript API"
3. Si dice "ADMINISTRAR", está habilitada ✅
4. Si dice "HABILITAR", haz clic para habilitarla

### 4. Verificar cuota y facturación

1. Ve a **API y servicios** > **Panel de control**
2. Verifica que no hayas excedido las cuotas
3. Verifica que la facturación esté habilitada (Google Maps requiere una cuenta de facturación aunque uses el nivel gratuito)

## Cómo Probar el Mapa

### 1. Ejecutar la aplicación

```powershell
cd Rodavia.Web\Rodavia.Web
dotnet run
```

### 2. Abrir la página de talleres

Navega a: `https://localhost:7071/talleres`

### 3. Abrir la consola del navegador

- **Chrome/Edge**: Presiona `F12` o `Ctrl+Shift+J`
- Ve a la pestaña **Console**

### 4. Buscar mensajes de diagnóstico

Deberías ver mensajes como:
```
Intentando inicializar mapa con ID: mapa-talleres
Cargando Google Maps API con clave: AIzaSyDDrp...
Google Maps API cargada exitosamente
Mapa base creado exitosamente
X marcadores agregados exitosamente
```

## Errores Comunes y Soluciones

### ❌ Error: "RefererNotAllowedMapError"

**Causa**: La URL desde la que cargas el mapa no está en la lista de referentes permitidos.

**Solución**:
1. Ve a Google Cloud Console > Credenciales
2. Edita tu API Key
3. En "Restricciones de aplicación", agrega:
   - `http://localhost:*`
   - `https://localhost:*`

### ❌ Error: "ApiNotActivatedMapError"

**Causa**: Maps JavaScript API no está habilitada en tu proyecto.

**Solución**:
1. Ve a Google Cloud Console > Biblioteca
2. Busca "Maps JavaScript API"
3. Haz clic en "HABILITAR"

### ❌ Error: "InvalidKeyMapError"

**Causa**: La API Key es inválida o ha sido regenerada.

**Solución**:
1. Verifica tu API Key en Google Cloud Console
2. Si es incorrecta, actualiza el secreto:
   ```powershell
   dotnet user-secrets set "GoogleMaps:ApiKey" "TU_NUEVA_CLAVE"
   ```

### ❌ Error: "Clave de API de Google Maps no configurada"

**Causa**: La API Key no está en user-secrets o está vacía.

**Solución**:
```powershell
cd Rodavia.Web\Rodavia.Web
dotnet user-secrets set "GoogleMaps:ApiKey" "AIzaSyDDrpPXn4n09DqNBwtuzVzwuHAsRnlB23M"
```

### ❌ Error: "Timeout esperando Google Maps"

**Causa**: El script de Google Maps no se carga por problemas de red o restricciones.

**Solución**:
1. Verifica tu conexión a internet
2. Verifica que no haya bloqueadores de contenido (AdBlock, etc.)
3. Verifica las restricciones de la API Key

### ❌ El mapa se ve gris o vacío

**Causa**: Facturación no habilitada o cuota excedida.

**Solución**:
1. Ve a Google Cloud Console > Facturación
2. Verifica que tienes una cuenta de facturación vinculada
3. Verifica que no hayas excedido tu cuota mensual gratuita ($200 USD)

### ❌ No se ven los marcadores de talleres

**Causa**: Los talleres no tienen coordenadas (latitud/longitud).

**Solución**:
1. Verifica en la consola: "Talleres a mostrar en mapa: X"
2. Si es 0, los datos semilla no se cargaron correctamente
3. Reinicia la aplicación para recargar datos semilla

## Verificación de Logs

### Logs del navegador (JavaScript)

Abre la consola del navegador y busca:
- ✅ "Rodavia Maps - Script seguro cargado correctamente"
- ✅ "Inicializando mapa con API Key: AIzaSyDDrp..."
- ✅ "X marcadores agregados exitosamente"

### Logs de la aplicación (C#)

En la terminal donde ejecutaste `dotnet run`, busca:
- ✅ "Inicializando mapa con API Key: AIzaSyDDrp..."
- ✅ "Talleres a mostrar en mapa: X"
- ✅ "Mapa inicializado exitosamente"

## Configuración para Producción (Azure)

### Opción 1: Variables de entorno en Azure App Service

1. Ve a Azure Portal > Tu App Service
2. Ve a **Configuración** > **Configuración de la aplicación**
3. Agrega una nueva configuración:
   - **Nombre**: `GoogleMaps__ApiKey`
   - **Valor**: `TU_CLAVE_DE_API`

### Opción 2: Azure Key Vault (Recomendado)

```bash
# Crear secreto en Key Vault
az keyvault secret set \
  --vault-name "tu-keyvault" \
  --name "GoogleMapsApiKey" \
  --value "TU_CLAVE_DE_API"
```

Luego configura la referencia en App Service:
```
@Microsoft.KeyVault(SecretUri=https://tu-keyvault.vault.azure.net/secrets/GoogleMapsApiKey/)
```

## Mejoras Futuras Recomendadas

1. **Implementar caché** para los marcadores de talleres
2. **Lazy loading** del script de Google Maps
3. **Optimización de marcadores** para muchos talleres (clustering)
4. **Modo offline** con mapa estático cuando falla la API
5. **Rotación de API Keys** para mayor seguridad

## Recursos Útiles

- [Google Maps JavaScript API Documentation](https://developers.google.com/maps/documentation/javascript)
- [Solución de problemas API Key](https://developers.google.com/maps/documentation/javascript/error-messages)
- [Precios y cuotas de Google Maps](https://developers.google.com/maps/documentation/javascript/usage-and-billing)
- [Google Cloud Console](https://console.cloud.google.com/)

## Soporte

Si sigues teniendo problemas:

1. **Revisa los logs** tanto del navegador como de la aplicación
2. **Verifica la configuración** en Google Cloud Console
3. **Comprueba la facturación** en Google Cloud
4. **Prueba con una nueva API Key** si todo lo demás falla
