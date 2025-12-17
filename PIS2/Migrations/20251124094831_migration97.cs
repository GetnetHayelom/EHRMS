using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration97 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "deduction",
                table: "DeductionHistories",
                newName: "deductionID");

            migrationBuilder.RenameColumn(
                name: "dedcutionModel",
                table: "DeductionHistories",
                newName: "dedcutionStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "deductionID",
                table: "DeductionHistories",
                newName: "deduction");

            migrationBuilder.RenameColumn(
                name: "dedcutionStatus",
                table: "DeductionHistories",
                newName: "dedcutionModel");
        }
    }
}
