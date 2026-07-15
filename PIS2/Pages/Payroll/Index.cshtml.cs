using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using Microsoft.AspNetCore.Authorization;
using PIS2.Services;
using PIS2.Data;

namespace PIS2.Pages.Payroll
{
    [Authorize(Roles = @"MIE\PMS_HRCLERK,MIE\PMS_HRMANAGER,MIE\PMS_PAYROLL")]
    public class IndexModel : PageModel
    {
        private readonly PISContext _db;
        private readonly PayrollService _payrollService;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(PISContext db, PayrollService payrollService, ILogger<IndexModel> logger)
        {
            _db = db;
            _payrollService = payrollService;
            _logger = logger;
        }

        public IList<payrollModel> Payrolls { get; set; } = new List<payrollModel>();
        
        public async Task OnGetAsync()
        {
            Payrolls = await _db.Payrolls.Include(p => p.companyModel).Where(p => p.IsPayroll).ToListAsync();
        }

        public async Task<IActionResult> OnPostGenerateAsync(int id)
        {
            if (!User.IsInRole("MIE\\PMS_PAYROLL")) return RedirectToPage("/Shared/AccessDenied");
            var selectedPayroll = await _db.Payrolls.FirstOrDefaultAsync(r => r.payrollID == id);
            if(!selectedPayroll.IsPayroll)
            {
                _logger.LogError("Error: Can not generate none payroll payments. PaymentID: {ID} UserName: {UserName}", id, User.Identity.Name);
                return new JsonResult(new { success=true, message="Payemnt is not payroll!"});
            }
            if(!(selectedPayroll.payrollStatus == Enums.payrollStatus.APPROVED || selectedPayroll.payrollStatus == Enums.payrollStatus.PROCESSED))
            {
                _logger.LogError("Error: Payroll status must be APPROVED or PROCCESSED. PaymentID: {ID} UserName: {UserName}", id, User.Identity.Name);
                return new JsonResult(new { success = true, message = "Payroll status must be APPROVED or PROCCESSED!" });
            }
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

            payroll.payrollStatus = Enums.payrollStatus.APPROVED;
            payroll.modifiedBy = User.Identity.Name ?? "system";
            await _db.SaveChangesAsync();

            return RedirectToPage(new { id });
        }
    }

}
