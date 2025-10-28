# 🏦 Tarjetas de Prueba - Transbank Oneclick Mall

## ⚠️ IMPORTANTE: Ambiente Sandbox

Estás usando **Transbank Oneclick Mall** (código de comercio `597055555541`) en ambiente de **integración/sandbox**.

## 🎯 Tarjetas de Prueba para Oneclick

### ✅ Tarjetas que APRUEBAN (Inscripción Exitosa)

| Tipo | Número | CVV | Vencimiento | Resultado |
|------|--------|-----|-------------|-----------|
| **Visa** | `4051 8856 0044 6623` | `123` | `12/25` | ✅ APROBADA |
| **Mastercard** | `5186 0595 5959 0568` | `123` | `12/25` | ✅ APROBADA |

### ❌ Tarjetas que RECHAZAN (Para Probar Flujo de Error)

| Tipo | Número | CVV | Vencimiento | Resultado |
|------|--------|-----|-------------|-----------|
| **Visa** | `4051 8860 0532 6620` | `123` | `12/25` | ❌ RECHAZADA |

## 🔍 ¿Por qué mi tarjeta fue rechazada?

Si usaste `4051 8856 0044 6623` y fue rechazada, puede ser por:

1. **RUT Incorrecto en Webpay**: Transbank sandbox requiere RUT chileno válido
   - **RUT de Prueba**: `11.111.111-1`
   - Si ingresaste otro RUT, la transacción se rechaza

2. **Tarjeta Incorrecta**: Verifica que hayas ingresado **exactamente**:
   ```
   4051 8856 0044 6623
   ```

3. **CVV Incorrecto**: Debe ser `123`

4. **Fecha de Vencimiento**: Debe ser futura, ej: `12/25`

## 🧪 Proceso Completo de Prueba

### Escenario 1: Inscripción Exitosa

1. En AutoGuía, ir a `/suscripciones`
2. Seleccionar plan **Pro** o **Premium**
3. Click en "Confirmar cambio"
4. Serás redirigido a **Webpay Transbank**
5. Ingresar datos:
   - **RUT**: `11.111.111-1`
   - **Clave**: `123` (cualquier número de 3 dígitos)
6. Ingresar tarjeta:
   - **Número**: `4051 8856 0044 6623`
   - **CVV**: `123`
   - **Vencimiento**: `12/25`
7. Confirmar
8. ✅ Deberías volver a AutoGuía con mensaje de **¡Pago Confirmado!**

### Escenario 2: Inscripción Rechazada (Prueba de Error)

1. Mismo proceso, pero usar tarjeta:
   - **Número**: `4051 8860 0532 6620`
2. ❌ Transbank rechazará y volverás con mensaje de error
3. Botón **Reintentar** debe volver a `/suscripciones`

## 📋 Checklist de Verificación

- [ ] Usar tarjeta **4051 8856 0044 6623** (con espacios o sin espacios)
- [ ] CVV: **123**
- [ ] Vencimiento: **12/25** o cualquier fecha futura
- [ ] RUT en Webpay: **11.111.111-1**
- [ ] Clave Webpay: **123**
- [ ] Verificar que estás en ambiente **sandbox** (URL contiene `webpay3gint.transbank.cl`)

## 🔗 Referencias Oficiales

- [Tarjetas de Prueba Transbank](https://www.transbankdevelopers.cl/documentacion/como_empezar#tarjetas-de-prueba)
- [Oneclick Mall - Documentación](https://www.transbankdevelopers.cl/documentacion/oneclick-mall)
- [Ambiente de Integración](https://www.transbankdevelopers.cl/documentacion/como_empezar#ambientes)

## 🐛 Troubleshooting

### "Mi tarjeta aprobada sale rechazada"
1. ✅ Verifica el RUT: **11.111.111-1**
2. ✅ Verifica que sea ambiente **sandbox** (no producción)
3. ✅ Limpia cache del navegador
4. ✅ Revisa logs en la terminal (busca respuesta de Transbank)

### "No me redirige de vuelta a AutoGuía"
1. ✅ Verifica que `/pago/retorno` esté configurado
2. ✅ Verifica que CSP permita form-action a Transbank
3. ✅ Revisa logs para ver si llegó el callback

### "Dice que no se recibió token"
1. ✅ Verifica que no se perdió la sesión
2. ✅ La página `/pago/retorno` es pública (sin [Authorize])
3. ✅ Revisa parámetros de URL al volver de Transbank

## ✅ Estado Actual

- ✅ CSP configurado para permitir Transbank
- ✅ Página de retorno pública (sin autenticación)
- ✅ Botones de reintento funcionando
- ✅ Manejo de rechazos de Transbank
- ✅ JavaScript form submission implementado
