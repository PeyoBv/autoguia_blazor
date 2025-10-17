# AutoGuía - Plataforma Automotriz Integral

## Descripción del Proyecto

AutoGuía es una plataforma web integral para el sector automotriz en Chile y Latinoamérica que centraliza **comparación de precios de repuestos**, información técnica, un mapa de talleres y un foro comunitario especializado. El modelo de negocio es freemium y la estrategia inicial es lanzar un MVP (Producto Mínimo Viable) para validar la idea y construir una comunidad.

## Características del MVP

### ✅ Implementadas
- **🛍️ Sistema de Comparación de Precios** - Comparador de repuestos automotrices entre múltiples tiendas
- **🔍 Catálogo de Productos** - Búsqueda avanzada con filtros por marca, modelo y vehículo
- **🏪 Directorio de Tiendas** - Listado de proveedores con ofertas y precios actualizados
- **🗺️ Mapa de Talleres** - Lista y búsqueda de talleres mecánicos con experiencias de usuarios
- **💬 Foro Comunitario Especializado** - Con categorías temáticas y sistema de puntuación
- **⭐ Sistema de Experiencias** - Reseñas detalladas de talleres con calificaciones
- **🔐 Autenticación de Usuarios** - Sistema completo con .NET Identity y roles
- **📱 Interfaz Responsive** - Diseño optimizado para todos los dispositivos

### 🚧 Próximas Funcionalidades
- Integración con APIs de tiendas reales
- Sistema de alertas de precios
- Comparador de servicios de talleres
- Panel de administración avanzado
- Aplicación móvil nativa

## Arquitectura Técnica

### Estructura de la Solución
```
AutoGuía/
├── AutoGuia.Core/              # Dominio y entidades de negocio
│   ├── Entities/               # Entidades del modelo de datos
│   │   ├── Usuario.cs          # Usuario del sistema
│   │   ├── Taller.cs           # Talleres mecánicos
│   │   ├── Vehiculo.cs         # Vehículos de usuarios
│   │   ├── Producto.cs         # Productos/repuestos
│   │   ├── Tienda.cs           # Tiendas proveedoras
│   │   ├── Oferta.cs           # Ofertas de productos
│   │   ├── Marca.cs            # Marcas de vehículos
│   │   ├── Modelo.cs           # Modelos de vehículos
│   │   ├── PublicacionForo.cs  # Publicaciones del foro
│   │   └── RespuestaForo.cs    # Respuestas del foro
│   └── DTOs/                   # Objetos de transferencia de datos
│       ├── TallerDto.cs        # DTOs para talleres
│       ├── ForoDto.cs          # DTOs para foro
│       └── ComparadorDto.cs    # DTOs para comparación de precios
├── AutoGuia.Infrastructure/    # Capa de datos y servicios
│   ├── Data/                   # DbContext y configuración EF
│   │   └── AutoGuiaDbContext.cs
│   └── Services/               # Implementación de servicios de negocio
│       ├── TallerService.cs    # Gestión de talleres
│       ├── ForoService.cs      # Gestión del foro
│       ├── ComparadorService.cs # Comparación de precios
│       ├── ProductoService.cs  # Gestión de productos
│       ├── TiendaService.cs    # Gestión de tiendas
│       ├── VehiculoService.cs  # Gestión de vehículos
│       └── IServices.cs        # Interfaces de servicios
└── AutoGuia.Web/              # Aplicación web Blazor
    ├── AutoGuia.Web/          # Proyecto servidor
    │   ├── Components/
    │   │   ├── Layout/         # Layouts de la aplicación
    │   │   ├── Pages/          # Páginas principales
    │   │   │   ├── Home.razor          # Página de inicio
    │   │   │   ├── Productos.razor     # Comparador de precios
    │   │   │   ├── DetalleProducto.razor # Detalles de producto
    │   │   │   ├── Talleres.razor      # Lista de talleres
    │   │   │   ├── DetalleTaller.razor # Detalles de taller
    │   │   │   ├── Foro.razor          # Foro comunitario
    │   │   │   └── TestServicios.razor # Página de testing
    │   │   └── Account/        # Páginas de autenticación
    │   └── Program.cs          # Configuración de la aplicación
    └── AutoGuia.Web.Client/   # Proyecto cliente (WebAssembly)
```

