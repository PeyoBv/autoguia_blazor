# 🚗 Guía de Usuario - Asistente de Diagnóstico AutoGuía

## 👋 Introducción

Bienvenido al **Asistente de Diagnóstico de AutoGuía**, una herramienta inteligente diseñada para ayudarte a identificar y comprender problemas en tu vehículo de manera rápida y confiable.

### ¿Qué puede hacer por ti?

El Asistente de Diagnóstico:
- 🔍 **Identifica síntomas** basándose en tu descripción del problema
- 🎯 **Diagnóstica causas posibles** ordenadas por probabilidad
- 📋 **Proporciona pasos de verificación** que puedes seguir tú mismo
- 💡 **Ofrece recomendaciones preventivas** para evitar problemas futuros
- 📊 **Guarda tu historial** para rastrear patrones y mantenimiento

### ¿Cómo funciona?

```
Paso 1: Describes el problema
    ⬇
Paso 2: El asistente identifica síntomas similares
    ⬇
Paso 3: Recibes diagnóstico con causas probables
    ⬇
Paso 4: Sigues pasos de verificación detallados
    ⬇
Paso 5: Accedes a recomendaciones preventivas
```

---

## 🚀 Primeros Pasos

### 1️⃣ Acceder al Asistente

**Opción A: Desde el Menú Principal**
1. Inicia sesión en tu cuenta de AutoGuía
2. Haz clic en **"Diagnóstico"** en el menú de navegación
3. Verás la pantalla de inicio del asistente

**Opción B: Desde el Dashboard**
1. En tu panel principal, busca la tarjeta **"Diagnóstico Rápido"**
2. Haz clic en **"Iniciar Diagnóstico"**

### 2️⃣ Dos Formas de Diagnosticar

#### 🔍 Búsqueda Rápida (Recomendada)
**Mejor para:** Cuando tienes un síntoma claro y quieres resultados rápidos.

1. Escribe una descripción del problema en el cuadro de texto
2. Haz clic en **"Diagnosticar"**
3. Espera los resultados (2-5 segundos)

#### 📚 Navegación por Sistema
**Mejor para:** Cuando no estás seguro de cómo describir el problema.

1. Selecciona el sistema afectado (Motor, Frenos, etc.)
2. Explora la lista de síntomas comunes
3. Haz clic en el que más se parezca a tu problema

---

## ✍️ Cómo Describir el Síntoma

La calidad de tu descripción determina la precisión del diagnóstico.

### ✅ Buenas Descripciones

**Ejemplo 1: Problema de Motor**
```
❌ Mal: "El motor hace ruido"
✅ Bien: "El motor hace ruidos metálicos de golpeteo cuando acelero a más de 3000 RPM"
```

**Ejemplo 2: Problema de Frenos**
```
❌ Mal: "Los frenos no funcionan bien"
✅ Bien: "El pedal de freno se siente esponjoso y necesito presionarlo más fuerte para frenar"
```

**Ejemplo 3: Problema de Transmisión**
```
❌ Mal: "Algo raro con los cambios"
✅ Bien: "La transmisión automática cambia de marcha de forma brusca entre 2da y 3ra velocidad"
```

**Ejemplo 4: Problema Eléctrico**
```
❌ Mal: "No enciende"
✅ Bien: "El motor no arranca, las luces del tablero están débiles y escucho clicks al girar la llave"
```

### 📝 Elementos de una Buena Descripción

Incluye:
1. **¿Qué sucede?** - Describe el síntoma específico
2. **¿Cuándo ocurre?** - Al arrancar, al acelerar, al frenar, etc.
3. **¿Con qué frecuencia?** - Siempre, a veces, solo en frío, etc.
4. **¿Hay sonidos?** - Golpeteo, silbido, chirrido, etc.
5. **¿Qué has observado?** - Humo, luces encendidas, olores, etc.

### 💡 Consejos para Mejores Resultados

- ✅ **Sé específico**: "Ruido metálico" es mejor que "ruido extraño"
- ✅ **Usa términos técnicos** si los conoces: "RPM", "ralentí", "embrague"
- ✅ **Menciona condiciones**: "Solo cuando hace frío", "Después de 20 minutos de conducir"
- ✅ **Incluye contexto**: "Después de cambiar el aceite", "Desde que cambié las llantas"
- ❌ **Evita términos vagos**: "algo raro", "no funciona bien", "está raro"

---

## 🔬 Entender el Diagnóstico

### Estructura de la Respuesta

Cuando recibas un diagnóstico, verás información organizada así:

