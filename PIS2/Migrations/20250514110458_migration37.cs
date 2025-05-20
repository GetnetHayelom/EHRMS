using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration37 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobPlacements_Departments_departmentID",
                table: "JobPlacements");

            migrationBuilder.DropForeignKey(
                name: "FK_OvertimeRecords_Departments_departmentID",
                table: "OvertimeRecords");

            migrationBuilder.AddForeignKey(
                name: "FK_JobPlacements_Departments_departmentID",
                table: "JobPlacements",
                column: "departmentID",
                principalTable: "Departments",
                principalColumn: "departmentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OvertimeRecords_Departments_departmentID",
                table: "OvertimeRecords",
                column: "departmentID",
                principalTable: "Departments",
                principalColumn: "departmentID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobPlacements_Departments_departmentID",
                table: "JobPlacements");

            migrationBuilder.DropForeignKey(
                name: "FK_OvertimeRecords_Departments_departmentID",
                table: "OvertimeRecords");

            migrationBuilder.AddForeignKey(
                name: "FK_JobPlacements_Departments_departmentID",
                table: "JobPlacements",
                column: "departmentID",
                principalTable: "Departments",
                principalColumn: "departmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_OvertimeRecords_Departments_departmentID",
                table: "OvertimeRecords",
                column: "departmentID",
                principalTable: "Departments",
                principalColumn: "departmentID");
        }
    }
}
