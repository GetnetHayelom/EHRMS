using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration147 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vacancies_Departments_departmentID",
                table: "Vacancies");

            migrationBuilder.AlterColumn<int>(
                name: "departmentID",
                table: "Vacancies",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "letterSender",
                table: "Letters",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "letterReceiver",
                table: "Letters",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "NextJobGradeID",
                table: "JobGrades",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PreviousJobGradeID",
                table: "JobGrades",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobGrades_NextJobGradeID",
                table: "JobGrades",
                column: "NextJobGradeID",
                unique: true,
                filter: "[NextJobGradeID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_JobGrades_PreviousJobGradeID",
                table: "JobGrades",
                column: "PreviousJobGradeID",
                unique: true,
                filter: "[PreviousJobGradeID] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_JobGrades_JobGrades_NextJobGradeID",
                table: "JobGrades",
                column: "NextJobGradeID",
                principalTable: "JobGrades",
                principalColumn: "jobGradeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobGrades_JobGrades_PreviousJobGradeID",
                table: "JobGrades",
                column: "PreviousJobGradeID",
                principalTable: "JobGrades",
                principalColumn: "jobGradeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vacancies_Departments_departmentID",
                table: "Vacancies",
                column: "departmentID",
                principalTable: "Departments",
                principalColumn: "departmentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobGrades_JobGrades_NextJobGradeID",
                table: "JobGrades");

            migrationBuilder.DropForeignKey(
                name: "FK_JobGrades_JobGrades_PreviousJobGradeID",
                table: "JobGrades");

            migrationBuilder.DropForeignKey(
                name: "FK_Vacancies_Departments_departmentID",
                table: "Vacancies");

            migrationBuilder.DropIndex(
                name: "IX_JobGrades_NextJobGradeID",
                table: "JobGrades");

            migrationBuilder.DropIndex(
                name: "IX_JobGrades_PreviousJobGradeID",
                table: "JobGrades");

            migrationBuilder.DropColumn(
                name: "NextJobGradeID",
                table: "JobGrades");

            migrationBuilder.DropColumn(
                name: "PreviousJobGradeID",
                table: "JobGrades");

            migrationBuilder.AlterColumn<int>(
                name: "departmentID",
                table: "Vacancies",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "letterSender",
                table: "Letters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "letterReceiver",
                table: "Letters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Vacancies_Departments_departmentID",
                table: "Vacancies",
                column: "departmentID",
                principalTable: "Departments",
                principalColumn: "departmentID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
