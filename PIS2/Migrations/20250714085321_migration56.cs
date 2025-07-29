using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration56 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobPlacements_Employments_employeeID",
                table: "JobPlacements");

            migrationBuilder.RenameColumn(
                name: "employeeID",
                table: "JobPlacements",
                newName: "employmentID");

            migrationBuilder.RenameIndex(
                name: "IX_JobPlacements_employeeID",
                table: "JobPlacements",
                newName: "IX_JobPlacements_employmentID");

            migrationBuilder.RenameColumn(
                name: "exprienceEndDate",
                table: "Experiences",
                newName: "experienceEndDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "jobPlacementDate",
                table: "JobPlacementHistories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_JobPlacements_Employments_employmentID",
                table: "JobPlacements",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobPlacements_Employments_employmentID",
                table: "JobPlacements");

            migrationBuilder.DropColumn(
                name: "jobPlacementDate",
                table: "JobPlacementHistories");

            migrationBuilder.RenameColumn(
                name: "employmentID",
                table: "JobPlacements",
                newName: "employeeID");

            migrationBuilder.RenameIndex(
                name: "IX_JobPlacements_employmentID",
                table: "JobPlacements",
                newName: "IX_JobPlacements_employeeID");

            migrationBuilder.RenameColumn(
                name: "experienceEndDate",
                table: "Experiences",
                newName: "exprienceEndDate");

            migrationBuilder.AddForeignKey(
                name: "FK_JobPlacements_Employments_employeeID",
                table: "JobPlacements",
                column: "employeeID",
                principalTable: "Employments",
                principalColumn: "employmentID");
        }
    }
}
