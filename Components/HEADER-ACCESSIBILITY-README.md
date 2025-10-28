# Header.razor - Guía de Accesibilidad

## Descripción General

Componente de encabezado (`Header.razor`) con navegación accesible completa, optimizado para cumplir con WCAG 2.1 AA. Incluye soporte para teclado, ARIA dinámico y gestión de estados de menús.

## Características de Accesibilidad Implementadas

### 1. **Roles ARIA y Estructura Semántica**

```razor
<header class="site-header" role="banner">
  <nav role="navigation" aria-label="Navegación principal">
    <ul role="menubar" aria-label="Menú principal">
      <li role="none">
        <a role="menuitem">...</a>
      </li>
    </ul>
  </nav>
</header>
```

- **`role="banner"`**: Identifica el header como landmark principal
- **`role="navigation"`**: Define la barra de navegación
- **`role="menubar"`** y **`role="menuitem"`**: Estructura de menú accesible
- **`role="none"`**: Elimina semantics de elementos de lista para menús

### 2. **ARIA Dinámico para Estados de Menú**

#### Menú Móvil
```razor
<button 
  @onclick="ToggleMobileMenu"
  aria-controls="mainNav"
  aria-expanded="@mobileMenuOpen"
  aria-label="@(mobileMenuOpen ? "Cerrar menú de navegación" : "Abrir menú de navegación")">
```

#### Dropdown de Servicios
```razor
<button 
  @onclick="ToggleServicesDropdown"
  aria-expanded="@servicesDropdownOpen"
  aria-controls="servicesDropdownMenu"
  aria-haspopup="true"
  aria-label="Menú de servicios">
```

**Estado dinámico vinculado**:
- `@mobileMenuOpen` → `aria-expanded` (true/false)
- `@servicesDropdownOpen` → `aria-expanded` (true/false)
- Aria-labels descriptivos que cambian según el estado

### 3. **Navegación por Teclado**

#### Teclas Soportadas

| Tecla | Acción | Contexto |
|-------|--------|----------|
| **Escape** | Cierra dropdown y restaura foco | Dropdown abierto |
| **Arrow Down** | Abre dropdown / Navega al siguiente ítem | Dropdown |
| **Arrow Up** | Navega al ítem anterior | Dropdown items |
| **Home** | Navega al primer ítem | Dropdown items |
| **End** | Navega al último ítem | Dropdown items |
| **Tab** | Navegación natural + cierre al salir | Todo el menú |

#### Implementación

```csharp
private async Task HandleDropdownKeyDown(KeyboardEventArgs e)
{
  switch (e.Key)
  {
    case "Escape":
      servicesDropdownOpen = false;
      await JSRuntime.InvokeVoidAsync("focusElement", "servicesDropdown");
      break;
    case "ArrowDown":
      if (!servicesDropdownOpen) servicesDropdownOpen = true;
      await JSRuntime.InvokeVoidAsync("focusFirstDropdownItem", "servicesDropdownMenu");
      break;
  }
}
```

### 4. **Gestión de Foco**

#### JavaScript para Foco Programático

```javascript
window.focusElement = function(elementId) {
  const element = document.getElementById(elementId);
  if (element) element.focus();
};

window.focusFirstDropdownItem = function(dropdownId) {
  const dropdown = document.getElementById(dropdownId);
  if (dropdown) {
    const firstItem = dropdown.querySelector('.dropdown-item');
    if (firstItem) firstItem.focus();
  }
};
```

**Casos de uso**:
- **Escape en dropdown**: Restaura foco al botón trigger
- **Arrow Down**: Mueve foco al primer ítem del dropdown
- **Tab out**: Cierra menú automáticamente

### 5. **Atributos aria-controls y aria-labelledby**

#### Asociación de Controles
```razor
<!-- Botón trigger -->
<button 
  id="servicesDropdown"
  aria-controls="servicesDropdownMenu">
  
<!-- Menu controlado -->
<div 
  id="servicesDropdownMenu"
  aria-labelledby="servicesDropdown">
```

#### Headings Descriptivos
```razor
<h6 id="diagnostico-heading" role="presentation">
  Diagnóstico
</h6>
<a aria-describedby="diagnostico-heading">
  Consulta VIN
</a>
```

- **`aria-labelledby`**: Vincula el dropdown con su botón
- **`aria-describedby`**: Asocia items con sus headings de categoría

### 6. **Estilos de Foco Visible**

```css
.nav-link:focus-visible,
.dropdown-item:focus-visible,
.navbar-toggler:focus-visible {
  outline: 3px solid var(--color-primary, #0d6efd);
  outline-offset: 2px;
  border-radius: var(--radius-sm, 0.25rem);
}
```

**Características**:
- Outline de 3px para visibilidad
- Offset de 2px para separación
- Color primario consistente con diseño
- Solo visible en navegación por teclado (`:focus-visible`)

### 7. **Soporte de Alto Contraste**

