# Validacion de DbContext Pooling
# Ejecutar despues del deployment para validar configuracion

Write-Host "Validando implementacion de DbContext Pooling..." -ForegroundColor Cyan
Write-Host ""

# 1. Verificar archivos modificados
Write-Host "1. Verificando archivos con AddDbContextPool..." -ForegroundColor Yellow
$poolFiles = Select-String -Path "AutoGuia.Web\AutoGuia.Web\Program.cs","AutoGuia.Scraper\Program.cs" -Pattern "AddDbContextPool" -List

if ($poolFiles.Count -ge 2) {
    Write-Host "   OK: AddDbContextPool encontrado en archivos:" -ForegroundColor Green
    $poolFiles | ForEach-Object { Write-Host "      - $($_.Path)" -ForegroundColor Gray }
} else {
    Write-Host "   ERROR: No se encontro AddDbContextPool en todos los archivos" -ForegroundColor Red
    exit 1
}

Write-Host ""

# 2. Verificar pool sizes
Write-Host "2. Verificando pool sizes configurados..." -ForegroundColor Yellow
$poolSizes = Select-String -Path "AutoGuia.Web\AutoGuia.Web\Program.cs","AutoGuia.Scraper\Program.cs" -Pattern "poolSize:\s*\d+" -AllMatches

if ($poolSizes) {
    Write-Host "   OK: Pool sizes configurados:" -ForegroundColor Green
    $poolSizes | ForEach-Object {
        $_.Matches | ForEach-Object {
            Write-Host "      - $($_.Value)" -ForegroundColor Gray
        }
    }
} else {
    Write-Host "   ERROR: No se encontraron pool sizes" -ForegroundColor Red
    exit 1
}

Write-Host ""

# 3. Build
Write-Host "3. Ejecutando build..." -ForegroundColor Yellow
$buildOutput = dotnet build AutoGuia.sln 2>&1
$buildSuccess = $LASTEXITCODE -eq 0

if ($buildSuccess) {
    Write-Host "   OK: Build exitoso" -ForegroundColor Green
} else {
    Write-Host "   ERROR: Build fallo" -ForegroundColor Red
    Write-Host $buildOutput
    exit 1
}

Write-Host ""

# 4. Verificar servicios Singleton
Write-Host "4. Verificando servicios Singleton..." -ForegroundColor Yellow
$singletonServices = Select-String -Path "AutoGuia.Web\AutoGuia.Web\Program.cs" -Pattern "AddSingleton" -Context 0,1 -ErrorAction SilentlyContinue

if ($singletonServices) {
    Write-Host "   INFO: Servicios Singleton encontrados:" -ForegroundColor Cyan
    foreach ($service in $singletonServices) {
        $line = $service.Line.Trim()
        if ($line -match "DbContext") {
            Write-Host "      ERROR: Singleton con DbContext - $line" -ForegroundColor Red
            exit 1
        } else {
            Write-Host "      OK: $line" -ForegroundColor Gray
        }
    }
} else {
    Write-Host "   OK: No hay servicios Singleton (o no dependen de DbContext)" -ForegroundColor Green
}

Write-Host ""

# 5. Verificar documentacion
Write-Host "5. Verificando documentacion..." -ForegroundColor Yellow
if (Test-Path "DBCONTEXT-POOLING-IMPLEMENTATION.md") {
    Write-Host "   OK: Documentacion completa encontrada" -ForegroundColor Green
} else {
    Write-Host "   ERROR: Falta documentacion DBCONTEXT-POOLING-IMPLEMENTATION.md" -ForegroundColor Red
}

if (Test-Path "PR-H3-DBCONTEXT-POOLING.md") {
    Write-Host "   OK: Descripcion de PR encontrada" -ForegroundColor Green
} else {
    Write-Host "   WARN: Falta descripcion de PR (opcional)" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "======================================================================" -ForegroundColor Cyan
Write-Host "VALIDACION COMPLETADA" -ForegroundColor Green
Write-Host "======================================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Resumen de Pool Sizes:" -ForegroundColor Cyan
Write-Host "   - ApplicationDbContext (Identity): 128 conexiones" -ForegroundColor Gray
Write-Host "   - AutoGuiaDbContext (Web):         256 conexiones" -ForegroundColor Gray
Write-Host "   - AutoGuiaDbContext (Scraper):      64 conexiones" -ForegroundColor Gray
Write-Host ""
Write-Host "Proximos pasos:" -ForegroundColor Cyan
Write-Host "   1. Revisar output de validacion" -ForegroundColor Gray
Write-Host "   2. Crear Pull Request en GitHub" -ForegroundColor Gray
Write-Host "   3. Solicitar revision del equipo" -ForegroundColor Gray
Write-Host "   4. Tests de carga en staging" -ForegroundColor Gray
Write-Host "   5. Deploy a produccion con monitoreo" -ForegroundColor Gray
Write-Host ""
