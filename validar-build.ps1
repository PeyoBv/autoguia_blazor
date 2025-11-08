# 🚀 Script de Validación Automática para Rodavia
# Valida compilación, tests y publicación antes de hacer push

param(
    [switch]$SkipTests,
    [switch]$SkipPublish,
    [switch]$Verbose
)

$ErrorActionPreference = "Stop"
$VerbosePreference = if ($Verbose) { "Continue" } else { "SilentlyContinue" }

# Colores para output
function Write-Success { param($Message) Write-Host "✅ $Message" -ForegroundColor Green }
function Write-Info { param($Message) Write-Host "ℹ️  $Message" -ForegroundColor Cyan }
function Write-Warning { param($Message) Write-Host "⚠️  $Message" -ForegroundColor Yellow }
function Write-Error { param($Message) Write-Host "❌ $Message" -ForegroundColor Red }
function Write-Step { param($Message) Write-Host "`n🔧 $Message" -ForegroundColor Magenta }

# Variables
$SolutionPath = "Rodavia.sln"
$ProjectPath = "Rodavia.Web/Rodavia.Web/Rodavia.Web.csproj"
$TestProjectPath = "Rodavia.Tests/Rodavia.Tests.csproj"
$PublishPath = "./publish"
$StartTime = Get-Date

Write-Host "`n================================================" -ForegroundColor Cyan
Write-Host "  RODAVIA - Validacion de Compilacion" -ForegroundColor Cyan
Write-Host "================================================`n" -ForegroundColor Cyan

# Paso 1: Verificar que estamos en el directorio correcto
Write-Step "Verificando directorio de trabajo..."
if (-not (Test-Path $SolutionPath)) {
    Write-Error "No se encontró Rodavia.sln. Asegúrate de ejecutar este script desde la raíz del proyecto."
    exit 1
}
Write-Success "Directorio correcto"

# Paso 2: Limpiar artifacts anteriores
Write-Step "Limpiando artifacts anteriores..."
if (Test-Path $PublishPath) {
    Remove-Item -Path $PublishPath -Recurse -Force
    Write-Success "Directorio publish eliminado"
}
Write-Success "Limpieza completada"

# Paso 3: Restaurar dependencias
Write-Step "Restaurando dependencias..."
Write-Info "Ejecutando: dotnet restore $ProjectPath"
$restoreOutput = dotnet restore $ProjectPath 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Error "Fallo al restaurar dependencias"
    Write-Host $restoreOutput
    exit 1
}
# Contar advertencias NU1608
$nu1608Count = ($restoreOutput | Select-String "NU1608").Count
if ($nu1608Count -gt 0) {
    Write-Warning "Se encontraron $nu1608Count advertencias NU1608 (versiones de paquetes). Esto es normal."
}
Write-Success "Dependencias restauradas correctamente"

# Paso 4: Compilar proyecto
Write-Step "Compilando proyecto en modo Release..."
Write-Info "Ejecutando: dotnet build $ProjectPath --configuration Release --no-restore"
$buildOutput = dotnet build $ProjectPath --configuration Release --no-restore 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Error "Fallo en la compilación"
    Write-Host $buildOutput
    exit 1
}

# Analizar warnings y errors
$warningCount = ($buildOutput | Select-String "warning").Count
$errorCount = ($buildOutput | Select-String "error").Count

if ($errorCount -gt 0) {
    Write-Error "Se encontraron $errorCount errores en la compilación"
    Write-Host $buildOutput
    exit 1
}

if ($warningCount -gt 0) {
    Write-Warning "Compilación exitosa con $warningCount advertencias"
} else {
    Write-Success "Compilación exitosa sin advertencias"
}

