using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PIS2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using PIS2.Data;
using PIS2.Enums;

namespace PIS2.Pages.Payroll
{
    [Authorize(Roles = @"MIE\HRCLERK,MIE\HRMANAGER,MIE\PAYROLL")]
    public class CreateModel : PageModel
    {
        private readonly PISContext _db;

        
        public CreateModel(PISContext db)
        {
            _db = db;
        }

        [BindProperty]
        public payrollModel Payroll { get; set; } = new payrollModel();

        public IList<companyModel> Companies { get; set; } = new List<companyModel>();

        
        public async Task OnGetAsync()
        {
            var lastPayroll = await _db.Payrolls.Where(p => p.payrollStatus == payrollStatus.COMPLETED).OrderBy(p => p.EndDate).LastOrDefaultAsync();
            Companies = await _db.Companies.Where(c => c.companyStatus == mainStatus.Active).ToListAsync();
            Payroll.payrollStatus = payrollStatus.PENDING;// default status
            Payroll.StartDate = lastPayroll?.EndDate.AddDays(1) ?? DateTime.Now;
            Payroll.EndDate = new DateTime(
                Payroll.StartDate.Year,
                Payroll.StartDate.Month,
                DateTime.DaysInMonth(Payroll.StartDate.Year, Payroll.StartDate.Month)
            );
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.IsInRole("PAYROLL")) return RedirectToPage("/Shared/AccessDenied");
            ModelState.Remove("Payroll.modifiedBy");
            Payroll.modifiedBy = User.Identity.Name ?? "system";
            Payroll.payrollMonth = Payroll.StartDate.Month.ToString();
            Payroll.IsPayroll = true;

            if (!ModelState.IsValid)
            {
                Companies = await _db.Companies.Where(c => c.companyStatus == mainStatus.Active).ToListAsync();
                return Page();
            }
            
            _db.Payrolls.Add(Payroll);
            await _db.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
