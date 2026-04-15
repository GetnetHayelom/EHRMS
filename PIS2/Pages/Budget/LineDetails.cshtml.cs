using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.Budget
{
    public class LineDetailsModel : PageModel
    {
        private readonly PISContext _context;
        public LineDetailsModel(PISContext context) => _context = context;

        public BudgetLine BudgetLine { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            BudgetLine = await _context.BudgetLines
                .Include(l => l.BudgetPlan)
                .Include(l => l.SubAccount)
                .FirstOrDefaultAsync(m => m.BudgetLineID == id);

            if (BudgetLine == null) return NotFound();
            return Page();
        }
    }
}
