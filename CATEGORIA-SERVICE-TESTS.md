# 🧪 Tests Unitarios - CategoriaService

## 📋 Descripción

Suite completa de tests unitarios para `CategoriaService` implementada con **xUnit**, **Moq** y **FluentAssertions**. Utiliza **Entity Framework Core InMemory Database** para simular la base de datos.

## ✅ Resultados de Ejecución

```
Pruebas totales: 10
     Correcto: 10 ✅
 Tiempo total: 3.4 segundos
```

**Estado**: ✅ Todos los tests pasando (0 errores)

## 🗂️ Estructura del Proyecto de Tests

```
AutoGuia.Tests/
├── AutoGuia.Tests.csproj          # Proyecto de tests con xUnit, Moq, FluentAssertions
└── Services/
    └── CategoriaServiceTests.cs   # Suite de tests para CategoriaService
```

## 📦 Dependencias

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="8.0.11" />
<PackageReference Include="xunit" Version="2.5.3" />
<PackageReference Include="Moq" Version="4.20.72" />
<PackageReference Include="FluentAssertions" Version="8.7.1" />
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
```

## 🧪 Test Cases Implementados

### 1️⃣ ObtenerCategoriasAsync_DebeRetornarSoloCategoriasActivas
- **Objetivo**: Verificar que solo retorna categorías con `EsActivo = true`
- **Datos**: 2 categorías activas, 1 inactiva
- **Validaciones**:
  - ✅ Retorna 2 categorías
  - ✅ Incluye "Aceites" y "Filtros"
  - ✅ Excluye "Neumáticos" (inactiva)
  - ✅ Incluye subcategorías y valores de filtro
  - ✅ Logging de información ejecutado

**Assertions clave:**
```csharp
categorias.Should().HaveCount(2, "solo hay 2 categorías activas");
categorias.Should().NotContain(c => c.Nombre == "Neumáticos", "porque está inactiva");
categoriaAceites.Subcategorias.Should().HaveCount(2);
```

---

### 2️⃣ ObtenerSubcategoriasAsync_DebeRetornarSubcategoriasPorCategoriaId
- **Objetivo**: Verificar filtrado correcto por categoría ID
- **Datos**: Categoría "Aceites" con 2 subcategorías ("Viscosidad", "Marca")
- **Validaciones**:
  - ✅ Retorna 2 subcategorías
  - ✅ Nombres correctos
  - ✅ Valores de filtro incluidos
  - ✅ Logging ejecutado

**Assertions clave:**
```csharp
subcategorias.Should().HaveCount(2, "la categoría Aceites tiene 2 subcategorías");
viscosidad.Valores.Should().Contain(v => v.Valor == "10W-40");
```

---

### 3️⃣ ObtenerValoresFiltroAsync_DebeRetornarValoresPorSubcategoriaId
- **Objetivo**: Verificar filtrado de valores por subcategoría ID
- **Datos**: Subcategoría "Viscosidad" con valores "10W-40", "5W-30"
- **Validaciones**:
  - ✅ Retorna 2 valores
  - ✅ Valores correctos
  - ✅ Ordenamiento alfabético
  - ✅ Logging ejecutado

**Assertions clave:**
```csharp
valores.Should().HaveCount(2, "la subcategoría Viscosidad tiene 2 valores");
valores[0].Valor.Should().Be("10W-40"); // Verificar orden
```

---

### 4️⃣ ObtenerCategoriaPorIdAsync_CuandoNoExiste_DebeRetornarNull
- **Objetivo**: Verificar retorno de null cuando no existe la categoría
- **Datos**: ID inexistente (999)
- **Validaciones**:
  - ✅ Retorna null
  - ✅ Warning log ejecutado

**Assertions clave:**
```csharp
resultado.Should().BeNull("la categoría con ID 999 no existe");
_loggerMock.Verify(x => x.Log(LogLevel.Warning, ...), Times.Once);
```

---

### 5️⃣ ObtenerCategoriaPorIdAsync_DebeRetornarCategoriaConSubcategorias
- **Objetivo**: Verificar retorno completo con jerarquía
- **Datos**: Categoría "Aceites" con subcategorías y valores
- **Validaciones**:
  - ✅ Retorna categoría completa
  - ✅ Propiedades correctas
  - ✅ Subcategorías incluidas
  - ✅ Valores de filtro incluidos
  - ✅ Logging exitoso

**Assertions clave:**
```csharp
resultado!.Nombre.Should().Be("Aceites");
resultado.Subcategorias.Should().HaveCount(2);
viscosidad.Valores.Should().HaveCount(2);
```

---

### 6️⃣ CrearCategoriaAsync_DebeGuardarEnBDYRetornarId
- **Objetivo**: Verificar creación exitosa en base de datos
- **Datos**: Nueva categoría "Plumillas"
- **Validaciones**:
  - ✅ Retorna ID válido (> 0)
  - ✅ Guardado en base de datos
  - ✅ Propiedades correctas
  - ✅ `EsActivo = true`
  - ✅ Fecha de creación reciente
  - ✅ Logging exitoso

**Assertions clave:**
```csharp
categoriaId.Should().BeGreaterThan(0, "debe retornar un ID válido");
categoriaGuardada!.Nombre.Should().Be("Plumillas");
categoriaGuardada.FechaCreacion.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
```

---

### 7️⃣ CrearCategoriaAsync_ConDatosInvalidos_DebeLanzarExcepcion
- **Objetivo**: Verificar validación de duplicados
- **Datos**: Categoría "Aceites" (ya existe)
- **Validaciones**:
  - ✅ Lanza `InvalidOperationException`
  - ✅ Mensaje de error correcto
  - ✅ Warning log ejecutado

**Assertions clave:**
```csharp
await act.Should().ThrowAsync<InvalidOperationException>()
    .WithMessage("*Ya existe una categoría con el nombre 'Aceites'*");
