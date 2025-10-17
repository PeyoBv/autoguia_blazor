-- AutoGuía - Script de inicialización de base de datos PostgreSQL
-- Este script se ejecuta automáticamente al crear el contenedor de base de datos

-- Crear extensiones necesarias
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Configurar zona horaria
SET timezone = 'America/Santiago';

-- Configurar locale para Chile
SET lc_time = 'es_CL.UTF-8';

-- Crear índices adicionales para optimización (se crearán automáticamente por EF)
-- pero podemos preparar la base para mejor rendimiento

-- Log de inicialización
DO $$
BEGIN
    RAISE NOTICE 'AutoGuía Database initialized successfully at %', NOW();
    RAISE NOTICE 'Database: %', current_database();
    RAISE NOTICE 'User: %', current_user;
    RAISE NOTICE 'Version: %', version();
END $$;