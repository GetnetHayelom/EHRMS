using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration57 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_JobGrades_jobGrade",
                table: "Jobs");

            migrationBuilder.RenameColumn(
                name: "jobGrade",
                table: "Jobs",
                newName: "jobGradeID");

            migrationBuilder.RenameIndex(
                name: "IX_Jobs_jobGrade",
                table: "Jobs",
                newName: "IX_Jobs_jobGradeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_JobGrades_jobGradeID",
                table: "Jobs",
                column: "jobGradeID",
                principalTable: "JobGrades",
                principalColumn: "jobGradeID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_JobGrades_jobGradeID",
                table: "Jobs");

            migrationBuilder.RenameColumn(
                name: "jobGradeID",
                table: "Jobs",
                newName: "jobGrade");

            migrationBuilder.RenameIndex(
                name: "IX_Jobs_jobGradeID",
                table: "Jobs",
                newName: "IX_Jobs_jobGrade");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_JobGrades_jobGrade",
                table: "Jobs",
                column: "jobGrade",
                principalTable: "JobGrades",
                principalColumn: "jobGradeID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
