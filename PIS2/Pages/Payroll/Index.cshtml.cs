using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using Microsoft.AspNetCore.Authorization;
using PIS2.Services;
using PIS2.Data;

namespace PIS2.Pages.Payroll
{
    public class IndexModel : PageModel
    {
        private readonly PISContext _db;
        private readonly PayrollService _payrollService;

        public IndexModel(PISContext db, PayrollService payrollService)
        {
            _db = db;
            _payrollService = payrollService;
        }

        public IList<payrollModel> Payrolls { get; set; } = new List<payrollModel>();
        

        [Authorize(Roles = @"MIE\PMS_HRCLERK,MIE\PMS_HRMANAGER,MIE\PMS_PAYROLL")]
        public async Task OnGetAsync()
        {
            
            Payrolls = await _db.Payrolls.Include(p => p.companyModel).ToListAsync();
        }

        public async Task<IActionResult> OnPostGenerateAsync(int id)
        {
            if (!User.IsInRole("MIE\\PMS_PAYROLL")) return RedirectToPage("/Shared/AccessDenied");
            await _payrollService.GeneratePayrollAsync(id, User.Identity.Name);
            // mark as posted
            //await _payrollService.PostPayrollAsync(id, User.Identity.Name ?? "system");
            Console.WriteLine("Payroll Posted ############################");
            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostPostAsync(int id)
        {
            if (!User.IsInRole("MIE\\PMS_HRMANAGER")) return RedirectToPage("/Shared/AccessDenied");
            // mark as posted
            await _payrollService.PostPayrollAsync(id, User.Identity.Name ?? "system");
            
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCompleteAsync(int id)
        {
            if (!User.IsInRole("MIE\\PMS_FINANCE")) return RedirectToPage("/Shared/AccessDenied");
            await _payrollService.CompletePayrollAsync(id, User.Identity.Name);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostApproveAsync(int id)
        {
            if (!User.IsInRole("MIE\\PMS_HRMANAGER")) return RedirectToPage("/Shared/AccessDenied");
            var payroll = await _db.Payrolls.FirstOrDefaultAsync(p => p.payrollID == id);
            if (payroll == null) return NotFound();

            payroll.payrollStatus = payrollStatus.APPROVED;
            payroll.modifiedBy = User.Identity.Name ?? "system";
            await _db.SaveChangesAsync();

            return RedirectToPage(new { id });
        }
    }

}
