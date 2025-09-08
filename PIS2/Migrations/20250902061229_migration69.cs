using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration69 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Structures_Departments_departmentModeldepartmentID",
                table: "Structures");

            migrationBuilder.DropForeignKey(
                name: "FK_Structures_Jobs_jobModeljobID",
                table: "Structures");

            migrationBuilder.DropIndex(
                name: "IX_Structures_departmentModeldepartmentID",
                table: "Structures");

            migrationBuilder.DropIndex(
                name: "IX_Structures_jobModeljobID",
                table: "Structures");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_jobCode",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "departmentModeldepartmentID",
                table: "Structures");

            migrationBuilder.DropColumn(
                name: "jobModeljobID",
                table: "Structures");

            migrationBuilder.AlterColumn<string>(
                name: "jobCode",
                table: "Jobs",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Structures_departmentID",
                table: "Structures",
                column: "departmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Structures_jobID",
                table: "Structures",
                column: "jobID");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_jobCode",
                table: "Jobs",
                column: "jobCode",
                unique: true,
                filter: "[jobCode] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Structures_Departments_departmentID",
                table: "Structures",
                column: "departmentID",
                principalTable: "Departments",
                principalColumn: "departmentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Structures_Jobs_jobID",
                table: "Structures",
                column: "jobID",
                principalTable: "Jobs",
                principalColumn: "jobID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Structures_Departments_departmentID",
                table: "Structures");

            migrationBuilder.DropForeignKey(
                name: "FK_Structures_Jobs_jobID",
                table: "Structures");

            migrationBuilder.DropIndex(
                name: "IX_Structures_departmentID",
                table: "Structures");

            migrationBuilder.DropIndex(
                name: "IX_Structures_jobID",
                table: "Structures");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_jobCode",
                table: "Jobs");

            migrationBuilder.AddColumn<int>(
                name: "departmentModeldepartmentID",
                table: "Structures",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "jobModeljobID",
                table: "Structures",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "jobCode",
                table: "Jobs",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Structures_departmentModeldepartmentID",
                table: "Structures",
                column: "departmentModeldepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Structures_jobModeljobID",
                table: "Structures",
                column: "jobModeljobID");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_jobCode",
                table: "Jobs",
                column: "jobCode",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Structures_Departments_departmentModeldepartmentID",
                table: "Structures",
                column: "departmentModeldepartmentID",
                principalTable: "Departments",
                principalColumn: "departmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Structures_Jobs_jobModeljobID",
                table: "Structures",
                column: "jobModeljobID",
                principalTable: "Jobs",
                principalColumn: "jobID");
        }
    }
}
