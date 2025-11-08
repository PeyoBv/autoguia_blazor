# ProductCard Component - Guía de Uso

## 📦 Ubicación
`Rodavia.Web/Rodavia.Web/Components/Shared/ProductCard.razor`

## ✨ Características

- ✅ **Accesibilidad WCAG 2.1 AA**
  - ARIA labels descriptivos
  - Roles semánticos (`role="article"`)
  - Focus-visible styles
  - Screen reader support
  - Soporte para prefers-contrast y prefers-reduced-motion

- ✅ **Optimización de Imágenes**
  - Lazy loading nativo (`loading="lazy"`)
  - Fallback SVG si la imagen falla
  - Aspect ratio 4:3 con object-fit
  - Hover effect con scale

- ✅ **UX Mejorada**
  - Loading state con spinner
  - Botón deshabilitado durante acción
  - Animaciones suaves (con respeto a reduced-motion)
  - Hover effects en card e imagen

- ✅ **Responsive**
  - Mobile-first design
  - Ajustes de padding y font-size en tablet+

## 🔧 Parámetros

| Parámetro | Tipo | Requerido | Default | Descripción |
|-----------|------|-----------|---------|-------------|
| `Title` | `string` | ✅ Sí | - | Título del producto |
| `ImageUrl` | `string?` | ❌ No | `null` | URL de la imagen |
| `ImageAlt` | `string?` | ❌ No | `Title` | Texto alternativo (a11y) |
| `ShortDescription` | `string?` | ❌ No | `null` | Descripción corta (3 líneas max) |
| `Price` | `decimal` | ❌ No | `0` | Precio del producto |
| `ButtonText` | `string` | ❌ No | `"Agregar"` | Texto del botón |
| `OnAdd` | `EventCallback` | ❌ No | - | Callback al hacer clic |
| `ShowLoadingState` | `bool` | ❌ No | `true` | Mostrar spinner al agregar |

## 📝 Ejemplos de Uso

### Ejemplo Básico
```razor
@page "/productos"
@using Rodavia.Web.Components.Shared

<div class="card-grid">
  <ProductCard 
    Title="Filtro de Aceite K&N"
    ImageUrl="/images/products/filtro-aceite.jpg"
    ShortDescription="Filtro de alto rendimiento para motores a gasolina"
    Price="15990"
    OnAdd="@HandleAddToCart" />
</div>

@code {
  private async Task HandleAddToCart()
  {
    await Task.Delay(1000); // Simular API call
    // Lógica para agregar al carrito
  }
}
```

### Ejemplo con Grid Responsive
```razor
<div class="card-grid">
  @foreach (var producto in productos)
  {
    <ProductCard 
      Title="@producto.Nombre"
      ImageUrl="@producto.ImagenUrl"
      ImageAlt="@producto.NombreCompleto"
      ShortDescription="@producto.Descripcion"
      Price="@producto.Precio"
      ButtonText="Comprar"
      OnAdd="@(() => AgregarAlCarrito(producto.Id))" />
  }
</div>

@code {
  private List<Producto> productos = new();

  private async Task AgregarAlCarrito(int productoId)
  {
    await CarritoService.AgregarAsync(productoId);
    ToastService.ShowSuccess("Producto agregado al carrito");
  }
}
```

### Ejemplo con Manejo de Errores
```razor
<ProductCard 
  Title="Batería Bosch 12V 75Ah"
  ImageUrl="@(producto.TieneImagen ? producto.ImagenUrl : null)"
  ShortDescription="Batería de alta duración para vehículos livianos"
  Price="89990"
  OnAdd="@HandleAddWithValidation" />

@code {
  private async Task HandleAddWithValidation()
  {
    try
    {
      var resultado = await ProductoService.ValidarDisponibilidadAsync(producto.Id);
      if (resultado.Disponible)
      {
        await CarritoService.AgregarAsync(producto.Id);
        Snackbar.Add("Producto agregado", Severity.Success);
      }
      else
      {
        Snackbar.Add("Producto sin stock", Severity.Warning);
      }
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, "Error al agregar producto");
      Snackbar.Add("Error al procesar", Severity.Error);
    }
  }
}
```

### Ejemplo con Grid de 3 Columnas (Desktop)
```razor
@* CSS ya incluido en site.css *@
<div class="container">
  <h2>Productos Destacados</h2>
  
  <div class="card-grid">
    <ProductCard 
      Title="Aceite Mobil 1 5W-30"
      ImageUrl="/images/products/aceite-mobil.jpg"
      ShortDescription="Aceite sintético de alto rendimiento"
      Price="32990"
      OnAdd="@(() => AddProduct(1))" />
      
    <ProductCard 
      Title="Pastillas de Freno Brembo"
      ImageUrl="/images/products/pastillas-brembo.jpg"
      ShortDescription="Pastillas cerámicas de alta performance"
      Price="45990"
      OnAdd="@(() => AddProduct(2))" />
      
    <ProductCard 
      Title="Filtro de Aire K&N"
      ImageUrl="/images/products/filtro-aire.jpg"
      ShortDescription="Filtro de aire reutilizable de alto flujo"
      Price="28990"
      OnAdd="@(() => AddProduct(3))" />
  </div>
</div>

@code {
  private async Task AddProduct(int id)
  {
    await CartService.AddAsync(id);
  }
}
```

