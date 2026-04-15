using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PIS2.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using PIS2.Data;

namespace PIS2.Pages.Deduction

{
    public class CreateModel : PageModel
    {
        private readonly PISContext _db;

        public CreateModel(PISContext db)
        {
            _db = db;
        }

        [BindProperty]
        public deductionType DeductionType { get; set; } = new deductionType();
        public SelectList Accounts { get; set; }
        public void OnGet()
        {
            getOptions(null);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.IsInRole("MIE\\PMS_HRADMIN"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }

            ModelState.Remove("DeductionType.modifiedBy");
            if (!ModelState.IsValid) { getOptions(null);  return Page(); };

            DeductionType.modifiedBy = User.Identity.Name ?? "system";
            DeductionType.deductionStatus = mainStatus.Active;
            

            _db.DeductionTypes.Add(DeductionType);
            await _db.SaveChangesAsync();

            return RedirectToPage("Index");
        }
        private void getOptions(int? id)
        {
            var accounts = _db.Accounts
                .Where(a => a.accountStatus == mainStatus.Active)
                .OrderBy(a => a.accountName)
                .ToList();
            Accounts = new SelectList(accounts, "accountID", "accountName", id);
        }
    }
}
