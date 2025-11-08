# 🧪 Guía de Testing y Despliegue - Transbank Webpay OneClick

## 📋 Tabla de Contenidos

1. [Configuración del Entorno Sandbox](#1-configuración-del-entorno-sandbox)
2. [Certificados y Credenciales](#2-certificados-y-credenciales)
3. [Configuración de ngrok para Webhooks](#3-configuración-de-ngrok-para-webhooks)
4. [Tarjetas de Prueba](#4-tarjetas-de-prueba)
5. [Casos de Prueba](#5-casos-de-prueba)
6. [Checklist de Producción](#6-checklist-de-producción)
7. [Troubleshooting](#7-troubleshooting)

---

## 1. Configuración del Entorno Sandbox

### 📍 URLs del Entorno Sandbox

```
Base URL: https://webpay3gint.transbank.cl
Inscripción OneClick: /rswebpaytransaction/api/oneclick/v1.0/inscriptions
Cobro OneClick: /rswebpaytransaction/api/oneclick/v1.0/transactions
```

### ⚙️ Credenciales por Defecto (Sandbox)

```json
{
  "Transbank": {
    "CommerceCode": "597055555584",
    "ApiKey": "579B532A7440BB0C9079DED94D31EA1615BACEB56610332264630D42D0A36B1C",
    "Environment": "Sandbox",
    "WebhookSecret": "tu-secreto-compartido-sandbox"
  }
}
```

### 📝 Configurar `appsettings.Development.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Rodavia.Web.Services.Payments": "Debug"
    }
  },
  "Transbank": {
    "CommerceCode": "597055555584",
    "ApiKey": "579B532A7440BB0C9079DED94D31EA1615BACEB56610332264630D42D0A36B1C",
    "Environment": "Sandbox",
    "WebhookUrl": "https://abc123.ngrok.io/api/payments/webhook",
    "ReturnUrl": "https://abc123.ngrok.io/pagos/transbank/retorno"
  },
  "ConnectionStrings": {
    "IdentityConnection": "Host=localhost;Port=5434;Database=identity_dev;Username=postgres;Password=postgres",
    "DefaultConnection": "Host=localhost;Port=5433;Database=Rodavia_dev;Username=postgres;Password=postgres"
  }
}
```

---

## 2. Certificados y Credenciales

### 🔐 Credenciales de Sandbox (Públicas)

**OneClick Mall Test:**
- **Commerce Code**: `597055555584`
- **API Key**: `579B532A7440BB0C9079DED94D31EA1615BACEB56610332264630D42D0A36B1C`

> ⚠️ **Importante**: Estas credenciales son **SOLO para Sandbox**. NO usar en producción.

### 🏢 Obtener Credenciales de Producción

1. **Registrarse en Transbank**:
   - Ir a [https://www.transbank.cl](https://www.transbank.cl)
   - Contactar al equipo comercial
   - Solicitar producto: **Webpay OneClick**

2. **Proceso de Certificación**:
   - Completar formulario de integración
   - Realizar pruebas obligatorias
   - Enviar evidencias de testing
   - Esperar aprobación (5-10 días hábiles)

3. **Recibir Credenciales**:
   ```json
   {
     "CommerceCode": "Tu código real (8 dígitos)",
     "ApiKey": "Tu API Key real (64 caracteres hex)",
     "Environment": "Production"
   }
   ```

4. **Configurar Producción**:
   ```json
   // appsettings.Production.json
   {
     "Transbank": {
       "CommerceCode": "12345678",
       "ApiKey": "TU_API_KEY_REAL_AQUI",
       "Environment": "Production",
       "WebhookUrl": "https://Rodavia.cl/api/payments/webhook"
     }
   }
   ```

### 🔒 Seguridad de Credenciales

**✅ Buenas Prácticas:**

```bash
# 1. Usar Azure Key Vault
dotnet add package Azure.Extensions.AspNetCore.Configuration.Secrets

# 2. Variables de Entorno
export TRANSBANK_COMMERCE_CODE="12345678"
export TRANSBANK_API_KEY="tu-api-key"

# 3. User Secrets (Desarrollo)
dotnet user-secrets init
dotnet user-secrets set "Transbank:ApiKey" "tu-api-key"
```

**❌ Nunca hacer:**
- Commitear credenciales en Git
- Compartir API Keys por email
- Usar credenciales de producción en Sandbox

---

## 3. Configuración de ngrok para Webhooks

### 🌐 ¿Por qué ngrok?

Transbank necesita enviar notificaciones webhook a una **URL pública**. En desarrollo local, ngrok expone tu `localhost` al internet.

### 📥 Instalación

**Windows (Chocolatey):**
```powershell
choco install ngrok
```

**Windows (Manual):**
1. Descargar de [https://ngrok.com/download](https://ngrok.com/download)
2. Extraer `ngrok.exe` a `C:\ngrok\`
3. Agregar al PATH

**Linux/Mac:**
```bash
brew install ngrok
```

### 🔑 Autenticación

```bash
# Registrarse en ngrok.com y obtener token
ngrok config add-authtoken TU_TOKEN_AQUI
```

### 🚀 Iniciar ngrok

**Exponer puerto HTTPS (7001):**
```bash
ngrok http https://localhost:7001
```

**Salida esperada:**
```
ngrok by @inconshreveable

Session Status                online
Account                       usuario@email.com
Version                       3.3.5
Region                        United States (us)
Forwarding                    https://abc123.ngrok.io -> https://localhost:7001

Connections                   ttl     opn     rt1     rt5     p50     p90
                              0       0       0.00    0.00    0.00    0.00
```

### ⚙️ Configurar URL de Retorno

**Actualizar `appsettings.Development.json`:**
```json
{
  "Transbank": {
    "WebhookUrl": "https://abc123.ngrok.io/api/payments/webhook",
    "ReturnUrl": "https://abc123.ngrok.io/pagos/transbank/retorno"
  }
}
```

**O configurar en el código:**
```csharp
// En MediosPago.razor
var baseUrl = "https://abc123.ngrok.io"; // Reemplazar con tu URL de ngrok
var returnUrl = $"{baseUrl}/pagos/transbank/retorno";
```

### 🔍 Monitorear Webhooks

**Abrir inspector de ngrok:**
```
http://localhost:4040
```

Aquí verás todas las requests entrantes de Transbank.

### 💡 Tips de ngrok

**URL Fija (Cuenta paga):**
```bash
ngrok http https://localhost:7001 --domain=Rodavia.ngrok.io
```

**Configuración persistente:**
```yaml
# ngrok.yml
version: "2"
authtoken: TU_TOKEN_AQUI
tunnels:
  Rodavia:
    proto: http
    addr: https://localhost:7001
    bind_tls: true
```

**Iniciar con config:**
```bash
ngrok start Rodavia
```

---

## 4. Tarjetas de Prueba

### 💳 Tarjetas de Débito RedCompra (Sandbox)

| Tarjeta | Número | CVV | Fecha Exp. | Resultado |
|---------|--------|-----|------------|-----------|
| **Visa Débito** | `4051885600446623` | `123` | `12/25` | ✅ Aprobada |
| **Visa Débito** | `5186059559590568` | `123` | `12/25` | ❌ Rechazada |

### 💳 Tarjetas de Crédito (Sandbox)

| Tarjeta | Número | CVV | Fecha Exp. | Resultado |
|---------|--------|-----|------------|-----------|
| **Visa** | `4051885600446623` | `123` | `12/25` | ✅ Aprobada |
| **Mastercard** | `5186059559590568` | `123` | `12/25` | ❌ Rechazada |

### 🔐 Credenciales de Usuario Transbank (Sandbox)

**RUT**: `11.111.111-1`  
**Password**: `123` (se muestra en la pantalla de Webpay)

### 📝 Flujo de Inscripción con Tarjeta de Prueba

1. Usuario hace clic en "Pagar con Webpay"
2. Sistema genera URL de Webpay
3. Usuario es redirigido a Transbank
4. **Pantalla de Webpay**:
   - Ingresar número de tarjeta: `4051885600446623`
   - CVV: `123`
   - Fecha expiración: `12/25`
   - Click "Continuar"
5. **Autenticación 3D Secure** (simulada):
   - RUT: `11.111.111-1`
   - Clave Dinámica: `123`
6. Transbank redirige a: `https://Rodavia.cl/pagos/transbank/retorno?token_ws=...`
7. Sistema confirma inscripción
8. Tarjeta queda inscrita como `****6623`

---

## 5. Casos de Prueba

### ✅ Test 1: Inscripción Exitosa

**Objetivo**: Verificar inscripción OneClick completa

**Pasos:**
```bash
1. Iniciar aplicación: dotnet run --project Rodavia.Web/Rodavia.Web
2. Iniciar ngrok: ngrok http https://localhost:7001
3. Navegar a: https://abc123.ngrok.io/cuenta/medios-pago
4. Click en "Pagar con Webpay"
5. Verificar redirección a Webpay
6. Ingresar tarjeta: 4051885600446623
7. Completar autenticación
8. Verificar retorno exitoso
9. Verificar tarjeta en lista
```

**Validaciones:**
```sql
-- Verificar transacción en BD
SELECT * FROM "TransbankTransactions" 
WHERE "Type" = 1 -- Inscription
  AND "Status" = 2 -- Approved
ORDER BY "CreatedAt" DESC LIMIT 1;

-- Verificar PaymentMethod creado
SELECT * FROM "PaymentMethods" 
WHERE "IsActive" = true 
ORDER BY "InscriptionDate" DESC LIMIT 1;

-- Verificar logs
SELECT * FROM "PaymentLogs" 
WHERE "Event" LIKE '%INSCRIPTION%' 
ORDER BY "CreatedAt" DESC LIMIT 5;
```

**Resultado Esperado:**
- ✅ Estado: `Aprobada`
- ✅ TbkToken: Generado (40 caracteres hex)
- ✅ Last4Digits: `6623`
- ✅ AuthorizationCode: 6 dígitos

---

### ✅ Test 2: Cobro Recurrente

**Objetivo**: Verificar cobro con token OneClick

**Pasos:**
```bash
1. Tener tarjeta inscrita (Test 1)
2. Crear suscripción de prueba
3. Ejecutar: POST /api/payments/suscripciones/{id}/cobrar-inicial
4. Verificar cobro exitoso
```

**Request (Postman):**
```http
POST https://abc123.ngrok.io/api/payments/suscripciones/1/cobrar-inicial
Authorization: Bearer {jwt_token}
Content-Type: application/json
```

**Validaciones:**
```sql
-- Verificar cobro
SELECT * FROM "TransbankTransactions" 
WHERE "Type" = 2 -- RecurringCharge
  AND "Status" = 2 -- Approved
  AND "SuscripcionId" = 1;

-- Verificar suscripción actualizada
SELECT "Id", "Estado", "FechaInicio", "FechaVencimiento", 
       "LastPaymentStatus", "LastTransactionId"
FROM "Suscripciones" 
WHERE "Id" = 1;
```

**Resultado Esperado:**
- ✅ Transacción: `Approved`
- ✅ Suscripción: `Activa`
- ✅ FechaVencimiento: +1 mes
- ✅ AuthorizationCode: Generado

---

### ❌ Test 3: Cobro Rechazado

**Objetivo**: Manejar rechazos de tarjeta

**Pasos:**
```bash
1. Inscribir tarjeta rechazada: 5186059559590568
2. Intentar cobro
3. Verificar manejo de error
```

**Validaciones:**
```sql
-- Verificar transacción rechazada
SELECT * FROM "TransbankTransactions" 
WHERE "Status" = 3 -- Rejected
ORDER BY "CreatedAt" DESC LIMIT 1;

-- Verificar contador de fallos
SELECT "FailedAttempts", "LastFailedAttempt" 
FROM "PaymentMethods" 
WHERE "Last4Digits" = '0568';

-- Verificar suscripción suspendida
SELECT "Estado" FROM "Suscripciones" 
WHERE "LastPaymentStatus" = 'Rejected';
```

**Resultado Esperado:**
- ✅ Status: `Rejected`
- ✅ FailedAttempts: Incrementado
- ✅ Suscripción: `Suspendida`
- ✅ ErrorMessage: Descriptivo

---

### 🔔 Test 4: Procesamiento de Webhook

**Objetivo**: Verificar recepción y procesamiento de webhook

**Pasos:**
```bash
1. Ejecutar inscripción/cobro
2. Esperar webhook de Transbank
3. Verificar en ngrok inspector (localhost:4040)
4. Validar procesamiento en BD
```

**Simular webhook manualmente:**
```bash
curl -X POST https://abc123.ngrok.io/api/payments/webhook \
  -H "Content-Type: application/json" \
  -d '{
    "token": "e9d555262db0f989e49d724b4db0b0af367cc415cde41f500a776550fc5fddd4",
    "buyOrder": "SUB-42-20251028155820",
    "status": "AUTHORIZED",
    "authorizationCode": "123456",
    "responseCode": "0",
    "amount": 9990,
    "transactionDate": "2025-10-28T15:58:20Z"
  }'
```

**Validaciones:**
```sql
-- Verificar webhook procesado
SELECT "WebhookProcessed", "WebhookProcessedAt" 
FROM "TransbankTransactions" 
WHERE "BuyOrder" = 'SUB-42-20251028155820';

-- Verificar log de webhook
SELECT * FROM "PaymentLogs" 
WHERE "Event" = 'WEBHOOK_PROCESSED' 
ORDER BY "CreatedAt" DESC LIMIT 1;
```

**Resultado Esperado:**
- ✅ WebhookProcessed: `true`
- ✅ Status actualizado según webhook
- ✅ Log registrado

---

### 🔒 Test 5: Idempotencia

**Objetivo**: Verificar que BuyOrder duplicados no crean cobros múltiples

**Pasos:**
```bash
1. Crear cobro con BuyOrder: "TEST-IDEM-001"
2. Intentar mismo cobro con mismo BuyOrder
3. Verificar que retorna transacción anterior
```

**Request:**
```http
POST /api/payments/cobrar
Content-Type: application/json

{
  "usuarioId": "user-guid-123",
  "paymentMethodId": 5,
  "monto": 9990,
  "buyOrder": "TEST-IDEM-001",
  "cuotas": 1
}
```

**Validaciones:**
```sql
-- Debe haber solo 1 transacción
SELECT COUNT(*) FROM "TransbankTransactions" 
WHERE "BuyOrder" = 'TEST-IDEM-001';
-- Resultado esperado: 1

-- Verificar log de idempotencia
SELECT * FROM "PaymentLogs" 
WHERE "Message" LIKE '%ya procesada previamente%';
```

**Resultado Esperado:**
- ✅ Solo 1 registro en BD
- ✅ Segunda llamada retorna datos de primera
- ✅ ResponseMessage: "Transacción ya procesada previamente"

---

### 🔄 Test 6: Background Service (Cobros Automáticos)

**Objetivo**: Verificar job diario de renovación

**Pasos:**
```bash
1. Crear suscripción con RenovacionAutomatica = true
2. Establecer FechaVencimiento = DateTime.UtcNow.AddDays(2)
3. Esperar ejecución del job (2:00 AM) o ejecutar manualmente
```

**Ejecutar manualmente:**
```csharp
// En Program.cs (temporalmente para testing)
var billingService = app.Services.GetRequiredService<ISubscriptionBillingService>();
await billingService.ProcesarCobrosRecurrentesAsync();
```

**Validaciones:**
```sql
-- Verificar suscripciones procesadas
SELECT * FROM "Suscripciones" 
WHERE "FechaVencimiento" > NOW() + INTERVAL '30 days'
  AND "RenovacionAutomatica" = true;

-- Verificar cobros generados
SELECT * FROM "TransbankTransactions" 
WHERE "Type" = 2 -- RecurringCharge
  AND "CreatedAt" > NOW() - INTERVAL '1 day';
```

**Resultado Esperado:**
- ✅ Suscripciones renovadas
- ✅ FechaVencimiento extendida
- ✅ Cobros procesados exitosamente

---

## 6. Checklist de Producción

### 📋 Pre-Despliegue

#### ✅ Configuración

- [ ] **Credenciales de Producción**
  - [ ] CommerceCode obtenido de Transbank
  - [ ] ApiKey obtenido de Transbank
  - [ ] Environment = "Production"
  
- [ ] **URLs Configuradas**
  - [ ] WebhookUrl apunta a dominio real (`https://Rodavia.cl/api/payments/webhook`)
  - [ ] ReturnUrl apunta a dominio real (`https://Rodavia.cl/pagos/transbank/retorno`)
  - [ ] SSL válido (certificado HTTPS)

- [ ] **Base de Datos**
  - [ ] Migración aplicada en producción
  - [ ] Índices creados correctamente
  - [ ] Backup configurado

- [ ] **Seguridad**
  - [ ] Credenciales en Azure Key Vault / Variables de Entorno
  - [ ] NO hay credenciales hardcodeadas
  - [ ] appsettings.Production.json excluido de Git
  - [ ] Validación HMAC de webhooks implementada (recomendado)

#### ✅ Testing

- [ ] **Todos los tests pasados en Sandbox**
  - [ ] Inscripción exitosa
  - [ ] Cobro exitoso
  - [ ] Cobro rechazado manejado
  - [ ] Webhook procesado
  - [ ] Idempotencia verificada
  - [ ] Background service testeado

- [ ] **Casos Edge**
  - [ ] Usuario sin medio de pago
  - [ ] Tarjeta expirada
  - [ ] Fondos insuficientes
  - [ ] Timeout de Transbank
  - [ ] Webhook duplicado

#### ✅ Monitoreo

- [ ] **Logging Configurado**
  - [ ] Serilog con sinks apropiados
  - [ ] Application Insights / Azure Monitor
  - [ ] Alertas para errores críticos

- [ ] **Métricas**
  - [ ] Tasa de éxito de cobros
  - [ ] Tiempo de respuesta de Transbank
  - [ ] Tasa de rechazo
  - [ ] Webhooks fallidos

#### ✅ Documentación

- [ ] **Runbooks**
  - [ ] Proceso de rollback
  - [ ] Manejo de incidentes
  - [ ] Contactos de soporte Transbank

- [ ] **Usuario**
  - [ ] FAQ de pagos
  - [ ] Manejo de rechazos
  - [ ] Política de reembolsos

---

### 🚀 Despliegue

#### 1. Desplegar Aplicación

```bash
# Build de producción
dotnet publish -c Release -o ./publish

# Subir a servidor
scp -r ./publish/* usuario@servidor:/var/www/Rodavia/

# Reiniciar servicio
sudo systemctl restart Rodavia
```

#### 2. Verificar Configuración

```bash
# Verificar variables de entorno
printenv | grep TRANSBANK

# Test de conectividad
curl -I https://Rodavia.cl/api/payments/info
```

#### 3. Smoke Tests

```bash
# 1. Health check
curl https://Rodavia.cl/health

# 2. Test endpoint público
curl https://Rodavia.cl/api/payments/info

# 3. Test inscripción (con usuario real)
# Navegar a https://Rodavia.cl/cuenta/medios-pago
# Completar flujo OneClick
```

#### 4. Monitorear Logs

```bash
# Ver logs en tiempo real
tail -f /var/log/Rodavia/payments.log

# Filtrar errores
grep -i error /var/log/Rodavia/payments.log
```

---

### 📊 Post-Despliegue

#### Primeras 24 horas:

- [ ] Monitorear tasa de éxito de transacciones
- [ ] Revisar logs cada 4 horas
- [ ] Verificar webhooks procesados correctamente
- [ ] Confirmar cobros automáticos (2:00 AM)

#### Primera semana:

- [ ] Analizar métricas de rendimiento
- [ ] Recopilar feedback de usuarios
- [ ] Ajustar timeouts si necesario
- [ ] Optimizar reintentos fallidos

#### Primer mes:

- [ ] Revisar tasas de rechazo
- [ ] Analizar motivos de suspensiones
- [ ] Optimizar flujo de usuario
- [ ] Planificar mejoras

---

## 7. Troubleshooting

### ❌ Problema: "Error al iniciar inscripción"

**Síntomas:**
- Usuario hace clic en "Pagar con Webpay"
- Mensaje de error: "Error al comunicarse con el servidor"

**Diagnóstico:**
```bash
# Verificar logs
grep "Iniciando inscripción" /var/log/Rodavia/payments.log

# Verificar configuración
dotnet run --project Rodavia.Web/Rodavia.Web -- --environment Development
```

**Soluciones:**
1. Verificar credenciales en `appsettings.json`
2. Verificar conectividad con Transbank:
   ```bash
   curl -v https://webpay3gint.transbank.cl
   ```
3. Revisar firewall/proxy

---

### ❌ Problema: "Webhook no se procesa"

**Síntomas:**
- Transacción queda en estado `Pending`
- `WebhookProcessed = false`

**Diagnóstico:**
```bash
# Verificar ngrok activo (desarrollo)
curl http://localhost:4040/api/tunnels

# Verificar logs de webhook
SELECT * FROM "PaymentLogs" 
WHERE "Event" LIKE '%WEBHOOK%' 
ORDER BY "CreatedAt" DESC LIMIT 10;
```

**Soluciones:**
1. Verificar URL pública accesible:
   ```bash
   curl https://Rodavia.cl/api/payments/webhook
   ```
2. Verificar endpoint permite `[AllowAnonymous]`
3. Revisar firewall no bloquea IP de Transbank

**IPs de Transbank (Whitelist):**
```
200.14.85.0/24
200.14.86.0/24
```

---

### ❌ Problema: "Cobro duplicado"

**Síntomas:**
- Usuario reporta 2 cobros por misma suscripción
- Múltiples transacciones con diferentes `BuyOrder`

**Diagnóstico:**
```sql
-- Buscar duplicados
SELECT "SuscripcionId", COUNT(*) 
FROM "TransbankTransactions" 
WHERE "Status" = 2 -- Approved
  AND "Type" = 2 -- RecurringCharge
GROUP BY "SuscripcionId" 
HAVING COUNT(*) > 1;
```

**Soluciones:**
1. Verificar generación única de `BuyOrder`
2. Implementar rate limiting en endpoint
3. Revisar lógica de idempotencia
4. Considerar distributed lock para background service

---

### ❌ Problema: "Tarjeta rechazada constantemente"

**Síntomas:**
- Todas las transacciones se rechazan
- `Status = Rejected`, `ResponseCode != 0`

**Diagnóstico:**
```sql
-- Ver estadísticas de rechazos
SELECT "ResponseCode", COUNT(*) 
FROM "TransbankTransactions" 
WHERE "Status" = 3 -- Rejected
GROUP BY "ResponseCode";
```

**Códigos de Respuesta Comunes:**
- `-1`: Rechazo genérico
- `-2`: Tarjeta bloqueada
- `-3`: Error en validación
- `-4`: Transacción no permitida
- `-5`: Fondos insuficientes

**Soluciones:**
1. Verificar tarjeta válida en Sandbox
2. Usar tarjeta de prueba correcta: `4051885600446623`
3. Verificar CVV y fecha expiración
4. Contactar soporte de Transbank

---

### 📞 Contacto Soporte Transbank

**Mesa de Ayuda:**
- Teléfono: +56 2 2661 2121
- Email: soporte@transbank.cl
- Horario: Lunes a Viernes 9:00 - 18:00

**Documentación Oficial:**
- [https://www.transbankdevelopers.cl](https://www.transbankdevelopers.cl)
- [https://github.com/TransbankDevelopers](https://github.com/TransbankDevelopers)

---

## 📚 Referencias

- **Documentación Transbank OneClick**: https://www.transbankdevelopers.cl/producto/webpay#oneclick
- **SDK .NET**: https://github.com/TransbankDevelopers/transbank-sdk-dotnet-webpay-rest
- **Postman Collection**: https://documenter.getpostman.com/view/1649387/transbank-webpay-oneclick/RW8AnaEB
- **ngrok Docs**: https://ngrok.com/docs
- **Azure Key Vault**: https://learn.microsoft.com/en-us/azure/key-vault/

---

## ✅ Checklist Final

### Desarrollo
- [x] Entidades creadas
- [x] DTOs implementados
- [x] Servicios desarrollados
- [x] Endpoints API creados
- [x] UI implementada
- [x] Migración aplicada
- [x] Tests unitarios pasados

### Testing
- [ ] Inscripción OneClick testeada
- [ ] Cobro recurrente testeado
- [ ] Webhook procesado correctamente
- [ ] Idempotencia verificada
- [ ] Background service validado
- [ ] Casos de error manejados

### Producción
- [ ] Credenciales de producción obtenidas
- [ ] Certificación Transbank completada
- [ ] Variables de entorno configuradas
- [ ] SSL configurado
- [ ] Monitoreo activo
- [ ] Backup configurado
- [ ] Runbooks documentados

---

**✅ Documento completo de testing y despliegue creado!**