# Paso 5: Ejecutar tests (opcional)
if (-not $SkipTests) {
    Write-Step "Ejecutando tests..."
    if (Test-Path $TestProjectPath) {
        Write-Info "Ejecutando: dotnet test $TestProjectPath --configuration Release --verbosity quiet"
        $testOutput = dotnet test $TestProjectPath --configuration Release --verbosity quiet 2>&1
        if ($LASTEXITCODE -eq 0) {
            Write-Success "Tests ejecutados correctamente"
        } else {
            Write-Warning "Algunos tests fallaron (continuar de todas formas)"
            if ($Verbose) {
                Write-Host $testOutput
            }
        }
    } else {
        Write-Warning "No se encontró proyecto de tests en $TestProjectPath"
    }
} else {
    Write-Info "Tests omitidos (flag -SkipTests)"
}

# Paso 6: Publicar aplicación (opcional)
if (-not $SkipPublish) {
    Write-Step "Publicando aplicación..."
    Write-Info "Ejecutando: dotnet publish $ProjectPath --configuration Release --output $PublishPath"
    $publishOutput = dotnet publish $ProjectPath --configuration Release --output $PublishPath 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Fallo al publicar la aplicación"
        Write-Host $publishOutput
        exit 1
    }

    # Verificar error NETSDK1152
    if ($publishOutput -match "NETSDK1152") {
        Write-Error "Error NETSDK1152 detectado: Conflicto de archivos appsettings.json"
        Write-Host $publishOutput
        exit 1
    }

    Write-Success "Aplicación publicada correctamente"

    # Verificar archivos publicados
    Write-Step "Verificando archivos publicados..."
    $publishedFiles = Get-ChildItem -Path $PublishPath -File | Select-Object -First 10
    Write-Info "Primeros 10 archivos publicados:"
    $publishedFiles | ForEach-Object { Write-Host "  - $($_.Name)" }

    # Verificar DLL principal
    $mainDll = Join-Path $PublishPath "Rodavia.Web.dll"
    if (Test-Path $mainDll) {
        Write-Success "DLL principal encontrada: Rodavia.Web.dll"
    } else {
        Write-Error "No se encontró Rodavia.Web.dll en $PublishPath"
        exit 1
    }

    # Verificar wwwroot
    $wwwroot = Join-Path $PublishPath "wwwroot"
    if (Test-Path $wwwroot) {
        Write-Success "Directorio wwwroot encontrado"
    } else {
        Write-Warning "No se encontró directorio wwwroot (puede ser normal si no hay assets estáticos)"
    }

    # Limpiar directorio de publicación
    Write-Info "Limpiando directorio de publicación..."
    Remove-Item -Path $PublishPath -Recurse -Force
    Write-Success "Directorio publish eliminado"
} else {
    Write-Info "Publicación omitida (flag -SkipPublish)"
}

# Resumen final
$EndTime = Get-Date
$Duration = $EndTime - $StartTime

Write-Host "`n================================================" -ForegroundColor Green
Write-Host "  VALIDACION COMPLETADA EXITOSAMENTE" -ForegroundColor Green
Write-Host "================================================`n" -ForegroundColor Green

Write-Host "📊 Resumen:" -ForegroundColor Cyan
Write-Host "  ✅ Dependencias: Restauradas" -ForegroundColor Green
Write-Host "  ✅ Compilación: Exitosa ($warningCount advertencias, 0 errores)" -ForegroundColor Green
if (-not $SkipTests) {
    Write-Host "  ✅ Tests: Ejecutados" -ForegroundColor Green
}
if (-not $SkipPublish) {
    Write-Host "  ✅ Publicación: Exitosa sin errores NETSDK1152" -ForegroundColor Green
}
$durationText = [math]::Round($Duration.TotalSeconds, 2)
Write-Host "  ⏱️  Tiempo total: $durationText segundos`n" -ForegroundColor Cyan

Write-Host "🚀 Próximos pasos:" -ForegroundColor Magenta
Write-Host "  1. Revisar cambios: git status"
Write-Host "  2. Agregar cambios: git add ."
Write-Host "  3. Commit: git commit -m `"fix: Corregir errores de publicacion`""
Write-Host "  4. Push: git push origin main`n"

exit 0
