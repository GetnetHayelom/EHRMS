using Microsoft.Build.ObjectModelRemoting;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using PIS2.Pages.Management;
using PIS2.Pages.OvertimeHistory;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using static System.Formats.Asn1.AsnWriter;

namespace PIS2.Views
{

    public class views
    { }
    public class EducationLevelData
    {
        public educationCategory EducationLevelCategory { get; set; }
        public string EducationLevelName { get; set; }
        public int EducationLevelCount { get; set; }
    }
    public class NameAndCount
    {
        public string zName { get; set; }
        public int zCount { get; set; }
    }
    public class YearAndCount
    {
        public int zYear { get; set; }
        public int zCount { get; set; }
    }
    public class CompanySummary
    {

        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string Manager { get; set; }
        public int Departments { get; set; }
        public int Employees { get; set; }
        public int xEmployees { get; set; }
        public decimal Salary { get; set; }
        public decimal Allowance { get; set; } = 0;
        public decimal Overtime { get; set; }
        public decimal SeverancePayable { get; set; }
        public decimal SeverancePaid { get; set; }

        public decimal getTotalExpence()
        {
            return Salary + Allowance + Overtime;
        }
        public decimal payableLeaves { get; set; }

    }
    public class DepartmentSummary
    {
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public int? CompanyID { get; set; }
        public string? CompanyName { get; set; }
        public string? Manager { get; set; }
        public int? Employees { get; set; }
        public int? xEmployees { get; set; }
        public decimal? Salary { get; set; }
        public leaveDetail? Leaves { get; set; }
        public decimal? Overtime { get; set; }
        public decimal? Allowance { get; set; }
        public decimal? Severance { get; set; } = 0;
        public decimal? Total { get; set; }
    }
    public class EmployeeView
    {
        public employmentModel Employment { get; set; }
        public personModel Person { get; set; }
        public jobPlacementModel Job { get; set; }
        public departmentModel DepartmentModel { get; set; }
        public companyModel Company { get; set; }
        public workSiteModel WorkSite { get; set; }
        public addressModel Address { get; set; }
        public shiftModel Shift { get; set; }
        public leaveDetail Leave { get; set; }
    }
    //Annual Leave Status with balance and payable for each employee
    public class AnnualLeaveSummary
    {
        public int employmentID { get; set; }
        public string givenID { get; set; }
        public int departmentID { get; set; }
        public string departmentName { get; set; }
        public int companyID { get; set; }
        public string companyName { get; set; }
        public decimal Incremented { get; set; }
        public decimal Used { get; set; }
        public decimal leaveBalance { get; set; }
        public decimal adjustedLeaveBalance { get; set; }
        public decimal adjustedLeaveBalanceCost { get; set; }

    }
    ///<summary>
    //Annual Leave Status with balance and payable for each company
    public class AnnuallLeaveSummaryCompanyView
    {
        public int CompanyID { get; set; } = 0;
        public string Company { get; set; } = "";
        public decimal LeaveBalance { get; set; } = 0;
        public decimal AllowedLeave { get; set; } = 0;
        public decimal PayableLeave { get; set; } = 0;
        public List<AnnuallLeaveSummaryDepartmentView> DepartmentList { get; set; } = new List<AnnuallLeaveSummaryDepartmentView>();
    }
    public class EmploymentYearlyStat
    {
        public int? Year { get; set; }
        public int? NewEmployees { get; set; }
        public int? TerminatedEmployees { get; set; }
        public int? ActiveEmployees { get; set; }
        public decimal? HireRatePercent { get; set; }
        public decimal? TerminationRatePercent { get; set; }
    }

    public class DepartmentEmploymentStats
    {
        public int? companyID { get; set; }
        public int? departmentID { get; set; }
        public string? companyName { get; set; }
        public string? departmentName { get; set; }
        public int? TotalEmployees { get; set; }
        public int? TerminatedEmployees { get; set; }
        public int? ActiveEmployees { get; set; }
        public decimal? TerminationRatePercent { get; set; }

    }

    ///<summary>
    ///Employee Detail View Report
    /// </summary>
    /// 
    public class EmployeeDetailView
    {
        public int EmploymentID { get; set; }
        public string GivenID { get; set; }
        public int? CompanyID { get; set; }
        public string? CompanyName { get; set; }
        public int? DepartmentID { get; set; }
        public string? DepartmentName { get; set; }
        public int? WorkSiteID { get; set; }
        public string? WorkSiteName { get; set; }
        public int EmploymentTypeID { get; set; }
        public string EmploymentTypeName { get; set; }
        public int? JobID { get; set; }
        public string? JobTitle { get; set; }
        public Gender PersonGender { get; set; }
        public string FullName { get; set; }
        public mainStatus EmploymentStatus { get; set; }
        public DateTime EmploymentDate { get; set; }
        public EmploymentPositions EmploymentPosition { get; set; }
    }