```
┌─────────────────────────────────────────────────────┐
│  🎯 DIAGNÓSTICO RESULTADO                           │
├─────────────────────────────────────────────────────┤
│                                                     │
│  🚗 SÍNTOMA IDENTIFICADO                           │
│  "Ruidos metálicos en el motor"                    │
│                                                     │
│  ⚠️ NIVEL DE URGENCIA: 3 (Alto)                   │
│  "Problema importante, lleve a servicio pronto"    │
│                                                     │
│  📋 CAUSAS POSIBLES (3 encontradas)                │
│                                                     │
│  1️⃣ Válvulas desgastadas (90% probabilidad)       │
│     ⚙️ Requiere servicio profesional: SÍ          │
│     📖 Ver detalles →                              │
│                                                     │
│  2️⃣ Rodamientos de biela gastados (70%)           │
│     ⚙️ Requiere servicio profesional: SÍ          │
│     📖 Ver detalles →                              │
│                                                     │
│  3️⃣ Nivel de aceite bajo (50%)                    │
│     ⚙️ Requiere servicio profesional: NO          │
│     📖 Ver detalles →                              │
│                                                     │
│  🔧 RECOMENDACIÓN GENERAL                          │
│  "Problema importante, DEBE llevar a servicio     │
│   profesional. No intente repararlo usted mismo." │
│                                                     │
│  ⛽ SERVICIO PROFESIONAL SUGERIDO: SÍ             │
│                                                     │
│  [📊 Ver Historial] [👍 Útil] [👎 No Útil]        │
└─────────────────────────────────────────────────────┘
```

### 🎨 Entender los Niveles de Urgencia

El sistema clasifica cada problema en 4 niveles según su gravedad:

| Nivel | Nombre | Color | Significado | Acción Recomendada |
|-------|--------|-------|-------------|-------------------|
| **1** | 🟢 Leve | Verde | Problema menor que no afecta seguridad | Monitorea el problema y aplica recomendaciones cuando sea conveniente |
| **2** | 🟡 Moderado | Amarillo | Debe atenderse pronto para evitar daños mayores | Programa servicio en los próximos 7-15 días |
| **3** | 🟠 Alto | Naranja | Problema importante que requiere atención inmediata | Lleva el vehículo al taller en los próximos 1-3 días |
| **4** | 🔴 Crítico | Rojo | Peligro inminente, compromete seguridad | ⚠️ **DETÉN EL VEHÍCULO INMEDIATAMENTE** y llama a grúa |

### 📊 Nivel de Probabilidad de Causas

Cada causa tiene un **nivel de probabilidad** (1-5):

- **Nivel 5 (90-100%)**: Muy probable - La causa más común para este síntoma
- **Nivel 4 (70-89%)**: Probable - Causa frecuente, vale la pena verificar
- **Nivel 3 (50-69%)**: Posible - Puede ser la causa en ciertos casos
- **Nivel 2 (30-49%)**: Poco probable - Menos común pero no imposible
- **Nivel 1 (0-29%)**: Rara vez - Causa poco frecuente

**💡 Tip**: Empieza verificando las causas con mayor probabilidad.

---

## 🔧 Guía de Pasos de Verificación

Cuando explores una causa específica, verás una lista de **pasos ordenados** que puedes seguir para verificarla.

### 📖 Ejemplo Completo: Verificar Batería Descargada

#### **Paso 1: Revisar luces del tablero**

**📝 Descripción**  
"Verificar si las luces del tablero encienden normalmente"

**📋 Instrucciones Detalladas**
1. Siéntate en el asiento del conductor
2. Gira la llave a posición **ON** (sin arrancar el motor)
3. Observa las luces del tablero
4. Compara el brillo con condiciones normales

**✅ Indicadores de Éxito**
- Las luces están muy débiles o no encienden
- Las luces parpadean intermitentemente
- Algunas luces no funcionan

**❌ Si no se cumplen los indicadores**
- Las luces encienden normalmente → Pasar al siguiente paso
- No hay luces en absoluto → Problema puede ser más grave

---

#### **Paso 2: Intentar arrancar el motor**

**📝 Descripción**  
"Intentar arrancar el vehículo y escuchar el motor de arranque"

**📋 Instrucciones Detalladas**
1. Asegúrate de estar en PARK o NEUTRAL
2. Pisa el pedal de freno (o embrague en manual)
3. Gira la llave a posición **START**
4. Escucha atentamente el sonido

**✅ Indicadores de Éxito**
- Escuchas "click, click, click" pero el motor no gira
- El motor de arranque gira muy lentamente
- No hay ningún sonido al girar la llave

**❌ Si no se cumplen los indicadores**
- Motor arranca normalmente → Problema no es la batería
- Motor hace otros ruidos → Investigar otras causas

