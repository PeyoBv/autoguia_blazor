# 🚀 Guía Rápida: Scraper de MercadoLibre

## ⚡ Inicio Rápido

### 1. Ejecutar Prueba

```powershell
cd c:\Users\barri\OneDrive\Documentos\GitHub\blazorautoguia
dotnet run --project AutoGuia.Scraper/AutoGuia.Scraper.csproj -- --test-ml
```

### 2. Ver Resultados

La prueba ejecutará 4 búsquedas automáticas y mostrará:
- ✅ Ofertas encontradas
- 💰 Precios en CLP
- 📦 Stock disponible
- 🔗 URLs de productos
- 📊 Estadísticas finales

---

## 🔧 Configuración Rápida

### Cambiar cantidad máxima de resultados

**Archivo:** `AutoGuia.Scraper/appsettings.json`

```json
{
  "ScrapingSettings": {
    "MercadoLibre": {
      "MaxResults": 20  // Cambiar aquí (default: 10)
    }
  }
}
```

### Cambiar país (Site ID)

```json
{
  "ScrapingSettings": {
    "MercadoLibre": {
      "SiteId": "MLA"  // Argentina
      // MLC = Chile (default)
      // MLB = Brasil
      // MLM = México
    }
  }
}
```

---

## 💻 Uso Programático

### Usar en tu código

```csharp
// 1. Obtener el scraper
var scrapers = serviceProvider.GetRequiredService<IEnumerable<IScraper>>();
var mlScraper = scrapers.First(s => s.TiendaNombre == "MercadoLibre");

// 2. Ejecutar búsqueda
var ofertas = await mlScraper.ScrapearProductosAsync(
    numeroDeParte: "filtro aceite",
    tiendaId: 1
);

// 3. Procesar resultados
foreach (var oferta in ofertas.Where(o => !o.TieneErrores))
{
    Console.WriteLine($"{oferta.Descripcion} - ${oferta.Precio}");
}
```

### Usar con el orquestador

```csharp
var orchestrator = serviceProvider.GetRequiredService<ScraperOrchestratorService>();

// El orquestador usará automáticamente MercadoLibreScraperService
await orchestrator.EjecutarScrapingAsync(productoId: 1);
```

---

## 📊 API de MercadoLibre

### Endpoint Público

```
GET https://api.mercadolibre.com/sites/MLC/search?q={query}&limit=10
```

**No requiere autenticación** ✅

### Probar directamente con curl

```bash
curl "https://api.mercadolibre.com/sites/MLC/search?q=filtro%20aceite&limit=5"
```

---

## 🎯 Estructura de Respuesta

### Lo que recibes (OfertaDto)

```csharp
{
    Precio: 8990.00M,
    UrlProducto: "https://articulo.mercadolibre.cl/...",
    StockDisponible: true,
    CantidadDisponible: 15,
    Descripcion: "Filtro De Aceite Toyota Hilux",
    Calificacion: 5.0M,  // 1-5 según reputación del vendedor
    TiempoEntrega: "Envío gratis"
}
```

---

## ⚙️ Agregar Autenticación (Opcional)

Si en el futuro necesitas funciones avanzadas:

### 1. Obtener Access Token

Visita: https://developers.mercadolibre.com/

### 2. Agregar a appsettings.json

```json
{
  "ScrapingSettings": {
    "MercadoLibre": {
      "AccessToken": "TU_TOKEN_AQUI"
    }
  }
}
```

### 3. Descomenta el código en MercadoLibreScraperService.cs

```csharp
var accessToken = _configuration["ScrapingSettings:MercadoLibre:AccessToken"];
if (!string.IsNullOrEmpty(accessToken))
{
    httpClient.DefaultRequestHeaders.Authorization = 
        new AuthenticationHeaderValue("Bearer", accessToken);
}
```

---

## 🐛 Troubleshooting

### Error: "No se encontraron resultados"

