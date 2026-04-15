using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;
using System;
using System.Threading.Tasks;

namespace PIS2.Pages.TaxRate
{
    [Authorize(Roles = "MIE\\PMS_HRADMIN")]
    public class EditModel : PageModel
    {
        private readonly PISContext _context;

        public EditModel(PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public taxRateModel TaxRate { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            TaxRate = await _context.TaxRates.FirstOrDefaultAsync(t => t.taxRateID == id);
            if (TaxRate == null) return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("TaxRate.modifiedBy");
            if (!ModelState.IsValid) return Page();

            var existingTaxRate = await _context.TaxRates.FirstOrDefaultAsync(t => t.taxRateID == TaxRate.taxRateID);
            if (existingTaxRate == null) return NotFound();

            // update properties
            existingTaxRate.amount = TaxRate.amount;
            existingTaxRate.taxRate = TaxRate.taxRate;
            existingTaxRate.deduction = TaxRate.deduction;
            existingTaxRate.reference = TaxRate.reference;
            existingTaxRate.taxStatus = TaxRate.taxStatus;
            existingTaxRate.modifiedDate = DateTime.Now;
            existingTaxRate.modifiedBy = User?.Identity?.Name ?? "System";

            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
