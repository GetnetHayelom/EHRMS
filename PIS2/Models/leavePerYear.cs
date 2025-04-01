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
        public double remainingLeaveCost { get; set; }
        
        public double? totalLeaveAmount { get; set; }
        public leavePerYear() { }
        
    }
}
