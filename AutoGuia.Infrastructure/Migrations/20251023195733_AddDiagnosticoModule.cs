using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AutoGuia.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDiagnosticoModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "diagnostico");

            migrationBuilder.CreateTable(
                name: "sistemas_automotrices",
                schema: "diagnostico",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    es_activo = table.Column<bool>(type: "boolean", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sistemas_automotrices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tiendas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    UrlSitioWeb = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    LogoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Direccion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Telefono = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    EsConfiable = table.Column<bool>(type: "boolean", nullable: false),
                    EsVerificada = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EsActivo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tiendas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sintomas",
                schema: "diagnostico",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sistema_automotriz_id = table.Column<int>(type: "integer", nullable: false),
                    descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    descripcion_tecnica = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    nivel_urgencia = table.Column<int>(type: "integer", nullable: false),
                    es_activo = table.Column<bool>(type: "boolean", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sintomas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sintomas_sistemas_automotrices_sistema_automotriz_id",
                        column: x => x.sistema_automotriz_id,
                        principalSchema: "diagnostico",
                        principalTable: "sistemas_automotrices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "causas_posibles",
                schema: "diagnostico",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sintoma_id = table.Column<int>(type: "integer", nullable: false),
                    descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    nivel_probabilidad = table.Column<int>(type: "integer", nullable: false),
                    descripcion_detallada = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    requiere_servicio_profesional = table.Column<bool>(type: "boolean", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_causas_posibles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_causas_posibles_sintomas_sintoma_id",
                        column: x => x.sintoma_id,
                        principalSchema: "diagnostico",
                        principalTable: "sintomas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "consultas_diagnostico",
                schema: "diagnostico",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usuario_id = table.Column<int>(type: "integer", nullable: false),
                    sintoma_descrito = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    fecha_consulta = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    respuesta_asistente = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    fue_util = table.Column<bool>(type: "boolean", nullable: false),
                    sintoma_relacionado_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_consultas_diagnostico", x => x.Id);
                    table.ForeignKey(
                        name: "FK_consultas_diagnostico_Usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_consultas_diagnostico_sintomas_sintoma_relacionado_id",
                        column: x => x.sintoma_relacionado_id,
                        principalSchema: "diagnostico",
                        principalTable: "sintomas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "pasos_verificacion",
                schema: "diagnostico",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    causa_posible_id = table.Column<int>(type: "integer", nullable: false),
                    descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    orden = table.Column<int>(type: "integer", nullable: false),
                    instrucciones_detalladas = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    indicadores_exito = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pasos_verificacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_pasos_verificacion_causas_posibles_causa_posible_id",
                        column: x => x.causa_posible_id,
                        principalSchema: "diagnostico",
                        principalTable: "causas_posibles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recomendaciones_preventivas",
                schema: "diagnostico",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    causa_posible_id = table.Column<int>(type: "integer", nullable: false),
                    descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    detalle = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    frecuencia_kilometros = table.Column<int>(type: "integer", nullable: false),
                    frecuencia_meses = table.Column<int>(type: "integer", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recomendaciones_preventivas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_recomendaciones_preventivas_causas_posibles_causa_posible_id",
                        column: x => x.causa_posible_id,
                        principalSchema: "diagnostico",
                        principalTable: "causas_posibles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 23, 19, 57, 32, 244, DateTimeKind.Utc).AddTicks(6501));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 23, 19, 57, 32, 244, DateTimeKind.Utc).AddTicks(6503));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 23, 19, 57, 32, 244, DateTimeKind.Utc).AddTicks(6505));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 23, 19, 57, 32, 244, DateTimeKind.Utc).AddTicks(6507));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 23, 19, 57, 32, 244, DateTimeKind.Utc).AddTicks(6509));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 23, 19, 57, 32, 244, DateTimeKind.Utc).AddTicks(6511));

            migrationBuilder.UpdateData(
                table: "Marcas",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 23, 19, 57, 32, 244, DateTimeKind.Utc).AddTicks(6132));

            migrationBuilder.UpdateData(
                table: "Marcas",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 23, 19, 57, 32, 244, DateTimeKind.Utc).AddTicks(6139));

            migrationBuilder.UpdateData(
                table: "Marcas",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 23, 19, 57, 32, 244, DateTimeKind.Utc).AddTicks(6141));

            migrationBuilder.UpdateData(
                table: "Marcas",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 23, 19, 57, 32, 244, DateTimeKind.Utc).AddTicks(6211));

            migrationBuilder.UpdateData(
                table: "Marcas",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 23, 19, 57, 32, 244, DateTimeKind.Utc).AddTicks(6213));

            migrationBuilder.UpdateData(
                table: "Modelos",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 23, 19, 57, 32, 244, DateTimeKind.Utc).AddTicks(6444));

            migrationBuilder.UpdateData(
                table: "Modelos",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 23, 19, 57, 32, 244, DateTimeKind.Utc).AddTicks(6448));

            migrationBuilder.UpdateData(
                table: "Modelos",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 23, 19, 57, 32, 244, DateTimeKind.Utc).AddTicks(6449));

            migrationBuilder.UpdateData(
                table: "Modelos",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 23, 19, 57, 32, 244, DateTimeKind.Utc).AddTicks(6451));

            migrationBuilder.UpdateData(
                table: "Modelos",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 23, 19, 57, 32, 244, DateTimeKind.Utc).AddTicks(6452));

            migrationBuilder.UpdateData(
                table: "Modelos",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 23, 19, 57, 32, 244, DateTimeKind.Utc).AddTicks(6454));

            migrationBuilder.UpdateData(
                table: "Modelos",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 23, 19, 57, 32, 244, DateTimeKind.Utc).AddTicks(6455));

            migrationBuilder.UpdateData(
                table: "Modelos",
                keyColumn: "Id",
                keyValue: 8,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 23, 19, 57, 32, 244, DateTimeKind.Utc).AddTicks(6457));

            migrationBuilder.UpdateData(
                table: "Modelos",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 23, 19, 57, 32, 244, DateTimeKind.Utc).AddTicks(6458));

            migrationBuilder.UpdateData(
                table: "Talleres",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaRegistro",
                value: new DateTime(2025, 10, 23, 19, 57, 32, 244, DateTimeKind.Utc).AddTicks(6661));

            migrationBuilder.UpdateData(
                table: "Talleres",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaRegistro",
                value: new DateTime(2025, 10, 23, 19, 57, 32, 244, DateTimeKind.Utc).AddTicks(6671));

            migrationBuilder.CreateIndex(
                name: "IX_Ofertas_TiendaId",
                table: "Ofertas",
                column: "TiendaId");

            migrationBuilder.CreateIndex(
                name: "IX_causas_posibles_sintoma_id",
                schema: "diagnostico",
                table: "causas_posibles",
                column: "sintoma_id");

            migrationBuilder.CreateIndex(
                name: "IX_consultas_diagnostico_sintoma_relacionado_id",
                schema: "diagnostico",
                table: "consultas_diagnostico",
                column: "sintoma_relacionado_id");

            migrationBuilder.CreateIndex(
                name: "IX_consultas_diagnostico_usuario_id",
                schema: "diagnostico",
                table: "consultas_diagnostico",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_pasos_verificacion_causa_posible_id",
                schema: "diagnostico",
                table: "pasos_verificacion",
                column: "causa_posible_id");

            migrationBuilder.CreateIndex(
                name: "IX_recomendaciones_preventivas_causa_posible_id",
                schema: "diagnostico",
                table: "recomendaciones_preventivas",
                column: "causa_posible_id");

            migrationBuilder.CreateIndex(
                name: "IX_sintomas_sistema_automotriz_id",
                schema: "diagnostico",
                table: "sintomas",
                column: "sistema_automotriz_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ofertas_Tiendas_TiendaId",
                table: "Ofertas",
                column: "TiendaId",
                principalTable: "Tiendas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ofertas_Tiendas_TiendaId",
                table: "Ofertas");

            migrationBuilder.DropTable(
                name: "consultas_diagnostico",
                schema: "diagnostico");

            migrationBuilder.DropTable(
                name: "pasos_verificacion",
                schema: "diagnostico");

            migrationBuilder.DropTable(
                name: "recomendaciones_preventivas",
                schema: "diagnostico");

            migrationBuilder.DropTable(
                name: "Tiendas");

            migrationBuilder.DropTable(
                name: "causas_posibles",
                schema: "diagnostico");

            migrationBuilder.DropTable(
                name: "sintomas",
                schema: "diagnostico");

            migrationBuilder.DropTable(
                name: "sistemas_automotrices",
                schema: "diagnostico");

            migrationBuilder.DropIndex(
                name: "IX_Ofertas_TiendaId",
                table: "Ofertas");

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
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 20, 3, 34, 3, 712, DateTimeKind.Utc).AddTicks(4663));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2025, 10, 20, 3, 34, 3, 712, DateTimeKind.Utc).AddTicks(4664));

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
        }
    }
}
