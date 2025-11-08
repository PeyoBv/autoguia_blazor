using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Rodavia.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddTransbankPayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<string>(type: "text", nullable: false),
                    TbkToken = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Last4Digits = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    CardType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ExpirationDate = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    CardholderName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    InscriptionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastValidationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FailedAttempts = table.Column<int>(type: "integer", nullable: false),
                    LastFailedAttempt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    InactiveReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransbankTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<string>(type: "text", nullable: false),
                    PaymentMethodId = table.Column<int>(type: "integer", nullable: true),
                    SuscripcionId = table.Column<int>(type: "integer", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TransactionToken = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AuthorizationCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    BuyOrder = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Installments = table.Column<int>(type: "integer", nullable: false),
                    ResponseCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    ResponseMessage = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AccountingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReturnUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Environment = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RequestPayload = table.Column<string>(type: "text", nullable: true),
                    ResponsePayload = table.Column<string>(type: "text", nullable: true),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    ErrorMessage = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    UserIp = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    WebhookProcessed = table.Column<bool>(type: "boolean", nullable: false),
                    WebhookProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransbankTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransbankTransactions_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TransbankTransactions_Suscripciones_SuscripcionId",
                        column: x => x.SuscripcionId,
                        principalTable: "Suscripciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PaymentLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TransactionId = table.Column<int>(type: "integer", nullable: true),
                    UsuarioId = table.Column<string>(type: "text", nullable: true),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Event = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Message = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    AdditionalData = table.Column<string>(type: "text", nullable: true),
                    StackTrace = table.Column<string>(type: "text", nullable: true),
                    IpAddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Source = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DurationMs = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentLogs_TransbankTransactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "TransbankTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentLogs_Event",
                table: "PaymentLogs",
                column: "Event");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentLogs_Level_CreatedAt",
                table: "PaymentLogs",
                columns: new[] { "Level", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentLogs_TransactionId",
                table: "PaymentLogs",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentLogs_UsuarioId",
                table: "PaymentLogs",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_TbkToken",
                table: "PaymentMethods",
                column: "TbkToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_UsuarioId",
                table: "PaymentMethods",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_UsuarioId_IsDefault_IsActive",
                table: "PaymentMethods",
                columns: new[] { "UsuarioId", "IsDefault", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_TransbankTransactions_BuyOrder",
                table: "TransbankTransactions",
                column: "BuyOrder",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransbankTransactions_PaymentMethodId",
                table: "TransbankTransactions",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_TransbankTransactions_Status_CreatedAt",
                table: "TransbankTransactions",
                columns: new[] { "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_TransbankTransactions_SuscripcionId",
                table: "TransbankTransactions",
                column: "SuscripcionId");

            migrationBuilder.CreateIndex(
                name: "IX_TransbankTransactions_TransactionToken",
                table: "TransbankTransactions",
                column: "TransactionToken");

            migrationBuilder.CreateIndex(
                name: "IX_TransbankTransactions_Type_Status",
                table: "TransbankTransactions",
                columns: new[] { "Type", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TransbankTransactions_UsuarioId",
                table: "TransbankTransactions",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentLogs");

            migrationBuilder.DropTable(
                name: "TransbankTransactions");

            migrationBuilder.DropTable(
                name: "PaymentMethods");
        }
    }
}
