using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AutoGuia.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCategoriasRadiosGadgets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 20, 3, 34, 3, 712, DateTimeKind.Utc).AddTicks(4656));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 20, 3, 34, 3, 712, DateTimeKind.Utc).AddTicks(4658));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 20, 3, 34, 3, 712, DateTimeKind.Utc).AddTicks(4660));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 20, 3, 34, 3, 712, DateTimeKind.Utc).AddTicks(4661));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Descripcion", "FechaCreacion", "IconUrl", "Nombre" },
                values: new object[] { "Radios multimedia para automóviles", new DateTime(2025, 10, 20, 3, 34, 3, 712, DateTimeKind.Utc).AddTicks(4663), "/icons/radios.svg", "Radios" });

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Descripcion", "FechaCreacion", "IconUrl", "Nombre" },
                values: new object[] { "Accesorios y gadgets automotrices", new DateTime(2025, 10, 20, 3, 34, 3, 712, DateTimeKind.Utc).AddTicks(4664), "/icons/gadgets.svg", "Gadgets" });

            migrationBuilder.UpdateData(
                table: "Marcas",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 20, 3, 34, 3, 712, DateTimeKind.Utc).AddTicks(4451));

            migrationBuilder.UpdateData(
                table: "Marcas",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 20, 3, 34, 3, 712, DateTimeKind.Utc).AddTicks(4454));

            migrationBuilder.UpdateData(
                table: "Marcas",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 20, 3, 34, 3, 712, DateTimeKind.Utc).AddTicks(4456));

            migrationBuilder.UpdateData(
                table: "Marcas",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 20, 3, 34, 3, 712, DateTimeKind.Utc).AddTicks(4457));

            migrationBuilder.UpdateData(
                table: "Marcas",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 20, 3, 34, 3, 712, DateTimeKind.Utc).AddTicks(4458));

            migrationBuilder.UpdateData(
                table: "Modelos",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 20, 3, 34, 3, 712, DateTimeKind.Utc).AddTicks(4613));

            migrationBuilder.UpdateData(
                table: "Modelos",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 20, 3, 34, 3, 712, DateTimeKind.Utc).AddTicks(4616));

            migrationBuilder.UpdateData(
                table: "Modelos",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 20, 3, 34, 3, 712, DateTimeKind.Utc).AddTicks(4618));

            migrationBuilder.UpdateData(
                table: "Modelos",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 20, 3, 34, 3, 712, DateTimeKind.Utc).AddTicks(4619));

            migrationBuilder.UpdateData(
                table: "Modelos",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 20, 3, 34, 3, 712, DateTimeKind.Utc).AddTicks(4620));

            migrationBuilder.UpdateData(
                table: "Modelos",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 20, 3, 34, 3, 712, DateTimeKind.Utc).AddTicks(4621));

            migrationBuilder.UpdateData(
                table: "Modelos",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 20, 3, 34, 3, 712, DateTimeKind.Utc).AddTicks(4623));

            migrationBuilder.UpdateData(
                table: "Modelos",
                keyColumn: "Id",
                keyValue: 8,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 20, 3, 34, 3, 712, DateTimeKind.Utc).AddTicks(4624));

            migrationBuilder.UpdateData(
                table: "Modelos",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 20, 3, 34, 3, 712, DateTimeKind.Utc).AddTicks(4625));

            migrationBuilder.UpdateData(
                table: "Subcategorias",
                keyColumn: "Id",
                keyValue: 1,
                column: "Nombre",
                value: "Tipo");

            migrationBuilder.UpdateData(
                table: "Subcategorias",
                keyColumn: "Id",
                keyValue: 2,
                column: "Nombre",
                value: "Viscosidad");

            migrationBuilder.UpdateData(
                table: "Subcategorias",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CategoriaId", "Nombre" },
                values: new object[] { 2, "Tipo" });

            migrationBuilder.UpdateData(
                table: "Subcategorias",
                keyColumn: "Id",
                keyValue: 5,
                column: "Nombre",
                value: "Tamaño");

            migrationBuilder.UpdateData(
                table: "Subcategorias",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CategoriaId", "Nombre" },
                values: new object[] { 3, "Tamaño" });

            migrationBuilder.UpdateData(
                table: "Subcategorias",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CategoriaId", "Nombre" },
                values: new object[] { 3, "Tipo" });

            migrationBuilder.UpdateData(
                table: "Subcategorias",
                keyColumn: "Id",
                keyValue: 9,
                column: "CategoriaId",
                value: 3);

            migrationBuilder.InsertData(
                table: "Subcategorias",
                columns: new[] { "Id", "CategoriaId", "Nombre" },
                values: new object[,]
                {
                    { 10, 4, "Tipo" },
                    { 11, 4, "Marca" },
                    { 12, 5, "Características" },
                    { 13, 5, "Marca" },
                    { 14, 6, "Tipo" },
                    { 15, 6, "Categoría" }
                });

            migrationBuilder.UpdateData(
                table: "Talleres",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaRegistro",
                value: new DateTime(2025, 10, 20, 3, 34, 3, 712, DateTimeKind.Utc).AddTicks(4835));

            migrationBuilder.UpdateData(
                table: "Talleres",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaRegistro",
                value: new DateTime(2025, 10, 20, 3, 34, 3, 712, DateTimeKind.Utc).AddTicks(4845));

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 1,
                column: "Valor",
                value: "Motor");

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 2,
                column: "Valor",
                value: "Transmisión");

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 2, "5W-30" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 2, "10W-40" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 2, "15W-40" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 3, "Castrol" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 3, "Mobil" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 4, "Verano" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 4, "Invierno" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 5, "165/70R13" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 5, "205/55R16" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 6, "Michelin" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 6, "Continental" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 7, "400mm" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 7, "450mm" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 7, "500mm" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 8, "Convencional" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 8, "Aerodinámico" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 9, "Bosch" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 9, "TRICO" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 10, "Motor" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 10, "Aire" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 11, "Fram" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 11, "Bosch" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 12, "Bluetooth" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 12, "Android Auto" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 13, "Pioneer" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 13, "Sony" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 14, "Limpieza" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 14, "Protección" });

            migrationBuilder.InsertData(
                table: "ValoresFiltro",
                columns: new[] { "Id", "SubcategoriaId", "Valor" },
                values: new object[,]
                {
                    { 31, 15, "Ceras" },
                    { 32, 15, "Cubre volante" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Subcategorias",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Subcategorias",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Subcategorias",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Subcategorias",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Subcategorias",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Subcategorias",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 20, 3, 24, 54, 411, DateTimeKind.Utc).AddTicks(2293));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 20, 3, 24, 54, 411, DateTimeKind.Utc).AddTicks(2297));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 20, 3, 24, 54, 411, DateTimeKind.Utc).AddTicks(2299));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 20, 3, 24, 54, 411, DateTimeKind.Utc).AddTicks(2301));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Descripcion", "FechaCreacion", "IconUrl", "Nombre" },
                values: new object[] { "Baterías para automóviles", new DateTime(2025, 10, 20, 3, 24, 54, 411, DateTimeKind.Utc).AddTicks(2302), "/icons/baterias.svg", "Baterías" });

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Descripcion", "FechaCreacion", "IconUrl", "Nombre" },
                values: new object[] { "Pastillas, discos y líquido de frenos", new DateTime(2025, 10, 20, 3, 24, 54, 411, DateTimeKind.Utc).AddTicks(2304), "/icons/frenos.svg", "Frenos" });

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
                table: "Subcategorias",
                keyColumn: "Id",
                keyValue: 1,
                column: "Nombre",
                value: "Viscosidad");

            migrationBuilder.UpdateData(
                table: "Subcategorias",
                keyColumn: "Id",
                keyValue: 2,
                column: "Nombre",
                value: "Tipo");

            migrationBuilder.UpdateData(
                table: "Subcategorias",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CategoriaId", "Nombre" },
                values: new object[] { 1, "Volumen" });

            migrationBuilder.UpdateData(
                table: "Subcategorias",
                keyColumn: "Id",
                keyValue: 5,
                column: "Nombre",
                value: "Medida");

            migrationBuilder.UpdateData(
                table: "Subcategorias",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CategoriaId", "Nombre" },
                values: new object[] { 2, "Tipo" });

            migrationBuilder.UpdateData(
                table: "Subcategorias",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CategoriaId", "Nombre" },
                values: new object[] { 4, "Tipo de Filtro" });

            migrationBuilder.UpdateData(
                table: "Subcategorias",
                keyColumn: "Id",
                keyValue: 9,
                column: "CategoriaId",
                value: 4);

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

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 1,
                column: "Valor",
                value: "5W-30");

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 2,
                column: "Valor",
                value: "10W-30");

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 1, "10W-40" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 1, "15W-40" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 1, "20W-50" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 2, "Sintético" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 2, "Semi-sintético" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 2, "Mineral" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 3, "Castrol" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 3, "Mobil" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 3, "Shell" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 3, "Valvoline" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 3, "Petronas" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 4, "1L" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 4, "4L" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 4, "5L" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 4, "208L" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 5, "175/70 R13" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 5, "185/65 R14" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 5, "195/65 R15" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 5, "205/55 R16" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 5, "215/55 R17" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 6, "Michelin" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 6, "Bridgestone" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 6, "Goodyear" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 6, "Continental" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 8, "Filtro de Aceite" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 8, "Filtro de Aire" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 8, "Filtro de Combustible" });

            migrationBuilder.UpdateData(
                table: "ValoresFiltro",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "SubcategoriaId", "Valor" },
                values: new object[] { 8, "Filtro de Cabina" });
        }
    }
}
