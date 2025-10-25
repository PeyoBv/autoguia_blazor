# 🔐 Configuración de GitHub Secrets para CI/CD

## Requisitos para GitHub Actions

Para que el workflow `.github/workflows/azure-deploy.yml` funcione correctamente, debes configurar los siguientes **secrets** en tu repositorio de GitHub.

---

## 📋 Cómo Agregar Secrets en GitHub

1. Ve a tu repositorio en GitHub
2. Navega a **Settings** → **Secrets and variables** → **Actions**
3. Haz clic en **New repository secret**
4. Agrega cada secret con su nombre y valor correspondiente

---

## 🔑 Secrets Requeridos

### 1. AZURE_CREDENTIALS
**Descripción:** Credenciales del Service Principal de Azure para autenticación

**Cómo obtenerlo:**
```bash
# Crear Service Principal
az ad sp create-for-rbac --name "autoguia-github-actions" \
  --role contributor \
  --scopes /subscriptions/{subscription-id}/resourceGroups/rg-autoguia-prod \
  --sdk-auth

# Output (copiar todo el JSON):
{
  "clientId": "xxxxx-xxxx-xxxx-xxxx-xxxxxxxxx",
  "clientSecret": "xxxxx~xxxx-xxxx-xxxx",
  "subscriptionId": "xxxxx-xxxx-xxxx-xxxx-xxxxxxxxx",
  "tenantId": "xxxxx-xxxx-xxxx-xxxx-xxxxxxxxx",
  "activeDirectoryEndpointUrl": "https://login.microsoftonline.com",
  "resourceManagerEndpointUrl": "https://management.azure.com/",
  "activeDirectoryGraphResourceId": "https://graph.windows.net/",
  "sqlManagementEndpointUrl": "https://management.core.windows.net:8443/",
  "galleryEndpointUrl": "https://gallery.azure.com/",
  "managementEndpointUrl": "https://management.core.windows.net/"
}
```

**Valor en GitHub Secret:** Pegar el JSON completo

---

### 2. GOOGLE_CLIENT_ID
**Descripción:** Client ID de Google OAuth 2.0

