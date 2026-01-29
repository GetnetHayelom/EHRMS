using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PIS2.Models;

namespace PIS2.Pages.Earning
{
    [Authorize(Roles = "MIE\\PMS_HRADMIN")]
    public class CreateModel : PageModel
    {
        private readonly PISContext _context;

        public CreateModel(PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public earningType EarningType { get; set; } = new earningType();

        public void OnGet()
        {
            
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.IsInRole("MIE\\PMS_HRADMIN"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }
            ModelState.Remove("EarningType.modifiedBy");
            EarningType.modifiedBy = User.Identity.Name;
            EarningType.modifiedDate = DateTime.Now;
            if (!ModelState.IsValid)
                return Page();

            _context.EarningTypes.Add(EarningType);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
