using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using PIS2.Pages.Management;
using PIS2.Pages.OvertimeHistory;
using System.Text.RegularExpressions;
using static System.Formats.Asn1.AsnWriter;

namespace PIS2.Views
{
    
    public class views
    {}
    public class EducationLevelData
    {
        public string EducationLevelCategory { get; set; }
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
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string Manager { get; set; }
        public int Employees { get; set; }
        public int xEmployees { get; set; }
        public decimal Salary { get; set; }
        public leaveDetail Leaves { get; set; }
        public decimal Overtime { get; set; }
        public decimal Allowance { get; set; }
        public decimal Severance { get; set; } = 0;
        public decimal Total { get; set; }
    }
    public class EmployeeView
    {
        public employmentModel Employment{ get; set; }
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
        public decimal Incremented { get; set;}
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
    public class CertificationDetailsView {
        public int? CompanyID { get; set; }
        public string? CompanyName { get; set; }
        public int? DepartmentID { get; set; }
        public string? DepartmentName { get; set; }
        public int? EducationLevelID { get; set; }
        public string? EducationLevelCategory { get; set; }
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
}
