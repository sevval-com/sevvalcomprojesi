using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sevval.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Yenilanmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Altyapi",
                table: "IlanBilgileri",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GenelOzellikler",
                table: "IlanBilgileri",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KonumOzellikleri",
                table: "IlanBilgileri",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Manzara",
                table: "IlanBilgileri",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Altyapi",
                table: "IlanBilgileri");

            migrationBuilder.DropColumn(
                name: "GenelOzellikler",
                table: "IlanBilgileri");

            migrationBuilder.DropColumn(
                name: "KonumOzellikleri",
                table: "IlanBilgileri");

            migrationBuilder.DropColumn(
                name: "Manzara",
                table: "IlanBilgileri");
        }
    }
}
