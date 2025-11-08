# 👤 Guía para Avatar Profesional del Autor

## 📋 Especificaciones del Avatar

- **Formato**: PNG con fondo transparente o JPG con fondo neutro
- **Resolución**: 400x400px mínimo (1:1 ratio)
- **Estilo**: Profesional, amigable, foto de perfil tipo LinkedIn
- **Fondo**: Gris neutro (#e5e7eb) o transparente
- **Iluminación**: Natural, frontal suave
- **Encuadre**: Headshot (rostro y hombros)

---

## 🎨 Opciones para Crear Avatar Real

### **Opción 1: Foto Profesional (Recomendado)**

**Equipo necesario:**
- Cámara de smartphone moderna (iPhone 12+, Samsung S21+)
- Iluminación natural (ventana grande) o ring light
- Fondo neutro (pared gris, blanca o con tela)

**Tips de fotografía:**
1. **Posición**: Sentado o de pie, cuerpo ligeramente girado (3/4)
2. **Expresión**: Sonrisa natural, mirada a la cámara
3. **Distancia**: 1.5 - 2 metros de la cámara
4. **Altura**: Cámara a nivel de los ojos
5. **Iluminación**: Luz suave frontal, evitar sombras duras
6. **Vestimenta**: Camisa o polo (colores azul, gris, blanco)

**Procesamiento:**
```bash
# Redimensionar a 400x400px
magick input.jpg -resize 400x400^ -gravity center -extent 400x400 avatar.jpg

# O usar herramienta online: 
# https://squoosh.app (Google)
# https://tinypng.com (optimización)
```

---

### **Opción 2: IA Avatar Generator (Rápido)**

#### **Leonardo.ai** (15 créditos gratis/día)

**Prompt exacto:**
```
Professional headshot portrait of a software developer, 
30s male/female, friendly smile, looking at camera, 
neutral soft gray background (#e5e7eb), 
natural soft lighting from front, 
smart casual attire (blue shirt), 
photorealistic, high detail, professional LinkedIn style,
8K resolution, no text, no logos
```

**Pasos:**
1. Ir a https://leonardo.ai
2. Modelo: Leonardo Phoenix o Kino XL
3. Aspect Ratio: 1:1 (Square)
4. Generate (4 créditos)
5. Download PNG

#### **Artbreeder** (Gratis)

1. Ir a https://www.artbreeder.com/
2. Sección "Portraits"
3. Ajustar parámetros:
   - Age: 25-35
   - Expression: Slight smile
   - Background: Neutral gray
4. Download 512x512px

---

### **Opción 3: Servicios de Avatar AI**

#### **ProfilePicture.ai** ($29 - Recomendado)
- Upload 15-20 fotos tuyas
- Genera 100+ avatares profesionales
- Incluye variaciones de fondo y estilo
- URL: https://www.profilepicture.ai/

#### **Photor.ai** ($19)
- 50 avatares profesionales
- Estilos: Corporate, Casual, Creative
- URL: https://photor.ai/

#### **Remini** (Gratis con límite)
- App móvil (iOS/Android)
- "AI Portrait" feature
- Mejora calidad de selfies

---

### **Opción 4: Placeholder Avatar (Actual)**

**Ya implementado:**
```html
<img src="/images/avatar-author.svg" alt="Avatar PeyoBv" />
```

**Características del SVG actual:**
- ✅ Profesional con gafas
- ✅ Expresión amigable (sonrisa)
- ✅ Fondo gris degradado
- ✅ Estilo ilustrado moderno
- ✅ Totalmente responsive
- ✅ Solo 3.5 KB

---

## 🖼️ Remover Fondo de Foto

Si ya tienes una foto pero con fondo no ideal:

### **Remove.bg** (Gratis - Mejor)
1. Ir a https://remove.bg
2. Upload foto
3. Download PNG sin fondo (gratis hasta 1024px)
4. Agregar fondo gris en editor

### **Photoshop Express Online**
1. Ir a https://photoshop.adobe.com/
2. Herramienta "Remove Background"
3. Agregar capa de fondo gris (#e5e7eb)

### **GIMP** (Gratis, Desktop)
1. Abrir imagen
2. Layer → Transparency → Add Alpha Channel
3. Select by Color → Click fondo
4. Delete
5. Layer → New Layer → Fill con #e5e7eb
6. Export como PNG

---

## 📐 Recortar a 1:1 (Cuadrado)

### **Online (Gratis)**

**Crop Image Online:**
```
https://croppola.com/
1. Upload imagen
2. Aspect Ratio: 1:1 (Square)
3. Ajustar encuadre (rostro centrado)
4. Download
```

**Canva:**
```
https://www.canva.com/
1. Custom Size: 400x400px
2. Upload foto
3. Ajustar y centrar rostro
4. Download PNG
```

---

## 🎯 Checklist Final

Antes de usar el avatar, verificar:

- [ ] **Resolución**: Mínimo 400x400px
- [ ] **Ratio**: 1:1 (cuadrado perfecto)
- [ ] **Formato**: PNG (preferido) o JPG optimizado
- [ ] **Peso**: < 100 KB (optimizar con TinyPNG)
- [ ] **Fondo**: Gris neutro o transparente
- [ ] **Rostro**: Centrado, bien iluminado
- [ ] **Expresión**: Amigable, profesional
- [ ] **Calidad**: Sin pixelado, alta definición

---

## 📁 Implementación

**Reemplazar archivo SVG:**
```bash
# Guardar tu avatar como:
Rodavia.Web/Rodavia.Web/wwwroot/images/avatar-author.png

# O sobrescribir SVG:
Rodavia.Web/Rodavia.Web/wwwroot/images/avatar-author.svg
```

**El Footer ya está configurado:**
```razor
<div class="avatar avatar-md avatar-bordered">
  <img src="/images/avatar-author.svg" alt="Avatar de PeyoBv" />
</div>
```

---

## 🎨 Estilos CSS Disponibles

Tamaños predefinidos:
- `.avatar-xs` - 24x24px
- `.avatar-sm` - 32x32px
- `.avatar-md` - 48x48px (actual en footer)
- `.avatar-lg` - 64x64px
- `.avatar-xl` - 96x96px
- `.avatar-2xl` - 128x128px

Variantes:
- `.avatar-bordered` - Borde blanco
- `.avatar-status` - Indicador de estado (online/offline)
- `.avatar-initials` - Avatar con iniciales

---

## 💡 Recomendación Final

**Para uso profesional inmediato:**
1. Usar **Leonardo.ai** con el prompt proporcionado
2. Generar 3-4 variaciones
3. Seleccionar la mejor
4. Optimizar con TinyPNG
5. Reemplazar `avatar-author.svg`

**Para máxima personalización:**
1. Tomar foto profesional propia
2. Remover fondo con Remove.bg
3. Agregar fondo gris en Canva
4. Optimizar y usar

---

**Última actualización**: 27 de octubre de 2025  
**Implementado por**: Rodavia Development Team
