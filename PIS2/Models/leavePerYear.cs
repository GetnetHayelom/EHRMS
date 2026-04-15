using Microsoft.EntityFrameworkCore;
using PIS2.Data;

namespace PIS2.Models
{
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
        public decimal rollOverLeave { get; set; }= 0;
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
}
