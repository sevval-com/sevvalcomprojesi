using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sevval.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddVideoApprovalFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovalDate",
                table: "VideolarSayfasi",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApprovalStatus",
                table: "VideolarSayfasi",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedByUserId",
                table: "VideolarSayfasi",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "VideolarSayfasi",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            // Index for efficient pending video queries
            migrationBuilder.CreateIndex(
                name: "IX_VideolarSayfasi_ApprovalStatus",
                table: "VideolarSayfasi",
                column: "ApprovalStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VideolarSayfasi_ApprovalStatus",
                table: "VideolarSayfasi");

            migrationBuilder.DropColumn(
                name: "ApprovalDate",
                table: "VideolarSayfasi");

            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "VideolarSayfasi");

            migrationBuilder.DropColumn(
                name: "ApprovedByUserId",
                table: "VideolarSayfasi");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "VideolarSayfasi");
        }
    }
}
