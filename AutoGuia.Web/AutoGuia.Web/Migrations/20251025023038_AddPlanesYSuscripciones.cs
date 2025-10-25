using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AutoGuia.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanesYSuscripciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Planes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Precio = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    Duracion = table.Column<int>(type: "integer", nullable: false),
                    LimiteDiagnosticos = table.Column<int>(type: "integer", nullable: false),
                    LimiteBusquedas = table.Column<int>(type: "integer", nullable: false),
                    AccesoForo = table.Column<bool>(type: "boolean", nullable: false),
                    AccesoMapas = table.Column<bool>(type: "boolean", nullable: false),
                    SoportePrioritario = table.Column<bool>(type: "boolean", nullable: false),
                    AccesoComparador = table.Column<bool>(type: "boolean", nullable: false),
                    Caracteristicas = table.Column<string[]>(type: "jsonb", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    Destacado = table.Column<bool>(type: "boolean", nullable: false),
                    ColorBadge = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    Orden = table.Column<int>(type: "integer", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Planes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Suscripciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<string>(type: "text", nullable: false),
                    PlanId = table.Column<int>(type: "integer", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    MetodoPago = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ReferenciaFactura = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    TransaccionId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    MontoPagado = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    RenovacionAutomatica = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCancelacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MotivoCancelacion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DiagnosticosUtilizados = table.Column<int>(type: "integer", nullable: false),
                    BusquedasUtilizadas = table.Column<int>(type: "integer", nullable: false),
                    UltimoReseteo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Notas = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suscripciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Suscripciones_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Suscripciones_Planes_PlanId",
                        column: x => x.PlanId,
                        principalTable: "Planes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Planes_Activo_Orden",
                table: "Planes",
                columns: new[] { "Activo", "Orden" });

            migrationBuilder.CreateIndex(
                name: "IX_Planes_Nombre",
                table: "Planes",
                column: "Nombre");

            migrationBuilder.CreateIndex(
                name: "IX_Suscripciones_Estado_FechaVencimiento",
                table: "Suscripciones",
                columns: new[] { "Estado", "FechaVencimiento" });

            migrationBuilder.CreateIndex(
                name: "IX_Suscripciones_PlanId",
                table: "Suscripciones",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Suscripciones_TransaccionId",
                table: "Suscripciones",
                column: "TransaccionId");

            migrationBuilder.CreateIndex(
                name: "IX_Suscripciones_UsuarioId",
                table: "Suscripciones",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Suscripciones");

            migrationBuilder.DropTable(
                name: "Planes");
        }
    }
}
