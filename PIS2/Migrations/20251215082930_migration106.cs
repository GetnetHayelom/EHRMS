using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration106 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accesses_Companies_companyID",
                table: "Accesses");

            migrationBuilder.DropForeignKey(
                name: "FK_Accesses_Users_userID",
                table: "Accesses");

            migrationBuilder.DropForeignKey(
                name: "FK_AccessHistories_Accesses_accessID",
                table: "AccessHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_AllowanceAssignments_Allowances_allowanceID",
                table: "AllowanceAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_AllowanceAssignments_Employments_employmentID",
                table: "AllowanceAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_AllowanceAssignmentsHistories_AllowanceAssignments_allowanceAssignmentID",
                table: "AllowanceAssignmentsHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_BankInfos_Persons_personID",
                table: "BankInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_Breaks_Shifts_shiftID",
                table: "Breaks");

            migrationBuilder.DropForeignKey(
                name: "FK_ContractHistories_Contracts_contractID",
                table: "ContractHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Employments_employmentID",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_DeductionRecords_DeductionTypes_deductionTypeID",
                table: "DeductionRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_DeductionRecords_PayrollPays_payrollPayID",
                table: "DeductionRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_DelegationHistories_Delegations_delegationID",
                table: "DelegationHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Delegations_Employments_delegationFrom",
                table: "Delegations");

            migrationBuilder.DropForeignKey(
                name: "FK_Delegations_Employments_delegationTo",
                table: "Delegations");

            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentHistories_Departments_departmentID",
                table: "DepartmentHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Companies_companyID",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_EarningRecords_EarningTypes_earningTypeId",
                table: "EarningRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_EarningRecords_PayrollPays_payrollPayID",
                table: "EarningRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_EmploymentHistories_EmploymentTypes_employmentTypeID",
                table: "EmploymentHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_EmploymentHistories_Employments_employmentID",
                table: "EmploymentHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_EmploymentMethodHistories_EmploymentMethods_employmentMethodID",
                table: "EmploymentMethodHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_EmploymentRequestHistories_EmploymentRequests_employmentRequestID",
                table: "EmploymentRequestHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_EmploymentRequests_EmploymentTypes_employmentTypeID",
                table: "EmploymentRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_EmploymentRequests_Jobs_jobID",
                table: "EmploymentRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Employments_Persons_personID",
                table: "Employments");

            migrationBuilder.DropForeignKey(
                name: "FK_Experiences_Persons_personID",
                table: "Experiences");

            migrationBuilder.DropForeignKey(
                name: "FK_Families_Persons_personID",
                table: "Families");

            migrationBuilder.DropForeignKey(
                name: "FK_Guaranties_Employments_employmentID",
                table: "Guaranties");

            migrationBuilder.DropForeignKey(
                name: "FK_GuarantyHistories_Guaranties_guarantyID",
                table: "GuarantyHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_JobPlacementHistories_JobPlacements_jobPlacementID",
                table: "JobPlacementHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_JobPlacements_Departments_departmentID",
                table: "JobPlacements");

            migrationBuilder.DropForeignKey(
                name: "FK_JobPlacements_Employments_employmentID",
                table: "JobPlacements");

            migrationBuilder.DropForeignKey(
                name: "FK_JobReqCosts_JobRequirements_jobRequirementID",
                table: "JobReqCosts");

            migrationBuilder.DropForeignKey(
                name: "FK_JobRequirementHistories_JobRequirements_jobRequirementID",
                table: "JobRequirementHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_JobRequirements_Departments_departmentID",
                table: "JobRequirements");

            migrationBuilder.DropForeignKey(
                name: "FK_JobRequirements_Jobs_jobID",
                table: "JobRequirements");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_JobCategories_jobCategoryID",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_JobClasses_jobClassID",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_JobGrades_jobGradeID",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_JobStepHistories_JobSteps_jobStepID",
                table: "JobStepHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_JobSteps_JobGrades_jobGradeID",
                table: "JobSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveHistories_Leaves_leaveID",
                table: "LeaveHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Leaves_Employments_employmentID",
                table: "Leaves");

            migrationBuilder.DropForeignKey(
                name: "FK_Leaves_LeaveTypes_leaveTypeID",
                table: "Leaves");

            migrationBuilder.DropForeignKey(
                name: "FK_LoyaltyHistories_Employments_employmentID",
                table: "LoyaltyHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_LoyaltyHistories_Loyalties_loyaltyID",
                table: "LoyaltyHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_OvertimeHistories_OvertimeRecords_overtimeRecordID",
                table: "OvertimeHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_OvertimeRecords_Departments_departmentID",
                table: "OvertimeRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_OvertimeRecords_Employments_employmentID",
                table: "OvertimeRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_OvertimeRecords_Overtimes_overtimeID",
                table: "OvertimeRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollHistories_Payrolls_payrollID",
                table: "PayrollHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollPays_Employments_employmentID",
                table: "PayrollPays");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollPays_Payrolls_payrollID",
                table: "PayrollPays");

            migrationBuilder.DropForeignKey(
                name: "FK_Penalties_Employments_employmentID",
                table: "Penalties");

            migrationBuilder.DropForeignKey(
                name: "FK_Penalties_PenaltyTypes_penaltyTypeID",
                table: "Penalties");

            migrationBuilder.DropForeignKey(
                name: "FK_PenaltyHistories_Penalties_penaltyID",
                table: "PenaltyHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonEducationLevels_EducationLevels_educationLevelID",
                table: "PersonEducationLevels");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonEducationLevels_Persons_personID",
                table: "PersonEducationLevels");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonHistories_Persons_personID",
                table: "PersonHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_prohibitionHistories_Prohibitions_prohibitionID",
                table: "prohibitionHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Prohibitions_Employments_employmentID",
                table: "Prohibitions");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequestHistories_ServiceRequests_serviceRequestID",
                table: "ServiceRequestHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequests_Employments_employmentID",
                table: "ServiceRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShiftAssignments_Employments_employmentID",
                table: "ShiftAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_ShiftAssignments_Shifts_shiftID",
                table: "ShiftAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_SiteAssignments_Employments_employmentID",
                table: "SiteAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_SiteAssignments_workSiteModel_workSiteID",
                table: "SiteAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_StructureHistories_Structures_structureID",
                table: "StructureHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Structures_Departments_departmentID",
                table: "Structures");

            migrationBuilder.DropForeignKey(
                name: "FK_Structures_Jobs_jobID",
                table: "Structures");

            migrationBuilder.DropForeignKey(
                name: "FK_Structures_Structures_reportsTo",
                table: "Structures");

            migrationBuilder.DropForeignKey(
                name: "FK_SubAccounts_Accounts_accountID",
                table: "SubAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Terminations_Employments_employmentID",
                table: "Terminations");

            migrationBuilder.DropForeignKey(
                name: "FK_UserHistories_Users_userID",
                table: "UserHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Persons_personID",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_workSiteModel_Addresses_addressID",
                table: "workSiteModel");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkSitesHistories_workSiteModel_workSiteID",
                table: "WorkSitesHistories");

            migrationBuilder.RenameColumn(
                name: "workSiteName",
                table: "WorkSitesHistories",
                newName: "worksiteName");

            migrationBuilder.RenameColumn(
                name: "worksiteStatus",
                table: "workSiteModel",
                newName: "workSiteStatus");

            migrationBuilder.AddColumn<int>(
                name: "maxEarlyIn",
                table: "Shifts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "maxEarlyOut",
                table: "Shifts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "maxLateIn",
                table: "Shifts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "maxLateOut",
                table: "Shifts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Accesses_Companies_companyID",
                table: "Accesses",
                column: "companyID",
                principalTable: "Companies",
                principalColumn: "companyID");

            migrationBuilder.AddForeignKey(
                name: "FK_Accesses_Users_userID",
                table: "Accesses",
                column: "userID",
                principalTable: "Users",
                principalColumn: "userID");

            migrationBuilder.AddForeignKey(
                name: "FK_AccessHistories_Accesses_accessID",
                table: "AccessHistories",
                column: "accessID",
                principalTable: "Accesses",
                principalColumn: "accessID");

            migrationBuilder.AddForeignKey(
                name: "FK_AllowanceAssignments_Allowances_allowanceID",
                table: "AllowanceAssignments",
                column: "allowanceID",
                principalTable: "Allowances",
                principalColumn: "allowanceID");

            migrationBuilder.AddForeignKey(
                name: "FK_AllowanceAssignments_Employments_employmentID",
                table: "AllowanceAssignments",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_AllowanceAssignmentsHistories_AllowanceAssignments_allowanceAssignmentID",
                table: "AllowanceAssignmentsHistories",
                column: "allowanceAssignmentID",
                principalTable: "AllowanceAssignments",
                principalColumn: "allowanceAssignmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_BankInfos_Persons_personID",
                table: "BankInfos",
                column: "personID",
                principalTable: "Persons",
                principalColumn: "personID");

            migrationBuilder.AddForeignKey(
                name: "FK_Breaks_Shifts_shiftID",
                table: "Breaks",
                column: "shiftID",
                principalTable: "Shifts",
                principalColumn: "shiftID");

            migrationBuilder.AddForeignKey(
                name: "FK_ContractHistories_Contracts_contractID",
                table: "ContractHistories",
                column: "contractID",
                principalTable: "Contracts",
                principalColumn: "contractID");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Employments_employmentID",
                table: "Contracts",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_DeductionRecords_DeductionTypes_deductionTypeID",
                table: "DeductionRecords",
                column: "deductionTypeID",
                principalTable: "DeductionTypes",
                principalColumn: "deductionTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_DeductionRecords_PayrollPays_payrollPayID",
                table: "DeductionRecords",
                column: "payrollPayID",
                principalTable: "PayrollPays",
                principalColumn: "payrollPayID");

            migrationBuilder.AddForeignKey(
                name: "FK_DelegationHistories_Delegations_delegationID",
                table: "DelegationHistories",
                column: "delegationID",
                principalTable: "Delegations",
                principalColumn: "delegationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Delegations_Employments_delegationFrom",
                table: "Delegations",
                column: "delegationFrom",
                principalTable: "Employments",
                principalColumn: "employmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Delegations_Employments_delegationTo",
                table: "Delegations",
                column: "delegationTo",
                principalTable: "Employments",
                principalColumn: "employmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentHistories_Departments_departmentID",
                table: "DepartmentHistories",
                column: "departmentID",
                principalTable: "Departments",
                principalColumn: "departmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Companies_companyID",
                table: "Departments",
                column: "companyID",
                principalTable: "Companies",
                principalColumn: "companyID");

            migrationBuilder.AddForeignKey(
                name: "FK_EarningRecords_EarningTypes_earningTypeId",
                table: "EarningRecords",
                column: "earningTypeId",
                principalTable: "EarningTypes",
                principalColumn: "earningTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_EarningRecords_PayrollPays_payrollPayID",
                table: "EarningRecords",
                column: "payrollPayID",
                principalTable: "PayrollPays",
                principalColumn: "payrollPayID");

            migrationBuilder.AddForeignKey(
                name: "FK_EmploymentHistories_EmploymentTypes_employmentTypeID",
                table: "EmploymentHistories",
                column: "employmentTypeID",
                principalTable: "EmploymentTypes",
                principalColumn: "employmentTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_EmploymentHistories_Employments_employmentID",
                table: "EmploymentHistories",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_EmploymentMethodHistories_EmploymentMethods_employmentMethodID",
                table: "EmploymentMethodHistories",
                column: "employmentMethodID",
                principalTable: "EmploymentMethods",
                principalColumn: "employmentMethodID");

            migrationBuilder.AddForeignKey(
                name: "FK_EmploymentRequestHistories_EmploymentRequests_employmentRequestID",
                table: "EmploymentRequestHistories",
                column: "employmentRequestID",
                principalTable: "EmploymentRequests",
                principalColumn: "employmentRequestID");

            migrationBuilder.AddForeignKey(
                name: "FK_EmploymentRequests_EmploymentTypes_employmentTypeID",
                table: "EmploymentRequests",
                column: "employmentTypeID",
                principalTable: "EmploymentTypes",
                principalColumn: "employmentTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_EmploymentRequests_Jobs_jobID",
                table: "EmploymentRequests",
                column: "jobID",
                principalTable: "Jobs",
                principalColumn: "jobID");

            migrationBuilder.AddForeignKey(
                name: "FK_Employments_Persons_personID",
                table: "Employments",
                column: "personID",
                principalTable: "Persons",
                principalColumn: "personID");

            migrationBuilder.AddForeignKey(
                name: "FK_Experiences_Persons_personID",
                table: "Experiences",
                column: "personID",
                principalTable: "Persons",
                principalColumn: "personID");

            migrationBuilder.AddForeignKey(
                name: "FK_Families_Persons_personID",
                table: "Families",
                column: "personID",
                principalTable: "Persons",
                principalColumn: "personID");

            migrationBuilder.AddForeignKey(
                name: "FK_Guaranties_Employments_employmentID",
                table: "Guaranties",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_GuarantyHistories_Guaranties_guarantyID",
                table: "GuarantyHistories",
                column: "guarantyID",
                principalTable: "Guaranties",
                principalColumn: "guarantyID");

            migrationBuilder.AddForeignKey(
                name: "FK_JobPlacementHistories_JobPlacements_jobPlacementID",
                table: "JobPlacementHistories",
                column: "jobPlacementID",
                principalTable: "JobPlacements",
                principalColumn: "jobPlacementID");

            migrationBuilder.AddForeignKey(
                name: "FK_JobPlacements_Departments_departmentID",
                table: "JobPlacements",
                column: "departmentID",
                principalTable: "Departments",
                principalColumn: "departmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_JobPlacements_Employments_employmentID",
                table: "JobPlacements",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_JobReqCosts_JobRequirements_jobRequirementID",
                table: "JobReqCosts",
                column: "jobRequirementID",
                principalTable: "JobRequirements",
                principalColumn: "jobRequirementID");

            migrationBuilder.AddForeignKey(
                name: "FK_JobRequirementHistories_JobRequirements_jobRequirementID",
                table: "JobRequirementHistories",
                column: "jobRequirementID",
                principalTable: "JobRequirements",
                principalColumn: "jobRequirementID");

            migrationBuilder.AddForeignKey(
                name: "FK_JobRequirements_Departments_departmentID",
                table: "JobRequirements",
                column: "departmentID",
                principalTable: "Departments",
                principalColumn: "departmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_JobRequirements_Jobs_jobID",
                table: "JobRequirements",
                column: "jobID",
                principalTable: "Jobs",
                principalColumn: "jobID");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_JobCategories_jobCategoryID",
                table: "Jobs",
                column: "jobCategoryID",
                principalTable: "JobCategories",
                principalColumn: "jobCategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_JobClasses_jobClassID",
                table: "Jobs",
                column: "jobClassID",
                principalTable: "JobClasses",
                principalColumn: "JobClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_JobGrades_jobGradeID",
                table: "Jobs",
                column: "jobGradeID",
                principalTable: "JobGrades",
                principalColumn: "jobGradeID");

            migrationBuilder.AddForeignKey(
                name: "FK_JobStepHistories_JobSteps_jobStepID",
                table: "JobStepHistories",
                column: "jobStepID",
                principalTable: "JobSteps",
                principalColumn: "jobStepID");

            migrationBuilder.AddForeignKey(
                name: "FK_JobSteps_JobGrades_jobGradeID",
                table: "JobSteps",
                column: "jobGradeID",
                principalTable: "JobGrades",
                principalColumn: "jobGradeID");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveHistories_Leaves_leaveID",
                table: "LeaveHistories",
                column: "leaveID",
                principalTable: "Leaves",
                principalColumn: "leaveID");

            migrationBuilder.AddForeignKey(
                name: "FK_Leaves_Employments_employmentID",
                table: "Leaves",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Leaves_LeaveTypes_leaveTypeID",
                table: "Leaves",
                column: "leaveTypeID",
                principalTable: "LeaveTypes",
                principalColumn: "leaveTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_LoyaltyHistories_Employments_employmentID",
                table: "LoyaltyHistories",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_LoyaltyHistories_Loyalties_loyaltyID",
                table: "LoyaltyHistories",
                column: "loyaltyID",
                principalTable: "Loyalties",
                principalColumn: "loyaltyID");

            migrationBuilder.AddForeignKey(
                name: "FK_OvertimeHistories_OvertimeRecords_overtimeRecordID",
                table: "OvertimeHistories",
                column: "overtimeRecordID",
                principalTable: "OvertimeRecords",
                principalColumn: "overtimeRecordID");

            migrationBuilder.AddForeignKey(
                name: "FK_OvertimeRecords_Departments_departmentID",
                table: "OvertimeRecords",
                column: "departmentID",
                principalTable: "Departments",
                principalColumn: "departmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_OvertimeRecords_Employments_employmentID",
                table: "OvertimeRecords",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_OvertimeRecords_Overtimes_overtimeID",
                table: "OvertimeRecords",
                column: "overtimeID",
                principalTable: "Overtimes",
                principalColumn: "overtimeID");

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollHistories_Payrolls_payrollID",
                table: "PayrollHistories",
                column: "payrollID",
                principalTable: "Payrolls",
                principalColumn: "payrollID");

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollPays_Employments_employmentID",
                table: "PayrollPays",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollPays_Payrolls_payrollID",
                table: "PayrollPays",
                column: "payrollID",
                principalTable: "Payrolls",
                principalColumn: "payrollID");

            migrationBuilder.AddForeignKey(
                name: "FK_Penalties_Employments_employmentID",
                table: "Penalties",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Penalties_PenaltyTypes_penaltyTypeID",
                table: "Penalties",
                column: "penaltyTypeID",
                principalTable: "PenaltyTypes",
                principalColumn: "penaltyTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_PenaltyHistories_Penalties_penaltyID",
                table: "PenaltyHistories",
                column: "penaltyID",
                principalTable: "Penalties",
                principalColumn: "penaltyID");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonEducationLevels_EducationLevels_educationLevelID",
                table: "PersonEducationLevels",
                column: "educationLevelID",
                principalTable: "EducationLevels",
                principalColumn: "educationLevelID");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonEducationLevels_Persons_personID",
                table: "PersonEducationLevels",
                column: "personID",
                principalTable: "Persons",
                principalColumn: "personID");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonHistories_Persons_personID",
                table: "PersonHistories",
                column: "personID",
                principalTable: "Persons",
                principalColumn: "personID");

            migrationBuilder.AddForeignKey(
                name: "FK_prohibitionHistories_Prohibitions_prohibitionID",
                table: "prohibitionHistories",
                column: "prohibitionID",
                principalTable: "Prohibitions",
                principalColumn: "prohibitionID");

            migrationBuilder.AddForeignKey(
                name: "FK_Prohibitions_Employments_employmentID",
                table: "Prohibitions",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequestHistories_ServiceRequests_serviceRequestID",
                table: "ServiceRequestHistories",
                column: "serviceRequestID",
                principalTable: "ServiceRequests",
                principalColumn: "serviceRequestID");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequests_Employments_employmentID",
                table: "ServiceRequests",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftAssignments_Employments_employmentID",
                table: "ShiftAssignments",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftAssignments_Shifts_shiftID",
                table: "ShiftAssignments",
                column: "shiftID",
                principalTable: "Shifts",
                principalColumn: "shiftID");

            migrationBuilder.AddForeignKey(
                name: "FK_SiteAssignments_Employments_employmentID",
                table: "SiteAssignments",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_SiteAssignments_workSiteModel_workSiteID",
                table: "SiteAssignments",
                column: "workSiteID",
                principalTable: "workSiteModel",
                principalColumn: "workSiteID");

            migrationBuilder.AddForeignKey(
                name: "FK_StructureHistories_Structures_structureID",
                table: "StructureHistories",
                column: "structureID",
                principalTable: "Structures",
                principalColumn: "structureID");

            migrationBuilder.AddForeignKey(
                name: "FK_Structures_Departments_departmentID",
                table: "Structures",
                column: "departmentID",
                principalTable: "Departments",
                principalColumn: "departmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Structures_Jobs_jobID",
                table: "Structures",
                column: "jobID",
                principalTable: "Jobs",
                principalColumn: "jobID");

            migrationBuilder.AddForeignKey(
                name: "FK_Structures_Structures_reportsTo",
                table: "Structures",
                column: "reportsTo",
                principalTable: "Structures",
                principalColumn: "structureID");

            migrationBuilder.AddForeignKey(
                name: "FK_SubAccounts_Accounts_accountID",
                table: "SubAccounts",
                column: "accountID",
                principalTable: "Accounts",
                principalColumn: "accountID");

            migrationBuilder.AddForeignKey(
                name: "FK_Terminations_Employments_employmentID",
                table: "Terminations",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserHistories_Users_userID",
                table: "UserHistories",
                column: "userID",
                principalTable: "Users",
                principalColumn: "userID");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Persons_personID",
                table: "Users",
                column: "personID",
                principalTable: "Persons",
                principalColumn: "personID");

            migrationBuilder.AddForeignKey(
                name: "FK_workSiteModel_Addresses_addressID",
                table: "workSiteModel",
                column: "addressID",
                principalTable: "Addresses",
                principalColumn: "addressID");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkSitesHistories_workSiteModel_workSiteID",
                table: "WorkSitesHistories",
                column: "workSiteID",
                principalTable: "workSiteModel",
                principalColumn: "workSiteID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accesses_Companies_companyID",
                table: "Accesses");

            migrationBuilder.DropForeignKey(
                name: "FK_Accesses_Users_userID",
                table: "Accesses");

            migrationBuilder.DropForeignKey(
                name: "FK_AccessHistories_Accesses_accessID",
                table: "AccessHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_AllowanceAssignments_Allowances_allowanceID",
                table: "AllowanceAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_AllowanceAssignments_Employments_employmentID",
                table: "AllowanceAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_AllowanceAssignmentsHistories_AllowanceAssignments_allowanceAssignmentID",
                table: "AllowanceAssignmentsHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_BankInfos_Persons_personID",
                table: "BankInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_Breaks_Shifts_shiftID",
                table: "Breaks");

            migrationBuilder.DropForeignKey(
                name: "FK_ContractHistories_Contracts_contractID",
                table: "ContractHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Employments_employmentID",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_DeductionRecords_DeductionTypes_deductionTypeID",
                table: "DeductionRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_DeductionRecords_PayrollPays_payrollPayID",
                table: "DeductionRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_DelegationHistories_Delegations_delegationID",
                table: "DelegationHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Delegations_Employments_delegationFrom",
                table: "Delegations");

            migrationBuilder.DropForeignKey(
                name: "FK_Delegations_Employments_delegationTo",
                table: "Delegations");

            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentHistories_Departments_departmentID",
                table: "DepartmentHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Companies_companyID",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_EarningRecords_EarningTypes_earningTypeId",
                table: "EarningRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_EarningRecords_PayrollPays_payrollPayID",
                table: "EarningRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_EmploymentHistories_EmploymentTypes_employmentTypeID",
                table: "EmploymentHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_EmploymentHistories_Employments_employmentID",
                table: "EmploymentHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_EmploymentMethodHistories_EmploymentMethods_employmentMethodID",
                table: "EmploymentMethodHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_EmploymentRequestHistories_EmploymentRequests_employmentRequestID",
                table: "EmploymentRequestHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_EmploymentRequests_EmploymentTypes_employmentTypeID",
                table: "EmploymentRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_EmploymentRequests_Jobs_jobID",
                table: "EmploymentRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Employments_Persons_personID",
                table: "Employments");

            migrationBuilder.DropForeignKey(
                name: "FK_Experiences_Persons_personID",
                table: "Experiences");

            migrationBuilder.DropForeignKey(
                name: "FK_Families_Persons_personID",
                table: "Families");

            migrationBuilder.DropForeignKey(
                name: "FK_Guaranties_Employments_employmentID",
                table: "Guaranties");

            migrationBuilder.DropForeignKey(
                name: "FK_GuarantyHistories_Guaranties_guarantyID",
                table: "GuarantyHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_JobPlacementHistories_JobPlacements_jobPlacementID",
                table: "JobPlacementHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_JobPlacements_Departments_departmentID",
                table: "JobPlacements");

            migrationBuilder.DropForeignKey(
                name: "FK_JobPlacements_Employments_employmentID",
                table: "JobPlacements");

            migrationBuilder.DropForeignKey(
                name: "FK_JobReqCosts_JobRequirements_jobRequirementID",
                table: "JobReqCosts");

            migrationBuilder.DropForeignKey(
                name: "FK_JobRequirementHistories_JobRequirements_jobRequirementID",
                table: "JobRequirementHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_JobRequirements_Departments_departmentID",
                table: "JobRequirements");

            migrationBuilder.DropForeignKey(
                name: "FK_JobRequirements_Jobs_jobID",
                table: "JobRequirements");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_JobCategories_jobCategoryID",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_JobClasses_jobClassID",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_JobGrades_jobGradeID",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_JobStepHistories_JobSteps_jobStepID",
                table: "JobStepHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_JobSteps_JobGrades_jobGradeID",
                table: "JobSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveHistories_Leaves_leaveID",
                table: "LeaveHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Leaves_Employments_employmentID",
                table: "Leaves");

            migrationBuilder.DropForeignKey(
                name: "FK_Leaves_LeaveTypes_leaveTypeID",
                table: "Leaves");

            migrationBuilder.DropForeignKey(
                name: "FK_LoyaltyHistories_Employments_employmentID",
                table: "LoyaltyHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_LoyaltyHistories_Loyalties_loyaltyID",
                table: "LoyaltyHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_OvertimeHistories_OvertimeRecords_overtimeRecordID",
                table: "OvertimeHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_OvertimeRecords_Departments_departmentID",
                table: "OvertimeRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_OvertimeRecords_Employments_employmentID",
                table: "OvertimeRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_OvertimeRecords_Overtimes_overtimeID",
                table: "OvertimeRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollHistories_Payrolls_payrollID",
                table: "PayrollHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollPays_Employments_employmentID",
                table: "PayrollPays");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollPays_Payrolls_payrollID",
                table: "PayrollPays");

            migrationBuilder.DropForeignKey(
                name: "FK_Penalties_Employments_employmentID",
                table: "Penalties");

            migrationBuilder.DropForeignKey(
                name: "FK_Penalties_PenaltyTypes_penaltyTypeID",
                table: "Penalties");

            migrationBuilder.DropForeignKey(
                name: "FK_PenaltyHistories_Penalties_penaltyID",
                table: "PenaltyHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonEducationLevels_EducationLevels_educationLevelID",
                table: "PersonEducationLevels");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonEducationLevels_Persons_personID",
                table: "PersonEducationLevels");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonHistories_Persons_personID",
                table: "PersonHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_prohibitionHistories_Prohibitions_prohibitionID",
                table: "prohibitionHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Prohibitions_Employments_employmentID",
                table: "Prohibitions");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequestHistories_ServiceRequests_serviceRequestID",
                table: "ServiceRequestHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequests_Employments_employmentID",
                table: "ServiceRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShiftAssignments_Employments_employmentID",
                table: "ShiftAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_ShiftAssignments_Shifts_shiftID",
                table: "ShiftAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_SiteAssignments_Employments_employmentID",
                table: "SiteAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_SiteAssignments_workSiteModel_workSiteID",
                table: "SiteAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_StructureHistories_Structures_structureID",
                table: "StructureHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Structures_Departments_departmentID",
                table: "Structures");

            migrationBuilder.DropForeignKey(
                name: "FK_Structures_Jobs_jobID",
                table: "Structures");

            migrationBuilder.DropForeignKey(
                name: "FK_Structures_Structures_reportsTo",
                table: "Structures");

            migrationBuilder.DropForeignKey(
                name: "FK_SubAccounts_Accounts_accountID",
                table: "SubAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Terminations_Employments_employmentID",
                table: "Terminations");

            migrationBuilder.DropForeignKey(
                name: "FK_UserHistories_Users_userID",
                table: "UserHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Persons_personID",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_workSiteModel_Addresses_addressID",
                table: "workSiteModel");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkSitesHistories_workSiteModel_workSiteID",
                table: "WorkSitesHistories");

            migrationBuilder.DropColumn(
                name: "maxEarlyIn",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "maxEarlyOut",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "maxLateIn",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "maxLateOut",
                table: "Shifts");

            migrationBuilder.RenameColumn(
                name: "worksiteName",
                table: "WorkSitesHistories",
                newName: "workSiteName");

            migrationBuilder.RenameColumn(
                name: "workSiteStatus",
                table: "workSiteModel",
                newName: "worksiteStatus");

            migrationBuilder.AddForeignKey(
                name: "FK_Accesses_Companies_companyID",
                table: "Accesses",
                column: "companyID",
                principalTable: "Companies",
                principalColumn: "companyID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Accesses_Users_userID",
                table: "Accesses",
                column: "userID",
                principalTable: "Users",
                principalColumn: "userID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccessHistories_Accesses_accessID",
                table: "AccessHistories",
                column: "accessID",
                principalTable: "Accesses",
                principalColumn: "accessID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AllowanceAssignments_Allowances_allowanceID",
                table: "AllowanceAssignments",
                column: "allowanceID",
                principalTable: "Allowances",
                principalColumn: "allowanceID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AllowanceAssignments_Employments_employmentID",
                table: "AllowanceAssignments",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AllowanceAssignmentsHistories_AllowanceAssignments_allowanceAssignmentID",
                table: "AllowanceAssignmentsHistories",
                column: "allowanceAssignmentID",
                principalTable: "AllowanceAssignments",
                principalColumn: "allowanceAssignmentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BankInfos_Persons_personID",
                table: "BankInfos",
                column: "personID",
                principalTable: "Persons",
                principalColumn: "personID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Breaks_Shifts_shiftID",
                table: "Breaks",
                column: "shiftID",
                principalTable: "Shifts",
                principalColumn: "shiftID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContractHistories_Contracts_contractID",
                table: "ContractHistories",
                column: "contractID",
                principalTable: "Contracts",
                principalColumn: "contractID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Employments_employmentID",
                table: "Contracts",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeductionRecords_DeductionTypes_deductionTypeID",
                table: "DeductionRecords",
                column: "deductionTypeID",
                principalTable: "DeductionTypes",
                principalColumn: "deductionTypeID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeductionRecords_PayrollPays_payrollPayID",
                table: "DeductionRecords",
                column: "payrollPayID",
                principalTable: "PayrollPays",
                principalColumn: "payrollPayID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DelegationHistories_Delegations_delegationID",
                table: "DelegationHistories",
                column: "delegationID",
                principalTable: "Delegations",
                principalColumn: "delegationID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Delegations_Employments_delegationFrom",
                table: "Delegations",
                column: "delegationFrom",
                principalTable: "Employments",
                principalColumn: "employmentID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Delegations_Employments_delegationTo",
                table: "Delegations",
                column: "delegationTo",
                principalTable: "Employments",
                principalColumn: "employmentID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentHistories_Departments_departmentID",
                table: "DepartmentHistories",
                column: "departmentID",
                principalTable: "Departments",
                principalColumn: "departmentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Companies_companyID",
                table: "Departments",
                column: "companyID",
                principalTable: "Companies",
                principalColumn: "companyID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EarningRecords_EarningTypes_earningTypeId",
                table: "EarningRecords",
                column: "earningTypeId",
                principalTable: "EarningTypes",
                principalColumn: "earningTypeID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EarningRecords_PayrollPays_payrollPayID",
                table: "EarningRecords",
                column: "payrollPayID",
                principalTable: "PayrollPays",
                principalColumn: "payrollPayID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmploymentHistories_EmploymentTypes_employmentTypeID",
                table: "EmploymentHistories",
                column: "employmentTypeID",
                principalTable: "EmploymentTypes",
                principalColumn: "employmentTypeID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmploymentHistories_Employments_employmentID",
                table: "EmploymentHistories",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmploymentMethodHistories_EmploymentMethods_employmentMethodID",
                table: "EmploymentMethodHistories",
                column: "employmentMethodID",
                principalTable: "EmploymentMethods",
                principalColumn: "employmentMethodID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmploymentRequestHistories_EmploymentRequests_employmentRequestID",
                table: "EmploymentRequestHistories",
                column: "employmentRequestID",
                principalTable: "EmploymentRequests",
                principalColumn: "employmentRequestID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmploymentRequests_EmploymentTypes_employmentTypeID",
                table: "EmploymentRequests",
                column: "employmentTypeID",
                principalTable: "EmploymentTypes",
                principalColumn: "employmentTypeID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmploymentRequests_Jobs_jobID",
                table: "EmploymentRequests",
                column: "jobID",
                principalTable: "Jobs",
                principalColumn: "jobID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Employments_Persons_personID",
                table: "Employments",
                column: "personID",
                principalTable: "Persons",
                principalColumn: "personID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Experiences_Persons_personID",
                table: "Experiences",
                column: "personID",
                principalTable: "Persons",
                principalColumn: "personID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Families_Persons_personID",
                table: "Families",
                column: "personID",
                principalTable: "Persons",
                principalColumn: "personID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Guaranties_Employments_employmentID",
                table: "Guaranties",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GuarantyHistories_Guaranties_guarantyID",
                table: "GuarantyHistories",
                column: "guarantyID",
                principalTable: "Guaranties",
                principalColumn: "guarantyID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobPlacementHistories_JobPlacements_jobPlacementID",
                table: "JobPlacementHistories",
                column: "jobPlacementID",
                principalTable: "JobPlacements",
                principalColumn: "jobPlacementID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobPlacements_Departments_departmentID",
                table: "JobPlacements",
                column: "departmentID",
                principalTable: "Departments",
                principalColumn: "departmentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobPlacements_Employments_employmentID",
                table: "JobPlacements",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobReqCosts_JobRequirements_jobRequirementID",
                table: "JobReqCosts",
                column: "jobRequirementID",
                principalTable: "JobRequirements",
                principalColumn: "jobRequirementID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobRequirementHistories_JobRequirements_jobRequirementID",
                table: "JobRequirementHistories",
                column: "jobRequirementID",
                principalTable: "JobRequirements",
                principalColumn: "jobRequirementID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobRequirements_Departments_departmentID",
                table: "JobRequirements",
                column: "departmentID",
                principalTable: "Departments",
                principalColumn: "departmentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobRequirements_Jobs_jobID",
                table: "JobRequirements",
                column: "jobID",
                principalTable: "Jobs",
                principalColumn: "jobID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_JobCategories_jobCategoryID",
                table: "Jobs",
                column: "jobCategoryID",
                principalTable: "JobCategories",
                principalColumn: "jobCategoryID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_JobClasses_jobClassID",
                table: "Jobs",
                column: "jobClassID",
                principalTable: "JobClasses",
                principalColumn: "JobClassId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_JobGrades_jobGradeID",
                table: "Jobs",
                column: "jobGradeID",
                principalTable: "JobGrades",
                principalColumn: "jobGradeID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobStepHistories_JobSteps_jobStepID",
                table: "JobStepHistories",
                column: "jobStepID",
                principalTable: "JobSteps",
                principalColumn: "jobStepID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobSteps_JobGrades_jobGradeID",
                table: "JobSteps",
                column: "jobGradeID",
                principalTable: "JobGrades",
                principalColumn: "jobGradeID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveHistories_Leaves_leaveID",
                table: "LeaveHistories",
                column: "leaveID",
                principalTable: "Leaves",
                principalColumn: "leaveID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Leaves_Employments_employmentID",
                table: "Leaves",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Leaves_LeaveTypes_leaveTypeID",
                table: "Leaves",
                column: "leaveTypeID",
                principalTable: "LeaveTypes",
                principalColumn: "leaveTypeID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LoyaltyHistories_Employments_employmentID",
                table: "LoyaltyHistories",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LoyaltyHistories_Loyalties_loyaltyID",
                table: "LoyaltyHistories",
                column: "loyaltyID",
                principalTable: "Loyalties",
                principalColumn: "loyaltyID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OvertimeHistories_OvertimeRecords_overtimeRecordID",
                table: "OvertimeHistories",
                column: "overtimeRecordID",
                principalTable: "OvertimeRecords",
                principalColumn: "overtimeRecordID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OvertimeRecords_Departments_departmentID",
                table: "OvertimeRecords",
                column: "departmentID",
                principalTable: "Departments",
                principalColumn: "departmentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OvertimeRecords_Employments_employmentID",
                table: "OvertimeRecords",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OvertimeRecords_Overtimes_overtimeID",
                table: "OvertimeRecords",
                column: "overtimeID",
                principalTable: "Overtimes",
                principalColumn: "overtimeID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollHistories_Payrolls_payrollID",
                table: "PayrollHistories",
                column: "payrollID",
                principalTable: "Payrolls",
                principalColumn: "payrollID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollPays_Employments_employmentID",
                table: "PayrollPays",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollPays_Payrolls_payrollID",
                table: "PayrollPays",
                column: "payrollID",
                principalTable: "Payrolls",
                principalColumn: "payrollID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Penalties_Employments_employmentID",
                table: "Penalties",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Penalties_PenaltyTypes_penaltyTypeID",
                table: "Penalties",
                column: "penaltyTypeID",
                principalTable: "PenaltyTypes",
                principalColumn: "penaltyTypeID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PenaltyHistories_Penalties_penaltyID",
                table: "PenaltyHistories",
                column: "penaltyID",
                principalTable: "Penalties",
                principalColumn: "penaltyID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonEducationLevels_EducationLevels_educationLevelID",
                table: "PersonEducationLevels",
                column: "educationLevelID",
                principalTable: "EducationLevels",
                principalColumn: "educationLevelID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonEducationLevels_Persons_personID",
                table: "PersonEducationLevels",
                column: "personID",
                principalTable: "Persons",
                principalColumn: "personID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonHistories_Persons_personID",
                table: "PersonHistories",
                column: "personID",
                principalTable: "Persons",
                principalColumn: "personID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_prohibitionHistories_Prohibitions_prohibitionID",
                table: "prohibitionHistories",
                column: "prohibitionID",
                principalTable: "Prohibitions",
                principalColumn: "prohibitionID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prohibitions_Employments_employmentID",
                table: "Prohibitions",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequestHistories_ServiceRequests_serviceRequestID",
                table: "ServiceRequestHistories",
                column: "serviceRequestID",
                principalTable: "ServiceRequests",
                principalColumn: "serviceRequestID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequests_Employments_employmentID",
                table: "ServiceRequests",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftAssignments_Employments_employmentID",
                table: "ShiftAssignments",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftAssignments_Shifts_shiftID",
                table: "ShiftAssignments",
                column: "shiftID",
                principalTable: "Shifts",
                principalColumn: "shiftID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SiteAssignments_Employments_employmentID",
                table: "SiteAssignments",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SiteAssignments_workSiteModel_workSiteID",
                table: "SiteAssignments",
                column: "workSiteID",
                principalTable: "workSiteModel",
                principalColumn: "workSiteID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StructureHistories_Structures_structureID",
                table: "StructureHistories",
                column: "structureID",
                principalTable: "Structures",
                principalColumn: "structureID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Structures_Departments_departmentID",
                table: "Structures",
                column: "departmentID",
                principalTable: "Departments",
                principalColumn: "departmentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Structures_Jobs_jobID",
                table: "Structures",
                column: "jobID",
                principalTable: "Jobs",
                principalColumn: "jobID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Structures_Structures_reportsTo",
                table: "Structures",
                column: "reportsTo",
                principalTable: "Structures",
                principalColumn: "structureID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SubAccounts_Accounts_accountID",
                table: "SubAccounts",
                column: "accountID",
                principalTable: "Accounts",
                principalColumn: "accountID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Terminations_Employments_employmentID",
                table: "Terminations",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserHistories_Users_userID",
                table: "UserHistories",
                column: "userID",
                principalTable: "Users",
                principalColumn: "userID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Persons_personID",
                table: "Users",
                column: "personID",
                principalTable: "Persons",
                principalColumn: "personID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_workSiteModel_Addresses_addressID",
                table: "workSiteModel",
                column: "addressID",
                principalTable: "Addresses",
                principalColumn: "addressID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkSitesHistories_workSiteModel_workSiteID",
                table: "WorkSitesHistories",
                column: "workSiteID",
                principalTable: "workSiteModel",
                principalColumn: "workSiteID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
