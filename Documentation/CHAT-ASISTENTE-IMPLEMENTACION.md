# Chat Asistente de Diagnóstico - AutoGuía

## 🎯 Descripción

Componente de chat flotante que permite a los usuarios interactuar con el asistente de diagnóstico automotriz desde cualquier página de la aplicación.

---

## ✨ Características Implementadas

### 1. Botón Flotante
- **Ubicación**: Esquina inferior derecha
- **Diseño**: Botón circular con icono de chat
- **Animación**: Efecto pulse para llamar la atención
- **Notificaciones**: Badge con contador de mensajes no leídos

### 2. Ventana de Chat
- **Tamaño**: 380px × 550px (responsive)
- **Animación**: Slide-up al abrir
- **Header**: 
  - Icono de robot
  - Título "Asistente AutoGuía"
  - Subtítulo "Diagnóstico vehicular"
  - Botón de cierre

### 3. Mensajes
- **Mensaje de Bienvenida**:
  - Saludo personalizado
  - Instrucciones de uso
  - 3 sugerencias rápidas predefinidas

- **Tipos de Mensajes**:
  - Usuario: Fondo degradado púrpura, alineado a la derecha
  - Asistente: Fondo blanco, alineado a la izquierda
  - Timestamp en cada mensaje

- **Indicador de Escritura**: 3 puntos animados mientras procesa

### 4. Input de Texto
- **Textarea**: 2 filas, autoajustable
- **Placeholder**: "Describe el problema de tu vehículo..."
- **Atajos de Teclado**: Enter para enviar, Shift+Enter para nueva línea
- **Botón Enviar**: Icono de avión de papel, deshabilitado mientras procesa

### 5. Integración con Servicio
- Conectado a `IDiagnosticoService`
- Procesa síntomas en tiempo real
- Muestra causas posibles con probabilidad
- Indica nivel de urgencia con emojis
- Recomienda servicio profesional cuando es necesario

---

## 🎨 Diseño Visual

### Paleta de Colores
```css
Degradado Principal: linear-gradient(135deg, #667eea 0%, #764ba2 100%)
Fondo Chat: #f8f9fa
Mensajes Usuario: Degradado púrpura
Mensajes Asistente: Blanco con borde #e9ecef
```

### Iconografía (Font Awesome)
- `fa-comments`: Botón flotante
- `fa-robot`: Avatar del asistente
- `fa-user`: Avatar del usuario
- `fa-times`: Botón cerrar
- `fa-paper-plane`: Botón enviar
- `fa-hand-wave`: Bienvenida

### Animaciones
- **pulse**: Botón flotante (2s infinite)
- **bounce**: Badge de notificaciones (0.5s)
- **slideUp**: Ventana de chat (0.3s)
- **fadeIn**: Mensajes individuales (0.3s)
- **typing**: Indicador de escritura (1.4s infinite)

---

## 📁 Archivos Creados

### 1. Componente Blazor
**Ruta**: `Rodavia.Web/Rodavia.Web/Components/Shared/ChatAsistente.razor`

**Responsabilidades**:
- Gestión de estado del chat (abierto/cerrado)
- Envío y recepción de mensajes
- Integración con servicio de diagnóstico
- Formateo de respuestas

### 2. Estilos CSS
**Ruta**: `Rodavia.Web/Rodavia.Web/wwwroot/css/chat-asistente.css`

**Características**:
- Estilos completos del chat
- Animaciones y transiciones
- Responsive design
- Scrollbar personalizado

---

## 🔧 Integración

### MainLayout.razor
```razor
@using Rodavia.Web.Components.Shared

<!-- Al final del archivo -->
<ChatAsistente />
```

### App.razor
```html
<link rel="stylesheet" href="css/chat-asistente.css" />
```

---

## 💬 Flujo de Conversación

```
1. Usuario abre chat → Mensaje de bienvenida + 3 sugerencias
2. Usuario escribe síntoma O hace clic en sugerencia
3. Envía mensaje → Indicador de escritura aparece
4. Servicio procesa síntoma
5. Asistente responde con:
   - Síntoma identificado
   - Nivel de urgencia (emoji + texto)
   - Top 3 causas posibles (con estrellas de probabilidad)
   - Recomendación
   - Alerta si requiere servicio profesional
```

