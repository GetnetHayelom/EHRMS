using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration125 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EarningRecords_EarningTypes_earningTypeId",
                table: "EarningRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_Earnings_EarningTypes_earningTypeId",
                table: "Earnings");

            migrationBuilder.RenameColumn(
                name: "earningTypeId",
                table: "Earnings",
                newName: "earningTypeID");

            migrationBuilder.RenameIndex(
                name: "IX_Earnings_earningTypeId",
                table: "Earnings",
                newName: "IX_Earnings_earningTypeID");

            migrationBuilder.RenameColumn(
                name: "earningTypeId",
                table: "EarningRecords",
                newName: "earningTypeID");

            migrationBuilder.RenameIndex(
                name: "IX_EarningRecords_earningTypeId",
                table: "EarningRecords",
                newName: "IX_EarningRecords_earningTypeID");

            migrationBuilder.CreateTable(
                name: "OtherPayments",
                columns: table => new
                {
                    paymentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    invoiceNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    earningID = table.Column<int>(type: "int", nullable: false),
                    GrossPay = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetPay = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtherPayments", x => x.paymentID);
                    table.ForeignKey(
                        name: "FK_OtherPayments_Earnings_earningID",
                        column: x => x.earningID,
                        principalTable: "Earnings",
                        principalColumn: "earningID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_OtherPayments_earningID",
                table: "OtherPayments",
                column: "earningID");

            migrationBuilder.AddForeignKey(
                name: "FK_EarningRecords_EarningTypes_earningTypeID",
                table: "EarningRecords",
                column: "earningTypeID",
                principalTable: "EarningTypes",
                principalColumn: "earningTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Earnings_EarningTypes_earningTypeID",
                table: "Earnings",
                column: "earningTypeID",
                principalTable: "EarningTypes",
                principalColumn: "earningTypeID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EarningRecords_EarningTypes_earningTypeID",
                table: "EarningRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_Earnings_EarningTypes_earningTypeID",
                table: "Earnings");

            migrationBuilder.DropTable(
                name: "OtherPayments");

            migrationBuilder.RenameColumn(
                name: "earningTypeID",
                table: "Earnings",
                newName: "earningTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Earnings_earningTypeID",
                table: "Earnings",
                newName: "IX_Earnings_earningTypeId");

            migrationBuilder.RenameColumn(
                name: "earningTypeID",
                table: "EarningRecords",
                newName: "earningTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_EarningRecords_earningTypeID",
                table: "EarningRecords",
                newName: "IX_EarningRecords_earningTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_EarningRecords_EarningTypes_earningTypeId",
                table: "EarningRecords",
                column: "earningTypeId",
                principalTable: "EarningTypes",
                principalColumn: "earningTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Earnings_EarningTypes_earningTypeId",
                table: "Earnings",
                column: "earningTypeId",
                principalTable: "EarningTypes",
                principalColumn: "earningTypeID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
