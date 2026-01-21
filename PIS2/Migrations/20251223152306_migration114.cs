using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration114 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeductionRecords_PayrollPays_payrollPayID1",
                table: "DeductionRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_Deductions_Employments_EmploymentModelemploymentID",
                table: "Deductions");

            migrationBuilder.DropForeignKey(
                name: "FK_EarningRecords_PayrollPays_payrollPayID1",
                table: "EarningRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_Earnings_Employments_EmploymentModelemploymentID",
                table: "Earnings");

            migrationBuilder.DropIndex(
                name: "IX_Earnings_EmploymentModelemploymentID",
                table: "Earnings");

            migrationBuilder.DropIndex(
                name: "IX_EarningRecords_payrollPayID1",
                table: "EarningRecords");

            migrationBuilder.DropIndex(
                name: "IX_Deductions_EmploymentModelemploymentID",
                table: "Deductions");

            migrationBuilder.DropIndex(
                name: "IX_DeductionRecords_payrollPayID1",
                table: "DeductionRecords");

            migrationBuilder.DropColumn(
                name: "EmploymentModelemploymentID",
                table: "Earnings");

            migrationBuilder.DropColumn(
                name: "payrollPayID1",
                table: "EarningRecords");

            migrationBuilder.DropColumn(
                name: "EmploymentModelemploymentID",
                table: "Deductions");

            migrationBuilder.DropColumn(
                name: "payrollPayID1",
                table: "DeductionRecords");

            migrationBuilder.AddColumn<int>(
                name: "DepartmentModeldepartmentID",
                table: "Leaves",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "departmentID",
                table: "Leaves",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "leaveCost",
                table: "Leaves",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "earningReference",
                table: "Earnings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "earningReference",
                table: "EarningRecords",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "Trainings",
                columns: table => new
                {
                    trainingID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    trainingTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    category = table.Column<int>(type: "int", nullable: false),
                    isMandatory = table.Column<bool>(type: "bit", nullable: false),
                    validityMonths = table.Column<int>(type: "int", nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trainings", x => x.trainingID);
                });

            migrationBuilder.CreateTable(
                name: "TrainingSessions",
                columns: table => new
                {
                    trainingSessionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    trainingID = table.Column<int>(type: "int", nullable: false),
                    startDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    endDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    deliveryMode = table.Column<int>(type: "int", nullable: false),
                    trainerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    provider = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    sessionStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingSessions", x => x.trainingSessionID);
                    table.ForeignKey(
                        name: "FK_TrainingSessions_Trainings_trainingID",
                        column: x => x.trainingID,
                        principalTable: "Trainings",
                        principalColumn: "trainingID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainingAttendances",
                columns: table => new
                {
                    trainingAttendanceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    trainingSessionID = table.Column<int>(type: "int", nullable: false),
                    employmentID = table.Column<int>(type: "int", nullable: false),
                    result = table.Column<int>(type: "int", nullable: false),
                    completionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    score = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    certificateNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    certificateExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingAttendances", x => x.trainingAttendanceID);
                    table.ForeignKey(
                        name: "FK_TrainingAttendances_Employments_employmentID",
                        column: x => x.employmentID,
                        principalTable: "Employments",
                        principalColumn: "employmentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainingAttendances_TrainingSessions_trainingSessionID",
                        column: x => x.trainingSessionID,
                        principalTable: "TrainingSessions",
                        principalColumn: "trainingSessionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainingCostAllocations",
                columns: table => new
                {
                    trainingCostAllocationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    trainingAttendanceID = table.Column<int>(type: "int", nullable: false),
                    AttendancetrainingAttendanceID = table.Column<int>(type: "int", nullable: true),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    allocatedCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingCostAllocations", x => x.trainingCostAllocationID);
                    table.ForeignKey(
                        name: "FK_TrainingCostAllocations_TrainingAttendances_AttendancetrainingAttendanceID",
                        column: x => x.AttendancetrainingAttendanceID,
                        principalTable: "TrainingAttendances",
                        principalColumn: "trainingAttendanceID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Leaves_DepartmentModeldepartmentID",
                table: "Leaves",
                column: "DepartmentModeldepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Earnings_employmentID",
                table: "Earnings",
                column: "employmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Deductions_employmentID",
                table: "Deductions",
                column: "employmentID");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingAttendances_employmentID",
                table: "TrainingAttendances",
                column: "employmentID");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingAttendances_trainingSessionID",
                table: "TrainingAttendances",
                column: "trainingSessionID");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingCostAllocations_AttendancetrainingAttendanceID",
                table: "TrainingCostAllocations",
                column: "AttendancetrainingAttendanceID");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessions_trainingID",
                table: "TrainingSessions",
                column: "trainingID");

            migrationBuilder.AddForeignKey(
                name: "FK_Deductions_Employments_employmentID",
                table: "Deductions",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Earnings_Employments_employmentID",
                table: "Earnings",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Leaves_Departments_DepartmentModeldepartmentID",
                table: "Leaves",
                column: "DepartmentModeldepartmentID",
                principalTable: "Departments",
                principalColumn: "departmentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deductions_Employments_employmentID",
                table: "Deductions");

            migrationBuilder.DropForeignKey(
                name: "FK_Earnings_Employments_employmentID",
                table: "Earnings");

            migrationBuilder.DropForeignKey(
                name: "FK_Leaves_Departments_DepartmentModeldepartmentID",
                table: "Leaves");

            migrationBuilder.DropTable(
                name: "TrainingCostAllocations");

            migrationBuilder.DropTable(
                name: "TrainingAttendances");

            migrationBuilder.DropTable(
                name: "TrainingSessions");

            migrationBuilder.DropTable(
                name: "Trainings");

            migrationBuilder.DropIndex(
                name: "IX_Leaves_DepartmentModeldepartmentID",
                table: "Leaves");

            migrationBuilder.DropIndex(
                name: "IX_Earnings_employmentID",
                table: "Earnings");

            migrationBuilder.DropIndex(
                name: "IX_Deductions_employmentID",
                table: "Deductions");

            migrationBuilder.DropColumn(
                name: "DepartmentModeldepartmentID",
                table: "Leaves");

            migrationBuilder.DropColumn(
                name: "departmentID",
                table: "Leaves");

            migrationBuilder.DropColumn(
                name: "leaveCost",
                table: "Leaves");

            migrationBuilder.DropColumn(
                name: "earningReference",
                table: "Earnings");

            migrationBuilder.AddColumn<int>(
                name: "EmploymentModelemploymentID",
                table: "Earnings",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "earningReference",
                table: "EarningRecords",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "payrollPayID1",
                table: "EarningRecords",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmploymentModelemploymentID",
                table: "Deductions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "payrollPayID1",
                table: "DeductionRecords",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Earnings_EmploymentModelemploymentID",
                table: "Earnings",
                column: "EmploymentModelemploymentID");

            migrationBuilder.CreateIndex(
                name: "IX_EarningRecords_payrollPayID1",
                table: "EarningRecords",
                column: "payrollPayID1");

            migrationBuilder.CreateIndex(
                name: "IX_Deductions_EmploymentModelemploymentID",
                table: "Deductions",
                column: "EmploymentModelemploymentID");

            migrationBuilder.CreateIndex(
                name: "IX_DeductionRecords_payrollPayID1",
                table: "DeductionRecords",
                column: "payrollPayID1");

            migrationBuilder.AddForeignKey(
                name: "FK_DeductionRecords_PayrollPays_payrollPayID1",
                table: "DeductionRecords",
                column: "payrollPayID1",
                principalTable: "PayrollPays",
                principalColumn: "payrollPayID");

            migrationBuilder.AddForeignKey(
                name: "FK_Deductions_Employments_EmploymentModelemploymentID",
                table: "Deductions",
                column: "EmploymentModelemploymentID",
                principalTable: "Employments",
                principalColumn: "employmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_EarningRecords_PayrollPays_payrollPayID1",
                table: "EarningRecords",
                column: "payrollPayID1",
                principalTable: "PayrollPays",
                principalColumn: "payrollPayID");

            migrationBuilder.AddForeignKey(
                name: "FK_Earnings_Employments_EmploymentModelemploymentID",
                table: "Earnings",
                column: "EmploymentModelemploymentID",
                principalTable: "Employments",
                principalColumn: "employmentID");
        }
    }
}
