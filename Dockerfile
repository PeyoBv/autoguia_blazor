# AutoGuía - Dockerfile para aplicación web
# Multi-stage build optimizado para producción

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Configurar usuario no-root para seguridad
RUN addgroup --system --gid 1001 autoguia && \
    adduser --system --uid 1001 --ingroup autoguia autoguia
USER autoguia

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar archivos de proyecto y restaurar dependencias
COPY ["AutoGuia.Web/AutoGuia.Web/AutoGuia.Web.csproj", "AutoGuia.Web/AutoGuia.Web/"]
COPY ["AutoGuia.Web/AutoGuia.Web.Client/AutoGuia.Web.Client.csproj", "AutoGuia.Web/AutoGuia.Web.Client/"]
COPY ["AutoGuia.Infrastructure/AutoGuia.Infrastructure.csproj", "AutoGuia.Infrastructure/"]
COPY ["AutoGuia.Core/AutoGuia.Core.csproj", "AutoGuia.Core/"]

RUN dotnet restore "AutoGuia.Web/AutoGuia.Web/AutoGuia.Web.csproj"

# Copiar código fuente
COPY . .
WORKDIR "/src/AutoGuia.Web/AutoGuia.Web"

# Build de la aplicación
RUN dotnet build "AutoGuia.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AutoGuia.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

# Copiar archivos publicados
COPY --from=publish /app/publish .

# Health check para monitoreo
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl --fail http://localhost:8080/health || exit 1

# Labels para metadatos
LABEL maintainer="AutoGuia Team" \
      version="1.0.0" \
      description="AutoGuía - Plataforma Automotriz Integral"

ENTRYPOINT ["dotnet", "AutoGuia.Web.dll"]