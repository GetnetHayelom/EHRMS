namespace PIS2.Models
{
    public class leaveDetail
    {
        public employmentModel Employee;
        public double TotalLeave, AllowedLeave, LastAccrualIncrement, leaveCost;
        public DateTime LeaveDetailStartDate, LeaveDetailsEndDate;
        public List<leavePerYear> AnnualLeaveHistory;
        public companyModel company;
        public departmentModel department;
        public personModel person;
       
        public leaveDetail(
            double totalLeave, double allowedLeave, double lastAccrualIncrement, 
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
            double totalLeave, double allowedLeave, double lastAccrualIncrement,
            DateTime leaveDetailStartDate, DateTime leaveDetailEndDate)
        {

            TotalLeave = totalLeave;
            AllowedLeave = allowedLeave;
            LastAccrualIncrement = lastAccrualIncrement;
            LeaveDetailStartDate = leaveDetailStartDate;
            LeaveDetailsEndDate = leaveDetailEndDate;
        }
        public leaveDetail(
           double totalLeave, double allowedLeave, double lastAccrualIncrement,
           DateTime leaveDetailStartDate, DateTime leaveDetailEndDate, double lCost, departmentModel dep)
        {

            TotalLeave = totalLeave;
            AllowedLeave = allowedLeave;
            LastAccrualIncrement = lastAccrualIncrement;
            LeaveDetailStartDate = leaveDetailStartDate;
            LeaveDetailsEndDate = leaveDetailEndDate;
            leaveCost = lCost;
            department = dep;
            company = dep.companyModel;
        }
        public leaveDetail() { }
    }
}
