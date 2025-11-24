using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sevval.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class VideolarSayfasiTablosuEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VideolarSayfasi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VideoAdi = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    VideoAciklamasi = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    VideoYolu = table.Column<string>(type: "TEXT", nullable: false),
                    IsYouTube = table.Column<bool>(type: "INTEGER", nullable: false),
                    KapakFotografiYolu = table.Column<string>(type: "TEXT", nullable: true),
                    YuklenmeTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    BegeniSayisi = table.Column<int>(type: "INTEGER", nullable: false),
                    GoruntulenmeSayisi = table.Column<int>(type: "INTEGER", nullable: false),
                    Kategori = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    YukleyenKullaniciId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideolarSayfasi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideolarSayfasi_AspNetUsers_YukleyenKullaniciId",
                        column: x => x.YukleyenKullaniciId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VideolarSayfasi_YukleyenKullaniciId",
                table: "VideolarSayfasi",
                column: "YukleyenKullaniciId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VideolarSayfasi");
        }
    }
}
