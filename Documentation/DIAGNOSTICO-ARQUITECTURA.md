# Arquitectura del Módulo de Diagnóstico - AutoGuía

## Descripción General

El módulo de diagnóstico automotriz implementa un sistema completo de análisis de síntomas vehiculares utilizando una arquitectura en capas con separación clara de responsabilidades.

---

## Diagrama de Arquitectura

```
┌──────────────────────────────────────────────────────────────────┐
│                        PRESENTACIÓN (UI/API)                      │
│  ┌─────────────────────────────────────────────────────────────┐  │
│  │ Componentes Blazor │ Controllers REST │ Endpoints Minimal  │  │
│  │ DiagnosticoController │ SistemasController                 │  │
│  └─────────────────────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────────────────┘
                              ↓
┌──────────────────────────────────────────────────────────────────┐
│                    CAPA DE APLICACIÓN (Services)                  │
│  ┌─────────────────────────────────────────────────────────────┐  │
│  │ IDiagnosticoService           ISistemaAutomotrizService    │  │
│  │ DiagnosticoService            SistemaAutomotrizService     │  │
│  │                                                             │  │
│  │ • Validación de parámetros                                 │  │
│  │ • Orquestación de repositorios                             │  │
│  │ • Lógica de negocio                                        │  │
│  │ • Generación de recomendaciones                            │  │
│  └─────────────────────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────────────────┘
                              ↓
┌──────────────────────────────────────────────────────────────────┐
│                  CAPA DE DATOS (Repositories)                     │
│  ┌─────────────────────────────────────────────────────────────┐  │
│  │ ISintomaRepository            ICausaPosibleRepository      │  │
│  │ SintomaRepository             CausaPosibleRepository       │  │
│  │                                                             │  │
│  │ IConsultaDiagnosticoRepository    ISistemaAutomotrizRepo   │  │
│  │ ConsultaDiagnosticoRepository     SistemaAutomotrizRepo    │  │
│  │                                                             │  │
│  │ • Consultas optimizadas (LINQ)                             │  │
│  │ • Proyección a DTOs                                        │  │
│  │ • Filtrado de datos activos                                │  │
│  │ • Ordenamiento de resultados                               │  │
│  └─────────────────────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────────────────┘
                              ↓
┌──────────────────────────────────────────────────────────────────┐
│                    CAPA DE DOMINIO (Entities)                     │
│  ┌─────────────────────────────────────────────────────────────┐  │
│  │ SistemaAutomotriz  Sintoma  CausaPosible  PasoVerificacion │  │
│  │ RecomendacionPreventiva  ConsultaDiagnostico               │  │
│  │                                                             │  │
│  │ • Modelos de base de datos                                 │  │
│  │ • Propiedades de negocio                                   │  │
│  │ • Relaciones entre entidades                               │  │
│  └─────────────────────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────────────────┘
                              ↓
┌──────────────────────────────────────────────────────────────────┐
│                    CAPA DE PERSISTENCIA (DB)                      │
│                                                                    │
│  PostgreSQL - Schema: diagnostico                                │
│  Tablas: sistemas_automotrices, sintomas, causas_posibles,      │
│          pasos_verificacion, recomendaciones_preventivas,        │
│          consultas_diagnostico                                   │
└──────────────────────────────────────────────────────────────────┘
```

---

## Modelos de Datos

### Relaciones Entre Entidades

```
SistemaAutomotriz (1) ──────→ (N) Sintoma
                                    ↓
                              CausaPosible (N)
                               ↙          ↖
                    PasoVerificacion    RecomendacionPreventiva
                           (N)                    (N)


Usuario (1) ─────────→ (N) ConsultaDiagnostico ←─── (N) Sintoma (opcional)
```

### Tabla: SistemaAutomotriz

```sql
CREATE TABLE diagnostico.sistemas_automotrices (
    id INT PRIMARY KEY IDENTITY(1,1),
    nombre NVARCHAR(100) NOT NULL,
    descripcion NVARCHAR(500) NOT NULL,
    es_activo BIT NOT NULL DEFAULT 1,
    fecha_creacion DATETIME2 DEFAULT GETUTCDATE(),
    fecha_actualizacion DATETIME2
);
```

**Propósito:** Categorización de síntomas por sistemas vehiculares

---

### Tabla: Sintoma

```sql
CREATE TABLE diagnostico.sintomas (
    id INT PRIMARY KEY IDENTITY(1,1),
    sistema_automotriz_id INT NOT NULL FK,
    descripcion NVARCHAR(200) NOT NULL,
    descripcion_tecnica NVARCHAR(500) NOT NULL,
    nivel_urgencia INT NOT NULL (1-4),
    es_activo BIT NOT NULL DEFAULT 1,
    fecha_creacion DATETIME2 DEFAULT GETUTCDATE(),
    fecha_actualizacion DATETIME2
);
```

