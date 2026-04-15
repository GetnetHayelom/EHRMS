using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration155 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddForeignKey(
                name: "FK_OtherPayments_BankInfos_creditAccountID",
                table: "OtherPayments",
                column: "creditAccountID",
                principalTable: "BankInfos",
                principalColumn: "bankInfoID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_OtherPayments_SubAccounts_creditAccountID",
                table: "OtherPayments",
                column: "creditAccountID",
                principalTable: "SubAccounts",
                principalColumn: "subAccountID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollPays_BankInfos_creditAccountID",
                table: "PayrollPays",
                column: "creditAccountID",
                principalTable: "BankInfos",
                principalColumn: "bankInfoID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollPays_SubAccounts_creditAccountID",
                table: "PayrollPays",
                column: "creditAccountID",
                principalTable: "SubAccounts",
                principalColumn: "subAccountID",
                onDelete: ReferentialAction.SetNull);
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
    }
}
