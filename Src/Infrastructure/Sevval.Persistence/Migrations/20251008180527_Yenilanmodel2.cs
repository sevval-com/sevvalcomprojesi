using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sevval.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Yenilanmodel2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Cephe",
                table: "IlanBilgileri",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisOzellikler",
                table: "IlanBilgileri",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EngelliVeYasliUygunluk",
                table: "IlanBilgileri",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IcOzellikler",
                table: "IlanBilgileri",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KonutTipi",
                table: "IlanBilgileri",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Muhit",
                table: "IlanBilgileri",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ulasim",
                table: "IlanBilgileri",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cephe",
                table: "IlanBilgileri");

            migrationBuilder.DropColumn(
                name: "DisOzellikler",
                table: "IlanBilgileri");

            migrationBuilder.DropColumn(
                name: "EngelliVeYasliUygunluk",
                table: "IlanBilgileri");

            migrationBuilder.DropColumn(
                name: "IcOzellikler",
                table: "IlanBilgileri");

            migrationBuilder.DropColumn(
                name: "KonutTipi",
                table: "IlanBilgileri");

            migrationBuilder.DropColumn(
                name: "Muhit",
                table: "IlanBilgileri");

            migrationBuilder.DropColumn(
                name: "Ulasim",
                table: "IlanBilgileri");
        }
    }
}
