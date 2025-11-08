# Script de Configuración de Azure App Service para Rodavia
# Este script ayuda a configurar el App Service en Azure con las variables necesarias

param(
    [Parameter(Mandatory=$true)]
    [string]$AppServiceName = "rodavia",
    
    [Parameter(Mandatory=$true)]
    [string]$ResourceGroupName,
    
    [Parameter(Mandatory=$false)]
    [string]$Location = "eastus",
    
    [Parameter(Mandatory=$false)]
    [string]$PricingTier = "B1"
)

Write-Host "🚀 Configurando Azure App Service para Rodavia..." -ForegroundColor Cyan
Write-Host ""

# Verificar si Azure CLI está instalado
$azureCliInstalled = Get-Command az -ErrorAction SilentlyContinue
if (-not $azureCliInstalled) {
    Write-Host "❌ Azure CLI no está instalado." -ForegroundColor Red
    Write-Host "   Descárgalo desde: https://aka.ms/installazurecliwindows" -ForegroundColor Yellow
    exit 1
}

Write-Host "✅ Azure CLI detectado" -ForegroundColor Green

# Login a Azure
Write-Host ""
Write-Host "🔐 Iniciando sesión en Azure..." -ForegroundColor Cyan
az login

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Error al iniciar sesión en Azure" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Sesión iniciada correctamente" -ForegroundColor Green

# Crear Resource Group si no existe
Write-Host ""
Write-Host "📦 Verificando Resource Group: $ResourceGroupName" -ForegroundColor Cyan
$rgExists = az group exists --name $ResourceGroupName

if ($rgExists -eq "false") {
    Write-Host "   Creando Resource Group..." -ForegroundColor Yellow
    az group create --name $ResourceGroupName --location $Location
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "   ✅ Resource Group creado: $ResourceGroupName" -ForegroundColor Green
    } else {
        Write-Host "   ❌ Error al crear Resource Group" -ForegroundColor Red
        exit 1
    }
} else {
    Write-Host "   ✅ Resource Group ya existe" -ForegroundColor Green
}

# Crear App Service Plan
Write-Host ""
Write-Host "🏗️ Creando App Service Plan..." -ForegroundColor Cyan
$planName = "$AppServiceName-plan"

az appservice plan create `
    --name $planName `
    --resource-group $ResourceGroupName `
    --location $Location `
    --sku $PricingTier `
    --is-linux

if ($LASTEXITCODE -eq 0) {
    Write-Host "   ✅ App Service Plan creado: $planName" -ForegroundColor Green
} else {
    Write-Host "   ⚠️ El plan ya existe o hubo un error" -ForegroundColor Yellow
}

# Crear Web App
Write-Host ""
Write-Host "🌐 Creando Web App..." -ForegroundColor Cyan

az webapp create `
    --name $AppServiceName `
    --resource-group $ResourceGroupName `
    --plan $planName `
    --runtime "DOTNET|8.0"

if ($LASTEXITCODE -eq 0) {
    Write-Host "   ✅ Web App creada: $AppServiceName" -ForegroundColor Green
} else {
    Write-Host "   ⚠️ La Web App ya existe o hubo un error" -ForegroundColor Yellow
}

# Configurar Web App
Write-Host ""
Write-Host "⚙️ Configurando Web App..." -ForegroundColor Cyan

# Habilitar HTTPS only
az webapp update `
    --name $AppServiceName `
    --resource-group $ResourceGroupName `
    --https-only true

Write-Host "   ✅ HTTPS only habilitado" -ForegroundColor Green

# Configurar Application Settings
Write-Host ""
Write-Host "🔧 Configurando Application Settings..." -ForegroundColor Cyan

# Leer User Secrets locales (si existen)
$userSecretsPath = "$env:APPDATA\Microsoft\UserSecrets"
$googleMapsKey = "YOUR_GOOGLE_MAPS_API_KEY"

# Buscar la API key de Google Maps en User Secrets
if (Test-Path $userSecretsPath) {
    $secretsFile = Get-ChildItem -Path $userSecretsPath -Recurse -Filter "secrets.json" | Select-Object -First 1
    if ($secretsFile) {
        $secrets = Get-Content $secretsFile.FullName | ConvertFrom-Json
        if ($secrets.'GoogleMaps:ApiKey') {
            $googleMapsKey = $secrets.'GoogleMaps:ApiKey'
            Write-Host "   📍 Google Maps API Key encontrada en User Secrets" -ForegroundColor Green
        }
    }
}

