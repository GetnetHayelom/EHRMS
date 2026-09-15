using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;
using System;
using System.Threading.Tasks;

namespace PIS2.Pages.TaxRate
{
    [Authorize(Roles ="HRADMIN")]
    public class CreateModel : PageModel
    {
        private readonly PISContext _context;

        [BindProperty]
        public taxRateModel TaxRate { get; set; }

        public CreateModel(PISContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            TaxRate = new taxRateModel
            {
                modifiedDate = DateTime.Now,
                taxStatus = mainStatus.Active
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("TaxRate.modifiedBy");
            if (!ModelState.IsValid)
                return Page();

            TaxRate.modifiedDate = DateTime.Now;
            TaxRate.modifiedBy = User?.Identity?.Name ?? "System";

            _context.TaxRates.Add(TaxRate);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
