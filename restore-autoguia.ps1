<#
.SYNOPSIS
    Script de restauración para AutoGuía - PostgreSQL y Configuración

.DESCRIPTION
    Restaura backups completos de:
    - Base de datos autoguia_dev (PostgreSQL)
    - Base de datos identity_dev (PostgreSQL)
    - Archivos de configuración (appsettings.*.json)

.PARAMETER BackupDate
    Fecha del backup a restaurar en formato YYYY-MM-DD_HHmmss o YYYY-MM-DD (busca el más reciente)

.PARAMETER BackupPath
    Ruta donde están los backups. Por defecto: .\backups

.PARAMETER Force
    Sobrescribe bases de datos existentes sin confirmar. PELIGROSO!

.EXAMPLE
    .\restore-autoguia.ps1 -BackupDate "2025-10-22"
    Restaura el backup más reciente del 22 de octubre de 2025

.EXAMPLE
    .\restore-autoguia.ps1 -BackupDate "2025-10-22_143052" -Force
    Restaura backup específico sin confirmación
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)]
    [string]$BackupDate,
    
    [string]$BackupPath = ".\backups",
    [switch]$Force
)

# Configuración
$ErrorActionPreference = "Stop"

# Banner
Write-Host ""
Write-Host "======================================================================" -ForegroundColor Cyan
Write-Host "  AUTOGUIA - RESTAURACION DE BACKUP" -ForegroundColor Cyan
Write-Host "======================================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "ADVERTENCIA: Este proceso ELIMINARA datos existentes!" -ForegroundColor Red
Write-Host ""

# Directorios
$dbBackupPath = Join-Path $BackupPath "database"
$configBackupPath = Join-Path $BackupPath "config"
$logsPath = Join-Path $BackupPath "logs"

# Archivo de log
$logFile = Join-Path $logsPath "restore_$(Get-Date -Format 'yyyy-MM-dd_HHmmss').log"

function Write-Log {
    param([string]$Message, [string]$Level = "INFO")
    
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $logMessage = "[$timestamp] [$Level] $Message"
    
    if (Test-Path $logsPath) {
        Add-Content -Path $logFile -Value $logMessage
    }
    
    switch ($Level) {
        "ERROR" { Write-Host "ERROR: $Message" -ForegroundColor Red }
        "WARN"  { Write-Host "WARN: $Message" -ForegroundColor Yellow }
        "SUCCESS" { Write-Host "OK: $Message" -ForegroundColor Green }
        default { Write-Host "INFO: $Message" -ForegroundColor Gray }
    }
}

Write-Log "========== INICIO DE RESTAURACION =========="
Write-Log "Fecha de backup solicitada: $BackupDate"

# Verificar que psql esté disponible
try {
    $psqlVersion = & psql --version 2>&1
    Write-Log "PostgreSQL client encontrado: $psqlVersion"
} catch {
    Write-Log "ERROR: psql no encontrado. Instalar PostgreSQL client tools." "ERROR"
    exit 1
}

# Buscar archivos de backup
Write-Log "Buscando archivos de backup..."

# Función para encontrar backup más reciente
function Find-LatestBackup {
    param([string]$DatabaseName, [string]$SearchPattern)
    
    $backups = Get-ChildItem -Path $dbBackupPath -Filter "${DatabaseName}_${SearchPattern}*" | Sort-Object Name -Descending
    
    if ($backups.Count -eq 0) {
        return $null
    }
    
    # Buscar .sql.gz primero, luego .sql
    $gzBackup = $backups | Where-Object { $_.Name -match "\.sql\.gz$" } | Select-Object -First 1
    if ($gzBackup) {
        return $gzBackup
    }
    
    $sqlBackup = $backups | Where-Object { $_.Name -match "\.sql$" } | Select-Object -First 1
    return $sqlBackup
}

$autoguiaBackup = Find-LatestBackup -DatabaseName "autoguia_dev" -SearchPattern $BackupDate
$identityBackup = Find-LatestBackup -DatabaseName "identity_dev" -SearchPattern $BackupDate

if (-not $autoguiaBackup) {
    Write-Log "No se encontro backup de autoguia_dev para fecha: $BackupDate" "ERROR"
    Write-Log "Backups disponibles:"
    Get-ChildItem -Path $dbBackupPath -Filter "autoguia_dev_*" | ForEach-Object { Write-Log "  - $($_.Name)" }
    exit 1
}

if (-not $identityBackup) {
    Write-Log "No se encontro backup de identity_dev para fecha: $BackupDate" "ERROR"
    exit 1
}

Write-Log "Backup encontrado (autoguia_dev): $($autoguiaBackup.Name)" "SUCCESS"
Write-Log "Backup encontrado (identity_dev): $($identityBackup.Name)" "SUCCESS"

# Buscar backup de configuración
$configBackup = Get-ChildItem -Path $configBackupPath -Filter "appsettings_${BackupDate}*" | Sort-Object Name -Descending | Select-Object -First 1

