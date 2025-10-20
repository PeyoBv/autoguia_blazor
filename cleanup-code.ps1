# Script para limpiar comentarios innecesarios y emojis en logs

$rootPath = "c:\Users\barri\OneDrive\Documentos\GitHub\blazorautoguia"

# Patrones de reemplazo: emoji -> remover
$replacements = @(
    # Logging con emojis innecesarios
    '🔍 ', ''
    '✅ ', ''
    '❌ ', ''
    '⚠️ ', ''
    '✨ ', ''
    '💡 ', ''
    '🎯 ', ''
    '📡 ', ''
    '📥 ', ''
    '🏪 ', ''
    '⏸️ ', ''
    '🔗 ', ''
    '📊 ', ''
    '🌐 ', ''
    '❓ ', ''
    '🎉 ', ''
    '📦 ', ''
    '🔎 ', ''
    '📍 ', ''
    '⚙️ ', ''
    '🛠️ ', ''
    '🌍 ', ''
    '🐛 ', ''
    '🌍 ', ''
    '➕ ', ''
    '🔄 ', ''
    '▶️ ', ''
    '⏹️ ', ''
    '⏸️ ', ''
)

# Archivos a limpiar
$files = @(
    "$rootPath\AutoGuia.Infrastructure\Services\CategoriaService.cs"
    "$rootPath\AutoGuia.Infrastructure\Services\ProductoService.cs"
    "$rootPath\AutoGuia.Infrastructure\Services\ForoService.cs"
    "$rootPath\AutoGuia.Infrastructure\Services\TallerService.cs"
    "$rootPath\AutoGuia.Web\AutoGuia.Web\Program.cs"
    "$rootPath\AutoGuia.Web\AutoGuia.Web\Services\ScraperIntegrationService.cs"
)

foreach ($file in $files) {
    if (Test-Path $file) {
        Write-Host "Limpiando: $file"
        $content = Get-Content $file -Raw -Encoding UTF8
        
        # Aplicar reemplazos
        for ($i = 0; $i -lt $replacements.Length; $i += 2) {
            $content = $content -replace [regex]::Escape($replacements[$i]), $replacements[$i+1]
        }
        
        Set-Content $file -Value $content -Encoding UTF8
        Write-Host "✓ Completado: $file"
    }
}

Write-Host "✓ Limpieza de código completada"
