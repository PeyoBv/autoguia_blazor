# Rodavia.Tests 🧪

Proyecto de tests unitarios para **Rodavia** - Plataforma Automotriz.

## 📦 Tecnologías

- **.NET 8** - Framework de desarrollo
- **xUnit** - Framework de testing
- **Moq** - Mocking framework
- **FluentAssertions** - Assertions expresivas
- **Entity Framework Core InMemory** - Base de datos en memoria para tests

## 🗂️ Estructura

```
Rodavia.Tests/
├── Rodavia.Tests.csproj          # Proyecto de tests
├── Services/
│   └── CategoriaServiceTests.cs   # Tests de CategoriaService (10 tests)
└── README.md                      # Este archivo
```

## ✅ Estado Actual

| Servicio           | Tests | Estado | Cobertura |
|--------------------|-------|--------|-----------|
| CategoriaService   | 10    | ✅ Pass | ~95%      |

**Total**: 10 tests, 10 passing ✅

## 🚀 Comandos

### Ejecutar todos los tests
```bash
dotnet test
```

### Ejecutar con detalle
```bash
dotnet test --verbosity normal
```

### Ejecutar tests específicos
```bash
dotnet test --filter "FullyQualifiedName~CategoriaServiceTests"
```

### Ver cobertura de código
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## 📝 Convenciones

### Nomenclatura de Tests
```
[NombreMetodo]_[Escenario]_[ResultadoEsperado]
```

**Ejemplos:**
- `ObtenerCategoriasAsync_DebeRetornarSoloCategoriasActivas()`
- `CrearCategoriaAsync_ConDatosInvalidos_DebeLanzarExcepcion()`

### Patrón AAA (Arrange-Act-Assert)
```csharp
[Fact]
public async Task NombreDelTest()
{
    // Arrange - Preparar datos
    var datos = ...;
    
    // Act - Ejecutar método
    var resultado = await _service.Metodo(datos);
    
    // Assert - Verificar resultado
    resultado.Should().NotBeNull();
}
```

## 🎯 Tests por Servicio

### CategoriaServiceTests (10 tests)

1. ✅ ObtenerCategoriasAsync_DebeRetornarSoloCategoriasActivas
2. ✅ ObtenerSubcategoriasAsync_DebeRetornarSubcategoriasPorCategoriaId
3. ✅ ObtenerValoresFiltroAsync_DebeRetornarValoresPorSubcategoriaId
4. ✅ ObtenerCategoriaPorIdAsync_CuandoNoExiste_DebeRetornarNull
5. ✅ ObtenerCategoriaPorIdAsync_DebeRetornarCategoriaConSubcategorias
6. ✅ CrearCategoriaAsync_DebeGuardarEnBDYRetornarId
7. ✅ CrearCategoriaAsync_ConDatosInvalidos_DebeLanzarExcepcion
8. ✅ ObtenerSubcategoriasAsync_CuandoCategoriaNoExisteOInactiva_DebeRetornarListaVacia
9. ✅ ObtenerValoresFiltroAsync_CuandoSubcategoriaNoExiste_DebeRetornarListaVacia
10. ✅ ObtenerCategoriaPorIdAsync_CuandoCategoriaEstaInactiva_DebeRetornarNull

## 📚 Documentación

Para más detalles sobre los tests de CategoriaService, ver:
- [CATEGORIA-SERVICE-TESTS.md](../CATEGORIA-SERVICE-TESTS.md)

## 🔧 Configuración

### Dependencias en .csproj
```xml
<PackageReference Include="xunit" Version="2.5.3" />
<PackageReference Include="Moq" Version="4.20.72" />
<PackageReference Include="FluentAssertions" Version="8.7.1" />
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="8.0.11" />
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
```

### Referencias de Proyectos
```xml
<ProjectReference Include="..\Rodavia.Infrastructure\Rodavia.Infrastructure.csproj" />
<ProjectReference Include="..\Rodavia.Core\Rodavia.Core.csproj" />
```

## 🐛 Debugging

### VS Code
1. Abrir archivo de test
2. Colocar breakpoint
3. Click en "Debug Test" sobre el método

### Terminal
```bash
dotnet test --filter "NombreDelTest" --logger "console;verbosity=detailed"
```

## 📈 Próximos Tests a Implementar

- [ ] ProductoServiceTests
- [ ] ComparadorServiceTests
- [ ] TallerServiceTests
- [ ] ForoServiceTests

## 🤝 Contribuir

Al agregar nuevos tests:

1. Seguir patrón AAA
2. Usar nomenclatura consistente
3. Mockear dependencias externas
4. Usar FluentAssertions para assertions
5. Agregar documentación XML
6. Verificar que todos los tests pasen

## 📊 Ejemplo de Test

```csharp
/// <summary>
/// Verificar que ObtenerCategoriasAsync retorna solo categorías activas
/// </summary>
[Fact]
public async Task ObtenerCategoriasAsync_DebeRetornarSoloCategoriasActivas()
{
    // Arrange
    // (Datos ya preparados en constructor con SeedDatabase())

    // Act
    var resultado = await _service.ObtenerCategoriasAsync();
    var categorias = resultado.ToList();

    // Assert
    categorias.Should().NotBeNull();
    categorias.Should().HaveCount(2, "solo hay 2 categorías activas");
    categorias.Should().Contain(c => c.Nombre == "Aceites");
    categorias.Should().NotContain(c => c.Nombre == "Neumáticos");
}
```

---

**Proyecto**: Rodavia  
**Framework**: .NET 8  
**Última actualización**: 20 de octubre de 2025
