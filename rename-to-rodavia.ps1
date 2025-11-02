# Script para renombrar AutoGuía a Rodavia
# Este script renombra todos los proyectos, namespaces, referencias y documentación

param(
    [switch]$DryRun = $false  # Modo de prueba sin hacer cambios reales
)

$ErrorActionPreference = "Stop"

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "  RENOMBRADO: AutoGuía -> Rodavia" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

if ($DryRun) {
    Write-Host "MODO DRY-RUN: No se harán cambios reales" -ForegroundColor Yellow
    Write-Host ""
}

$projectRoot = $PSScriptRoot

# Paso 1: Renombrar carpetas de proyectos
Write-Host "[1/8] Renombrando carpetas de proyectos..." -ForegroundColor Green

$foldersToRename = @(
    @{Old="AutoGuia.Core"; New="Rodavia.Core"},
    @{Old="AutoGuia.Infrastructure"; New="Rodavia.Infrastructure"},
    @{Old="AutoGuia.Web"; New="Rodavia.Web"},
    @{Old="AutoGuia.Scraper"; New="Rodavia.Scraper"},
    @{Old="AutoGuia.Scraper.Tests"; New="Rodavia.Scraper.Tests"},
    @{Old="AutoGuia.Tests"; New="Rodavia.Tests"},
    @{Old="autoguia"; New="rodavia"}
)

foreach ($folder in $foldersToRename) {
    $oldPath = Join-Path $projectRoot $folder.Old
    $newPath = Join-Path $projectRoot $folder.New
    
    if (Test-Path $oldPath) {
        Write-Host "  Renombrando: $($folder.Old) -> $($folder.New)" -ForegroundColor White
        if (-not $DryRun) {
            Rename-Item -Path $oldPath -NewName $folder.New -Force
        }
    }
}

Write-Host ""

# Paso 2: Renombrar archivos .sln
Write-Host "[2/8] Renombrando archivo de solución..." -ForegroundColor Green

$oldSln = Join-Path $projectRoot "AutoGuia.sln"
$newSln = Join-Path $projectRoot "Rodavia.sln"

if (Test-Path $oldSln) {
    Write-Host "  Renombrando: AutoGuia.sln -> Rodavia.sln" -ForegroundColor White
    if (-not $DryRun) {
        Rename-Item -Path $oldSln -NewName "Rodavia.sln" -Force
    }
}

Write-Host ""

# Paso 3: Actualizar contenido de archivos .sln
Write-Host "[3/8] Actualizando referencias en archivos .sln..." -ForegroundColor Green

if ((Test-Path $newSln) -and (-not $DryRun)) {
    $slnContent = Get-Content $newSln -Raw -Encoding UTF8
    $slnContent = $slnContent -replace 'AutoGuia\.', 'Rodavia.'
    $slnContent = $slnContent -replace 'AutoGuia\\', 'Rodavia\'
    $slnContent = $slnContent -replace 'autoguia', 'rodavia'
    Set-Content -Path $newSln -Value $slnContent -Encoding UTF8 -NoNewline
    Write-Host "  Actualizado: Rodavia.sln" -ForegroundColor White
}

Write-Host ""

# Paso 4: Actualizar archivos .csproj
Write-Host "[4/8] Actualizando archivos .csproj..." -ForegroundColor Green

$csprojFiles = Get-ChildItem -Path $projectRoot -Filter "*.csproj" -Recurse
foreach ($file in $csprojFiles) {
    $content = Get-Content $file.FullName -Raw -Encoding UTF8
    $newContent = $content -replace 'AutoGuia\.', 'Rodavia.'
    $newContent = $newContent -replace 'AutoGuia\\', 'Rodavia\'
    $newContent = $newContent -replace '>AutoGuia<', '>Rodavia<'
    
    if ($content -ne $newContent) {
        Write-Host "  Actualizado: $($file.Name)" -ForegroundColor White
        if (-not $DryRun) {
            Set-Content -Path $file.FullName -Value $newContent -Encoding UTF8 -NoNewline
        }
    }
}

Write-Host ""

# Paso 5: Actualizar namespaces en archivos .cs
Write-Host "[5/8] Actualizando namespaces en archivos .cs..." -ForegroundColor Green

$csFiles = Get-ChildItem -Path $projectRoot -Filter "*.cs" -Recurse | Where-Object { $_.DirectoryName -notmatch '\\obj\\|\\bin\\' }
$csCount = 0

foreach ($file in $csFiles) {
    $content = Get-Content $file.FullName -Raw -Encoding UTF8
    $newContent = $content -replace 'namespace AutoGuia\.', 'namespace Rodavia.'
    $newContent = $newContent -replace 'using AutoGuia\.', 'using Rodavia.'
    
    if ($content -ne $newContent) {
        $csCount++
        if (-not $DryRun) {
            Set-Content -Path $file.FullName -Value $newContent -Encoding UTF8 -NoNewline
        }
    }
}

Write-Host "  Actualizados: $csCount archivos .cs" -ForegroundColor White
Write-Host ""

