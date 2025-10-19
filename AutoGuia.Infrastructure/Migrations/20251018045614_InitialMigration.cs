using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AutoGuia.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Marcas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LogoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Descripcion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EsActivo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marcas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NumeroDeParte = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ImagenUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EsActivo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Talleres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Direccion = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Ciudad = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Region = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CodigoPostal = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    SitioWeb = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Latitud = table.Column<double>(type: "double precision", nullable: true),
                    Longitud = table.Column<double>(type: "double precision", nullable: true),
                    HorarioAtencion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CalificacionPromedio = table.Column<decimal>(type: "numeric", nullable: true),
                    TotalResenas = table.Column<int>(type: "integer", nullable: false),
                    Especialidades = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ServiciosOfrecidos = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    EsVerificado = table.Column<bool>(type: "boolean", nullable: false),
                    EsActivo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaVerificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Talleres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tiendas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UrlSitioWeb = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    LogoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Descripcion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EsActivo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tiendas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Apellido = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Biografia = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EsActivo = table.Column<bool>(type: "boolean", nullable: false),
                    EspecialidadAutomotriz = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    AnosExperiencia = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Modelos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MarcaId = table.Column<int>(type: "integer", nullable: false),
                    ImagenUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Descripcion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    AnioInicioProduccion = table.Column<int>(type: "integer", nullable: true),
                    AnioFinProduccion = table.Column<int>(type: "integer", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EsActivo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modelos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Modelos_Marcas_MarcaId",
                        column: x => x.MarcaId,
                        principalTable: "Marcas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Resena",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Calificacion = table.Column<int>(type: "integer", nullable: false),
                    Comentario = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    FechaPublicacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TallerId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<string>(type: "text", nullable: false),
                    NombreUsuario = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resena", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Resena_Talleres_TallerId",
                        column: x => x.TallerId,
                        principalTable: "Talleres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ofertas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductoId = table.Column<int>(type: "integer", nullable: false),
                    TiendaId = table.Column<int>(type: "integer", nullable: false),
                    Precio = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    UrlProductoEnTienda = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    SKU = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    EsDisponible = table.Column<bool>(type: "boolean", nullable: false),
                    EsOferta = table.Column<bool>(type: "boolean", nullable: false),
                    PrecioAnterior = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EsActivo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ofertas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ofertas_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ofertas_Tiendas_TiendaId",
                        column: x => x.TiendaId,
                        principalTable: "Tiendas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PublicacionesForo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Contenido = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    Categoria = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Etiquetas = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Vistas = table.Column<int>(type: "integer", nullable: false),
                    Likes = table.Column<int>(type: "integer", nullable: false),
                    EsDestacado = table.Column<bool>(type: "boolean", nullable: false),
                    EsCerrado = table.Column<bool>(type: "boolean", nullable: false),
                    EsActivo = table.Column<bool>(type: "boolean", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicacionesForo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PublicacionesForo_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResenasTaller",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Calificacion = table.Column<int>(type: "integer", nullable: false),
                    Comentario = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EsActivo = table.Column<bool>(type: "boolean", nullable: false),
                    TallerId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResenasTaller", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResenasTaller_Talleres_TallerId",
                        column: x => x.TallerId,
                        principalTable: "Talleres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResenasTaller_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vehiculos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Marca = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Modelo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Ano = table.Column<int>(type: "integer", nullable: false),
                    Version = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Patente = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    TipoMotor = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TipoCombustible = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Transmision = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Kilometraje = table.Column<int>(type: "integer", nullable: true),
                    Color = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Descripcion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EsActivo = table.Column<bool>(type: "boolean", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehiculos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehiculos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductoVehiculoCompatibles",
                columns: table => new
                {
                    ProductoId = table.Column<int>(type: "integer", nullable: false),
                    ModeloId = table.Column<int>(type: "integer", nullable: false),
                    Ano = table.Column<int>(type: "integer", nullable: false),
                    NotasCompatibilidad = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EsActivo = table.Column<bool>(type: "boolean", nullable: false),
                    ModeloId1 = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductoVehiculoCompatibles", x => new { x.ProductoId, x.ModeloId });
                    table.ForeignKey(
                        name: "FK_ProductoVehiculoCompatibles_Modelos_ModeloId",
                        column: x => x.ModeloId,
                        principalTable: "Modelos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductoVehiculoCompatibles_Modelos_ModeloId1",
                        column: x => x.ModeloId1,
                        principalTable: "Modelos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductoVehiculoCompatibles_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RespuestasForo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Contenido = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Likes = table.Column<int>(type: "integer", nullable: false),
                    EsRespuestaAceptada = table.Column<bool>(type: "boolean", nullable: false),
                    EsActivo = table.Column<bool>(type: "boolean", nullable: false),
                    PublicacionId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    RespuestaPadreId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RespuestasForo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RespuestasForo_PublicacionesForo_PublicacionId",
                        column: x => x.PublicacionId,
                        principalTable: "PublicacionesForo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RespuestasForo_RespuestasForo_RespuestaPadreId",
                        column: x => x.RespuestaPadreId,
                        principalTable: "RespuestasForo",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RespuestasForo_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Marcas",
                columns: new[] { "Id", "Descripcion", "EsActivo", "FechaCreacion", "LogoUrl", "Nombre" },
                values: new object[,]
                {
                    { 1, null, true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9472), "/images/marcas/toyota.png", "Toyota" },
                    { 2, null, true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9474), "/images/marcas/honda.png", "Honda" },
                    { 3, null, true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9475), "/images/marcas/nissan.png", "Nissan" },
                    { 4, null, true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9477), "/images/marcas/chevrolet.png", "Chevrolet" },
                    { 5, null, true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9478), "/images/marcas/ford.png", "Ford" }
                });

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "Id", "Descripcion", "EsActivo", "FechaCreacion", "ImagenUrl", "Nombre", "NumeroDeParte" },
                values: new object[,]
                {
                    { 1, "Pastillas de freno cerámicas para mayor durabilidad y menor ruido", true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9802), "/images/productos/pastillas-freno-bosch.jpg", "Pastillas de Freno Delanteras", "BP-1234" },
                    { 2, "Filtro de aceite de alta calidad para motor", true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9810), "/images/productos/filtro-aceite-mann.jpg", "Filtro de Aceite", "FO-9012" },
                    { 3, "Amortiguador de gas presurizado para mejor confort y control", true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9812), "/images/productos/amortiguador-monroe.jpg", "Amortiguador Delantero", "AD-7890" },
                    { 4, "Batería de arranque libre de mantenimiento", true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9813), "/images/productos/bateria-bosch.jpg", "Batería 12V 65Ah", "BT-9753" },
                    { 5, "Aceite sintético premium para motores de alta performance", true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9815), "/images/productos/aceite-castrol.jpg", "Aceite Motor 5W-30 Sintético", "AM-2468" }
                });

            migrationBuilder.InsertData(
                table: "Talleres",
                columns: new[] { "Id", "CalificacionPromedio", "Ciudad", "CodigoPostal", "Descripcion", "Direccion", "Email", "EsActivo", "EsVerificado", "Especialidades", "FechaRegistro", "FechaVerificacion", "HorarioAtencion", "Latitud", "Longitud", "Nombre", "Region", "ServiciosOfrecidos", "SitioWeb", "Telefono", "TotalResenas" },
                values: new object[,]
                {
                    { 1, 4.5m, "", null, null, "Av. San Miguel 1234, Santiago", "contacto@tallersanmiguel.cl", true, false, "Mecánica general, Frenos, Suspensión", new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9991), null, "Lunes a Viernes: 8:00 - 18:00, Sábados: 8:00 - 14:00", null, null, "Taller Mecánico San Miguel", "", null, null, "+56912345678", 0 },
                    { 2, 4.2m, "", null, null, "Las Condes 5678, Las Condes", "info@autoserviceexpress.cl", true, false, "Mantención preventiva, Cambio de aceite, Afinación", new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9999), null, "Lunes a Viernes: 7:30 - 19:00, Sábados: 8:00 - 15:00", null, null, "AutoServicio Express", "", null, null, "+56987654321", 0 }
                });

            // ⚠️ TIENDAS COMENTADAS: Ahora se crean desde DataSeeder.cs con nombres correctos
            // Las tiendas deben coincidir con los nombres de los scrapers:
            // - Autoplanet (AutoplanetScraperService.TiendaNombre)
            // - MercadoLibre (MercadoLibreScraperService.TiendaNombre)
            // - MundoRepuestos (MundoRepuestosScraperService.TiendaNombre)
            
            /*
            migrationBuilder.InsertData(
                table: "Tiendas",
                columns: new[] { "Id", "Descripcion", "EsActivo", "FechaCreacion", "LogoUrl", "Nombre", "UrlSitioWeb" },
                values: new object[,]
                {
                    { 1, "Tu tienda de confianza para repuestos automotrices", true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9767), "/images/tiendas/repuestos-santiago.png", "Repuestos Santiago", "https://repuestossantiago.cl" },
                    { 2, "Especialistas en repuestos importados y nacionales", true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9771), "/images/tiendas/autopartes-chile.png", "AutoPartes Chile", "https://autoparteschile.cl" },
                    { 3, "Los mejores precios en repuestos automotrices", true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9772), "/images/tiendas/mega-repuestos.png", "MegaRepuestos", "https://megarepuestos.cl" }
                });
            */

            migrationBuilder.InsertData(
                table: "Modelos",
                columns: new[] { "Id", "AnioFinProduccion", "AnioInicioProduccion", "Descripcion", "EsActivo", "FechaCreacion", "ImagenUrl", "MarcaId", "Nombre" },
                values: new object[,]
                {
                    { 1, 2024, 2000, null, true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9713), null, 1, "Corolla" },
                    { 2, 2024, 2005, null, true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9718), null, 1, "Yaris" },
                    { 3, 2024, 2010, null, true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9720), null, 1, "RAV4" },
                    { 4, 2024, 2000, null, true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9721), null, 2, "Civic" },
                    { 5, 2024, 2008, null, true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9723), null, 2, "Accord" },
                    { 6, 2024, 2012, null, true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9724), null, 2, "CR-V" },
                    { 7, 2024, 2007, null, true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9726), null, 3, "Sentra" },
                    { 8, 2024, 2012, null, true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9727), null, 3, "Versa" },
                    { 9, 2024, 2014, null, true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9729), null, 3, "X-Trail" }
                });

            // ⚠️ OFERTAS COMENTADAS: Estas ofertas referencian las tiendas antiguas
            // Las ofertas reales se generarán dinámicamente mediante los scrapers
            
            /*
            migrationBuilder.InsertData(
                table: "Ofertas",
                columns: new[] { "Id", "EsActivo", "EsDisponible", "EsOferta", "FechaActualizacion", "FechaCreacion", "Precio", "PrecioAnterior", "ProductoId", "SKU", "TiendaId", "UrlProductoEnTienda" },
                values: new object[,]
                {
                    { 1, true, true, true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9846), new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9846), 35000m, 42000m, 1, "BP-1234-RS", 1, "https://repuestossantiago.cl/productos/pastillas-freno-bp1234" },
                    { 2, true, true, false, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9860), new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9860), 38000m, null, 1, "BP-1234-AC", 2, "https://autoparteschile.cl/pastillas-bosch-bp1234" },
                    { 3, true, true, false, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9863), new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9862), 33000m, null, 1, "BP-1234-MR", 3, "https://megarepuestos.cl/frenos/pastillas-bp1234" },
                    { 4, true, true, false, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9864), new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9864), 8500m, null, 2, "FO-9012-RS", 1, "https://repuestossantiago.cl/productos/filtro-aceite-fo9012" },
                    { 5, true, true, false, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9866), new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9866), 9200m, null, 2, "FO-9012-AC", 2, "https://autoparteschile.cl/filtros/aceite-mann-fo9012" },
                    { 6, true, true, false, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9901), new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9901), 85000m, null, 3, "AD-7890-AC", 2, "https://autoparteschile.cl/suspension/amortiguador-monroe-ad7890" },
                    { 7, true, true, false, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9904), new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9904), 89000m, null, 4, "BT-9753-RS", 1, "https://repuestossantiago.cl/productos/bateria-bosch-bt9753" },
                    { 8, true, true, true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9906), new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9905), 43500m, 48000m, 5, "AM-2468-MR", 3, "https://megarepuestos.cl/lubricantes/aceite-am2468" }
                });
            */

            migrationBuilder.InsertData(
                table: "ProductoVehiculoCompatibles",
                columns: new[] { "ModeloId", "ProductoId", "Ano", "EsActivo", "FechaCreacion", "ModeloId1", "NotasCompatibilidad" },
                values: new object[,]
                {
                    { 1, 1, 0, true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9938), null, null },
                    { 2, 1, 0, true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9940), null, null },
                    { 1, 2, 0, true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9940), null, null },
                    { 2, 2, 0, true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9941), null, null },
                    { 4, 2, 0, true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9942), null, null },
                    { 7, 3, 0, true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9943), null, null },
                    { 8, 3, 0, true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9944), null, null },
                    { 1, 4, 0, true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9945), null, null },
                    { 2, 4, 0, true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9945), null, null },
                    { 4, 4, 0, true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9946), null, null },
                    { 5, 4, 0, true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9947), null, null },
                    { 7, 4, 0, true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9948), null, null },
                    { 1, 5, 0, true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9949), null, null },
                    { 4, 5, 0, true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9949), null, null },
                    { 5, 5, 0, true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9950), null, null },
                    { 7, 5, 0, true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9951), null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Modelos_MarcaId",
                table: "Modelos",
                column: "MarcaId");

            migrationBuilder.CreateIndex(
                name: "IX_Ofertas_ProductoId",
                table: "Ofertas",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Ofertas_TiendaId",
                table: "Ofertas",
                column: "TiendaId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoVehiculoCompatibles_ModeloId",
                table: "ProductoVehiculoCompatibles",
                column: "ModeloId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoVehiculoCompatibles_ModeloId1",
                table: "ProductoVehiculoCompatibles",
                column: "ModeloId1");

            migrationBuilder.CreateIndex(
                name: "IX_PublicacionesForo_UsuarioId",
                table: "PublicacionesForo",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Resena_TallerId",
                table: "Resena",
                column: "TallerId");

            migrationBuilder.CreateIndex(
                name: "IX_ResenasTaller_TallerId",
                table: "ResenasTaller",
                column: "TallerId");

            migrationBuilder.CreateIndex(
                name: "IX_ResenasTaller_UsuarioId",
                table: "ResenasTaller",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_RespuestasForo_PublicacionId",
                table: "RespuestasForo",
                column: "PublicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_RespuestasForo_RespuestaPadreId",
                table: "RespuestasForo",
                column: "RespuestaPadreId");

            migrationBuilder.CreateIndex(
                name: "IX_RespuestasForo_UsuarioId",
                table: "RespuestasForo",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_UsuarioId",
                table: "Vehiculos",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ofertas");

            migrationBuilder.DropTable(
                name: "ProductoVehiculoCompatibles");

            migrationBuilder.DropTable(
                name: "Resena");

            migrationBuilder.DropTable(
                name: "ResenasTaller");

            migrationBuilder.DropTable(
                name: "RespuestasForo");

            migrationBuilder.DropTable(
                name: "Vehiculos");

            migrationBuilder.DropTable(
                name: "Tiendas");

            migrationBuilder.DropTable(
                name: "Modelos");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Talleres");

            migrationBuilder.DropTable(
                name: "PublicacionesForo");

            migrationBuilder.DropTable(
                name: "Marcas");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
