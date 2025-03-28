using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration25 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "workSiteHistoryDate",
                table: "WorkSitesHistories",
                newName: "modifiedDate");

            migrationBuilder.RenameColumn(
                name: "overtimeUser",
                table: "OvertimeHistories",
                newName: "modifiedBy");

            migrationBuilder.RenameColumn(
                name: "overtimeHistoryDate",
                table: "OvertimeHistories",
                newName: "modifiedDate");

            migrationBuilder.RenameColumn(
                name: "loyaltyUser",
                table: "LoyaltyHistories",
                newName: "modifiedBy");

            migrationBuilder.RenameColumn(
                name: "leaveUser",
                table: "LeaveHistories",
                newName: "modifiedBy");

            migrationBuilder.RenameColumn(
                name: "leaveHistoryDate",
                table: "LeaveHistories",
                newName: "modifiedDate");

            migrationBuilder.RenameColumn(
                name: "employmentHistoryDate",
                table: "EmploymentHistories",
                newName: "modifiedDate");

            migrationBuilder.RenameColumn(
                name: "allowanceUser",
                table: "AllowanceAssignmentsHistories",
                newName: "modifiedBy");

            migrationBuilder.RenameColumn(
                name: "allowanceAssignmentHistoryDate",
                table: "AllowanceAssignmentsHistories",
                newName: "modifiedDate");

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "WorkSitesHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "workSiteModel",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "SubAccounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "Shifts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "Persons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "PersonEducationLevels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "Overtimes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "OvertimeRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "modifiedDate",
                table: "LoyaltyHistories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "Loyalties",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "LeaveTypes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "Leaves",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "JobPlacements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "JobGrades",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "JobClasses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "JobCategories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "Holidays",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "EmploymentTypes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "Employments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "EmploymentHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "EducationLevels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "Departments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "Breaks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "BankInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "Allowances",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "AllowanceAssignments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "JobRequirements",
                columns: table => new
                {
                    jobRequirementID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    departmentID = table.Column<int>(type: "int", nullable: false),
                    jobID = table.Column<int>(type: "int", nullable: false),
                    requiredNumber = table.Column<int>(type: "int", nullable: false),
                    jobRequirementStatus = table.Column<int>(type: "int", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    modifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobRequirements", x => x.jobRequirementID);
                    table.ForeignKey(
                        name: "FK_JobRequirements_Departments_departmentID",
                        column: x => x.departmentID,
                        principalTable: "Departments",
                        principalColumn: "departmentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobRequirements_Jobs_jobID",
                        column: x => x.jobID,
                        principalTable: "Jobs",
                        principalColumn: "jobID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    userID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    personID = table.Column<int>(type: "int", nullable: false),
                    userStatus = table.Column<int>(type: "int", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    modifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.userID);
                    table.ForeignKey(
                        name: "FK_Users_Persons_personID",
                        column: x => x.personID,
                        principalTable: "Persons",
                        principalColumn: "personID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobRequirementHistories",
                columns: table => new
                {
                    jobRequirementHistoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    jobRequirementID = table.Column<int>(type: "int", nullable: false),
                    requiredNumber = table.Column<int>(type: "int", nullable: false),
                    jobRequirementStatus = table.Column<int>(type: "int", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    modifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobRequirementHistories", x => x.jobRequirementHistoryID);
                    table.ForeignKey(
                        name: "FK_JobRequirementHistories_JobRequirements_jobRequirementID",
                        column: x => x.jobRequirementID,
                        principalTable: "JobRequirements",
                        principalColumn: "jobRequirementID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonHistories",
                columns: table => new
                {
                    personHistoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    personID = table.Column<int>(type: "int", nullable: false),
                    personFirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    personFatherName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    personLastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    personDoB = table.Column<DateTime>(type: "datetime2", nullable: false),
                    personGender = table.Column<int>(type: "int", nullable: false),
                    personIDType = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    personIDNumber = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    personPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    personRecordNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    addressID = table.Column<int>(type: "int", nullable: true),
                    personEmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    userModeluserID = table.Column<int>(type: "int", nullable: true),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    modifiedDate = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonHistories", x => x.personHistoryID);
                    table.ForeignKey(
                        name: "FK_PersonHistories_Addresses_addressID",
                        column: x => x.addressID,
                        principalTable: "Addresses",
                        principalColumn: "addressID");
                    table.ForeignKey(
                        name: "FK_PersonHistories_Persons_personID",
                        column: x => x.personID,
                        principalTable: "Persons",
                        principalColumn: "personID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonHistories_Users_userModeluserID",
                        column: x => x.userModeluserID,
                        principalTable: "Users",
                        principalColumn: "userID");
                });

            migrationBuilder.CreateTable(
                name: "UserHistories",
                columns: table => new
                {
                    userHistoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    userID = table.Column<int>(type: "int", nullable: false),
                    userStatus = table.Column<int>(type: "int", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    modifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserHistories", x => x.userHistoryID);
                    table.ForeignKey(
                        name: "FK_UserHistories_Users_userID",
                        column: x => x.userID,
                        principalTable: "Users",
                        principalColumn: "userID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobRequirementHistories_jobRequirementID",
                table: "JobRequirementHistories",
                column: "jobRequirementID");

            migrationBuilder.CreateIndex(
                name: "IX_JobRequirements_departmentID",
                table: "JobRequirements",
                column: "departmentID");

            migrationBuilder.CreateIndex(
                name: "IX_JobRequirements_jobID",
                table: "JobRequirements",
                column: "jobID");

            migrationBuilder.CreateIndex(
                name: "IX_PersonHistories_addressID",
                table: "PersonHistories",
                column: "addressID");

            migrationBuilder.CreateIndex(
                name: "IX_PersonHistories_personID",
                table: "PersonHistories",
                column: "personID");

            migrationBuilder.CreateIndex(
                name: "IX_PersonHistories_personIDType_personIDNumber",
                table: "PersonHistories",
                columns: new[] { "personIDType", "personIDNumber" },
                unique: true,
                filter: "[personIDType] IS NOT NULL AND [personIDNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PersonHistories_userModeluserID",
                table: "PersonHistories",
                column: "userModeluserID");

            migrationBuilder.CreateIndex(
                name: "IX_UserHistories_userID",
                table: "UserHistories",
                column: "userID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_personID",
                table: "Users",
                column: "personID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobRequirementHistories");

            migrationBuilder.DropTable(
                name: "PersonHistories");

            migrationBuilder.DropTable(
                name: "UserHistories");

            migrationBuilder.DropTable(
                name: "JobRequirements");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "WorkSitesHistories");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "workSiteModel");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "SubAccounts");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "PersonEducationLevels");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "Overtimes");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "OvertimeRecords");

            migrationBuilder.DropColumn(
                name: "modifiedDate",
                table: "LoyaltyHistories");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "Loyalties");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "LeaveTypes");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "Leaves");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "JobPlacements");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "JobGrades");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "JobClasses");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "JobCategories");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "Holidays");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "EmploymentTypes");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "Employments");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "EmploymentHistories");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "EducationLevels");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "Breaks");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "BankInfos");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "Allowances");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "AllowanceAssignments");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "modifiedDate",
                table: "WorkSitesHistories",
                newName: "workSiteHistoryDate");

            migrationBuilder.RenameColumn(
                name: "modifiedDate",
                table: "OvertimeHistories",
                newName: "overtimeHistoryDate");

            migrationBuilder.RenameColumn(
                name: "modifiedBy",
                table: "OvertimeHistories",
                newName: "overtimeUser");

            migrationBuilder.RenameColumn(
                name: "modifiedBy",
                table: "LoyaltyHistories",
                newName: "loyaltyUser");

            migrationBuilder.RenameColumn(
                name: "modifiedDate",
                table: "LeaveHistories",
                newName: "leaveHistoryDate");

            migrationBuilder.RenameColumn(
                name: "modifiedBy",
                table: "LeaveHistories",
                newName: "leaveUser");

            migrationBuilder.RenameColumn(
                name: "modifiedDate",
                table: "EmploymentHistories",
                newName: "employmentHistoryDate");

            migrationBuilder.RenameColumn(
                name: "modifiedDate",
                table: "AllowanceAssignmentsHistories",
                newName: "allowanceAssignmentHistoryDate");

            migrationBuilder.RenameColumn(
                name: "modifiedBy",
                table: "AllowanceAssignmentsHistories",
                newName: "allowanceUser");
        }
    }
}
