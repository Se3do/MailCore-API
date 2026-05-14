using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MailCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailDelivery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeliveryStatus",
                table: "Emails",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Pending");

            migrationBuilder.AddColumn<string>(
                name: "LastSendError",
                table: "Emails",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SendAttempts",
                table: "Emails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SentAt",
                table: "Emails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Emails_DeliveryStatus",
                table: "Emails",
                column: "DeliveryStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Emails_DeliveryStatus",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "DeliveryStatus",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "LastSendError",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "SendAttempts",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "SentAt",
                table: "Emails");
        }
    }
}