**Propósito:** Síntomas catalógados por sistema

**Niveles de Urgencia:**
- 1 = Leve
- 2 = Moderado
- 3 = Alto
- 4 = Crítico

---

### Tabla: CausaPosible

```sql
CREATE TABLE diagnostico.causas_posibles (
    id INT PRIMARY KEY IDENTITY(1,1),
    sintoma_id INT NOT NULL FK,
    descripcion NVARCHAR(200) NOT NULL,
    descripcion_detallada NVARCHAR(MAX) NOT NULL,
    nivel_probabilidad INT NOT NULL (1-5),
    requiere_servicio_profesional BIT NOT NULL DEFAULT 0,
    fecha_creacion DATETIME2 DEFAULT GETUTCDATE(),
    fecha_actualizacion DATETIME2
);
```

**Propósito:** Causas posibles para cada síntoma

**Nivel de Probabilidad:** 1-5 (1=bajo, 5=muy alto)

---

### Tabla: PasoVerificacion

```sql
CREATE TABLE diagnostico.pasos_verificacion (
    id INT PRIMARY KEY IDENTITY(1,1),
    causa_posible_id INT NOT NULL FK,
    orden INT NOT NULL,
    descripcion NVARCHAR(200) NOT NULL,
    instrucciones_detalladas NVARCHAR(MAX) NOT NULL,
    indicadores_exito NVARCHAR(MAX),
    fecha_creacion DATETIME2 DEFAULT GETUTCDATE(),
    fecha_actualizacion DATETIME2
);
```

**Propósito:** Pasos ordenados para verificar cada causa

---

### Tabla: RecomendacionPreventiva

```sql
CREATE TABLE diagnostico.recomendaciones_preventivas (
    id INT PRIMARY KEY IDENTITY(1,1),
    causa_posible_id INT NOT NULL FK,
    descripcion NVARCHAR(200) NOT NULL,
    detalle NVARCHAR(MAX) NOT NULL,
    frecuencia_kilometros INT DEFAULT 0,
    frecuencia_meses INT DEFAULT 0,
    fecha_creacion DATETIME2 DEFAULT GETUTCDATE(),
    fecha_actualizacion DATETIME2
);
```

**Propósito:** Recomendaciones preventivas para cada causa

---

### Tabla: ConsultaDiagnostico

```sql
CREATE TABLE diagnostico.consultas_diagnostico (
    id INT PRIMARY KEY IDENTITY(1,1),
    usuario_id INT NOT NULL FK,
    sintoma_descrito NVARCHAR(1000) NOT NULL,
    fecha_consulta DATETIME2 DEFAULT GETUTCDATE(),
    respuesta_asistente NVARCHAR(2000) NOT NULL,
    fue_util BIT NOT NULL DEFAULT 0,
    sintoma_relacionado_id INT FK (nullable)
);
```

**Propósito:** Historial de consultas del usuario (auditoría y análisis)

---

## DTOs (Data Transfer Objects)

### Flujo de Datos

```
Entity → Projection → DTO → JSON Response
```

### DTOs Principales

**SintomaDto**
```csharp
public class SintomaDto
{
    public int Id { get; set; }
    public string Descripcion { get; set; }
    public string DescripcionTecnica { get; set; }
    public int NivelUrgencia { get; set; }
    public int SistemaAutomotrizId { get; set; }
    public string NombreSistema { get; set; }
}
```

**CausaPosibleDto**
```csharp
public class CausaPosibleDto
{
    public int Id { get; set; }
    public string Descripcion { get; set; }
    public int NivelProbabilidad { get; set; }
    public bool RequiereServicioProfesional { get; set; }
    public List<PasoVerificacionDto> PasosVerificacion { get; set; }
    public List<RecomendacionPreventivaDto> Recomendaciones { get; set; }
}
```

**ResultadoDiagnosticoDto**
```csharp
public class ResultadoDiagnosticoDto
{
    public int NivelUrgencia { get; set; }
    public string SintomaIdentificado { get; set; }
    public List<CausaPosibleDto> CausasPosibles { get; set; }
    public string Recomendacion { get; set; }
    public bool SugerirServicioProfesional { get; set; }
}
```

---

## Flujo de Negocio

### 1. Diagnóstico de Síntoma (Happy Path)

