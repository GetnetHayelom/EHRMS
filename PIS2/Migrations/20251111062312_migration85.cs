using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration85 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deductions_Payrolls_payrollID",
                table: "Deductions");

            migrationBuilder.DropForeignKey(
                name: "FK_Earnings_Payrolls_payrollID",
                table: "Earnings");

            migrationBuilder.DropIndex(
                name: "IX_Earnings_payrollID",
                table: "Earnings");

            migrationBuilder.DropIndex(
                name: "IX_Deductions_payrollID",
                table: "Deductions");

            migrationBuilder.DropColumn(
                name: "payrollID",
                table: "Earnings");

            migrationBuilder.DropColumn(
                name: "payrollID",
                table: "Deductions");

            migrationBuilder.AddColumn<int>(
                name: "companyID",
                table: "Payrolls",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "companyModelcompanyID",
                table: "Payrolls",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "payrollMonth",
                table: "Payrolls",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "totalEmployees",
                table: "Payrolls",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "totalGross",
                table: "Payrolls",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "totalNet",
                table: "Payrolls",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "totalPensionEmployee",
                table: "Payrolls",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "totalPensionEmployer",
                table: "Payrolls",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "totalTax",
                table: "Payrolls",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "earningIteration",
                table: "EarningTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "dedcutionPriority",
                table: "DeductionTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "deductionIteration",
                table: "DeductionTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "fromGross",
                table: "DeductionTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isMandatory",
                table: "DeductionTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<decimal>(
                name: "deductionAmount",
                table: "Deductions",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.CreateTable(
                name: "TaxRates",
                columns: table => new
                {
                    taxRateID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    reference = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    from = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ceiling = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    taxRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    deduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    tazStatus = table.Column<int>(type: "int", nullable: false),
                    modifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxRates", x => x.taxRateID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payrolls_companyModelcompanyID",
                table: "Payrolls",
                column: "companyModelcompanyID");

            migrationBuilder.AddForeignKey(
                name: "FK_Payrolls_Companies_companyModelcompanyID",
                table: "Payrolls",
                column: "companyModelcompanyID",
                principalTable: "Companies",
                principalColumn: "companyID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payrolls_Companies_companyModelcompanyID",
                table: "Payrolls");

            migrationBuilder.DropTable(
                name: "TaxRates");

            migrationBuilder.DropIndex(
                name: "IX_Payrolls_companyModelcompanyID",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "companyID",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "companyModelcompanyID",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "payrollMonth",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "totalEmployees",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "totalGross",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "totalNet",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "totalPensionEmployee",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "totalPensionEmployer",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "totalTax",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "earningIteration",
                table: "EarningTypes");

            migrationBuilder.DropColumn(
                name: "dedcutionPriority",
                table: "DeductionTypes");

            migrationBuilder.DropColumn(
                name: "deductionIteration",
                table: "DeductionTypes");

            migrationBuilder.DropColumn(
                name: "fromGross",
                table: "DeductionTypes");

            migrationBuilder.DropColumn(
                name: "isMandatory",
                table: "DeductionTypes");

            migrationBuilder.AddColumn<int>(
                name: "payrollID",
                table: "Earnings",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "deductionAmount",
                table: "Deductions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "payrollID",
                table: "Deductions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Earnings_payrollID",
                table: "Earnings",
                column: "payrollID");

            migrationBuilder.CreateIndex(
                name: "IX_Deductions_payrollID",
                table: "Deductions",
                column: "payrollID");

            migrationBuilder.AddForeignKey(
                name: "FK_Deductions_Payrolls_payrollID",
                table: "Deductions",
                column: "payrollID",
                principalTable: "Payrolls",
                principalColumn: "payrollID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Earnings_Payrolls_payrollID",
                table: "Earnings",
                column: "payrollID",
                principalTable: "Payrolls",
                principalColumn: "payrollID",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
