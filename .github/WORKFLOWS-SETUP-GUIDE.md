# Guía de Configuración de GitHub Actions Workflows

## 📋 Índice
- [Descripción General](#descripción-general)
- [Workflows Disponibles](#workflows-disponibles)
- [Configuración de Secretos](#configuración-de-secretos)
- [Estructura de Workflows](#estructura-de-workflows)
- [Pruebas y Validación](#pruebas-y-validación)
- [Troubleshooting](#troubleshooting)

## Descripción General

Este repositorio contiene 7 workflows de GitHub Actions para CI/CD, despliegue a Azure, y tareas de mantenimiento. Todos los workflows siguen las mejores prácticas de GitHub Actions:

✅ **Condicionales YAML Nativos**: Uso de `if: success()`, `if: failure()` en lugar de shell scripts  
✅ **Separación de Responsabilidades**: Jobs distintos para build, deploy, y notificaciones  
✅ **Sin Exit Codes Redundantes**: No se usa `exit 1` en jobs de notificación  
✅ **Documentación Inline**: Cada workflow incluye comentarios explicativos  

## Workflows Disponibles

### 1. CI/CD Pipeline (`ci.yml`)
**Propósito**: Pipeline de integración continua completo  
**Trigger**: Push y PR a `main` y `develop`  
**Jobs**:
- `build-and-test` - Compila y ejecuta tests con cobertura
- `security-scan` - Analiza paquetes vulnerables
- `code-quality` - Análisis con SonarCloud (opcional)
- `docker-build` - Construye imagen Docker
- `notify-success` - Notificación de éxito
- `notify-failure` - Notificación de fallo

**Secretos Opcionales**:
- `CODECOV_TOKEN` - Para reportes de cobertura
- `SONAR_TOKEN` - Para análisis de SonarCloud
- `DOCKER_USERNAME`, `DOCKER_PASSWORD` - Para push a Docker Hub

### 2. Production CI/CD (`production-ci-cd.yml`)
**Propósito**: Pipeline de producción con validaciones estrictas  
**Trigger**: Push y PR a `main` y `develop`  
**Jobs**: Similar a `ci.yml` con validaciones adicionales  

**Secretos Opcionales**: Mismos que `ci.yml`

### 3. Azure Deploy - Main (`main_rodavia.yml`)
**Propósito**: Despliegue principal a Azure con autenticación federada  
**Trigger**: Push a `main` o ejecución manual  
**Jobs**:
- `build` - Compila y publica la aplicación
- `deploy` - Despliega a Azure App Service
- `notify-success` - Confirma despliegue exitoso
- `notify-failure` - Alerta de fallo en despliegue

**Secretos Requeridos**:
```
AZUREAPPSERVICE_CLIENTID_F30058E83D074DDAB853693A89BC5A84
AZUREAPPSERVICE_TENANTID_8E4E13FA377D4242BB4508CC5DB3C76C
AZUREAPPSERVICE_SUBSCRIPTIONID_B400291DBEA44229B55D00A16DFC81FF
```

**Configuración en Azure**:
1. App Service debe llamarse `rodavia`
2. Configurar autenticación federada (OIDC)
3. .NET 8.x runtime configurado

### 4. Azure Deploy - Simple (`azure-webapps.yml`)
**Propósito**: Despliegue simple usando Publish Profile  
**Trigger**: Push a `main` o ejecución manual  
**Jobs**:
- `build` - Compila la aplicación
- `deploy` - Despliega usando publish profile
- `notify-success` - Notificación de éxito
- `notify-failure` - Notificación de fallo

**Secretos Requeridos**:
```
AZUREAPPSERVICE_PUBLISHPROFILE
```

**Cómo obtener Publish Profile**:
1. Azure Portal > App Service `rodavia`
2. Overview > Get publish profile
3. Copiar contenido XML completo
4. GitHub > Settings > Secrets > New secret
5. Nombre: `AZUREAPPSERVICE_PUBLISHPROFILE`

### 5. Azure Deploy - Advanced (`azure-webapps-advanced.yml`)
**Propósito**: Despliegue multi-entorno (Staging + Production)  
**Trigger**: Push a `main` (→ staging) o `production` (→ production)  
**Jobs**:
- `build` - Compila una vez para ambos entornos
- `deploy-staging` - Despliega a staging (automático)
- `deploy-production` - Despliega a production (requiere aprobación)
- `notify-success` - Confirma despliegue
- `notify-failure` - Alerta de fallo

**Secretos Requeridos**:
```
AZURE_STAGING_PUBLISHPROFILE   # Para staging
AZUREAPPSERVICE_PUBLISHPROFILE  # Para production
```

**Configuración de Environments**:
1. GitHub > Settings > Environments
2. Crear `staging` (sin restricciones)
3. Crear `production` (agregar reviewers requeridos)

**App Services Requeridos en Azure**:
- `rodavia-staging` (para staging)
- `rodavia` (para production)

### 6. Azure Deploy - Full Config (`azure-deploy.yml`)
**Propósito**: Despliegue con configuración completa y health checks  
**Trigger**: Push a `main` o `production`, o ejecución manual  
**Jobs**:
- `build-and-test` - Build con tests
- `deploy-to-azure` - Deploy con configuración de App Settings
- `health-check` - Verificación post-deployment
- `notify-success` - Notificación de éxito
- `notify-failure` - Notificación de fallo

**Secretos Requeridos**:
```yaml
# Autenticación Azure (Service Principal)
AZURE_CREDENTIALS  # JSON del Service Principal

# Configuración de aplicación (opcionales)
GOOGLE_CLIENT_ID
GOOGLE_CLIENT_SECRET
SMTP_USERNAME
SMTP_PASSWORD
SQL_CONNECTION_STRING
APPLICATIONINSIGHTS_CONNECTION_STRING
SLACK_WEBHOOK  # Para notificaciones
```

**Cómo crear AZURE_CREDENTIALS**:
```bash
az ad sp create-for-rbac \
  --name "rodavia-deploy" \
  --role contributor \
  --scopes /subscriptions/{subscription-id}/resourceGroups/{resource-group} \
  --sdk-auth
```

El comando retorna un JSON - copiar completo como secreto.

### 7. Backups Automáticos (`backups-cron.yml`)
**Propósito**: Backups semanales de bases de datos  
**Trigger**: Cron (Domingos 3:00 AM UTC) o manual  
**Jobs**:
- `backup-databases` - Ejecuta backups y los comprime
- `notify-failure` - Crea issue automático si falla
- `backup-summary` - Confirma éxito

**Secretos Opcionales**:
```
POSTGRES_PASSWORD  # Para conexión a PostgreSQL
```

**Nota**: Este workflow es un template. Requiere configurar servicio PostgreSQL o backup remoto.

## Configuración de Secretos

### Secretos por Workflow

| Workflow | Secretos Requeridos | Secretos Opcionales |
|----------|-------------------|-------------------|
| `ci.yml` | - | `CODECOV_TOKEN`, `SONAR_TOKEN`, `DOCKER_USERNAME`, `DOCKER_PASSWORD` |
| `production-ci-cd.yml` | - | Mismos que `ci.yml` |
| `main_rodavia.yml` | `AZUREAPPSERVICE_CLIENTID_*`, `TENANTID_*`, `SUBSCRIPTIONID_*` | - |
| `azure-webapps.yml` | `AZUREAPPSERVICE_PUBLISHPROFILE` | - |
| `azure-webapps-advanced.yml` | `AZURE_STAGING_PUBLISHPROFILE`, `AZUREAPPSERVICE_PUBLISHPROFILE` | - |
| `azure-deploy.yml` | `AZURE_CREDENTIALS` | `GOOGLE_CLIENT_ID`, `SMTP_*`, `SQL_CONNECTION_STRING`, etc. |
| `backups-cron.yml` | - | `POSTGRES_PASSWORD` |

### Cómo Agregar Secretos

1. GitHub > Repository > Settings
2. Secrets and variables > Actions
3. New repository secret
4. Nombre del secreto (exactamente como en tabla)
5. Valor del secreto
6. Add secret

## Estructura de Workflows

Todos los workflows de deployment siguen esta estructura estandarizada:

```yaml
jobs:
  build:
    # Compila la aplicación, crea artifacts
    steps:
      - Checkout código
      - Setup .NET
      - Restore dependencias
      - Build
      - Publish
      - Upload artifact

  deploy:
    needs: build
    # Despliega usando artifact del build
    steps:
      - Download artifact
      - Login a Azure
      - Deploy to App Service

  notify-success:
    needs: [build, deploy]
    if: success()  # ✅ Solo si TODOS los jobs pasaron
    steps:
      - Mostrar resumen de éxito

  notify-failure:
    needs: [build, deploy]
    if: failure()  # ❌ Solo si ALGÚN job falló
    steps:
      - Mostrar detalles del fallo
```

### Ventajas de Esta Estructura

1. **Separación clara**: Build y Deploy son independientes
2. **Reutilización**: Artifact se genera una vez, se usa múltiples veces
3. **Notificaciones precisas**: Success/Failure son mutuamente excluyentes
4. **Debug fácil**: Si falla, sabes exactamente qué job tiene el problema

## Pruebas y Validación

### 1. Validar Sintaxis YAML Localmente

```bash
# Opción 1: Python
python3 -c "import yaml; yaml.safe_load(open('.github/workflows/ci.yml'))"

# Opción 2: yamllint (instalar primero)
yamllint .github/workflows/

# Opción 3: GitHub CLI
gh workflow list
```

### 2. Probar Workflow Manualmente

1. GitHub > Actions
2. Seleccionar workflow (ej: "Build and Deploy to Azure")
3. Run workflow > Seleccionar branch
4. Run workflow

### 3. Verificar Build Local

```bash
# Restaurar y compilar
dotnet restore Rodavia.sln
dotnet build Rodavia.sln --configuration Release

# Ejecutar tests
dotnet test Rodavia.sln --configuration Release

# Publicar (simular deployment)
dotnet publish Rodavia.Web/Rodavia.Web/Rodavia.Web.csproj \
  --configuration Release \
  --output ./publish
```

### 4. Monitorear Ejecución

- GitHub > Actions > Workflow run específico
- Ver logs en tiempo real
- Revisar artifacts generados
- Verificar notificaciones (success/failure jobs)

## Troubleshooting

### Problema: "Secret not found"

**Síntoma**: Workflow falla con mensaje tipo `Error: Secret AZUREAPPSERVICE_PUBLISHPROFILE is not set`

**Solución**:
1. Verificar nombre exacto del secreto (case-sensitive)
2. GitHub Settings > Secrets > Confirmar secreto existe
3. Verificar que el secreto tenga valor (no esté vacío)

### Problema: "Authentication failed to Azure"

**Síntoma**: Deploy job falla al autenticar con Azure

**Soluciones según método de autenticación**:

**Para Publish Profile** (`azure-webapps.yml`):
1. Verificar que Publish Profile no haya expirado
2. Re-descargar desde Azure Portal
3. Actualizar secreto en GitHub

**Para Federated Auth** (`main_rodavia.yml`):
1. Verificar que ClientId, TenantId, SubscriptionId sean correctos
2. Confirmar que App Registration tiene permisos
3. Verificar que Federated Credential esté configurado

**Para Service Principal** (`azure-deploy.yml`):
1. Verificar que Service Principal exista: `az ad sp list --display-name rodavia-deploy`
2. Verificar permisos: Debe tener rol Contributor en el Resource Group
3. Re-generar credenciales si es necesario

### Problema: "App Service not found"

**Síntoma**: Deploy falla con mensaje `Error: Failed to find app-name 'rodavia'`

**Solución**:
1. Verificar que el App Service exista en Azure Portal
2. Confirmar que el nombre en workflow coincide EXACTAMENTE
3. Verificar que Service Principal/Publish Profile tiene acceso al App Service

### Problema: "Artifact not found"

**Síntoma**: Deploy job falla porque no encuentra el artifact

**Solución**:
1. Verificar que build job completó exitosamente
2. Confirmar que `upload-artifact` se ejecutó
3. Verificar que nombre en `upload` y `download` coinciden exactamente
4. Actualizar a `actions/upload-artifact@v4` y `actions/download-artifact@v4`

### Problema: "Notification job always fails/skips"

**Síntoma**: `notify-success` siempre aparece como skipped o `notify-failure` siempre falla

**Causa**: Mal uso de condicionales o falta `needs`

**Solución correcta**:
```yaml
notify-success:
  needs: [build, deploy]  # ✅ Debe listar TODOS los jobs que debe esperar
  if: success()           # ✅ Solo ejecuta si TODOS en needs pasaron

notify-failure:
  needs: [build, deploy]  # ✅ Debe listar TODOS los jobs que debe esperar
  if: failure()           # ✅ Solo ejecuta si ALGUNO en needs falló
```

**❌ NO hacer**:
```yaml
# MAL - No uses always() para notificaciones condicionales
if: always()  # ❌ Ejecuta siempre, incluso cuando no corresponde

# MAL - No uses exit 1 en notificaciones
run: |
  echo "Failed"
  exit 1  # ❌ Hace que el job aparezca como failed
```

### Problema: "Coverage check fails"

**Síntoma**: Job `build-and-test` falla en el paso de coverage threshold

**Solución**:
1. Revisar porcentaje actual de cobertura en logs
2. Si está por debajo del 70%, agregar más tests
3. Para development, comentar temporalmente el check:
```yaml
# - name: ✅ Check coverage threshold (70%)
#   run: ...
```

### Problema: "Docker build fails"

**Síntoma**: `docker-build` job falla

**Solución**:
1. Verificar que `Dockerfile` existe en root del repo
2. Verificar sintaxis del Dockerfile localmente: `docker build .`
3. Si push falla, verificar `DOCKER_USERNAME` y `DOCKER_PASSWORD`
4. Confirmar que solo hace push en main: `if: github.ref == 'refs/heads/main'`

## Mejores Prácticas

### 1. Nombres Descriptivos de Jobs

✅ **Bueno**:
```yaml
jobs:
  build:
    name: Build Application  # Descriptivo
```

❌ **Malo**:
```yaml
jobs:
  build:
    name: Build  # Poco descriptivo
```

### 2. Usar Condicionales YAML

✅ **Bueno**:
```yaml
notify-success:
  if: success()  # YAML nativo
```

❌ **Malo**:
```yaml
notify:
  if: always()
  steps:
    - run: |
        if [ "${{ needs.build.result }}" == "success" ]; then
          echo "Success"
        fi
```

### 3. Separar Jobs de Trabajo y Notificación

✅ **Bueno**:
```yaml
jobs:
  build: ...
  deploy: ...
  notify-success: ...
  notify-failure: ...
```

❌ **Malo**:
```yaml
jobs:
  build-and-notify: ...  # Mezcla responsabilidades
```

### 4. Documentar Secretos Requeridos

✅ **Bueno**:
```yaml
# REQUISITOS:
# - Secretos requeridos:
#   * AZUREAPPSERVICE_PUBLISHPROFILE
# - App Service debe llamarse 'rodavia'
```

### 5. Usar Environment URLs

✅ **Bueno**:
```yaml
deploy:
  environment:
    name: production
    url: https://rodavia.azurewebsites.net  # ✅ Link directo al deployment
```

## Recursos Adicionales

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Azure Web Apps Deploy Action](https://github.com/Azure/webapps-deploy)
- [Workflow Syntax Reference](https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions)
- [Status Check Functions](https://docs.github.com/en/actions/learn-github-actions/expressions#status-check-functions)

## Contacto y Soporte

Para issues o preguntas sobre los workflows:
1. Crear issue en el repositorio con label `workflow`
2. Incluir logs relevantes del workflow que falla
3. Especificar qué workflow está fallando y en qué paso

---

**Última actualización**: Noviembre 2025  
**Versión**: 2.0  
**Mantenido por**: Equipo Rodavia
