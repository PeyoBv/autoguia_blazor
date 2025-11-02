# 💳 Integración Transbank Webpay OneClick - Setup y Testing

## 📋 Descripción General

Este documento detalla la configuración y uso del sistema de pagos con **Transbank Webpay OneClick** integrado en AutoGuía para facturación de suscripciones recurrentes.

## 🏗️ Arquitectura Implementada

### Componentes Principales

```
AutoGuia.Core/
├── Entities/
│   ├── PaymentMethod.cs           # Tarjetas inscritas (tokens TBK)
│   ├── TransbankTransaction.cs    # Registro de transacciones
│   └── PaymentLog.cs              # Logs de eventos de pago
└── DTOs/
    └── TransbankDtos.cs           # DTOs para requests/responses

AutoGuia.Infrastructure/
└── Services/
    └── Payments/
        ├── ITransbankGateway.cs            # Interfaz del gateway
        ├── TransbankGateway.cs             # Implementación del gateway
        ├── ISubscriptionBillingService.cs   # Interfaz de facturación
        └── SubscriptionBillingService.cs    # Lógica de cobros

AutoGuia.Web/
├── Controllers/
│   └── PaymentsController.cs      # Endpoints API REST
└── Services/
    └── SubscriptionBillingBackgroundService.cs  # Job automático diario
```

### Flujo de Funcionamiento

#### 1. Inscripción de Tarjeta (OneClick)
```
Usuario → POST /api/payments/inscripcion/iniciar
       → Transbank devuelve URL y Token
       → Usuario es redirigido a Webpay
       → Usuario ingresa datos de tarjeta
       → Transbank redirect a ReturnUrl con Token
       → POST /api/payments/inscripcion/confirmar?token=XXX
       → Se almacena TBK Token para cobros futuros
```

#### 2. Cobro Inicial de Suscripción
```
Usuario activa Plan Premium
       → POST /api/payments/suscripciones/{id}/cobrar-inicial
       → Se usa token TBK inscrito
       → Transbank procesa cobro
       → Suscripción se activa si es exitoso
```

#### 3. Cobros Recurrentes Automáticos
```
Background Service ejecuta diariamente a las 2 AM
       → Detecta suscripciones por vencer
       → Cobra automáticamente con token TBK
       → Renueva suscripción si es exitoso
       → Suspende suscripción si falla
```

## 🔧 Configuración

### 1. Credenciales de Transbank

#### Sandbox (Desarrollo)
Las credenciales de prueba ya están configuradas en `appsettings.json`:

```json
{
  "Transbank": {
    "CommerceCode": "597055555584",
    "ApiKey": "579B532A7440BB0C9079DED94D31EA1615BACEB56610332264630D42D0A36B1C",
    "Environment": "Sandbox"
  }
}
```

