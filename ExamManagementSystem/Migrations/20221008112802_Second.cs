using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamManagementSystem.Migrations
{
    public partial class Second : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OptionId",
                table: "Answers",
                newName: "AnswerId");

            migrationBuilder.CreateTable(
                name: "ExamToStudents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ExamId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamToStudents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamToStudents_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamToStudents_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Answers_AnswerId",
                table: "Answers",
                column: "AnswerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamToStudents_ExamId",
                table: "ExamToStudents",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamToStudents_StudentId",
                table: "ExamToStudents",
                column: "StudentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Options_AnswerId",
                table: "Answers",
                column: "AnswerId",
                principalTable: "Options",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Options_AnswerId",
                table: "Answers");

            migrationBuilder.DropTable(
                name: "ExamToStudents");

            migrationBuilder.DropIndex(
                name: "IX_Answers_AnswerId",
                table: "Answers");

            migrationBuilder.RenameColumn(
                name: "AnswerId",
                table: "Answers",
                newName: "OptionId");
        }
    }
}
