-- 🔍 Queries de Monitoreo de DbContext Pooling en PostgreSQL
-- Ejecutar estas queries para validar el correcto funcionamiento del pool

-- =====================================================
-- 1. CONEXIONES ACTIVAS POR BASE DE DATOS
-- =====================================================
-- Muestra el total de conexiones activas y su porcentaje de uso
SELECT 
    datname AS base_datos,
    count(*) AS conexiones_activas,
    max_conn AS max_conexiones,
    ROUND(count(*) * 100.0 / max_conn, 2) AS porcentaje_uso
FROM pg_stat_activity
CROSS JOIN (
    SELECT setting::int AS max_conn 
    FROM pg_settings 
    WHERE name = 'max_connections'
) s
WHERE datname IN ('autoguia_dev', 'identity_dev', 'autoguia_app', 'autoguia_identity')
GROUP BY datname, max_conn
ORDER BY porcentaje_uso DESC;

-- 🚨 ALERTA: Si porcentaje_uso > 80%, considerar:
--    - Aumentar poolSize en Program.cs
--    - Investigar leak de conexiones
--    - Revisar servicios que no liberan DbContext

-- =====================================================
-- 2. ESTADO DE CONEXIONES (Idle, Active, etc.)
-- =====================================================
-- Identifica conexiones en diferentes estados
SELECT 
    datname AS base_datos,
    state AS estado,
    count(*) AS total_conexiones,
    ROUND(avg(EXTRACT(EPOCH FROM (now() - state_change))), 2) AS promedio_tiempo_estado_seg
FROM pg_stat_activity
WHERE datname IN ('autoguia_dev', 'identity_dev', 'autoguia_app', 'autoguia_identity')
GROUP BY datname, state
ORDER BY datname, total_conexiones DESC;

-- 🚨 ALERTA: Si avg_idle_seconds > 60 y state = 'idle':
--    - Posible problema de disposal de DbContext
--    - Verificar que servicios usen 'using' o Dispose correctamente

-- =====================================================
-- 3. CONEXIONES IDLE EN TRANSACCIÓN (Potencial problema)
-- =====================================================
-- Detecta conexiones que están idle pero dentro de una transacción
SELECT 
    datname AS base_datos,
    pid AS process_id,
    usename AS usuario,
    application_name AS aplicacion,
    state,
    EXTRACT(EPOCH FROM (now() - state_change)) AS segundos_en_estado,
    query_start,
    SUBSTRING(query, 1, 100) AS query_preview
FROM pg_stat_activity
WHERE 
    datname IN ('autoguia_dev', 'identity_dev', 'autoguia_app', 'autoguia_identity')
    AND state = 'idle in transaction'
ORDER BY segundos_en_estado DESC;

-- 🚨 ALERTA: Si hay conexiones 'idle in transaction' > 30s:
--    - Posible deadlock o transacción no completada
--    - Revisar código de servicios que usan transacciones
--    - Implementar timeout en transacciones

-- =====================================================
-- 4. DEADLOCKS DETECTADOS
-- =====================================================
-- Muestra estadísticas de deadlocks
SELECT 
    datname AS base_datos,
    deadlocks AS total_deadlocks
FROM pg_stat_database
WHERE datname IN ('autoguia_dev', 'identity_dev', 'autoguia_app', 'autoguia_identity')
ORDER BY deadlocks DESC;

-- 🚨 ALERTA: Si deadlocks > 0:
--    - Revisar orden de bloqueo de tablas
--    - Implementar retry policy con Polly
--    - Reducir scope de transacciones

-- =====================================================
-- 5. TOP 10 QUERIES MÁS LENTAS (Requiere pg_stat_statements)
-- =====================================================
-- Primero habilitar extensión si no está activa:
-- CREATE EXTENSION IF NOT EXISTS pg_stat_statements;

SELECT 
    SUBSTRING(query, 1, 100) AS query_preview,
    calls AS total_llamadas,
    ROUND(mean_exec_time::numeric, 2) AS promedio_ms,
    ROUND(max_exec_time::numeric, 2) AS max_ms,
    ROUND(total_exec_time::numeric, 2) AS total_ms
FROM pg_stat_statements
WHERE query NOT LIKE '%pg_stat_statements%'
    AND query NOT LIKE '%pg_catalog%'
ORDER BY mean_exec_time DESC
LIMIT 10;

-- 🚨 ALERTA: Si mean_exec_time > 1000ms (1s):
--    - Revisar índices en tablas
--    - Usar EXPLAIN ANALYZE para plan de ejecución
--    - Considerar caché (Redis) para queries frecuentes

-- =====================================================
-- 6. BLOQUEOS ACTIVOS (Locks)
-- =====================================================
-- Muestra bloqueos que pueden causar contention
SELECT 
    pg_stat_activity.datname AS base_datos,
    pg_locks.locktype AS tipo_bloqueo,
    pg_class.relname AS tabla,
    pg_locks.mode AS modo,
    pg_locks.granted AS otorgado,
    pg_stat_activity.usename AS usuario,
    pg_stat_activity.query_start AS inicio_query,
    SUBSTRING(pg_stat_activity.query, 1, 100) AS query_preview
