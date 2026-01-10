using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sevval.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMessagingListingId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ListingId",
                table: "MessagingMessages",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessagingMessages_ListingId_RecipientId_SenderId_CreatedOnUtc",
                table: "MessagingMessages",
                columns: new[] { "ListingId", "RecipientId", "SenderId", "CreatedOnUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_MessagingMessages_ListingId_SenderId_RecipientId_CreatedOnUtc",
                table: "MessagingMessages",
                columns: new[] { "ListingId", "SenderId", "RecipientId", "CreatedOnUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MessagingMessages_ListingId_RecipientId_SenderId_CreatedOnUtc",
                table: "MessagingMessages");

            migrationBuilder.DropIndex(
                name: "IX_MessagingMessages_ListingId_SenderId_RecipientId_CreatedOnUtc",
                table: "MessagingMessages");

            migrationBuilder.DropColumn(
                name: "ListingId",
                table: "MessagingMessages");
        }
    }
}
