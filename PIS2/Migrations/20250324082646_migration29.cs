using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration29 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "departmentID",
                table: "OvertimeRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OvertimeRecords_departmentID",
                table: "OvertimeRecords",
                column: "departmentID");

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
                name: "FK_OvertimeRecords_Departments_departmentID",
                table: "OvertimeRecords");

            migrationBuilder.DropIndex(
                name: "IX_OvertimeRecords_departmentID",
                table: "OvertimeRecords");

            migrationBuilder.DropColumn(
                name: "departmentID",
                table: "OvertimeRecords");
        }
    }
}
