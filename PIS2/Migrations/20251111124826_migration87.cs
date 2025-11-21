using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration87 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "fromGross",
                table: "DeductionTypes");

            migrationBuilder.RenameColumn(
                name: "deductionIteration",
                table: "DeductionTypes",
                newName: "deductionStatus");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "DeductionTypes",
                newName: "deductFrom");

            migrationBuilder.AlterColumn<string>(
                name: "payrollMonth",
                table: "Payrolls",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "DeductionTypes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "deductionReference",
                table: "Deductions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "deductionIteration",
                table: "Deductions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "DeductionTypes");

            migrationBuilder.DropColumn(
                name: "deductionIteration",
                table: "Deductions");

            migrationBuilder.RenameColumn(
                name: "deductionStatus",
                table: "DeductionTypes",
                newName: "deductionIteration");

            migrationBuilder.RenameColumn(
                name: "deductFrom",
                table: "DeductionTypes",
                newName: "Status");

            migrationBuilder.AlterColumn<string>(
                name: "payrollMonth",
                table: "Payrolls",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "fromGross",
                table: "DeductionTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "deductionReference",
                table: "Deductions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
