using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamManagementSystem.Migrations
{
    public partial class RemovedScorecardTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScoreCards");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScoreCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CorrectAnswerId = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    SelectedAnswerId = table.Column<int>(type: "int", nullable: false),
                    ObtainedMarks = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoreCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScoreCards_Answers_CorrectAnswerId",
                        column: x => x.CorrectAnswerId,
                        principalTable: "Answers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScoreCards_Answers_SelectedAnswerId",
                        column: x => x.SelectedAnswerId,
                        principalTable: "Answers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScoreCards_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScoreCards_CorrectAnswerId",
                table: "ScoreCards",
                column: "CorrectAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreCards_QuestionId",
                table: "ScoreCards",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreCards_SelectedAnswerId",
                table: "ScoreCards",
                column: "SelectedAnswerId");
        }
    }
}
