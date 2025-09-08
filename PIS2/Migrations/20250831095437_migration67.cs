using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration67 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "penaltyMethod",
                table: "PenaltyTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "penaltyRate",
                table: "PenaltyTypes",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "penaltyAmount",
                table: "Penalties",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "penaltyEndDate",
                table: "Penalties",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "penaltyReference",
                table: "Penalties",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "penaltyStartDate",
                table: "Penalties",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "modifiedDate",
                table: "JobRequirements",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "approvedNumber",
                table: "JobRequirements",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "hiredNumber",
                table: "JobRequirements",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DepartmentHistories",
                columns: table => new
                {
                    departmentHistoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    departmentID = table.Column<int>(type: "int", nullable: false),
                    departmentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    departmentStatus = table.Column<int>(type: "int", nullable: false),
                    employmentID = table.Column<int>(type: "int", nullable: false),
                    employmentModelemploymentID = table.Column<int>(type: "int", nullable: true),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    modifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentHistories", x => x.departmentHistoryID);
                    table.ForeignKey(
                        name: "FK_DepartmentHistories_Departments_departmentID",
                        column: x => x.departmentID,
                        principalTable: "Departments",
                        principalColumn: "departmentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DepartmentHistories_Employments_employmentModelemploymentID",
                        column: x => x.employmentModelemploymentID,
                        principalTable: "Employments",
                        principalColumn: "employmentID");
                });

            migrationBuilder.CreateTable(
                name: "jobReqCost",
                columns: table => new
                {
                    jobReqCostID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    jobRequiremenetID = table.Column<int>(type: "int", nullable: false),
                    jobReqStatus = table.Column<int>(type: "int", nullable: false),
                    jobReqCostReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    jobReqCostEstimate = table.Column<double>(type: "float", nullable: false),
                    jobReqCostActual = table.Column<double>(type: "float", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    modifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_jobReqCost", x => x.jobReqCostID);
                    table.ForeignKey(
                        name: "FK_jobReqCost_JobRequirements_jobRequiremenetID",
                        column: x => x.jobRequiremenetID,
                        principalTable: "JobRequirements",
                        principalColumn: "jobRequirementID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentHistories_departmentID",
                table: "DepartmentHistories",
                column: "departmentID");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentHistories_employmentModelemploymentID",
                table: "DepartmentHistories",
                column: "employmentModelemploymentID");

            migrationBuilder.CreateIndex(
                name: "IX_jobReqCost_jobRequiremenetID",
                table: "jobReqCost",
                column: "jobRequiremenetID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DepartmentHistories");

            migrationBuilder.DropTable(
                name: "jobReqCost");

            migrationBuilder.DropColumn(
                name: "penaltyMethod",
                table: "PenaltyTypes");

            migrationBuilder.DropColumn(
                name: "penaltyRate",
                table: "PenaltyTypes");

            migrationBuilder.DropColumn(
                name: "penaltyAmount",
                table: "Penalties");

            migrationBuilder.DropColumn(
                name: "penaltyEndDate",
                table: "Penalties");

            migrationBuilder.DropColumn(
                name: "penaltyReference",
                table: "Penalties");

            migrationBuilder.DropColumn(
                name: "penaltyStartDate",
                table: "Penalties");

            migrationBuilder.DropColumn(
                name: "approvedNumber",
                table: "JobRequirements");

            migrationBuilder.DropColumn(
                name: "hiredNumber",
                table: "JobRequirements");

            migrationBuilder.AlterColumn<DateTime>(
                name: "modifiedDate",
                table: "JobRequirements",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