### Stack Tecnológico
- **Framework**: .NET 8
- **UI**: Blazor con modo de renderizado Automático (Servidor + WebAssembly)
- **Base de datos**: Entity Framework Core con InMemory Database (para MVP)
- **Autenticación**: ASP.NET Core Identity
- **Frontend**: Bootstrap 5 + Font Awesome
- **IDE**: Visual Studio Code

## Entidades del Dominio

### Sistema de Comparación de Precios

#### Producto
- Información detallada del repuesto (nombre, descripción, número de parte)
- Categoría y subcategoría
- Marca y especificaciones técnicas
- Compatibilidad con vehículos específicos
- Imágenes y documentación técnica

#### Tienda
- Información comercial y de contacto
- Políticas de envío y garantía
- Calificación y confiabilidad
- Catálogo de productos disponibles

#### Oferta
- Precios actualizados por tienda
- Disponibilidad en stock
- Descuentos y promociones
- Enlaces directos para compra
- Historial de precios

#### Marca y Modelo
- Marcas de vehículos soportadas
- Modelos específicos por marca
- Años de fabricación
- Compatibilidad con productos

### Sistema de Talleres y Experiencias

#### Usuario
- Información personal y profesional
- Especialidad automotriz y años de experiencia
- Relaciones con vehículos, publicaciones y respuestas del foro
- Historial de experiencias en talleres

#### Taller
- Información de contacto y ubicación
- Coordenadas para mapas
- Calificaciones y experiencias de usuarios
- Estado de verificación
- Especialidades y servicios ofrecidos

#### ExperienciaUsuario
- Reseñas detalladas de servicios recibidos
- Calificación con estrellas (1-5)
- Tipo de experiencia (positiva/negativa)
- Tipo de servicio realizado
- Sistema de likes y respuestas

### Sistema de Foro Comunitario

#### PublicacionForo
- Sistema de publicaciones con categorías temáticas:
  - 🌟 **Valoraciones y Reseñas** - Experiencias con talleres y productos
  - ❓ **Consultas Técnicas** - Preguntas mecánicas especializadas
  - 🏎️ **Rendimiento y Tuning** - Discusiones sobre performance
  - 🔧 **Productos Alternativos** - Alternativas de repuestos
- Sistema de puntuación con likes/dislikes
- Etiquetas para mejor organización
- Contadores de vistas y respuestas

#### RespuestaForo
- Respuestas anidadas y sistema de puntuación
- Marcado de respuestas aceptadas
- Sistema de moderación

### Vehículo
- Información técnica (marca, modelo, año, motor, etc.)
- Relación con usuario propietario
- Compatibilidad con productos específicos

## Instalación y Configuración

### Prerrequisitos
- .NET 8 SDK
- Visual Studio Code
- Extensión C# Dev Kit

### Pasos de Instalación

1. **Clonar el repositorio**
   ```bash
   git clone <repository-url>
   cd autoguia
   ```

2. **Restaurar dependencias**
   ```bash
   dotnet restore AutoGuia.sln
   ```

3. **Compilar la solución**
   ```bash
   dotnet build AutoGuia.sln
   ```

4. **Configurar Google Maps API** (opcional pero recomendado)
   ```bash
   # Configurar clave de API usando el administrador de secretos
   cd AutoGuia.Web/AutoGuia.Web
   dotnet user-secrets set "GoogleMaps:ApiKey" "TU_CLAVE_DE_GOOGLE_MAPS"
   ```
   > Ver `GOOGLE_MAPS_SETUP.md` para instrucciones detalladas de configuración

5. **Ejecutar la aplicación**
   ```bash
   dotnet run --project AutoGuia.Web/AutoGuia.Web/AutoGuia.Web.csproj
   ```

6. **Acceder a la aplicación**
   - Abrir navegador en `https://localhost:7xxx` o `http://localhost:5xxx`

### Usando VS Code

1. Abrir el proyecto en VS Code
2. Usar `Ctrl+Shift+P` → "Tasks: Run Task" → "build" para compilar
3. Usar `Ctrl+Shift+P` → "Tasks: Run Task" → "run" para ejecutar
4. O presionar `F5` para depurar