#### Producción
1. Registrarse en [Transbank Developers](https://www.transbankdevelopers.cl)
2. Solicitar credenciales de producción
3. Actualizar `appsettings.Production.json`:

```json
{
  "Transbank": {
    "CommerceCode": "TU_COMMERCE_CODE_REAL",
    "ApiKey": "TU_API_KEY_REAL",
    "Environment": "Production"
  }
}
```

### 2. Base de Datos

Ejecutar migración para crear tablas de pagos:

```powershell
cd AutoGuia.Web\AutoGuia.Web
dotnet ef migrations add AddTransbankPayments
dotnet ef database update
```

Tablas creadas:
- `PaymentMethods` - Medios de pago (tarjetas inscritas)
- `TransbankTransactions` - Transacciones
- `PaymentLogs` - Logs de eventos

### 3. Configurar Webhooks con ngrok

Los webhooks de Transbank requieren una URL pública HTTPS. En desarrollo, usar **ngrok**:

#### Instalar ngrok
```powershell
# Windows (con Chocolatey)
choco install ngrok

# O descargar desde https://ngrok.com/download
```

#### Iniciar túnel ngrok
```powershell
# Exponer puerto 5001 (HTTPS de Kestrel en desarrollo)
ngrok http https://localhost:5001

# Ngrok mostrará algo como:
# Forwarding  https://abc123.ngrok.io -> https://localhost:5001
```

#### Configurar Webhook en Transbank
1. Ir al [Portal de Transbank](https://www.transbankdevelopers.cl/mi-panel)
2. Configurar URL de webhook: `https://abc123.ngrok.io/api/payments/webhook`
3. Guardar configuración

**Nota:** La URL de ngrok cambia cada vez que se reinicia. Para URL estable, usar plan de pago de ngrok o servicio similar.

## 🧪 Testing en Sandbox

### Tarjetas de Prueba Transbank

| Tarjeta | Número | CVV | Resultado |
|---------|--------|-----|-----------|
| **Visa** | 4051885600446623 | 123 | ✅ Aprobada |
| **Mastercard** | 5186059559590568 | 123 | ✅ Aprobada |
| **Visa (Rechazo)** | 4051885600446630 | 123 | ❌ Rechazada |

### Flujo de Prueba Completo

#### 1. Inscribir Tarjeta de Prueba

```bash
# Llamar al endpoint de inscripción
curl -X POST https://localhost:5001/api/payments/inscripcion/iniciar \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "email": "test@autoguia.cl",
    "username": "usuario_test",
    "returnUrl": "https://localhost:5001/cuenta/medios-pago"
  }'
```

Respuesta:
```json
{
  "success": true,
  "token": "e9d555262db0f989e49d724b4db0b0af367cc415cde41f500a776550fc5fddd4",
  "urlWebpay": "https://webpay3gint.transbank.cl/webpayserver/bp_inscription.cgi",
  "transactionId": 123
}
```

#### 2. Simular Ingreso de Tarjeta
1. Navegar a `urlWebpay` en navegador
2. Ingresar tarjeta de prueba: `4051885600446623`
3. CVV: `123`
4. Transbank redirige a `returnUrl?token=XXXX`

#### 3. Confirmar Inscripción
```bash
curl -X POST https://localhost:5001/api/payments/inscripcion/confirmar \
  -H "Content-Type: application/json" \
  -d '{
    "token": "e9d555262db0f989e49d724b4db0b0af367cc415cde41f500a776550fc5fddd4"
  }'
```

#### 4. Realizar Cobro de Prueba
```bash
curl -X POST https://localhost:5001/api/payments/suscripciones/1/cobrar-inicial \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

### Verificar Logs

Los logs de pago se almacenan en:
1. **Base de datos** - Tabla `PaymentLogs`
2. **Archivos** - `logs/autoguia-*.log` (Serilog)
3. **Consola** - Output de la aplicación

Ejemplo de consulta:
```sql
SELECT * FROM PaymentLogs 
WHERE Event LIKE '%CHARGE%' 
ORDER BY CreatedAt DESC 
LIMIT 10;
```

## 📊 Endpoints API Disponibles

### Inscripción de Medios de Pago

#### `POST /api/payments/inscripcion/iniciar`
Inicia inscripción de tarjeta.

**Request:**
```json
{
  "email": "usuario@example.com",
  "username": "nombre_usuario",
  "returnUrl": "https://tuapp.com/callback"
}
```

**Response:**
```json
{
  "success": true,
  "token": "xxx",
  "urlWebpay": "https://webpay3gint.transbank.cl/...",
  "transactionId": 123
}
```

#### `POST /api/payments/inscripcion/confirmar?token=xxx`
Confirma inscripción después de redirect de Transbank.

**Response:**
```json
{
  "success": true,
  "tbkToken": "yyy",
  "last4Digits": "6623",
  "cardType": "Visa",
  "paymentMethodId": 456
}
```

### Gestión de Medios de Pago

#### `GET /api/payments/medios-pago`
Lista medios de pago del usuario autenticado.

#### `DELETE /api/payments/medios-pago/{id}`
Elimina un medio de pago.

#### `PUT /api/payments/medios-pago/{id}/predeterminado`
Establece un medio de pago como predeterminado.

### Cobros

#### `POST /api/payments/suscripciones/{id}/cobrar-inicial`
Procesa cobro inicial de suscripción.

#### `POST /api/payments/suscripciones/{id}/renovar`
Renueva manualmente una suscripción.

### Consultas

#### `GET /api/payments/transacciones?limit=50`
Obtiene historial de transacciones del usuario.

### Webhook

#### `POST /api/payments/webhook`
Endpoint para recibir notificaciones de Transbank (sin autenticación).

## 🔄 Background Service

El servicio `SubscriptionBillingBackgroundService` se ejecuta automáticamente:

- **Horario:** Diariamente a las 2:00 AM
- **Tareas:**
  1. Actualizar suscripciones vencidas
  2. Cobrar suscripciones próximas a vencer (3 días antes)
  3. Reintentar cobros fallidos (primeros 5 días del mes)

### Logs del Background Service
```
🚀 Servicio de facturación de suscripciones iniciado
⏰ Iniciando proceso de facturación diario
📋 Paso 1: Actualizando estado de suscripciones vencidas
💳 Paso 2: Procesando cobros recurrentes
✅ Cobros recurrentes completados: 10 procesados, 9 exitosos, 1 fallidos
🔄 Paso 3: Reintentando cobros fallidos
🎉 Proceso de facturación diario completado exitosamente
```

## 🔐 Seguridad

### Idempotencia
Cada transacción tiene un `BuyOrder` único. Si se intenta cobrar dos veces con la misma orden, se devuelve el resultado de la primera transacción.

### Validación de Webhook
El endpoint de webhook no requiere autenticación (Transbank no envía headers de auth), pero valida:
- Token de transacción existe en BD
- Transacción no procesada previamente
- Datos coinciden con transacción registrada

### Tokens TBK
Los tokens de Transbank se almacenan encriptados en la BD y solo se usan internamente para cobros.

## 🚀 Migración a Producción

### Checklist Pre-Producción

- [ ] ✅ Obtener credenciales de producción de Transbank
- [ ] ✅ Actualizar `appsettings.Production.json` con credenciales reales
- [ ] ✅ Configurar URL de webhook permanente (no ngrok)
- [ ] ✅ Probar flujo completo en ambiente de staging
- [ ] ✅ Configurar alertas para fallos de cobro
- [ ] ✅ Documentar proceso de soporte para usuarios
- [ ] ✅ Implementar monitoring de transacciones

### Configuración de Producción

1. **Actualizar Environment:**
```json
{
  "Transbank": {
    "Environment": "Production"
  }
}
```

2. **Webhook en Producción:**
```
https://tudominio.com/api/payments/webhook
```

3. **Verificar Certificados SSL:**
Transbank requiere certificados SSL válidos en producción.

## 📈 Monitoring y Alertas

### Métricas Importantes
- Tasa de aprobación de cobros
- Tiempo promedio de respuesta de Transbank
- Número de webhooks no procesados
- Suscripciones suspendidas por fallo de pago

### Consultas SQL Útiles

```sql
-- Transacciones fallidas en últimas 24h
SELECT * FROM TransbankTransactions 
WHERE Status IN (3, 5) -- Rejected, Error
  AND CreatedAt >= NOW() - INTERVAL '24 hours';

-- Suscripciones suspendidas
SELECT * FROM Suscripciones 
WHERE Estado = 4 -- Suspendida
ORDER BY UpdatedAt DESC;

-- Tasa de aprobación diaria
SELECT 
  DATE(CreatedAt) as Fecha,
  COUNT(*) as Total,
  SUM(CASE WHEN Status = 2 THEN 1 ELSE 0 END) as Aprobadas,
  ROUND(SUM(CASE WHEN Status = 2 THEN 1 ELSE 0 END) * 100.0 / COUNT(*), 2) as TasaAprobacion
FROM TransbankTransactions
WHERE Type = 2 -- RecurringCharge
  AND CreatedAt >= NOW() - INTERVAL '30 days'
GROUP BY DATE(CreatedAt)
ORDER BY Fecha DESC;
```

## 🆘 Troubleshooting

### Error: "Medio de pago no válido"
- **Causa:** Token TBK expiró o fue eliminado en Transbank
- **Solución:** Usuario debe re-inscribir tarjeta

### Error: "Transacción rechazada"
- **Causa:** Fondos insuficientes, tarjeta bloqueada, etc.
- **Solución:** Usuario debe actualizar método de pago o contactar banco

### Webhook no se recibe
- **Causa:** URL incorrecta, firewall bloqueando, SSL inválido
- **Solución:** 
  1. Verificar URL en portal de Transbank
  2. Probar manualmente con curl
  3. Revisar logs de ngrok/servidor

### Background Service no ejecuta
- **Causa:** Servicio no registrado en Program.cs
- **Solución:** Verificar línea:
```csharp
builder.Services.AddHostedService<SubscriptionBillingBackgroundService>();
```

## 📚 Referencias

- [Documentación Transbank OneClick](https://www.transbankdevelopers.cl/documentacion/oneclick)
- [Portal de Desarrolladores](https://www.transbankdevelopers.cl)
- [Códigos de Respuesta](https://www.transbankdevelopers.cl/referencia/codigos-de-respuesta)
- [Tarjetas de Prueba](https://www.transbankdevelopers.cl/documentacion/como_empezar#tarjetas-de-prueba)

## 🎯 Próximos Pasos

1. **Implementar UI en Blazor** para gestión de medios de pago
2. **Dashboard de Admin** para monitoring de transacciones
3. **Notificaciones por email** para cobros fallidos
4. **Reporte mensual** de facturación
5. **Integración con sistema de facturación chilena** (SII)

---

**Última actualización:** Octubre 2025  
**Versión:** 1.0  
**Mantenedor:** Equipo AutoGuía
