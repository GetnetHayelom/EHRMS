using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models.Finance;

namespace PIS2.Pages.Budget
{
    public class LineCreateModel : PageModel
    {
        private readonly PISContext _context;
        public LineCreateModel(PISContext context) => _context = context;

        [BindProperty]
        public BudgetLine BudgetLine { get; set; }

        public BudgetPlan ParentPlan { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            ParentPlan = await _context.BudgetPlans.FindAsync(id);
            if (ParentPlan == null) return NotFound();

            // Filter out sub-accounts already budgeted in this plan
            var existingSubAccountIds = await _context.BudgetLines
                .Where(l => l.BudgetPlanID == id)
                .Select(l => l.subAccountID)
                .ToListAsync();

            ViewData["subAccountID"] = new SelectList(_context.SubAccounts
                .Where(s => !existingSubAccountIds.Contains(s.subAccountID)), "subAccountID", "subAccountName");

            BudgetLine = new BudgetLine { BudgetPlanID = id };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            BudgetLine.modifiedBy = User.Identity?.Name ?? "System";
            BudgetLine.modifiedDate = DateTime.Now;

            _context.BudgetLines.Add(BudgetLine);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Account/Budgeting/Details", new { id = BudgetLine.BudgetPlanID });
        }
    }
}