---

#### **Paso 3: Verificar conexiones de la batería**

**📝 Descripción**  
"Revisar que los terminales de la batería estén bien conectados"

**📋 Instrucciones Detalladas**
1. Abre el capó del vehículo
2. Localiza la batería (caja negra con cables rojo y negro)
3. Inspecciona los terminales:
   - Cable **ROJO** = Positivo (+)
   - Cable **NEGRO** = Negativo (-)
4. Intenta mover los terminales con la mano
5. Busca corrosión (polvo blanco/verde)

**✅ Indicadores de Éxito**
- Los terminales se mueven fácilmente (están flojos)
- Hay corrosión visible en los terminales
- Los cables están dañados o partidos

**❌ Si no se cumplen los indicadores**
- Conexiones están firmes y limpias → Batería puede estar descargada

---

### 🎯 ¿Qué Hacer Después de los Pasos?

#### ✅ **Si TODOS los indicadores se cumplen**
→ Confirmaste la causa, procede a:
- Seguir las recomendaciones del diagnóstico
- Llevar al taller si requiere servicio profesional
- Intentar solución temporal si es seguro (ejemplo: carga de batería)

#### ❌ **Si NO se cumplen los indicadores**
→ Esta causa probablemente NO es el problema:
- Regresa al diagnóstico
- Revisa la siguiente causa en la lista
- Si ninguna causa aplica, consulta a profesional

#### ⚠️ **Si no estás seguro**
→ Por seguridad:
- No intentes pasos que requieran herramientas especializadas
- Consulta con un mecánico profesional
- Muestra el diagnóstico de AutoGuía al mecánico

---

## 💡 Recomendaciones Preventivas

Después de cada diagnóstico, recibirás **recomendaciones para prevenir** problemas futuros.

### 📅 Entender las Frecuencias

#### **Por Kilómetros**
```
Cada 5,000 km  →  Cambio de aceite
Cada 10,000 km →  Rotación de llantas
Cada 20,000 km →  Filtro de aire
Cada 30,000 km →  Ajuste de válvulas
Cada 50,000 km →  Bujías
Cada 100,000 km → Correa de distribución
```

#### **Por Meses**
```
Cada 1 mes    →  Revisión visual general
Cada 3 meses  →  Líquidos y niveles
Cada 6 meses  →  Alineación y balanceo
Cada 12 meses →  Inspección completa
Cada 24 meses →  Batería, líquido de frenos
```

### 📝 Ejemplo de Recomendación Completa

```
┌───────────────────────────────────────────────┐
│  💡 RECOMENDACIÓN PREVENTIVA                  │
├───────────────────────────────────────────────┤
│                                               │
│  📌 Cambiar aceite del motor regularmente    │
│                                               │
│  📖 DETALLE                                   │
│  El aceite lubrica todas las partes móviles  │
│  del motor, reduce fricción y disipa calor.  │
│  Aceite viejo o sucio causa desgaste         │
│  prematuro de válvulas, pistones y           │
│  rodamientos, resultando en ruidos y         │
│  pérdida de rendimiento.                     │
│                                               │
│  📅 FRECUENCIA                                │
│  🚗 Cada 5,000 km                            │
│  📆 O cada 3 meses                           │
│  ⚠️ (Lo que llegue primero)                  │
│                                               │
│  💰 COSTO ESTIMADO                           │
│  $30 - $50 USD                               │
│  (Varía según tipo de aceite y vehículo)     │
│                                               │
│  🔔 [Crear Recordatorio]                     │
└───────────────────────────────────────────────┘
```

### 🔔 Configurar Recordatorios

AutoGuía puede enviarte recordatorios automáticos:

1. Haz clic en **"Crear Recordatorio"**
2. Elige método de notificación:
   - 📧 Email
   - 📱 Notificación push
   - 📅 Agregar a calendario
3. Configura anticipación (ej: avisar 500 km antes)
4. Guarda el recordatorio

**Ejemplo de Email**:
```
Asunto: 🔔 Recordatorio: Cambio de aceite en 200 km

Hola Juan,

Tu vehículo Toyota Corolla 2018 necesitará cambio de 
aceite en aproximadamente 200 km (llevas 4,800 km 
desde el último cambio).

Frecuencia recomendada: Cada 5,000 km
Último cambio: 15/09/2025 (4,800 km recorridos)
Próximo cambio: Aproximadamente 22/10/2025

[Ver detalles] [Encontrar taller] [Posponer]

---
AutoGuía - Tu asistente automotriz inteligente
```

---

## 📊 Historial de Consultas

