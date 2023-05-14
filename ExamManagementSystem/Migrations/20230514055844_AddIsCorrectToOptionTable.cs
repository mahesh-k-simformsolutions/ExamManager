using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamManagementSystem.Migrations
{
    public partial class AddIsCorrectToOptionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrectOptionId",
                table: "Questions");

            migrationBuilder.AddColumn<bool>(
                name: "IsCorrect",
                table: "Options",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCorrect",
                table: "Options");

            migrationBuilder.AddColumn<int>(
                name: "CorrectOptionId",
                table: "Questions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
