using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration154 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PayrollPays_Payrolls_payrollModelpayrollID",
                table: "PayrollPays");

            migrationBuilder.DropIndex(
                name: "IX_PayrollPays_payrollModelpayrollID",
                table: "PayrollPays");

            migrationBuilder.RenameColumn(
                name: "payrollModelpayrollID",
                table: "PayrollPays",
                newName: "debitAccountID");

            migrationBuilder.AddColumn<int>(
                name: "creditAccountID",
                table: "PayrollPays",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "creditAccountID",
                table: "OtherPayments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "debitAccountID",
                table: "OtherPayments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PayrollPays_creditAccountID",
                table: "PayrollPays",
                column: "creditAccountID");

            migrationBuilder.CreateIndex(
                name: "IX_OtherPayments_creditAccountID",
                table: "OtherPayments",
                column: "creditAccountID");

            migrationBuilder.AddForeignKey(
                name: "FK_OtherPayments_BankInfos_creditAccountID",
                table: "OtherPayments",
                column: "creditAccountID",
                principalTable: "BankInfos",
                principalColumn: "bankInfoID");

            migrationBuilder.AddForeignKey(
                name: "FK_OtherPayments_SubAccounts_creditAccountID",
                table: "OtherPayments",
                column: "creditAccountID",
                principalTable: "SubAccounts",
                principalColumn: "subAccountID");

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollPays_BankInfos_creditAccountID",
                table: "PayrollPays",
                column: "creditAccountID",
                principalTable: "BankInfos",
                principalColumn: "bankInfoID");

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollPays_SubAccounts_creditAccountID",
                table: "PayrollPays",
                column: "creditAccountID",
                principalTable: "SubAccounts",
                principalColumn: "subAccountID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OtherPayments_BankInfos_creditAccountID",
                table: "OtherPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_OtherPayments_SubAccounts_creditAccountID",
                table: "OtherPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollPays_BankInfos_creditAccountID",
                table: "PayrollPays");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollPays_SubAccounts_creditAccountID",
                table: "PayrollPays");

            migrationBuilder.DropIndex(
                name: "IX_PayrollPays_creditAccountID",
                table: "PayrollPays");

            migrationBuilder.DropIndex(
                name: "IX_OtherPayments_creditAccountID",
                table: "OtherPayments");

            migrationBuilder.DropColumn(
                name: "creditAccountID",
                table: "PayrollPays");

            migrationBuilder.DropColumn(
                name: "creditAccountID",
                table: "OtherPayments");

            migrationBuilder.DropColumn(
                name: "debitAccountID",
                table: "OtherPayments");

            migrationBuilder.RenameColumn(
                name: "debitAccountID",
                table: "PayrollPays",
                newName: "payrollModelpayrollID");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollPays_payrollModelpayrollID",
                table: "PayrollPays",
                column: "payrollModelpayrollID");

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollPays_Payrolls_payrollModelpayrollID",
                table: "PayrollPays",
                column: "payrollModelpayrollID",
                principalTable: "Payrolls",
                principalColumn: "payrollID");
        }
    }
}
