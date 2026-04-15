using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration160 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "earningTypeCode",
                table: "EarningTypes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "deductionCode",
                table: "DeductionTypes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "deductionDescription",
                table: "DeductionTypes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "earningTypeCode",
                table: "EarningTypes");

            migrationBuilder.DropColumn(
                name: "deductionCode",
                table: "DeductionTypes");

            migrationBuilder.DropColumn(
                name: "deductionDescription",
                table: "DeductionTypes");
        }
    }
}
