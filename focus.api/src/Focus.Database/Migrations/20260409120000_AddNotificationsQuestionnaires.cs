using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Focus.Database.Migrations
{
    public partial class AddNotificationsQuestionnaires : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotificationPreferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    RemindBeforeMinutes = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_NotificationPreferences", x => x.Id); });

            migrationBuilder.CreateTable(
                name: "TaskNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    SlotStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ScheduledForUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SentAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    AttemptCount = table.Column<int>(type: "integer", nullable: false),
                    LastError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_TaskNotifications", x => x.Id); });

            migrationBuilder.CreateTable(
                name: "PsychologicalQuestionnaires",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_PsychologicalQuestionnaires", x => x.Id); });

            migrationBuilder.CreateTable(
                name: "PsychologicalQuestionnaireQuestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionnaireId = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    MinValue = table.Column<int>(type: "integer", nullable: false),
                    MaxValue = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_PsychologicalQuestionnaireQuestions", x => x.Id); });

            migrationBuilder.CreateTable(
                name: "UserQuestionnaireSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionnaireId = table.Column<Guid>(type: "uuid", nullable: false),
                    Cadence = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    NextDueAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_UserQuestionnaireSchedules", x => x.Id); });

            migrationBuilder.CreateTable(
                name: "QuestionnaireResponses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionnaireId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubmittedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalScore = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_QuestionnaireResponses", x => x.Id); });

            migrationBuilder.CreateTable(
                name: "QuestionnaireResponseItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ResponseId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_QuestionnaireResponseItems", x => x.Id); });

            migrationBuilder.CreateIndex(name: "IX_NotificationPreferences_UserId", table: "NotificationPreferences", column: "UserId", unique: true);
            migrationBuilder.CreateIndex(name: "IX_TaskNotifications_Status_ScheduledForUtc", table: "TaskNotifications", columns: new[] { "Status", "ScheduledForUtc" });
            migrationBuilder.CreateIndex(name: "IX_TaskNotifications_TaskId_SlotStart", table: "TaskNotifications", columns: new[] { "TaskId", "SlotStart" }, unique: true);
            migrationBuilder.CreateIndex(name: "IX_PsychologicalQuestionnaires_Code", table: "PsychologicalQuestionnaires", column: "Code", unique: true);
            migrationBuilder.CreateIndex(name: "IX_PsychologicalQuestionnaireQuestions_QuestionnaireId_SortOrder", table: "PsychologicalQuestionnaireQuestions", columns: new[] { "QuestionnaireId", "SortOrder" });
            migrationBuilder.CreateIndex(name: "IX_UserQuestionnaireSchedules_UserId_QuestionnaireId", table: "UserQuestionnaireSchedules", columns: new[] { "UserId", "QuestionnaireId" }, unique: true);
            migrationBuilder.CreateIndex(name: "IX_UserQuestionnaireSchedules_IsEnabled_NextDueAtUtc", table: "UserQuestionnaireSchedules", columns: new[] { "IsEnabled", "NextDueAtUtc" });
            migrationBuilder.CreateIndex(name: "IX_QuestionnaireResponses_UserId_SubmittedAtUtc", table: "QuestionnaireResponses", columns: new[] { "UserId", "SubmittedAtUtc" });
            migrationBuilder.CreateIndex(name: "IX_QuestionnaireResponseItems_ResponseId_QuestionId", table: "QuestionnaireResponseItems", columns: new[] { "ResponseId", "QuestionId" }, unique: true);

            var questionnaireId = new Guid("11111111-1111-1111-1111-111111111111");
            migrationBuilder.InsertData(
                table: "PsychologicalQuestionnaires",
                columns: new[] { "Id", "Code", "Name", "Description", "IsActive", "CreatedAt" },
                values: new object[] { questionnaireId, "WELLBEING_WEEKLY", "Еженедельный опрос самочувствия", "Короткий опрос для анализа динамики состояния пользователя.", true, DateTime.UtcNow });
            migrationBuilder.InsertData(
                table: "PsychologicalQuestionnaireQuestions",
                columns: new[] { "Id", "QuestionnaireId", "Text", "SortOrder", "MinValue", "MaxValue" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111112"), questionnaireId, "Насколько вы были энергичны на этой неделе?", 1, 1, 5 },
                    { new Guid("11111111-1111-1111-1111-111111111113"), questionnaireId, "Насколько часто чувствовали стресс?", 2, 1, 5 },
                    { new Guid("11111111-1111-1111-1111-111111111114"), questionnaireId, "Насколько легко удавалось концентрироваться?", 3, 1, 5 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "NotificationPreferences");
            migrationBuilder.DropTable(name: "TaskNotifications");
            migrationBuilder.DropTable(name: "PsychologicalQuestionnaireQuestions");
            migrationBuilder.DropTable(name: "UserQuestionnaireSchedules");
            migrationBuilder.DropTable(name: "QuestionnaireResponses");
            migrationBuilder.DropTable(name: "QuestionnaireResponseItems");
            migrationBuilder.DropTable(name: "PsychologicalQuestionnaires");
        }
    }
}