## 🎨 CSS Grid Recomendado

El componente funciona perfectamente con el grid de `site.css`:

```css
.card-grid {
  display: grid;
  grid-template-columns: 1fr;           /* Mobile: 1 columna */
  gap: var(--spacing-lg);
}

@media (min-width: 768px) {
  .card-grid {
    grid-template-columns: repeat(2, 1fr); /* Tablet: 2 columnas */
  }
}

@media (min-width: 1024px) {
  .card-grid {
    grid-template-columns: repeat(3, 1fr); /* Desktop: 3 columnas */
  }
}
```

## ♿ Accesibilidad

### ARIA Labels
```html
<!-- Card wrapper -->
<div role="article" aria-label="Producto: Filtro de Aceite">

<!-- Imagen -->
<img alt="Filtro de Aceite K&N" loading="lazy" />

<!-- Precio -->
<div aria-label="Precio: $15.990">

<!-- Botón -->
<button 
  aria-label="Agregar Filtro de Aceite al carrito por $15.990"
  aria-describedby="product-title-abc123 product-desc-abc123">
```

### Keyboard Navigation
- `Tab` para navegar entre cards
- `Enter` o `Space` para activar botón
- Focus visible con outline de 3px

### Screen Reader
- Estructura semántica con `<h3>` para títulos
- `role="article"` para delimitar cada producto
- `.sr-only` para estados ("Agregando...", "Imagen no disponible")

## 🖼️ Fallback de Imagen

Si `ImageUrl` es null o la carga falla:
```html
<div class="product-card-image-fallback">
  <svg><!-- Icono de imagen rota --></svg>
  <span class="sr-only">Imagen no disponible</span>
</div>
```

## 🔄 Loading State

Cuando `ShowLoadingState="true"` (default):
```html
<!-- Durante OnAdd -->
<button disabled>
  <span class="spinner-border" role="status"></span>
  <span class="sr-only">Agregando...</span>
</button>
```

## 🎭 Variantes de Botón

Puedes cambiar el texto del botón:
```razor
<ProductCard 
  ButtonText="Comprar Ahora"
  OnAdd="..." />

<ProductCard 
  ButtonText="Ver Detalles"
  OnAdd="..." />

<ProductCard 
  ButtonText="Cotizar"
  OnAdd="..." />
```

## 📱 Responsive Breakpoints

| Breakpoint | Comportamiento |
|------------|----------------|
| Mobile (<768px) | Padding 1.5rem, font-size base |
| Tablet (≥768px) | Padding 2rem, font-size lg |
| Desktop (≥1024px) | Grid 3 columnas, hover effects |

## 🚀 Performance

- **Lazy Loading**: Imágenes se cargan solo al entrar al viewport
- **Object-fit**: Crop inteligente sin distorsión
- **Aspect Ratio**: 4:3 para evitar layout shift
- **GPU Acceleration**: Transform en hover usa GPU
- **Debounce**: OnAdd no se puede spam-clickear (disabled durante ejecución)

## 🧪 Testing

```razor
<!-- Test: Sin imagen -->
<ProductCard 
  Title="Producto Sin Imagen"
  ImageUrl="@null"
  Price="10000"
  OnAdd="@(() => {})" />

<!-- Test: Imagen rota -->
<ProductCard 
  Title="Producto Imagen Rota"
  ImageUrl="/images/no-existe.jpg"
  Price="10000"
  OnAdd="@(() => {})" />

<!-- Test: Sin descripción -->
<ProductCard 
  Title="Producto Sin Descripción"
  ImageUrl="/images/producto.jpg"
  Price="10000"
  OnAdd="@(() => {})" />

<!-- Test: Precio alto -->
<ProductCard 
  Title="Producto Caro"
  ImageUrl="/images/producto.jpg"
  Price="1500000"
  OnAdd="@(() => {})" />
```

## 🔧 Personalización

### Override de Estilos
```razor
<style>
  /* Cambiar aspect ratio a 1:1 */
  .product-card-image-wrapper {
    aspect-ratio: 1 / 1;
  }

  /* Card más compacta */
  .product-card-body {
    padding: var(--spacing-md);
  }

  /* Botón full-width */
  .product-card-btn {
    width: 100%;
  }
</style>
```

### Agregar Badge
```razor
<div class="product-card">
  @if (producto.EnOferta)
  {
    <span class="product-badge">-20%</span>
  }
  <!-- Resto del card -->
</div>
```

## 📦 Dependencias

- ✅ **site.css**: Variables CSS (--color-*, --spacing-*, etc.)
- ✅ **Bootstrap Icons** (opcional): Para iconos de carrito
- ❌ No requiere JavaScript externo
- ❌ No requiere librerías adicionales

## 🎯 Casos de Uso

1. **Catálogo de Productos** - Grid responsive de productos
2. **Resultados de Búsqueda** - Mostrar productos filtrados
3. **Productos Relacionados** - Carrusel o grid pequeño
4. **Comparador de Precios** - Productos de diferentes tiendas
5. **Wishlist** - Lista de favoritos con botón "Agregar a carrito"

---

**Creado**: Octubre 2025  
**Versión**: 1.0  
**Compatibilidad**: .NET 8 Blazor, todos los navegadores modernos
