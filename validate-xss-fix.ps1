# Validacion XSS - AutoGuia
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "   VALIDACION DE REMEDIACION XSS - AUTOGUIA" -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host ""

# 1. Verificar @Html.Raw
Write-Host "[1/5] Buscando @Html.Raw..." -ForegroundColor Yellow
$htmlRawCount = (Get-ChildItem -Path "AutoGuia.Web" -Recurse -Include "*.razor","*.cshtml" | Select-String -Pattern "@Html\.Raw" -SimpleMatch).Count
if ($htmlRawCount -eq 0) {
    Write-Host "OK: No se encontraron @Html.Raw" -ForegroundColor Green
} else {
    Write-Host "ERROR: $htmlRawCount instancias encontradas" -ForegroundColor Red
}

# 2. Verificar HtmlSanitizer
Write-Host "[2/5] Verificando HtmlSanitizer..." -ForegroundColor Yellow
$htmlSanitizerInstalled = (Get-Content "AutoGuia.Infrastructure\AutoGuia.Infrastructure.csproj" | Select-String -Pattern "HtmlSanitizer").Count -gt 0
if ($htmlSanitizerInstalled) {
    Write-Host "OK: HtmlSanitizer instalado" -ForegroundColor Green
} else {
    Write-Host "ERROR: HtmlSanitizer NO instalado" -ForegroundColor Red
}

# 3. Verificar servicio
Write-Host "[3/5] Verificando HtmlSanitizationService..." -ForegroundColor Yellow
$serviceExists = Test-Path "AutoGuia.Infrastructure\Services\HtmlSanitizationService.cs"
if ($serviceExists) {
    Write-Host "OK: Servicio existe" -ForegroundColor Green
} else {
    Write-Host "ERROR: Servicio NO existe" -ForegroundColor Red
}

# 4. Verificar middleware
Write-Host "[4/5] Verificando SecurityHeadersMiddleware..." -ForegroundColor Yellow
$middlewareExists = Test-Path "AutoGuia.Infrastructure\Middleware\SecurityHeadersMiddleware.cs"
if ($middlewareExists) {
    Write-Host "OK: Middleware existe" -ForegroundColor Green
} else {
    Write-Host "ERROR: Middleware NO existe" -ForegroundColor Red
}

# 5. Ejecutar tests
Write-Host "[5/5] Ejecutando tests..." -ForegroundColor Yellow
dotnet test AutoGuia.Tests\AutoGuia.Tests.csproj --filter "FullyQualifiedName~HtmlSanitizationServiceTests" --nologo --verbosity quiet
if ($LASTEXITCODE -eq 0) {
    Write-Host "OK: Tests pasaron" -ForegroundColor Green
} else {
    Write-Host "ERROR: Tests fallaron" -ForegroundColor Red
}

Write-Host ""
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "RESUMEN" -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan
$allPassed = ($htmlRawCount -eq 0) -and $htmlSanitizerInstalled -and $serviceExists -and $middlewareExists -and ($LASTEXITCODE -eq 0)
if ($allPassed) {
    Write-Host "REMEDIACION COMPLETADA" -ForegroundColor Green
} else {
    Write-Host "HAY PROBLEMAS PENDIENTES" -ForegroundColor Red
}
Write-Host ""
