using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using Microsoft.AspNetCore.Authorization;


namespace PIS2.Pages.Payroll
{
    public class DetailsModel : PageModel
    {
        private readonly PISContext _db;
        private readonly PayrollService _payrollService;

        public DetailsModel(PISContext db, PayrollService payrollService)
        {
            _db = db;
            _payrollService = payrollService;
        }

        public payrollModel Payroll { get; set; } = new payrollModel();
        public IList<payrollPay> PayrollPays { get; set; } = new List<payrollPay>();
        public List<earningType> EarningTypes { get; set; }
        public List<deductionType> DeductionTypes { get; set; }

        [Authorize(Roles = @"MIE\PMS_HRCLERK,MIE\PMS_HRMANAGER,MIE\PMS_PAYROLL")]
        public async Task<IActionResult> OnGetAsync(int id)
        {
            EarningTypes = await _db.EarningTypes.ToListAsync();
            DeductionTypes = await _db.DeductionTypes.ToListAsync();

            Payroll = await _db.Payrolls.Include(p => p.companyModel).FirstOrDefaultAsync(p => p.payrollID == id);

            if (Payroll == null) return NotFound();

            if (Payroll.payrollStatus == payrollStatus.POSTED || Payroll.payrollStatus == payrollStatus.COMPLETED)
            {
                PayrollPays = await _db.PayrollPays
                    .Include(pp => pp.EmploymentModel).ThenInclude(e => e.personModel)
                    .Include(pp => pp.EmploymentModel).ThenInclude(e => e.JobPlacements)
                    .Include(e => e.EarningRecords).ThenInclude(er => er.earningType)
                    .Include(e => e.DeductionRecords).ThenInclude(dr => dr.DeductionType)
                    .Where(pp => pp.payrollID == id)
                    .ToListAsync();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostApproveAsync(int id)
        {
            if (!User.IsInRole("MIE\\PMS_PAYROLL")) return RedirectToPage("/Shared/AccessDenied");
            var payroll = await _db.Payrolls.FirstOrDefaultAsync(p => p.payrollID == id);
            if (payroll == null) return NotFound();

            payroll.payrollStatus = payrollStatus.APPROVED;
            payroll.modifiedBy = User.Identity.Name ?? "system";
            await _db.SaveChangesAsync();

            return RedirectToPage(new { id });
        }

        public async Task<IActionResult> OnPostPostAsync(int id)
        {
            if (!User.IsInRole("MIE\\PMS_PAYROLL")) return RedirectToPage("/Shared/AccessDenied");
            var payroll = await _db.Payrolls.FirstOrDefaultAsync(p => p.payrollID == id);
            if (payroll == null) return NotFound();
            if (payroll.payrollStatus == payrollStatus.PENDING) return BadRequest("Payroll must be approved first.");

            // Generate payroll pays
            await _payrollService.GeneratePayrollAsync(payroll.payrollID, User.Identity.Name ?? "system");
            Console.WriteLine("Payroll Generated ############################");
            // mark as posted
            await _payrollService.PostPayrollAsync(payroll.payrollID, User.Identity.Name ?? "system");
            Console.WriteLine("Payroll Posted ############################");

            return RedirectToPage(new { id });
        }
    }
}
