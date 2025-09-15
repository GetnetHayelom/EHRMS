using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration73 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "penaltyStartDate",
                table: "Penalties",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "penaltyEndDate",
                table: "Penalties",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<decimal>(
                name: "penaltyAmount",
                table: "Penalties",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<bool>(
                name: "leaveJob",
                table: "LeaveTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "leaveLegality",
                table: "LeaveTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "DeductionTypes",
                columns: table => new
                {
                    deductionTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    deductionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isRecurring = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeductionTypes", x => x.deductionTypeID);
                });

            migrationBuilder.CreateTable(
                name: "EarningTypes",
                columns: table => new
                {
                    earningTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    earningName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isRecurring = table.Column<bool>(type: "bit", nullable: false),
                    isTaxable = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EarningTypes", x => x.earningTypeID);
                });

            migrationBuilder.CreateTable(
                name: "Payrolls",
                columns: table => new
                {
                    payrollID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    payrollName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    payrollStatus = table.Column<int>(type: "int", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payrolls", x => x.payrollID);
                });

            migrationBuilder.CreateTable(
                name: "PayrollHistories",
                columns: table => new
                {
                    payrollHistoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    payrollID = table.Column<int>(type: "int", nullable: false),
                    payrollStatus = table.Column<int>(type: "int", nullable: true),
                    modfiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollHistories", x => x.payrollHistoryID);
                    table.ForeignKey(
                        name: "FK_PayrollHistories_Payrolls_payrollID",
                        column: x => x.payrollID,
                        principalTable: "Payrolls",
                        principalColumn: "payrollID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PayrollPays",
                columns: table => new
                {
                    payrollPayID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    payrollID = table.Column<int>(type: "int", nullable: false),
                    employmentID = table.Column<int>(type: "int", nullable: false),
                    GrossPay = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetPay = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollPays", x => x.payrollPayID);
                    table.ForeignKey(
                        name: "FK_PayrollPays_Employments_employmentID",
                        column: x => x.employmentID,
                        principalTable: "Employments",
                        principalColumn: "employmentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PayrollPays_Payrolls_payrollID",
                        column: x => x.payrollID,
                        principalTable: "Payrolls",
                        principalColumn: "payrollID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Deductions",
                columns: table => new
                {
                    deductionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    deductionTypeID = table.Column<int>(type: "int", nullable: false),
                    deductionReference = table.Column<int>(type: "int", nullable: false),
                    employmentID = table.Column<int>(type: "int", nullable: false),
                    deductionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    payrollID = table.Column<int>(type: "int", nullable: false),
                    deductionStatus = table.Column<int>(type: "int", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    payrollPayID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deductions", x => x.deductionID);
                    table.ForeignKey(
                        name: "FK_Deductions_DeductionTypes_deductionTypeID",
                        column: x => x.deductionTypeID,
                        principalTable: "DeductionTypes",
                        principalColumn: "deductionTypeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Deductions_Employments_employmentID",
                        column: x => x.employmentID,
                        principalTable: "Employments",
                        principalColumn: "employmentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Deductions_PayrollPays_payrollPayID",
                        column: x => x.payrollPayID,
                        principalTable: "PayrollPays",
                        principalColumn: "payrollPayID");
                    table.ForeignKey(
                        name: "FK_Deductions_Payrolls_payrollID",
                        column: x => x.payrollID,
                        principalTable: "Payrolls",
                        principalColumn: "payrollID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Earnings",
                columns: table => new
                {
                    earningID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EarningTypeId = table.Column<int>(type: "int", nullable: false),
                    earningReference = table.Column<int>(type: "int", nullable: false),
                    employmentID = table.Column<int>(type: "int", nullable: false),
                    earningAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    payrollID = table.Column<int>(type: "int", nullable: true),
                    earningStatus = table.Column<int>(type: "int", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    payrollPayID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Earnings", x => x.earningID);
                    table.ForeignKey(
                        name: "FK_Earnings_EarningTypes_EarningTypeId",
                        column: x => x.EarningTypeId,
                        principalTable: "EarningTypes",
                        principalColumn: "earningTypeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Earnings_Employments_employmentID",
                        column: x => x.employmentID,
                        principalTable: "Employments",
                        principalColumn: "employmentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Earnings_PayrollPays_payrollPayID",
                        column: x => x.payrollPayID,
                        principalTable: "PayrollPays",
                        principalColumn: "payrollPayID");
                    table.ForeignKey(
                        name: "FK_Earnings_Payrolls_payrollID",
                        column: x => x.payrollID,
                        principalTable: "Payrolls",
                        principalColumn: "payrollID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Deductions_deductionTypeID",
                table: "Deductions",
                column: "deductionTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Deductions_employmentID",
                table: "Deductions",
                column: "employmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Deductions_payrollID",
                table: "Deductions",
                column: "payrollID");

            migrationBuilder.CreateIndex(
                name: "IX_Deductions_payrollPayID",
                table: "Deductions",
                column: "payrollPayID");

            migrationBuilder.CreateIndex(
                name: "IX_Earnings_EarningTypeId",
                table: "Earnings",
                column: "EarningTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Earnings_employmentID",
                table: "Earnings",
                column: "employmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Earnings_payrollID",
                table: "Earnings",
                column: "payrollID");

            migrationBuilder.CreateIndex(
                name: "IX_Earnings_payrollPayID",
                table: "Earnings",
                column: "payrollPayID");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollHistories_payrollID",
                table: "PayrollHistories",
                column: "payrollID");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollPays_employmentID",
                table: "PayrollPays",
                column: "employmentID");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollPays_payrollID",
                table: "PayrollPays",
                column: "payrollID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Deductions");

            migrationBuilder.DropTable(
                name: "Earnings");

            migrationBuilder.DropTable(
                name: "PayrollHistories");

            migrationBuilder.DropTable(
                name: "DeductionTypes");

            migrationBuilder.DropTable(
                name: "EarningTypes");

            migrationBuilder.DropTable(
                name: "PayrollPays");

            migrationBuilder.DropTable(
                name: "Payrolls");

            migrationBuilder.DropColumn(
                name: "leaveJob",
                table: "LeaveTypes");

            migrationBuilder.DropColumn(
                name: "leaveLegality",
                table: "LeaveTypes");

            migrationBuilder.AlterColumn<DateTime>(
                name: "penaltyStartDate",
                table: "Penalties",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "penaltyEndDate",
                table: "Penalties",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "penaltyAmount",
                table: "Penalties",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
