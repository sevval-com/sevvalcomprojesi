using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sevval.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class VideolarSayfasiTablosuEklendi2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DislikeSayisi",
                table: "VideolarSayfasi",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "VideoLikes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VideoId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    IsLike = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoLikes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideoLikes_VideolarSayfasi_VideoId",
                        column: x => x.VideoId,
                        principalTable: "VideolarSayfasi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VideoYorumlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VideoId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    YorumMetni = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    YorumTarihi = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoYorumlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoYorumlari_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideoYorumlari_VideolarSayfasi_VideoId",
                        column: x => x.VideoId,
                        principalTable: "VideolarSayfasi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VideoLikes_UserId",
                table: "VideoLikes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoLikes_VideoId",
                table: "VideoLikes",
                column: "VideoId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoYorumlari_UserId",
                table: "VideoYorumlari",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoYorumlari_VideoId",
                table: "VideoYorumlari",
                column: "VideoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VideoLikes");

            migrationBuilder.DropTable(
                name: "VideoYorumlari");

            migrationBuilder.DropColumn(
                name: "DislikeSayisi",
                table: "VideolarSayfasi");
        }
    }
}
