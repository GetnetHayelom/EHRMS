using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration88 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "EarningTypes");

            migrationBuilder.RenameColumn(
                name: "earningName",
                table: "EarningTypes",
                newName: "modifiedBy");

            migrationBuilder.RenameColumn(
                name: "earningIteration",
                table: "EarningTypes",
                newName: "earningTypeStatus");

            migrationBuilder.AddColumn<string>(
                name: "earningTypeDescription",
                table: "EarningTypes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "earningTypeName",
                table: "EarningTypes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "earningIteration",
                table: "Earnings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "earningTypeDescription",
                table: "EarningTypes");

            migrationBuilder.DropColumn(
                name: "earningTypeName",
                table: "EarningTypes");

            migrationBuilder.DropColumn(
                name: "earningIteration",
                table: "Earnings");

            migrationBuilder.RenameColumn(
                name: "modifiedBy",
                table: "EarningTypes",
                newName: "earningName");

            migrationBuilder.RenameColumn(
                name: "earningTypeStatus",
                table: "EarningTypes",
                newName: "earningIteration");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "EarningTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
