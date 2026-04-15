using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration151 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "jobGradeIDMax",
                table: "Jobs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "jobPlacementCareer",
                table: "JobPlacementHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_jobGradeIDMax",
                table: "Jobs",
                column: "jobGradeIDMax");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_JobGrades_jobGradeIDMax",
                table: "Jobs",
                column: "jobGradeIDMax",
                principalTable: "JobGrades",
                principalColumn: "jobGradeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_JobGrades_jobGradeIDMax",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_jobGradeIDMax",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "jobGradeIDMax",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "jobPlacementCareer",
                table: "JobPlacementHistories");
        }
    }
}
