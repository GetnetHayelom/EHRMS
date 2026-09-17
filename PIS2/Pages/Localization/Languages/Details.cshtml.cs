using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models.Foundation;

namespace PIS2.Pages.Localization.Languages
{
    [Authorize(Roles = "ADMIN")]
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

    public DetailsModel(PISContext context)
        {
            _context = context;
        }

        public LanguageModel Language { get; set; }
            = new LanguageModel();

        [BindProperty]
        public List<TranslationRow> Translations { get; set; }
            = new List<TranslationRow>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Get selected language
            var language = await _context.Languages
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.languageID == id);

            if (language == null)
            {
                return NotFound();
            }

            Language = language;

            // Get the default/master language
            var defaultLanguage = await _context.Languages
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.isDefault &&
                    x.isActive);

            if (defaultLanguage == null)
            {
                throw new InvalidOperationException(
                    "No default language has been configured.");
            }

            // Get ALL translation keys from the default language.
            // The default language is the master dictionary.
            var masterTranslations = await _context.AppTranslations
                .AsNoTracking()
                .Where(x =>
                    x.languageID == defaultLanguage.languageID &&
                    x.isActive)
                .OrderBy(x => x.module)
                .ThenBy(x => x.translationKey)
                .ToListAsync();

            // Get translations for the selected language.
            var languageTranslations = await _context.AppTranslations
                .AsNoTracking()
                .Where(x =>
                    x.languageID == id &&
                    x.isActive)
                .ToDictionaryAsync(
                    x => x.translationKey,
                    x => x.translationValue);

            // Build the dictionary displayed on the page.
            foreach (var master in masterTranslations)
            {
                languageTranslations.TryGetValue(
                    master.translationKey,
                    out var translatedValue);

                Translations.Add(
                    new TranslationRow
                    {
                        TranslationKey = master.translationKey,
                        Module = master.module,
                        Description = master.description,

                        TranslationValue =
                            translatedValue ?? string.Empty
                    });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var language = await _context.Languages
                .FirstOrDefaultAsync(x =>
                    x.languageID == id);

            if (language == null)
            {
                return NotFound();
            }

            Language = language;

            foreach (var translation in Translations)
            {
                if (string.IsNullOrWhiteSpace(
                    translation.TranslationKey))
                {
                    continue;
                }

                var existing =
                    await _context.AppTranslations
                        .FirstOrDefaultAsync(x =>
                            x.languageID == id &&
                            x.translationKey ==
                            translation.TranslationKey);

                if (existing == null)
                {
                    // Do not create an empty translation.
                    if (!string.IsNullOrWhiteSpace(
                        translation.TranslationValue))
                    {
                        _context.AppTranslations.Add(
                            new AppTranslationModel
                            {
                                translationKey =
                                    translation.TranslationKey,

                                languageID =
                                    id,

                                translationValue =
                                    translation.TranslationValue.Trim(),

                                module =
                                    translation.Module,

                                description =
                                    translation.Description,

                                isActive = true
                            });
                    }
                }
                else
                {
                    existing.translationValue =
                        translation.TranslationValue?.Trim()
                        ?? string.Empty;

                    existing.module =
                        translation.Module;

                    existing.description =
                        translation.Description;

                    existing.isActive = true;
                }
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Translations saved successfully.";

            return RedirectToPage(
                "./Details",
                new { id });
        }

        public class TranslationRow
        {
            public string TranslationKey { get; set; }
                = string.Empty;

            public string? Module { get; set; }

            public string? Description { get; set; }

            public string TranslationValue { get; set; }
                = string.Empty;
        }
    }

}