**Posibles causas:**
- Número de parte muy específico
- País incorrecto (verifica `SiteId`)
- Producto no disponible en MercadoLibre

**Solución:** Probar con búsqueda más genérica como `"filtro aceite"`

### Error: "HTTP 429 Too Many Requests"

**Causa:** Excediste el rate limit (10 requests/segundo)

**Solución:**
```json
{
  "Stores": {
    "MercadoLibre": {
      "RequestDelayMs": 2000  // Aumentar delay a 2 segundos
    }
  }
}
```

### Error: "Timeout"

**Solución:**
```json
{
  "ScrapingSettings": {
    "MercadoLibre": {
      "TimeoutSeconds": 60  // Aumentar timeout
    }
  }
}
```

---

## 📈 Rate Limits

| Tipo | Límite | Configurado |
|------|--------|-------------|
| Sin auth | 10 req/s | ✅ 1 req/s (seguro) |
| Con auth | 50 req/s | N/A |

---

## 🔍 Logs Útiles

### Ver logs detallados

**Modificar:** `appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "AutoGuia.Scraper.Scrapers.MercadoLibreScraperService": "Debug"
    }
  }
}
```

### Logs que verás

```
🔍 Iniciando scraping en MercadoLibre para número de parte: filtro aceite
📡 URL de búsqueda: https://api.mercadolibre.com/sites/MLC/search?q=filtro+aceite&limit=10
📦 Se encontraron 10 resultados en MercadoLibre
✅ Scraping completado: 10/10 ofertas procesadas de MercadoLibre
```

---

## 📚 Archivos Importantes

| Archivo | Descripción |
|---------|-------------|
| `Scrapers/MercadoLibreScraperService.cs` | Implementación del scraper |
| `Services/MercadoLibreTestService.cs` | Servicio de prueba |
| `appsettings.json` | Configuración |
| `PASO_2_COMPLETADO.md` | Documentación completa |
| `EJEMPLOS_IMPLEMENTACION.md` | Más ejemplos de código |

---

## 🎯 Próximos Pasos

### ¿Qué hacer ahora?

1. **Ejecutar la prueba** para verificar que todo funciona
2. **Revisar los logs** para entender el flujo
3. **Modificar configuración** para experimentar
4. **Integrar con tu código** usando el orquestador

### ¿Qué sigue después?

- Implementar scraper de Autoplanet (HTML)
- Implementar scraper de MundoRepuestos (Playwright)
- Agregar tests unitarios
- Implementar retry con Polly

---

## 💡 Tips

### Optimización

- ✅ Usa `MaxResults: 5` para pruebas rápidas
- ✅ Aumenta `RequestDelayMs` si hay errores de rate limit
- ✅ Usa búsquedas genéricas para más resultados

### Testing

- ✅ Siempre prueba con `--test-ml` antes de integrar
- ✅ Verifica que los precios sean realistas
- ✅ Revisa que las URLs sean válidas

### Producción

- ⚠️ Implementa caché de resultados (5-10 minutos)
- ⚠️ Monitorea rate limits con métricas
- ⚠️ Agrega retry automático con Polly

---

## 🆘 Ayuda

### Documentación completa
- Ver `PASO_2_COMPLETADO.md` para detalles técnicos
- Ver `ARQUITECTURA_MODULAR.md` para el diseño general

### API de MercadoLibre
- https://developers.mercadolibre.com/es_ar/items-y-busquedas

### Preguntas frecuentes
1. **¿Necesito API key?** No, para búsquedas básicas.
2. **¿Funciona en otros países?** Sí, cambia `SiteId` en config.
3. **¿Cuántos resultados puedo obtener?** Hasta 50 por request.
4. **¿Puedo filtrar por categoría?** Sí, modificando la URL de búsqueda.

---

**Generado por:** GitHub Copilot  
**Fecha:** 18 de octubre de 2025  
**Versión:** 1.0