Tu historial es una **herramienta poderosa** para rastrear la salud de tu vehículo.

### 🔍 ¿Qué Información Guardamos?

Cada consulta incluye:

| Campo | Descripción | Ejemplo |
|-------|-------------|---------|
| 📅 **Fecha** | Cuándo realizaste la consulta | 23/10/2025 15:30 |
| 📝 **Síntoma Descrito** | Tu descripción original | "Motor hace ruidos al acelerar" |
| 🎯 **Síntoma Identificado** | Lo que el sistema identificó | "Ruidos metálicos en motor" |
| ⚠️ **Nivel de Urgencia** | Gravedad del problema | 3 (Alto) |
| 🔧 **Recomendación** | Consejo del sistema | "Llevar a servicio profesional" |
| 👍👎 **Tu Feedback** | Si fue útil o no | ✅ Útil |

### 📈 Análisis de Patrones

#### **Ejemplo 1: Problema Recurrente**

```
Historial de Usuario: Juan P.

📅 15/08/2025 → "Motor hace ruidos metálicos"
   Diagnóstico: Válvulas desgastadas
   Acción: Llevó a taller
   
📅 05/09/2025 → "Motor sigue haciendo ruidos"
   Diagnóstico: Válvulas desgastadas
   Acción: Reclamó al taller
   
📅 20/10/2025 → "Ruidos persisten en motor"
   Diagnóstico: Válvulas desgastadas
   Acción: ???

🚨 PATRÓN DETECTADO: Problema recurrente
💡 SUGERENCIA: El problema raíz no fue resuelto.
   Considera un segundo diagnóstico profesional o
   cambiar de taller mecánico.
```

#### **Ejemplo 2: Mantenimiento Deficiente**

```
Historial de Usuario: María G.

📅 01/05/2025 → "Pérdida de potencia"
   Causa: Filtro de aire sucio
   
📅 15/07/2025 → "Motor se sobrecalienta"
   Causa: Líquido refrigerante bajo
   
📅 23/10/2025 → "Ruidos en motor"
   Causa: Aceite bajo

🚨 PATRÓN DETECTADO: Falta de mantenimiento preventivo
💡 SUGERENCIA: Establece rutina de mantenimiento:
   - Aceite cada 5,000 km
   - Filtros cada 10,000 km
   - Líquidos cada 3 meses
```

### 📤 Exportar Historial

Puedes exportar tu historial en diferentes formatos:

**Formato PDF** (Para mecánicos)
```
[Exportar a PDF]
→ Genera reporte con:
  - Todas las consultas
  - Síntomas y diagnósticos
  - Fechas y frecuencias
  - Gráficos de patrones
```

**Formato Excel** (Para análisis)
```
[Exportar a Excel]
→ Genera hoja de cálculo con:
  - Tabla de consultas
  - Columnas ordenables
  - Filtros por fecha/urgencia
  - Estadísticas resumidas
```

**Formato Impreso** (Para archivo físico)
```
[Imprimir]
→ Versión optimizada para papel:
  - Información esencial
  - Sin gráficos pesados
  - Formato A4/Carta
```

---

## 👍👎 Feedback y Mejora Continua

Tu feedback es **fundamental** para mejorar el sistema.

### ¿Cómo Proporcionar Feedback?

Después de cada diagnóstico, verás dos botones:

```
┌─────────────────────────────┐
│  ¿Te fue útil este          │
│  diagnóstico?               │
│                             │
│  [👍 Sí, fue útil]          │
│  [👎 No, no fue útil]       │
└─────────────────────────────┘
```

#### Si seleccionas "Sí, fue útil" 👍

```
✅ ¡Gracias por tu feedback!

¿El diagnóstico te ayudó a:
□ Identificar el problema
□ Resolver el problema yo mismo
□ Entender qué decirle al mecánico
□ Ahorrar dinero

[Enviar] [Saltar]
```

#### Si seleccionas "No, no fue útil" 👎

```
😔 Lamentamos que no te hayamos ayudado.

¿Por qué no fue útil?
□ No identificó el síntoma correctamente
□ Las causas no aplicaban a mi caso
□ Los pasos eran muy complicados
□ Faltaba información
□ Otro: [___________________]

[Enviar Feedback] [Cancelar]
```

### 🎯 ¿Por Qué es Importante Tu Feedback?

Tu opinión ayuda a:

1. **🎯 Mejorar la Precisión**
   - Ajustar algoritmos de diagnóstico
   - Identificar síntomas mal clasificados
   - Priorizar causas más comunes

2. **📚 Expandir la Base de Conocimiento**
   - Agregar nuevos síntomas
   - Descubrir causas no consideradas
   - Actualizar descripciones técnicas

