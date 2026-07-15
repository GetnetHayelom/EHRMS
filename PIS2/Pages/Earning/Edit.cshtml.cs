using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
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
      
        public SelectList Accounts { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (!User.IsInRole("MIE\\PMS_HRADMIN"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }
            EarningType = await _context.EarningTypes.Include(e => e.Account).FirstOrDefaultAsync(e => e.earningTypeID==id);

            if (EarningType == null)
            {
                return NotFound();
            }
            getOptions(EarningType.accountID);
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
                getOptions(EarningType.accountID);
                return Page();
            }

            var earningTypeFromDb = await _context.EarningTypes.FindAsync(EarningType.earningTypeID);

            if (earningTypeFromDb == null)
            {
                getOptions(EarningType.accountID);
                return NotFound();
            }

            // Update fields manually to avoid overposting
            earningTypeFromDb.earningTypeName = EarningType.earningTypeName;
            earningTypeFromDb.earningTypeDescription = EarningType.earningTypeDescription;
            earningTypeFromDb.isTaxable = EarningType.isTaxable;
            earningTypeFromDb.isRecurring = EarningType.isRecurring;
            earningTypeFromDb.isPayroll = EarningType.isPayroll;
            earningTypeFromDb.earningTypeStatus = EarningType.earningTypeStatus;
            earningTypeFromDb.accountID = EarningType.accountID;
            earningTypeFromDb.modifiedBy = User.Identity?.Name ?? "System";
            earningTypeFromDb.modifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Earning Type updated successfully!";
            return RedirectToPage("Details", new { id=earningTypeFromDb.earningTypeID});
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
