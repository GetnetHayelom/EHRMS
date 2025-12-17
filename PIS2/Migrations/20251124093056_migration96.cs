using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration96 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "penaltyBase",
                table: "PenaltyTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "DeductionHistories",
                columns: table => new
                {
                    deductionHistoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    deduction = table.Column<int>(type: "int", nullable: false),
                    deductionModeldeductionID = table.Column<int>(type: "int", nullable: true),
                    dedcutionModel = table.Column<int>(type: "int", nullable: false),
                    dedcutionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    modifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeductionHistories", x => x.deductionHistoryID);
                    table.ForeignKey(
                        name: "FK_DeductionHistories_Deductions_deductionModeldeductionID",
                        column: x => x.deductionModeldeductionID,
                        principalTable: "Deductions",
                        principalColumn: "deductionID");
                });

            migrationBuilder.CreateTable(
                name: "EarningHistories",
                columns: table => new
                {
                    earningHistoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    earningID = table.Column<int>(type: "int", nullable: false),
                    earningModelearningID = table.Column<int>(type: "int", nullable: true),
                    earningStatus = table.Column<int>(type: "int", nullable: false),
                    earningAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    modifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EarningHistories", x => x.earningHistoryID);
                    table.ForeignKey(
                        name: "FK_EarningHistories_Earnings_earningModelearningID",
                        column: x => x.earningModelearningID,
                        principalTable: "Earnings",
                        principalColumn: "earningID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeductionHistories_deductionModeldeductionID",
                table: "DeductionHistories",
                column: "deductionModeldeductionID");

            migrationBuilder.CreateIndex(
                name: "IX_EarningHistories_earningModelearningID",
                table: "EarningHistories",
                column: "earningModelearningID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeductionHistories");

            migrationBuilder.DropTable(
                name: "EarningHistories");

            migrationBuilder.DropColumn(
                name: "penaltyBase",
                table: "PenaltyTypes");
        }
    }
}