    ///<summary>
    ///Overtime details view
    /// </summary>
    /// 
    public class OvertimeDetailView
    {
        public int? OvertimeRecordID { get; set; }
        public int? EmploymentID { get; set; }
        public int? OvertimeID { get; set; }
        public decimal? TimeElapsed { get; set; }
        public decimal? OvertimeRate { get; set; }
        public decimal? EmployeeRate { get; set; }
        public decimal? OvertimeCost { get; set; }
        public DateTime? OvertimeDate { get; set; }
        public overtimeStatus? OvertimeStatus { get; set; }
        public string? GivenID { get; set; }
        public int? DepartmentID { get; set; }
        public string? DepartmentName { get; set; }
        public int? CompanyID { get; set; }
        public string? CompanyName { get; set; }
        public string? OvertimeName { get; set; }
    }


    ///<summary>
    ///Overtime summary view
    /// </summary>
    //

    public class OvertimeSummaryView
    {
        public int? CompanyID { get; set; }
        public string? CompanyName { get; set; }
        public int? DepartmentID { get; set; }
        public string? DepartmentName { get; set; }
        public int? OvertimeID { get; set; }
        public string? OvertimeName { get; set; }
        public int? EmployeesInvolved { get; set; }
        public int? Records { get; set; }
        public decimal? TotalHours { get; set; }
        public decimal? TotalDays { get; set; }
        public decimal? TotalCost { get; set; }
        public decimal? HoursPerEmployee { get; set; }
        public decimal? DaysPerEmployee { get; set; }
        public decimal? CostPerEmployee { get; set; }

    }


    ///<summary>
    ///Allowance View
    ///</summary>
    ///

    public class AllowanceDetailView
    {
        public int AllowanceID { get; set; }
        public string EmployeeID { get; set; }
        public EmploymentPositions Position { get; set; }
        public int? CompanyID { get; set; }
        public string? CompanyName { get; set; }
        public int? DepartmentID { get; set; }
        public string? DepartmentName { get; set; }
        public int? JobID { get; set; }
        public string? JobTitle { get; set; }
        public int? JobClass { get; set; }
        public string? JobClassName { get; set; }
        public int AllowanceTypeID { get; set; }
        public string AllowanceTypeName { get; set; }
        public DateTime AllowanceStart { get; set; }
        public DateTime? AllowanceEnd { get; set; }
        public decimal AllowanceAmount { get; set; }
        public mainStatus AllowanceStatus { get; set; }
        public allowanceDuration AllowanceDuration { get; set; }
    }

    ///<summary>
    ///For picking with required job experience
    /// </summary>
    /// 
    public class TalentExperienceView
    {
        public int personID { get; set; }
        public string FullName { get; set; }
        public string jobTitle { get; set; }
        public int Duration { get; set; }
        public int jobClassID { get; set; }
        public mainStatus EmploymentStatus { get; set; }
        public Gender PersonGender { get; set; }

    }

    ///<summary>
    ///Certification Details for Active Employee
    /// </summary>
    /// 
    public class CertificationDetailsView
    {
        public int? CompanyID { get; set; }
        public string? CompanyName { get; set; }
        public int? DepartmentID { get; set; }
        public string? DepartmentName { get; set; }
        public int? EducationLevelID { get; set; }
        public educationCategory? EducationLevelCategory { get; set; }
        public string? EducationField { get; set; }
        public string? EducationDiscipline { get; set; }
        public string? EducationDomain { get; set; }
        public DateTime? EducationLevelDate { get; set; }
        public string? EducationLevelMark { get; set; }
        public string? EducationLevelInstitutionName { get; set; }
        public int PersonID { get; set; }
        public string FullName { get; set; }
        public Gender PersonGender { get; set; }
        public string? GivenID { get; set; }
        public mainStatus EmploymentStatus { get; set; }
        public EmploymentPositions? EmploymentPosition { get; set; }


    }

    /// <summary>
    /// Structure
    /// </summary>
    public class StructureView
    {
        public int? StructureID { get; set; }
        public int? CompanyID { get; set; }
        public string? CompanyName { get; set; }
        public int? DepartmentID { get; set; }
        public string? DepartmentName { get; set; }
        public int? JobID { get; set; }
        public string? JobTitle { get; set; }
        public int? StructureStatus { get; set; }
        public int? ReportsTo { get; set; }
        public int? RequiredNumber { get; set; }
        public int? ActivePlacements { get; set; }
    }

