using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.Budget
{
    public class LineEditModel : PageModel
    {
        private readonly PISContext _context;
        public LineEditModel(PISContext context) => _context = context;

        [BindProperty]
        public BudgetLine BudgetLine { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            BudgetLine = await _context.BudgetLines
                .Include(l => l.SubAccount)
                .Include(l => l.BudgetPlan)
                .FirstOrDefaultAsync(m => m.BudgetLineID == id);

            if (BudgetLine == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var lineToUpdate = await _context.BudgetLines.FindAsync(BudgetLine.BudgetLineID);

            if (lineToUpdate == null) return NotFound();

            lineToUpdate.AllocatedAmount = BudgetLine.AllocatedAmount;
            lineToUpdate.Notes = BudgetLine.Notes;
            lineToUpdate.modifiedBy = User.Identity?.Name ?? "System";
            lineToUpdate.modifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return RedirectToPage("/Budget/LineDetails", new { id = lineToUpdate.BudgetLineID });
        }
    }
}