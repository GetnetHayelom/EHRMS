using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration91 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeductionRecords_PayrollPays_payrollPayID",
                table: "DeductionRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_EarningRecords_PayrollPays_payrollPayID",
                table: "EarningRecords");

            migrationBuilder.AddColumn<int>(
                name: "payrollPayID1",
                table: "EarningRecords",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "payrollPayID1",
                table: "DeductionRecords",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EarningRecords_payrollPayID1",
                table: "EarningRecords",
                column: "payrollPayID1");

            migrationBuilder.CreateIndex(
                name: "IX_DeductionRecords_payrollPayID1",
                table: "DeductionRecords",
                column: "payrollPayID1");

            migrationBuilder.AddForeignKey(
                name: "FK_DeductionRecords_PayrollPays_payrollPayID",
                table: "DeductionRecords",
                column: "payrollPayID",
                principalTable: "PayrollPays",
                principalColumn: "payrollPayID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeductionRecords_PayrollPays_payrollPayID1",
                table: "DeductionRecords",
                column: "payrollPayID1",
                principalTable: "PayrollPays",
                principalColumn: "payrollPayID");

            migrationBuilder.AddForeignKey(
                name: "FK_EarningRecords_PayrollPays_payrollPayID",
                table: "EarningRecords",
                column: "payrollPayID",
                principalTable: "PayrollPays",
                principalColumn: "payrollPayID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EarningRecords_PayrollPays_payrollPayID1",
                table: "EarningRecords",
                column: "payrollPayID1",
                principalTable: "PayrollPays",
                principalColumn: "payrollPayID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeductionRecords_PayrollPays_payrollPayID",
                table: "DeductionRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_DeductionRecords_PayrollPays_payrollPayID1",
                table: "DeductionRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_EarningRecords_PayrollPays_payrollPayID",
                table: "EarningRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_EarningRecords_PayrollPays_payrollPayID1",
                table: "EarningRecords");

            migrationBuilder.DropIndex(
                name: "IX_EarningRecords_payrollPayID1",
                table: "EarningRecords");

            migrationBuilder.DropIndex(
                name: "IX_DeductionRecords_payrollPayID1",
                table: "DeductionRecords");

            migrationBuilder.DropColumn(
                name: "payrollPayID1",
                table: "EarningRecords");

            migrationBuilder.DropColumn(
                name: "payrollPayID1",
                table: "DeductionRecords");

            migrationBuilder.AddForeignKey(
                name: "FK_DeductionRecords_PayrollPays_payrollPayID",
                table: "DeductionRecords",
                column: "payrollPayID",
                principalTable: "PayrollPays",
                principalColumn: "payrollPayID");

            migrationBuilder.AddForeignKey(
                name: "FK_EarningRecords_PayrollPays_payrollPayID",
                table: "EarningRecords",
                column: "payrollPayID",
                principalTable: "PayrollPays",
                principalColumn: "payrollPayID");
        }
    }
}