if ($configBackup) {
    Write-Log "Backup encontrado (config): $($configBackup.Name)" "SUCCESS"
} else {
    Write-Log "No se encontro backup de configuracion para fecha: $BackupDate" "WARN"
}

# Confirmación
if (-not $Force) {
    Write-Host ""
    Write-Host "Se restauraran los siguientes backups:" -ForegroundColor Yellow
    Write-Host "  - $($autoguiaBackup.Name)" -ForegroundColor Gray
    Write-Host "  - $($identityBackup.Name)" -ForegroundColor Gray
    if ($configBackup) {
        Write-Host "  - $($configBackup.Name)" -ForegroundColor Gray
    }
    Write-Host ""
    $confirmation = Read-Host "Desea continuar? (S/N)"
    
    if ($confirmation -ne "S" -and $confirmation -ne "s") {
        Write-Log "Restauracion cancelada por el usuario" "WARN"
        exit 0
    }
}

Write-Host ""
Write-Host "1. RESTAURACION DE BASES DE DATOS" -ForegroundColor Yellow
Write-Host "----------------------------------------------------------------------" -ForegroundColor Gray

# Configuración de conexión PostgreSQL
$pgHost = "localhost"
$pgUser = "postgres"

# Función para restaurar base de datos
function Restore-Database {
    param(
        [string]$DatabaseName,
        [int]$Port,
        [System.IO.FileInfo]$BackupFile
    )
    
    Write-Log "Restaurando base de datos: $DatabaseName (puerto $Port)"
    
    $sqlFile = $BackupFile.FullName
    
    # Descomprimir si es necesario
    if ($sqlFile -match "\.gz$") {
        Write-Log "Descomprimiendo backup..."
        $uncompressedFile = $sqlFile -replace "\.gz$", ""
        
        if (Get-Command "7z" -ErrorAction SilentlyContinue) {
            & 7z e "$sqlFile" "-o$($BackupFile.DirectoryName)" -y | Out-Null
            $sqlFile = $uncompressedFile
            Write-Log "Backup descomprimido: $sqlFile"
        } else {
            Write-Log "7-Zip no disponible, no se puede descomprimir" "ERROR"
            return $false
        }
    }
    
    try {
        # Paso 1: Terminar conexiones activas
        Write-Log "Terminando conexiones activas..."
        $terminateSQL = @"
SELECT pg_terminate_backend(pid)
FROM pg_stat_activity
WHERE datname = '$DatabaseName' AND pid <> pg_backend_pid();
"@
        
        $terminateArgs = @(
            "-h", $pgHost,
            "-p", $Port,
            "-U", $pgUser,
            "-d", "postgres",
            "-c", $terminateSQL
        )
        
        & psql @terminateArgs 2>&1 | Out-Null
        
        # Paso 2: Drop database si existe
        Write-Log "Eliminando base de datos existente..."
        $dropArgs = @(
            "-h", $pgHost,
            "-p", $Port,
            "-U", $pgUser,
            "-d", "postgres",
            "-c", "DROP DATABASE IF EXISTS $DatabaseName;"
        )
        
        $dropProcess = Start-Process -FilePath "psql" -ArgumentList $dropArgs -Wait -NoNewWindow -PassThru
        
        if ($dropProcess.ExitCode -eq 0) {
            Write-Log "Base de datos eliminada: $DatabaseName"
        }
        
        # Paso 3: Crear database
        Write-Log "Creando nueva base de datos..."
        $createArgs = @(
            "-h", $pgHost,
            "-p", $Port,
            "-U", $pgUser,
            "-d", "postgres",
            "-c", "CREATE DATABASE $DatabaseName OWNER postgres;"
        )
        
        $createProcess = Start-Process -FilePath "psql" -ArgumentList $createArgs -Wait -NoNewWindow -PassThru
        
        if ($createProcess.ExitCode -ne 0) {
            Write-Log "Error al crear base de datos" "ERROR"
            return $false
        }
        
        Write-Log "Base de datos creada: $DatabaseName"
        
        # Paso 4: Restaurar desde backup
        Write-Log "Restaurando datos desde backup..."
        $restoreArgs = @(
            "-h", $pgHost,
            "-p", $Port,
            "-U", $pgUser,
            "-d", $DatabaseName,
            "-f", $sqlFile
        )
        
        $restoreProcess = Start-Process -FilePath "psql" -ArgumentList $restoreArgs -Wait -NoNewWindow -PassThru -RedirectStandardError "$logsPath\psql_error_restore.log"
        
        if ($restoreProcess.ExitCode -eq 0) {
            Write-Log "Restauracion completada: $DatabaseName" "SUCCESS"
            
            # Limpiar archivo descomprimido si se creó
            if ($BackupFile.FullName -match "\.gz$" -and (Test-Path $uncompressedFile)) {
                Remove-Item $uncompressedFile -Force
            }
            
            return $true
        } else {
            Write-Log "Error durante restauracion. Ver log: $logsPath\psql_error_restore.log" "ERROR"
            return $false
        }
        
    } catch {
        Write-Log "Excepcion durante restauracion: $_" "ERROR"
        return $false
    }
}

