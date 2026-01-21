using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration109 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "employmentMethod",
                table: "Vacancies",
                newName: "employmentMethodID");

            migrationBuilder.AlterColumn<string>(
                name: "Remark",
                table: "Vacancies",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "employmentMethodID",
                table: "Vacancies",
                newName: "employmentMethod");

            migrationBuilder.AlterColumn<string>(
                name: "Remark",
                table: "Vacancies",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}