# Paso 6: Actualizar archivos .razor
Write-Host "[6/8] Actualizando archivos .razor..." -ForegroundColor Green

$razorFiles = Get-ChildItem -Path $projectRoot -Filter "*.razor" -Recurse | Where-Object { $_.DirectoryName -notmatch '\\obj\\|\\bin\\' }
$razorCount = 0

foreach ($file in $razorFiles) {
    $content = Get-Content $file.FullName -Raw -Encoding UTF8
    $newContent = $content -replace '@using AutoGuia\.', '@using Rodavia.'
    $newContent = $newContent -replace 'AutoGuia\.', 'Rodavia.'
    $newContent = $newContent -replace 'AutoGuía', 'Rodavia'
    $newContent = $newContent -replace 'autoguia', 'rodavia'
    
    if ($content -ne $newContent) {
        $razorCount++
        if (-not $DryRun) {
            Set-Content -Path $file.FullName -Value $newContent -Encoding UTF8 -NoNewline
        }
    }
}

Write-Host "  Actualizados: $razorCount archivos .razor" -ForegroundColor White
Write-Host ""

# Paso 7: Actualizar archivos de configuración (json, yml, env, etc.)
Write-Host "[7/8] Actualizando archivos de configuración..." -ForegroundColor Green

$configPatterns = @("*.json", "*.yml", "*.yaml", "*.env", "*.example", "*.ps1", "*.sh", "*.sql", "*.md")
$configCount = 0

foreach ($pattern in $configPatterns) {
    $files = Get-ChildItem -Path $projectRoot -Filter $pattern -Recurse | Where-Object { 
        $_.DirectoryName -notmatch '\\obj\\|\\bin\\|\\node_modules\\|\\.git\\' -and
        $_.Name -ne "rename-to-rodavia.ps1"
    }
    
    foreach ($file in $files) {
        $content = Get-Content $file.FullName -Raw -Encoding UTF8
        $newContent = $content -replace 'AutoGuia\.', 'Rodavia.'
        $newContent = $newContent -replace 'AutoGuia\\', 'Rodavia\'
        $newContent = $newContent -replace 'autoguia', 'rodavia'
        $newContent = $newContent -replace 'AutoGuía', 'Rodavia'
        $newContent = $newContent -replace 'AUTOGUIA', 'RODAVIA'
        $newContent = $newContent -replace 'admin@autoguia\.cl', 'admin@rodavia.cl'
        $newContent = $newContent -replace 'autoguia-', 'rodavia-'
        $newContent = $newContent -replace 'autoguia_', 'rodavia_'
        $newContent = $newContent -replace 'blazorautoguia', 'blazorrodavia'
        
        if ($content -ne $newContent) {
            $configCount++
            if (-not $DryRun) {
                Set-Content -Path $file.FullName -Value $newContent -Encoding UTF8 -NoNewline
            }
        }
    }
}

Write-Host "  Actualizados: $configCount archivos de configuración" -ForegroundColor White
Write-Host ""

# Paso 8: Renombrar scripts específicos
Write-Host "[8/8] Renombrando scripts específicos..." -ForegroundColor Green

$scriptsToRename = @(
    @{Old="backup-autoguia.ps1"; New="backup-rodavia.ps1"},
    @{Old="restore-autoguia.ps1"; New="restore-rodavia.ps1"}
)

foreach ($script in $scriptsToRename) {
    $oldPath = Join-Path $projectRoot $script.Old
    $newPath = Join-Path $projectRoot $script.New
    
    if (Test-Path $oldPath) {
        Write-Host "  Renombrando: $($script.Old) -> $($script.New)" -ForegroundColor White
        if (-not $DryRun) {
            Rename-Item -Path $oldPath -NewName $script.New -Force
        }
    }
}

Write-Host ""

# Resumen
Write-Host "=========================================" -ForegroundColor Green
Write-Host "  RENOMBRADO COMPLETADO" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Green
Write-Host ""

if ($DryRun) {
    Write-Host "Este fue un DRY-RUN. Ejecuta sin -DryRun para aplicar cambios." -ForegroundColor Yellow
} else {
    Write-Host "Cambios aplicados:" -ForegroundColor White
    Write-Host "   Carpetas de proyectos renombradas" -ForegroundColor White
    Write-Host "   Archivo .sln actualizado" -ForegroundColor White
    Write-Host "   Archivos .csproj actualizados" -ForegroundColor White
    Write-Host "   $csCount archivos .cs actualizados" -ForegroundColor White
    Write-Host "   $razorCount archivos .razor actualizados" -ForegroundColor White
    Write-Host "   $configCount archivos de configuración actualizados" -ForegroundColor White
    Write-Host ""
    Write-Host "SIGUIENTE PASO:" -ForegroundColor Yellow
    Write-Host "  1. Ejecutar: dotnet build Rodavia.sln" -ForegroundColor White
    Write-Host "  2. Verificar que no hay errores de compilación" -ForegroundColor White
    Write-Host "  3. Hacer commit de los cambios" -ForegroundColor White
}

Write-Host ""
