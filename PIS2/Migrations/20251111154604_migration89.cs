using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration89 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deductions_PayrollPays_payrollPayID",
                table: "Deductions");

            migrationBuilder.DropForeignKey(
                name: "FK_Earnings_PayrollPays_payrollPayID",
                table: "Earnings");

            migrationBuilder.AlterColumn<int>(
                name: "payrollPayID",
                table: "Earnings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PayrollPayspayrollPayID",
                table: "Earnings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "payrollPayID",
                table: "Deductions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PayrollPayspayrollPayID",
                table: "Deductions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Earnings_PayrollPayspayrollPayID",
                table: "Earnings",
                column: "PayrollPayspayrollPayID");

            migrationBuilder.CreateIndex(
                name: "IX_Deductions_PayrollPayspayrollPayID",
                table: "Deductions",
                column: "PayrollPayspayrollPayID");

            migrationBuilder.AddForeignKey(
                name: "FK_Deductions_PayrollPays_PayrollPayspayrollPayID",
                table: "Deductions",
                column: "PayrollPayspayrollPayID",
                principalTable: "PayrollPays",
                principalColumn: "payrollPayID");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Deductions_PayrollPays_payrollPayID",
            //    table: "Deductions",
            //    column: "payrollPayID",
            //    principalTable: "PayrollPays",
            //    principalColumn: "payrollPayID",
            //    onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Earnings_PayrollPays_PayrollPayspayrollPayID",
                table: "Earnings",
                column: "PayrollPayspayrollPayID",
                principalTable: "PayrollPays",
                principalColumn: "payrollPayID");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Earnings_PayrollPays_payrollPayID",
            //    table: "Earnings",
            //    column: "payrollPayID",
            //    principalTable: "PayrollPays",
            //    principalColumn: "payrollPayID",
            //    onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deductions_PayrollPays_PayrollPayspayrollPayID",
                table: "Deductions");

            migrationBuilder.DropForeignKey(
                name: "FK_Deductions_PayrollPays_payrollPayID",
                table: "Deductions");

            migrationBuilder.DropForeignKey(
                name: "FK_Earnings_PayrollPays_PayrollPayspayrollPayID",
                table: "Earnings");

            migrationBuilder.DropForeignKey(
                name: "FK_Earnings_PayrollPays_payrollPayID",
                table: "Earnings");

            migrationBuilder.DropIndex(
                name: "IX_Earnings_PayrollPayspayrollPayID",
                table: "Earnings");

            migrationBuilder.DropIndex(
                name: "IX_Deductions_PayrollPayspayrollPayID",
                table: "Deductions");

            migrationBuilder.DropColumn(
                name: "PayrollPayspayrollPayID",
                table: "Earnings");

            migrationBuilder.DropColumn(
                name: "PayrollPayspayrollPayID",
                table: "Deductions");

            //migrationBuilder.AlterColumn<int>(
            //    name: "payrollPayID",
            //    table: "Earnings",
            //    type: "int",
            //    nullable: true,
            //    oldClrType: typeof(int),
            //    oldType: "int");

            //migrationBuilder.AlterColumn<int>(
            //    name: "payrollPayID",
            //    table: "Deductions",
            //    type: "int",
            //    nullable: true,
            //    oldClrType: typeof(int),
            //    oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Deductions_PayrollPays_payrollPayID",
                table: "Deductions",
                column: "payrollPayID",
                principalTable: "PayrollPays",
                principalColumn: "payrollPayID");

            migrationBuilder.AddForeignKey(
                name: "FK_Earnings_PayrollPays_payrollPayID",
                table: "Earnings",
                column: "payrollPayID",
                principalTable: "PayrollPays",
                principalColumn: "payrollPayID");
        }
    }
}
