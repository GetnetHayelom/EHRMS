using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration161 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "departmentID",
                table: "PayrollPays",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "jobPlacementID",
                table: "PayrollPays",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PayrollPays_departmentID",
                table: "PayrollPays",
                column: "departmentID");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollPays_jobPlacementID",
                table: "PayrollPays",
                column: "jobPlacementID");

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollPays_Departments_departmentID",
                table: "PayrollPays",
                column: "departmentID",
                principalTable: "Departments",
                principalColumn: "departmentID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollPays_JobPlacements_jobPlacementID",
                table: "PayrollPays",
                column: "jobPlacementID",
                principalTable: "JobPlacements",
                principalColumn: "jobPlacementID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PayrollPays_Departments_departmentID",
                table: "PayrollPays");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollPays_JobPlacements_jobPlacementID",
                table: "PayrollPays");

            migrationBuilder.DropIndex(
                name: "IX_PayrollPays_departmentID",
                table: "PayrollPays");

            migrationBuilder.DropIndex(
                name: "IX_PayrollPays_jobPlacementID",
                table: "PayrollPays");

            migrationBuilder.DropColumn(
                name: "departmentID",
                table: "PayrollPays");

            migrationBuilder.DropColumn(
                name: "jobPlacementID",
                table: "PayrollPays");
        }
    }
}
