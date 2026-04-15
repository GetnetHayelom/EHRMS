using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Views
{
    public class OTViews
    {
    }
    public class OvertimeHistoryView
    {
        public int overtimeRecordID { get; set; }
        public overtimeStatus overtimeHistoryAction { get; set; }
        public DateTime modifiedDate { get; set; }
        public string modifiedBy { get; set; }
        public int employmentID { get; set; }
        public DateTime overtimeRecordDate { get; set; }
        public int overtimeID { get; set; }
        public overtimeStatus overtimeRecordStatus { get; set; }
        public decimal overtimeAmount { get; }
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
        [Precision(18, 2)]
        public decimal? TimeElapsed { get; set; }
        [Precision(18, 2)]
        public decimal? OvertimeRate { get; set; }
        [Precision(18, 2)]
        public decimal? EmployeeRate { get; set; }
        [Precision(18, 2)]
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
        [Precision(18, 2)]
        public decimal? TotalHours { get; set; }
        [Precision(18, 2)]
        public decimal? TotalDays { get; set; }
        [Precision(18, 2)]
        public decimal? TotalCost { get; set; }
        [Precision(18, 2)]
        public decimal? HoursPerEmployee { get; set; }
        [Precision(18, 2)]
        public decimal? DaysPerEmployee { get; set; }
        [Precision(18, 2)]
        public decimal? CostPerEmployee { get; set; }

    }
}