    public class TerminationDetailView
    {
        public int terminationID { get; set; }
        public int employmentID { get; set; }
        public DateTime terminationDate { get; set; }
        public string? terminationReason { get; set; }
        public string? terminationRemark { get; set; }
        public int terminationStatus { get; set; }
        public string FullName { get; set; }
        public int personGender { get; set; }
        public int departmentID { get; set; }
        public string departmentName { get; set; }
        public int companyID { get; set; }
        public string companyName { get; set; }
        public int jobClassId { get; set; }
        public string jobClassName { get; set; }
        public int jobCategoryID { get; set; }
        public string jobCategoryName { get; set; }


    }
    /// <summary>
    /// Transaction History View
    /// </summary>
    public class TransactionVM
    {
        public int PayrollPayID { get; set; }
        public string PayrollName { get; set; }
        public string PayrollMonth { get; set; }
        public DateTime PayrollStart { get; set; }
        public DateTime PayrollEnd { get; set; }
        public payrollStatus PayrollStatus { get; set; }

        public decimal? Amount { get; set; } = 0;
        public decimal? GrossPay { get; set; }
        public decimal? NetPay { get; set; }

        public string ProcessedBy { get; set; }
    }

    /// <summary>
    /// Evaluation Summary View
    /// </summary>
    // Primary Evaluation Data
    public class EvaluationSummaryView
    {
        public int evaluationID { get; set; }
        public string evaluationName { get; set; }
        public int evaluationStatus { get; set; } // Matches your WHERE filter
        public DateTime evaluationStartDate { get; set; }
        public DateTime evaluationEndDate { get; set; }

        // Employee Data (Using the CONCAT_WS alias)
        public int employmentID { get; set; }
        public string personFullName { get; set; }
        public int? jobPlacementID { get; set; }
        // Type Data
        public int evaluationTypeID { get; set; }
        public string evaluationTypeName { get; set; }
        public decimal evaluationTypeWeight { get; set; }
        public bool isFixed { get; set; }

        // Task Data
        public int evaluationTaskID { get; set; }
        public string evaluationTaskName { get; set; }
        public decimal evaluationTaskWeight { get; set; }

        // SubTask Data
        public int evaluationSubTaskID { get; set; }
        public string evaluationSubTaskName { get; set; }
        public decimal evaluationSubTaskWeight { get; set; }

        // Valuation Metrics
        public decimal timeValuation { get; set; }
        public decimal resourceValuation { get; set; }
        public decimal performanceValuation { get; set; }

        // Calculated Columns from SQL View
        public decimal SubTaskAvgScore { get; set; }
        public decimal WeightedSubTaskScore { get; set; }

    }

    // The top-level object for the report
    public class EvalSingleEmployeeReport
    {
        public int evaluationID { get; set; }
        public string evaluationName { get; set; }
        public string personFullName { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public decimal FinalGrandTotal { get; set; }
        public decimal? PreviousGrandTotal { get; set; }

        // Grouped by Evaluation Type
        public List<EvalTypeSummary> Types { get; set; } = new();
    }

    public class EvalTypeSummary
    {
        public string typeName { get; set; }
        public decimal typeWeight { get; set; }
         // Actual score earned for this type

        // Summarized by Task
        public List<EvalTaskSummary> Tasks { get; set; } = new();
        public decimal typeContribution { get{
                if (Tasks.Sum(t => t.taskWeight) == 0) return 0;
                return Tasks.Sum(t => t.taskScore) * typeWeight / Tasks.Sum(t => t.taskWeight);
            } }
    }

    public class EvalTaskSummary
    {
        public string taskName { get; set; }
        public decimal taskWeight { get; set; }
        public decimal avgTime { get; set; }
        public decimal avgResource { get; set; }
        public decimal avgPerformance { get; set; }
        public List<EvalSubTaskSummary> SubTasks { get; set; } = new();
        public decimal taskScore
        {
            get
            {
                decimal totalWeight = SubTasks.Sum(s => s.subtaskWeight) * 4;
                if (totalWeight == 0) return 0; // Prevent DivideByZeroException

                decimal scoreSum = SubTasks.Sum(s => s.subtaskScore);
                return (taskWeight * scoreSum) / totalWeight;
            }
        }
        
    }

    public class EvalSubTaskSummary
    {
        public string subtaskName { get; set; }
        public decimal subtaskWeight { get; set; }
        public decimal subTime { get; set; }
        public decimal subResource { get; set; }
        public decimal subPerformance { get; set; }
        public decimal subtaskScore
        {
            get
            {
                return ((subTime + subResource + subPerformance) / 3) * subtaskWeight;
            }
        }
    }

    public class EvalGrandView
    {
        public int employmentID { get; set; }
        public string givenID { get; set; }
        public mainStatus employmentStatus { get; set; }
        public int evaluationID { get; set; }
        public string evaluationName { get; set; }
        public string personFullName { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public evaluationStatus evaluationStatus { get; set; }
        public int? jobPlacementID { get; set; }
        public int? departmentID { get; set; }
        public string? departmentName { get; set; }
        public int? companyID { get; set; }
        public string? companyName { get; set; }
        public int evaluationTypeID { get; set; }
        public string evaluationTypeName { get; set; }
        public decimal TaskScoreSum { get; set; }
        public decimal TypeScore { get; set; }
        public decimal FinalScore{ get; set; }
    }
}