3. **👥 Beneficiar a la Comunidad**
   - Otros usuarios obtienen mejores diagnósticos
   - Reducir diagnósticos incorrectos
   - Crear contenido más útil

4. **📊 Generar Estadísticas**
   - Identificar problemas más comunes
   - Optimizar recomendaciones
   - Mejorar experiencia de usuario

---

## ❓ Preguntas Frecuentes (FAQ)

### 🔍 Sobre el Diagnóstico

#### **P: ¿Qué tan confiable es el diagnóstico de AutoGuía?**

**R:** El Asistente de Diagnóstico de AutoGuía es una **herramienta de referencia inicial** basada en una extensa base de datos de síntomas y causas automotrices comunes. 

**Precisión estimada**:
- ✅ 85-90% para síntomas comunes y bien descritos
- ✅ 70-80% para síntomas menos específicos
- ✅ 60-70% para problemas complejos o raros

**⚠️ IMPORTANTE**: El diagnóstico **NO reemplaza** a un mecánico profesional certificado. Para problemas críticos (nivel 4) o si no estás seguro, **siempre consulta a un profesional**.

---

#### **P: ¿Qué hago si el diagnóstico sugiere ir al taller?**

**R:** Sigue estos pasos:

1. **Revisa los pasos de verificación** para confirmar la causa
2. **Si son pasos simples** (revisar niveles, conexiones), inténtalos
3. **Si son pasos complejos** (desmontar piezas, usar herramientas especializadas), confía en un profesional
4. **Lleva la información del diagnóstico** al mecánico:
   - Imprime o guarda en PDF
   - Comparte el síntoma identificado
   - Menciona las causas más probables

**💡 Tip**: Un mecánico apreciará que llegues con información preliminar, ahorra tiempo de diagnóstico (y dinero).

---

#### **P: El diagnóstico no encontró mi síntoma, ¿qué hago?**

**R:** Prueba estas alternativas:

1. **Describe con más detalle**:
   - Agrega cuándo ocurre
   - Menciona sonidos específicos
   - Incluye condiciones (frío, caliente, etc.)

2. **Usa términos técnicos** si los conoces:
   - "Ralentí irregular" en vez de "motor tiembla"
   - "Pedal esponjoso" en vez de "freno suave"

3. **Navega por sistema**:
   - Selecciona el sistema afectado
   - Explora la lista de síntomas
   - Encuentra el más similar

4. **Contacta soporte**:
   - Reporta el síntoma no encontrado
   - Ayuda a mejorar la base de datos
   - Recibirás respuesta personalizada

---

### 🔐 Sobre Privacidad y Seguridad

#### **P: ¿Mi historial es privado?**

**R:** **Sí, completamente privado**:

- 🔒 Solo tú puedes ver tu historial
- 🔐 Datos encriptados en la base de datos
- 🚫 No compartimos información con terceros sin tu consentimiento
- 🗑️ Puedes eliminar consultas en cualquier momento
- 📧 No enviamos emails no solicitados

**Excepciones** (con tu consentimiento explícito):
- Compartir con mecánico seleccionado
- Estadísticas anónimas agregadas para mejora del servicio
- Cumplimiento de órdenes judiciales (casos excepcionales)

---

#### **P: ¿Puedo eliminar mi historial?**

**R:** Sí, tienes control total:

**Eliminar consulta individual**:
1. Ve a tu historial
2. Haz clic en "⋮" (menú) junto a la consulta
3. Selecciona "Eliminar consulta"
4. Confirma la acción

**Eliminar todo el historial**:
1. Ve a Configuración → Privacidad
2. Haz clic en "Eliminar historial completo"
3. Confirma ingresando tu contraseña
4. Todos los datos se borran permanentemente

**⚠️ Nota**: Una vez eliminado, **no se puede recuperar**.

---

### 🚗 Sobre Cobertura de Síntomas

#### **P: ¿Qué sistemas automotrices cubre AutoGuía?**

**R:** Actualmente cubrimos **5 sistemas principales**:

| Sistema | Síntomas Cubiertos | Ejemplos |
|---------|-------------------|----------|
| 🔧 **Motor** | 15+ síntomas | Arranque, ruidos, potencia, sobrecalentamiento |
| ⚙️ **Transmisión** | 8+ síntomas | Cambios bruscos, patinamiento, ruidos |
| 🛑 **Frenos** | 10+ síntomas | Pedal esponjoso, ruidos, vibración |
| 🔌 **Eléctrico** | 12+ síntomas | Batería, alternador, luces, fusibles |
| 🌀 **Suspensión** | 7+ síntomas | Ruidos, alineación, confort |

