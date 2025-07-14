using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration53 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expriences_JobPlacements_jobPlacementModeljobPlacementID",
                table: "Expriences");

            migrationBuilder.DropIndex(
                name: "IX_Expriences_jobPlacementModeljobPlacementID",
                table: "Expriences");

            migrationBuilder.DropColumn(
                name: "jobPlacementID",
                table: "Expriences");

            migrationBuilder.DropColumn(
                name: "jobPlacementModeljobPlacementID",
                table: "Expriences");

            migrationBuilder.AddColumn<DateTime>(
                name: "modifiedDate",
                table: "PersonEducationLevels",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "jobTitle",
                table: "Expriences",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "experienceType",
                table: "Expriences",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "jobDepartment",
                table: "Expriences",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "jobGrade",
                table: "Expriences",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "jobSalary",
                table: "Expriences",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "jobStep",
                table: "Expriences",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "Expriences",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "modifiedDate",
                table: "Expriences",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "modifiedDate",
                table: "PersonEducationLevels");

            migrationBuilder.DropColumn(
                name: "experienceType",
                table: "Expriences");

            migrationBuilder.DropColumn(
                name: "jobDepartment",
                table: "Expriences");

            migrationBuilder.DropColumn(
                name: "jobGrade",
                table: "Expriences");

            migrationBuilder.DropColumn(
                name: "jobSalary",
                table: "Expriences");

            migrationBuilder.DropColumn(
                name: "jobStep",
                table: "Expriences");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "Expriences");

            migrationBuilder.DropColumn(
                name: "modifiedDate",
                table: "Expriences");

            migrationBuilder.AlterColumn<string>(
                name: "jobTitle",
                table: "Expriences",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "jobPlacementID",
                table: "Expriences",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "jobPlacementModeljobPlacementID",
                table: "Expriences",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Expriences_jobPlacementModeljobPlacementID",
                table: "Expriences",
                column: "jobPlacementModeljobPlacementID");

            migrationBuilder.AddForeignKey(
                name: "FK_Expriences_JobPlacements_jobPlacementModeljobPlacementID",
                table: "Expriences",
                column: "jobPlacementModeljobPlacementID",
                principalTable: "JobPlacements",
                principalColumn: "jobPlacementID");
        }
    }
}
