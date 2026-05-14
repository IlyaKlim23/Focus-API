using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Focus.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddScheduledSlotsAndProductivityPredictions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductivityPredictions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    SlotStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Score = table.Column<double>(type: "double precision", precision: 9, scale: 6, nullable: false),
                    Factors = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductivityPredictions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductivityPredictions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScheduledSlots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    SlotStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduledSlots_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScheduledSlots_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductivityPredictions_UserId_SlotStart",
                table: "ProductivityPredictions",
                columns: new[] { "UserId", "SlotStart" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledSlots_TaskId",
                table: "ScheduledSlots",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledSlots_UserId",
                table: "ScheduledSlots",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledSlots_UserId_SlotStart",
                table: "ScheduledSlots",
                columns: new[] { "UserId", "SlotStart" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductivityPredictions");

            migrationBuilder.DropTable(
                name: "ScheduledSlots");
        }
    }
}
