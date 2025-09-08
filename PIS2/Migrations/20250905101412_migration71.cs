using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration71 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_jobReqCost_JobRequirements_jobRequiremenetID",
                table: "jobReqCost");

            migrationBuilder.DropPrimaryKey(
                name: "PK_jobReqCost",
                table: "jobReqCost");

            migrationBuilder.RenameTable(
                name: "jobReqCost",
                newName: "JobReqCosts");

            migrationBuilder.RenameIndex(
                name: "IX_jobReqCost_jobRequiremenetID",
                table: "JobReqCosts",
                newName: "IX_JobReqCosts_jobRequiremenetID");

            migrationBuilder.AddColumn<double>(
                name: "jobRequirementStatus",
                table: "JobReqCosts",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobReqCosts",
                table: "JobReqCosts",
                column: "jobReqCostID");

            migrationBuilder.AddForeignKey(
                name: "FK_JobReqCosts_JobRequirements_jobRequiremenetID",
                table: "JobReqCosts",
                column: "jobRequiremenetID",
                principalTable: "JobRequirements",
                principalColumn: "jobRequirementID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobReqCosts_JobRequirements_jobRequiremenetID",
                table: "JobReqCosts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobReqCosts",
                table: "JobReqCosts");

            migrationBuilder.DropColumn(
                name: "jobRequirementStatus",
                table: "JobReqCosts");

            migrationBuilder.RenameTable(
                name: "JobReqCosts",
                newName: "jobReqCost");

            migrationBuilder.RenameIndex(
                name: "IX_JobReqCosts_jobRequiremenetID",
                table: "jobReqCost",
                newName: "IX_jobReqCost_jobRequiremenetID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_jobReqCost",
                table: "jobReqCost",
                column: "jobReqCostID");

            migrationBuilder.AddForeignKey(
                name: "FK_jobReqCost_JobRequirements_jobRequiremenetID",
                table: "jobReqCost",
                column: "jobRequiremenetID",
                principalTable: "JobRequirements",
                principalColumn: "jobRequirementID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
