# UI Redesign: Huawei-inspired homepage & navigation

Protótipo inicial del rediseño de UI. Contiene:
- Components/Header.razor (mega-menu, búsqueda, botones)
- Shared/MainLayout.razor
- Components/Hero.razor (placeholder)
- Shared/Footer.razor (credit: PeyoBv)
- Pages/About.razor
- wwwroot/js/site.js
- wwwroot/images/.gitkeep

Pendientes (no incluidos en este PR):
- wwwroot/css/site.css (estilos base)
- Components/ProductCard.razor (opcional, listo para añadir)
- README.md final
- Reemplazar placeholders de imágenes (hero/product/author/logo)

## Checklist

- [ ] Revisar responsive (móvil/tablet/desktop)
- [ ] Verificar accesibilidad (roles ARIA, focus, contrast)
- [ ] Reemplazar assets placeholder por imágenes con licencia
- [ ] Ejecutar dotnet build && dotnet run localmente
- [ ] Añadir bUnit tests y tests e2e (pendiente)

## Instrucciones para reviewers

1. `git checkout main && git pull origin main`
2. `git checkout -b feature/ui-redesign origin/feature/ui-redesign`
3. `dotnet restore && dotnet build`
4. `dotnet run --project AutoGuia.Web/AutoGuia.Web/AutoGuia.Web.csproj`

## Nota

No se han añadido imágenes con derechos de autor. El footer incluye "Creado por: PeyoBv"; puedo cambiarlo si el autor lo solicita.

Marca este PR como "ready for review". Adjunta capturas manualmente si quieres (no se incluyeron screenshots automáticos).

---

## Tests Status

- ✅ **ProductCardTests**: 14/14 tests passing (100%)
- ✅ **NavMenuTests**: 3/3 tests passing (100%)
- ✅ **Total UI Components Tests**: 17/17 passing
- ⚠️ **Total Project Tests**: 145/146 passing (99.3% - 1 external API test failing)
