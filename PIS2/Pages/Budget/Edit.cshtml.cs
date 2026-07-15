using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;
using PIS2.Pages.Shared;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PIS2.Pages.Budget
{
    [Authorize(Roles ="MIE\\PMS_HRCLERCK, MIE\\PMS_HRMANAGER")]
    public class EditModel : PageModel
    {
        private readonly PISContext _context;
        private readonly ILogger<EditModel> _logger;
        public EditModel(PISContext context, ILogger<EditModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public BudgetPlan BudgetPlan { get; set; } = default!;
        public SelectList StatusList { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var budgetplan = await _context.BudgetPlans
                .FirstOrDefaultAsync(m => m.BudgetPlanID == id);

            if (budgetplan == null) return NotFound();
            StatusList = new SelectList(Enum.GetValues(typeof(Enums.BudgetStatus)));
            BudgetPlan = budgetplan;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("modifiedBy");
            ModelState.Remove("BudgetPlan.modifiedBy");
            // 1. Set Metadata
            BudgetPlan.modifiedBy = User.Identity?.Name;
            BudgetPlan.modifiedDate = DateTime.Now;

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
                StatusList = new SelectList(Enum.GetValues(typeof(Enums.BudgetStatus)));
                return Page();
            }

            _context.Attach(BudgetPlan).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Error: Failed to update budget plan #{BudgetPlan.BudgetPlanID}: User->{User.Identity.Name}");
                var msg = new { success = false, message = "Error updating budget details!" };
                return new JsonResult(msg);
            }

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

            bplan.Status = Enums.BudgetStatus.APPROVED;
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

            return new JsonResult(new {success=true, message="Budget Approved Succesfully!"});
        }

        public async Task<IActionResult> OnPostDecline()
        {
            if (!User.IsInRole("MIE\\PMS_HRMANAGER")) 
            {
                return new JsonResult(new { success = false, message = "User not authorized to approve budget!" });
            }
            var bplan = await _context.BudgetPlans.FirstOrDefaultAsync(b => b.BudgetPlanID == BudgetPlan.BudgetPlanID);

            if (bplan == null) {return new JsonResult(new { success = false, message = "Budget Plan not found!" }); }
            bplan.Status = Enums.BudgetStatus.DECLINED;
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
            if (bplan.Status != Enums.BudgetStatus.ACTIVE) { return new JsonResult(new { success = false, message = "Budget plan is not active" }); }
            bplan.Status = Enums.BudgetStatus.CLOSED;
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
            if (bplan.Status != Enums.BudgetStatus.APPROVED) { return new JsonResult(new { success = false, message = "Budget plan is not approved" }); }

            bplan.Status = Enums.BudgetStatus.ACTIVE;
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
        private bool BudgetPlanExists(int id)
        {
            return _context.BudgetPlans.Any(e => e.BudgetPlanID == id);
        }
    }
}