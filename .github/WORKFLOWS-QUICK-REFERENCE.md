# GitHub Actions - Quick Reference

## 🚀 Ejecución Manual de Workflows

```bash
# Via GitHub CLI
gh workflow run "Build and Deploy to Azure App Service"
gh workflow run ci.yml

# Via UI
GitHub > Actions > Select workflow > Run workflow
```

## 🔑 Secretos Esenciales por Workflow

### Deployment a Azure (Opción 1 - Publish Profile)
```
Workflow: azure-webapps.yml
Secret: AZUREAPPSERVICE_PUBLISHPROFILE

Obtener desde: Azure Portal > App Service > Get publish profile
```

### Deployment a Azure (Opción 2 - Federated Auth)
```
Workflow: main_rodavia.yml
Secrets:
  - AZUREAPPSERVICE_CLIENTID_F30058E83D074DDAB853693A89BC5A84
  - AZUREAPPSERVICE_TENANTID_8E4E13FA377D4242BB4508CC5DB3C76C
  - AZUREAPPSERVICE_SUBSCRIPTIONID_B400291DBEA44229B55D00A16DFC81FF

Configurar en: Azure Portal > App Registration > Federated credentials
```

### Deployment Multi-Entorno
```
Workflow: azure-webapps-advanced.yml
Secrets:
  - AZURE_STAGING_PUBLISHPROFILE (para staging)
  - AZUREAPPSERVICE_PUBLISHPROFILE (para production)

Branch main → Staging automático
Branch production → Production con aprobación
```

## 📊 Estado de Jobs

### Condicionales YAML
```yaml
if: success()    # Ejecuta si todos los jobs previos pasaron
if: failure()    # Ejecuta si algún job previo falló
if: always()     # Ejecuta siempre (usar con cuidado)
if: cancelled()  # Ejecuta si el workflow fue cancelado
```

### Acceder a Resultados
```yaml
needs: [build, deploy]
if: needs.build.result == 'success'

# Valores posibles:
# - success
# - failure
# - cancelled
# - skipped
```

## 🔧 Comandos Útiles

### Validar YAML Localmente
```bash
# Python (viene instalado por defecto)
python3 -c "import yaml; yaml.safe_load(open('.github/workflows/ci.yml'))"

# Ver workflows disponibles
gh workflow list

# Ver ejecuciones recientes
gh run list --limit 5
```

### Build Local (Simular CI)
```bash
# Restaurar dependencias
dotnet restore Rodavia.sln

# Compilar
dotnet build Rodavia.sln --configuration Release

# Ejecutar tests
dotnet test Rodavia.sln --configuration Release

# Publicar (simular deployment)
dotnet publish Rodavia.Web/Rodavia.Web/Rodavia.Web.csproj \
  --configuration Release \
  --output ./publish
```

## 🏗️ Estructura Estándar de Workflow

```yaml
jobs:
  build:
    name: Build Application
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
      - run: dotnet restore
      - run: dotnet build --no-restore
      - run: dotnet publish
      - uses: actions/upload-artifact@v4

  deploy:
    name: Deploy to Azure
    needs: build
    runs-on: ubuntu-latest
    environment:
      name: production
      url: https://rodavia.azurewebsites.net
    steps:
      - uses: actions/download-artifact@v4
      - uses: azure/webapps-deploy@v3

  notify-success:
    needs: [build, deploy]
    if: success()
    runs-on: ubuntu-latest
    steps:
      - run: echo "✅ Success!"

  notify-failure:
    needs: [build, deploy]
    if: failure()
    runs-on: ubuntu-latest
    steps:
      - run: echo "❌ Failed!"
```

## 🐛 Troubleshooting Rápido

| Problema | Causa Común | Solución |
|----------|-------------|----------|
| "Secret not found" | Secreto no configurado | Settings > Secrets > Add |
| "Authentication failed" | Credenciales inválidas | Re-generar y actualizar secreto |
| "App not found" | Nombre incorrecto | Verificar nombre en Azure Portal |
| "Artifact not found" | Build falló antes | Revisar logs del job build |
| "Notify always skips" | Falta `needs` | Agregar `needs: [build, deploy]` |

## 📝 Checklist Pre-Deployment

- [ ] Secretos configurados en GitHub Settings
- [ ] App Service existe en Azure con nombre correcto
- [ ] .NET 8.x runtime configurado en Azure
- [ ] Workflow YAML validado localmente
- [ ] Build local exitoso
- [ ] Rama correcta seleccionada (main/production)

## 🎯 Workflows Disponibles

| Workflow | Propósito | Trigger | Jobs |
|----------|-----------|---------|------|
| `ci.yml` | CI completo | Push/PR | 4 jobs + notificaciones |
| `production-ci-cd.yml` | CI producción | Push/PR | 4 jobs + notificaciones |
| `main_rodavia.yml` | Deploy principal | Push main | 2 jobs + notificaciones |
| `azure-webapps.yml` | Deploy simple | Push main | 2 jobs + notificaciones |
| `azure-webapps-advanced.yml` | Multi-entorno | Push main/prod | 3 jobs + notificaciones |
| `azure-deploy.yml` | Deploy completo | Push main/prod | 3 jobs + notificaciones |
| `backups-cron.yml` | Backups auto | Cron semanal | 2 jobs + notificación |

## 🔗 Links Útiles

- [Documentación completa](.github/WORKFLOWS-SETUP-GUIDE.md)
- [GitHub Actions Docs](https://docs.github.com/en/actions)
- [Azure Deploy Action](https://github.com/Azure/webapps-deploy)

---

**Tip**: Para ayuda detallada, consultar `.github/WORKFLOWS-SETUP-GUIDE.md`
