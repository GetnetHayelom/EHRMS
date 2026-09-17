using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models.Finance;

namespace PIS2.Pages.Account.Budgeting
{
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;
        public IndexModel(PISContext context) => _context = context;

        public List<BudgetPlanSummary> Plans { get; set; } = new();
        public DashboardStats Stats { get; set; } = new();

        

        public async Task OnGetAsync()
        {
            var plans = await _context.BudgetPlans
                .Include(b => b.BudgetLines)
                .OrderByDescending(b => b.StartDate)
                .ToListAsync();

            foreach (var p in plans)
            {
                var allocated = p.BudgetLines.Sum(l => l.AllocatedAmount);
                var spent = await _context.JournalEntryLines
                    .Where(j => j.JournalEntry.EntryDate >= p.StartDate && j.JournalEntry.EntryDate <= p.EndDate)
                    .SumAsync(j => j.Debit);

                Plans.Add(new BudgetPlanSummary
                {
                    Plan = p,
                    TotalAllocated = allocated,
                    TotalSpent = spent
                });
            }

            // Dashboard Aggregates
            Stats.ActivePlansCount = plans.Count(x => x.Status == Enums.BudgetStatus.ACTIVE);
            Stats.TotalGlobalBudget = Plans.Sum(x => x.TotalAllocated);
            Stats.TotalGlobalSpent = Plans.Sum(x => x.TotalSpent);
        }
    }
    public class BudgetPlanSummary
    {
        public BudgetPlan Plan { get; set; }
        public decimal TotalAllocated { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal Utilization => TotalAllocated > 0 ? (TotalSpent / TotalAllocated) * 100 : 0;
    }

    public class DashboardStats
    {
        public int ActivePlansCount { get; set; }
        public decimal TotalGlobalBudget { get; set; }
        public decimal TotalGlobalSpent { get; set; }
        public decimal GlobalRemaining => TotalGlobalBudget - TotalGlobalSpent;
    }
}