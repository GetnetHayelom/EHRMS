using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration100 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "dedcutionStatus",
                table: "DeductionHistories",
                newName: "deductionStatus");

            migrationBuilder.RenameColumn(
                name: "dedcutionAmount",
                table: "DeductionHistories",
                newName: "deductionAmount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "deductionStatus",
                table: "DeductionHistories",
                newName: "dedcutionStatus");

            migrationBuilder.RenameColumn(
                name: "deductionAmount",
                table: "DeductionHistories",
                newName: "dedcutionAmount");
        }
    }
}
