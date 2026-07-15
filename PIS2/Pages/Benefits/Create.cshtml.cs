using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.Benefits
{
    public class CreateModel : PageModel
    {
        private readonly PISContext _context;

        public CreateModel(PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public payrollModel OtherPay { get; set; } = default!;

        public IActionResult OnGet()
        {
            // Populate Dropdowns
            ViewData["earningID"] = new SelectList(_context.Earnings, "earningID", "earningReference");
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Re-populate dropdowns if validation fails
                ViewData["earningID"] = new SelectList(_context.Earnings, "earningID", "earningReference");
                return Page();
            }

            // Set system-managed fields
            OtherPay.payrollStatus = Enums.payrollStatus.PENDING;
            // Assuming you have a way to get the current user, e.g., User.Identity.Name
            OtherPay.modifiedBy = User.Identity?.Name ?? "System";

            _context.Payrolls.Add(OtherPay);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}