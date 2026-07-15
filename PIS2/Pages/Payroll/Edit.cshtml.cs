using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using PIS2.Models;
using PIS2.Data;
using PIS2.Enums;

namespace PIS2.Pages.Payroll
{
    public class EditModel : PageModel
    {
        private readonly PISContext _db;
        private ILogger<EditModel> _logger;
        public EditModel(PISContext db, ILogger<EditModel> logger)
        {
            _db = db;
            _logger = logger;
        }

        [BindProperty]
        public payrollModel Payroll { get; set; } = new payrollModel();

        public SelectList Companies { get; set; }

        [Authorize(Roles = @"MIE\PMS_HRCLERK,MIE\PMS_HRMANAGER,MIE\PMS_PAYROLL")]
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var payroll = await _db.Payrolls.FirstOrDefaultAsync(m => m.payrollID == id);
            if (payroll == null) return NotFound();

            if (payroll.payrollStatus != payrollStatus.PENDING) 
            {
                throw new ArgumentException("Only pending payroll can be edited!");
            }
            Payroll = payroll;
            var comps =await _db.Companies.Where(c => c.companyStatus == mainStatus.Active).ToListAsync();
            Companies = new SelectList(comps, "companyID", "companyName");
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.IsInRole("MIE\\PMS_PAYROLL")) return RedirectToPage("/Shared/AccessDenied");
            var payroll = await _db.Payrolls.FirstOrDefaultAsync(m => m.payrollID == Payroll.payrollID);
            if (payroll == null) return NotFound();

            if (payroll.payrollStatus != payrollStatus.PENDING)
            {
                throw new ArgumentException("Only pending payroll can be edited!");
            }
            
            ModelState.Remove("Payroll.modifiedBy");
            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        Console.WriteLine($"{kv.Key} --> {error.ErrorMessage}");
                        TempData["message"] = ("Error", $"{kv.Key} --> {error.ErrorMessage}");
                        _logger.LogError(error.ErrorMessage, $"Error validation failed for {kv.Key} payrollID-{Payroll.payrollID}: User->{User.Identity.Name}");
                    }
                }
                return Page();
            }
            payroll.modifiedBy = User.Identity.Name;
            payroll.companyID = Payroll.companyID;
            payroll.payrollName = Payroll.payrollName;
            payroll.StartDate = Payroll.StartDate;
            payroll.EndDate = Payroll.EndDate;
            
            await _db.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
