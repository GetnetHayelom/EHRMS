using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;

namespace PIS2.Pages.Deduction
{
    public class EditModel : PageModel
    {
        private readonly PISContext _context;

        public EditModel(PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public deductionType DeductionType { get; set; } = new deductionType();
        public SelectList Accounts { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            DeductionType = await _context.DeductionTypes.FindAsync(id);
            if (DeductionType == null)
            {
                return NotFound();
            }
            getOptions(DeductionType.accountID);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(DeductionType).State = EntityState.Modified;
            DeductionType.modifiedBy = User?.Identity?.Name ?? "System";

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Deduction Type updated successfully!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.DeductionTypes.Any(e => e.deductionTypeID == DeductionType.deductionTypeID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Details", new { id=DeductionType.deductionTypeID});
        }
        private void getOptions(int? id)
        {
            var accounts = _context.Accounts
                .Where(a => a.accountStatus == mainStatus.Active)
                .OrderBy(a => a.accountName)
                .ToList();
            Accounts = new SelectList(accounts, "accountID", "accountName", id);
        }
    }
}
