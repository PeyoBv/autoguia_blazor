<#
.SYNOPSIS
    Script de backup automático para AutoGuía - PostgreSQL y Configuración

.DESCRIPTION
    Crea backups completos de:
    - Base de datos autoguia_dev (PostgreSQL)
    - Base de datos identity_dev (PostgreSQL)
    - Archivos de configuración (appsettings.*.json)
    - User secrets (si existen)

.PARAMETER BackupPath
    Ruta donde se guardarán los backups. Por defecto: .\backups

.PARAMETER SkipCompression
    Omite la compresión de backups SQL con gzip. Por defecto: comprime

.PARAMETER RetentionDays
    Días de retención de backups antiguos. Por defecto: 30

.EXAMPLE
    .\backup-autoguia.ps1
    Crea un backup completo comprimido en .\backups

.EXAMPLE
    .\backup-autoguia.ps1 -BackupPath "D:\Backups" -RetentionDays 60 -SkipCompression
    Crea backup en ruta personalizada con retención de 60 días sin compresión
#>

[CmdletBinding()]
param(
    [string]$BackupPath = ".\backups",
    [switch]$SkipCompression,
    [int]$RetentionDays = 30
)

# Configuración de colores para output
$ErrorActionPreference = "Stop"
$ProgressPreference = "SilentlyContinue"

# Banner
Write-Host ""
Write-Host "======================================================================" -ForegroundColor Cyan
Write-Host "  AUTOGUIA - BACKUP AUTOMATICO" -ForegroundColor Cyan
Write-Host "======================================================================" -ForegroundColor Cyan
Write-Host ""

# Timestamp para nombres de archivo
$timestamp = Get-Date -Format "yyyy-MM-dd_HHmmss"
$date = Get-Date -Format "yyyy-MM-dd"

# Crear directorios si no existen
$dbBackupPath = Join-Path $BackupPath "database"
$configBackupPath = Join-Path $BackupPath "config"
$logsPath = Join-Path $BackupPath "logs"

@($dbBackupPath, $configBackupPath, $logsPath) | ForEach-Object {
    if (-not (Test-Path $_)) {
        New-Item -ItemType Directory -Path $_ -Force | Out-Null
        Write-Host "OK: Directorio creado: $_" -ForegroundColor Green
    }
}

# Archivo de log
$logFile = Join-Path $logsPath "backup_$date.log"

function Write-Log {
    param([string]$Message, [string]$Level = "INFO")
    
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $logMessage = "[$timestamp] [$Level] $Message"
    Add-Content -Path $logFile -Value $logMessage
    
    switch ($Level) {
        "ERROR" { Write-Host "ERROR: $Message" -ForegroundColor Red }
        "WARN"  { Write-Host "WARN: $Message" -ForegroundColor Yellow }
        "SUCCESS" { Write-Host "OK: $Message" -ForegroundColor Green }
        default { Write-Host "INFO: $Message" -ForegroundColor Gray }
    }
}

Write-Log "========== INICIO DE BACKUP =========="
Write-Log "Timestamp: $timestamp"
Write-Log "Ruta de backup: $BackupPath"
Write-Log "Retencion: $RetentionDays dias"

# Verificar que pg_dump esté disponible
try {
    $pgDumpVersion = & pg_dump --version 2>&1
    Write-Log "PostgreSQL client encontrado: $pgDumpVersion"
} catch {
    Write-Log "ERROR: pg_dump no encontrado. Instalar PostgreSQL client tools." "ERROR"
    Write-Log "Descarga: https://www.postgresql.org/download/" "ERROR"
    exit 1
}

# Configuración de conexión PostgreSQL
$pgHost = "localhost"
$pgUser = "postgres"

# IMPORTANTE: El password se lee de la variable de entorno PGPASSWORD
# Configurar antes de ejecutar este script:
# PowerShell: $env:PGPASSWORD = "tu_password"
# CMD: set PGPASSWORD=tu_password
# GitHub Actions: usar secrets.POSTGRES_PASSWORD

Write-Host ""
Write-Host "1. BACKUP DE BASES DE DATOS" -ForegroundColor Yellow
Write-Host "----------------------------------------------------------------------" -ForegroundColor Gray

# Función para hacer backup de una base de datos
function Backup-Database {
    param(
        [string]$DatabaseName,
        [int]$Port,
        [string]$OutputPath
    )
    
    Write-Log "Iniciando backup de base de datos: $DatabaseName (puerto $Port)"
    
    $backupFile = Join-Path $OutputPath "${DatabaseName}_$timestamp.sql"
    
    try {
        # Ejecutar pg_dump
        $pgDumpArgs = @(
            "-h", $pgHost,
            "-p", $Port,
            "-U", $pgUser,
            "-d", $DatabaseName,
            "--no-password",
            "-F", "p",  # Formato plain (SQL)
            "--verbose",
            "-f", $backupFile
        )
        
        Write-Log "Ejecutando: pg_dump $($pgDumpArgs -join ' ')"
        
        $process = Start-Process -FilePath "pg_dump" -ArgumentList $pgDumpArgs -Wait -NoNewWindow -PassThru -RedirectStandardError "$logsPath\pg_dump_error_$date.log"
        
        if ($process.ExitCode -eq 0) {
            $fileSize = (Get-Item $backupFile).Length / 1MB
            Write-Log "Backup completado: $backupFile (${fileSize:N2} MB)" "SUCCESS"
            
            # Comprimir si no se omitió
            if (-not $SkipCompression) {
                Write-Log "Comprimiendo backup..."
                $gzipFile = "$backupFile.gz"
                
                # Usar 7-Zip si está disponible, sino intentar con Compress-Archive
                if (Get-Command "7z" -ErrorAction SilentlyContinue) {
                    & 7z a -tgzip "$gzipFile" "$backupFile" | Out-Null
                    Remove-Item $backupFile -Force
                    $compressedSize = (Get-Item $gzipFile).Length / 1MB
                    Write-Log "Backup comprimido: $gzipFile (${compressedSize:N2} MB)" "SUCCESS"
                } else {
                    Write-Log "7-Zip no disponible, backup sin comprimir" "WARN"
                }
            }
            
            return $true
        } else {
            Write-Log "Error en pg_dump. Ver log: $logsPath\pg_dump_error_$date.log" "ERROR"
            return $false
        }
        
    } catch {
        Write-Log "Excepcion durante backup: $_" "ERROR"
        return $false
    }
}

