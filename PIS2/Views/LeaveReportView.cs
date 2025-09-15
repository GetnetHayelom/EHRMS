using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Views
{
    public class LeaveReportView
    {
       
        public string EmployeeId { get; set; }
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public int LeaveTypeID { get; set; }
        public string LeaveType { get; set; }
        public decimal? LeaveDays { get; set; }
        public DateTime LeaveStart { get; set; }
        public DateTime LeaveEnd { get; set; }
        
    }


}
