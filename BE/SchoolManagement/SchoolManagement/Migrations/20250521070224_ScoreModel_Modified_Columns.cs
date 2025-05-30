using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagement.Migrations
{
    public partial class ScoreModel_Modified_Columns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ScoreNum",
                table: "Scores",
                newName: "Process2");

            migrationBuilder.AddColumn<float>(
                name: "AverageScore",
                table: "Scores",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Final",
                table: "Scores",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Midterm",
                table: "Scores",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Process1",
                table: "Scores",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageScore",
                table: "Scores");

            migrationBuilder.DropColumn(
                name: "Final",
                table: "Scores");

            migrationBuilder.DropColumn(
                name: "Midterm",
                table: "Scores");

            migrationBuilder.DropColumn(
                name: "Process1",
                table: "Scores");

            migrationBuilder.RenameColumn(
                name: "Process2",
                table: "Scores",
                newName: "ScoreNum");
        }
    }
}
