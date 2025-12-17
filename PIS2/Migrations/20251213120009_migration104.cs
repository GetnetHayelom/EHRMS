using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration104 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobReqCosts_JobRequirements_jobRequiremenetID",
                table: "JobReqCosts");

            migrationBuilder.RenameColumn(
                name: "jobRequirementStatus",
                table: "JobReqCosts",
                newName: "jobReqCostReference");

            migrationBuilder.RenameColumn(
                name: "jobRequiremenetID",
                table: "JobReqCosts",
                newName: "jobRequirementID");

            migrationBuilder.RenameIndex(
                name: "IX_JobReqCosts_jobRequiremenetID",
                table: "JobReqCosts",
                newName: "IX_JobReqCosts_jobRequirementID");

            migrationBuilder.RenameColumn(
                name: "holidayRepition",
                table: "Holidays",
                newName: "holidayCycle");

            migrationBuilder.RenameColumn(
                name: "exprienceID",
                table: "Experiences",
                newName: "experienceID");

            migrationBuilder.AddColumn<DateTime>(
                name: "modifiedDate",
                table: "Terminations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "modifiedDate",
                table: "SubAccounts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "modifiedDate",
                table: "Shifts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "modifiedDate",
                table: "Penalties",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "modifiedDate",
                table: "Overtimes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "modifiedDate",
                table: "LeaveTypes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "modifiedDate",
                table: "Jobs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<DateTime>(
                name: "modifiedDate",
                table: "JobRequirements",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "modifiedDate",
                table: "JobGrades",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "modifiedDate",
                table: "JobClasses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "modifiedDate",
                table: "JobCategories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "modifiedDate",
                table: "Holidays",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "modifiedDate",
                table: "EmploymentTypes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "modifiedDate",
                table: "EducationLevels",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "modifiedDate",
                table: "EarningTypes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "modifiedDate",
                table: "DeductionTypes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "modifiedDate",
                table: "Companies",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "modifiedDate",
                table: "BusinessUnits",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "modifiedDate",
                table: "Breaks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "modifiedDate",
                table: "BankInfos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AuditLogs",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "allowanceAmount",
                table: "Allowances",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(8,2)");

            migrationBuilder.AddColumn<DateTime>(
                name: "modifiedDate",
                table: "Allowances",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "modifiedDate",
                table: "Addresses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_JobReqCosts_JobRequirements_jobRequirementID",
                table: "JobReqCosts",
                column: "jobRequirementID",
                principalTable: "JobRequirements",
                principalColumn: "jobRequirementID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobReqCosts_JobRequirements_jobRequirementID",
                table: "JobReqCosts");

            migrationBuilder.DropColumn(
                name: "modifiedDate",
                table: "Terminations");

            migrationBuilder.DropColumn(
                name: "modifiedDate",
                table: "SubAccounts");

            migrationBuilder.DropColumn(
                name: "modifiedDate",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "modifiedDate",
                table: "Penalties");

            migrationBuilder.DropColumn(
                name: "modifiedDate",
                table: "Overtimes");

            migrationBuilder.DropColumn(
                name: "modifiedDate",
                table: "LeaveTypes");

            migrationBuilder.DropColumn(
                name: "modifiedDate",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "modifiedDate",
                table: "JobGrades");

            migrationBuilder.DropColumn(
                name: "modifiedDate",
                table: "JobClasses");

            migrationBuilder.DropColumn(
                name: "modifiedDate",
                table: "JobCategories");

            migrationBuilder.DropColumn(
                name: "modifiedDate",
                table: "Holidays");

            migrationBuilder.DropColumn(
                name: "modifiedDate",
                table: "EmploymentTypes");

            migrationBuilder.DropColumn(
                name: "modifiedDate",
                table: "EducationLevels");

            migrationBuilder.DropColumn(
                name: "modifiedDate",
                table: "EarningTypes");

            migrationBuilder.DropColumn(
                name: "modifiedDate",
                table: "DeductionTypes");

            migrationBuilder.DropColumn(
                name: "modifiedDate",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "modifiedDate",
                table: "BusinessUnits");

            migrationBuilder.DropColumn(
                name: "modifiedDate",
                table: "Breaks");

            migrationBuilder.DropColumn(
                name: "modifiedDate",
                table: "BankInfos");

            migrationBuilder.DropColumn(
                name: "modifiedDate",
                table: "Allowances");

            migrationBuilder.DropColumn(
                name: "modifiedDate",
                table: "Addresses");

            migrationBuilder.RenameColumn(
                name: "jobRequirementID",
                table: "JobReqCosts",
                newName: "jobRequiremenetID");

            migrationBuilder.RenameColumn(
                name: "jobReqCostReference",
                table: "JobReqCosts",
                newName: "jobRequirementStatus");

            migrationBuilder.RenameIndex(
                name: "IX_JobReqCosts_jobRequirementID",
                table: "JobReqCosts",
                newName: "IX_JobReqCosts_jobRequiremenetID");

            migrationBuilder.RenameColumn(
                name: "holidayCycle",
                table: "Holidays",
                newName: "holidayRepition");

            migrationBuilder.RenameColumn(
                name: "experienceID",
                table: "Experiences",
                newName: "exprienceID");

            migrationBuilder.AlterColumn<string>(
                name: "modifiedDate",
                table: "JobRequirements",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "ModifiedDate",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<decimal>(
                name: "allowanceAmount",
                table: "Allowances",
                type: "decimal(8,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddForeignKey(
                name: "FK_JobReqCosts_JobRequirements_jobRequiremenetID",
                table: "JobReqCosts",
                column: "jobRequiremenetID",
                principalTable: "JobRequirements",
                principalColumn: "jobRequirementID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