# Configurar variables de entorno
az webapp config appsettings set `
    --name $AppServiceName `
    --resource-group $ResourceGroupName `
    --settings `
        "ASPNETCORE_ENVIRONMENT=Production" `
        "GoogleMaps__ApiKey=$googleMapsKey" `
        "Authentication__Google__ClientId=YOUR_GOOGLE_CLIENT_ID" `
        "Authentication__Google__ClientSecret=YOUR_GOOGLE_CLIENT_SECRET"

Write-Host "   ✅ Application Settings configuradas" -ForegroundColor Green
Write-Host "   ⚠️ Recuerda actualizar las credenciales de Google OAuth en Azure Portal" -ForegroundColor Yellow

# Descargar Publish Profile
Write-Host ""
Write-Host "📥 Descargando Publish Profile..." -ForegroundColor Cyan

$publishProfilePath = ".\$AppServiceName.PublishSettings"

az webapp deployment list-publishing-profiles `
    --name $AppServiceName `
    --resource-group $ResourceGroupName `
    --xml > $publishProfilePath

if (Test-Path $publishProfilePath) {
    Write-Host "   ✅ Publish Profile guardado en: $publishProfilePath" -ForegroundColor Green
    Write-Host ""
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
    Write-Host "📋 SIGUIENTE PASO: Configurar GitHub Secret" -ForegroundColor Yellow
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "1. Ve a tu repositorio en GitHub:" -ForegroundColor White
    Write-Host "   https://github.com/PeyoBv/rodavia_blazor/settings/secrets/actions" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "2. Haz clic en 'New repository secret'" -ForegroundColor White
    Write-Host ""
    Write-Host "3. Configura el secreto:" -ForegroundColor White
    Write-Host "   Name: AZUREAPPSERVICE_PUBLISHPROFILE" -ForegroundColor Cyan
    Write-Host "   Secret: [Contenido del archivo $publishProfilePath]" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "4. El contenido del archivo se copiará al portapapeles..." -ForegroundColor White
    
    # Copiar al portapapeles (solo en Windows)
    Get-Content $publishProfilePath | Set-Clipboard
    Write-Host "   ✅ Contenido copiado al portapapeles" -ForegroundColor Green
    Write-Host ""
} else {
    Write-Host "   ❌ Error al descargar Publish Profile" -ForegroundColor Red
}

# Resumen
Write-Host ""
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Green
Write-Host "✅ CONFIGURACIÓN COMPLETADA" -ForegroundColor Green
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Green
Write-Host ""
Write-Host "📊 Resumen de recursos creados:" -ForegroundColor Cyan
Write-Host "   • Resource Group: $ResourceGroupName" -ForegroundColor White
Write-Host "   • App Service Plan: $planName ($PricingTier)" -ForegroundColor White
Write-Host "   • Web App: $AppServiceName" -ForegroundColor White
Write-Host "   • Runtime: .NET 8.0" -ForegroundColor White
Write-Host "   • HTTPS Only: Habilitado" -ForegroundColor White
Write-Host ""
Write-Host "🌐 URL de la aplicación:" -ForegroundColor Cyan
Write-Host "   https://$AppServiceName.azurewebsites.net" -ForegroundColor Green
Write-Host ""
Write-Host "🔗 Portal de Azure:" -ForegroundColor Cyan
Write-Host "   https://portal.azure.com/#@/resource/subscriptions/[id]/resourceGroups/$ResourceGroupName/providers/Microsoft.Web/sites/$AppServiceName" -ForegroundColor Blue
Write-Host ""
Write-Host "📚 Próximos pasos:" -ForegroundColor Yellow
Write-Host "   1. Configura el secreto AZUREAPPSERVICE_PUBLISHPROFILE en GitHub" -ForegroundColor White
Write-Host "   2. Actualiza las credenciales de Google OAuth en Azure Portal" -ForegroundColor White
Write-Host "   3. Haz push a la rama main para activar el deployment" -ForegroundColor White
Write-Host "   4. Monitorea el despliegue en GitHub Actions" -ForegroundColor White
Write-Host ""
