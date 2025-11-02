# Script seguro para renombrar proyecto AutoGuia a Rodavia
# Este script usa un enfoque diferente para evitar problemas de bloqueo

Write-Host "=== RENOMBRADO SEGURO DE AUTOGUIA A RODAVIA ===" -ForegroundColor Cyan
Write-Host ""

# Paso 1: Remover carpetas del índice de Git (sin eliminar archivos físicos)
Write-Host "[1/4] Removiendo carpetas del índice de Git..." -ForegroundColor Yellow
git rm -r --cached AutoGuia.Core
git rm -r --cached AutoGuia.Infrastructure  
git rm -r --cached AutoGuia.Scraper
git rm -r --cached AutoGuia.Tests
git rm -r --cached "AutoGuia.Web\AutoGuia.Web"
git rm -r --cached "AutoGuia.Web\AutoGuia.Web.Client"

Write-Host "✓ Carpetas removidas del índice" -ForegroundColor Green
Write-Host ""

# Paso 2: Renombrar físicamente las carpetas con robocopy (más confiable que rename)
Write-Host "[2/4] Renombrando carpetas físicamente..." -ForegroundColor Yellow

# Core
robocopy "AutoGuia.Core" "Rodavia.Core" /E /MOVE /NFL /NDL /NJH /NJS
# Infrastructure
robocopy "AutoGuia.Infrastructure" "Rodavia.Infrastructure" /E /MOVE /NFL /NDL /NJH /NJS
# Scraper
robocopy "AutoGuia.Scraper" "Rodavia.Scraper" /E /MOVE /NFL /NDL /NJH /NJS
# Tests
robocopy "AutoGuia.Tests" "Rodavia.Tests" /E /MOVE /NFL /NDL /NJH /NJS
# Web (primero client, luego server)
robocopy "AutoGuia.Web\AutoGuia.Web.Client" "AutoGuia.Web\Rodavia.Web.Client" /E /MOVE /NFL /NDL /NJH /NJS
robocopy "AutoGuia.Web\AutoGuia.Web" "AutoGuia.Web\Rodavia.Web" /E /MOVE /NFL /NDL /NJH /NJS
# Renombrar carpeta Web principal
robocopy "AutoGuia.Web" "Rodavia.Web" /E /MOVE /NFL /NDL /NJH /NJS

Write-Host "✓ Carpetas renombradas físicamente" -ForegroundColor Green
Write-Host ""

# Paso 3: Agregar nuevas carpetas a Git
Write-Host "[3/4] Agregando carpetas renombradas a Git..." -ForegroundColor Yellow
git add Rodavia.Core
git add Rodavia.Infrastructure
git add Rodavia.Scraper
git add Rodavia.Tests
git add Rodavia.Web

Write-Host "✓ Carpetas agregadas a Git" -ForegroundColor Green
Write-Host ""

# Paso 4: Renombrar archivos .sln y scripts
Write-Host "[4/4] Renombrando archivos adicionales..." -ForegroundColor Yellow
git mv AutoGuia.sln Rodavia.sln
git mv backup-autoguia.ps1 backup-rodavia.ps1
git mv restore-autoguia.ps1 restore-rodavia.ps1

Write-Host "✓ Archivos adicionales renombrados" -ForegroundColor Green
Write-Host ""

Write-Host "=== RENOMBRADO COMPLETADO ===" -ForegroundColor Green
Write-Host ""
Write-Host "Siguiente paso: Ejecutar el script de actualización de contenido" -ForegroundColor Cyan
Write-Host "  .\update-project-content.ps1" -ForegroundColor White
