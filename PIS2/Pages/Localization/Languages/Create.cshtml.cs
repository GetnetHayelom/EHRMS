using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models.Foundation;

namespace PIS2.Pages.Localization.Languages
{
    [Authorize(Roles = "ADMIN")]
    public class CreateModel : PageModel
    {
        private readonly PISContext _context;

    public CreateModel(PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public LanguageModel Language { get; set; }
            = new LanguageModel();

        public void OnGet()
        {
            Language.isActive = true;
            Language.isDefault = false;
            Language.isRTL = false;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var codeExists = await _context.Languages
                .AnyAsync(x =>
                    x.languageCode == Language.languageCode);

            if (codeExists)
            {
                ModelState.AddModelError(
                    "Language.languageCode",
                    "A language with this code already exists.");

                return Page();
            }

            // Only one default language is allowed.
            if (Language.isDefault)
            {
                var existingDefault =
                    await _context.Languages
                        .FirstOrDefaultAsync(x => x.isDefault);

                if (existingDefault != null)
                {
                    ModelState.AddModelError(
                        "Language.isDefault",
                        $"'{existingDefault.languageName}' is already the default language.");

                    return Page();
                }
            }

            _context.Languages.Add(Language);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Language created successfully.";

            return RedirectToPage("./Index");
        }
    }


}
