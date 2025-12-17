using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PIS2.Models;

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

        public void OnGet()
        {
            // nothing to initialize for now
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.IsInRole("MIE\\PMS_HRADMIN"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }

            ModelState.Remove("DeductionType.modifiedBy");
            if (!ModelState.IsValid) return Page();

            DeductionType.modifiedBy = User.Identity.Name ?? "system";
            DeductionType.deductionStatus = mainStatus.Active;
            

            _db.DeductionTypes.Add(DeductionType);
            await _db.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
