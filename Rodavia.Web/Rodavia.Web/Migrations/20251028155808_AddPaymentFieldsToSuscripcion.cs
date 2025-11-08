using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rodavia.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentFieldsToSuscripcion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastPaymentStatus",
                table: "Suscripciones",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastTransactionId",
                table: "Suscripciones",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextBillingDate",
                table: "Suscripciones",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentCustomerId",
                table: "Suscripciones",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethodToken",
                table: "Suscripciones",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentProvider",
                table: "Suscripciones",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentSubscriptionId",
                table: "Suscripciones",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastPaymentStatus",
                table: "Suscripciones");

            migrationBuilder.DropColumn(
                name: "LastTransactionId",
                table: "Suscripciones");

            migrationBuilder.DropColumn(
                name: "NextBillingDate",
                table: "Suscripciones");

            migrationBuilder.DropColumn(
                name: "PaymentCustomerId",
                table: "Suscripciones");

            migrationBuilder.DropColumn(
                name: "PaymentMethodToken",
                table: "Suscripciones");

            migrationBuilder.DropColumn(
                name: "PaymentProvider",
                table: "Suscripciones");

            migrationBuilder.DropColumn(
                name: "PaymentSubscriptionId",
                table: "Suscripciones");
        }
    }
}
