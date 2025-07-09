using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration49 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ServiceRequests_employmentID",
                table: "ServiceRequests");

            migrationBuilder.DropIndex(
                name: "IX_Prohibitions_employmentID",
                table: "Prohibitions");

            migrationBuilder.DropIndex(
                name: "IX_PersonHistories_personIDType_personIDNumber",
                table: "PersonHistories");

            migrationBuilder.DropIndex(
                name: "IX_PersonEducationLevels_educationLevelID_personID",
                table: "PersonEducationLevels");

            migrationBuilder.DropIndex(
                name: "IX_LoyaltyHistories_loyaltyID",
                table: "LoyaltyHistories");

            migrationBuilder.DropIndex(
                name: "IX_Guaranties_employmentID",
                table: "Guaranties");

            migrationBuilder.DropIndex(
                name: "IX_Employments_personID_givenID",
                table: "Employments");

            migrationBuilder.DropIndex(
                name: "IX_Breaks_breakEnd_breakStart",
                table: "Breaks");

            migrationBuilder.DropIndex(
                name: "IX_BankInfos_bankAccountNumber_personID_bankName",
                table: "BankInfos");

            migrationBuilder.DropIndex(
                name: "IX_AllowanceAssignments_allowanceID_employmentID",
                table: "AllowanceAssignments");

            migrationBuilder.RenameColumn(
                name: "experienceEndDate",
                table: "Expriences",
                newName: "exprienceEndDate");

            migrationBuilder.RenameColumn(
                name: "experienceId",
                table: "Expriences",
                newName: "exprienceID");

            migrationBuilder.RenameColumn(
                name: "banikInfoStatus",
                table: "BankInfos",
                newName: "bankInfoStatus");

            migrationBuilder.AlterColumn<string>(
                name: "userName",
                table: "Users",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "personIDType",
                table: "PersonHistories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "personIDNumber",
                table: "PersonHistories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "educationField",
                table: "PersonEducationLevels",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "jobStepName",
                table: "JobSteps",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "jobStepNumber",
                table: "JobSteps",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "guarantyType",
                table: "Guaranties",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "jobPlacementID",
                table: "Expriences",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "jobTitle",
                table: "Expriences",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "isPensionAllowed",
                table: "EmploymentTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "givenID",
                table: "Employments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "employmentMethodName",
                table: "EmploymentMethods",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "contractRemark",
                table: "Contracts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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

            migrationBuilder.CreateTable(
                name: "Families",
                columns: table => new
                {
                    familyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employmentID = table.Column<int>(type: "int", nullable: false),
                    personID = table.Column<int>(type: "int", nullable: false),
                    relation = table.Column<int>(type: "int", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Families", x => x.familyID);
                    table.ForeignKey(
                        name: "FK_Families_Employments_employmentID",
                        column: x => x.employmentID,
                        principalTable: "Employments",
                        principalColumn: "employmentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Families_Persons_personID",
                        column: x => x.personID,
                        principalTable: "Persons",
                        principalColumn: "personID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_userName",
                table: "Users",
                column: "userName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_shiftStart_shiftEnd",
                table: "Shifts",
                columns: new[] { "shiftStart", "shiftEnd" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_employmentID_requestedService",
                table: "ServiceRequests",
                columns: new[] { "employmentID", "requestedService" },
                filter: "serviceRequestStatus =1");

            migrationBuilder.CreateIndex(
                name: "IX_Prohibitions_employmentID_prohibitionStart_prohibitionEnd_prohibitionType",
                table: "Prohibitions",
                columns: new[] { "employmentID", "prohibitionStart", "prohibitionEnd", "prohibitionType" },
                unique: true,
                filter: "prohibitionStatus=1");

            migrationBuilder.CreateIndex(
                name: "IX_PersonEducationLevels_educationLevelID_personID_educationField",
                table: "PersonEducationLevels",
                columns: new[] { "educationLevelID", "personID", "educationField" },
                unique: true,
                filter: "[educationField] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LoyaltyHistories_loyaltyID_employmentID",
                table: "LoyaltyHistories",
                columns: new[] { "loyaltyID", "employmentID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobSteps_jobStepName",
                table: "JobSteps",
                column: "jobStepName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Guaranties_employmentID_guarantyType_guarantyStatus",
                table: "Guaranties",
                columns: new[] { "employmentID", "guarantyType", "guarantyStatus" },
                unique: true,
                filter: "guarantyStatus = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Employments_personID",
                table: "Employments",
                column: "personID",
                unique: true,
                filter: "[employmentStatus] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentMethods_employmentMethodName",
                table: "EmploymentMethods",
                column: "employmentMethodName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EducationLevels_educationLevelName",
                table: "EducationLevels",
                column: "educationLevelName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Breaks_breakEnd_breakStart_shiftID",
                table: "Breaks",
                columns: new[] { "breakEnd", "breakStart", "shiftID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AllowanceAssignments_allowanceID_employmentID_allowanceStatus",
                table: "AllowanceAssignments",
                columns: new[] { "allowanceID", "employmentID", "allowanceStatus" },
                unique: true,
                filter: "[allowanceStatus] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Families_employmentID",
                table: "Families",
                column: "employmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Families_personID_employmentID",
                table: "Families",
                columns: new[] { "personID", "employmentID" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Families");

            migrationBuilder.DropIndex(
                name: "IX_Users_userName",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Shifts_shiftStart_shiftEnd",
                table: "Shifts");

            migrationBuilder.DropIndex(
                name: "IX_ServiceRequests_employmentID_requestedService",
                table: "ServiceRequests");

            migrationBuilder.DropIndex(
                name: "IX_Prohibitions_employmentID_prohibitionStart_prohibitionEnd_prohibitionType",
                table: "Prohibitions");

            migrationBuilder.DropIndex(
                name: "IX_PersonEducationLevels_educationLevelID_personID_educationField",
                table: "PersonEducationLevels");

            migrationBuilder.DropIndex(
                name: "IX_LoyaltyHistories_loyaltyID_employmentID",
                table: "LoyaltyHistories");

            migrationBuilder.DropIndex(
                name: "IX_JobSteps_jobStepName",
                table: "JobSteps");

            migrationBuilder.DropIndex(
                name: "IX_Guaranties_employmentID_guarantyType_guarantyStatus",
                table: "Guaranties");

            migrationBuilder.DropIndex(
                name: "IX_Employments_personID",
                table: "Employments");

            migrationBuilder.DropIndex(
                name: "IX_EmploymentMethods_employmentMethodName",
                table: "EmploymentMethods");

            migrationBuilder.DropIndex(
                name: "IX_EducationLevels_educationLevelName",
                table: "EducationLevels");

            migrationBuilder.DropIndex(
                name: "IX_Breaks_breakEnd_breakStart_shiftID",
                table: "Breaks");

            migrationBuilder.DropIndex(
                name: "IX_AllowanceAssignments_allowanceID_employmentID_allowanceStatus",
                table: "AllowanceAssignments");

            migrationBuilder.DropColumn(
                name: "educationField",
                table: "PersonEducationLevels");

            migrationBuilder.DropColumn(
                name: "jobStepNumber",
                table: "JobSteps");

            migrationBuilder.DropColumn(
                name: "jobTitle",
                table: "Expriences");

            migrationBuilder.DropColumn(
                name: "isPensionAllowed",
                table: "EmploymentTypes");

            migrationBuilder.DropColumn(
                name: "contractRemark",
                table: "Contracts");

            migrationBuilder.RenameColumn(
                name: "exprienceEndDate",
                table: "Expriences",
                newName: "experienceEndDate");

            migrationBuilder.RenameColumn(
                name: "exprienceID",
                table: "Expriences",
                newName: "experienceId");

            migrationBuilder.RenameColumn(
                name: "bankInfoStatus",
                table: "BankInfos",
                newName: "banikInfoStatus");

            migrationBuilder.AlterColumn<string>(
                name: "userName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "personIDType",
                table: "PersonHistories",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "personIDNumber",
                table: "PersonHistories",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "jobStepName",
                table: "JobSteps",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "guarantyType",
                table: "Guaranties",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "jobPlacementID",
                table: "Expriences",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "givenID",
                table: "Employments",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "employmentMethodName",
                table: "EmploymentMethods",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

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

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_employmentID",
                table: "ServiceRequests",
                column: "employmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Prohibitions_employmentID",
                table: "Prohibitions",
                column: "employmentID");

            migrationBuilder.CreateIndex(
                name: "IX_PersonHistories_personIDType_personIDNumber",
                table: "PersonHistories",
                columns: new[] { "personIDType", "personIDNumber" },
                unique: true,
                filter: "[personIDType] IS NOT NULL AND [personIDNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PersonEducationLevels_educationLevelID_personID",
                table: "PersonEducationLevels",
                columns: new[] { "educationLevelID", "personID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoyaltyHistories_loyaltyID",
                table: "LoyaltyHistories",
                column: "loyaltyID");

            migrationBuilder.CreateIndex(
                name: "IX_Guaranties_employmentID",
                table: "Guaranties",
                column: "employmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Employments_personID_givenID",
                table: "Employments",
                columns: new[] { "personID", "givenID" },
                unique: true,
                filter: "[employmentStatus] = 1");

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
                name: "IX_AllowanceAssignments_allowanceID_employmentID",
                table: "AllowanceAssignments",
                columns: new[] { "allowanceID", "employmentID" },
                unique: true);
        }
    }
}
