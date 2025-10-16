# Instrucciones de GitHub Copilot para AutoGuía

## Contexto del Proyecto

AutoGuía es una plataforma web integral para el sector automotriz en Chile y Latinoamérica desarrollada con .NET 8 y Blazor. El proyecto implementa una arquitectura modular con separación de responsabilidades.

## Estructura del Proyecto

```
AutoGuía/
├── AutoGuia.Core/              # Dominio - Entidades y DTOs
├── AutoGuia.Infrastructure/    # Datos - DbContext y Services  
├── AutoGuia.Web/              # UI - Aplicación Blazor
│   ├── AutoGuia.Web/          # Servidor
│   └── AutoGuia.Web.Client/   # Cliente WebAssembly
└── AutoGuia.sln               # Solución principal
```

## Patrones y Convenciones

### Naming Conventions
- **Entidades**: PascalCase (ej: `Usuario`, `Taller`, `PublicacionForo`)
- **Propiedades**: PascalCase (ej: `NombreCompleto`, `FechaCreacion`)
- **Métodos**: PascalCase (ej: `ObtenerTalleresAsync`, `CrearPublicacion`)
- **Variables locales**: camelCase (ej: `talleresFiltrados`, `publicacionId`)

### Arquitectura
- **Core**: Contiene entidades del dominio y DTOs
- **Infrastructure**: Implementa acceso a datos y lógica de negocio
- **Web**: Interfaz de usuario con Blazor (modo Auto render)

### Servicios
- Interfaces en `Infrastructure/Services/IServices.cs`
- Implementaciones en archivos separados (`TallerService.cs`, `ForoService.cs`)
- Registro en `Program.cs` con Dependency Injection

## Funcionalidades Implementadas

### ✅ Completadas
- Sistema de autenticación con ASP.NET Identity
- Gestión de talleres con filtros y búsqueda
- Foro comunitario con publicaciones y respuestas
- Navegación responsive con Bootstrap 5
- Base de datos InMemory con datos semilla

### 🚧 En Desarrollo
- Integración con API de mapas
- Sistema de calificaciones completo
- Validación avanzada de formularios

## Tecnologías Utilizadas

- **.NET 8** - Framework principal
- **Blazor** - UI con renderizado automático (Server + WASM)
- **Entity Framework Core** - ORM con InMemory Database
- **Bootstrap 5** - Framework CSS
- **Font Awesome** - Iconos
- **ASP.NET Identity** - Autenticación y autorización

## Guidelines para Copilot

### Al generar código:

1. **Seguir convenciones existentes** del proyecto
2. **Usar async/await** para operaciones de base de datos
3. **Implementar manejo de errores** con try-catch
4. **Agregar comentarios XML** para métodos públicos
5. **Validar entrada de usuario** en formularios
6. **Mantener separación de responsabilidades** entre capas

### Para nuevas entidades:
```csharp
// Ejemplo de estructura base
public class NuevaEntidad
{
    public int Id { get; set; }
    [Required]
    public string Nombre { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public bool EsActivo { get; set; } = true;
}
```

### Para servicios:
```csharp
// Interface en IServices.cs
public interface INuevoService
{
    Task<IEnumerable<NuevaEntidadDto>> ObtenerAsync();
    Task<NuevaEntidadDto?> ObtenerPorIdAsync(int id);
    Task<int> CrearAsync(CrearNuevaEntidadDto entidad);
}

// Implementación
public class NuevoService : INuevoService
{
    private readonly AutoGuiaDbContext _context;
    
    public NuevoService(AutoGuiaDbContext context)
    {
        _context = context;
    }
    
    // Implementar métodos...
}
```

### Para páginas Blazor:
```razor
@page "/ruta"
@rendermode InteractiveServer
@inject IServicio Servicio

<PageTitle>Título - AutoGuía</PageTitle>

<div class="container py-4">
    <!-- Contenido de la página -->
</div>

@code {
    // Lógica del componente
}
```

## Comandos Útiles

### Desarrollo
```bash
dotnet build AutoGuia.sln           # Compilar
dotnet run --project AutoGuia.Web/AutoGuia.Web/AutoGuia.Web.csproj  # Ejecutar
```

### VS Code Tasks
- **build**: Compilar la solución
- **run**: Ejecutar la aplicación en modo desarrollo

## Base de Datos

- **Tipo**: InMemory Database (para MVP)
- **Context**: `AutoGuiaDbContext`
- **Inicialización**: Automática con datos semilla en `Program.cs`
- **Entidades principales**: Usuario, Taller, Vehiculo, PublicacionForo, RespuestaForo

## Próximas Implementaciones Sugeridas

1. **Mapas interactivos** - Integrar Google Maps o Leaflet
2. **Upload de imágenes** - Para talleres y perfil de usuarios  
3. **Sistema de notificaciones** - Para respuestas del foro
4. **API REST** - Para futuras aplicaciones móviles
5. **Tests unitarios** - Para servicios y componentes

## Notas Importantes

- El proyecto usa **modo de renderizado automático** (Server + WebAssembly)
- Las **referencias entre proyectos** están configuradas correctamente
- Los **servicios están registrados** en Program.cs
- La **navegación** está actualizada con las páginas principales
- **Font Awesome** está incluido para iconos