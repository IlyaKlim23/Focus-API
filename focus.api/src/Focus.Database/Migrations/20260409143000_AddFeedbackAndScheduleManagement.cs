using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Focus.Database.Migrations;

public partial class AddFeedbackAndScheduleManagement : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "UserFeedbacks",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                Message = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                Rating = table.Column<int>(type: "integer", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserFeedbacks", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_UserFeedbacks_UserId_CreatedAt",
            table: "UserFeedbacks",
            columns: new[] { "UserId", "CreatedAt" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "UserFeedbacks");
    }
}
