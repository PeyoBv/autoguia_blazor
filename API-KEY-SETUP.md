# 🔑 Configuración API Key GetAPI.cl

## ✅ API Key Configurada

**API Key:** `keyTestLimited`  
**Estado:** ✅ Configurada en `appsettings.Development.json`  
**Servicio:** GetAPI.cl - Consulta de Patentes Chilenas  
**Plan:** Test Limited (5 consultas gratis por día con rate limit)

---

## 🚗 Patentes de Prueba

### **Patente Válida Confirmada:**
- **`SGXR42`** ✅ (ejemplo oficial de GetAPI.cl)

### **Otras Patentes para Probar:**
Estas son patentes de formato válido chileno que puedes probar:

**Formato Antiguo (4 letras + 2 números):**
- `ABCD12`
- `WXYZ99`
- `HJKL45`

**Formato Nuevo (2 letras + 4 números):**
- `AB1234`
- `XY9876`
- `CD5678`

---

## 🎯 Cómo Probar la Aplicación

### **Paso 1: Abrir Consulta de Vehículos**
URL: http://localhost:5070/consulta-vehiculo

### **Paso 2: Seleccionar Tab "Buscar por Patente"**
Es el tab principal (izquierda)

### **Paso 3: Probar con Patente Confirmada**
1. Ingresa: **`SGXR42`**
2. Click en "Buscar"
3. Deberías ver información completa del vehículo

### **Paso 4: Probar con VIN (sin API Key)**
1. Cambia al tab "Buscar por VIN"
2. Click en cualquier botón de prueba:
   - BMW X5
   - Chevrolet Cruze
   - Ford F-150
   - BMW 330i
   - Mazda 3
3. Verás información del vehículo desde NHTSA (gratuito)

---

## 📡 Endpoints de GetAPI.cl

### **Consulta por Patente:**
```
GET https://chile.getapi.cl/v1/vehicles/plate/{patente}
Header: X-Api-Key: keyTestLimited
```

**Ejemplo CURL:**
```bash
curl --request GET \
  --url https://chile.getapi.cl/v1/vehicles/plate/SGXR42 \
  --header 'X-Api-Key: keyTestLimited' \
  --header 'accept: application/json'
```

### **Parámetros que Retorna:**
Según la documentación de GetAPI.cl:
- Chasis o VIN
- Número de motor
- Color
- Tipo de bancina (combustible)
- Marca
- Modelo
- Versión
- Año
- Y otros datos del vehículo

---

## ⚠️ Limitaciones del Plan Test

**Rate Limit:**
- 5 consultas gratis por día
- Después de 5 consultas, la API retornará error 429 (Too Many Requests)

**Si excedes el límite:**
1. Espera 24 horas para que se resetee
2. O actualiza a un plan de pago en: https://chile.getapi.cl/planes
3. O usa solo búsqueda por VIN (NHTSA - ilimitado y gratuito)

---

## 🔍 Logs Esperados

### **Búsqueda por Patente Exitosa:**
```
🔍 [Composite] Iniciando búsqueda por Patente: SGXR42
📡 [Composite] Consultando GetAPI.cl...
✅ [GetAPI] Patente consultada: Toyota Corolla 2020
✅ [Composite] Éxito con GetAPI.cl: Toyota Corolla 2020
```

### **Búsqueda por Patente con Error (API Key inválida):**
```
🔍 [Composite] Iniciando búsqueda por Patente: SGXR42
📡 [Composite] Consultando GetAPI.cl...
❌ [GetAPI] API Key inválida o expirada (401 Unauthorized)
⚠️ [Composite] GetAPI.cl no disponible
```

### **Búsqueda por VIN Exitosa (siempre funciona):**
```
🔍 [Composite] Iniciando búsqueda por VIN: 1FTFW1ET5BFA44527
📡 [Composite] Consultando NHTSA...
✅ [NHTSA] VIN decodificado: FORD F-150 2011
✅ [Composite] Éxito con NHTSA: FORD F-150 2011
```

---

## 💡 Consejos

1. **Empieza probando VIN primero** (siempre funciona, no requiere API Key)
2. **Luego prueba la patente `SGXR42`** (confirmada por GetAPI.cl)
3. **Monitorea los logs** en la consola de la aplicación para ver el flujo
4. **Si ves 401 Unauthorized**, verifica que `EnableGetApi: true` en appsettings.Development.json

---

## 📞 Contacto GetAPI.cl

Si necesitas más consultas o una API Key de producción:
- **Website:** https://chile.getapi.cl
- **Planes:** https://chile.getapi.cl/planes
- **Documentación:** https://chile.getapi.cl/docs

---

**¡La aplicación está lista para probar!** 🚀  
Abre http://localhost:5070/consulta-vehiculo y prueba con la patente **`SGXR42`**
