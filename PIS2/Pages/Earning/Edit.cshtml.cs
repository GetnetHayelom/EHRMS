using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Earning
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
        public earningType EarningType { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            EarningType = await _context.EarningTypes.FindAsync(id);

            if (EarningType == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.IsInRole("MIE\\PMS_HRADMIN"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }

            ModelState.Remove("EarningType.modifiedBy");
            EarningType.modifiedBy = User.Identity.Name;

            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        Console.WriteLine($"{kv.Key} --> {error.ErrorMessage}");
                    }
                    Console.WriteLine(kv.ToString());
                }
                return Page();
            }

            var earningTypeFromDb = await _context.EarningTypes.FindAsync(EarningType.earningTypeID);

            if (earningTypeFromDb == null)
            {
                return NotFound();
            }

            // Update fields manually to avoid overposting
            earningTypeFromDb.earningTypeName = EarningType.earningTypeName;
            earningTypeFromDb.earningTypeDescription = EarningType.earningTypeDescription;
            earningTypeFromDb.isTaxable = EarningType.isTaxable;
            earningTypeFromDb.isRecurring = EarningType.isRecurring;
            earningTypeFromDb.isPayroll = EarningType.isPayroll;
            earningTypeFromDb.earningTypeStatus = EarningType.earningTypeStatus;
            earningTypeFromDb.modifiedBy = User.Identity?.Name ?? "System";
            earningTypeFromDb.modifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Earning Type updated successfully!";
            return RedirectToPage("Details", new { id=earningTypeFromDb.earningTypeID});
        }
    }
}
