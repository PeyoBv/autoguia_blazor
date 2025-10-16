# AutoGuía - Plataforma Automotriz Integral

## Descripción del Proyecto

AutoGuía es una plataforma web integral para el sector automotriz en Chile y Latinoamérica. Centraliza información técnica, un mapa de talleres, un catálogo de repuestos y un foro comunitario. El modelo de negocio es freemium y la estrategia inicial es lanzar un MVP (Producto Mínimo Viable) para validar la idea y construir una comunidad.

## Características del MVP

### ✅ Implementadas
- **Página de Inicio (Landing Page)** - Presenta la propuesta de valor de AutoGuía
- **Mapa de Talleres** - Lista y búsqueda de talleres mecánicos con información detallada
- **Mapa Interactivo** - Integración con Google Maps Platform para visualización de talleres
- **Foro Comunitario** - Publicaciones, respuestas y sistema de categorías
- **Autenticación de Usuarios** - Sistema de registro e inicio de sesión con .NET Identity
- **Arquitectura Modular** - Separación de responsabilidades con proyectos Core, Infrastructure y Web

### 🚧 Próximas Funcionalidades
- Catálogo de repuestos
- Sistema de calificaciones y reseñas completo
- Panel de administración
- Aplicación móvil
- Aplicación móvil

## Arquitectura Técnica

### Estructura de la Solución
```
AutoGuía/
├── AutoGuia.Core/              # Dominio y entidades de negocio
│   ├── Entities/               # Entidades del modelo de datos
│   └── DTOs/                   # Objetos de transferencia de datos
├── AutoGuia.Infrastructure/    # Capa de datos y servicios
│   ├── Data/                   # DbContext y configuración EF
│   └── Services/               # Implementación de servicios de negocio
└── AutoGuia.Web/              # Aplicación web Blazor
    ├── AutoGuia.Web/          # Proyecto servidor
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

### Usuario
- Información personal y profesional
- Especialidad automotriz y años de experiencia
- Relaciones con vehículos, publicaciones y respuestas del foro

### Taller
- Información de contacto y ubicación
- Coordenadas para mapas
- Calificaciones y reseñas
- Estado de verificación

### Vehículo
- Información técnica (marca, modelo, año, motor, etc.)
- Relación con usuario propietario

### Foro (PublicacionForo, RespuestaForo)
- Sistema de publicaciones con categorías y etiquetas
- Respuestas anidadas y sistema de likes
- Marcado de respuestas aceptadas

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

### 🏠 Página de Inicio
- Hero section con propuesta de valor
- Tarjetas de características principales
- Llamadas a la acción para talleres y foro
- Diseño responsivo con Bootstrap

### 🗺️ Mapa de Talleres
- Listado de talleres con información detallada
- Búsqueda por ciudad y filtros por región
- Indicadores de talleres verificados
- Sistema de calificaciones con estrellas
- **Mapa interactivo con Google Maps Platform**
- **Marcadores diferenciados** (verificados vs no verificados)
- **InfoWindows** con información completa del taller
- **Filtros dinámicos** que actualizan marcadores en tiempo real
- **Botón "Ver en Mapa"** que centra el mapa en el taller seleccionado
- **Integración con Google Maps** para navegación (cómo llegar)

### 💬 Foro Comunitario
- Creación de publicaciones con categorías
- Sistema de etiquetas para mejor organización
- Contadores de vistas, respuestas y likes
- Publicaciones destacadas
- Formulario de nueva publicación
- Paginación de contenido

### 🔐 Autenticación
- Registro de usuarios
- Inicio de sesión
- Gestión de perfil
- Integrado con .NET Identity

## Datos de Ejemplo

La aplicación incluye datos semilla para demostrar funcionalidades:

### Usuarios de Ejemplo
- **Juan Pérez** - Mecánico especialista en motores diésel
- **María González** - Especialista en sistemas eléctricos
- **Carlos Silva** - Aficionado a autos clásicos

### Talleres de Ejemplo
- **Taller Mecánico Central** - Santiago (Verificado)
- **AutoService Las Condes** - Las Condes (Verificado)
- **Taller Rodriguez** - Valparaíso

### Publicaciones del Foro
- Consultas sobre mantenimiento (cambio de aceite)
- Problemas mecánicos (frenos que rechinan)
- Recomendaciones de talleres

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
│   └── NavMenu.razor         # Menú de navegación
└── Pages/
    ├── Home.razor            # Página de inicio
    ├── Talleres.razor        # Lista de talleres
    └── Foro.razor           # Foro comunitario
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
- ASP.NET Core Identity
- Servicios de AutoGuía (ITallerService, IForoService)
- Blazor con renderizado automático

## Próximos Pasos

### Funcionalidades Pendientes
1. **Integración con API de Mapas** (Google Maps/OpenStreetMap)
2. **Sistema de Reseñas** completo para talleres
3. **Catálogo de Repuestos** con búsqueda avanzada
4. **Panel de Administración** para gestión de contenido
5. **Notificaciones en tiempo real**
6. **API REST** para aplicaciones móviles
7. **Migración a base de datos real** (SQL Server/PostgreSQL)

### Mejoras Técnicas
1. **Validación de formularios** más robusta
2. **Manejo de errores** centralizado
3. **Logging** estructurado
4. **Tests unitarios** y de integración
5. **CI/CD Pipeline**
6. **Containerización** con Docker

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