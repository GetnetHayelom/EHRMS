using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employments_personModelpersonID",
                table: "Employments");

            migrationBuilder.DropIndex(
                name: "IX_EducationLevels_personModelpersonID",
                table: "EducationLevels");

            migrationBuilder.DropIndex(
                name: "IX_Departments_companyID",
                table: "Departments");

            migrationBuilder.AlterColumn<string>(
                name: "workSiteName",
                table: "workSiteModel",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "subAccountName",
                table: "SubAccounts",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "shiftName",
                table: "Shifts",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "personIDType",
                table: "Persons",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "personIDNumber",
                table: "Persons",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "overtimeName",
                table: "Overtimes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "overtimeStatus",
                table: "OvertimeRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "loyaltyName",
                table: "Loyalties",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "leaveTypeName",
                table: "LeaveTypes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "jobTitle",
                table: "Jobs",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "jobGradeName",
                table: "JobGrades",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "jobCategoryName",
                table: "JobCategories",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "holidayName",
                table: "Holidays",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "employmentTypeName",
                table: "EmploymentTypes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "departmentName",
                table: "Departments",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "companyName",
                table: "Companies",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "bankName",
                table: "BankInfos",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "bankAccountNumber",
                table: "BankInfos",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "allowanceName",
                table: "Allowances",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "allowanceStatus",
                table: "AllowanceAssignments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_workSiteModel_workSiteName",
                table: "workSiteModel",
                column: "workSiteName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubAccounts_subAccountName",
                table: "SubAccounts",
                column: "subAccountName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_shiftName",
                table: "Shifts",
                column: "shiftName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Persons_personIDType_personIDNumber",
                table: "Persons",
                columns: new[] { "personIDType", "personIDNumber" },
                unique: true,
                filter: "[personIDType] IS NOT NULL AND [personIDNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PersonEducationLevels_educationLevelID_personID",
                table: "PersonEducationLevels",
                columns: new[] { "educationLevelID", "personID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Overtimes_overtimeName",
                table: "Overtimes",
                column: "overtimeName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OvertimeRecords_employmentID_overtimeRecordDate_overtimeRecordStartTime_overtimeRecordEndTime",
                table: "OvertimeRecords",
                columns: new[] { "employmentID", "overtimeRecordDate", "overtimeRecordStartTime", "overtimeRecordEndTime" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Loyalties_loyaltyName",
                table: "Loyalties",
                column: "loyaltyName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveTypes_leaveTypeName",
                table: "LeaveTypes",
                column: "leaveTypeName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Leaves_employmentID_leaveStartDate_leaveEndDate",
                table: "Leaves",
                columns: new[] { "employmentID", "leaveStartDate", "leaveEndDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_jobTitle",
                table: "Jobs",
                column: "jobTitle",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobPlacements_employeeID",
                table: "JobPlacements",
                column: "employeeID",
                unique: true,
                filter: "[jobPlacementStatus]=1");

            migrationBuilder.CreateIndex(
                name: "IX_JobPlacements_jobID_employeeID",
                table: "JobPlacements",
                columns: new[] { "jobID", "employeeID" },
                unique: true,
                filter: "[jobPlacementStatus]=1");

            migrationBuilder.CreateIndex(
                name: "IX_JobGrades_jobGradeName",
                table: "JobGrades",
                column: "jobGradeName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobCategories_jobCategoryName",
                table: "JobCategories",
                column: "jobCategoryName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Holidays_holidayName",
                table: "Holidays",
                column: "holidayName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentTypes_employmentTypeName",
                table: "EmploymentTypes",
                column: "employmentTypeName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employments_personID",
                table: "Employments",
                column: "personID",
                unique: true,
                filter: "[employmentStatus] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_EducationLevels_personModelpersonID",
                table: "EducationLevels",
                column: "educationLevelName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_companyID_departmentName",
                table: "Departments",
                columns: new[] { "companyID", "departmentName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_companyName",
                table: "Companies",
                column: "companyName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Breaks_breakEnd_breakStart",
                table: "Breaks",
                columns: new[] { "breakEnd", "breakStart" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankInfos_bankAccountNumber_personID_bankName",
                table: "BankInfos",
                columns: new[] { "bankAccountNumber", "personID", "bankName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Allowances_allowanceName",
                table: "Allowances",
                column: "allowanceName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AllowanceAssignments_allowanceID_employmentID",
                table: "AllowanceAssignments",
                columns: new[] { "allowanceID", "employmentID" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_workSiteModel_workSiteName",
                table: "workSiteModel");

            migrationBuilder.DropIndex(
                name: "IX_SubAccounts_subAccountName",
                table: "SubAccounts");

            migrationBuilder.DropIndex(
                name: "IX_Shifts_shiftName",
                table: "Shifts");

            migrationBuilder.DropIndex(
                name: "IX_Persons_personIDType_personIDNumber",
                table: "Persons");

            migrationBuilder.DropIndex(
                name: "IX_PersonEducationLevels_educationLevelID_personID",
                table: "PersonEducationLevels");

            migrationBuilder.DropIndex(
                name: "IX_Overtimes_overtimeName",
                table: "Overtimes");

            migrationBuilder.DropIndex(
                name: "IX_OvertimeRecords_employmentID_overtimeRecordDate_overtimeRecordStartTime_overtimeRecordEndTime",
                table: "OvertimeRecords");

            migrationBuilder.DropIndex(
                name: "IX_Loyalties_loyaltyName",
                table: "Loyalties");

            migrationBuilder.DropIndex(
                name: "IX_LeaveTypes_leaveTypeName",
                table: "LeaveTypes");

            migrationBuilder.DropIndex(
                name: "IX_Leaves_employmentID_leaveStartDate_leaveEndDate",
                table: "Leaves");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_jobTitle",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_JobPlacements_employeeID",
                table: "JobPlacements");

            migrationBuilder.DropIndex(
                name: "IX_JobPlacements_jobID_employeeID",
                table: "JobPlacements");

            migrationBuilder.DropIndex(
                name: "IX_JobGrades_jobGradeName",
                table: "JobGrades");

            migrationBuilder.DropIndex(
                name: "IX_JobCategories_jobCategoryName",
                table: "JobCategories");

            migrationBuilder.DropIndex(
                name: "IX_Holidays_holidayName",
                table: "Holidays");

            migrationBuilder.DropIndex(
                name: "IX_EmploymentTypes_employmentTypeName",
                table: "EmploymentTypes");

            migrationBuilder.DropIndex(
                name: "IX_Employments_personID",
                table: "Employments");

            migrationBuilder.DropIndex(
                name: "IX_EducationLevels_personModelpersonID",
                table: "EducationLevels");

            migrationBuilder.DropIndex(
                name: "IX_Departments_companyID_departmentName",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Companies_companyName",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Breaks_breakEnd_breakStart",
                table: "Breaks");

            migrationBuilder.DropIndex(
                name: "IX_BankInfos_bankAccountNumber_personID_bankName",
                table: "BankInfos");

            migrationBuilder.DropIndex(
                name: "IX_Allowances_allowanceName",
                table: "Allowances");

            migrationBuilder.DropIndex(
                name: "IX_AllowanceAssignments_allowanceID_employmentID",
                table: "AllowanceAssignments");

            migrationBuilder.DropColumn(
                name: "overtimeStatus",
                table: "OvertimeRecords");

            migrationBuilder.DropColumn(
                name: "allowanceStatus",
                table: "AllowanceAssignments");

            migrationBuilder.AlterColumn<string>(
                name: "workSiteName",
                table: "workSiteModel",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "subAccountName",
                table: "SubAccounts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "shiftName",
                table: "Shifts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "personIDType",
                table: "Persons",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "personIDNumber",
                table: "Persons",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "overtimeName",
                table: "Overtimes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "loyaltyName",
                table: "Loyalties",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "leaveTypeName",
                table: "LeaveTypes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "jobTitle",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "jobGradeName",
                table: "JobGrades",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "jobCategoryName",
                table: "JobCategories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "holidayName",
                table: "Holidays",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "employmentTypeName",
                table: "EmploymentTypes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "departmentName",
                table: "Departments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "companyName",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "bankName",
                table: "BankInfos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "bankAccountNumber",
                table: "BankInfos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "allowanceName",
                table: "Allowances",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Employments_personModelpersonID",
                table: "Employments",
                column: "personID");

            migrationBuilder.CreateIndex(
                name: "IX_EducationLevels_personModelpersonID",
                table: "EducationLevels",
                column: "educationLevelName");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_companyID",
                table: "Departments",
                column: "companyID");
        }
    }
}