# Restaurar autoguia_dev (puerto 5433)
$autoguiaSuccess = Restore-Database -DatabaseName "autoguia_dev" -Port 5433 -BackupFile $autoguiaBackup

# Restaurar identity_dev (puerto 5434)
$identitySuccess = Restore-Database -DatabaseName "identity_dev" -Port 5434 -BackupFile $identityBackup

Write-Host ""
Write-Host "2. RESTAURACION DE CONFIGURACION" -ForegroundColor Yellow
Write-Host "----------------------------------------------------------------------" -ForegroundColor Gray

$configSuccess = $false

if ($configBackup) {
    try {
        Write-Log "Restaurando archivos de configuracion..."
        
        $tempConfigDir = Join-Path $env:TEMP "autoguia_restore_$(Get-Date -Format 'yyyyMMdd_HHmmss')"
        New-Item -ItemType Directory -Path $tempConfigDir -Force | Out-Null
        
        # Descomprimir
        Expand-Archive -Path $configBackup.FullName -DestinationPath $tempConfigDir -Force
        
        # Copiar archivos a sus ubicaciones originales
        $restoredFiles = Get-ChildItem -Path $tempConfigDir -File
        foreach ($file in $restoredFiles) {
            if ($file.Name -match "^appsettings") {
                # Determinar destino según nombre
                $destination = ""
                if ($file.Name -match "Scraper") {
                    $destination = "AutoGuia.Scraper\$($file.Name)"
                } else {
                    $destination = "AutoGuia.Web\AutoGuia.Web\$($file.Name)"
                }
                
                if (Test-Path $destination) {
                    # Backup del archivo actual
                    $backupName = "$destination.backup_$(Get-Date -Format 'yyyyMMdd_HHmmss')"
                    Copy-Item -Path $destination -Destination $backupName -Force
                    Write-Log "Backup creado: $backupName"
                }
                
                Copy-Item -Path $file.FullName -Destination $destination -Force
                Write-Log "Restaurado: $destination"
            }
        }
        
        # Limpiar temporal
        Remove-Item -Path $tempConfigDir -Recurse -Force
        
        Write-Log "Configuracion restaurada exitosamente" "SUCCESS"
        $configSuccess = $true
        
    } catch {
        Write-Log "Error al restaurar configuracion: $_" "ERROR"
    }
} else {
    Write-Log "No hay backup de configuracion para restaurar" "WARN"
    $configSuccess = $true  # No fallar si no hay config
}

# Resumen
Write-Host ""
Write-Host "======================================================================" -ForegroundColor Cyan
Write-Host "  RESUMEN DE RESTAURACION" -ForegroundColor Cyan
Write-Host "======================================================================" -ForegroundColor Cyan

$totalSuccess = $autoguiaSuccess -and $identitySuccess -and $configSuccess

if ($autoguiaSuccess) {
    Write-Host "  [OK] Base de datos autoguia_dev" -ForegroundColor Green
} else {
    Write-Host "  [FAIL] Base de datos autoguia_dev" -ForegroundColor Red
}

if ($identitySuccess) {
    Write-Host "  [OK] Base de datos identity_dev" -ForegroundColor Green
} else {
    Write-Host "  [FAIL] Base de datos identity_dev" -ForegroundColor Red
}

if ($configSuccess) {
    Write-Host "  [OK] Archivos de configuracion" -ForegroundColor Green
} else {
    Write-Host "  [FAIL] Archivos de configuracion" -ForegroundColor Red
}

Write-Host ""
Write-Host "  Log de ejecucion: $logFile" -ForegroundColor Gray
Write-Host ""

if ($totalSuccess) {
    Write-Host "RESTAURACION COMPLETADA EXITOSAMENTE" -ForegroundColor Green
    Write-Host ""
    Write-Host "Proximos pasos:" -ForegroundColor Yellow
    Write-Host "  1. Reiniciar aplicacion: dotnet run --project AutoGuia.Web/AutoGuia.Web" -ForegroundColor Gray
    Write-Host "  2. Validar datos recuperados" -ForegroundColor Gray
    Write-Host "  3. Verificar configuracion" -ForegroundColor Gray
    Write-Host ""
    Write-Log "========== RESTAURACION EXITOSA ==========" "SUCCESS"
    exit 0
} else {
    Write-Host "RESTAURACION COMPLETADA CON ERRORES - Revisar log" -ForegroundColor Yellow
    Write-Log "========== RESTAURACION CON ERRORES ==========" "WARN"
    exit 1
}
