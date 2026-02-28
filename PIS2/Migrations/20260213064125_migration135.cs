using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration135 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employments_EmploymentRequests_employmentRequestID",
                table: "Employments");

            migrationBuilder.DropTable(
                name: "EmploymentRequestHistories");

            migrationBuilder.DropTable(
                name: "EmploymentRequests");

            migrationBuilder.RenameColumn(
                name: "employmentRequestID",
                table: "Employments",
                newName: "jobRequirementID");

            migrationBuilder.RenameIndex(
                name: "IX_Employments_employmentRequestID",
                table: "Employments",
                newName: "IX_Employments_jobRequirementID");

            migrationBuilder.AddColumn<int>(
                name: "VacancyStage",
                table: "Vacancies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "educationLevelMark",
                table: "PersonEducationLevels",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "employmentTypeID",
                table: "JobRequirements",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "JobModeljobID",
                table: "JobRequirementHistories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "jobID",
                table: "JobRequirementHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_JobRequirements_employmentTypeID",
                table: "JobRequirements",
                column: "employmentTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_JobRequirementHistories_JobModeljobID",
                table: "JobRequirementHistories",
                column: "JobModeljobID");

            migrationBuilder.AddForeignKey(
                name: "FK_Employments_JobRequirements_jobRequirementID",
                table: "Employments",
                column: "jobRequirementID",
                principalTable: "JobRequirements",
                principalColumn: "jobRequirementID");

            migrationBuilder.AddForeignKey(
                name: "FK_JobRequirementHistories_Jobs_JobModeljobID",
                table: "JobRequirementHistories",
                column: "JobModeljobID",
                principalTable: "Jobs",
                principalColumn: "jobID");

            migrationBuilder.AddForeignKey(
                name: "FK_JobRequirements_EmploymentTypes_employmentTypeID",
                table: "JobRequirements",
                column: "employmentTypeID",
                principalTable: "EmploymentTypes",
                principalColumn: "employmentTypeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employments_JobRequirements_jobRequirementID",
                table: "Employments");

            migrationBuilder.DropForeignKey(
                name: "FK_JobRequirementHistories_Jobs_JobModeljobID",
                table: "JobRequirementHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_JobRequirements_EmploymentTypes_employmentTypeID",
                table: "JobRequirements");

            migrationBuilder.DropIndex(
                name: "IX_JobRequirements_employmentTypeID",
                table: "JobRequirements");

            migrationBuilder.DropIndex(
                name: "IX_JobRequirementHistories_JobModeljobID",
                table: "JobRequirementHistories");

            migrationBuilder.DropColumn(
                name: "VacancyStage",
                table: "Vacancies");

            migrationBuilder.DropColumn(
                name: "employmentTypeID",
                table: "JobRequirements");

            migrationBuilder.DropColumn(
                name: "JobModeljobID",
                table: "JobRequirementHistories");

            migrationBuilder.DropColumn(
                name: "jobID",
                table: "JobRequirementHistories");

            migrationBuilder.RenameColumn(
                name: "jobRequirementID",
                table: "Employments",
                newName: "employmentRequestID");

            migrationBuilder.RenameIndex(
                name: "IX_Employments_jobRequirementID",
                table: "Employments",
                newName: "IX_Employments_employmentRequestID");

            migrationBuilder.AlterColumn<string>(
                name: "educationLevelMark",
                table: "PersonEducationLevels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "EmploymentRequests",
                columns: table => new
                {
                    employmentRequestID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employmentTypeID = table.Column<int>(type: "int", nullable: false),
                    jobID = table.Column<int>(type: "int", nullable: false),
                    employmentModelemploymentID = table.Column<int>(type: "int", nullable: true),
                    employmentRequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    requestStatus = table.Column<int>(type: "int", nullable: false),
                    requiredNo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmploymentRequests", x => x.employmentRequestID);
                    table.ForeignKey(
                        name: "FK_EmploymentRequests_EmploymentTypes_employmentTypeID",
                        column: x => x.employmentTypeID,
                        principalTable: "EmploymentTypes",
                        principalColumn: "employmentTypeID");
                    table.ForeignKey(
                        name: "FK_EmploymentRequests_Employments_employmentModelemploymentID",
                        column: x => x.employmentModelemploymentID,
                        principalTable: "Employments",
                        principalColumn: "employmentID");
                    table.ForeignKey(
                        name: "FK_EmploymentRequests_Jobs_jobID",
                        column: x => x.jobID,
                        principalTable: "Jobs",
                        principalColumn: "jobID");
                });

            migrationBuilder.CreateTable(
                name: "EmploymentRequestHistories",
                columns: table => new
                {
                    employmentRequestHistoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employmentRequestID = table.Column<int>(type: "int", nullable: false),
                    employmentTypeModelemploymentTypeID = table.Column<int>(type: "int", nullable: true),
                    jobModeljobID = table.Column<int>(type: "int", nullable: true),
                    employmentType = table.Column<int>(type: "int", nullable: false),
                    jobID = table.Column<int>(type: "int", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    modifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    requestStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmploymentRequestHistories", x => x.employmentRequestHistoryID);
                    table.ForeignKey(
                        name: "FK_EmploymentRequestHistories_EmploymentRequests_employmentRequestID",
                        column: x => x.employmentRequestID,
                        principalTable: "EmploymentRequests",
                        principalColumn: "employmentRequestID");
                    table.ForeignKey(
                        name: "FK_EmploymentRequestHistories_EmploymentTypes_employmentTypeModelemploymentTypeID",
                        column: x => x.employmentTypeModelemploymentTypeID,
                        principalTable: "EmploymentTypes",
                        principalColumn: "employmentTypeID");
                    table.ForeignKey(
                        name: "FK_EmploymentRequestHistories_Jobs_jobModeljobID",
                        column: x => x.jobModeljobID,
                        principalTable: "Jobs",
                        principalColumn: "jobID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentRequestHistories_employmentRequestID",
                table: "EmploymentRequestHistories",
                column: "employmentRequestID");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentRequestHistories_employmentTypeModelemploymentTypeID",
                table: "EmploymentRequestHistories",
                column: "employmentTypeModelemploymentTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentRequestHistories_jobModeljobID",
                table: "EmploymentRequestHistories",
                column: "jobModeljobID");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentRequests_employmentModelemploymentID",
                table: "EmploymentRequests",
                column: "employmentModelemploymentID");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentRequests_employmentTypeID",
                table: "EmploymentRequests",
                column: "employmentTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentRequests_jobID",
                table: "EmploymentRequests",
                column: "jobID");

            migrationBuilder.AddForeignKey(
                name: "FK_Employments_EmploymentRequests_employmentRequestID",
                table: "Employments",
                column: "employmentRequestID",
                principalTable: "EmploymentRequests",
                principalColumn: "employmentRequestID");
        }
    }
}
