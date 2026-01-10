using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sevval.Persistence.Migrations
{
    public partial class AddMessagingTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MessagingMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SenderId = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    RecipientId = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Body = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    MessageType = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessagingMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageReadStates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MessageId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ReaderId = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    DeliveredOnUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ReadOnUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageReadStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageReadStates_MessagingMessages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "MessagingMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessageReadStates_MessageId_ReaderId",
                table: "MessageReadStates",
                columns: new[] { "MessageId", "ReaderId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessagingMessages_RecipientId_SenderId_CreatedOnUtc",
                table: "MessagingMessages",
                columns: new[] { "RecipientId", "SenderId", "CreatedOnUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_MessagingMessages_SenderId_RecipientId_CreatedOnUtc",
                table: "MessagingMessages",
                columns: new[] { "SenderId", "RecipientId", "CreatedOnUtc" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageReadStates");

            migrationBuilder.DropTable(
                name: "MessagingMessages");
        }
    }
}
