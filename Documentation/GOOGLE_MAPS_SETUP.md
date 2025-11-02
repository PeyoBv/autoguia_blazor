# Configuración de Google Maps en AutoGuía

## Obtener una clave de API de Google Maps

1. **Ir a Google Cloud Console**
   - Visita: https://console.cloud.google.com/

2. **Crear o seleccionar un proyecto**
   - Crea un nuevo proyecto o selecciona uno existente

3. **Habilitar la API de Google Maps**
   - Navega a "API y servicios" > "Biblioteca"
   - Busca "Maps JavaScript API"
   - Haz clic en "Habilitar"

4. **Crear credenciales**
   - Ve a "API y servicios" > "Credenciales"
   - Haz clic en "Crear credenciales" > "Clave de API"
   - Copia la clave generada

## Configurar la clave en el proyecto

### Para desarrollo local:

1. **Usar el administrador de secretos** (recomendado):
   ```bash
   cd AutoGuia.Web/AutoGuia.Web
   dotnet user-secrets set "GoogleMaps:ApiKey" "TU_CLAVE_REAL_AQUI"
   ```

2. **O modificar appsettings.Development.json**:
   ```json
   {
     "GoogleMaps": {
       "ApiKey": "AIzaSyDDrpPXn4n09DqNBwtuzVzwuHAsRnlB23M"
     }
   }
   ```

### Para producción:

- Configurar la variable de ambiente: `GoogleMaps__ApiKey`
- O usar Azure Key Vault si desplegás en Azure

## Restricciones de seguridad recomendadas

En Google Cloud Console, ve a tu clave de API y configura:

1. **Restricciones de aplicación**
   - Restricciones de HTTP (sitios web)
   - Agregar tu dominio: `https://tudominio.com/*`
   - Para desarrollo local: `http://localhost:*` y `https://localhost:*`

2. **Restricciones de API**
   - Restringir la clave solo a "Maps JavaScript API"

## Verificar la configuración

1. Ejecuta la aplicación:
   ```bash
   dotnet run --project AutoGuia.Web/AutoGuia.Web/AutoGuia.Web.csproj
   ```

2. Ve a la página de Talleres: `https://localhost:7071/talleres`

3. El mapa debería cargar con los marcadores de los talleres

## Funcionalidades del mapa implementadas

✅ **Mapa interactivo** con Google Maps
✅ **Marcadores diferenciados** (verificados vs no verificados)
✅ **InfoWindows** con información detallada
✅ **Filtros dinámicos** que actualizan los marcadores
✅ **Centrado automático** en marcadores
✅ **Botón "Ver en Mapa"** que centra el mapa en el taller
✅ **Botón "Cómo Llegar"** que abre Google Maps para navegación
✅ **Manejo de errores** si la API no está disponible

## Solución de problemas

### Error: "Clave de API no configurada"
- Verifica que configuraste la clave correctamente
- No uses la clave placeholder "YOUR_GOOGLE_MAPS_API_KEY_HERE"

### Error: "RefererNotAllowedMapError"
- Configura las restricciones de dominio en Google Cloud Console

### El mapa no carga
- Verifica que tienes créditos disponibles en Google Cloud
- Comprueba que la API esté habilitada
- Revisa la consola del navegador para más detalles

### Los marcadores no aparecen
- Verifica que los talleres tienen coordenadas válidas
- Comprueba la consola para errores de JavaScript