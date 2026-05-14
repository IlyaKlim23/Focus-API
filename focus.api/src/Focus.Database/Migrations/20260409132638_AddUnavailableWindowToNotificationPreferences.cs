using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Focus.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddUnavailableWindowToNotificationPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UnavailableFromMinutes",
                table: "NotificationPreferences",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UnavailableToMinutes",
                table: "NotificationPreferences",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnavailableFromMinutes",
                table: "NotificationPreferences");

            migrationBuilder.DropColumn(
                name: "UnavailableToMinutes",
                table: "NotificationPreferences");
        }
    }
}