# Backup de autoguia_dev (puerto 5433)
$autoguiaSuccess = Backup-Database -DatabaseName "autoguia_dev" -Port 5433 -OutputPath $dbBackupPath

# Backup de identity_dev (puerto 5434)
$identitySuccess = Backup-Database -DatabaseName "identity_dev" -Port 5434 -OutputPath $dbBackupPath

Write-Host ""
Write-Host "2. BACKUP DE CONFIGURACION" -ForegroundColor Yellow
Write-Host "----------------------------------------------------------------------" -ForegroundColor Gray

# Backup de archivos de configuración
$configFiles = @(
    "AutoGuia.Web\AutoGuia.Web\appsettings.json",
    "AutoGuia.Web\AutoGuia.Web\appsettings.Development.json",
    "AutoGuia.Web\AutoGuia.Web\appsettings.Production.json",
    "AutoGuia.Scraper\appsettings.json",
    "AutoGuia.Scraper\appsettings.Production.json"
)

$configBackupFile = Join-Path $configBackupPath "appsettings_$timestamp.zip"

try {
    Write-Log "Creando backup de archivos de configuracion..."
    
    $tempConfigDir = Join-Path $env:TEMP "autoguia_config_$timestamp"
    New-Item -ItemType Directory -Path $tempConfigDir -Force | Out-Null
    
    foreach ($file in $configFiles) {
        if (Test-Path $file) {
            $fileName = Split-Path $file -Leaf
            $destPath = Join-Path $tempConfigDir $fileName
            Copy-Item -Path $file -Destination $destPath -Force
            Write-Log "Copiado: $file"
        }
    }
    
    # Comprimir archivos
    Compress-Archive -Path "$tempConfigDir\*" -DestinationPath $configBackupFile -Force
    $configSize = (Get-Item $configBackupFile).Length / 1KB
    Write-Log "Configuracion respaldada: $configBackupFile (${configSize:N2} KB)" "SUCCESS"
    
    # Limpiar temporal
    Remove-Item -Path $tempConfigDir -Recurse -Force
    
    $configSuccess = $true
} catch {
    Write-Log "Error al respaldar configuracion: $_" "ERROR"
    $configSuccess = $false
}

Write-Host ""
Write-Host "3. LIMPIEZA DE BACKUPS ANTIGUOS" -ForegroundColor Yellow
Write-Host "----------------------------------------------------------------------" -ForegroundColor Gray

# Eliminar backups antiguos (retención)
try {
    Write-Log "Limpiando backups antiguos (retencion: $RetentionDays dias)..."
    
    $cutoffDate = (Get-Date).AddDays(-$RetentionDays)
    
    # Limpiar backups de base de datos
    $oldDbBackups = Get-ChildItem -Path $dbBackupPath -File | Where-Object { $_.LastWriteTime -lt $cutoffDate }
    foreach ($file in $oldDbBackups) {
        Remove-Item $file.FullName -Force
        Write-Log "Eliminado backup antiguo: $($file.Name)"
    }
    
    # Limpiar backups de configuración
    $oldConfigBackups = Get-ChildItem -Path $configBackupPath -File | Where-Object { $_.LastWriteTime -lt $cutoffDate }
    foreach ($file in $oldConfigBackups) {
        Remove-Item $file.FullName -Force
        Write-Log "Eliminado config antiguo: $($file.Name)"
    }
    
    $deletedCount = $oldDbBackups.Count + $oldConfigBackups.Count
    if ($deletedCount -gt 0) {
        Write-Log "Backups antiguos eliminados: $deletedCount" "SUCCESS"
    } else {
        Write-Log "No hay backups antiguos para eliminar"
    }
    
} catch {
    Write-Log "Error durante limpieza: $_" "WARN"
}

# Resumen
Write-Host ""
Write-Host "======================================================================" -ForegroundColor Cyan
Write-Host "  RESUMEN DE BACKUP" -ForegroundColor Cyan
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
Write-Host "  Directorio de backups: $BackupPath" -ForegroundColor Gray
Write-Host "  Log de ejecucion: $logFile" -ForegroundColor Gray
Write-Host ""

if ($totalSuccess) {
    Write-Host "BACKUP COMPLETADO EXITOSAMENTE" -ForegroundColor Green
    Write-Log "========== BACKUP EXITOSO ==========" "SUCCESS"
    exit 0
} else {
    Write-Host "BACKUP COMPLETADO CON ERRORES - Revisar log" -ForegroundColor Yellow
    Write-Log "========== BACKUP CON ERRORES ==========" "WARN"
    exit 1
}
