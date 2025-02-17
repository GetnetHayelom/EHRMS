using Microsoft.EntityFrameworkCore;

namespace PIS2.Models
{
    public class leavePerYear
    {
        private readonly PIS2.Models.PISContext _context;

        public leavePerYear(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; } = DateTime.Now;
        public double startingLeaveAmount { get; set; } = 0;
        public double accruedLeaveAmount { get; set; } = 0;
        public double usedLeaveAmount { get; set; } = 0;
        public double rollOverLeave { get; set; }= 0;
        public double remainingLeaveAmount{
            get
            {
                return (startingLeaveAmount + accruedLeaveAmount) - usedLeaveAmount;
            }
        }
        public double? totalLeaveAmount { get; set; }
        public leavePerYear() { }
        //public leavePerYear(DateTime startDate,DateTime endDate, employmentModel employee, double accrual)
        //{
        //    //employee = _context.Employments.Where(e => e.employmentID == employee.personID).Include(employmentTypeModel).FirstOrDefault();
        //    //    ..
        //    //List<leaveModel> leaves = _context.Leaves.Where(l => l.employmentID == employee.employmentID && l.leaveStartDate >= startDate && l.leaveEndDate <= endDate).ToList();
        //    double baseLeave = employee.employmentTypeModel.employmentBaseLeave;
        //    AccruedLeaveAmount = employee.employmentDate.Year - startDate.Year +1;
        //}
        //public override string ToString()
        //{
        //    return $"Start Date: {StartDate.ToShortDateString()}, End Date: {EndDate.ToShortDateString()}, Accrued Leave: {AccruedLeaveAmount:0.#}, Allowed Leave: {AllowedLeaveAmount:0.#}";
        //}
    }
}
