using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamManagementSystem.Migrations
{
    public partial class Second : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ScoreCards_CorrectAnswerId",
                table: "ScoreCards");

            migrationBuilder.DropIndex(
                name: "IX_ScoreCards_QuestionId",
                table: "ScoreCards");

            migrationBuilder.DropIndex(
                name: "IX_ScoreCards_SelectedAnswerId",
                table: "ScoreCards");

            migrationBuilder.DropIndex(
                name: "IX_ExamToStudents_StudentId",
                table: "ExamToStudents");

            migrationBuilder.DropIndex(
                name: "IX_Answers_AnswerId",
                table: "Answers");

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

            migrationBuilder.CreateIndex(
                name: "IX_ExamToStudents_StudentId",
                table: "ExamToStudents",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_AnswerId",
                table: "Answers",
                column: "AnswerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ScoreCards_CorrectAnswerId",
                table: "ScoreCards");

            migrationBuilder.DropIndex(
                name: "IX_ScoreCards_QuestionId",
                table: "ScoreCards");

            migrationBuilder.DropIndex(
                name: "IX_ScoreCards_SelectedAnswerId",
                table: "ScoreCards");

            migrationBuilder.DropIndex(
                name: "IX_ExamToStudents_StudentId",
                table: "ExamToStudents");

            migrationBuilder.DropIndex(
                name: "IX_Answers_AnswerId",
                table: "Answers");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreCards_CorrectAnswerId",
                table: "ScoreCards",
                column: "CorrectAnswerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScoreCards_QuestionId",
                table: "ScoreCards",
                column: "QuestionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScoreCards_SelectedAnswerId",
                table: "ScoreCards",
                column: "SelectedAnswerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamToStudents_StudentId",
                table: "ExamToStudents",
                column: "StudentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Answers_AnswerId",
                table: "Answers",
                column: "AnswerId",
                unique: true);
        }
    }
}
