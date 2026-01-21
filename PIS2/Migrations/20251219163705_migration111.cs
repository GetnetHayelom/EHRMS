using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration111 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applicants_Persons_personModelpersonID",
                table: "Applicants");

            migrationBuilder.DropForeignKey(
                name: "FK_Applicants_Vacancies_VacancyModelVacancyID",
                table: "Applicants");

            migrationBuilder.DropForeignKey(
                name: "FK_JobReqCosts_Vacancies_VacancyModelVacancyID",
                table: "JobReqCosts");

            migrationBuilder.DropForeignKey(
                name: "FK_Vacancies_EmploymentMethods_employmentMethodModelemploymentMethodID",
                table: "Vacancies");

            migrationBuilder.DropForeignKey(
                name: "FK_Vacancies_EmploymentTypes_employmentTypeModelemploymentTypeID",
                table: "Vacancies");

            migrationBuilder.DropForeignKey(
                name: "FK_Vacancies_JobRequirements_jobRequirmentmodeljobRequirementID",
                table: "Vacancies");

            migrationBuilder.DropIndex(
                name: "IX_Vacancies_employmentMethodModelemploymentMethodID",
                table: "Vacancies");

            migrationBuilder.DropIndex(
                name: "IX_Vacancies_employmentTypeModelemploymentTypeID",
                table: "Vacancies");

            migrationBuilder.DropIndex(
                name: "IX_Vacancies_jobRequirmentmodeljobRequirementID",
                table: "Vacancies");

            migrationBuilder.DropIndex(
                name: "IX_JobReqCosts_VacancyModelVacancyID",
                table: "JobReqCosts");

            migrationBuilder.DropIndex(
                name: "IX_Applicants_personModelpersonID",
                table: "Applicants");

            migrationBuilder.DropIndex(
                name: "IX_Applicants_VacancyModelVacancyID",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "employmentMethodModelemploymentMethodID",
                table: "Vacancies");

            migrationBuilder.DropColumn(
                name: "employmentTypeModelemploymentTypeID",
                table: "Vacancies");

            migrationBuilder.DropColumn(
                name: "jobRequirmentmodeljobRequirementID",
                table: "Vacancies");

            migrationBuilder.DropColumn(
                name: "VacancyModelVacancyID",
                table: "JobReqCosts");

            migrationBuilder.DropColumn(
                name: "VacancyModelVacancyID",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "personModelpersonID",
                table: "Applicants");

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_employmentMethodID",
                table: "Vacancies",
                column: "employmentMethodID");

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_employmentTypeID",
                table: "Vacancies",
                column: "employmentTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_jobRequirementID",
                table: "Vacancies",
                column: "jobRequirementID");

            migrationBuilder.CreateIndex(
                name: "IX_JobReqCosts_VacancyID",
                table: "JobReqCosts",
                column: "VacancyID");

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_personID",
                table: "Applicants",
                column: "personID");

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_VacancyID",
                table: "Applicants",
                column: "VacancyID");

            migrationBuilder.AddForeignKey(
                name: "FK_Applicants_Persons_personID",
                table: "Applicants",
                column: "personID",
                principalTable: "Persons",
                principalColumn: "personID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Applicants_Vacancies_VacancyID",
                table: "Applicants",
                column: "VacancyID",
                principalTable: "Vacancies",
                principalColumn: "VacancyID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobReqCosts_Vacancies_VacancyID",
                table: "JobReqCosts",
                column: "VacancyID",
                principalTable: "Vacancies",
                principalColumn: "VacancyID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vacancies_EmploymentMethods_employmentMethodID",
                table: "Vacancies",
                column: "employmentMethodID",
                principalTable: "EmploymentMethods",
                principalColumn: "employmentMethodID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vacancies_EmploymentTypes_employmentTypeID",
                table: "Vacancies",
                column: "employmentTypeID",
                principalTable: "EmploymentTypes",
                principalColumn: "employmentTypeID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vacancies_JobRequirements_jobRequirementID",
                table: "Vacancies",
                column: "jobRequirementID",
                principalTable: "JobRequirements",
                principalColumn: "jobRequirementID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applicants_Persons_personID",
                table: "Applicants");

            migrationBuilder.DropForeignKey(
                name: "FK_Applicants_Vacancies_VacancyID",
                table: "Applicants");

            migrationBuilder.DropForeignKey(
                name: "FK_JobReqCosts_Vacancies_VacancyID",
                table: "JobReqCosts");

            migrationBuilder.DropForeignKey(
                name: "FK_Vacancies_EmploymentMethods_employmentMethodID",
                table: "Vacancies");

            migrationBuilder.DropForeignKey(
                name: "FK_Vacancies_EmploymentTypes_employmentTypeID",
                table: "Vacancies");

            migrationBuilder.DropForeignKey(
                name: "FK_Vacancies_JobRequirements_jobRequirementID",
                table: "Vacancies");

            migrationBuilder.DropIndex(
                name: "IX_Vacancies_employmentMethodID",
                table: "Vacancies");

            migrationBuilder.DropIndex(
                name: "IX_Vacancies_employmentTypeID",
                table: "Vacancies");

            migrationBuilder.DropIndex(
                name: "IX_Vacancies_jobRequirementID",
                table: "Vacancies");

            migrationBuilder.DropIndex(
                name: "IX_JobReqCosts_VacancyID",
                table: "JobReqCosts");

            migrationBuilder.DropIndex(
                name: "IX_Applicants_personID",
                table: "Applicants");

            migrationBuilder.DropIndex(
                name: "IX_Applicants_VacancyID",
                table: "Applicants");

            migrationBuilder.AddColumn<int>(
                name: "employmentMethodModelemploymentMethodID",
                table: "Vacancies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "employmentTypeModelemploymentTypeID",
                table: "Vacancies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "jobRequirmentmodeljobRequirementID",
                table: "Vacancies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VacancyModelVacancyID",
                table: "JobReqCosts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VacancyModelVacancyID",
                table: "Applicants",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "personModelpersonID",
                table: "Applicants",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_employmentMethodModelemploymentMethodID",
                table: "Vacancies",
                column: "employmentMethodModelemploymentMethodID");

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_employmentTypeModelemploymentTypeID",
                table: "Vacancies",
                column: "employmentTypeModelemploymentTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_jobRequirmentmodeljobRequirementID",
                table: "Vacancies",
                column: "jobRequirmentmodeljobRequirementID");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Applicants_Persons_personModelpersonID",
                table: "Applicants",
                column: "personModelpersonID",
                principalTable: "Persons",
                principalColumn: "personID");

            migrationBuilder.AddForeignKey(
                name: "FK_Applicants_Vacancies_VacancyModelVacancyID",
                table: "Applicants",
                column: "VacancyModelVacancyID",
                principalTable: "Vacancies",
                principalColumn: "VacancyID");

            migrationBuilder.AddForeignKey(
                name: "FK_JobReqCosts_Vacancies_VacancyModelVacancyID",
                table: "JobReqCosts",
                column: "VacancyModelVacancyID",
                principalTable: "Vacancies",
                principalColumn: "VacancyID");

            migrationBuilder.AddForeignKey(
                name: "FK_Vacancies_EmploymentMethods_employmentMethodModelemploymentMethodID",
                table: "Vacancies",
                column: "employmentMethodModelemploymentMethodID",
                principalTable: "EmploymentMethods",
                principalColumn: "employmentMethodID");

            migrationBuilder.AddForeignKey(
                name: "FK_Vacancies_EmploymentTypes_employmentTypeModelemploymentTypeID",
                table: "Vacancies",
                column: "employmentTypeModelemploymentTypeID",
                principalTable: "EmploymentTypes",
                principalColumn: "employmentTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Vacancies_JobRequirements_jobRequirmentmodeljobRequirementID",
                table: "Vacancies",
                column: "jobRequirmentmodeljobRequirementID",
                principalTable: "JobRequirements",
                principalColumn: "jobRequirementID");
        }
    }
}
