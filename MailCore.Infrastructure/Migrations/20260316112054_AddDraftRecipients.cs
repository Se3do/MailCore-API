using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MailCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDraftRecipients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BccRecipients",
                table: "Drafts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CcRecipients",
                table: "Drafts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ToRecipients",
                table: "Drafts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BccRecipients",
                table: "Drafts");

            migrationBuilder.DropColumn(
                name: "CcRecipients",
                table: "Drafts");

            migrationBuilder.DropColumn(
                name: "ToRecipients",
                table: "Drafts");
        }
    }
}
