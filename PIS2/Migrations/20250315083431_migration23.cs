using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration23 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "workSiteID",
                table: "JobPlacements",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobPlacements_workSiteID",
                table: "JobPlacements",
                column: "workSiteID");

            migrationBuilder.AddForeignKey(
                name: "FK_JobPlacements_workSiteModel_workSiteID",
                table: "JobPlacements",
                column: "workSiteID",
                principalTable: "workSiteModel",
                principalColumn: "workSiteID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobPlacements_workSiteModel_workSiteID",
                table: "JobPlacements");

            migrationBuilder.DropIndex(
                name: "IX_JobPlacements_workSiteID",
                table: "JobPlacements");

            migrationBuilder.DropColumn(
                name: "workSiteID",
                table: "JobPlacements");
        }
    }
}
