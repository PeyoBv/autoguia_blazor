#!/bin/bash
# AutoGuía - Scripts de desarrollo Docker

set -e

# Colores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Rodavia - Scripts de desarrollo Docker
      
echo -e "${BLUE}🐳 Rodavia Docker Development Scripts${NC}"
echo "========================================"

# Función para mostrar ayuda
show_help() {
    echo "Uso: ./docker-dev.sh [COMANDO]"
    echo ""
    echo "Comandos disponibles:"
    echo "  build         Construir imágenes Docker"
    echo "  up            Iniciar todos los servicios"
    echo "  up-dev        Iniciar solo servicios de desarrollo (DB + Redis)"
    echo "  down          Detener todos los servicios"
    echo "  logs          Mostrar logs de todos los servicios"
    echo "  logs [servicio]  Mostrar logs de un servicio específico"
    echo "  clean         Limpiar contenedores y volúmenes"
    echo "  rebuild       Reconstruir todo desde cero"
    echo "  db-reset      Resetear base de datos"
    echo "  shell [servicio]  Abrir shell en un contenedor"
    echo ""
    echo "Ejemplos:"
    echo "  ./docker-dev.sh up-dev      # Solo DB y Redis para desarrollo"
    echo "  ./docker-dev.sh logs web    # Ver logs de la aplicación web"
    echo "  ./docker-dev.sh shell db    # Conectarse a PostgreSQL"
}

# Verificar si docker-compose está disponible
check_docker_compose() {
    if command -v docker-compose >/dev/null 2>&1; then
        DOCKER_COMPOSE="docker-compose"
    elif command -v docker >/dev/null 2>&1 && docker compose version >/dev/null 2>&1; then
        DOCKER_COMPOSE="docker compose"
    else
        echo -e "${RED}❌ Error: Docker Compose no está disponible${NC}"
        exit 1
    fi
}

# Comandos principales
case "${1}" in
    build)
        echo -e "${YELLOW}🔨 Construyendo imágenes Docker...${NC}"
        check_docker_compose
        $DOCKER_COMPOSE build --no-cache
        echo -e "${GREEN}✅ Imágenes construidas exitosamente${NC}"
        ;;
    
    up)
        echo -e "${YELLOW}🚀 Iniciando todos los servicios...${NC}"
        check_docker_compose
        $DOCKER_COMPOSE up -d
        echo -e "${GREEN}✅ Servicios iniciados${NC}"
        echo "📱 Aplicación web: http://localhost"
        echo "🗄️  Adminer: http://localhost:8081"
        ;;
    
    up-dev)
        echo -e "${YELLOW}🔧 Iniciando servicios de desarrollo...${NC}"
        check_docker_compose
        $DOCKER_COMPOSE -f docker-compose.dev.yml up -d
        echo -e "${GREEN}✅ Servicios de desarrollo iniciados${NC}"
        echo "🗄️  Base de datos: localhost:5433"
        echo "🔴 Redis: localhost:6380"
        echo "🗄️  Adminer: http://localhost:8080"
        ;;
    
    down)
        echo -e "${YELLOW}🛑 Deteniendo servicios...${NC}"
        check_docker_compose
        $DOCKER_COMPOSE down
        $DOCKER_COMPOSE -f docker-compose.dev.yml down 2>/dev/null || true
        echo -e "${GREEN}✅ Servicios detenidos${NC}"
        ;;
    
    logs)
        check_docker_compose
        if [ -n "$2" ]; then
            echo -e "${YELLOW}📋 Logs de $2...${NC}"
            $DOCKER_COMPOSE logs -f "$2"
        else
            echo -e "${YELLOW}📋 Logs de todos los servicios...${NC}"
            $DOCKER_COMPOSE logs -f
        fi
        ;;
    
    clean)
        echo -e "${YELLOW}🧹 Limpiando contenedores y volúmenes...${NC}"
        check_docker_compose
        $DOCKER_COMPOSE down -v --remove-orphans
        $DOCKER_COMPOSE -f docker-compose.dev.yml down -v --remove-orphans 2>/dev/null || true
        docker system prune -f
        echo -e "${GREEN}✅ Limpieza completada${NC}"
        ;;
    
    rebuild)
        echo -e "${YELLOW}🔄 Reconstruyendo todo desde cero...${NC}"
        check_docker_compose
        $DOCKER_COMPOSE down -v --remove-orphans
        $DOCKER_COMPOSE build --no-cache
        $DOCKER_COMPOSE up -d
        echo -e "${GREEN}✅ Reconstrucción completada${NC}"
        ;;
    
    db-reset)
        echo -e "${YELLOW}🗃️  Reseteando base de datos...${NC}"
        check_docker_compose
        $DOCKER_COMPOSE stop rodavia-db 2>/dev/null || true
        docker volume rm blazorautoguia_postgres-data 2>/dev/null || true
        $DOCKER_COMPOSE up -d rodavia-db
        echo -e "${GREEN}✅ Base de datos reseteada${NC}"
        ;;
    
    shell)
        check_docker_compose
        case "$2" in
            db|database)
                echo -e "${YELLOW}🔌 Conectando a PostgreSQL...${NC}"
                                $DOCKER_COMPOSE exec rodavia-db psql -U rodavia_dev -d rodavia_dev
                
            fi
            ;;
        shell)
            if [[ "$confirm_shell" == "yes" ]]; then
                echo -e "${YELLOW}Abriendo bash en contenedor rodavia-web...${NC}"
            fi
            $DOCKER_COMPOSE exec rodavia-web /bin/bash
                ;;
            web)
                echo -e "${YELLOW}🔌 Conectando a contenedor web...${NC}"
                $DOCKER_COMPOSE exec autoguia-web /bin/bash
                ;;
            redis)
                echo -e "${YELLOW}🔌 Conectando a Redis...${NC}"
                $DOCKER_COMPOSE exec redis redis-cli
                ;;
            *)
                echo -e "${RED}❌ Servicio no reconocido. Opciones: db, web, redis${NC}"
                ;;
        esac
        ;;
    
    help|--help|-h)
        show_help
        ;;
    
    *)
        echo -e "${RED}❌ Comando no reconocido: $1${NC}"
        show_help
        exit 1
        ;;
esac