**🚀 Próximamente** (Q1 2026):
- ❄️ Sistema de A/C y Calefacción
- 🚗 Sistema de Dirección
- 💨 Sistema de Escape
- 💧 Sistema de Enfriamiento

---

#### **P: ¿Funcionaría con mi vehículo específico (marca/modelo)?**

**R:** El Asistente de Diagnóstico funciona con **principios universales** que aplican a la mayoría de vehículos:

**✅ Compatible con**:
- Automóviles de pasajeros (sedanes, SUVs, hatchbacks)
- Transmisión manual y automática
- Motor de gasolina y diésel
- Vehículos de 1990 en adelante

**⚠️ Limitaciones**:
- Vehículos eléctricos (Tesla, etc.) → Cobertura parcial
- Vehículos clásicos (pre-1990) → Puede haber diferencias
- Vehículos comerciales pesados → No optimizado
- Motocicletas → No cubierto actualmente

**💡 Tip**: Si tienes un vehículo moderno con sistemas electrónicos avanzados, algunos diagnósticos pueden requerir escáner OBD-II profesional.

---

### 💰 Sobre Costos y Servicios

#### **P: ¿El Asistente de Diagnóstico es gratis?**

**R:** Depende de tu plan de AutoGuía:

| Plan | Diagnósticos | Historial | Recomendaciones |
|------|--------------|-----------|-----------------|
| 🆓 **Gratuito** | 5 por mes | Últimos 10 | Básicas |
| 💎 **Premium** | Ilimitados | Completo | Detalladas + Recordatorios |
| 🏢 **Empresarial** | Ilimitados | Multi-vehículos | Personalizadas |

**Prueba gratis**: Los primeros 30 días tienes acceso Premium sin costo.

---

#### **P: ¿Puedo compartir mi historial con mi mecánico?**

**R:** ¡Absolutamente! Es una excelente idea:

**Método 1: Exportar PDF**
1. Ve a tu historial
2. Haz clic en "Exportar"
3. Selecciona formato PDF
4. Descarga y envía por email/WhatsApp

**Método 2: Compartir enlace**
1. Abre la consulta específica
2. Haz clic en "Compartir"
3. Genera enlace temporal (expira en 48 horas)
4. Envía el enlace al mecánico

**Método 3: Imprimir**
1. Abre la consulta
2. Haz clic en "Imprimir"
3. Lleva el papel impreso al taller

**💡 Beneficios**:
- ✅ Mecánico entiende el problema más rápido
- ✅ Ahorras tiempo de diagnóstico
- ✅ Evitas malas interpretaciones
- ✅ Potencialmente ahorras dinero

---

## 🔧 Troubleshooting (Solución de Problemas)

### 🚫 Problema: "No se encontró el síntoma"

**Síntomas**:
- El diagnóstico retorna: "No se encontraron síntomas coincidentes"
- No hay causas posibles listadas

**Soluciones**:

1. **Reformula tu descripción**
   ```
   ❌ Antes: "hace ruido"
   ✅ Después: "hace ruido metálico de golpeteo al acelerar"
   ```

2. **Usa sinónimos o términos alternativos**
   ```
   ❌ "El auto vibra"
   ✅ "El vehículo tiembla" o "Vibraciones en el volante"
   ```

3. **Divide el problema**
   ```
   ❌ "El motor hace ruido y pierde potencia"
   ✅ Primera consulta: "El motor hace ruido metálico"
   ✅ Segunda consulta: "Pérdida de potencia al acelerar"
   ```

4. **Usa navegación por sistema**
   - Selecciona el sistema afectado
   - Busca manualmente en la lista

5. **Contacta soporte**
   - Reporta el síntoma no encontrado
   - Include detalles de tu vehículo

---

### ⚠️ Problema: "Dice llevar al taller, pero quiero intentar arreglarlo"

**Consideraciones de Seguridad**:

**✅ Puedes intentar si**:
- Los pasos NO requieren herramientas especializadas
- NO involucran sistemas de seguridad (frenos, dirección)
- Tienes conocimientos básicos de mecánica
- El nivel de urgencia es 1 o 2

**Ejemplos seguros para DIY**:
- Revisar y rellenar niveles de aceite
- Cambiar fusibles quemados
- Limpiar terminales de batería
- Cambiar escobillas limpiaparabrisas
- Revisar presión de llantas

**❌ NO intentes si**:
- El nivel de urgencia es 3 o 4
- Involucra frenos o dirección
- Requiere desmontar componentes del motor
- Necesitas herramientas que no tienes
- No estás 100% seguro de lo que haces