## Funcionalidades Principales

### 🛍️ Sistema de Comparación de Precios
- **Búsqueda inteligente** de repuestos automotrices
- **Filtros avanzados** por marca, modelo y tipo de vehículo
- **Comparación en tiempo real** de precios entre múltiples tiendas
- **Detalles de productos** con especificaciones técnicas
- **Enlaces directos** a tiendas para compra
- **Página de testing** (/test-productos) para verificar servicios

### 🏠 Página de Inicio
- Hero section con enfoque en comparación de precios
- Acceso directo al comparador de precios
- Tarjetas de características principales
- Llamadas a la acción para comparar precios y ver catálogo
- Diseño responsivo con Bootstrap

### 🗺️ Sistema de Talleres
- **Listado completo** de talleres con información detallada
- **Búsqueda por ciudad** y filtros por región
- **Indicadores de talleres verificados**
- **Sistema de experiencias** con calificaciones de usuarios
- **Mapa interactivo** con Google Maps Platform
- **Marcadores diferenciados** (verificados vs no verificados)
- **InfoWindows** con información completa del taller
- **Filtros dinámicos** que actualizan marcadores en tiempo real
- **Botón "Ver en Mapa"** que centra el mapa en el taller seleccionado

### ⭐ Experiencias de Talleres
- **Reseñas detalladas** de usuarios sobre servicios recibidos
- **Calificación con estrellas** (1-5) por experiencia
- **Clasificación** de experiencias (positivas/negativas)
- **Categorización por tipo de servicio** (frenos, motor, etc.)
- **Sistema de interacciones** (likes, respuestas)
- **Modal para crear experiencias** con formulario completo
- **Avatares de usuarios** con iniciales
- **Fechas y detalles** de cada experiencia

### 💬 Foro Comunitario Especializado
- **Categorías temáticas específicas**:
  - 🌟 **Valoraciones y Reseñas** - Experiencias con talleres y productos
  - ❓ **Consultas Técnicas** - Preguntas mecánicas especializadas  
  - 🏎️ **Rendimiento y Tuning** - Discusiones sobre performance
  - 🔧 **Productos Alternativos** - Alternativas de repuestos
- **Sistema de puntuación avanzado** con likes/dislikes
- **Filtros dinámicos** por categoría con efectos visuales
- **Ordenamiento múltiple** (recientes, populares, más comentadas)
- **Contadores en tiempo real** de publicaciones por categoría
- **Sistema de etiquetas** para mejor organización
- **Publicaciones destacadas** y moderación
- **Formulario mejorado** para nueva publicación

### 🔐 Autenticación
- Registro de usuarios con validación
- Inicio de sesión seguro
- Gestión de perfil personalizado
- **Sistema de roles** (Admin, Usuario)
- Integrado con .NET Identity

## Datos de Ejemplo

La aplicación incluye **52 entidades semilla** para demostrar funcionalidades:

### Sistema de Comparación de Precios
- **15 Productos** - Filtros de aceite, pastillas de freno, bujías, etc.
- **8 Tiendas** - MercadoLibre Chile, Sodimac, Repuestos García, etc.
- **25+ Ofertas** - Precios competitivos entre diferentes proveedores
- **4 Marcas** - Toyota, Chevrolet, Ford, Nissan
- **12 Modelos** - Corolla, Spark, Fiesta, March, etc.

### Usuarios de Ejemplo
- **Juan Pérez** - Mecánico especialista en motores diésel
- **María González** - Especialista en sistemas eléctricos
- **Carlos Silva** - Aficionado a autos clásicos

### Talleres de Ejemplo
- **Taller Mecánico San Miguel** - Santiago (Verificado) ⭐⭐⭐⭐⭐
- **AutoService Las Condes** - Las Condes (Verificado) ⭐⭐⭐⭐
- **Taller Rodriguez** - Valparaíso ⭐⭐⭐

### Experiencias en Talleres
- **Experiencias positivas** con calificaciones 4-5 estrellas
- **Experiencias detalladas** por tipo de servicio
- **Comentarios realistas** sobre calidad, tiempo y precios
- **Sistema de likes** para experiencias útiles

