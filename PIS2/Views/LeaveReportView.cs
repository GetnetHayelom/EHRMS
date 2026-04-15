using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Views
{
    public class LeaveReportView
    {
        
        public int EmploymentID { get; set; }
        public string GivenID { get; set; }
        public string FullName { get; set; }
        public Gender Gender { get; set; }
        public string? CompanyName { get; set; }
        public int? CompanyID { get; set; }
        public string? DepartmentName { get; set; }
        public int? DepartmentID { get; set; }
        
        public int? LeaveTypeID { get; set; }
        public leaveStatus LeaveStatus { get; set; }
        public DateTime LeaveStart { get; set; }
        public DateTime LeaveEnd { get; set; }       
        public string? LeaveType { get; set; }
        [Precision(18, 2)]
        public decimal? LeaveDays { get; set; }
        [Precision(18, 2)]
        public decimal LeaveValue { get; set; }
        public leaveGroup LeaveGroup { get; set; }
        public leaveTypeImpact LeaveTypeImpact { get; set; }
        public bool LeaveJob { get; set; }
        public bool LeaveLegality { get; set; }
        
        public int LeaveId { get; set; }
        public DateTime LeaveRequestDate { get; set; }
        public DateTime? ApproveDate { get; set; }
        public DateTime? PostDate { get; set; }
        public DateTime? CompleteDate { get; set; }


    }

    public class LeaveHistoryView 
    {
        public int leaveID { get; set; }
        public leaveStatus leaveHistoryAction { get; set; }
        public DateTime modifiedDate { get; set; }
        public string modifiedBy { get; set; }
        public int employmentID { get; set; }
        public decimal leaveDays { get; set; }
        public int leaveTypeID { get; set; }
        public leaveStatus leaveStatus { get; set; }
        public leaveGroup leaveGroup { get; set; }
    }
    /// <summary>
    /// Annual Leave Status with balance and payable for each employee
    /// </summary>
    //
    public class AnnualLeaveSummary
    {
        public int employmentID { get; set; }
        public string givenID { get; set; }
        public int departmentID { get; set; }
        public string departmentName { get; set; }
        public int companyID { get; set; }
        public string companyName { get; set; }
        [Precision(18, 2)]
        public decimal Incremented { get; set; }
        [Precision(18, 2)]
        public decimal Used { get; set; }
        [Precision(18, 2)]
        public decimal leaveBalance { get; set; }
        [Precision(18, 2)]
        public decimal adjustedLeaveBalance { get; set; }
        [Precision(18, 2)]
        public decimal adjustedLeaveBalanceCost { get; set; }

    }
    ///<summary>
    ///Annual Leave Status with balance and payable for each company
    ///
    public class LeaveBalanceDepartmentView
    {
        public int CompanyID { get; set; } = 0;
        public string Company { get; set; } = "";
        public int DepartmentID { get; set; } = 0;
        public string Department { get; set; } = "";
        [Precision(18, 2)]
        public decimal LeaveBalance { get; set; } = 0;
        [Precision(18, 2)]
        public decimal AllowedLeave { get; set; } = 0;
        [Precision(18, 2)]
        public decimal PayableLeave { get; set; } = 0;
    }

    public class LeaveDepView
    {
        public string Department;
        public int Employees;
        [Precision(18, 2)]
        public decimal total;
        [Precision(18, 2)]
        public decimal AllowedLeave;
        [Precision(18, 2)]
        public decimal Absent;
        [Precision(18, 2)]
        public decimal AnnualLeave;
    }


    public class LeaveTypeView
    {
        public string Type;
        public int Employees;
        [Precision(18, 2)]
        public decimal AllowedLeave;
        [Precision(18, 2)]
        public decimal Absent;
        [Precision(18, 2)]
        public decimal AnnualLeave;
    }
}
