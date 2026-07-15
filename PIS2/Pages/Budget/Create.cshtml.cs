using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;
using System.Threading.Tasks;

namespace PIS2.Pages.Budget
{
    public class CreateModel : PageModel
    {
        private readonly PISContext _context;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(PISContext context, ILogger<CreateModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public BudgetPlan BudgetPlan { get; set; }

        // To allow the user to select the initial status
        public SelectList StatusList { get; set; }
        public SelectList SubAccountList { get; set; }

        public async Task<IActionResult> OnGet()
        {
            // Initialize with default dates (Current Year)
            BudgetPlan = new BudgetPlan
            {
                StartDate = new DateTime(DateTime.Now.Year, 1, 1),
                EndDate = new DateTime(DateTime.Now.Year, 12, 31),
                Status = BudgetStatus.HOLD
            };

            StatusList = new SelectList(Enum.GetValues(typeof(BudgetStatus)));
            var deps = await _context.Departments.Where(d => d.departmentStatus == mainStatus.Active).ToListAsync();
            SubAccountList = new SelectList(deps, "subAccountID", "departmentName");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("modifiedBy");
            ModelState.Remove("BudgetPlan.modifiedBy");
            // 1. Set Metadata
            BudgetPlan.modifiedBy = User.Identity?.Name;
            BudgetPlan.modifiedDate = DateTime.Now;
            BudgetPlan.Status = BudgetStatus.HOLD;
            //MODEL VALIDATION ERROR
            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        Console.WriteLine($"{kv.Key} --> {error.ErrorMessage}");
                        TempData["message"] = ("Error", $"{kv.Key} --> {error.ErrorMessage}");
                        _logger.LogError(error.ErrorMessage, $"Error: Validation failed for {kv.Key}: User->{User.Identity.Name}");
                    }
                }
                StatusList = new SelectList(Enum.GetValues(typeof(BudgetStatus)));
                return Page();
            }



            // 2. Add the Header to the Context
            _context.BudgetPlans.Add(BudgetPlan);

            // We save here first to generate the BudgetPlanID
            await _context.SaveChangesAsync();            

            // 4. Redirect to the Details page where they can now fill in the amounts
            return RedirectToPage("./Details", new { id = BudgetPlan.BudgetPlanID });
        }

        
    }
}