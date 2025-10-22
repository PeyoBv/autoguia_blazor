# Estrategia de Backups - AutoGuía

## 📋 Índice

1. [Visión General](#visión-general)
2. [Objetivos y SLAs](#objetivos-y-slas)
3. [Componentes Respaldados](#componentes-respaldados)
4. [Procedimientos de Backup](#procedimientos-de-backup)
5. [Procedimientos de Restauración](#procedimientos-de-restauración)
6. [Automatización](#automatización)
7. [Monitoreo y Alertas](#monitoreo-y-alertas)
8. [Política de Retención](#política-de-retención)
9. [Seguridad](#seguridad)
10. [Disaster Recovery](#disaster-recovery)

---

## Visión General

AutoGuía implementa una estrategia de backups multinivel diseñada para proteger datos críticos de negocio contra pérdida accidental, corrupción o desastres. El sistema utiliza **pg_dump** para bases de datos PostgreSQL y archivado comprimido para configuraciones.

### Arquitectura de Backups

```
┌─────────────────────────────────────────────────────┐
│  PRODUCCIÓN                                         │
│                                                     │
│  ┌──────────────┐         ┌──────────────┐        │
│  │ autoguia_dev │         │ identity_dev │        │
│  │  (Port 5433) │         │  (Port 5434) │        │
│  └──────┬───────┘         └──────┬───────┘        │
│         │                        │                 │
│         ▼                        ▼                 │
│  ┌──────────────────────────────────────┐         │
│  │  backup-autoguia.ps1 (Semanal)       │         │
│  │  - pg_dump SQL completo              │         │
│  │  - Compresión gzip                   │         │
│  │  - Validación integridad             │         │
│  └──────────────┬───────────────────────┘         │
└─────────────────┼───────────────────────────────┬─┘
                  │                               │
                  ▼                               ▼
        ┌─────────────────┐           ┌─────────────────┐
        │ LOCAL STORAGE   │           │  CLOUD STORAGE  │
        │ ./backups/      │           │  (Futuro)       │
        │ - Retención 30d │           │  - Retención 1y │
        └─────────────────┘           └─────────────────┘
```

---

## Objetivos y SLAs

### Recovery Time Objective (RTO)

| Componente | RTO Target | Procedimiento |
|-----------|-----------|---------------|
| Base de datos (autoguia_dev) | **< 30 minutos** | Restauración desde backup SQL |
| Base de datos (identity_dev) | **< 30 minutos** | Restauración desde backup SQL |
| Configuración aplicación | **< 5 minutos** | Descompresión de archivos |
| Sistema completo | **< 1 hora** | Restauración completa + validación |

### Recovery Point Objective (RPO)

| Ambiente | RPO | Frecuencia Backup |
|----------|-----|-------------------|
| **Desarrollo** | 7 días | Semanal (GitHub Actions) |
| **Producción** (futuro) | 4 horas | Cada 4 horas + Diario |

### Objetivos de Disponibilidad

- **Uptime Target**: 99.5% mensual
- **Data Loss Tolerance**: Máximo 7 días en desarrollo, 0 en producción
- **Tiempo de Retención**: 30 días local, 1 año cloud (futuro)

---

## Componentes Respaldados

### 1. Bases de Datos PostgreSQL

#### autoguia_dev (Puerto 5433)
- **Contenido**: Datos de aplicación principal
  - Talleres, Vehículos, Categorías
  - Publicaciones de foro y respuestas
  - Productos y consumibles
  - Scrapers y fuentes externas
- **Tamaño Estimado**: 50-500 MB (depende de contenido)
- **Método**: `pg_dump --format=plain --no-owner --no-acl`
- **Compresión**: gzip (ratio ~10:1)

#### identity_dev (Puerto 5434)
- **Contenido**: Sistema de autenticación ASP.NET Identity
  - Usuarios (`AspNetUsers`)
  - Roles (`AspNetRoles`)
  - Claims y relaciones
- **Tamaño Estimado**: 5-50 MB
- **Método**: `pg_dump --format=plain --no-owner --no-acl`
- **Compresión**: gzip

### 2. Archivos de Configuración

#### appsettings.*.json
- `AutoGuia.Web/AutoGuia.Web/appsettings.Production.json`
- `AutoGuia.Web/AutoGuia.Web/appsettings.Development.json`
- `AutoGuia.Scraper/appsettings.Production.json`
- `AutoGuia.Scraper/appsettings.json`

**Contienen**:
- Connection strings (PostgreSQL)
- API keys (Google Maps, MercadoLibre, VIN API)
- Configuración de servicios externos
- Parámetros de rate limiting y caching

**Método**: Archivado ZIP con timestamp

### 3. Exclusiones

❌ **NO se respaldan**:
- Archivos binarios (`bin/`, `obj/`)
- Dependencias NuGet (restaurables con `dotnet restore`)
- Logs temporales
- Archivos de caché local
- Secretos en `.env` (usar Azure Key Vault en producción)

---

## Procedimientos de Backup

### Backup Manual

```powershell
# Backup completo con configuración por defecto
.\backup-autoguia.ps1

# Backup sin compresión (más rápido, más espacio)
.\backup-autoguia.ps1 -SkipCompression

# Backup con retención personalizada (60 días)
.\backup-autoguia.ps1 -RetentionDays 60

# Backup solo base de datos (sin config)
.\backup-autoguia.ps1 -DatabaseOnly
```

### Proceso Paso a Paso

**1. Pre-backup**
```powershell
# Verificar espacio en disco
Get-PSDrive C | Select-Object Used,Free

# Verificar PostgreSQL está corriendo
Get-Process -Name postgres -ErrorAction SilentlyContinue

# Verificar herramientas
pg_dump --version
```

**2. Ejecución**
```powershell
# Ejecutar script de backup
.\backup-autoguia.ps1

# Salida esperada:
# ======================================================================
#   AUTOGUIA - BACKUP COMPLETO
# ======================================================================
# [OK] Backup de base de datos autoguia_dev completado
# [OK] Backup de base de datos identity_dev completado
# [OK] Backup de configuracion completado
# [OK] Limpieza de backups antiguos completada
```

**3. Post-backup**
```powershell
# Verificar archivos generados
Get-ChildItem .\backups\database\ | Select-Object Name, Length, LastWriteTime

# Validar integridad
7z t .\backups\database\autoguia_dev_YYYY-MM-DD_HHmmss.sql.gz
```

### Estructura de Archivos Generados

```
backups/
├── database/
│   ├── autoguia_dev_2025-10-22_143052.sql.gz
│   ├── identity_dev_2025-10-22_143052.sql.gz
│   └── ...
├── config/
│   ├── appsettings_2025-10-22_143052.zip
│   └── ...
└── logs/
    ├── backup_2025-10-22_143052.log
    └── ...
```

---

## Procedimientos de Restauración

### Restauración Manual

```powershell
# Listar backups disponibles
Get-ChildItem .\backups\database\ | Sort-Object LastWriteTime -Descending

# Restauración completa (con confirmación)
.\restore-autoguia.ps1 -BackupDate "2025-10-22"

# Restauración específica sin confirmación (PELIGROSO)
.\restore-autoguia.ps1 -BackupDate "2025-10-22_143052" -Force
```

### Proceso Paso a Paso

**1. Pre-restauración**
```powershell
# IMPORTANTE: Hacer backup del estado actual antes de restaurar
.\backup-autoguia.ps1

# Detener aplicación (evitar conexiones activas)
Stop-Process -Name "AutoGuia.Web" -ErrorAction SilentlyContinue
```

**2. Ejecución**
```powershell
# Ejecutar script de restauración
.\restore-autoguia.ps1 -BackupDate "2025-10-22"

# El script:
# 1. Busca archivos de backup
# 2. Solicita confirmación
# 3. Termina conexiones activas
# 4. Elimina bases de datos existentes
# 5. Crea nuevas bases de datos
# 6. Restaura datos desde pg_dump
# 7. Restaura archivos de configuración
```

**3. Post-restauración**
```powershell
# Reiniciar aplicación
dotnet run --project AutoGuia.Web\AutoGuia.Web\AutoGuia.Web.csproj

# Verificar logs
Get-Content .\backups\logs\restore_*.log -Tail 50

# Validar datos
psql -h localhost -p 5433 -U postgres -d autoguia_dev -c "SELECT COUNT(*) FROM talleres;"
```

### Escenarios de Recuperación

#### Escenario 1: Corrupción de Datos (Tabla Específica)
```sql
-- Restaurar a DB temporal
CREATE DATABASE autoguia_temp;
-- Usar psql para restaurar backup
\i backups/database/autoguia_dev_YYYY-MM-DD.sql

-- Copiar tabla específica
INSERT INTO autoguia_dev.talleres 
SELECT * FROM autoguia_temp.talleres;

-- Limpiar
DROP DATABASE autoguia_temp;
```

#### Escenario 2: Migración Fallida
```powershell
# Restaurar versión pre-migración
.\restore-autoguia.ps1 -BackupDate "2025-10-21" -Force

# Revertir código
git reset --hard HEAD~1
```

#### Escenario 3: Disaster Recovery Completo
```powershell
# 1. Reinstalar PostgreSQL
# 2. Clonar repositorio
git clone https://github.com/usuario/AutoGuia.git

# 3. Copiar backups desde almacenamiento externo
# 4. Restaurar
.\restore-autoguia.ps1 -BackupDate "ULTIMO-BACKUP"

# 5. Reconstruir aplicación
dotnet restore
dotnet build
```

---

## Automatización

### GitHub Actions (Semanal)

Archivo: `.github/workflows/backups-cron.yml`

```yaml
name: Backups Automáticos Semanales

on:
  schedule:
    # Cada domingo a las 3:00 AM UTC
    - cron: '0 3 * * 0'
  workflow_dispatch:

jobs:
  backup:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Ejecutar Backup
        run: .\backup-autoguia.ps1
        env:
          PGPASSWORD: ${{ secrets.POSTGRES_PASSWORD }}
      
      - name: Upload Backups (Artifact)
        uses: actions/upload-artifact@v4
        with:
          name: autoguia-backups-${{ github.run_number }}
          path: backups/**/*.gz
          retention-days: 30
```

### Tarea Programada Windows (Producción)

```powershell
# Crear tarea programada (PowerShell como Administrador)
$action = New-ScheduledTaskAction -Execute "PowerShell.exe" `
  -Argument "-ExecutionPolicy Bypass -File C:\AutoGuia\backup-autoguia.ps1"

$trigger = New-ScheduledTaskTrigger -Daily -At 2:00AM

$principal = New-ScheduledTaskPrincipal -UserId "SYSTEM" -LogonType ServiceAccount

Register-ScheduledTask -TaskName "AutoGuia Backup Diario" `
  -Action $action -Trigger $trigger -Principal $principal `
  -Description "Backup automático de bases de datos AutoGuía"
```

### Validación Post-Backup (Opcional)

```powershell
# Script de validación (validar-backup.ps1)
$latestBackup = Get-ChildItem .\backups\database\autoguia_dev_*.sql.gz | Sort-Object LastWriteTime -Descending | Select-Object -First 1

# Verificar integridad de archivo
$hashActual = Get-FileHash $latestBackup.FullName -Algorithm SHA256
if ($hashActual.Hash) {
    Write-Host "OK: Backup íntegro - Hash SHA256: $($hashActual.Hash.Substring(0,16))..."
} else {
    Write-Host "ERROR: Backup corrupto"
    # Enviar alerta por email
}

# Verificar tamaño mínimo (ej: 1 MB)
if ($latestBackup.Length -lt 1MB) {
    Write-Host "WARN: Backup sospechosamente pequeño ($($latestBackup.Length) bytes)"
}
```

---

## Monitoreo y Alertas

### Métricas Clave

| Métrica | Umbral | Acción |
|---------|--------|--------|
| **Tiempo de ejecución** | > 10 minutos | Investigar performance |
| **Tamaño de backup** | < 1 MB o > 10 GB | Validar integridad |
| **Edad del último backup** | > 8 días | Alerta crítica |
| **Espacio en disco** | < 10 GB libre | Limpiar backups antiguos |
| **Fallos consecutivos** | >= 2 | Alerta de sistema |

### Logs

Todos los logs se almacenan en `backups/logs/`:

```
backup_2025-10-22_143052.log       # Backup exitoso
backup_2025-10-23_030015_error.log # Backup con errores
restore_2025-10-24_091230.log      # Restauración
```

### Dashboard de Estado (Futuro)

Visualización de:
- Últimos 10 backups (fecha, tamaño, duración)
- Gráfico de tendencia de tamaño
- Pruebas de restauración exitosas
- Espacio disponible en storage

---

## Política de Retención

### Retención Local (backups/)

| Antigüedad | Acción |
|-----------|--------|
| 0-7 días | **Mantener todos** |
| 8-30 días | **Mantener semanales** (domingo) |
| > 30 días | **Eliminar** |

Implementado en `backup-autoguia.ps1` con parámetro `-RetentionDays 30`

### Retención Cloud (Futuro - Azure Blob Storage)

| Tier | Antigüedad | Costo | Acceso |
|------|-----------|-------|--------|
| **Hot** | 0-30 días | Alto | Inmediato |
| **Cool** | 31-180 días | Medio | Minutos |
| **Archive** | 181-365 días | Bajo | Horas |

### Backups de Larga Duración

**Hitos importantes** (retención permanente):
- Lanzamiento de versión mayor (v1.0, v2.0)
- Pre-migración de base de datos
- Pre-actualización de dependencias críticas

```powershell
# Backup permanente (sin retención)
.\backup-autoguia.ps1 -RetentionDays 0

# Renombrar para identificación
Rename-Item .\backups\database\autoguia_dev_*.sql.gz -NewName "RELEASE_v1.0_autoguia_dev.sql.gz"
```

---

## Seguridad

### Protección de Backups

#### Permisos de Archivos
```powershell
# Solo administrador puede leer backups
$acl = Get-Acl .\backups
$acl.SetAccessRuleProtection($true, $false)
$adminRule = New-Object System.Security.AccessControl.FileSystemAccessRule(
    "Administrators", "FullControl", "ContainerInherit,ObjectInherit", "None", "Allow"
)
$acl.AddAccessRule($adminRule)
Set-Acl .\backups $acl
```

#### Encriptación (Recomendado para Producción)
```powershell
# Encriptar backup con GPG
gpg --symmetric --cipher-algo AES256 autoguia_dev_*.sql.gz

# Desencriptar para restauración
gpg --decrypt autoguia_dev_*.sql.gz.gpg > autoguia_dev.sql.gz
```

### Secretos en Backups

⚠️ **IMPORTANTE**: Los backups de configuración contienen:
- Connection strings con credenciales
- API keys
- Tokens de autenticación

**Medidas de protección**:
1. ✅ Archivos excluidos de Git (`.gitignore`)
2. ✅ Permisos restrictivos en filesystem
3. 🔄 Encriptación con GPG (implementar en producción)
4. 🔄 Azure Key Vault para secretos (migración futura)

---

## Disaster Recovery

### Plan de Recuperación ante Desastres

#### Paso 1: Evaluación (T+0 minutos)
```
[ ] Identificar naturaleza del problema (corrupción, eliminación, fallo hardware)
[ ] Determinar alcance del daño (tabla, DB completa, sistema)
[ ] Verificar disponibilidad de backups recientes
[ ] Estimar RPO (cuántos datos se perdieron)
```

#### Paso 2: Preparación (T+5 minutos)
```powershell
# Aislar problema
Stop-Service -Name "PostgreSQL"
Stop-Process -Name "AutoGuia.Web"

# Documentar estado actual
Get-Process | Export-Csv estado_pre_recovery.csv

# Hacer backup del estado corrupto (forense)
.\backup-autoguia.ps1 -BackupPath ".\backups-corrupted"
```

#### Paso 3: Restauración (T+10 minutos)
```powershell
# Restaurar desde último backup válido
.\restore-autoguia.ps1 -BackupDate "ULTIMO-VALIDO" -Force

# Verificar logs
Get-Content .\backups\logs\restore_*.log
```

#### Paso 4: Validación (T+30 minutos)
```sql
-- Verificar integridad
SELECT COUNT(*) FROM talleres;
SELECT COUNT(*) FROM "AspNetUsers";

-- Validar datos recientes
SELECT * FROM publicaciones_foro ORDER BY fecha_creacion DESC LIMIT 10;
```

#### Paso 5: Reinicio (T+40 minutos)
```powershell
# Reiniciar servicios
Start-Service -Name "PostgreSQL"
dotnet run --project AutoGuia.Web\AutoGuia.Web\AutoGuia.Web.csproj

# Monitorear logs
Get-Content .\AutoGuia.Web\AutoGuia.Web\logs\autoguia-*.log -Wait
```

#### Paso 6: Post-Mortem (T+24 horas)
```
[ ] Documentar causa raíz del incidente
[ ] Identificar datos perdidos (si aplica)
[ ] Actualizar procedimientos de backup
[ ] Implementar prevenciones adicionales
[ ] Comunicar lecciones aprendidas al equipo
```

### Tiempo Estimado de Recuperación

| Escenario | RTO | RPO | Complejidad |
|-----------|-----|-----|-------------|
| Eliminación accidental de registros | 15 min | 7 días | Baja |
| Corrupción de tabla | 30 min | 7 días | Media |
| Caída de base de datos | 45 min | 7 días | Media |
| Fallo de disco completo | 2-4 horas | 7 días | Alta |

---

## Checklist de Implementación

### Fase 1: Configuración Inicial ✅
- [x] Crear scripts de backup (`backup-autoguia.ps1`)
- [x] Crear scripts de restauración (`restore-autoguia.ps1`)
- [x] Configurar estructura de directorios (`backups/`)
- [x] Documentar procedimientos (este archivo)

### Fase 2: Automatización 🔄
- [ ] Implementar GitHub Actions workflow
- [ ] Configurar tarea programada en Windows (producción)
- [ ] Crear dashboard de monitoreo

### Fase 3: Seguridad y Compliance 🔄
- [ ] Implementar encriptación de backups
- [ ] Configurar permisos restrictivos
- [ ] Migrar secretos a Azure Key Vault
- [ ] Auditar acceso a backups

### Fase 4: Cloud y Redundancia 📅 (Futuro)
- [ ] Configurar Azure Blob Storage
- [ ] Implementar replicación geográfica
- [ ] Backup offsite automático
- [ ] Pruebas de disaster recovery

---

## Contactos y Escalación

### Responsables

| Rol | Nombre | Contacto |
|-----|--------|----------|
| **DBA Principal** | [Nombre] | [Email/Tel] |
| **DevOps Lead** | [Nombre] | [Email/Tel] |
| **Gerente TI** | [Nombre] | [Email/Tel] |

### Procedimiento de Escalación

1. **Nivel 1** (0-30 min): Desarrollador ejecuta restauración según procedimiento
2. **Nivel 2** (30-60 min): DBA interviene si fallan procedimientos estándar
3. **Nivel 3** (60+ min): Gerencia TI para decisiones de negocio y comunicación

---

## Anexos

### A. Comandos Útiles PostgreSQL

```sql
-- Ver tamaño de base de datos
SELECT pg_size_pretty(pg_database_size('autoguia_dev'));

-- Ver tablas más grandes
SELECT schemaname, tablename, pg_size_pretty(pg_total_relation_size(schemaname||'.'||tablename)) AS size
FROM pg_tables
ORDER BY pg_total_relation_size(schemaname||'.'||tablename) DESC
LIMIT 10;

-- Ver conexiones activas
SELECT * FROM pg_stat_activity WHERE datname = 'autoguia_dev';
```

### B. Troubleshooting Común

**Error: "pg_dump: command not found"**
```powershell
# Agregar PostgreSQL al PATH
$env:Path += ";C:\Program Files\PostgreSQL\16\bin"
```

**Error: "FATAL: password authentication failed"**
```powershell
# Configurar PGPASSWORD antes de ejecutar
$env:PGPASSWORD = "tu_password"
.\backup-autoguia.ps1
```

**Error: "disk full" durante backup**
```powershell
# Limpiar backups antiguos manualmente
Get-ChildItem .\backups\database -Recurse | Where-Object LastWriteTime -lt (Get-Date).AddDays(-30) | Remove-Item -Force
```

---

## Versionado de Documento

| Versión | Fecha | Autor | Cambios |
|---------|-------|-------|---------|
| 1.0 | 2025-10-22 | AutoGuía Team | Versión inicial |

---

**Última revisión**: 22 de octubre de 2025  
**Próxima revisión programada**: Cada 3 meses
