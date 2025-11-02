# Service Icons - AutoGuía

Set de 6 iconos vectoriales flat para servicios automotrices.

## 📦 Archivos Incluidos

| Archivo | Descripción | Uso |
|---------|-------------|-----|
| `icon-warranty.svg` | Garantía - Escudo con checkmark | Sección de garantías, políticas de devolución |
| `icon-support.svg` | Soporte - Headset con micrófono | Atención al cliente, soporte técnico 24/7 |
| `icon-delivery.svg` | Entrega - Camión de delivery | Envíos, logística, seguimiento de pedidos |
| `icon-finance.svg` | Financiación - Tarjeta con símbolo % | Opciones de pago, financiamiento, créditos |
| `icon-accessories.svg` | Accesorios - Bolsa de compras con estrella | Accesorios premium, complementos |
| `icon-parts.svg` | Repuestos - Engranaje con herramienta | Repuestos originales, piezas de recambio |

## 🎨 Especificaciones Técnicas

- **Formato**: SVG vectorial
- **Tamaño**: 64x64px viewBox
- **Colores**:
  - Primary: `#0d6efd` (blue) - Strokes y acentos principales
  - Dark: `#212529` (black) - Detalles sólidos
  - Light: `#f8f9fa` (white/light gray) - Fondos opcionales
- **Stroke**: 2.5px consistente (líneas principales)
- **Line caps**: Round (terminaciones redondeadas)
- **Line joins**: Round (uniones suaves)
- **Opacidad fill**: 0.1 para fondos (efecto glassmorphism)

## 💻 Uso en HTML

### Básico (img tag)
```html
<img src="/images/icons/icon-warranty.svg" alt="Garantía" width="48" height="48" />
```

### Inline SVG (personalizable)
```html
<svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" viewBox="0 0 64 64">
  <!-- Copiar contenido del archivo SVG -->
</svg>
```

### Con CSS personalizado
```html
<div class="service-icon">
  <img src="/images/icons/icon-support.svg" alt="Soporte 24/7" />
</div>

<style>
.service-icon img {
  width: 64px;
  height: 64px;
  filter: drop-shadow(0 2px 8px rgba(13, 110, 253, 0.2));
  transition: transform 0.2s;
}
.service-icon img:hover {
  transform: scale(1.1);
}
</style>
```

## 🎯 Ejemplos de Uso en Blazor

### Componente Card con icono
```razor
<div class="service-card">
  <img src="/images/icons/icon-warranty.svg" alt="Garantía extendida" class="service-icon" />
  <h3>Garantía Extendida</h3>
  <p>Hasta 3 años de cobertura en repuestos originales</p>
</div>
```

### Grid de servicios
```razor
<div class="row g-4">
  @foreach (var service in services)
  {
    <div class="col-md-4">
      <div class="text-center p-4 border rounded">
        <img src="/images/icons/@service.Icon" alt="@service.Name" width="64" height="64" />
        <h4 class="mt-3">@service.Name</h4>
        <p class="text-muted">@service.Description</p>
      </div>
    </div>
  }
</div>

@code {
  private List<Service> services = new()
  {
    new() { Icon = "icon-warranty.svg", Name = "Garantía", Description = "Hasta 3 años" },
    new() { Icon = "icon-delivery.svg", Name = "Envío Gratis", Description = "Pedidos +$50.000" },
    new() { Icon = "icon-finance.svg", Name = "Financiamiento", Description = "Hasta 12 cuotas" }
  };
  
  class Service { public string Icon { get; set; } public string Name { get; set; } public string Description { get; set; } }
}
```

## 🔧 Personalización

### Cambiar color del icono (CSS Filter)
```css
/* Rojo */
.icon-red {
  filter: hue-rotate(220deg) saturate(1.5);
}

/* Verde */
.icon-green {
  filter: hue-rotate(80deg) saturate(1.2);
}

/* Gris */
.icon-gray {
  filter: grayscale(1);
}
```

### Animaciones
```css
@keyframes pulse {
  0%, 100% { transform: scale(1); }
  50% { transform: scale(1.05); }
}

.icon-animated {
  animation: pulse 2s ease-in-out infinite;
}
```

## 📱 Responsive Sizing

```css
.service-icon {
  width: 48px;
  height: 48px;
}

@media (min-width: 768px) {
  .service-icon {
    width: 64px;
    height: 64px;
  }
}

@media (min-width: 1200px) {
  .service-icon {
    width: 80px;
    height: 80px;
  }
}
```

## ♿ Accesibilidad

- ✅ Siempre incluir atributo `alt` descriptivo
- ✅ Usar `role="img"` si es inline SVG decorativo
- ✅ Agregar `aria-hidden="true"` si hay texto explicativo cercano
- ✅ Mantener contraste mínimo 4.5:1 (color vs background)

```html
<!-- Icono decorativo con texto -->
<div>
  <img src="/images/icons/icon-warranty.svg" alt="" aria-hidden="true" />
  <span>Garantía extendida disponible</span>
</div>

<!-- Icono funcional sin texto -->
<button>
  <img src="/images/icons/icon-support.svg" alt="Contactar soporte" />
</button>
```

## 🎨 Integración con Design System

Estos iconos están diseñados para complementar:
- Logo AutoGuía (mismo color primary #0d6efd)
- Bootstrap 5 utilities
- Hero banner y componentes existentes
- Avatar system (consistencia visual)

## 📦 Optimización

- **Tamaño archivo**: ~1-2KB por icono
- **Compresión**: Ya optimizados (sin espacios innecesarios)
- **Carga**: Usar sprite SVG para múltiples iconos en una página:

```html
<svg style="display: none;">
  <symbol id="icon-warranty" viewBox="0 0 64 64">
    <!-- Contenido del icono -->
  </symbol>
  <!-- Más símbolos... -->
</svg>

<!-- Uso -->
<svg width="48" height="48"><use href="#icon-warranty"/></svg>
```

## 🚀 Próximas Extensiones

Para agregar más iconos al set:
1. Mantener viewBox 64x64
2. Usar stroke-width: 2.5px
3. Aplicar colores del sistema (#0d6efd primary)
4. Seguir estilo flat con fills opacity: 0.1
5. Incluir acentos mínimos (círculos, líneas decorativas)

---

**Creado**: Octubre 2025  
**Versión**: 1.0  
**Compatibilidad**: Todos los navegadores modernos (SVG 1.1)