FROM pg_locks
JOIN pg_stat_activity ON pg_locks.pid = pg_stat_activity.pid
LEFT JOIN pg_class ON pg_locks.relation = pg_class.oid
WHERE pg_stat_activity.datname IN ('autoguia_dev', 'identity_dev', 'autoguia_app', 'autoguia_identity')
    AND NOT pg_locks.granted
ORDER BY pg_stat_activity.query_start;

-- 🚨 ALERTA: Si hay bloqueos no otorgados (granted = false):
--    - Posible contention en tablas
--    - Revisar duración de transacciones
--    - Implementar timeouts adecuados

-- =====================================================
-- 7. CONEXIONES POR APLICACIÓN
-- =====================================================
-- Identifica de dónde vienen las conexiones
SELECT 
    datname AS base_datos,
    application_name AS aplicacion,
    count(*) AS total_conexiones,
    state,
    ROUND(avg(EXTRACT(EPOCH FROM (now() - state_change))), 2) AS promedio_edad_seg
FROM pg_stat_activity
WHERE datname IN ('autoguia_dev', 'identity_dev', 'autoguia_app', 'autoguia_identity')
GROUP BY datname, application_name, state
ORDER BY total_conexiones DESC;

-- =====================================================
-- 8. RESET DE ESTADÍSTICAS (Usar con precaución)
-- =====================================================
-- Solo en desarrollo, nunca en producción durante operación normal
-- SELECT pg_stat_statements_reset();
-- SELECT pg_stat_reset();

-- =====================================================
-- 9. CONFIGURACIÓN ACTUAL DE CONEXIONES
-- =====================================================
-- Muestra límites configurados en PostgreSQL
SELECT 
    name AS parametro,
    setting AS valor,
    unit AS unidad,
    context AS contexto,
    source AS fuente
FROM pg_settings
WHERE name IN (
    'max_connections',
    'superuser_reserved_connections',
    'idle_in_transaction_session_timeout',
    'statement_timeout',
    'lock_timeout'
)
ORDER BY name;

-- =====================================================
-- 10. MONITOREO CONTINUO (Query Long-Running)
-- =====================================================
-- Detecta queries que llevan más de 30 segundos ejecutándose
SELECT 
    pid,
    datname AS base_datos,
    usename AS usuario,
    application_name AS aplicacion,
    state,
    EXTRACT(EPOCH FROM (now() - query_start)) AS segundos_ejecutando,
    query_start,
    SUBSTRING(query, 1, 150) AS query_preview
FROM pg_stat_activity
WHERE 
    datname IN ('autoguia_dev', 'identity_dev', 'autoguia_app', 'autoguia_identity')
    AND state = 'active'
    AND EXTRACT(EPOCH FROM (now() - query_start)) > 30
ORDER BY segundos_ejecutando DESC;

-- 🚨 ALERTA: Si hay queries > 30s activas:
--    - Posible query no optimizada
--    - Considerar timeout en application
--    - Revisar plan de ejecución con EXPLAIN

-- =====================================================
-- 📊 DASHBOARD COMPLETO (Ejecutar cada 5 minutos)
-- =====================================================
SELECT 
    'CONEXIONES TOTALES' AS metrica,
    count(*)::text AS valor
FROM pg_stat_activity
WHERE datname IN ('autoguia_dev', 'identity_dev', 'autoguia_app', 'autoguia_identity')

UNION ALL

SELECT 
    'CONEXIONES ACTIVAS',
    count(*)::text
FROM pg_stat_activity
WHERE datname IN ('autoguia_dev', 'identity_dev', 'autoguia_app', 'autoguia_identity')
    AND state = 'active'

UNION ALL

SELECT 
    'CONEXIONES IDLE',
    count(*)::text
FROM pg_stat_activity
WHERE datname IN ('autoguia_dev', 'identity_dev', 'autoguia_app', 'autoguia_identity')
    AND state = 'idle'

UNION ALL

SELECT 
    'IDLE IN TRANSACTION',
    count(*)::text
FROM pg_stat_activity
WHERE datname IN ('autoguia_dev', 'identity_dev', 'autoguia_app', 'autoguia_identity')
    AND state = 'idle in transaction'

UNION ALL

SELECT 
    'DEADLOCKS TOTALES',
    sum(deadlocks)::text
FROM pg_stat_database
WHERE datname IN ('autoguia_dev', 'identity_dev', 'autoguia_app', 'autoguia_identity')

ORDER BY metrica;

-- =====================================================
-- 📝 NOTAS DE USO
-- =====================================================
-- 1. Ejecutar estas queries periódicamente en staging/producción
-- 2. Configurar alertas en monitoreo (Grafana, Azure Monitor, etc.)
-- 3. Ajustar pool sizes según métricas reales
-- 4. Documentar patrones anómalos para análisis
-- 5. Revisar después de deployment por 24-48h

-- 🎯 UMBRALES RECOMENDADOS:
-- - Porcentaje uso pool: < 80%
-- - Conexiones idle: < 10% del total
-- - Deadlocks: 0 esperado
-- - Queries > 1s: < 5% del total
-- - Idle in transaction: 0 esperado
