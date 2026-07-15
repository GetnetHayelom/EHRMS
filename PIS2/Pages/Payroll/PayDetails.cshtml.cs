using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models; // Change to your actual namespace

namespace PIS2.Pages.Payroll
{
    public class PayDetailsModel : PageModel
    {
        private readonly PISContext _context;

        public PayDetailsModel(PISContext context)
        {
            _context = context;
        }

        public payrollPay PayrollPay { get; set; }
        public jobPlacementModel jobp { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            PayrollPay = await _context.PayrollPays
                .Include(p => p.payrollModel)
                .Include(p => p.EmploymentModel)
                    .ThenInclude(e => e.personModel) // Assuming Employment has an Employee relation
                .Include(p => p.CreditAccount)
                .Include(p => p.DebitAccount)
                .Include(p => p.EarningRecords)
                    .ThenInclude(er => er.earningType)
                .Include(p => p.DeductionRecords)
                    .ThenInclude(dr => dr.DeductionType)
                .FirstOrDefaultAsync(m => m.payrollPayID == id);

            if (PayrollPay == null)
            {
                return NotFound();
            }
            jobp = _context.JobPlacements.FirstOrDefault(j => j.employmentID == PayrollPay.employmentID && j.jobPlacementStatus == mainStatus.Active);
            return Page();
        }
    }
}