using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration107 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobReqCosts_JobRequirements_jobRequirementID",
                table: "JobReqCosts");

            migrationBuilder.DropIndex(
                name: "IX_JobReqCosts_jobRequirementID",
                table: "JobReqCosts");

            migrationBuilder.RenameColumn(
                name: "jobRequirementID",
                table: "JobReqCosts",
                newName: "VacancyID");

            migrationBuilder.AlterColumn<int>(
                name: "maxLateOut",
                table: "Shifts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "maxEarlyOut",
                table: "Shifts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "VacancyModelVacancyID",
                table: "JobReqCosts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "maxLeaveIncrementManagement",
                table: "EmploymentTypes",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "timeBound",
                table: "EmploymentTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Vacancies",
                columns: table => new
                {
                    VacancyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VacancyTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    jobID = table.Column<int>(type: "int", nullable: false),
                    jobModeljobID = table.Column<int>(type: "int", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    departmentID = table.Column<int>(type: "int", nullable: false),
                    departmentModeldepartmentID = table.Column<int>(type: "int", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VacancyRequiredNumber = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DatePosted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClosingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    jobRequirementID = table.Column<int>(type: "int", nullable: true),
                    jobRequirmentmodeljobRequirementID = table.Column<int>(type: "int", nullable: true),
                    employmentMethod = table.Column<int>(type: "int", nullable: false),
                    employmentMethodModelemploymentMethodID = table.Column<int>(type: "int", nullable: true),
                    employmentTypeID = table.Column<int>(type: "int", nullable: false),
                    employmentTypeModelemploymentTypeID = table.Column<int>(type: "int", nullable: true),
                    VacancyType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vacancies", x => x.VacancyID);
                    table.ForeignKey(
                        name: "FK_Vacancies_Departments_departmentModeldepartmentID",
                        column: x => x.departmentModeldepartmentID,
                        principalTable: "Departments",
                        principalColumn: "departmentID");
                    table.ForeignKey(
                        name: "FK_Vacancies_EmploymentMethods_employmentMethodModelemploymentMethodID",
                        column: x => x.employmentMethodModelemploymentMethodID,
                        principalTable: "EmploymentMethods",
                        principalColumn: "employmentMethodID");
                    table.ForeignKey(
                        name: "FK_Vacancies_EmploymentTypes_employmentTypeModelemploymentTypeID",
                        column: x => x.employmentTypeModelemploymentTypeID,
                        principalTable: "EmploymentTypes",
                        principalColumn: "employmentTypeID");
                    table.ForeignKey(
                        name: "FK_Vacancies_JobRequirements_jobRequirmentmodeljobRequirementID",
                        column: x => x.jobRequirmentmodeljobRequirementID,
                        principalTable: "JobRequirements",
                        principalColumn: "jobRequirementID");
                    table.ForeignKey(
                        name: "FK_Vacancies_Jobs_jobModeljobID",
                        column: x => x.jobModeljobID,
                        principalTable: "Jobs",
                        principalColumn: "jobID");
                });

            migrationBuilder.CreateTable(
                name: "Applicants",
                columns: table => new
                {
                    ApplicantID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VacancyID = table.Column<int>(type: "int", nullable: false),
                    VacancyModelVacancyID = table.Column<int>(type: "int", nullable: true),
                    personID = table.Column<int>(type: "int", nullable: false),
                    personModelpersonID = table.Column<int>(type: "int", nullable: true),
                    ResumeFilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppliedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applicants", x => x.ApplicantID);
                    table.ForeignKey(
                        name: "FK_Applicants_Persons_personModelpersonID",
                        column: x => x.personModelpersonID,
                        principalTable: "Persons",
                        principalColumn: "personID");
                    table.ForeignKey(
                        name: "FK_Applicants_Vacancies_VacancyModelVacancyID",
                        column: x => x.VacancyModelVacancyID,
                        principalTable: "Vacancies",
                        principalColumn: "VacancyID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobReqCosts_VacancyModelVacancyID",
                table: "JobReqCosts",
                column: "VacancyModelVacancyID");

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_personModelpersonID",
                table: "Applicants",
                column: "personModelpersonID");

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_VacancyModelVacancyID",
                table: "Applicants",
                column: "VacancyModelVacancyID");

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_departmentModeldepartmentID",
                table: "Vacancies",
                column: "departmentModeldepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_employmentMethodModelemploymentMethodID",
                table: "Vacancies",
                column: "employmentMethodModelemploymentMethodID");

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_employmentTypeModelemploymentTypeID",
                table: "Vacancies",
                column: "employmentTypeModelemploymentTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_jobModeljobID",
                table: "Vacancies",
                column: "jobModeljobID");

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_jobRequirmentmodeljobRequirementID",
                table: "Vacancies",
                column: "jobRequirmentmodeljobRequirementID");

            migrationBuilder.AddForeignKey(
                name: "FK_JobReqCosts_Vacancies_VacancyModelVacancyID",
                table: "JobReqCosts",
                column: "VacancyModelVacancyID",
                principalTable: "Vacancies",
                principalColumn: "VacancyID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobReqCosts_Vacancies_VacancyModelVacancyID",
                table: "JobReqCosts");

            migrationBuilder.DropTable(
                name: "Applicants");

            migrationBuilder.DropTable(
                name: "Vacancies");

            migrationBuilder.DropIndex(
                name: "IX_JobReqCosts_VacancyModelVacancyID",
                table: "JobReqCosts");

            migrationBuilder.DropColumn(
                name: "VacancyModelVacancyID",
                table: "JobReqCosts");

            migrationBuilder.DropColumn(
                name: "maxLeaveIncrementManagement",
                table: "EmploymentTypes");

            migrationBuilder.DropColumn(
                name: "timeBound",
                table: "EmploymentTypes");

            migrationBuilder.RenameColumn(
                name: "VacancyID",
                table: "JobReqCosts",
                newName: "jobRequirementID");

            migrationBuilder.AlterColumn<int>(
                name: "maxLateOut",
                table: "Shifts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "maxEarlyOut",
                table: "Shifts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobReqCosts_jobRequirementID",
                table: "JobReqCosts",
                column: "jobRequirementID");

            migrationBuilder.AddForeignKey(
                name: "FK_JobReqCosts_JobRequirements_jobRequirementID",
                table: "JobReqCosts",
                column: "jobRequirementID",
                principalTable: "JobRequirements",
                principalColumn: "jobRequirementID");
        }
    }
}
