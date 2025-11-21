using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PIS2.Models;

namespace PIS2.Pages.Earning
{
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
            // Default initialization if needed
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("EarningType.modifiedBy");
            EarningType.modifiedBy = User.Identity.Name;
            if (!ModelState.IsValid)
                return Page();

            _context.EarningTypes.Add(EarningType);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
