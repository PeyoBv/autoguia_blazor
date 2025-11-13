# 💳 CREDENCIALES PARA SIMULAR COMPRAS - TRANSBANK

## 🔐 Ambiente SANDBOX (Pruebas)

Estas credenciales te permiten probar el sistema de pagos **SIN DINERO REAL**.

### Configuración del Comercio

```
Código de Comercio: 597055555584
API Key: 579B532A7440BB0C9079DED94D31EA1615BACEB56610332264630D42D0A36B1C
Ambiente: Sandbox
URL Base: https://webpay3gint.transbank.cl
```

---

## 💳 TARJETAS DE PRUEBA

### ✅ Tarjeta APROBADA (Usa esta para compras exitosas)

```
Número de Tarjeta: 4051 8856 0044 6623
CVV: 123
Fecha de Vencimiento: 12/25
Tipo: Visa
Resultado: ✅ APROBADA
```

### ❌ Tarjeta RECHAZADA (Para probar manejo de errores)

```
Número de Tarjeta: 5186 0595 5959 0568
CVV: 123
Fecha de Vencimiento: 12/25
Tipo: Mastercard
Resultado: ❌ RECHAZADA
```

---

## 🔒 Autenticación 3D Secure

Cuando Transbank te pida autenticarte, usa:

```
RUT: 11.111.111-1
Clave Dinámica: 123
```

> **Nota**: Estos datos aparecerán en la pantalla de Webpay durante el proceso.

---

## 📋 FLUJO DE PRUEBA COMPLETO

### Paso 1: Iniciar Pago
1. Navega a la sección de **Suscripciones** o **Medios de Pago**
2. Click en **"Pagar con Webpay"**
3. Serás redirigido a la página de Transbank

### Paso 2: Ingresar Tarjeta
En la pantalla de Webpay:
```
Número de tarjeta: 4051885600446623
CVV: 123
Fecha de expiración: 12/25
```
Click en **"Continuar"**

### Paso 3: Autenticación
En la pantalla de autenticación 3D Secure:
```
RUT: 11.111.111-1
Clave: 123
```
Click en **"Aceptar"**

### Paso 4: Confirmación
- ✅ Verás un mensaje de "Transacción Aprobada"
- Serás redirigido de vuelta a Rodavia
- La suscripción/pago quedará confirmada
- La tarjeta quedará guardada como `****6623`

---

## 🧪 CASOS DE PRUEBA SUGERIDOS

### Test 1: Pago Exitoso
- **Tarjeta**: `4051885600446623`
- **Resultado esperado**: Transacción aprobada, suscripción activa

### Test 2: Pago Rechazado
- **Tarjeta**: `5186059559590568`
- **Resultado esperado**: Error mostrado al usuario, pago no procesado

### Test 3: Guardar Medio de Pago
- **Tarjeta**: `4051885600446623`
- **Resultado esperado**: Tarjeta guardada como `****6623`, disponible para futuros pagos

### Test 4: Cobro Recurrente
1. Guardar tarjeta con Test 3
2. Crear suscripción con renovación automática
3. **Resultado esperado**: Cobros automáticos cada mes

---

## ⚠️ IMPORTANTE

### ✅ Lo que PUEDES hacer:
- Hacer infinitas pruebas sin costo
- Simular pagos aprobados y rechazados
- Probar todo el flujo de suscripciones
- Verificar el sistema de cobros automáticos

### ❌ Lo que NO puedes hacer:
- NO se cobran tarjetas reales
- NO funciona con dinero real
- NO usar en producción (requiere credenciales reales de Transbank)

### 🔒 Seguridad:
- Estas credenciales son **públicas** y solo para sandbox
- Están incluidas en la documentación oficial de Transbank
- NO hay riesgo de seguridad al compartirlas

---

## 📊 Verificar Transacciones

Después de hacer una prueba, puedes verificar en:

### Base de Datos
```sql
-- Ver últimas transacciones
SELECT * FROM "TransbankTransactions" 
ORDER BY "CreatedAt" DESC 
LIMIT 10;

-- Ver medios de pago guardados
SELECT * FROM "PaymentMethods" 
WHERE "IsActive" = true;

-- Ver suscripciones activas
SELECT * FROM "Suscripciones" 
WHERE "Estado" = 'Activa';
```

### Logs de la Aplicación
Los logs mostrarán información detallada de cada transacción.

---

## 🔗 Recursos Adicionales

- **Guía Completa**: `Documentation/TRANSBANK-TESTING-GUIDE.md`
- **Setup de Transbank**: `Documentation/TRANSBANK-SETUP.md`
- **Documentación Oficial**: https://www.transbankdevelopers.cl
- **Portal de Desarrolladores**: https://github.com/TransbankDevelopers

---

## 💡 Tips

1. **Siempre usa la tarjeta aprobada** (`4051885600446623`) para pruebas normales
2. **Usa la tarjeta rechazada** (`5186059559590568`) solo para probar manejo de errores
3. **Anota el código de autorización** que recibes (6 dígitos)
4. **Verifica los logs** después de cada prueba
5. **Prueba el flujo completo** incluyendo webhooks

---

## ❓ Preguntas Frecuentes

### ¿Por qué no funciona mi tarjeta real?
La aplicación está configurada en **modo SANDBOX**. Solo funcionan las tarjetas de prueba listadas arriba.

### ¿Cuándo puedo usar tarjetas reales?
Para usar tarjetas reales, necesitas:
1. Credenciales de producción de Transbank
2. Configurar `Environment: Production` en `appsettings.json`
3. Completar el proceso de certificación con Transbank

### ¿Se pueden hacer muchas pruebas?
**Sí**, puedes hacer **infinitas pruebas** sin ningún costo. Es un ambiente de pruebas.

### ¿Los cobros recurrentes funcionan en sandbox?
**Sí**, el sistema completo de cobros automáticos funciona igual que en producción.

---

**✅ ¡Listo para probar! Usa la tarjeta `4051885600446623` para simular compras exitosas.**