---

## 🎯 Formato de Respuesta

### Ejemplo de Respuesta del Asistente

```
🔍 Síntoma identificado: Ruido metálico al frenar

🟡 Urgencia: Moderado

💡 Causas posibles:
• Pastillas de freno desgastadas ⭐⭐⭐⭐⭐
  ⚠️ Requiere servicio profesional
• Discos de freno rayados ⭐⭐⭐⭐
• Suciedad en sistema de frenos ⭐⭐⭐

📋 Recomendación:
Te sugerimos revisar el sistema de frenos lo antes posible...

⚠️ Recomendación importante: Te sugerimos acudir con un 
profesional para una evaluación completa.
```

---

## 📱 Responsive Design

### Desktop (> 480px)
- Ventana: 380px × 550px
- Botón flotante: 60px × 60px
- Posición: 20px desde abajo y derecha

### Mobile (≤ 480px)
- Ventana: calc(100vw - 40px) × calc(100vh - 100px)
- Máximo: 600px de altura
- Botón flotante: 50px × 50px

---

## 🚀 Características Futuras (Roadmap)

### Fase 2
- [ ] Historial de conversaciones persistente
- [ ] Scroll automático con JavaScript Interop
- [ ] Exportar conversación a PDF
- [ ] Compartir diagnóstico por email

### Fase 3
- [ ] Envío de imágenes del problema
- [ ] Audio/Voz para describir síntomas
- [ ] Búsqueda de talleres cercanos desde chat
- [ ] Cotización instantánea de reparación

### Fase 4
- [ ] IA mejorada con machine learning
- [ ] Predicción basada en historial del vehículo
- [ ] Integración con IoT (OBD-II)
- [ ] Chatbot multilingüe

---

## 🧪 Testing

### Tests Pendientes
- [ ] Unit tests para lógica de mensajes
- [ ] Integration tests con servicio de diagnóstico
- [ ] UI tests con bUnit
- [ ] Tests de accesibilidad (ARIA)

---

## 🔐 Seguridad y Privacidad

### Implementado
- ID de usuario desde claims (autenticación)
- No se almacenan datos sensibles del vehículo

### Por Implementar
- [ ] Cifrado de mensajes en tránsito
- [ ] Opción de borrar historial
- [ ] Consentimiento GDPR

---

## 📊 Métricas y Analytics

### Datos Recolectados
- Consultas realizadas
- Síntomas más buscados
- Feedback de utilidad (por implementar)
- Tiempo promedio de respuesta

---

## 🎓 Uso para Desarrolladores

### Agregar Nueva Sugerencia Rápida

```razor
<button class="btn-sugerencia" @onclick="@(() => UsarSugerencia("Nuevo síntoma"))">
    Texto del botón
</button>
```

### Personalizar Formato de Respuesta

Editar método `FormatearRespuestaDiagnostico()` en `ChatAsistente.razor`

### Cambiar Colores del Chat

Editar variables CSS en `chat-asistente.css`:
```css
background: linear-gradient(135deg, #TU_COLOR_1 0%, #TU_COLOR_2 100%);
```

---

## ✅ Checklist de Implementación

- [x] Crear componente ChatAsistente.razor
- [x] Crear estilos chat-asistente.css
- [x] Integrar en MainLayout.razor
- [x] Agregar referencia CSS en App.razor
- [x] Conectar con IDiagnosticoService
- [x] Implementar envío de mensajes
- [x] Implementar recepción de respuestas
- [x] Formatear respuestas del asistente
- [x] Agregar sugerencias rápidas
- [x] Indicador de escritura
- [x] Animaciones y transiciones
- [x] Responsive design
- [x] Compilación exitosa

---

## 🐛 Bugs Conocidos

- Ninguno reportado hasta el momento

---

## 📞 Soporte

Para preguntas o mejoras, contactar al equipo de desarrollo de AutoGuía.

**Versión**: 1.0  
**Fecha de creación**: 23 de octubre de 2025  
**Autor**: AutoGuía Development Team
