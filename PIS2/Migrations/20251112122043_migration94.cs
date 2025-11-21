using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration94 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PensionEmployee",
                table: "PayrollPays");

            migrationBuilder.DropColumn(
                name: "PensionEmployer",
                table: "PayrollPays");

            migrationBuilder.DropColumn(
                name: "TotalTax",
                table: "PayrollPays");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PensionEmployee",
                table: "PayrollPays",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PensionEmployer",
                table: "PayrollPays",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalTax",
                table: "PayrollPays",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