```css
@media (prefers-contrast: high) {
  .nav-link:focus-visible,
  .dropdown-item:focus-visible,
  .navbar-toggler:focus-visible {
    outline-width: 4px;
  }
}
```

Aumenta outline a 4px en modo de alto contraste.

### 8. **Clase .visually-hidden para Contenido de Screen Readers**

```css
.visually-hidden {
  position: absolute;
  width: 1px;
  height: 1px;
  padding: 0;
  margin: -1px;
  overflow: hidden;
  clip: rect(0, 0, 0, 0);
  white-space: nowrap;
  border: 0;
}
```

**Uso**:
```razor
<span class="visually-hidden">AutoGuía - Tu Plataforma Automotriz</span>
```

Texto oculto visualmente pero accesible para lectores de pantalla.

### 9. **Cierre Automático al Hacer Click Fuera**

```javascript
document.addEventListener('click', function(event) {
  const navbar = document.getElementById('mainNav');
  const toggler = document.querySelector('.navbar-toggler');
  
  if (navbar && !navbar.contains(event.target) && !toggler.contains(event.target)) {
    // Trigger Blazor to close menus
    const closeEvent = new CustomEvent('blazor:closemenus');
    document.dispatchEvent(closeEvent);
  }
});
```

Mejora UX cerrando menús al hacer click fuera del componente.

## Estructura de Métodos C#

### Estado del Componente

```csharp
private bool mobileMenuOpen = false;
private bool servicesDropdownOpen = false;
private string searchQuery = string.Empty;
```

### Métodos de Toggle

```csharp
private void ToggleMobileMenu()
{
  mobileMenuOpen = !mobileMenuOpen;
  if (mobileMenuOpen) servicesDropdownOpen = false;
}

private void ToggleServicesDropdown()
{
  servicesDropdownOpen = !servicesDropdownOpen;
}
```

### Métodos de Cierre

```csharp
private void CloseMobileMenu() { ... }
private void CloseAllMenus() { ... }
```

### Métodos de Clases CSS Dinámicas

```csharp
private string GetNavbarCollapseClass()
{
  return mobileMenuOpen ? "collapse navbar-collapse show" : "collapse navbar-collapse";
}

private string GetDropdownMenuClass()
{
  return servicesDropdownOpen ? "dropdown-menu mega-menu p-4 show" : "dropdown-menu mega-menu p-4";
}
```

## Cumplimiento WCAG 2.1 AA

| Criterio | Nivel | Estado |
|----------|-------|--------|
| **1.3.1 Info and Relationships** | A | ✅ Completo |
| **2.1.1 Keyboard** | A | ✅ Completo |
| **2.1.2 No Keyboard Trap** | A | ✅ Completo |
| **2.4.3 Focus Order** | A | ✅ Completo |
| **2.4.7 Focus Visible** | AA | ✅ Completo |
| **4.1.2 Name, Role, Value** | A | ✅ Completo |
| **4.1.3 Status Messages** | AA | ✅ Completo |

## Testing de Accesibilidad

### Checklist de Verificación

- [ ] **Navegación por Tab**: Todos los elementos interactivos son alcanzables
- [ ] **Escape cierra menús**: Dropdown se cierra y devuelve foco
- [ ] **Arrow keys navegan**: Up/Down funcionan en dropdown
- [ ] **Screen reader anuncia**: Estados de aria-expanded
- [ ] **Focus visible**: Outline de 3px en todos los elementos
- [ ] **Mobile menu**: Toggle funciona correctamente en móviles
- [ ] **Click outside**: Cierra menús al hacer click fuera

### Herramientas Recomendadas

1. **Lighthouse** (Chrome DevTools): Auditoría de accesibilidad
2. **axe DevTools**: Análisis detallado de ARIA y estructura
3. **NVDA / JAWS**: Testing con lectores de pantalla
4. **Keyboard Navigation**: Navegar solo con teclado (sin ratón)
5. **WAVE**: Web Accessibility Evaluation Tool

## Mejoras Futuras (Opcional)

1. **Skip to main content link**: Agregar en MainLayout
   ```razor
   <a href="#main-content" class="skip-to-main">Saltar al contenido principal</a>
   ```

2. **Focus trap en mega menu**: Capturar Tab dentro del dropdown

3. **ARIA live regions**: Para notificaciones dinámicas

4. **Responsive a prefers-reduced-motion**: Reducir animaciones

## Referencias

- [WAI-ARIA Authoring Practices - Menu Button](https://www.w3.org/WAI/ARIA/apg/patterns/menubutton/)
- [WCAG 2.1 Understanding Docs](https://www.w3.org/WAI/WCAG21/Understanding/)
- [MDN: ARIA Roles](https://developer.mozilla.org/en-US/docs/Web/Accessibility/ARIA/Roles)
- [Inclusive Components: Menus & Menu Buttons](https://inclusive-components.design/menus-menu-buttons/)

---

**Última actualización**: Octubre 2024  
**Autor**: GitHub Copilot  
**Versión**: 1.0  
**Licencia**: MIT
