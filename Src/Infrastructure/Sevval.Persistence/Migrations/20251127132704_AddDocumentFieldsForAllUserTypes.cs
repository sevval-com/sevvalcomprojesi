using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sevval.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentFieldsForAllUserTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RecoveryToken",
                table: "DeletedAccounts",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Document1Path",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Document2Path",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeletedAccounts_RecoveryToken",
                table: "DeletedAccounts",
                column: "RecoveryToken");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DeletedAccounts_RecoveryToken",
                table: "DeletedAccounts");

            migrationBuilder.DropColumn(
                name: "RecoveryToken",
                table: "DeletedAccounts");

            migrationBuilder.DropColumn(
                name: "Document1Path",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Document2Path",
                table: "AspNetUsers");
        }
    }
}
