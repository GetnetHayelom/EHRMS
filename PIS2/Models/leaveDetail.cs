namespace PIS2.Models
{
    public class leaveDetail
    {
        public employmentModel Employee;
        public double TotalLeave, AllowedLeave, LastAccrualIncrement;
        public DateTime LeaveDetailStartDate, LeaveDetailsEndDate;
        public List<leavePerYear> AnnualLeaveHistory;
        
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
        public leaveDetail() { }
    }
}
