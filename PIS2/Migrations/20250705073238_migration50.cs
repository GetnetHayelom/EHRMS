using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration50 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JobSteps_jobGradeID",
                table: "JobSteps");

            migrationBuilder.RenameColumn(
                name: "jobPlacementHistory",
                table: "JobPlacementHistories",
                newName: "jobPlacementSalary");

            migrationBuilder.CreateIndex(
                name: "IX_JobSteps_jobGradeID_jobStepNumber_jobStepStatus",
                table: "JobSteps",
                columns: new[] { "jobGradeID", "jobStepNumber", "jobStepStatus" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JobSteps_jobGradeID_jobStepNumber_jobStepStatus",
                table: "JobSteps");

            migrationBuilder.RenameColumn(
                name: "jobPlacementSalary",
                table: "JobPlacementHistories",
                newName: "jobPlacementHistory");

            migrationBuilder.CreateIndex(
                name: "IX_JobSteps_jobGradeID",
                table: "JobSteps",
                column: "jobGradeID");
        }
    }
}
