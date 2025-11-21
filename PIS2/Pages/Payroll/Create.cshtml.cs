using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PIS2.Models;
using Microsoft.EntityFrameworkCore;

namespace PIS2.Pages.Payroll
{
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
            Companies = await _db.Companies.Where(c => c.companyStatus == mainStatus.Active).ToListAsync();
            Payroll.payrollStatus = payrollStatus.PENDING; // default status
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Payroll.modifiedBy");
            Payroll.modifiedBy = User.Identity.Name ?? "system";
            Payroll.payrollMonth = Payroll.StartDate.Month.ToString();
            
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