```
Usuario describe síntoma
    ↓
DiagnosticoController.DiagnosticarSintoma()
    ↓
Validar descripción (10-1000 caracteres)
    ↓
DiagnosticoService.DiagnosticarSintomaAsync()
    ↓
SintomaRepository.BuscarSintomaPorDescripcionAsync()
    ├─ Busca coincidencias con LIKE/Contains
    └─ Retorna SintomaDto con mejor coincidencia
    ↓
¿Síntoma encontrado?
    ├─ SÍ → CausaPosibleRepository.ObtenerCausasPorSintomaAsync()
    │       Retorna causas ordenadas por probabilidad
    │       ↓
    │       Validar si alguna causa requiere servicio profesional
    │       ↓
    │       GenerarRecomendacion() basada en urgencia y necesidad profesional
    │       ↓
    │       ResultadoDiagnosticoDto completo
    │
    └─ NO → Mensaje: "No se encontraron síntomas coincidentes"
    ↓
ConsultaDiagnosticoRepository.CrearConsultaAsync()
    Registra consulta para auditoría y análisis
    ↓
Retorna ResultadoDiagnosticoDto al usuario
```

### 2. Búsqueda de Sistemas

```
Usuario abre navegación de sistemas
    ↓
SistemasController.ObtenerTodosSistemas()
    ↓
SistemaAutomotrizService.ObtenerTodosLosSistemasAsync()
    ↓
SistemaAutomotrizRepository.ObtenerTodosLosSistemasAsync()
    Proyección anidada: Sistema + Síntomas activos
    ↓
Retorna List<SistemaAutomotrizDto>
    ↓
UI muestra árbol: Sistema → Síntomas
```

### 3. Registro de Feedback

```
Usuario visualiza: "¿Fue útil?"
    ↓
DiagnosticoController.RegistrarFeedback(consultaId, fueUtil)
    ↓
DiagnosticoService.RegistrarFeedbackAsync()
    ↓
ConsultaDiagnosticoRepository.ActualizarFeedbackAsync()
    ↓
ConsultaDiagnostico.FueUtil = true/false
    ↓
Respuesta: "Feedback registrado exitosamente"
```

---

## Patrones de Diseño Utilizados

### 1. Repository Pattern
**Propósito:** Abstracción de acceso a datos

```csharp
public interface ISintomaRepository
{
    Task<List<SintomaDto>> ObtenerSintomasPorSistemaAsync(int sistemaId);
    Task<SintomaDto?> ObtenerSintomaPorIdAsync(int id);
    Task<List<SintomaDto>> BuscarSintomaPorDescripcionAsync(string descripcion);
}
```

**Ventajas:**
- Fácil de testear (mock repositories)
- Cambio de BD sin afectar servicios
- Reutilización de lógica de acceso a datos

---

### 2. Service/Application Layer Pattern
**Propósito:** Lógica de negocio centralizada

```csharp
public interface IDiagnosticoService
{
    Task<ResultadoDiagnosticoDto> DiagnosticarSintomaAsync(string descripcion, int usuarioId);
}
```

**Responsabilidades:**
- Validación de entrada
- Orquestación de repositorios
- Decisiones de negocio
- Transformación de datos

---

### 3. DTO Pattern
**Propósito:** Transferencia de datos segura

**Beneficios:**
- Serialización a JSON eficiente
- Proyección LINQ directa (sin mappers)
- Datos sólo necesarios expuestos
- Seguridad (no exponemos IDs internos sensibles)

---

### 4. Dependency Injection (DI)
**Propósito:** Desacoplamiento e inversión de control

```csharp
// Program.cs
builder.Services.AddScoped<ISintomaRepository, SintomaRepository>();
builder.Services.AddScoped<IDiagnosticoService, DiagnosticoService>();
```

**Ventajas:**
- Lifecycles (Scoped, Transient, Singleton)
- Testing con mocks
- Configuración centralizada

---

## Optimizaciones de Rendimiento

### 1. Proyección Directa a DTO

```csharp
// ✅ BIEN: Una consulta, sin materialización de entidades
var resultado = await context.Sintomas
    .Where(s => s.SistemaAutomotrizId == sistemaId)
    .Select(s => new SintomaDto
    {
        Id = s.Id,
        Descripcion = s.Descripcion,
        NivelUrgencia = s.NivelUrgencia
    })
    .ToListAsync();

// ❌ MAL: Carga entidades completas, materializa en memoria
var sintomas = await context.Sintomas
    .Where(s => s.SistemaAutomotrizId == sistemaId)
    .ToListAsync(); // Aquí se materializan
var dtos = sintomas.Select(s => new SintomaDto { ... }).ToList();
```

### 2. Índices en Foreign Keys

```sql
CREATE INDEX IX_sintomas_sistema_automotriz_id 
ON diagnostico.sintomas(sistema_automotriz_id);

CREATE INDEX IX_causas_posibles_sintoma_id 
ON diagnostico.causas_posibles(sintoma_id);
```

### 3. Filtrado en BD vs En Memoria