**⚠️ REGLA DE ORO**: Si tienes dudas, consulta profesional. Tu seguridad es lo primero.

---

### 🔍 Problema: "Las causas no aplican a mi caso"

**Posibles razones**:

1. **Descripción incompleta**
   - **Solución**: Agrega más detalles (cuándo, cómo, frecuencia)

2. **Síntoma mal identificado**
   - **Solución**: Reformula o usa navegación manual

3. **Problema muy específico**
   - **Solución**: Consulta profesional con escáner OBD-II

4. **Vehículo con características especiales**
   - **Solución**: Menciona en la descripción (turbo, híbrido, etc.)

**Qué hacer**:
- Marca el diagnóstico como "No útil"
- Proporciona feedback detallado
- Prueba reformular la consulta
- Consulta a mecánico profesional

---

### 📱 Problema: "No encuentro dónde acceder al Asistente"

**Ubicaciones del Asistente**:

1. **Menú Principal** → "Diagnóstico"
2. **Dashboard** → Tarjeta "Diagnóstico Rápido"
3. **Menú Hamburguesa** (móvil) → "Diagnóstico"
4. **Búsqueda** → Escribe "diagnóstico"

**Si aún no lo encuentras**:
- Verifica que tu sesión esté iniciada
- Limpia caché del navegador
- Prueba en modo incógnito
- Contacta soporte técnico

---

### 🌐 Problema: "El diagnóstico tarda mucho en cargar"

**Soluciones**:

1. **Verifica tu conexión a internet**
   - Mínimo recomendado: 2 Mbps

2. **Cierra pestañas innecesarias**
   - Libera recursos del navegador

3. **Actualiza la página**
   - Presiona F5 o Ctrl+R

4. **Prueba otro navegador**
   - Recomendados: Chrome, Firefox, Edge

5. **Reporta el problema**
   - Si persiste después de 30 segundos

---

## 💡 Tips y Trucos

### 1️⃣ Usa Búsqueda Avanzada

**Filtrar por Sistema**:
```
Historial → Filtros → Sistema: Motor
→ Muestra solo problemas de motor
```

**Filtrar por Urgencia**:
```
Historial → Filtros → Urgencia: Alta (3-4)
→ Muestra solo problemas graves
```

**Filtrar por Fecha**:
```
Historial → Filtros → Rango: Último mes
→ Muestra consultas recientes
```

**Ordenar Resultados**:
```
Historial → Ordenar por: Fecha descendente
→ Más recientes primero
```

---

### 2️⃣ Aprende del Historial

**Identificar Patrones**:
- Revisa consultas de los últimos 6 meses
- Busca síntomas que se repiten
- Anota fechas de mantenimiento

**Crear Calendario de Mantenimiento**:
1. Exporta historial a Excel
2. Agrega columna "Próximo Mantenimiento"
3. Calcula fechas según recomendaciones
4. Configura recordatorios

**Ejemplo**:
```
Fecha       | Síntoma       | Mantenimiento | Próximo
------------|---------------|---------------|----------
01/05/2025  | Aceite bajo   | Cambio aceite | 01/08/2025
15/07/2025  | Filtro sucio  | Cambio filtro | 15/01/2026
```

---

### 3️⃣ Mantén Notas Personalizadas

**Agregar Notas a Consultas**:
1. Abre una consulta del historial
2. Haz clic en "Agregar Nota"
3. Escribe observaciones:
   - "Llevé a Taller XYZ, cobró $150"
   - "Problema resuelto temporalmente"
   - "Mecánico sugirió cambiar pieza Y"

**Crear Perfil de Vehículo**:
```
Mi Vehículo → Información
- Marca: Toyota
- Modelo: Corolla
- Año: 2018
- Kilometraje actual: 45,000 km
- Último servicio: 15/09/2025
- Próximo servicio: 15/12/2025
```

---

### 4️⃣ Comparte con Tu Mecánico de Confianza

**Establecer Mecánico Favorito**:
1. Ve a Configuración → Talleres
2. Haz clic en "Agregar Taller de Confianza"
3. Llena información:
   - Nombre del taller
   - Email del mecánico
   - Teléfono
   - Dirección

**Compartir Automáticamente**:
- Activa "Compartir diagnósticos con mi taller"
- El mecánico recibe notificación por email
- Acelera proceso de reparación

---

### 5️⃣ Configura Alertas Inteligentes

**Alertas por Urgencia**:
```
Configuración → Notificaciones
☑️ Alertarme si nivel de urgencia es 3 o 4
☑️ Enviar resumen semanal de consultas
☐ Recordatorios de mantenimiento
```

