using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Views
{
    public class LeaveReportView
    {
        
        public int EmploymentID { get; set; }
        public string GivenID { get; set; }
        public string? CompanyName { get; set; }
        public int? CompanyID { get; set; }
        public string? DepartmentName { get; set; }
        public int? DepartmentID { get; set; }
        
        public int? LeaveTypeID { get; set; }
        public leaveStatus LeaveStatus { get; set; }
        public DateTime LeaveStart { get; set; }
        public DateTime LeaveEnd { get; set; }       
        public string? LeaveType { get; set; }
        public decimal? LeaveDays { get; set; }
        public decimal LeaveValue { get; set; }
        public leaveGroup LeaveGroup { get; set; }
        public bool LeaveJob { get; set; }
        public bool LeaveLegality { get; set; }
        
        public int LeaveId { get; set; }
        public DateTime LeaveRequestDate { get; set; }

    }


}
