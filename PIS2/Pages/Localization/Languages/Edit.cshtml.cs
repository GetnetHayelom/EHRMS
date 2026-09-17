using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models.Foundation;

namespace PIS2.Pages.Localization.Languages
{
    [Authorize(Roles = "ADMIN")]
    public class EditModel : PageModel
    {
        private readonly PISContext _context;


    public EditModel(PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public LanguageModel Language { get; set; }
            = new LanguageModel();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var language = await _context.Languages
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.languageID == id);

            if (language == null)
            {
                return NotFound();
            }

            Language = language;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (id != Language.languageID)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var languageExists = await _context.Languages
                .AnyAsync(x =>
                    x.languageCode == Language.languageCode &&
                    x.languageID != id);

            if (languageExists)
            {
                ModelState.AddModelError(
                    "Language.languageCode",
                    "A language with this code already exists.");

                return Page();
            }

            var existingLanguage =
                await _context.Languages
                    .FirstOrDefaultAsync(x =>
                        x.languageID == id);

            if (existingLanguage == null)
            {
                return NotFound();
            }

            // --------------------------------------------------------
            // DEFAULT LANGUAGE
            // --------------------------------------------------------

            if (Language.isDefault)
            {
                var existingDefault =
                    await _context.Languages
                        .FirstOrDefaultAsync(x =>
                            x.isDefault &&
                            x.languageID != id);

                if (existingDefault != null)
                {
                    ModelState.AddModelError(
                        "Language.isDefault",
                        $"'{existingDefault.languageName}' is already the default language.");

                    return Page();
                }
            }
            else
            {
                // Prevent the system from having no default language.
                if (existingLanguage.isDefault)
                {
                    ModelState.AddModelError(
                        "Language.isDefault",
                        "The default language cannot be disabled. Set another language as default first.");

                    return Page();
                }
            }

            // --------------------------------------------------------
            // UPDATE
            // --------------------------------------------------------

            existingLanguage.languageCode =
                Language.languageCode;

            existingLanguage.languageName =
                Language.languageName;

            existingLanguage.nativeName =
                Language.nativeName;

            existingLanguage.isRTL =
                Language.isRTL;

            existingLanguage.isActive =
                Language.isActive;

            existingLanguage.isDefault =
                Language.isDefault;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Language updated successfully.";

            return RedirectToPage("./Index");
        }
    }


}
