# 🎨 Guía de Diseño Minimalista y Moderno - Rodavia

## Color Palette
- **Primary**: #0078D4 (Azul vibrante)
- **Dark Blue**: #1a1f3a, #2d3561 (Fondos navbar)
- **Light Gray**: #f8f9fa (Fondos claros)
- **Text Primary**: #1a1f3a (Gris oscuro)
- **Text Secondary**: #666666 (Gris medio)
- **Success**: #28a745 (Verde)
- **Warning**: #ffc107 (Amarillo)
- **Danger**: #dc3545 (Rojo)

## Componentes Reutilizables

### Header/Título de Página
```html
<div class="page-header">
    <h1 class="page-title">
        <i class="fas fa-icon me-2"></i>Título de Página
    </h1>
    <p class="page-subtitle">Descripción breve</p>
</div>
```

**CSS:**
```css
.page-header {
    background: linear-gradient(135deg, #f8f9fa 0%, #ffffff 100%);
    padding: 2rem;
    border-radius: 12px;
    margin-bottom: 2rem;
    border-left: 4px solid #0078D4;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
}

.page-title {
    font-size: 2rem;
    font-weight: 700;
    color: #0078D4;
    margin: 0;
    letter-spacing: -0.5px;
}

.page-subtitle {
    font-size: 1rem;
    color: #666;
    margin-top: 0.5rem;
    margin-bottom: 0;
}
```

### Cards Minimalistas
```html
<div class="card-minimal">
    <div class="card-minimal-header">
        <h3 class="card-minimal-title">
            <i class="fas fa-icon"></i>Título
        </h3>
    </div>
    <div class="card-minimal-body">
        <!-- Contenido -->
    </div>
</div>
```

**CSS:**
```css
.card-minimal {
    background: #ffffff;
    border: 1px solid #e8e8e8;
    border-radius: 10px;
    overflow: hidden;
    transition: all 0.3s ease;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
}

.card-minimal:hover {
    box-shadow: 0 8px 16px rgba(0, 0, 0, 0.1);
    transform: translateY(-4px);
}

.card-minimal-header {
    padding: 1.5rem;
    border-bottom: 1px solid #f0f0f0;
    background: linear-gradient(135deg, #f8f9fa 0%, #ffffff 100%);
}

.card-minimal-title {
    font-size: 1.2rem;
    font-weight: 600;
    color: #1a1f3a;
    margin: 0;
    display: flex;
    align-items: center;
    gap: 0.75rem;
}

.card-minimal-body {
    padding: 1.5rem;
}
```

### Botones Modernos
```html
<button class="btn-modern btn-primary">Primario</button>
<button class="btn-modern btn-secondary">Secundario</button>
<button class="btn-modern btn-outline">Outline</button>
```

**CSS:**
```css
.btn-modern {
    padding: 0.75rem 1.5rem;
    border: none;
    border-radius: 8px;
    font-weight: 600;
    font-size: 1rem;
    cursor: pointer;
    transition: all 0.3s ease;
    display: inline-flex;
    align-items: center;
    gap: 0.5rem;
    white-space: nowrap;
}

.btn-primary {
    background: linear-gradient(135deg, #0078D4 0%, #005a96 100%);
    color: #ffffff;
    box-shadow: 0 4px 12px rgba(0, 120, 212, 0.3);
}

.btn-primary:hover {
    transform: translateY(-2px);
    box-shadow: 0 6px 16px rgba(0, 120, 212, 0.4);
}

.btn-secondary {
    background: #f0f0f0;
    color: #1a1f3a;
}

.btn-secondary:hover {
    background: #e0e0e0;
    transform: translateY(-2px);
}

.btn-outline {
    background: transparent;
    color: #0078D4;
    border: 2px solid #0078D4;
}

.btn-outline:hover {
    background: rgba(0, 120, 212, 0.1);
    transform: translateY(-2px);
}
```

### Inputs & Forms
```html
<div class="form-group">
    <label class="form-label">Etiqueta</label>
    <input type="text" class="form-input" placeholder="Placeholder">
</div>
```

**CSS:**
```css
.form-group {
    margin-bottom: 1.5rem;
}

.form-label {
    display: block;
    font-weight: 600;
    color: #1a1f3a;
    margin-bottom: 0.5rem;
    font-size: 0.95rem;
}

.form-input {
    width: 100%;
    padding: 0.75rem 1rem;
    border: 1px solid #ddd;
    border-radius: 8px;
    font-size: 1rem;
    transition: all 0.3s ease;
    font-family: inherit;
}

.form-input:focus {
    outline: none;
    border-color: #0078D4;
    box-shadow: 0 0 0 3px rgba(0, 120, 212, 0.1);
}
```

### Badge & Status
```html
<span class="badge-modern badge-success">Activo</span>
<span class="badge-modern badge-warning">Pendiente</span>
```

**CSS:**
```css
.badge-modern {
    display: inline-flex;
    align-items: center;
    gap: 0.4rem;
    padding: 0.4rem 0.8rem;
    border-radius: 20px;
    font-size: 0.85rem;
    font-weight: 600;
}

.badge-success {
    background: rgba(40, 167, 69, 0.15);
    color: #28a745;
}

.badge-warning {
    background: rgba(255, 193, 7, 0.15);
    color: #ffc107;
}
```

## Páginas por Modernizar

### 1. Talleres
- [ ] Header con filtros modernos
- [ ] Grid de cards minimalistas
- [ ] Mapa integrado con borde moderno
- [ ] Filtros estilizados

### 2. Productos/Consumibles
- [ ] Header con búsqueda moderna
- [ ] Grid de productos con cards
- [ ] Filtros por categoría
- [ ] Precios destacados

### 3. Comunidad/Foro
- [ ] Header con botón "Nueva Publicación"
- [ ] Cards de publicaciones minimalistas
- [ ] Sistema de likes/comentarios
- [ ] Avatares modernos

### 4. IA Diagnóstico
- [ ] Chat moderno y limpio
- [ ] Input de mensajes estilizado
- [ ] Respuestas con formato claro
- [ ] Histórico de consultas

### 5. Registrarse
- [ ] Formulario limpio y centrado
- [ ] Validaciones en tiempo real
- [ ] Botón de submit moderno
- [ ] Link a login

### 6. Iniciar Sesión
- [ ] Formulario simétrico
- [ ] Recuperar contraseña link
- [ ] Botón de login moderno
- [ ] Link a registro

## Tipografía
- **Headings**: Roboto Bold (700)
- **Body**: Roboto Regular (400-500)
- **Sizes**: 
  - H1: 2rem
  - H2: 1.5rem
  - H3: 1.2rem
  - Body: 1rem
  - Small: 0.85rem

## Espaciado
- **Pequeño**: 0.5rem (8px)
- **Medio**: 1rem (16px)
- **Grande**: 1.5rem (24px)
- **Extra**: 2rem (32px)

## Efectos
- **Hover**: translateY(-2px), shadow increase
- **Focus**: Outline con color primario
- **Loading**: Spinner moderno
- **Transiciones**: 0.3s ease

## Responsive
- **Desktop**: 1200px+
- **Tablet**: 768px - 1199px
- **Mobile**: < 768px
