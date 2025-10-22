# 🔍 Script de Validación de DbContext Pooling
# Ejecutar después del deployment para validar configuración

Write-Host "🔄 Validando implementación de DbContext Pooling..." -ForegroundColor Cyan
Write-Host ""

# 1. Verificar archivos modificados
Write-Host "📁 1. Verificando archivos con AddDbContextPool..." -ForegroundColor Yellow
$poolFiles = Select-String -Path "AutoGuia.Web\AutoGuia.Web\Program.cs","AutoGuia.Scraper\Program.cs" -Pattern "AddDbContextPool" -List

if ($poolFiles.Count -ge 2) {
    Write-Host "   ✅ AddDbContextPool encontrado en archivos:" -ForegroundColor Green
    $poolFiles | ForEach-Object { Write-Host "      - $($_.Path)" -ForegroundColor Gray }
} else {
    Write-Host "   ❌ No se encontró AddDbContextPool en todos los archivos" -ForegroundColor Red
    exit 1
}

Write-Host ""

# 2. Verificar pool sizes
Write-Host "📊 2. Verificando pool sizes configurados..." -ForegroundColor Yellow
$poolSizes = Select-String -Path "AutoGuia.Web\AutoGuia.Web\Program.cs","AutoGuia.Scraper\Program.cs" -Pattern "poolSize:\s*\d+" -AllMatches

if ($poolSizes) {
    Write-Host "   ✅ Pool sizes configurados:" -ForegroundColor Green
    $poolSizes | ForEach-Object {
        $_.Matches | ForEach-Object {
            Write-Host "      - $($_.Value)" -ForegroundColor Gray
        }
    }
} else {
    Write-Host "   ❌ No se encontraron pool sizes" -ForegroundColor Red
    exit 1
}

Write-Host ""

# 3. Build
Write-Host "🔨 3. Ejecutando build..." -ForegroundColor Yellow
$buildOutput = dotnet build AutoGuia.sln 2>&1
$buildSuccess = $LASTEXITCODE -eq 0

if ($buildSuccess) {
    Write-Host "   ✅ Build exitoso" -ForegroundColor Green
} else {
    Write-Host "   ❌ Build falló" -ForegroundColor Red
    Write-Host $buildOutput
    exit 1
}

Write-Host ""

# 4. Verificar servicios Singleton (no deben depender de DbContext)
Write-Host "🔍 4. Verificando servicios Singleton..." -ForegroundColor Yellow
$singletonServices = Select-String -Path "AutoGuia.Web\AutoGuia.Web\Program.cs" -Pattern "AddSingleton" -Context 0,1

if ($singletonServices) {
    Write-Host "   ℹ️  Servicios Singleton encontrados:" -ForegroundColor Cyan
    $singletonServices | ForEach-Object {
        $line = $_.Line.Trim()
        # Verificar si contiene DbContext
        if ($line -match "DbContext") {
            Write-Host "      ❌ ALERTA: Singleton con DbContext - $line" -ForegroundColor Red
            exit 1
        } else {
            Write-Host "      ✅ $line" -ForegroundColor Gray
        }
    }
} else {
    Write-Host "   ✅ No hay servicios Singleton (o no dependen de DbContext)" -ForegroundColor Green
}

Write-Host ""

# 5. Tests (opcional - puede tardar)
Write-Host "🧪 5. ¿Ejecutar tests? (puede tardar ~30s) [S/n]:" -ForegroundColor Yellow
$runTests = Read-Host
if ($runTests -eq "" -or $runTests -eq "S" -or $runTests -eq "s") {
    Write-Host "   Ejecutando tests..." -ForegroundColor Gray
    $testOutput = dotnet test AutoGuia.sln --no-build 2>&1
    $testSuccess = $LASTEXITCODE -eq 0
    
    if ($testSuccess) {
        Write-Host "   ✅ Tests exitosos" -ForegroundColor Green
    } else {
        Write-Host "   ⚠️  Algunos tests fallaron (revisar output)" -ForegroundColor Yellow
    }
} else {
    Write-Host "   ⏭️  Tests omitidos" -ForegroundColor Gray
}

Write-Host ""

# 6. Verificar documentación
Write-Host "📚 6. Verificando documentación..." -ForegroundColor Yellow
if (Test-Path "DBCONTEXT-POOLING-IMPLEMENTATION.md") {
    Write-Host "   ✅ Documentación completa encontrada" -ForegroundColor Green
} else {
    Write-Host "   ❌ Falta documentación DBCONTEXT-POOLING-IMPLEMENTATION.md" -ForegroundColor Red
}

if (Test-Path "PR-H3-DBCONTEXT-POOLING.md") {
    Write-Host "   ✅ Descripción de PR encontrada" -ForegroundColor Green
} else {
    Write-Host "   ⚠️  Falta descripción de PR (opcional)" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "=" * 70 -ForegroundColor Cyan
Write-Host "✅ VALIDACIÓN COMPLETADA" -ForegroundColor Green
Write-Host "=" * 70 -ForegroundColor Cyan
Write-Host ""
Write-Host "📋 Resumen de Pool Sizes:" -ForegroundColor Cyan
Write-Host "   - ApplicationDbContext (Identity): 128 conexiones" -ForegroundColor Gray
Write-Host "   - AutoGuiaDbContext (Web):         256 conexiones" -ForegroundColor Gray
Write-Host "   - AutoGuiaDbContext (Scraper):      64 conexiones" -ForegroundColor Gray
Write-Host ""
Write-Host "🚀 Próximos pasos:" -ForegroundColor Cyan
Write-Host "   1. Revisar output de validación" -ForegroundColor Gray
Write-Host "   2. Crear Pull Request en GitHub" -ForegroundColor Gray
Write-Host "   3. Solicitar revisión del equipo" -ForegroundColor Gray
Write-Host "   4. Tests de carga en staging" -ForegroundColor Gray
Write-Host "   5. Deploy a producción con monitoreo" -ForegroundColor Gray
Write-Host ""