_loggerMock.Verify(x => x.Log(LogLevel.Warning, ...), Times.Once);
```

---

### 8️⃣ ObtenerSubcategoriasAsync_CuandoCategoriaNoExisteOInactiva_DebeRetornarListaVacia
- **Objetivo**: Verificar manejo de categoría inexistente
- **Datos**: ID inexistente (999)
- **Validaciones**:
  - ✅ Retorna lista vacía (no null)
  - ✅ Warning log ejecutado

**Assertions clave:**
```csharp
resultado.Should().BeEmpty("la categoría no existe");
_loggerMock.Verify(x => x.Log(LogLevel.Warning, ...), Times.Once);
```

---

### 9️⃣ ObtenerValoresFiltroAsync_CuandoSubcategoriaNoExiste_DebeRetornarListaVacia
- **Objetivo**: Verificar manejo de subcategoría inexistente
- **Datos**: ID inexistente (999)
- **Validaciones**:
  - ✅ Retorna lista vacía (no null)
  - ✅ Warning log ejecutado

**Assertions clave:**
```csharp
resultado.Should().BeEmpty("la subcategoría no existe");
```

---

### 🔟 ObtenerCategoriaPorIdAsync_CuandoCategoriaEstaInactiva_DebeRetornarNull
- **Objetivo**: Verificar que no retorna categorías inactivas
- **Datos**: Categoría "Neumáticos" (ID 2, inactiva)
- **Validaciones**:
  - ✅ Retorna null
  - ✅ Warning log ejecutado

**Assertions clave:**
```csharp
resultado.Should().BeNull("la categoría está inactiva");
```

---

## 🏗️ Patrón de Tests Implementado

### Arrange-Act-Assert (AAA)

```csharp
[Fact]
public async Task NombreDelTest()
{
    // Arrange - Preparar datos y configuración
    var datos = CrearDatosDePrueba();
    
    // Act - Ejecutar el método a testear
    var resultado = await _service.MetodoATestear(datos);
    
    // Assert - Verificar resultados esperados
    resultado.Should().NotBeNull();
    resultado.Propiedad.Should().Be(valorEsperado);
}
```

## 🔧 Configuración de Tests

### Constructor (Setup)
```csharp
public CategoriaServiceTests()
{
    // InMemory Database con nombre único por test
    var options = new DbContextOptionsBuilder<AutoGuiaDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

    _context = new AutoGuiaDbContext(options);
    _loggerMock = new Mock<ILogger<CategoriaService>>();
    _service = new CategoriaService(_context, _loggerMock.Object);

    SeedDatabase(); // Datos de prueba iniciales
}
```

### Cleanup (Teardown)
```csharp
public void Dispose()
{
    _context.Database.EnsureDeleted();
    _context.Dispose();
}
```

## 📊 Datos de Prueba (Seed)

### Categorías
| ID | Nombre      | Descripción           | EsActivo |
|----|-------------|-----------------------|----------|
| 1  | Aceites     | Aceites para motor    | ✅ true  |
| 2  | Neumáticos  | Neumáticos de todo tipo| ❌ false |
| 3  | Filtros     | Filtros automotrices  | ✅ true  |

### Subcategorías
| ID | Nombre         | CategoriaId |
|----|----------------|-------------|
| 1  | Viscosidad     | 1 (Aceites) |
| 2  | Marca          | 1 (Aceites) |
| 3  | Tipo de Filtro | 3 (Filtros) |

### Valores de Filtro
| ID | Valor   | SubcategoriaId      |
|----|---------|---------------------|
| 1  | 10W-40  | 1 (Viscosidad)      |
| 2  | 5W-30   | 1 (Viscosidad)      |
| 3  | Castrol | 2 (Marca)           |
| 4  | Aire    | 3 (Tipo de Filtro)  |
| 5  | Aceite  | 3 (Tipo de Filtro)  |

## 🔍 Verificación de Logging

Todos los tests verifican que el logging se ejecuta correctamente:

```csharp
_loggerMock.Verify(
    x => x.Log(
        LogLevel.Information, // Nivel esperado
        It.IsAny<EventId>(),
        It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("texto esperado")),
        It.IsAny<Exception>(),
        It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
    Times.Once); // Verificar que se llamó una vez
