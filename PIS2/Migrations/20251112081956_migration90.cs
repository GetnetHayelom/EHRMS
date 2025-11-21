using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration90 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deductions_Employments_employmentID",
                table: "Deductions");

            migrationBuilder.DropForeignKey(
                name: "FK_Deductions_PayrollPays_PayrollPayspayrollPayID",
                table: "Deductions");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Deductions_PayrollPays_payrollPayID",
            //    table: "Deductions");

            migrationBuilder.DropForeignKey(
                name: "FK_Earnings_EarningTypes_EarningTypeId",
                table: "Earnings");

            migrationBuilder.DropForeignKey(
                name: "FK_Earnings_Employments_employmentID",
                table: "Earnings");

            migrationBuilder.DropForeignKey(
                name: "FK_Earnings_PayrollPays_PayrollPayspayrollPayID",
                table: "Earnings");

            migrationBuilder.DropForeignKey(
                name: "FK_Earnings_PayrollPays_payrollPayID",
                table: "Earnings");

            migrationBuilder.DropIndex(
                name: "IX_Earnings_employmentID",
                table: "Earnings");

            migrationBuilder.DropIndex(
                name: "IX_Earnings_payrollPayID",
                table: "Earnings");

            migrationBuilder.DropIndex(
                name: "IX_Deductions_employmentID",
                table: "Deductions");

            //migrationBuilder.DropIndex(
            //    name: "IX_Deductions_payrollPayID",
            //    table: "Deductions");

            migrationBuilder.DropColumn(
                name: "penaltyMethod",
                table: "PenaltyTypes");

            migrationBuilder.DropColumn(
                name: "penaltyAmount",
                table: "Penalties");

            migrationBuilder.DropColumn(
                name: "earningReference",
                table: "Earnings");

            migrationBuilder.DropColumn(
                name: "payrollPayID",
                table: "Deductions");

            migrationBuilder.RenameColumn(
                name: "penaltyRate",
                table: "PenaltyTypes",
                newName: "penaltyAmount");

            migrationBuilder.RenameColumn(
                name: "EarningTypeId",
                table: "Earnings",
                newName: "earningTypeId");

            migrationBuilder.RenameColumn(
                name: "payrollPayID",
                table: "Earnings",
                newName: "earningBase");

            migrationBuilder.RenameColumn(
                name: "PayrollPayspayrollPayID",
                table: "Earnings",
                newName: "EmploymentModelemploymentID");

            migrationBuilder.RenameIndex(
                name: "IX_Earnings_EarningTypeId",
                table: "Earnings",
                newName: "IX_Earnings_earningTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Earnings_PayrollPayspayrollPayID",
                table: "Earnings",
                newName: "IX_Earnings_EmploymentModelemploymentID");

            migrationBuilder.RenameColumn(
                name: "deductFrom",
                table: "DeductionTypes",
                newName: "deductBase");

            migrationBuilder.RenameColumn(
                name: "PayrollPayspayrollPayID",
                table: "Deductions",
                newName: "EmploymentModelemploymentID");

            migrationBuilder.RenameIndex(
                name: "IX_Deductions_PayrollPayspayrollPayID",
                table: "Deductions",
                newName: "IX_Deductions_EmploymentModelemploymentID");

            migrationBuilder.AddColumn<bool>(
                name: "IsPercentage",
                table: "PenaltyTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPercentage",
                table: "Earnings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<decimal>(
                name: "deductionAmount",
                table: "Deductions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPercentage",
                table: "Deductions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "DeductionRecords",
                columns: table => new
                {
                    deductionRecordID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    deductionTypeID = table.Column<int>(type: "int", nullable: false),
                    deductionReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    deductionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    payrollPayID = table.Column<int>(type: "int", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeductionRecords", x => x.deductionRecordID);
                    table.ForeignKey(
                        name: "FK_DeductionRecords_DeductionTypes_deductionTypeID",
                        column: x => x.deductionTypeID,
                        principalTable: "DeductionTypes",
                        principalColumn: "deductionTypeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeductionRecords_PayrollPays_payrollPayID",
                        column: x => x.payrollPayID,
                        principalTable: "PayrollPays",
                        principalColumn: "payrollPayID");
                });

            migrationBuilder.CreateTable(
                name: "EarningRecords",
                columns: table => new
                {
                    earningRecordID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    earningTypeId = table.Column<int>(type: "int", nullable: false),
                    earningReference = table.Column<int>(type: "int", nullable: false),
                    earningAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    payrollPayID = table.Column<int>(type: "int", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EarningRecords", x => x.earningRecordID);
                    table.ForeignKey(
                        name: "FK_EarningRecords_EarningTypes_earningTypeId",
                        column: x => x.earningTypeId,
                        principalTable: "EarningTypes",
                        principalColumn: "earningTypeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EarningRecords_PayrollPays_payrollPayID",
                        column: x => x.payrollPayID,
                        principalTable: "PayrollPays",
                        principalColumn: "payrollPayID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeductionRecords_deductionTypeID",
                table: "DeductionRecords",
                column: "deductionTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_DeductionRecords_payrollPayID",
                table: "DeductionRecords",
                column: "payrollPayID");

            migrationBuilder.CreateIndex(
                name: "IX_EarningRecords_earningTypeId",
                table: "EarningRecords",
                column: "earningTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EarningRecords_payrollPayID",
                table: "EarningRecords",
                column: "payrollPayID");

            migrationBuilder.AddForeignKey(
                name: "FK_Deductions_Employments_EmploymentModelemploymentID",
                table: "Deductions",
                column: "EmploymentModelemploymentID",
                principalTable: "Employments",
                principalColumn: "employmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Earnings_EarningTypes_earningTypeId",
                table: "Earnings",
                column: "earningTypeId",
                principalTable: "EarningTypes",
                principalColumn: "earningTypeID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Earnings_Employments_EmploymentModelemploymentID",
                table: "Earnings",
                column: "EmploymentModelemploymentID",
                principalTable: "Employments",
                principalColumn: "employmentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deductions_Employments_EmploymentModelemploymentID",
                table: "Deductions");

            migrationBuilder.DropForeignKey(
                name: "FK_Earnings_EarningTypes_earningTypeId",
                table: "Earnings");

            migrationBuilder.DropForeignKey(
                name: "FK_Earnings_Employments_EmploymentModelemploymentID",
                table: "Earnings");

            migrationBuilder.DropTable(
                name: "DeductionRecords");

            migrationBuilder.DropTable(
                name: "EarningRecords");

            migrationBuilder.DropColumn(
                name: "IsPercentage",
                table: "PenaltyTypes");

            migrationBuilder.DropColumn(
                name: "IsPercentage",
                table: "Earnings");

            migrationBuilder.DropColumn(
                name: "IsPercentage",
                table: "Deductions");

            migrationBuilder.RenameColumn(
                name: "penaltyAmount",
                table: "PenaltyTypes",
                newName: "penaltyRate");

            migrationBuilder.RenameColumn(
                name: "earningTypeId",
                table: "Earnings",
                newName: "EarningTypeId");

            migrationBuilder.RenameColumn(
                name: "earningBase",
                table: "Earnings",
                newName: "payrollPayID");

            migrationBuilder.RenameColumn(
                name: "EmploymentModelemploymentID",
                table: "Earnings",
                newName: "PayrollPayspayrollPayID");

            migrationBuilder.RenameIndex(
                name: "IX_Earnings_earningTypeId",
                table: "Earnings",
                newName: "IX_Earnings_EarningTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Earnings_EmploymentModelemploymentID",
                table: "Earnings",
                newName: "IX_Earnings_PayrollPayspayrollPayID");

            migrationBuilder.RenameColumn(
                name: "deductBase",
                table: "DeductionTypes",
                newName: "deductFrom");

            migrationBuilder.RenameColumn(
                name: "EmploymentModelemploymentID",
                table: "Deductions",
                newName: "PayrollPayspayrollPayID");

            migrationBuilder.RenameIndex(
                name: "IX_Deductions_EmploymentModelemploymentID",
                table: "Deductions",
                newName: "IX_Deductions_PayrollPayspayrollPayID");

            migrationBuilder.AddColumn<int>(
                name: "penaltyMethod",
                table: "PenaltyTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "penaltyAmount",
                table: "Penalties",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "earningReference",
                table: "Earnings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "deductionAmount",
                table: "Deductions",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<int>(
                name: "payrollPayID",
                table: "Deductions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Earnings_employmentID",
                table: "Earnings",
                column: "employmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Earnings_payrollPayID",
                table: "Earnings",
                column: "payrollPayID");

            migrationBuilder.CreateIndex(
                name: "IX_Deductions_employmentID",
                table: "Deductions",
                column: "employmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Deductions_payrollPayID",
                table: "Deductions",
                column: "payrollPayID");

            migrationBuilder.AddForeignKey(
                name: "FK_Deductions_Employments_employmentID",
                table: "Deductions",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Deductions_PayrollPays_PayrollPayspayrollPayID",
                table: "Deductions",
                column: "PayrollPayspayrollPayID",
                principalTable: "PayrollPays",
                principalColumn: "payrollPayID");

            migrationBuilder.AddForeignKey(
                name: "FK_Deductions_PayrollPays_payrollPayID",
                table: "Deductions",
                column: "payrollPayID",
                principalTable: "PayrollPays",
                principalColumn: "payrollPayID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Earnings_EarningTypes_EarningTypeId",
                table: "Earnings",
                column: "EarningTypeId",
                principalTable: "EarningTypes",
                principalColumn: "earningTypeID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Earnings_Employments_employmentID",
                table: "Earnings",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Earnings_PayrollPays_PayrollPayspayrollPayID",
                table: "Earnings",
                column: "PayrollPayspayrollPayID",
                principalTable: "PayrollPays",
                principalColumn: "payrollPayID");

            migrationBuilder.AddForeignKey(
                name: "FK_Earnings_PayrollPays_payrollPayID",
                table: "Earnings",
                column: "payrollPayID",
                principalTable: "PayrollPays",
                principalColumn: "payrollPayID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
