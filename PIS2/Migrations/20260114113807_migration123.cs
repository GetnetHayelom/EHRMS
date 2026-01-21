using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration123 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "evaluationValuation",
                table: "EvaluationValuations",
                newName: "evaluationValuationID");

            migrationBuilder.AddColumn<int>(
                name: "jobPlacementID",
                table: "Evaluations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_jobPlacementID",
                table: "Evaluations",
                column: "jobPlacementID");

            migrationBuilder.AddForeignKey(
                name: "FK_Evaluations_JobPlacements_jobPlacementID",
                table: "Evaluations",
                column: "jobPlacementID",
                principalTable: "JobPlacements",
                principalColumn: "jobPlacementID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Evaluations_JobPlacements_jobPlacementID",
                table: "Evaluations");

            migrationBuilder.DropIndex(
                name: "IX_Evaluations_jobPlacementID",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "jobPlacementID",
                table: "Evaluations");

            migrationBuilder.RenameColumn(
                name: "evaluationValuationID",
                table: "EvaluationValuations",
                newName: "evaluationValuation");
        }
    }
}
