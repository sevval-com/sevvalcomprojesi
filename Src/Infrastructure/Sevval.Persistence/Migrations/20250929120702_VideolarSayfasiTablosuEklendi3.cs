using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sevval.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class VideolarSayfasiTablosuEklendi3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentYorumId",
                table: "VideoYorumlari",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VideoYorumlari_ParentYorumId",
                table: "VideoYorumlari",
                column: "ParentYorumId");

            migrationBuilder.AddForeignKey(
                name: "FK_VideoYorumlari_VideoYorumlari_ParentYorumId",
                table: "VideoYorumlari",
                column: "ParentYorumId",
                principalTable: "VideoYorumlari",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VideoYorumlari_VideoYorumlari_ParentYorumId",
                table: "VideoYorumlari");

            migrationBuilder.DropIndex(
                name: "IX_VideoYorumlari_ParentYorumId",
                table: "VideoYorumlari");

            migrationBuilder.DropColumn(
                name: "ParentYorumId",
                table: "VideoYorumlari");
        }
    }
}
