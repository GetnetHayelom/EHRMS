using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration98 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "remainingIteration",
                table: "Earnings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "deductionBase",
                table: "Deductions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "remainingIteration",
                table: "Deductions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "remainingIteration",
                table: "Earnings");

            migrationBuilder.DropColumn(
                name: "deductionBase",
                table: "Deductions");

            migrationBuilder.DropColumn(
                name: "remainingIteration",
                table: "Deductions");
        }
    }
}
