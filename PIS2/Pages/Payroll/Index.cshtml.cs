using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

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

        public async Task OnGetAsync()
        {
            Payrolls = await _db.Payrolls.Include(p => p.companyModel).ToListAsync();
        }

        public async Task<IActionResult> OnPostGenerateAsync(int id)
        {
            await _payrollService.GeneratePayrollAsync(id, User.Identity.Name);
            // mark as posted
            await _payrollService.PostPayrollAsync(id, User.Identity.Name ?? "system");
            Console.WriteLine("Payroll Posted ############################");
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCompleteAsync(int id)
        {
            await _payrollService.CompletePayrollAsync(id, User.Identity.Name);
            return RedirectToPage();
        }
    }

}
