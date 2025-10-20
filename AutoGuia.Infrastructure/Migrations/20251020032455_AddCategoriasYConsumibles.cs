using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AutoGuia.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoriasYConsumibles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ofertas_Tiendas_TiendaId",
                table: "Ofertas");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductoVehiculoCompatibles_Modelos_ModeloId",
                table: "ProductoVehiculoCompatibles");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductoVehiculoCompatibles_Modelos_ModeloId1",
                table: "ProductoVehiculoCompatibles");

            migrationBuilder.DropTable(
                name: "Tiendas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductoVehiculoCompatibles",
                table: "ProductoVehiculoCompatibles");

            migrationBuilder.DropIndex(
                name: "IX_ProductoVehiculoCompatibles_ModeloId1",
                table: "ProductoVehiculoCompatibles");

            migrationBuilder.DropIndex(
                name: "IX_Ofertas_TiendaId",
                table: "Ofertas");

            migrationBuilder.DeleteData(
                table: "Ofertas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Ofertas",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Ofertas",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Ofertas",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Ofertas",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Ofertas",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Ofertas",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Ofertas",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "ProductoVehiculoCompatibles",
                keyColumns: new[] { "ModeloId", "ProductoId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "ProductoVehiculoCompatibles",
                keyColumns: new[] { "ModeloId", "ProductoId" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "ProductoVehiculoCompatibles",
                keyColumns: new[] { "ModeloId", "ProductoId" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "ProductoVehiculoCompatibles",
                keyColumns: new[] { "ModeloId", "ProductoId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "ProductoVehiculoCompatibles",
                keyColumns: new[] { "ModeloId", "ProductoId" },
                keyValues: new object[] { 4, 2 });

            migrationBuilder.DeleteData(
                table: "ProductoVehiculoCompatibles",
                keyColumns: new[] { "ModeloId", "ProductoId" },
                keyValues: new object[] { 7, 3 });

            migrationBuilder.DeleteData(
                table: "ProductoVehiculoCompatibles",
                keyColumns: new[] { "ModeloId", "ProductoId" },
                keyValues: new object[] { 8, 3 });

            migrationBuilder.DeleteData(
                table: "ProductoVehiculoCompatibles",
                keyColumns: new[] { "ModeloId", "ProductoId" },
                keyValues: new object[] { 1, 4 });

            migrationBuilder.DeleteData(
                table: "ProductoVehiculoCompatibles",
                keyColumns: new[] { "ModeloId", "ProductoId" },
                keyValues: new object[] { 2, 4 });

            migrationBuilder.DeleteData(
                table: "ProductoVehiculoCompatibles",
                keyColumns: new[] { "ModeloId", "ProductoId" },
                keyValues: new object[] { 4, 4 });

            migrationBuilder.DeleteData(
                table: "ProductoVehiculoCompatibles",
                keyColumns: new[] { "ModeloId", "ProductoId" },
                keyValues: new object[] { 5, 4 });

            migrationBuilder.DeleteData(
                table: "ProductoVehiculoCompatibles",
                keyColumns: new[] { "ModeloId", "ProductoId" },
                keyValues: new object[] { 7, 4 });

            migrationBuilder.DeleteData(
                table: "ProductoVehiculoCompatibles",
                keyColumns: new[] { "ModeloId", "ProductoId" },
                keyValues: new object[] { 1, 5 });

            migrationBuilder.DeleteData(
                table: "ProductoVehiculoCompatibles",
                keyColumns: new[] { "ModeloId", "ProductoId" },
                keyValues: new object[] { 4, 5 });

            migrationBuilder.DeleteData(
                table: "ProductoVehiculoCompatibles",
                keyColumns: new[] { "ModeloId", "ProductoId" },
                keyValues: new object[] { 5, 5 });

            migrationBuilder.DeleteData(
                table: "ProductoVehiculoCompatibles",
                keyColumns: new[] { "ModeloId", "ProductoId" },
                keyValues: new object[] { 7, 5 });

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DropColumn(
                name: "ModeloId1",
                table: "ProductoVehiculoCompatibles");

            migrationBuilder.AlterColumn<string>(
                name: "NumeroDeParte",
                table: "Productos",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Productos",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CalificacionPromedio",
                table: "Productos",
                type: "numeric(3,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoriaId",
                table: "Productos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Especificaciones",
                table: "Productos",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacion",
                table: "Productos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FiltroValor1",
                table: "Productos",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FiltroValor2",
                table: "Productos",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FiltroValor3",
                table: "Productos",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Marca",
                table: "Productos",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalResenas",
                table: "Productos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductoVehiculoCompatibles",
                table: "ProductoVehiculoCompatibles",
                columns: new[] { "ProductoId", "ModeloId", "Ano" });

            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IconUrl = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EsActivo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subcategorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CategoriaId = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subcategorias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subcategorias_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ValoresFiltro",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SubcategoriaId = table.Column<int>(type: "integer", nullable: false),
                    Valor = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValoresFiltro", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ValoresFiltro_Subcategorias_SubcategoriaId",
                        column: x => x.SubcategoriaId,
                        principalTable: "Subcategorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "Id", "Descripcion", "EsActivo", "FechaCreacion", "IconUrl", "Nombre" },
                values: new object[,]
                {
                    { 1, "Aceites para motor, transmisión y diferencial", true, new DateTime(2025, 10, 20, 3, 24, 54, 411, DateTimeKind.Utc).AddTicks(2293), "/icons/aceites.svg", "Aceites" },
                    { 2, "Neumáticos para todo tipo de vehículos", true, new DateTime(2025, 10, 20, 3, 24, 54, 411, DateTimeKind.Utc).AddTicks(2297), "/icons/neumaticos.svg", "Neumáticos" },
                    { 3, "Plumillas limpiaparabrisas", true, new DateTime(2025, 10, 20, 3, 24, 54, 411, DateTimeKind.Utc).AddTicks(2299), "/icons/plumillas.svg", "Plumillas" },
                    { 4, "Filtros de aire, aceite, combustible y cabina", true, new DateTime(2025, 10, 20, 3, 24, 54, 411, DateTimeKind.Utc).AddTicks(2301), "/icons/filtros.svg", "Filtros" },
                    { 5, "Baterías para automóviles", true, new DateTime(2025, 10, 20, 3, 24, 54, 411, DateTimeKind.Utc).AddTicks(2302), "/icons/baterias.svg", "Baterías" },
                    { 6, "Pastillas, discos y líquido de frenos", true, new DateTime(2025, 10, 20, 3, 24, 54, 411, DateTimeKind.Utc).AddTicks(2304), "/icons/frenos.svg", "Frenos" }
                });

            migrationBuilder.UpdateData(
                table: "Marcas",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 20, 3, 24, 54, 411, DateTimeKind.Utc).AddTicks(2100));

            migrationBuilder.UpdateData(
                table: "Marcas",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 20, 3, 24, 54, 411, DateTimeKind.Utc).AddTicks(2104));

            migrationBuilder.UpdateData(
                table: "Marcas",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 20, 3, 24, 54, 411, DateTimeKind.Utc).AddTicks(2105));

            migrationBuilder.UpdateData(
                table: "Marcas",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 20, 3, 24, 54, 411, DateTimeKind.Utc).AddTicks(2106));

            migrationBuilder.UpdateData(
                table: "Marcas",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 20, 3, 24, 54, 411, DateTimeKind.Utc).AddTicks(2108));

            migrationBuilder.UpdateData(
                table: "Modelos",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 20, 3, 24, 54, 411, DateTimeKind.Utc).AddTicks(2246));

            migrationBuilder.UpdateData(
                table: "Modelos",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 20, 3, 24, 54, 411, DateTimeKind.Utc).AddTicks(2249));

            migrationBuilder.UpdateData(
                table: "Modelos",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 20, 3, 24, 54, 411, DateTimeKind.Utc).AddTicks(2251));

            migrationBuilder.UpdateData(
                table: "Modelos",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 20, 3, 24, 54, 411, DateTimeKind.Utc).AddTicks(2253));

            migrationBuilder.UpdateData(
                table: "Modelos",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 20, 3, 24, 54, 411, DateTimeKind.Utc).AddTicks(2254));

            migrationBuilder.UpdateData(
                table: "Modelos",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 20, 3, 24, 54, 411, DateTimeKind.Utc).AddTicks(2256));

            migrationBuilder.UpdateData(
                table: "Modelos",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 20, 3, 24, 54, 411, DateTimeKind.Utc).AddTicks(2257));

            migrationBuilder.UpdateData(
                table: "Modelos",
                keyColumn: "Id",
                keyValue: 8,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 20, 3, 24, 54, 411, DateTimeKind.Utc).AddTicks(2259));

            migrationBuilder.UpdateData(
                table: "Modelos",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 20, 3, 24, 54, 411, DateTimeKind.Utc).AddTicks(2260));

            migrationBuilder.UpdateData(
                table: "Talleres",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaRegistro",
                value: new DateTime(2025, 10, 20, 3, 24, 54, 411, DateTimeKind.Utc).AddTicks(2467));

            migrationBuilder.UpdateData(
                table: "Talleres",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaRegistro",
                value: new DateTime(2025, 10, 20, 3, 24, 54, 411, DateTimeKind.Utc).AddTicks(2476));

            migrationBuilder.InsertData(
                table: "Subcategorias",
                columns: new[] { "Id", "CategoriaId", "Nombre" },
                values: new object[,]
                {
                    { 1, 1, "Viscosidad" },
                    { 2, 1, "Tipo" },
                    { 3, 1, "Marca" },
                    { 4, 1, "Volumen" },
                    { 5, 2, "Medida" },
                    { 6, 2, "Marca" },
                    { 7, 2, "Tipo" },
                    { 8, 4, "Tipo de Filtro" },
                    { 9, 4, "Marca" }
                });

            migrationBuilder.InsertData(
                table: "ValoresFiltro",
                columns: new[] { "Id", "SubcategoriaId", "Valor" },
                values: new object[,]
                {
                    { 1, 1, "5W-30" },
                    { 2, 1, "10W-30" },
                    { 3, 1, "10W-40" },
                    { 4, 1, "15W-40" },
                    { 5, 1, "20W-50" },
                    { 6, 2, "Sintético" },
                    { 7, 2, "Semi-sintético" },
                    { 8, 2, "Mineral" },
                    { 9, 3, "Castrol" },
                    { 10, 3, "Mobil" },
                    { 11, 3, "Shell" },
                    { 12, 3, "Valvoline" },
                    { 13, 3, "Petronas" },
                    { 14, 4, "1L" },
                    { 15, 4, "4L" },
                    { 16, 4, "5L" },
                    { 17, 4, "208L" },
                    { 18, 5, "175/70 R13" },
                    { 19, 5, "185/65 R14" },
                    { 20, 5, "195/65 R15" },
                    { 21, 5, "205/55 R16" },
                    { 22, 5, "215/55 R17" },
                    { 23, 6, "Michelin" },
                    { 24, 6, "Bridgestone" },
                    { 25, 6, "Goodyear" },
                    { 26, 6, "Continental" },
                    { 27, 8, "Filtro de Aceite" },
                    { 28, 8, "Filtro de Aire" },
                    { 29, 8, "Filtro de Combustible" },
                    { 30, 8, "Filtro de Cabina" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Productos_CategoriaId",
                table: "Productos",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Subcategorias_CategoriaId",
                table: "Subcategorias",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_ValoresFiltro_SubcategoriaId",
                table: "ValoresFiltro",
                column: "SubcategoriaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_Categorias_CategoriaId",
                table: "Productos",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductoVehiculoCompatibles_Modelos_ModeloId",
                table: "ProductoVehiculoCompatibles",
                column: "ModeloId",
                principalTable: "Modelos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Categorias_CategoriaId",
                table: "Productos");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductoVehiculoCompatibles_Modelos_ModeloId",
                table: "ProductoVehiculoCompatibles");

            migrationBuilder.DropTable(
                name: "ValoresFiltro");

            migrationBuilder.DropTable(
                name: "Subcategorias");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductoVehiculoCompatibles",
                table: "ProductoVehiculoCompatibles");

            migrationBuilder.DropIndex(
                name: "IX_Productos_CategoriaId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "CalificacionPromedio",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "CategoriaId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Especificaciones",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "FechaActualizacion",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "FiltroValor1",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "FiltroValor2",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "FiltroValor3",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Marca",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "TotalResenas",
                table: "Productos");

            migrationBuilder.AddColumn<int>(
                name: "ModeloId1",
                table: "ProductoVehiculoCompatibles",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NumeroDeParte",
                table: "Productos",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Productos",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductoVehiculoCompatibles",
                table: "ProductoVehiculoCompatibles",
                columns: new[] { "ProductoId", "ModeloId" });

            migrationBuilder.CreateTable(
                name: "Tiendas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descripcion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    EsActivo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LogoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UrlSitioWeb = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tiendas", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Marcas",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9472));

            migrationBuilder.UpdateData(
                table: "Marcas",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9474));

            migrationBuilder.UpdateData(
                table: "Marcas",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9475));

            migrationBuilder.UpdateData(
                table: "Marcas",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9477));

            migrationBuilder.UpdateData(
                table: "Marcas",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9478));

            migrationBuilder.UpdateData(
                table: "Modelos",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9713));

            migrationBuilder.UpdateData(
                table: "Modelos",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9718));

            migrationBuilder.UpdateData(
                table: "Modelos",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9720));

            migrationBuilder.UpdateData(
                table: "Modelos",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9721));

            migrationBuilder.UpdateData(
                table: "Modelos",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9723));

            migrationBuilder.UpdateData(
                table: "Modelos",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9724));

            migrationBuilder.UpdateData(
                table: "Modelos",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9726));

            migrationBuilder.UpdateData(
                table: "Modelos",
                keyColumn: "Id",
                keyValue: 8,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9727));

            migrationBuilder.UpdateData(
                table: "Modelos",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9729));

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

            migrationBuilder.UpdateData(
                table: "Talleres",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaRegistro",
                value: new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9991));

            migrationBuilder.UpdateData(
                table: "Talleres",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaRegistro",
                value: new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9999));

            migrationBuilder.InsertData(
                table: "Tiendas",
                columns: new[] { "Id", "Descripcion", "EsActivo", "FechaCreacion", "LogoUrl", "Nombre", "UrlSitioWeb" },
                values: new object[,]
                {
                    { 1, "Tu tienda de confianza para repuestos automotrices", true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9767), "/images/tiendas/repuestos-santiago.png", "Repuestos Santiago", "https://repuestossantiago.cl" },
                    { 2, "Especialistas en repuestos importados y nacionales", true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9771), "/images/tiendas/autopartes-chile.png", "AutoPartes Chile", "https://autoparteschile.cl" },
                    { 3, "Los mejores precios en repuestos automotrices", true, new DateTime(2025, 10, 18, 4, 56, 14, 47, DateTimeKind.Utc).AddTicks(9772), "/images/tiendas/mega-repuestos.png", "MegaRepuestos", "https://megarepuestos.cl" }
                });

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
                name: "IX_ProductoVehiculoCompatibles_ModeloId1",
                table: "ProductoVehiculoCompatibles",
                column: "ModeloId1");

            migrationBuilder.CreateIndex(
                name: "IX_Ofertas_TiendaId",
                table: "Ofertas",
                column: "TiendaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ofertas_Tiendas_TiendaId",
                table: "Ofertas",
                column: "TiendaId",
                principalTable: "Tiendas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductoVehiculoCompatibles_Modelos_ModeloId",
                table: "ProductoVehiculoCompatibles",
                column: "ModeloId",
                principalTable: "Modelos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductoVehiculoCompatibles_Modelos_ModeloId1",
                table: "ProductoVehiculoCompatibles",
                column: "ModeloId1",
                principalTable: "Modelos",
                principalColumn: "Id");
        }
    }
}
