using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.Budget
{
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;
        private ILogger<DetailsModel> _logger;
        public DetailsModel(PISContext context, ILogger<DetailsModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public BudgetPlan BudgetPlan { get; set; }
        public List<BudgetSummaryViewModel> Performance { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            BudgetPlan = await _context.BudgetPlans
                .Include(b => b.BudgetLines)
                    .ThenInclude(l => l.SubAccount)
                .FirstOrDefaultAsync(m => m.BudgetPlanID == id);

            if (BudgetPlan == null) return NotFound();

            var jl = _context.JournalEntryLines
                .Where(j => j.JournalEntry.EntryDate >= BudgetPlan.StartDate
                         && j.JournalEntry.EntryDate <= BudgetPlan.EndDate)
                .GroupBy(j => j.subAccountID)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(x => x.Debit)
                );

            // Calculate Performance: Budget vs Actuals
            Performance = BudgetPlan.BudgetLines.Select(line =>
            {
                jl.TryGetValue(line.subAccountID, out var spent);

                return new BudgetSummaryViewModel
                {
                    BudgetLineID = line.BudgetLineID,
                    BudgetID = line.BudgetPlanID,
                    SubAccountNumber = line.SubAccount?.subAccountNumber ?? "",
                    DepartmentName = line.SubAccount?.subAccountDescription ?? "",
                    Budgeted = line.AllocatedAmount,
                    ActualSpent = spent
                };
            }).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateLinesAsync(int id, List<BudgetLine> updatedLines)
        {
            // Simple update logic for the Budget Entry portion
            foreach (var line in updatedLines)
            {
                var dbLine = await _context.BudgetLines.FindAsync(line.BudgetLineID);
                if (dbLine != null)
                {
                    dbLine.AllocatedAmount = line.AllocatedAmount;
                    dbLine.Notes = line.Notes;
                    dbLine.modifiedDate = DateTime.Now;
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToPage(new { id });
        }
        public async Task<IActionResult> OnPostInitializeAsync()
        {
            if (!(User.IsInRole("MIE\\PMS_HRMANAGER") || User.IsInRole("MIE\\PMS_HRCLERK")))
            {
                return new JsonResult(new { success = false, message = "User not authorized to initialize budget plan!" });
            }
            var bplan = await _context.BudgetPlans.Include(b => b.BudgetLines).FirstOrDefaultAsync(b => b.BudgetPlanID == BudgetPlan.BudgetPlanID);
            
            if (bplan == null) { return new JsonResult(new { success = false, message = "Budget Plan not found!" }); }
            if (bplan.BudgetLines != null && bplan.BudgetLines.Any()) { return new JsonResult(new { success = false, message = "Budget plan already inistialized!" }); }
            if (bplan.Status != BudgetStatus.APPROVED) { return new JsonResult(new { success = false, message = "Budget plan is not approved!" }); }
            bplan.Status = BudgetStatus.ACTIVE;
            bplan.modifiedBy = User.Identity.Name;
            bplan.modifiedDate = DateTime.Now;

            // Fetch all Active Sub-Accounts (Departments)
            // This creates a "blank slate" for the budget entry immediately
            var activeSubAccounts = await _context.SubAccounts
                .Where(s => s.subAccountStatus == mainStatus.Active)
                .ToListAsync();

            if (activeSubAccounts.Any())
            {
                var initialLines = activeSubAccounts.Select(sa => new BudgetLine
                {
                    BudgetPlanID = BudgetPlan.BudgetPlanID,
                    subAccountID = sa.subAccountID,
                    AllocatedAmount = 0, // Set to 0 so they can edit in the next step
                    Notes = "Initial Setup",
                    modifiedBy = BudgetPlan.modifiedBy,
                    modifiedDate = DateTime.Now
                }).ToList();

                _context.BudgetLines.AddRange(initialLines);
                await _context.SaveChangesAsync();
            }
            // 4. Redirect to the Details page where they can now fill in the amounts
            return RedirectToPage("./Details", new { id = BudgetPlan.BudgetPlanID });
        }

        public async Task<IActionResult> OnPostApprove()
        {
            if (!User.IsInRole("MIE\\PMS_HRMANAGER"))
            {
                return new JsonResult(new { success = false, message = "User not authorized to approve budget!" });
            }
            var bplan = await _context.BudgetPlans.FirstOrDefaultAsync(b => b.BudgetPlanID == BudgetPlan.BudgetPlanID);

            if (bplan == null) { return new JsonResult(new { success = false, message = "Budget Plan not found!" }); }

            bplan.Status = BudgetStatus.APPROVED;
            bplan.modifiedBy = User.Identity.Name;
            bplan.modifiedDate = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Error: Failed to update budget plan #{BudgetPlan.BudgetPlanID}: User->{User.Identity.Name}");
                var msg = new { success = false, message = "Error approving budget plan!" };
                return new JsonResult(msg);
            }

            return new JsonResult(new { success = true, message = "Budget Approved Succesfully!" });
        }

        public async Task<IActionResult> OnPostDecline()
        {
            if (!User.IsInRole("MIE\\PMS_HRMANAGER"))
            {
                return new JsonResult(new { success = false, message = "User not authorized to approve budget!" });
            }
            var bplan = await _context.BudgetPlans.FirstOrDefaultAsync(b => b.BudgetPlanID == BudgetPlan.BudgetPlanID);

            if (bplan == null) { return new JsonResult(new { success = false, message = "Budget Plan not found!" }); }
            bplan.Status = BudgetStatus.DECLINED;
            bplan.modifiedBy = User.Identity.Name;
            bplan.modifiedDate = DateTime.Now;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Error: Failed to update budget plan status #{BudgetPlan.BudgetPlanID}: User->{User.Identity.Name}");
                var msg = new { success = false, message = "Error approving budget plan!" };
                return new JsonResult(msg);
            }

            return new JsonResult(new { success = true, message = "Budget Approved Succesfully!" });
        }
        public async Task<IActionResult> OnPostClose()
        {
            if (!User.IsInRole("MIE\\PMS_HRMANAGER"))
            {
                return new JsonResult(new { success = false, message = "User not authorized to approve budget!" });
            }
            var bplan = await _context.BudgetPlans.FirstOrDefaultAsync(b => b.BudgetPlanID == BudgetPlan.BudgetPlanID);
            if (bplan == null) { return new JsonResult(new { success = false, message = "Budget Plan not found!" }); }
            if (bplan.Status != BudgetStatus.ACTIVE) { return new JsonResult(new { success = false, message = "Budget plan is not active" }); }
            bplan.Status = BudgetStatus.CLOSED;
            bplan.modifiedBy = User.Identity.Name;
            bplan.modifiedDate = DateTime.Now;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Error: Failed to update budget plan status #{BudgetPlan.BudgetPlanID}: User->{User.Identity.Name}");
                var msg = new { success = false, message = "Error approving budget plan!" };
                return new JsonResult(msg);
            }

            return new JsonResult(new { success = true, message = "Budget Approved Succesfully!" });
        }

        public async Task<IActionResult> OnPostActivate()
        {
            var bplan = await _context.BudgetPlans.FirstOrDefaultAsync(b => b.BudgetPlanID == BudgetPlan.BudgetPlanID);
            if (bplan == null) { return new JsonResult(new { success = false, message = "Budget Plan not found!" }); }
            if (bplan.Status != BudgetStatus.APPROVED) { return new JsonResult(new { success = false, message = "Budget plan is not approved" }); }

            bplan.Status = BudgetStatus.ACTIVE;
            bplan.modifiedBy = User.Identity.Name;
            bplan.modifiedDate = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Error: Failed to activate budget plan #{BudgetPlan.BudgetPlanID}: User->{User.Identity.Name}");
                var msg = new { success = false, message = "Error activating budget plan!" };
                return new JsonResult(msg);
            }

            return new JsonResult(new { success = true, message = "Budget Activated Succesfully!" });
        }
    }
}
