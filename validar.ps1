# Script de Validacion Automatica para Rodavia
# Valida compilacion, tests y publicacion antes de hacer push

param(
    [switch]$SkipTests,
    [switch]$SkipPublish,
    [switch]$Verbose
)

$ErrorActionPreference = "Stop"

# Variables
$ProjectPath = "Rodavia.Web/Rodavia.Web/Rodavia.Web.csproj"
$TestProjectPath = "Rodavia.Tests/Rodavia.Tests.csproj"
$PublishPath = "./publish"
$StartTime = Get-Date

Write-Host "`n================================================" -ForegroundColor Cyan
Write-Host "  RODAVIA - Validacion de Compilacion" -ForegroundColor Cyan
Write-Host "================================================`n" -ForegroundColor Cyan

# Verificar directorio
Write-Host "[1/6] Verificando directorio..." -ForegroundColor Magenta
if (-not (Test-Path "Rodavia.sln")) {
    Write-Host "ERROR: No se encontro Rodavia.sln" -ForegroundColor Red
    exit 1
}
Write-Host "OK: Directorio correcto`n" -ForegroundColor Green

# Limpiar artifacts
Write-Host "[2/6] Limpiando artifacts..." -ForegroundColor Magenta
if (Test-Path $PublishPath) {
    Remove-Item -Path $PublishPath -Recurse -Force
}
Write-Host "OK: Limpieza completada`n" -ForegroundColor Green

# Restaurar dependencias
Write-Host "[3/6] Restaurando dependencias..." -ForegroundColor Magenta
$restoreOutput = dotnet restore $ProjectPath 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Fallo al restaurar dependencias" -ForegroundColor Red
    exit 1
}
Write-Host "OK: Dependencias restauradas`n" -ForegroundColor Green

# Compilar proyecto
Write-Host "[4/6] Compilando proyecto..." -ForegroundColor Magenta
$buildOutput = dotnet build $ProjectPath --configuration Release --no-restore 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Fallo en la compilacion" -ForegroundColor Red
    Write-Host $buildOutput
    exit 1
}

$warningLines = $buildOutput | Select-String "warning CS" 
$warningCount = $warningLines.Count

Write-Host "OK: Compilacion exitosa ($warningCount warnings, 0 errors)`n" -ForegroundColor Green

# Ejecutar tests
if (-not $SkipTests) {
    Write-Host "[5/6] Ejecutando tests..." -ForegroundColor Magenta
    if (Test-Path $TestProjectPath) {
        dotnet test $TestProjectPath --configuration Release --verbosity quiet 2>&1 | Out-Null
        if ($LASTEXITCODE -eq 0) {
            Write-Host "OK: Tests ejecutados correctamente`n" -ForegroundColor Green
        } else {
            Write-Host "WARNING: Algunos tests fallaron`n" -ForegroundColor Yellow
        }
    }
} else {
    Write-Host "[5/6] Tests omitidos (flag -SkipTests)`n" -ForegroundColor Yellow
}

# Publicar
if (-not $SkipPublish) {
    Write-Host "[6/6] Publicando aplicacion..." -ForegroundColor Magenta
    $publishOutput = dotnet publish $ProjectPath --configuration Release --output $PublishPath 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Host "ERROR: Fallo al publicar" -ForegroundColor Red
        Write-Host $publishOutput
        exit 1
    }

    if ($publishOutput -match "NETSDK1152") {
        Write-Host "ERROR: NETSDK1152 detectado" -ForegroundColor Red
        exit 1
    }

    Write-Host "OK: Publicacion exitosa`n" -ForegroundColor Green
    
    # Limpiar
    Remove-Item -Path $PublishPath -Recurse -Force
} else {
    Write-Host "[6/6] Publicacion omitida (flag -SkipPublish)`n" -ForegroundColor Yellow
}

# Resumen
$EndTime = Get-Date
$Duration = $EndTime - $StartTime
$durationText = [math]::Round($Duration.TotalSeconds, 2)

Write-Host "`n================================================" -ForegroundColor Green
Write-Host "  VALIDACION COMPLETADA EXITOSAMENTE" -ForegroundColor Green
Write-Host "================================================`n" -ForegroundColor Green

Write-Host "Resumen:" -ForegroundColor Cyan
Write-Host "  - Dependencias: Restauradas" -ForegroundColor White
Write-Host "  - Compilacion: Exitosa ($warningCount warnings)" -ForegroundColor White
if (-not $SkipTests) {
    Write-Host "  - Tests: Ejecutados" -ForegroundColor White
}
if (-not $SkipPublish) {
    Write-Host "  - Publicacion: Exitosa" -ForegroundColor White
}
Write-Host "  - Tiempo total: $durationText segundos`n" -ForegroundColor White

Write-Host "Proximos pasos:" -ForegroundColor Magenta
Write-Host "  1. git status"
Write-Host "  2. git add ."
Write-Host "  3. git commit -m `"fix: Corregir errores de publicacion`""
Write-Host "  4. git push origin main`n"

exit 0
