# AutoGuía - Scripts de desarrollo Docker para Windows
# PowerShell script para desarrollo

param(
    [Parameter(Mandatory=$false)]
    [string]$Command,
    [Parameter(Mandatory=$false)]
    [string]$Service
)

# Colores para output
$Red = [System.ConsoleColor]::Red
$Green = [System.ConsoleColor]::Green
$Yellow = [System.ConsoleColor]::Yellow
$Blue = [System.ConsoleColor]::Blue

function Write-ColorOutput($ForegroundColor) {
    $fc = $host.UI.RawUI.ForegroundColor
    $host.UI.RawUI.ForegroundColor = $ForegroundColor
    if ($args) {
        Write-Output $args
    } else {
        $input | Write-Output
    }
    $host.UI.RawUI.ForegroundColor = $fc
}

function Show-Help {
    Write-ColorOutput $Blue "🐳 AutoGuía Docker Development Scripts"
    Write-Output "========================================"
    Write-Output ""
    Write-Output "Uso: .\docker-dev.ps1 [COMANDO] [SERVICIO]"
    Write-Output ""
    Write-Output "Comandos disponibles:"
    Write-Output "  build         Construir imágenes Docker"
    Write-Output "  up            Iniciar todos los servicios"
    Write-Output "  up-dev        Iniciar solo servicios de desarrollo (DB + Redis)"
    Write-Output "  down          Detener todos los servicios"
    Write-Output "  logs          Mostrar logs de todos los servicios"
    Write-Output "  logs [servicio]  Mostrar logs de un servicio específico"
    Write-Output "  clean         Limpiar contenedores y volúmenes"
    Write-Output "  rebuild       Reconstruir todo desde cero"
    Write-Output "  db-reset      Resetear base de datos"
    Write-Output "  shell [servicio]  Abrir shell en un contenedor"
    Write-Output ""
    Write-Output "Ejemplos:"
    Write-Output "  .\docker-dev.ps1 up-dev      # Solo DB y Redis para desarrollo"
    Write-Output "  .\docker-dev.ps1 logs web    # Ver logs de la aplicación web"
    Write-Output "  .\docker-dev.ps1 shell db    # Conectarse a PostgreSQL"
}

function Check-DockerCompose {
    $global:DockerCompose = $null
    
    if (Get-Command "docker-compose" -ErrorAction SilentlyContinue) {
        $global:DockerCompose = "docker-compose"
    } elseif (Get-Command "docker" -ErrorAction SilentlyContinue) {
        try {
            docker compose version *>$null
            $global:DockerCompose = "docker compose"
        } catch {}
    }
    
    if (-not $global:DockerCompose) {
        Write-ColorOutput $Red "❌ Error: Docker Compose no está disponible"
        exit 1
    }
}

# Comando principal
switch ($Command.ToLower()) {
    "build" {
        Write-ColorOutput $Yellow "🔨 Construyendo imágenes Docker..."
        Check-DockerCompose
        Invoke-Expression "$DockerCompose build --no-cache"
        Write-ColorOutput $Green "✅ Imágenes construidas exitosamente"
    }
    
    "up" {
        Write-ColorOutput $Yellow "🚀 Iniciando todos los servicios..."
        Check-DockerCompose
        Invoke-Expression "$DockerCompose up -d"
        Write-ColorOutput $Green "✅ Servicios iniciados"
        Write-Output "📱 Aplicación web: http://localhost"
        Write-Output "🗄️  Adminer: http://localhost:8081"
    }
    
    "up-dev" {
        Write-ColorOutput $Yellow "🔧 Iniciando servicios de desarrollo..."
        Check-DockerCompose
        Invoke-Expression "$DockerCompose -f docker-compose.dev.yml up -d"
        Write-ColorOutput $Green "✅ Servicios de desarrollo iniciados"
        Write-Output "🗄️  Base de datos: localhost:5433"
        Write-Output "🔴 Redis: localhost:6380"
        Write-Output "🗄️  Adminer: http://localhost:8080"
    }
    
    "down" {
        Write-ColorOutput $Yellow "🛑 Deteniendo servicios..."
        Check-DockerCompose
        Invoke-Expression "$DockerCompose down"
        try {
            Invoke-Expression "$DockerCompose -f docker-compose.dev.yml down"
        } catch {}
        Write-ColorOutput $Green "✅ Servicios detenidos"
    }
    
    "logs" {
        Check-DockerCompose
        if ($Service) {
            Write-ColorOutput $Yellow "📋 Logs de $Service..."
            Invoke-Expression "$DockerCompose logs -f $Service"
        } else {
            Write-ColorOutput $Yellow "📋 Logs de todos los servicios..."
            Invoke-Expression "$DockerCompose logs -f"
        }
    }
    
    "clean" {
        Write-ColorOutput $Yellow "🧹 Limpiando contenedores y volúmenes..."
        Check-DockerCompose
        Invoke-Expression "$DockerCompose down -v --remove-orphans"
        try {
            Invoke-Expression "$DockerCompose -f docker-compose.dev.yml down -v --remove-orphans"
        } catch {}
        docker system prune -f
        Write-ColorOutput $Green "✅ Limpieza completada"
    }
    
    "rebuild" {
        Write-ColorOutput $Yellow "🔄 Reconstruyendo todo desde cero..."
        Check-DockerCompose
        Invoke-Expression "$DockerCompose down -v --remove-orphans"
        Invoke-Expression "$DockerCompose build --no-cache"
        Invoke-Expression "$DockerCompose up -d"
        Write-ColorOutput $Green "✅ Reconstrucción completada"
    }
    
    "db-reset" {
        Write-ColorOutput $Yellow "🗃️  Reseteando base de datos..."
        Check-DockerCompose
        try {
            Invoke-Expression "$DockerCompose stop autoguia-db"
        } catch {}
        try {
            docker volume rm blazorautoguia_postgres-data
        } catch {}
        Invoke-Expression "$DockerCompose up -d autoguia-db"
        Write-ColorOutput $Green "✅ Base de datos reseteada"
    }
    
    "shell" {
        Check-DockerCompose
        switch ($Service.ToLower()) {
            "db" {
                Write-ColorOutput $Yellow "🔌 Conectando a PostgreSQL..."
                Invoke-Expression "$DockerCompose exec autoguia-db psql -U autoguia -d autoguia"
            }
            "database" {
                Write-ColorOutput $Yellow "🔌 Conectando a PostgreSQL..."
                Invoke-Expression "$DockerCompose exec autoguia-db psql -U autoguia -d autoguia"
            }
            "web" {
                Write-ColorOutput $Yellow "🔌 Conectando a contenedor web..."
                Invoke-Expression "$DockerCompose exec autoguia-web /bin/bash"
            }
            "redis" {
                Write-ColorOutput $Yellow "🔌 Conectando a Redis..."
                Invoke-Expression "$DockerCompose exec redis redis-cli"
            }
            default {
                Write-ColorOutput $Red "❌ Servicio no reconocido. Opciones: db, web, redis"
            }
        }
    }
    
    "help" {
        Show-Help
    }
    
    default {
        if ($Command) {
            Write-ColorOutput $Red "❌ Comando no reconocido: $Command"
        }
        Show-Help
    }
}