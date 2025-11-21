using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration92 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "payrollModelpayrollID",
                table: "PayrollPays",
                type: "int",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PayrollPays_Payrolls_payrollModelpayrollID",
                table: "PayrollPays");

            migrationBuilder.DropIndex(
                name: "IX_PayrollPays_payrollModelpayrollID",
                table: "PayrollPays");

            migrationBuilder.DropColumn(
                name: "payrollModelpayrollID",
                table: "PayrollPays");
        }
    }
}
