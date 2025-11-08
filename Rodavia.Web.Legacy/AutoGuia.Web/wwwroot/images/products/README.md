# 🚗 Guía para Obtener Imágenes de Autos (PNG con Fondo Transparente)

## 📋 Especificaciones Requeridas

- **Formato**: PNG con fondo transparente
- **Resolución**: Mínimo 1000x1000px (1:1 ratio)
- **Vista**: 3/4 frontal (three-quarter view)
- **Calidad**: Alta definición, reflejos realistas
- **Sin**: Logos, matrículas, textos, watermarks

---

## 🌐 Recursos Gratuitos Recomendados

### 1. **Pngimg.com** ⭐ MEJOR OPCIÓN
- **URL**: https://pngimg.com/images/transport/car
- **Ventajas**: 
  - ✅ 100% gratis, sin registro
  - ✅ Fondo transparente nativo
  - ✅ Alta calidad (2000+ px)
  - ✅ Vista 3/4 profesional
- **Categorías**:
  - Compactos
  - Sedanes
  - SUVs
  - Deportivos
  - Eléctricos

**Ejemplo de descarga:**
```powershell
Invoke-WebRequest -Uri "https://pngimg.com/uploads/car/car_PNG1733.png" -OutFile "wwwroot/images/products/compact-white.png"
```

### 2. **FreePNGs.com**
- **URL**: https://www.freepngs.com/search/car
- **Ventajas**: PNG transparentes de alta calidad
- **Proceso**: Buscar "car side view" o "car 3/4"

### 3. **StickPNG.com**
- **URL**: https://www.stickpng.com/cat/transport/cars
- **Ventajas**: Buena selección de marcas populares

### 4. **Cleanpng.com**
- **URL**: https://www.cleanpng.com/
- **Búsqueda**: "Toyota Corolla PNG", "Honda Civic PNG"
- **Limitación**: 10 descargas/día gratis

---

## 🎨 Alternativa: Generación con IA

### **Leonardo.ai** (15 créditos gratis/día)
**Prompt exacto:**
```
Studio product photo of a [MARCA] [MODELO], three-quarter front view, 
white studio background, high detail, realistic reflections, 
neutral softbox lighting, centered composition, 
photorealistic, 8K, no logos, no license plates
```

**Ejemplos:**
- "Studio product photo of a Toyota Corolla 2024, three-quarter front view..."
- "Studio product photo of a Honda HR-V SUV, three-quarter front view..."

**Pasos:**
1. Registrarse en https://leonardo.ai
2. Ir a "Image Generation"
3. Modelo: Leonardo Phoenix
4. Pegar prompt
5. Generate (usa 4 créditos)
6. Descargar PNG

### **Remove.bg** (Para remover fondo)
Si tienes imagen con fondo:
1. Ir a https://remove.bg
2. Upload imagen
3. Descargar PNG sin fondo (gratis hasta 1024px)

---

## 📁 Estructura de Archivos

```
wwwroot/images/products/
├── compact-white.png          # Auto compacto
├── sedan-silver.png            # Sedán ejecutivo
├── suv-blue.png                # SUV/Camioneta
├── electric-red.png            # Eléctrico
└── sport-black.png             # Deportivo
```

---

## 🔧 Script de Descarga Masiva

```powershell
# Lista de URLs de pngimg.com (actualizar con URLs reales)
$cars = @{
    "compact-white" = "URL_DEL_PNG_1"
    "sedan-silver" = "URL_DEL_PNG_2"
    "suv-blue" = "URL_DEL_PNG_3"
}

$outputPath = "Rodavia.Web/Rodavia.Web/wwwroot/images/products"

foreach ($car in $cars.GetEnumerator()) {
    Write-Host "⬇️  Descargando $($car.Key)..."
    Invoke-WebRequest -Uri $car.Value -OutFile "$outputPath/$($car.Key).png"
    Write-Host "✅ $($car.Key).png descargado"
}
```

---

## 🎯 Autos Prioritarios para Rodavia Chile

### Categoría Compactos (Más vendidos)
- ✅ Suzuki Swift
- ✅ Chevrolet Onix
- ✅ Kia Rio
- ✅ Hyundai Accent

### Categoría SUV (Tendencia)
- ✅ Hyundai Creta
- ✅ Kia Sportage
- ✅ Nissan Qashqai
- ✅ Mazda CX-5

### Categoría Pickup (Mercado chileno)
- ✅ Toyota Hilux
- ✅ Nissan Frontier
- ✅ Ford Ranger
- ✅ Mitsubishi L200

---

## 📝 Licencias y Atribución

### Pngimg.com
- **Licencia**: Dominio público / CC0
- **Atribución**: No requerida, pero recomendada
- **Uso comercial**: ✅ Permitido

### Leonardo.ai
- **Licencia**: Propiedad del usuario generador
- **Uso comercial**: ✅ Permitido (plan gratuito y pagado)

---

## ⚠️ Importante

1. **Verificar matrículas**: Asegurarse que no sean visibles
2. **Logos de marca**: En Chile es legal mostrar logos en contexto editorial
3. **Optimizar tamaño**: Comprimir PNG con TinyPNG.com antes de subir
4. **Nombres descriptivos**: `toyota-corolla-2024-white.png`

---

## 🚀 Placeholders Actuales

Mientras tanto, el proyecto usa **SVG placeholders vectoriales**:
- `car-placeholder-compact.svg` (Azul)
- `car-placeholder-suv.svg` (Verde)
- `car-placeholder-sedan.svg` (Índigo)

Estos se pueden reemplazar cuando tengas las imágenes reales.

---

**Última actualización**: 27 de octubre de 2025
**Autor**: Rodavia Development Team
