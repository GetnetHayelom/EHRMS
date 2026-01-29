using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Benefits
{
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;

        public IndexModel(PISContext context)
        {
            _context = context;
        }
        public IList<otherPay> OtherPayments { get; set; } = default!;

        // Summary properties for the dashboard
        public decimal TotalGross { get; set; }
        public decimal TotalNet { get; set; }

        public async Task OnGetAsync()
        {
            OtherPayments = await _context.OtherPayments
                .Include(o => o.earningModel).ThenInclude(e => e.EmploymentModel).ThenInclude(e => e.personModel)
                .Include(e => e.earningModel).ThenInclude(e => e.earningType)
                .OrderByDescending(o => o.paymentID)
                .ToListAsync();

            TotalGross = OtherPayments.Sum(x => x.GrossPay);
            TotalNet = OtherPayments.Sum(x => x.NetPay);
        }
    }
}