```csharp
// ✅ BIEN: Filtrado en BD antes de ToListAsync
var activos = await context.Sintomas
    .Where(s => s.EsActivo)
    .ToListAsync();

// ❌ MAL: Trae todos, filtra en memoria
var sintomas = await context.Sintomas.ToListAsync();
var activos = sintomas.Where(s => s.EsActivo).ToList();
```

---

## Seguridad

### 1. Autenticación
- JWT Bearer tokens requeridos para endpoints sensibles
- Usuario extraído de Claims
- Validación en controladores

### 2. Autorización
- Historial: solo usuario propietario puede ver
- Feedback: validar propiedad de consulta
- Admin: acceso total (futuro)

### 3. Validación
- ModelState en controladores
- Validación de parámetros en servicios
- Ranges, Required, StringLength en DTOs

### 4. SQL Injection Prevention
- Parámetros parametrizados (automático con EF Core)
- No uses string.Format en queries LINQ

---

## Testing

### Cobertura de Tests

```
Services: 100% cobertura
├── DiagnosticoService ✅ (17 tests)
│   ├── Diagnóstico exitoso
│   ├── Síntoma no encontrado
│   ├── Niveles de urgencia (Theory)
│   ├── Historial
│   └── Feedback
│
└── SistemaAutomotrizService (próximo)

Repositories: Mocked en tests de servicios
Controllers: Integración tests (próximo)
```

### Ejemplo de Test

```csharp
[Fact]
public async Task DiagnosticarSintomaAsync_ConSintomaEncontrado_DebeRetornarResultado()
{
    // Arrange
    var sintomaDto = new SintomaDto { ... };
    _sintomaRepositoryMock
        .Setup(x => x.BuscarSintomaPorDescripcionAsync(It.IsAny<string>()))
        .ReturnsAsync(new List<SintomaDto> { sintomaDto });

    // Act
    var resultado = await _diagnosticoService.DiagnosticarSintomaAsync("motor", 1);

    // Assert
    Assert.NotNull(resultado);
}
```

---

## Decisiones Arquitectónicas

### 1. ¿Por qué usar Repository Pattern?

**Decisión:** Sí, implementado
- Desacoplamiento de acceso a datos
- Facilita cambios de BD
- Mejora testabilidad

### 2. ¿Por qué proyección directa a DTO?

**Decisión:** Sí, LINQ Select en repositorios
- Mejor rendimiento
- Menos memoria
- Una consulta a BD

### 3. ¿Por qué separar Servicios de Repositorios?

**Decisión:** Sí
- Servicios = lógica de negocio
- Repositorios = acceso a datos
- Reutilización clara

### 4. ¿Por qué EntityFramework?

**Decisión:** Sí, ya en proyecto
- ORM potente
- LINQ para consultas
- Migraciones automáticas
- Relaciones simplificadas

---

## Escalabilidad Futura

### 1. Caché de Sistemas
```csharp
// Sistemas y síntomas cambian poco
IMemoryCache cache;
if (!cache.TryGetValue("sistemas", out var sistemas))
{
    sistemas = await repository.ObtenerTodosAsync();
    cache.Set("sistemas", sistemas, TimeSpan.FromHours(1));
}
```

### 2. Búsqueda Avanzada
```csharp
// Agregación: síntomas por palabras clave
var resultados = await context.Sintomas
    .Where(s => EF.Functions.Contains(s.Descripcion, searchTerm))
    .ToListAsync();
```

### 3. Machine Learning
```csharp
// Análisis de feedback para mejorar precisión
// Entrenamiento de modelos con histórico
```

---

## Stack Tecnológico

| Capa | Tecnología |
|------|------------|
| Presentación | Blazor WebAssembly, ASP.NET Core |
| API | RESTful, Minimal APIs |
| Aplicación | C#, .NET 8 |
| Datos | Entity Framework Core 8 |
| BD | PostgreSQL, SQL Server |
| Testing | Xunit, Moq |
| Deployment | Docker, Kubernetes |

---

## Roadmap del Módulo

### ✅ Completado (Fase 1)
- Entidades de dominio
- Repositorios
- Servicios
- Controllers
- Tests unitarios
- Documentación API
- Guía de usuario

### 🔄 En Progreso (Fase 2)
- Integration tests
- Tests de controladores
- Caché de sistemas
- Análisis de feedback

### 📋 Futuro (Fase 3)
- ML para predicción de problemas
- Historial de reparaciones
- Alertas predictivas
- Integración con talleres

---

## Conclusión

El módulo de diagnóstico implementa una arquitectura robusta, escalable y mantenible que separa claramente las responsabilidades entre capas, utiliza patrones de diseño comprobados, y proporciona una base sólida para futuras expansiones.

**Versión:** 1.0  
**Última actualización:** 2025-10-23  
**Autor:** AutoGuía Development Team
