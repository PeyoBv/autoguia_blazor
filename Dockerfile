# Rodavia - Dockerfile para aplicación web
# Multi-stage build optimizado para producción

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Configurar usuario no-root para seguridad
RUN addgroup --system --gid 1001 rodavia && \
    adduser --system --uid 1001 --ingroup rodavia rodavia
USER rodavia

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar archivos de proyecto y restaurar dependencias
COPY ["Rodavia.Web/Rodavia.Web/Rodavia.Web.csproj", "Rodavia.Web/Rodavia.Web/"]
COPY ["Rodavia.Web/Rodavia.Web.Client/Rodavia.Web.Client.csproj", "Rodavia.Web/Rodavia.Web.Client/"]
COPY ["Rodavia.Infrastructure/Rodavia.Infrastructure.csproj", "Rodavia.Infrastructure/"]
COPY ["Rodavia.Core/Rodavia.Core.csproj", "Rodavia.Core/"]

RUN dotnet restore "Rodavia.Web/Rodavia.Web/Rodavia.Web.csproj"

# Copiar código fuente
COPY . .
WORKDIR "/src/Rodavia.Web/Rodavia.Web"

# Build de la aplicación
RUN dotnet build "Rodavia.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Rodavia.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

# Copiar archivos publicados
COPY --from=publish /app/publish .

# Health check para monitoreo
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl --fail http://localhost:8080/health || exit 1

# Labels para metadatos
LABEL maintainer="Rodavia Team" \
      version="1.0" \
      description="Rodavia - Plataforma Automotriz Integral"

ENTRYPOINT ["dotnet", "Rodavia.Web.dll"]