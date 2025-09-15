using Microsoft.EntityFrameworkCore;

namespace PIS2.Models
{
    [Keyless]
    public class leaveDetail
    {
        
        public employmentModel Employee;
        public decimal TotalLeave, AllowedLeave, LastAccrualIncrement, leaveCost;
        public DateTime LeaveDetailStartDate, LeaveDetailsEndDate;
        public List<leavePerYear> AnnualLeaveHistory;
        public companyModel company;
        public departmentModel department;
        public personModel person;
       
        public leaveDetail(
            decimal totalLeave, decimal allowedLeave, decimal lastAccrualIncrement, 
            DateTime leaveDetailStartDate, DateTime leaveDetailEndDate, 
            List<leavePerYear> annualLeaveHistory) {

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
            company = dep.companyModel ?? new companyModel();
        }
        public leaveDetail() { }
    }
}
