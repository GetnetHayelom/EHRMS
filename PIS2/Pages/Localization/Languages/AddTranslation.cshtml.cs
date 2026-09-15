using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.Localization.Languages
{
    [Authorize(Roles = "ADMIN")]
    public class AddTranslationModel : PageModel
    {
        private readonly PISContext _context;

        public AddTranslationModel(PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TranslationInput Input { get; set; }
            = new TranslationInput();

        public async Task<IActionResult> OnGetAsync()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Clean the values
            Input.TranslationKey =
                Input.TranslationKey?.Trim() ?? string.Empty;

            Input.EnglishValue =
                Input.EnglishValue?.Trim() ?? string.Empty;

            Input.Module =
                Input.Module?.Trim() ?? string.Empty;

            Input.Description =
                Input.Description?.Trim();

            // Validate key
            if (string.IsNullOrWhiteSpace(Input.TranslationKey))
            {
                ModelState.AddModelError(
                    "Input.TranslationKey",
                    "Translation key is required.");
            }

            // Validate English value
            if (string.IsNullOrWhiteSpace(Input.EnglishValue))
            {
                ModelState.AddModelError(
                    "Input.EnglishValue",
                    "English value is required.");
            }

            // Validate module
            if (string.IsNullOrWhiteSpace(Input.Module))
            {
                ModelState.AddModelError(
                    "Input.Module",
                    "Module is required.");
            }

            // Check duplicate key
            var duplicateKey =
                await _context.AppTranslations
                    .AnyAsync(x =>
                        x.translationKey ==
                        Input.TranslationKey);

            if (duplicateKey)
            {
                ModelState.AddModelError(
                    "Input.TranslationKey",
                    "This translation key already exists.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Find the default language
            var english =
                await _context.Languages
                    .FirstOrDefaultAsync(x =>
                        x.languageCode == "en" &&
                        x.isActive);

            if (english == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The English language has not been configured.");

                return Page();
            }

            // Create the master English translation
            var translation =
                new AppTranslationModel
                {
                    translationKey =
                        Input.TranslationKey,

                    languageID =
                        english.languageID,

                    translationValue =
                        Input.EnglishValue,

                    module =
                        Input.Module,

                    description =
                        Input.Description,

                    isActive = true
                };

            _context.AppTranslations.Add(translation);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Translation key added successfully.";

            return RedirectToPage("./Index");
        }

        public class TranslationInput
        {
            public string TranslationKey { get; set; }
                = string.Empty;

            public string EnglishValue { get; set; }
                = string.Empty;

            public string Module { get; set; }
                = string.Empty;

            public string? Description { get; set; }
        }
    }
}