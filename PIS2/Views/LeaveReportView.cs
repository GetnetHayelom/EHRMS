using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;
using PIS2.Models.Foundation;
using PIS2.Models.Organization;

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

    public class LeaveReportCompany
    {
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public int EmployeeTotal { get; set; }
        [Precision(18, 2)]
        public decimal WorkingDays { get; set; }
        public int CompanyTotal { get; set; }
        [Precision(18, 2)]
        public decimal? CompanySum { get; set; }
        [Precision(18, 2)]
        public decimal? CompanyCost { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<LeaveReportDepartment>? Departments { get; set; }
    }

    public class LeaveReportDepartment
    {
        public string DepartmentName { get; set; }
        public int DepartmentID { get; set; }
        public int DepartmentTotal { get; set; }
        public int EmployeeTotal { get; set; }
        [Precision(18, 2)]
        public decimal? DepartmentSum { get; set; }
        [Precision(18, 2)]
        public decimal? DepartmentCost { get; set; }
        public List<LeaveReportType>? LeaveTypes { get; set; }
    }

    public class LeaveReportType
    {
        public string LeaveType { get; set; }
        public int LeaveTypeCount { get; set; }
        [Precision(18, 2)]
        public decimal? LeaveTypeSum { get; set; }
        [Precision(18, 2)]
        public decimal? LeaveTypeCost { get; set; }
    }

    [Keyless]
    public class leaveDetail
    {

        public employmentModel Employee;
        public decimal TotalLeave, AllowedLeave, LastAccrualIncrement, leaveCost;
        public DateTime LeaveDetailStartDate, LeaveDetailsEndDate;
        public List<leavePerYear> AnnualLeaveHistory;
        public companyModel? company;
        public departmentModel department;
        public personModel person;

        public leaveDetail(
            decimal totalLeave, decimal allowedLeave, decimal lastAccrualIncrement,
            DateTime leaveDetailStartDate, DateTime leaveDetailEndDate,
            List<leavePerYear> annualLeaveHistory)
        {

            TotalLeave = totalLeave;
            AllowedLeave = allowedLeave;
            LastAccrualIncrement = lastAccrualIncrement;
            LeaveDetailStartDate = leaveDetailStartDate;
            LeaveDetailsEndDate = leaveDetailEndDate;
            AnnualLeaveHistory = annualLeaveHistory;


        }
        public leaveDetail(
            decimal totalLeave, decimal allowedLeave, decimal lastAccrualIncrement,
            DateTime leaveDetailStartDate, DateTime leaveDetailEndDate)
        {

            TotalLeave = totalLeave;
            AllowedLeave = allowedLeave;
            LastAccrualIncrement = lastAccrualIncrement;
            LeaveDetailStartDate = leaveDetailStartDate;
            LeaveDetailsEndDate = leaveDetailEndDate;
        }
        public leaveDetail(
           decimal totalLeave, decimal allowedLeave, decimal lastAccrualIncrement,
           DateTime leaveDetailStartDate, DateTime leaveDetailEndDate, decimal lCost, departmentModel dep)
        {

            TotalLeave = totalLeave;
            AllowedLeave = allowedLeave;
            LastAccrualIncrement = lastAccrualIncrement;
            LeaveDetailStartDate = leaveDetailStartDate;
            LeaveDetailsEndDate = leaveDetailEndDate;
            leaveCost = lCost;
            department = dep;
            //company = dep.companyModel ?? new companyModel();
        }
        public leaveDetail() { }
    }
    public class leavePerYear
    {
        private readonly PISContext _context;

        public leavePerYear(PISContext context)
        {
            _context = context;
        }

        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; } = DateTime.Now;
        public decimal startingLeaveAmount { get; set; } = 0;
        public decimal accruedLeaveAmount { get; set; } = 0;
        public decimal usedLeaveAmount { get; set; } = 0;
        public decimal rollOverLeave { get; set; } = 0;
        public decimal remainingLeaveAmount
        {
            get
            {
                return (startingLeaveAmount + accruedLeaveAmount) - usedLeaveAmount;
            }
        }
        public decimal remainingLeaveCost { get; set; }

        public decimal? totalLeaveAmount { get; set; }
        public leavePerYear() { }

    }

    public class LeaveGroup
    {
        public string? CompanyName { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal SumDays { get; set; }
        public List<LeaveReportView> Records { get; set; } = new();
    }
    public class ExpiringLeaveDto
    {
        public int employmentID { get; set; }
        public string givenID { get; set; }
        public string FullName { get; set; }
        public decimal days { get; set; }
        public DateTime expDate { get; set; }
        public Gender Gender { get; set; }
        public string? CompanyName { get; set; }
        public int? CompanyID { get; set; }
        public string? DepartmentName { get; set; }
        public int? DepartmentID { get; set; }
        public decimal? Cost { get; set; }
    }
}