**Alertas por Kilometraje**:
```
Mi Vehículo → Alertas
☑️ Avisar cada 1,000 km para actualizar kilometraje
☑️ Recordar mantenimiento según km recorridos
```

---

## 📞 Contacto y Soporte

### 🆘 ¿Necesitas Ayuda?

**📧 Email de Soporte**
- Dirección: soporte@Rodavia.com
- Tiempo de respuesta: 24-48 horas
- Horario: 24/7

**💬 Chat en Vivo**
- Disponible: Lunes a Viernes, 9 AM - 6 PM (hora local)
- Respuesta promedio: 2-5 minutos
- Ubicación: Esquina inferior derecha de la pantalla

**📱 WhatsApp**
- Número: +56 9 XXXX XXXX (próximamente)
- Horario: Lunes a Viernes, 9 AM - 6 PM

**🌐 Centro de Ayuda**
- URL: https://Rodavia.com/ayuda
- FAQs, tutoriales en video, guías paso a paso

**👥 Comunidad de Usuarios**
- Foro: https://comunidad.Rodavia.com
- Comparte experiencias, haz preguntas, ayuda a otros

---

### 🐛 Reportar Problemas

**Si encuentras un error o diagnóstico incorrecto**:

1. **Desde el diagnóstico**:
   - Haz clic en "⋮" (menú)
   - Selecciona "Reportar Problema"
   - Llena el formulario:
     ```
     Tipo de problema:
     □ Síntoma mal identificado
     □ Causa incorrecta
     □ Pasos no funcionan
     □ Error técnico
     
     Descripción detallada:
     [Explica qué salió mal]
     
     Información del vehículo:
     - Marca/Modelo: __________
     - Año: __________
     - Kilometraje: __________
     ```

2. **Por email**:
   - Envía a: bugs@Rodavia.com
   - Incluye capturas de pantalla si es posible
   - Detalla pasos para reproducir el error

**Recibirás**:
- Confirmación en menos de 24 horas
- Actualización del ticket en 48-72 horas
- Notificación cuando el problema esté resuelto

---

### 📚 Recursos Adicionales

**🎓 Tutoriales en Video**
- YouTube: @RodaviaOficial
- Videos de 3-5 minutos
- Cómo usar cada función

**📖 Guías Descargables (PDF)**
- Guía de Inicio Rápido
- Guía de Mantenimiento Preventivo
- Glosario de Términos Automotrices

**📰 Blog de AutoGuía**
- Tips de mantenimiento
- Casos de estudio reales
- Noticias automotrices

---

## ⚖️ Términos de Uso

### 📜 Descargo de Responsabilidad

AutoGuía proporciona el Asistente de Diagnóstico como:

✅ **Herramienta educativa** para entender problemas automotrices  
✅ **Referencia inicial** antes de consultar profesional  
✅ **Guía informativa** sobre mantenimiento preventivo

❌ **NO es**:
- ❌ Reemplazo de mecánico certificado
- ❌ Garantía de diagnóstico 100% preciso
- ❌ Asesoramiento profesional vinculante
- ❌ Responsable de reparaciones fallidas

### 🛡️ Limitaciones de Responsabilidad

- El **usuario es responsable** de cualquier acción tomada basándose en diagnósticos
- AutoGuía **no se hace responsable** por daños resultantes de seguir recomendaciones
- Los diagnósticos son **mejor esfuerzo**, no garantizados
- Para problemas críticos o de seguridad, **siempre consulte profesional**

### 🔐 Privacidad

- Datos personales protegidos según GDPR y CCPA
- No vendemos información a terceros
- Cookies solo para funcionalidad esencial
- Derecho a eliminar datos en cualquier momento

Para detalles completos, consulta:
- [Términos y Condiciones Completos](https://Rodavia.com/terminos)
- [Política de Privacidad](https://Rodavia.com/privacidad)

---

## 📄 Información del Documento

**Versión**: 1.0  
**Última actualización**: 23 de octubre de 2025  
**Idioma**: Español (LATAM)  
**Formato**: Markdown  
**Licencia**: © 2025 AutoGuía - Todos los derechos reservados

---

## 🚀 ¡Comienza Ahora!

**¿Listo para diagnosticar tu vehículo?**

1. ✅ [Inicia sesión en AutoGuía](https://Rodavia.com/login)
2. 🔍 [Accede al Asistente de Diagnóstico](https://Rodavia.com/diagnostico)
3. 📝 Describe tu síntoma
4. 🎯 Recibe diagnóstico instantáneo

**¿Necesitas ayuda?** Contáctanos en soporte@Rodavia.com

---

**¡Gracias por confiar en AutoGuía!** 🚗💙