### Publicaciones del Foro (Por Categoría)
- **Valoraciones**: Recomendaciones de talleres y productos
- **Consultas Técnicas**: Problemas con frenos, motor, transmisión
- **Rendimiento**: Discusiones sobre modificaciones y tuning
- **Productos Alternativos**: Comparaciones de marcas y repuestos

## Comandos Útiles

### Desarrollo
```bash
# Compilar solución
dotnet build AutoGuia.sln

# Ejecutar aplicación web
dotnet run --project AutoGuia.Web/AutoGuia.Web/AutoGuia.Web.csproj

# Ejecutar en modo watch (recarga automática)
dotnet watch --project AutoGuia.Web/AutoGuia.Web/AutoGuia.Web.csproj

# Limpiar compilación
dotnet clean AutoGuia.sln

# Restaurar paquetes NuGet
dotnet restore AutoGuia.sln
```

### Base de Datos
```bash
# La aplicación usa InMemory Database por defecto
# Los datos se crean automáticamente al iniciar la aplicación
```

## Estructura del Código

### Patrón de Arquitectura
- **Separación de responsabilidades**: Core, Infrastructure, Web
- **Dependency Injection**: Servicios registrados en Program.cs
- **Repository Pattern**: Implementado en servicios de Infrastructure

### Componentes Blazor
```
AutoGuia.Web/Components/
├── Layout/
│   ├── MainLayout.razor      # Layout principal
│   └── NavMenu.razor         # Menú de navegación actualizado
└── Pages/
    ├── Home.razor            # Página de inicio (comparador)
    ├── Productos.razor       # Comparador de precios principal
    ├── DetalleProducto.razor # Detalles de producto con ofertas
    ├── Talleres.razor        # Lista de talleres
    ├── DetalleTaller.razor   # Detalles de taller con experiencias
    ├── Foro.razor           # Foro comunitario con categorías
    └── TestServicios.razor  # Página de testing de servicios
```

## Configuración de Desarrollo

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "DataSource=app.db;Cache=Shared"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Program.cs - Servicios Registrados
- Entity Framework con InMemory Database
- ASP.NET Core Identity con roles
- **Servicios de Comparación de Precios**:
  - IComparadorService - Búsqueda y comparación
  - IProductoService - Gestión de productos
  - ITiendaService - Gestión de tiendas
  - IVehiculoService - Gestión de marcas/modelos
- **Servicios Tradicionales**:
  - ITallerService - Gestión de talleres
  - IForoService - Gestión del foro
- Blazor con renderizado automático (Server + WebAssembly)

## Próximos Pasos

### Funcionalidades Pendientes
1. **Integración con APIs Reales** de tiendas (MercadoLibre, Sodimac, etc.)
2. **Sistema de Alertas de Precios** - Notificaciones cuando bajan precios
3. **Comparador de Servicios** de talleres con precios
4. **Panel de Administración Avanzado** para gestión de contenido
5. **Sistema de Favoritos** para productos y talleres
6. **API REST** para aplicaciones móviles
7. **Migración a base de datos real** (SQL Server/PostgreSQL)
8. **Sistema de Notificaciones** en tiempo real
9. **Geolocalización** para talleres cercanos
10. **Integración con sistemas de pago**

### Mejoras Técnicas
1. **Optimización de consultas** de base de datos
2. **Cache** para mejorar performance
3. **Validación de formularios** más robusta
4. **Manejo de errores** centralizado
5. **Logging** estructurado con Serilog
6. **Tests unitarios** y de integración
7. **CI/CD Pipeline** con GitHub Actions
8. **Containerización** con Docker
9. **Monitoreo y métricas** de aplicación
10. **Seguridad avanzada** con rate limiting

## Contribución

Para contribuir al proyecto:

1. Fork del repositorio
2. Crear rama feature (`git checkout -b feature/nueva-funcionalidad`)
3. Commit cambios (`git commit -am 'Agregar nueva funcionalidad'`)
4. Push a la rama (`git push origin feature/nueva-funcionalidad`)
5. Crear Pull Request

## Licencia

Este proyecto está bajo la licencia MIT. Ver archivo `LICENSE` para más detalles.

## Contacto

Para preguntas o sugerencias sobre el proyecto, crear un issue en el repositorio.

---

**AutoGuía** - Conectando la comunidad automotriz 🚗💨