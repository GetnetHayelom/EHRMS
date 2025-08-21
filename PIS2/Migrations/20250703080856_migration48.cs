using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration48 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JobPlacements_jobID_employeeID",
                table: "JobPlacements");

            migrationBuilder.DropColumn(
                name: "jobPlacementStep",
                table: "JobPlacements");

            migrationBuilder.AddColumn<int>(
                name: "jobStepID",
                table: "JobPlacements",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "jobStepModeljobStepID",
                table: "JobPlacements",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "delegations",
                columns: table => new
                {
                    delegationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    delegationFrom = table.Column<int>(type: "int", nullable: false),
                    delegationTo = table.Column<int>(type: "int", nullable: false),
                    delegationStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    delegationEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    delegationScope = table.Column<int>(type: "int", nullable: false),
                    delegationStatus = table.Column<int>(type: "int", nullable: false),
                    modifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_delegations", x => x.delegationID);
                    table.ForeignKey(
                        name: "FK_delegations_Employments_delegationFrom",
                        column: x => x.delegationFrom,
                        principalTable: "Employments",
                        principalColumn: "employmentID");
                    table.ForeignKey(
                        name: "FK_delegations_Employments_delegationTo",
                        column: x => x.delegationTo,
                        principalTable: "Employments",
                        principalColumn: "employmentID");
                });

            migrationBuilder.CreateTable(
                name: "JobSteps",
                columns: table => new
                {
                    jobStepID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    jobStepName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    jobGradeID = table.Column<int>(type: "int", nullable: false),
                    jobStepSalary = table.Column<double>(type: "float", nullable: false),
                    jobStepStatus = table.Column<int>(type: "int", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSteps", x => x.jobStepID);
                    table.ForeignKey(
                        name: "FK_JobSteps_JobGrades_jobGradeID",
                        column: x => x.jobGradeID,
                        principalTable: "JobGrades",
                        principalColumn: "jobGradeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShiftAssignments",
                columns: table => new
                {
                    shiftAssignmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employmentID = table.Column<int>(type: "int", nullable: false),
                    EmploymentModelemploymentID = table.Column<int>(type: "int", nullable: true),
                    shiftID = table.Column<int>(type: "int", nullable: false),
                    shiftModelshiftID = table.Column<int>(type: "int", nullable: true),
                    modifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftAssignments", x => x.shiftAssignmentID);
                    table.ForeignKey(
                        name: "FK_ShiftAssignments_Employments_EmploymentModelemploymentID",
                        column: x => x.EmploymentModelemploymentID,
                        principalTable: "Employments",
                        principalColumn: "employmentID");
                    table.ForeignKey(
                        name: "FK_ShiftAssignments_Shifts_shiftModelshiftID",
                        column: x => x.shiftModelshiftID,
                        principalTable: "Shifts",
                        principalColumn: "shiftID");
                });

            migrationBuilder.CreateTable(
                name: "SiteAssignments",
                columns: table => new
                {
                    siteAssignmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employmentID = table.Column<int>(type: "int", nullable: false),
                    employmentModelemploymentID = table.Column<int>(type: "int", nullable: true),
                    workSiteID = table.Column<int>(type: "int", nullable: false),
                    workSiteModelworkSiteID = table.Column<int>(type: "int", nullable: true),
                    modifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteAssignments", x => x.siteAssignmentID);
                    table.ForeignKey(
                        name: "FK_SiteAssignments_Employments_employmentModelemploymentID",
                        column: x => x.employmentModelemploymentID,
                        principalTable: "Employments",
                        principalColumn: "employmentID");
                    table.ForeignKey(
                        name: "FK_SiteAssignments_workSiteModel_workSiteModelworkSiteID",
                        column: x => x.workSiteModelworkSiteID,
                        principalTable: "workSiteModel",
                        principalColumn: "workSiteID");
                });

            migrationBuilder.CreateTable(
                name: "JobPlacementHistories",
                columns: table => new
                {
                    jobPlacementHistoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    jobPlacementID = table.Column<int>(type: "int", nullable: false),
                    jobStepID = table.Column<int>(type: "int", nullable: true),
                    jobPlacementReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    jobPlacementHistory = table.Column<double>(type: "float", nullable: false),
                    jobPlacementStatus = table.Column<int>(type: "int", nullable: false),
                    jobPlacementReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    modifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobPlacementHistories", x => x.jobPlacementHistoryID);
                    table.ForeignKey(
                        name: "FK_JobPlacementHistories_JobPlacements_jobPlacementID",
                        column: x => x.jobPlacementID,
                        principalTable: "JobPlacements",
                        principalColumn: "jobPlacementID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobPlacementHistories_JobSteps_jobStepID",
                        column: x => x.jobStepID,
                        principalTable: "JobSteps",
                        principalColumn: "jobStepID");
                });

            migrationBuilder.CreateTable(
                name: "JobStepHistories",
                columns: table => new
                {
                    jobStepHistoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    jobStepID = table.Column<int>(type: "int", nullable: false),
                    jobStepName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    jobStepSalary = table.Column<double>(type: "float", nullable: false),
                    jobStepStatus = table.Column<int>(type: "int", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    modifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobStepHistories", x => x.jobStepHistoryID);
                    table.ForeignKey(
                        name: "FK_JobStepHistories_JobSteps_jobStepID",
                        column: x => x.jobStepID,
                        principalTable: "JobSteps",
                        principalColumn: "jobStepID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobPlacements_jobStepModeljobStepID",
                table: "JobPlacements",
                column: "jobStepModeljobStepID");

            migrationBuilder.CreateIndex(
                name: "IX_delegations_delegationFrom",
                table: "delegations",
                column: "delegationFrom");

            migrationBuilder.CreateIndex(
                name: "IX_delegations_delegationTo",
                table: "delegations",
                column: "delegationTo");

            migrationBuilder.CreateIndex(
                name: "IX_JobPlacementHistories_jobPlacementID",
                table: "JobPlacementHistories",
                column: "jobPlacementID");

            migrationBuilder.CreateIndex(
                name: "IX_JobPlacementHistories_jobStepID",
                table: "JobPlacementHistories",
                column: "jobStepID");

            migrationBuilder.CreateIndex(
                name: "IX_JobStepHistories_jobStepID",
                table: "JobStepHistories",
                column: "jobStepID");

            migrationBuilder.CreateIndex(
                name: "IX_JobSteps_jobGradeID",
                table: "JobSteps",
                column: "jobGradeID");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftAssignments_EmploymentModelemploymentID",
                table: "ShiftAssignments",
                column: "EmploymentModelemploymentID");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftAssignments_shiftModelshiftID",
                table: "ShiftAssignments",
                column: "shiftModelshiftID");

            migrationBuilder.CreateIndex(
                name: "IX_SiteAssignments_employmentModelemploymentID",
                table: "SiteAssignments",
                column: "employmentModelemploymentID");

            migrationBuilder.CreateIndex(
                name: "IX_SiteAssignments_workSiteModelworkSiteID",
                table: "SiteAssignments",
                column: "workSiteModelworkSiteID");

            migrationBuilder.AddForeignKey(
                name: "FK_JobPlacements_JobSteps_jobStepModeljobStepID",
                table: "JobPlacements",
                column: "jobStepModeljobStepID",
                principalTable: "JobSteps",
                principalColumn: "jobStepID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobPlacements_JobSteps_jobStepModeljobStepID",
                table: "JobPlacements");

            migrationBuilder.DropTable(
                name: "delegations");

            migrationBuilder.DropTable(
                name: "JobPlacementHistories");

            migrationBuilder.DropTable(
                name: "JobStepHistories");

            migrationBuilder.DropTable(
                name: "ShiftAssignments");

            migrationBuilder.DropTable(
                name: "SiteAssignments");

            migrationBuilder.DropTable(
                name: "JobSteps");

            migrationBuilder.DropIndex(
                name: "IX_JobPlacements_jobStepModeljobStepID",
                table: "JobPlacements");

            migrationBuilder.DropColumn(
                name: "jobStepID",
                table: "JobPlacements");

            migrationBuilder.DropColumn(
                name: "jobStepModeljobStepID",
                table: "JobPlacements");

            migrationBuilder.AddColumn<string>(
                name: "jobPlacementStep",
                table: "JobPlacements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_JobPlacements_jobID_employeeID",
                table: "JobPlacements",
                columns: new[] { "jobID", "employeeID" },
                unique: true,
                filter: "[jobPlacementStatus]=1");
        }
    }
}
