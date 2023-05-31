using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamManagementSystem.Migrations
{
    public partial class AddExamIdFieldInAnwerTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExamId",
                table: "Answers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Answers_ExamId",
                table: "Answers",
                column: "ExamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Exams_ExamId",
                table: "Answers",
                column: "ExamId",
                principalTable: "Exams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Exams_ExamId",
                table: "Answers");

            migrationBuilder.DropIndex(
                name: "IX_Answers_ExamId",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "ExamId",
                table: "Answers");
        }
    }
}
