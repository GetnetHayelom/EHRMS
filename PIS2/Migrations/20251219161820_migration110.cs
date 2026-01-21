using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration110 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vacancies_Departments_departmentModeldepartmentID",
                table: "Vacancies");

            migrationBuilder.DropForeignKey(
                name: "FK_Vacancies_Jobs_jobModeljobID",
                table: "Vacancies");

            migrationBuilder.DropIndex(
                name: "IX_Vacancies_departmentModeldepartmentID",
                table: "Vacancies");

            migrationBuilder.DropIndex(
                name: "IX_Vacancies_jobModeljobID",
                table: "Vacancies");

            migrationBuilder.DropColumn(
                name: "departmentModeldepartmentID",
                table: "Vacancies");

            migrationBuilder.DropColumn(
                name: "jobModeljobID",
                table: "Vacancies");

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_departmentID",
                table: "Vacancies",
                column: "departmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_jobID",
                table: "Vacancies",
                column: "jobID");

            migrationBuilder.AddForeignKey(
                name: "FK_Vacancies_Departments_departmentID",
                table: "Vacancies",
                column: "departmentID",
                principalTable: "Departments",
                principalColumn: "departmentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vacancies_Jobs_jobID",
                table: "Vacancies",
                column: "jobID",
                principalTable: "Jobs",
                principalColumn: "jobID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vacancies_Departments_departmentID",
                table: "Vacancies");

            migrationBuilder.DropForeignKey(
                name: "FK_Vacancies_Jobs_jobID",
                table: "Vacancies");

            migrationBuilder.DropIndex(
                name: "IX_Vacancies_departmentID",
                table: "Vacancies");

            migrationBuilder.DropIndex(
                name: "IX_Vacancies_jobID",
                table: "Vacancies");

            migrationBuilder.AddColumn<int>(
                name: "departmentModeldepartmentID",
                table: "Vacancies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "jobModeljobID",
                table: "Vacancies",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_departmentModeldepartmentID",
                table: "Vacancies",
                column: "departmentModeldepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_jobModeljobID",
                table: "Vacancies",
                column: "jobModeljobID");

            migrationBuilder.AddForeignKey(
                name: "FK_Vacancies_Departments_departmentModeldepartmentID",
                table: "Vacancies",
                column: "departmentModeldepartmentID",
                principalTable: "Departments",
                principalColumn: "departmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Vacancies_Jobs_jobModeljobID",
                table: "Vacancies",
                column: "jobModeljobID",
                principalTable: "Jobs",
                principalColumn: "jobID");
        }
    }
}
