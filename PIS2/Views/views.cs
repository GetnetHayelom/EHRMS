using Microsoft.Build.ObjectModelRemoting;
using Microsoft.EntityFrameworkCore;
using PIS2.Enums;
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
        [Precision(18, 2)]
        public decimal Salary { get; set; }
        [Precision(18, 2)]
        public decimal Allowance { get; set; } = 0;
        [Precision(18, 2)]
        public decimal Overtime { get; set; }
        [Precision(18, 2)]
        public decimal SeverancePayable { get; set; }
        [Precision(18, 2)]
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
        [Precision(18, 2)]
        public decimal? Salary { get; set; }
        public leaveDetail? Leaves { get; set; }
        [Precision(18, 2)]
        public decimal? Overtime { get; set; }
        [Precision(18, 2)]
        public decimal? Allowance { get; set; }
        [Precision(18, 2)]
        public decimal? Severance { get; set; } = 0;
        [Precision(18, 2)]
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
    
    public class EmploymentYearlyStat
    {
        public int? Year { get; set; }
        public int? NewEmployees { get; set; }
        public int? TerminatedEmployees { get; set; }
        public int? ActiveEmployees { get; set; }
        [Precision(18, 2)]
        public decimal? HireRatePercent { get; set; }
        [Precision(18, 2)]
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
        [Precision(18, 2)]
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
        [Precision(18, 2)]
        public decimal? Salary { get; set; }
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
        [Precision(18, 2)]
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
        public DateTime employmentDate { get; set; }
        public string givenID { get; set; }
        public int employmentTypeID { get; set; }
        public string employmentTypeName { get; set; }
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
        [Precision(18, 2)]
        public decimal jobPlacementSalary { get; set; }

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
        [Precision(18, 2)]
        public decimal? Amount { get; set; } = 0;
        [Precision(18, 2)]
        public decimal? GrossPay { get; set; }
        [Precision(18, 2)]
        public decimal? NetPay { get; set; }

        public string ProcessedBy { get; set; }
    }
  
    public class WorksiteSummaryView 
    { 
        public string workSiteName { get; set; }
        public int Male { get; set; }
        public int Female { get; set; }
        public int Total { get; set; }
    }
}

