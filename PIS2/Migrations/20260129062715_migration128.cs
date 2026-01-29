using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration128 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "TrainingSessions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "modifiedDate",
                table: "TrainingSessions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "Trainings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "modifiedDate",
                table: "Trainings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "TrainingCostAllocations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "modifiedDate",
                table: "TrainingCostAllocations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "TrainingAttendances",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "modifiedDate",
                table: "TrainingAttendances",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "TrainingSessions");

            migrationBuilder.DropColumn(
                name: "modifiedDate",
                table: "TrainingSessions");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "Trainings");

            migrationBuilder.DropColumn(
                name: "modifiedDate",
                table: "Trainings");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "TrainingCostAllocations");

            migrationBuilder.DropColumn(
                name: "modifiedDate",
                table: "TrainingCostAllocations");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "TrainingAttendances");

            migrationBuilder.DropColumn(
                name: "modifiedDate",
                table: "TrainingAttendances");
        }
    }
}
