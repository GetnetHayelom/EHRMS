using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration152 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobPlacements_JobSteps_jobStepModeljobStepID",
                table: "JobPlacements");

            migrationBuilder.DropIndex(
                name: "IX_JobPlacements_jobStepModeljobStepID",
                table: "JobPlacements");

            migrationBuilder.DropColumn(
                name: "jobStepModeljobStepID",
                table: "JobPlacements");

            migrationBuilder.CreateIndex(
                name: "IX_JobPlacements_jobStepID",
                table: "JobPlacements",
                column: "jobStepID");

            migrationBuilder.AddForeignKey(
                name: "FK_JobPlacements_JobSteps_jobStepID",
                table: "JobPlacements",
                column: "jobStepID",
                principalTable: "JobSteps",
                principalColumn: "jobStepID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobPlacements_JobSteps_jobStepID",
                table: "JobPlacements");

            migrationBuilder.DropIndex(
                name: "IX_JobPlacements_jobStepID",
                table: "JobPlacements");

            migrationBuilder.AddColumn<int>(
                name: "jobStepModeljobStepID",
                table: "JobPlacements",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobPlacements_jobStepModeljobStepID",
                table: "JobPlacements",
                column: "jobStepModeljobStepID");

            migrationBuilder.AddForeignKey(
                name: "FK_JobPlacements_JobSteps_jobStepModeljobStepID",
                table: "JobPlacements",
                column: "jobStepModeljobStepID",
                principalTable: "JobSteps",
                principalColumn: "jobStepID");
        }
    }
}
