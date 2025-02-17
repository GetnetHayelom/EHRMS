using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration18 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Leave_NumberOfDays",
                table: "Leaves");

            migrationBuilder.AddColumn<double>(
                name: "maxLeaveIncrement",
                table: "EmploymentTypes",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "employmentCarriedOverLeave",
                table: "Employments",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "employmentTerminationDate",
                table: "Employments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "employmentStatus",
                table: "EmploymentHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "givenID",
                table: "EmploymentHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Leave_NumberOfDays",
                table: "Leaves",
                sql: "leaveDays>0 AND leaveDays <= DATEDIFF(DAY, leaveStartDate, leaveEndDate)+1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Leave_NumberOfDays",
                table: "Leaves");

            migrationBuilder.DropColumn(
                name: "maxLeaveIncrement",
                table: "EmploymentTypes");

            migrationBuilder.DropColumn(
                name: "employmentCarriedOverLeave",
                table: "Employments");

            migrationBuilder.DropColumn(
                name: "employmentTerminationDate",
                table: "Employments");

            migrationBuilder.DropColumn(
                name: "employmentStatus",
                table: "EmploymentHistories");

            migrationBuilder.DropColumn(
                name: "givenID",
                table: "EmploymentHistories");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Leave_NumberOfDays",
                table: "Leaves",
                sql: "[leaveDays]>0 AND [leaveDays] <= DATEDIFF(DAY, [leaveStartDate], [leaveEndDate])+1");
        }
    }
}
