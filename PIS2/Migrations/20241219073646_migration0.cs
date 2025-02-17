using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    accountID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    accountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    accountName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    accountDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    accountStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.accountID);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    addressID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    addressCountry = table.Column<int>(type: "int", nullable: false),
                    addressRegion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    addressZone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    addressWoreda = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    addressTabya = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    addressStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.addressID);
                });

            migrationBuilder.CreateTable(
                name: "Allowances",
                columns: table => new
                {
                    allowanceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    allowanceName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    allowanceDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    allowanceAmount = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    allowanceTaxable = table.Column<bool>(type: "bit", nullable: false),
                    allowanceStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Allowances", x => x.allowanceID);
                });

            migrationBuilder.CreateTable(
                name: "EducationLevels",
                columns: table => new
                {
                    educationLevelID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    educationLevelName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    educationLevelGrade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    educationLevelCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    educationLevelDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    educationLevelStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationLevels", x => x.educationLevelID);
                });

            migrationBuilder.CreateTable(
                name: "EmploymentTypes",
                columns: table => new
                {
                    employmentTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employmentTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    employmentTypeDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isLeaveCount = table.Column<bool>(type: "bit", nullable: false),
                    isExprienceCount = table.Column<bool>(type: "bit", nullable: false),
                    isSalaryAllowed = table.Column<bool>(type: "bit", nullable: false),
                    isLoyalityAllowed = table.Column<bool>(type: "bit", nullable: false),
                    isSeveranceAllowed = table.Column<bool>(type: "bit", nullable: false),
                    employmentTypeStatus = table.Column<int>(type: "int", nullable: false),
                    annualAccrualRate = table.Column<double>(type: "float", nullable: false),
                    employmentBaseLeave = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmploymentTypes", x => x.employmentTypeID);
                });

            migrationBuilder.CreateTable(
                name: "Holidays",
                columns: table => new
                {
                    holidayID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    holidayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    holidayType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    holidayStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    holidayEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    holidayRepition = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    holidayStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holidays", x => x.holidayID);
                });

            migrationBuilder.CreateTable(
                name: "JobCategories",
                columns: table => new
                {
                    jobCategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    jobCategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    jobCategoryDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    jobCategoryStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobCategories", x => x.jobCategoryID);
                });

            migrationBuilder.CreateTable(
                name: "JobClasses",
                columns: table => new
                {
                    JobClassId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobClassName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JobClassDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JobClasStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobClasses", x => x.JobClassId);
                });

            migrationBuilder.CreateTable(
                name: "JobGrades",
                columns: table => new
                {
                    jobGradeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    jobGradeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    jobGradeDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    jobGradeBasicSalary = table.Column<double>(type: "float", nullable: false),
                    jobGradeStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobGrades", x => x.jobGradeID);
                });

            migrationBuilder.CreateTable(
                name: "LeaveTypes",
                columns: table => new
                {
                    leaveTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    leaveTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    leaveTypeImpact = table.Column<int>(type: "int", nullable: false),
                    leaveTypeStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveTypes", x => x.leaveTypeID);
                });

            migrationBuilder.CreateTable(
                name: "Loyalties",
                columns: table => new
                {
                    loyaltyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    loyaltyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    loyaltyAmount = table.Column<double>(type: "float", nullable: false),
                    loyaltyCounter = table.Column<double>(type: "float", nullable: false),
                    loyaltyStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loyalties", x => x.loyaltyID);
                });

            migrationBuilder.CreateTable(
                name: "Overtimes",
                columns: table => new
                {
                    overtimeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    overtimeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    overtimeDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    overtimeStatus = table.Column<int>(type: "int", nullable: false),
                    overtimeRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Overtimes", x => x.overtimeID);
                });

            migrationBuilder.CreateTable(
                name: "Shifts",
                columns: table => new
                {
                    shiftID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    shiftName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    shiftStart = table.Column<TimeSpan>(type: "time", nullable: false),
                    shiftEnd = table.Column<TimeSpan>(type: "time", nullable: false),
                    shiftStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shifts", x => x.shiftID);
                });

            migrationBuilder.CreateTable(
                name: "SubAccounts",
                columns: table => new
                {
                    subAccountID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    accountID = table.Column<int>(type: "int", nullable: false),
                    subAccountName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    subAccountDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    subAccountStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubAccounts", x => x.subAccountID);
                    table.ForeignKey(
                        name: "FK_SubAccounts_Accounts_accountID",
                        column: x => x.accountID,
                        principalTable: "Accounts",
                        principalColumn: "accountID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    personID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    personFirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    personFatherName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    personLastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    personDoB = table.Column<DateTime>(type: "datetime2", nullable: false),
                    personGender = table.Column<int>(type: "int", nullable: false),
                    personIDType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    personIDNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    personPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    personRecordNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    addressID = table.Column<int>(type: "int", nullable: true),
                    personEmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.personID);
                    table.ForeignKey(
                        name: "FK_Persons_Addresses_addressID",
                        column: x => x.addressID,
                        principalTable: "Addresses",
                        principalColumn: "addressID");
                });

            migrationBuilder.CreateTable(
                name: "workSiteModel",
                columns: table => new
                {
                    workSiteID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    workSiteName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    workSiteNature = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    workSiteEstablishDate = table.Column<DateOnly>(type: "date", nullable: false),
                    worksiteStatus = table.Column<int>(type: "int", nullable: false),
                    addressID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workSiteModel", x => x.workSiteID);
                    table.ForeignKey(
                        name: "FK_workSiteModel_Addresses_addressID",
                        column: x => x.addressID,
                        principalTable: "Addresses",
                        principalColumn: "addressID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    jobID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    jobTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    jobGrade = table.Column<int>(type: "int", nullable: false),
                    jobCategoryID = table.Column<int>(type: "int", nullable: false),
                    jobClassID = table.Column<int>(type: "int", nullable: false),
                    jobDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    jobStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.jobID);
                    table.ForeignKey(
                        name: "FK_Jobs_JobCategories_jobCategoryID",
                        column: x => x.jobCategoryID,
                        principalTable: "JobCategories",
                        principalColumn: "jobCategoryID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Jobs_JobClasses_jobClassID",
                        column: x => x.jobClassID,
                        principalTable: "JobClasses",
                        principalColumn: "JobClassId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Jobs_JobGrades_jobGrade",
                        column: x => x.jobGrade,
                        principalTable: "JobGrades",
                        principalColumn: "jobGradeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Breaks",
                columns: table => new
                {
                    breakID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    shiftID = table.Column<int>(type: "int", nullable: false),
                    breakStart = table.Column<TimeSpan>(type: "time", nullable: false),
                    breakEnd = table.Column<TimeSpan>(type: "time", nullable: false),
                    breakName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    breakStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Breaks", x => x.breakID);
                    table.ForeignKey(
                        name: "FK_Breaks_Shifts_shiftID",
                        column: x => x.shiftID,
                        principalTable: "Shifts",
                        principalColumn: "shiftID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BankInfos",
                columns: table => new
                {
                    bankInfoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    personID = table.Column<int>(type: "int", nullable: false),
                    bankName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    bankAccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    bankBranch = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    banikInfoStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankInfos", x => x.bankInfoID);
                    table.ForeignKey(
                        name: "FK_BankInfos_Persons_personID",
                        column: x => x.personID,
                        principalTable: "Persons",
                        principalColumn: "personID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employments",
                columns: table => new
                {
                    employmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    givenID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    personID = table.Column<int>(type: "int", nullable: false),
                    employmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    employmentStatus = table.Column<int>(type: "int", nullable: false),
                    employmentReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    workingHoursPerWeek = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employments", x => x.employmentID);
                    table.ForeignKey(
                        name: "FK_Employments_Persons_personID",
                        column: x => x.personID,
                        principalTable: "Persons",
                        principalColumn: "personID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonEducationLevels",
                columns: table => new
                {
                    personEducationLevelID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    personID = table.Column<int>(type: "int", nullable: false),
                    educationLevelID = table.Column<int>(type: "int", nullable: false),
                    educationLevelDate = table.Column<DateOnly>(type: "date", nullable: false),
                    educationLevelMark = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    educationLevelInstitutionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    educationLevelNumber = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonEducationLevels", x => x.personEducationLevelID);
                    table.ForeignKey(
                        name: "FK_PersonEducationLevels_EducationLevels_educationLevelID",
                        column: x => x.educationLevelID,
                        principalTable: "EducationLevels",
                        principalColumn: "educationLevelID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonEducationLevels_Persons_personID",
                        column: x => x.personID,
                        principalTable: "Persons",
                        principalColumn: "personID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkSitesHistories",
                columns: table => new
                {
                    workSiteHistoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    workSiteID = table.Column<int>(type: "int", nullable: false),
                    workSiteHistoryAction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    workSiteHistoryDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkSitesHistories", x => x.workSiteHistoryID);
                    table.ForeignKey(
                        name: "FK_WorkSitesHistories_workSiteModel_workSiteID",
                        column: x => x.workSiteID,
                        principalTable: "workSiteModel",
                        principalColumn: "workSiteID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "educationLevelModeljobModel",
                columns: table => new
                {
                    EducationLevelseducationLevelID = table.Column<int>(type: "int", nullable: false),
                    JobsjobID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_educationLevelModeljobModel", x => new { x.EducationLevelseducationLevelID, x.JobsjobID });
                    table.ForeignKey(
                        name: "FK_educationLevelModeljobModel_EducationLevels_EducationLevelseducationLevelID",
                        column: x => x.EducationLevelseducationLevelID,
                        principalTable: "EducationLevels",
                        principalColumn: "educationLevelID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_educationLevelModeljobModel_Jobs_JobsjobID",
                        column: x => x.JobsjobID,
                        principalTable: "Jobs",
                        principalColumn: "jobID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AllowanceAssignments",
                columns: table => new
                {
                    allowanceAssignmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    allowanceID = table.Column<int>(type: "int", nullable: false),
                    allowanceAssignmentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    employmentID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllowanceAssignments", x => x.allowanceAssignmentID);
                    table.ForeignKey(
                        name: "FK_AllowanceAssignments_Allowances_allowanceID",
                        column: x => x.allowanceID,
                        principalTable: "Allowances",
                        principalColumn: "allowanceID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AllowanceAssignments_Employments_employmentID",
                        column: x => x.employmentID,
                        principalTable: "Employments",
                        principalColumn: "employmentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    companyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    companyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    companyShort = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    employmentID = table.Column<int>(type: "int", nullable: true),
                    employmentModelemploymentID = table.Column<int>(type: "int", nullable: true),
                    addressID = table.Column<int>(type: "int", nullable: true),
                    companyStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.companyID);
                    table.ForeignKey(
                        name: "FK_Companies_Addresses_addressID",
                        column: x => x.addressID,
                        principalTable: "Addresses",
                        principalColumn: "addressID");
                    table.ForeignKey(
                        name: "FK_Companies_Employments_employmentModelemploymentID",
                        column: x => x.employmentModelemploymentID,
                        principalTable: "Employments",
                        principalColumn: "employmentID");
                });

            migrationBuilder.CreateTable(
                name: "EmploymentHistories",
                columns: table => new
                {
                    employmentHistoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employmentID = table.Column<int>(type: "int", nullable: false),
                    employmentHistoryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    employmentTypeID = table.Column<int>(type: "int", nullable: false),
                    employmentrHistoryRemark = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmploymentHistories", x => x.employmentHistoryID);
                    table.ForeignKey(
                        name: "FK_EmploymentHistories_EmploymentTypes_employmentTypeID",
                        column: x => x.employmentTypeID,
                        principalTable: "EmploymentTypes",
                        principalColumn: "employmentTypeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmploymentHistories_Employments_employmentID",
                        column: x => x.employmentID,
                        principalTable: "Employments",
                        principalColumn: "employmentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Leaves",
                columns: table => new
                {
                    leaveID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employmentID = table.Column<int>(type: "int", nullable: false),
                    leaveReaquestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    leaveStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    leaveEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    leaveDays = table.Column<double>(type: "float", nullable: false),
                    leaveTypeID = table.Column<int>(type: "int", nullable: false),
                    leaveStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leaves", x => x.leaveID);
                    table.ForeignKey(
                        name: "FK_Leaves_Employments_employmentID",
                        column: x => x.employmentID,
                        principalTable: "Employments",
                        principalColumn: "employmentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Leaves_LeaveTypes_leaveTypeID",
                        column: x => x.leaveTypeID,
                        principalTable: "LeaveTypes",
                        principalColumn: "leaveTypeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoyaltyHistories",
                columns: table => new
                {
                    loyaltyHistoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employmentID = table.Column<int>(type: "int", nullable: false),
                    loyaltyID = table.Column<int>(type: "int", nullable: false),
                    loyaltyAmount = table.Column<double>(type: "float", nullable: false),
                    loyaltyHistoryStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoyaltyHistories", x => x.loyaltyHistoryID);
                    table.ForeignKey(
                        name: "FK_LoyaltyHistories_Employments_employmentID",
                        column: x => x.employmentID,
                        principalTable: "Employments",
                        principalColumn: "employmentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LoyaltyHistories_Loyalties_loyaltyID",
                        column: x => x.loyaltyID,
                        principalTable: "Loyalties",
                        principalColumn: "loyaltyID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OvertimeRecords",
                columns: table => new
                {
                    overtimeRecordID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employmentID = table.Column<int>(type: "int", nullable: false),
                    overtimeID = table.Column<int>(type: "int", nullable: false),
                    overtimeRecordDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    overtimeRecordStartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    overtimeRecordEndTime = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OvertimeRecords", x => x.overtimeRecordID);
                    table.ForeignKey(
                        name: "FK_OvertimeRecords_Employments_employmentID",
                        column: x => x.employmentID,
                        principalTable: "Employments",
                        principalColumn: "employmentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OvertimeRecords_Overtimes_overtimeID",
                        column: x => x.overtimeID,
                        principalTable: "Overtimes",
                        principalColumn: "overtimeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AllowanceAssignmentsHistories",
                columns: table => new
                {
                    allowanceAssignmentHistoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    allowanceAssignmentID = table.Column<int>(type: "int", nullable: false),
                    allowanceAssignmentHistoryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    allowanceAssignmentHistoryStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllowanceAssignmentsHistories", x => x.allowanceAssignmentHistoryID);
                    table.ForeignKey(
                        name: "FK_AllowanceAssignmentsHistories_AllowanceAssignments_allowanceAssignmentID",
                        column: x => x.allowanceAssignmentID,
                        principalTable: "AllowanceAssignments",
                        principalColumn: "allowanceAssignmentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    departmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    departmentShort = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    departmentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    departmentStatus = table.Column<int>(type: "int", nullable: false),
                    subAccountID = table.Column<int>(type: "int", nullable: true),
                    companyID = table.Column<int>(type: "int", nullable: false),
                    employmentID = table.Column<int>(type: "int", nullable: true),
                    employmentModelemploymentID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.departmentID);
                    table.ForeignKey(
                        name: "FK_Departments_Companies_companyID",
                        column: x => x.companyID,
                        principalTable: "Companies",
                        principalColumn: "companyID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Departments_Employments_employmentModelemploymentID",
                        column: x => x.employmentModelemploymentID,
                        principalTable: "Employments",
                        principalColumn: "employmentID");
                    table.ForeignKey(
                        name: "FK_Departments_SubAccounts_subAccountID",
                        column: x => x.subAccountID,
                        principalTable: "SubAccounts",
                        principalColumn: "subAccountID");
                });

            migrationBuilder.CreateTable(
                name: "LeaveHistories",
                columns: table => new
                {
                    leaveHistoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    leaveID = table.Column<int>(type: "int", nullable: false),
                    leaveHistoryAction = table.Column<int>(type: "int", nullable: false),
                    leaveHistoryDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveHistories", x => x.leaveHistoryID);
                    table.ForeignKey(
                        name: "FK_LeaveHistories_Leaves_leaveID",
                        column: x => x.leaveID,
                        principalTable: "Leaves",
                        principalColumn: "leaveID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OvertimeHistories",
                columns: table => new
                {
                    overtimeHistoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    overtimeRecordID = table.Column<int>(type: "int", nullable: false),
                    overtimeHistoryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    overtimeHistoryAction = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OvertimeHistories", x => x.overtimeHistoryID);
                    table.ForeignKey(
                        name: "FK_OvertimeHistories_OvertimeRecords_overtimeRecordID",
                        column: x => x.overtimeRecordID,
                        principalTable: "OvertimeRecords",
                        principalColumn: "overtimeRecordID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobPlacements",
                columns: table => new
                {
                    jobPlacementID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employeeID = table.Column<int>(type: "int", nullable: false),
                    departmentID = table.Column<int>(type: "int", nullable: false),
                    jobID = table.Column<int>(type: "int", nullable: false),
                    shiftID = table.Column<int>(type: "int", nullable: true),
                    jobPlacementSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    jobPlacementDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    jobPlacementStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobPlacements", x => x.jobPlacementID);
                    table.ForeignKey(
                        name: "FK_JobPlacements_Departments_departmentID",
                        column: x => x.departmentID,
                        principalTable: "Departments",
                        principalColumn: "departmentID");
                    table.ForeignKey(
                        name: "FK_JobPlacements_Employments_employeeID",
                        column: x => x.employeeID,
                        principalTable: "Employments",
                        principalColumn: "employmentID");
                    table.ForeignKey(
                        name: "FK_JobPlacements_Jobs_jobID",
                        column: x => x.jobID,
                        principalTable: "Jobs",
                        principalColumn: "jobID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobPlacements_Shifts_shiftID",
                        column: x => x.shiftID,
                        principalTable: "Shifts",
                        principalColumn: "shiftID");
                });

            migrationBuilder.CreateTable(
                name: "Expriences",
                columns: table => new
                {
                    experienceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    personID = table.Column<int>(type: "int", nullable: false),
                    personModelpersonID = table.Column<int>(type: "int", nullable: false),
                    experienceStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    experienceEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    jobPlacementID = table.Column<int>(type: "int", nullable: false),
                    jobPlacementModeljobPlacementID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expriences", x => x.experienceId);
                    table.ForeignKey(
                        name: "FK_Expriences_JobPlacements_jobPlacementModeljobPlacementID",
                        column: x => x.jobPlacementModeljobPlacementID,
                        principalTable: "JobPlacements",
                        principalColumn: "jobPlacementID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Expriences_Persons_personModelpersonID",
                        column: x => x.personModelpersonID,
                        principalTable: "Persons",
                        principalColumn: "personID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AllowanceAssignments_allowanceModelallowanceID",
                table: "AllowanceAssignments",
                column: "allowanceID");

            migrationBuilder.CreateIndex(
                name: "IX_AllowanceAssignments_employmentID",
                table: "AllowanceAssignments",
                column: "employmentID");

            migrationBuilder.CreateIndex(
                name: "IX_AllowanceAssignmentsHistories_allowanceAssignmentID",
                table: "AllowanceAssignmentsHistories",
                column: "allowanceAssignmentID");

            migrationBuilder.CreateIndex(
                name: "IX_BankInfos_personModelpersonID",
                table: "BankInfos",
                column: "personID");

            migrationBuilder.CreateIndex(
                name: "IX_Breaks_shiftModelshiftID",
                table: "Breaks",
                column: "shiftID");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_addressID",
                table: "Companies",
                column: "addressID");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_employmentModelemploymentID",
                table: "Companies",
                column: "employmentModelemploymentID");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_companyID",
                table: "Departments",
                column: "companyID");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_employmentModelemploymentID",
                table: "Departments",
                column: "employmentModelemploymentID");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_employmentModelemploymentID1",
                table: "Departments",
                column: "employmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_subAccountModelsubAccountID",
                table: "Departments",
                column: "subAccountID");

            migrationBuilder.CreateIndex(
                name: "IX_educationLevelModeljobModel_JobsjobID",
                table: "educationLevelModeljobModel",
                column: "JobsjobID");

            migrationBuilder.CreateIndex(
                name: "IX_EducationLevels_personModelpersonID",
                table: "EducationLevels",
                column: "educationLevelName");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentHistories_employmentID",
                table: "EmploymentHistories",
                column: "employmentID");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentHistories_employmentTypeID",
                table: "EmploymentHistories",
                column: "employmentTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Employments_employmentTypeModelemploymentTypeID",
                table: "Employments",
                column: "employmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Employments_personModelpersonID",
                table: "Employments",
                column: "personID");

            migrationBuilder.CreateIndex(
                name: "IX_Expriences_jobPlacementModeljobPlacementID",
                table: "Expriences",
                column: "jobPlacementModeljobPlacementID");

            migrationBuilder.CreateIndex(
                name: "IX_Expriences_personModelpersonID",
                table: "Expriences",
                column: "personModelpersonID");

            migrationBuilder.CreateIndex(
                name: "IX_JobPlacements_departmentModeldepartmentID",
                table: "JobPlacements",
                column: "departmentID");

            migrationBuilder.CreateIndex(
                name: "IX_JobPlacements_employmentModelemploymentID",
                table: "JobPlacements",
                column: "employeeID");

            migrationBuilder.CreateIndex(
                name: "IX_JobPlacements_jobModeljobID",
                table: "JobPlacements",
                column: "jobID");

            migrationBuilder.CreateIndex(
                name: "IX_JobPlacements_shiftID",
                table: "JobPlacements",
                column: "shiftID");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_jobCategoryModeljobCategoryID",
                table: "Jobs",
                column: "jobCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_jobClassModelJobClassId",
                table: "Jobs",
                column: "jobClassID");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_jobGrade",
                table: "Jobs",
                column: "jobGrade");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveHistories_leaveID",
                table: "LeaveHistories",
                column: "leaveID");

            migrationBuilder.CreateIndex(
                name: "IX_Leaves_employmentModelemploymentID",
                table: "Leaves",
                column: "employmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Leaves_leaveTypeModelleaveTypeID",
                table: "Leaves",
                column: "leaveTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_LoyaltyHistories_employmentID",
                table: "LoyaltyHistories",
                column: "employmentID");

            migrationBuilder.CreateIndex(
                name: "IX_LoyaltyHistories_loyaltyID",
                table: "LoyaltyHistories",
                column: "loyaltyID");

            migrationBuilder.CreateIndex(
                name: "IX_OvertimeHistories_overtimeRecordID",
                table: "OvertimeHistories",
                column: "overtimeRecordID");

            migrationBuilder.CreateIndex(
                name: "IX_OvertimeRecords_employmentModelemploymentID",
                table: "OvertimeRecords",
                column: "employmentID");

            migrationBuilder.CreateIndex(
                name: "IX_OvertimeRecords_overtimeModelovertimeID",
                table: "OvertimeRecords",
                column: "overtimeID");

            migrationBuilder.CreateIndex(
                name: "IX_PersonEducationLevels_educationLevelModeleducationLevelID",
                table: "PersonEducationLevels",
                column: "educationLevelID");

            migrationBuilder.CreateIndex(
                name: "IX_PersonEducationLevels_personModelpersonID",
                table: "PersonEducationLevels",
                column: "personID");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_addressModeladdressID",
                table: "Persons",
                column: "addressID");

            migrationBuilder.CreateIndex(
                name: "IX_SubAccounts_accountID",
                table: "SubAccounts",
                column: "accountID");

            migrationBuilder.CreateIndex(
                name: "IX_SubAccounts_accountModelaccountID",
                table: "SubAccounts",
                column: "subAccountID");

            migrationBuilder.CreateIndex(
                name: "IX_workSiteModel_AddressModeladdressID",
                table: "workSiteModel",
                column: "addressID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkSitesHistories_workSiteID",
                table: "WorkSitesHistories",
                column: "workSiteID");
            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AllowanceAssignmentsHistories");

            migrationBuilder.DropTable(
                name: "BankInfos");

            migrationBuilder.DropTable(
                name: "Breaks");

            migrationBuilder.DropTable(
                name: "educationLevelModeljobModel");

            migrationBuilder.DropTable(
                name: "EmploymentHistories");

            migrationBuilder.DropTable(
                name: "Expriences");

            migrationBuilder.DropTable(
                name: "Holidays");

            migrationBuilder.DropTable(
                name: "LeaveHistories");

            migrationBuilder.DropTable(
                name: "LoyaltyHistories");

            migrationBuilder.DropTable(
                name: "OvertimeHistories");

            migrationBuilder.DropTable(
                name: "PersonEducationLevels");

            migrationBuilder.DropTable(
                name: "WorkSitesHistories");

            migrationBuilder.DropTable(
                name: "AllowanceAssignments");

            migrationBuilder.DropTable(
                name: "EmploymentTypes");

            migrationBuilder.DropTable(
                name: "JobPlacements");

            migrationBuilder.DropTable(
                name: "Leaves");

            migrationBuilder.DropTable(
                name: "Loyalties");

            migrationBuilder.DropTable(
                name: "OvertimeRecords");

            migrationBuilder.DropTable(
                name: "EducationLevels");

            migrationBuilder.DropTable(
                name: "workSiteModel");

            migrationBuilder.DropTable(
                name: "Allowances");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "Shifts");

            migrationBuilder.DropTable(
                name: "LeaveTypes");

            migrationBuilder.DropTable(
                name: "Overtimes");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "SubAccounts");

            migrationBuilder.DropTable(
                name: "JobCategories");

            migrationBuilder.DropTable(
                name: "JobClasses");

            migrationBuilder.DropTable(
                name: "JobGrades");

            migrationBuilder.DropTable(
                name: "Employments");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "Addresses");
        }
    }
}