**Cómo obtenerlo:**
1. Ve a [Google Cloud Console](https://console.cloud.google.com/)
2. APIs & Services → Credentials
3. Copia el **Client ID** de tu OAuth 2.0 Client

**Ejemplo:** `123456789-abcdefghijklmnop.apps.googleusercontent.com`

---

### 3. GOOGLE_CLIENT_SECRET
**Descripción:** Client Secret de Google OAuth 2.0

**Cómo obtenerlo:**
1. Mismo lugar que Client ID
2. Copia el **Client Secret**

**Ejemplo:** `GOCSPX-xxxxxxxxxxxxxxxxxxxxxxxx`

---

### 4. SMTP_USERNAME
**Descripción:** Email de Gmail para envío de correos

**Valor:** Tu email de Gmail completo

**Ejemplo:** `autoguia@gmail.com`

---

### 5. SMTP_PASSWORD
**Descripción:** App Password de Gmail

**Cómo obtenerlo:**
1. Ve a [Cuenta de Google](https://myaccount.google.com/)
2. Seguridad → Verificación en 2 pasos (activar)
3. Contraseñas de aplicaciones → Crear nueva
4. Seleccionar: **Correo** y **Otro (nombre personalizado)**
5. Copiar contraseña de 16 caracteres

**Ejemplo:** `abcd efgh ijkl mnop`

---

### 6. SQL_CONNECTION_STRING
**Descripción:** Connection string de Azure SQL Database

**Formato:**
```
Server=tcp:autoguia-sql-server.database.windows.net,1433;Initial Catalog=autoguia-db;Persist Security Info=False;User ID=sqladmin;Password=TuPassword123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

**Cómo obtenerlo:**
```bash
# Después de crear SQL Database en Azure
az sql db show-connection-string \
  --client ado.net \
  --name autoguia-db \
  --server autoguia-sql-server
```

**⚠️ IMPORTANTE:** Reemplazar `<username>` y `<password>` con tus credenciales reales

---

### 7. APPLICATIONINSIGHTS_CONNECTION_STRING
**Descripción:** Connection string de Application Insights

**Cómo obtenerlo:**
```bash
# Después de crear Application Insights
az monitor app-insights component show \
  --app ai-autoguia \
  --resource-group rg-autoguia-prod \
  --query connectionString -o tsv
```

**Ejemplo:** `InstrumentationKey=xxxxx-xxxx-xxxx-xxxx-xxxxxxxxx;IngestionEndpoint=https://...`

---

### 8. SLACK_WEBHOOK (Opcional)
**Descripción:** Webhook URL de Slack para notificaciones de deployment

**Cómo obtenerlo:**
1. Ve a tu workspace de Slack
2. Apps → Incoming Webhooks
3. Crear webhook para tu canal
4. Copiar URL

**Ejemplo:** `https://hooks.slack.com/services/YOUR_WORKSPACE/YOUR_CHANNEL/YOUR_WEBHOOK_TOKEN`

**Nota:** Este secret es opcional. Si no lo configuras, el workflow continuará sin enviar notificaciones.

---

## 📝 Resumen de Secrets

| Secret Name | Requerido | Descripción |
|-------------|-----------|-------------|
| `AZURE_CREDENTIALS` | ✅ Sí | Service Principal JSON de Azure |
| `GOOGLE_CLIENT_ID` | ✅ Sí | Google OAuth Client ID |
| `GOOGLE_CLIENT_SECRET` | ✅ Sí | Google OAuth Client Secret |
| `SMTP_USERNAME` | ✅ Sí | Email de Gmail |
| `SMTP_PASSWORD` | ✅ Sí | App Password de Gmail |
| `SQL_CONNECTION_STRING` | ✅ Sí | Connection string de Azure SQL |
| `APPLICATIONINSIGHTS_CONNECTION_STRING` | ✅ Sí | Connection string de App Insights |
| `SLACK_WEBHOOK` | ⚠️ Opcional | Webhook URL de Slack |

---

## ✅ Verificación de Secrets

Después de agregar todos los secrets, verifica que estén configurados:

1. Ve a **Settings** → **Secrets and variables** → **Actions**
2. Deberías ver todos los secrets listados (sin poder ver sus valores)
3. Si falta alguno, agrégalo antes de ejecutar el workflow

---

## 🚀 Ejecutar Workflow

Una vez configurados todos los secrets:

### Opción 1: Push Automático
```bash
git add .
git commit -m "feat: Configurar deployment Azure"
git push origin main
```

El workflow se ejecutará automáticamente.

### Opción 2: Ejecución Manual
1. Ve a tu repositorio en GitHub
2. Actions → Azure Deploy
3. Run workflow → Seleccionar rama → Run workflow

---

## 🔒 Seguridad

### ✅ Buenas Prácticas
- ✅ Nunca commitear secrets en el código
- ✅ Usar GitHub Secrets para valores sensibles
- ✅ Rotar passwords regularmente
- ✅ Usar App Passwords en lugar de passwords principales
- ✅ Limitar permisos del Service Principal a solo lo necesario

### ⚠️ IMPORTANTE
- Los secrets de GitHub están encriptados
- Solo son accesibles durante la ejecución del workflow
- No se muestran en logs ni outputs
- Solo usuarios con permisos de administrador pueden verlos

---

## 📖 Referencias

- [GitHub Encrypted Secrets](https://docs.github.com/en/actions/security-guides/encrypted-secrets)
- [Azure Service Principal](https://learn.microsoft.com/en-us/cli/azure/create-an-azure-service-principal-azure-cli)
- [Google OAuth 2.0](https://developers.google.com/identity/protocols/oauth2)
- [Gmail App Passwords](https://support.google.com/accounts/answer/185833)

---

**Última actualización:** Octubre 2024  
**Autor:** Equipo AutoGuía
