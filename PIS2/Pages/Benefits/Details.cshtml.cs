using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Benefits
{
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

        public DetailsModel(PISContext context)
        {
            _context = context;
        }

        public otherPay OtherPay { get; set; } = default!;
        public List<AuditLog> History { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            // Fetch the main record
            OtherPay = await _context.OtherPayments
                .Include(o => o.earningModel).ThenInclude(e => e.EmploymentModel).ThenInclude(e => e.personModel)
                .Include(e => e.earningModel).ThenInclude(e => e.earningType)
                .FirstOrDefaultAsync(m => m.paymentID == id);

            if (OtherPay == null) return NotFound();

            // Fetch Audit Logs for this specific record
            History = await _context.AuditLogs
                .Where(a => a.TableName == "otherPay" && a.RecordID == id)
                .OrderByDescending(a => a.ModifiedDate)
                .ToListAsync();

            return Page();
        }
    }
}