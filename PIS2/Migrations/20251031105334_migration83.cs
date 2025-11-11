using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration83 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobPlacements_Employments_employmentID",
                table: "JobPlacements");

            migrationBuilder.AddForeignKey(
                name: "FK_JobPlacements_Employments_employmentID",
                table: "JobPlacements",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobPlacements_Employments_employmentID",
                table: "JobPlacements");

            migrationBuilder.AddForeignKey(
                name: "FK_JobPlacements_Employments_employmentID",
                table: "JobPlacements",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID");
        }
    }
}
