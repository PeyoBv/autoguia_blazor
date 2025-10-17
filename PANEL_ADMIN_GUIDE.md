# Panel de Administración - AutoGuía

## 🔐 Acceso al Panel de Administración

### Credenciales por Defecto
- **Usuario**: `admin@autoguia.cl`
- **Contraseña**: `Admin123!`

### Acceso
1. Navega a la página de inicio de sesión: `/Account/Login`
2. Inicia sesión con las credenciales de administrador
3. Una vez autenticado, verás el enlace "Panel Admin" en el menú de navegación
4. Haz clic en "Panel Admin" para acceder al panel de administración

---

## 🛠️ Funcionalidades del Panel de Administración

### Gestión de Talleres
**URL**: `/admin/gestion-talleres`

#### Estadísticas del Dashboard
- **Total de Talleres**: Número total de talleres registrados
- **Verificados**: Talleres que han sido verificados por administradores
- **Pendientes**: Talleres que aún no han sido verificados
- **Calificación Promedio**: Promedio de calificaciones de todos los talleres

#### Filtros y Búsqueda
- **Búsqueda por texto**: Busca por nombre, ciudad o región
- **Filtro por estado**: Filtra talleres verificados, pendientes o todos
- **Limpiar filtros**: Restablece todos los filtros aplicados

#### Operaciones CRUD

##### ➕ Crear Nuevo Taller
1. Haz clic en el botón "Nuevo Taller"
2. Completa el formulario con la información requerida:
   - **Campos obligatorios**: Nombre, Dirección, Ciudad, Región
   - **Campos opcionales**: Descripción, Teléfono, Email, Horario, Especialidades, Coordenadas
3. Marca "Taller verificado" si el taller ya fue validado
4. Haz clic en "Guardar" para crear el taller

##### ✏️ Editar Taller Existente
1. En la tabla de talleres, haz clic en el ícono de editar (lápiz) del taller deseado
2. Modifica la información necesaria en el formulario
3. Haz clic en "Guardar" para aplicar los cambios

##### 👁️ Ver Detalles del Taller
1. Haz clic en el ícono de ojo para ver información detallada
2. Se mostrará un diálogo con el ID del taller (funcionalidad expandible)

##### 🗑️ Eliminar Taller
1. Haz clic en el ícono de papelera del taller a eliminar
2. Confirma la acción en el diálogo de confirmación
3. El taller será marcado como inactivo (eliminación lógica)

---

## 🏗️ Arquitectura Técnica

### Seguridad
- **Autorización basada en roles**: Solo usuarios con rol "Admin" pueden acceder
- **Protección a nivel de página**: `[Authorize(Roles = "Admin")]`
- **Inicialización automática**: El usuario admin se crea automáticamente al iniciar la aplicación

### Estructura de Datos

#### DTOs Utilizados
- `TallerDto`: Lectura de talleres con información completa
- `CrearTallerDto`: Creación de nuevos talleres con validaciones
- `ActualizarTallerDto`: Actualización de talleres existentes

#### Servicios
- `ITallerService`: Interfaz con operaciones CRUD
- `TallerService`: Implementación con Entity Framework Core

#### Validaciones
- Campos requeridos: Nombre, Dirección, Ciudad, Región
- Validaciones de formato: Email, Teléfono
- Longitud máxima de campos
- Rangos de coordenadas geográficas (-90/90 lat, -180/180 lng)

### Base de Datos
- **Eliminación lógica**: Los talleres eliminados se marcan como `EsActivo = false`
- **Auditoría**: Fecha de registro automática
- **Índices**: Optimización para consultas por ciudad y región

---

## 🚀 Próximas Funcionalidades

### En Desarrollo
- [ ] **Gestión de Usuarios**: CRUD completo de usuarios registrados
- [ ] **Gestión de Reseñas**: Moderación de comentarios y calificaciones
- [ ] **Reportes y Analytics**: Estadísticas detalladas y gráficos
- [ ] **Gestión de Repuestos**: Administración del catálogo de repuestos

### Futuras Implementaciones
- [ ] **Sistema de Logs**: Auditoría de acciones administrativas
- [ ] **Exportación de Datos**: Descarga de reportes en Excel/PDF
- [ ] **Notificaciones**: Sistema de alertas para administradores
- [ ] **Configuración Global**: Parámetros generales del sistema

---

## 🔧 Configuración y Personalización

### Agregar Nuevos Roles
```csharp
// En Program.cs, método InicializarRolesYAdminAsync
if (!await roleManager.RoleExistsAsync("NuevoRol"))
{
    await roleManager.CreateAsync(new IdentityRole("NuevoRol"));
}
```

### Crear Nuevas Páginas de Administración
1. Crear archivo en `/Components/Pages/Admin/`
2. Agregar `@page "/admin/nueva-funcionalidad"`
3. La página heredará automáticamente `[Authorize(Roles = "Admin")]` del `_Imports.razor`

### Personalizar el Dashboard
- Las estadísticas se calculan dinámicamente desde la base de datos
- Los componentes están diseñados para ser responsivos (Bootstrap 5)
- Los iconos utilizan Font Awesome 6

---

## 🐛 Troubleshooting

### Problemas Comunes

#### No aparece el enlace "Panel Admin"
- Verifica que el usuario tenga el rol "Admin" asignado
- Asegúrate de haber iniciado sesión correctamente
- Refresca la página después del login

#### Error al crear/editar talleres
- Verifica que todos los campos requeridos estén completos
- Revisa las validaciones de formato (email, teléfono)
- Confirma que las coordenadas estén en el rango válido

#### Problemas de compilación
- Ejecuta `dotnet clean` seguido de `dotnet build`
- Verifica que todas las referencias de NuGet estén actualizadas
- Revisa los `using statements` en los archivos `.razor`

### Logs y Depuración
- Los errores se muestran en la consola del navegador
- Usa las herramientas de desarrollo del navegador para debug
- Revisa los logs de Entity Framework Core en la consola de la aplicación

---

## 📞 Soporte

Para reportar bugs o solicitar nuevas funcionalidades:
1. Crear un issue en el repositorio de GitHub
2. Incluir descripción detallada del problema o solicitud
3. Proporcionar pasos para reproducir bugs
4. Incluir capturas de pantalla si es necesario

---

*Última actualización: Enero 2025*