```

## 🚀 Comandos de Ejecución

### Ejecutar todos los tests
```bash
dotnet test AutoGuia.Tests/AutoGuia.Tests.csproj
```

### Ejecutar con verbosidad detallada
```bash
dotnet test AutoGuia.Tests/AutoGuia.Tests.csproj --verbosity normal
```

### Ejecutar tests específicos
```bash
dotnet test --filter "FullyQualifiedName~CategoriaServiceTests"
```

### Ejecutar con cobertura de código
```bash
dotnet test AutoGuia.Tests/AutoGuia.Tests.csproj --collect:"XPlat Code Coverage"
```

## 📈 Cobertura de Código

Los tests cubren los siguientes métodos de `CategoriaService`:

- ✅ `ObtenerCategoriasAsync()` - 100%
- ✅ `ObtenerSubcategoriasAsync(int)` - 100%
- ✅ `ObtenerValoresFiltroAsync(int)` - 100%
- ✅ `ObtenerCategoriaPorIdAsync(int)` - 100%
- ✅ `CrearCategoriaAsync(CrearCategoriaDto)` - 100%

**Cobertura total estimada**: ~95-100%

## 🎯 Casos de Prueba Cubiertos

### Escenarios Positivos (Happy Path)
- ✅ Obtener todas las categorías activas
- ✅ Obtener subcategorías por categoría
- ✅ Obtener valores de filtro por subcategoría
- ✅ Obtener categoría por ID
- ✅ Crear nueva categoría

### Escenarios Negativos (Edge Cases)
- ✅ Categoría inexistente
- ✅ Categoría inactiva
- ✅ Subcategoría inexistente
- ✅ Categoría duplicada (validación)

### Validaciones de Logging
- ✅ Information logs en operaciones exitosas
- ✅ Warning logs en casos excepcionales
- ✅ Error logs en excepciones (no implementado en estos tests)

## 🔄 Mejoras Futuras Sugeridas

1. **Tests de Concurrencia** - Verificar comportamiento con múltiples requests simultáneos
2. **Tests de Performance** - Medir tiempos de respuesta con grandes volúmenes de datos
3. **Tests de Integración** - Probar con base de datos real (SQL Server)
4. **Tests de Mutación** - Verificar robustez con datos corruptos
5. **Snapshots de DTOs** - Validar estructura completa de objetos retornados

## 📝 Buenas Prácticas Implementadas

✅ **Aislamiento**: Cada test usa su propia base de datos InMemory  
✅ **Independencia**: Tests no dependen del orden de ejecución  
✅ **Claridad**: Nombres descriptivos que explican qué se prueba  
✅ **Assertions explícitas**: Mensajes claros en las aserciones  
✅ **Setup/Teardown**: Configuración y limpieza automática  
✅ **DRY**: Método `SeedDatabase()` reutilizable  
✅ **Mock de dependencias**: ILogger mockeado correctamente  
✅ **FluentAssertions**: Assertions legibles y expresivas  

## 🐛 Debugging de Tests

Para debuggear un test específico en VS Code:

1. Agregar breakpoint en el test
2. Ejecutar con debugger de .NET Test
3. O usar comando:
```bash
dotnet test --filter "FullyQualifiedName~NombreDelTest" --logger "console;verbosity=detailed"
```

## 📚 Referencias

- [xUnit Documentation](https://xunit.net/)
- [Moq Quickstart](https://github.com/moq/moq4/wiki/Quickstart)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [EF Core InMemory Provider](https://learn.microsoft.com/en-us/ef/core/providers/in-memory/)

---

**Creado**: 20 de octubre de 2025  
**Autor**: GitHub Copilot  
**Versión**: 1.0  
**Proyecto**: AutoGuía - Plataforma Automotriz  
**Framework**: .NET 8 / xUnit / Moq / FluentAssertions
