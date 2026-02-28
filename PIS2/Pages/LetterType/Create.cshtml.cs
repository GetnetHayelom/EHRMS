using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PIS2.Models;

namespace PIS2.Pages.LetterType
{
    public class CreateModel : PageModel
    {
        private readonly PISContext _db;

        public CreateModel(PISContext db)
        {
            _db = db;
        }

        [BindProperty]
        public letterTypeModel LetterType { get; set; } = new();

        public async Task<IActionResult> OnPostAsync()
        {
            if (_db.LetterTypes.Any(x => x.letterTypeCode == LetterType.letterTypeCode))
            {
                ModelState.AddModelError("", "Code already exists");
                return Page();
            }

            ModelState.Remove("LetterType.modifiedBy");
            LetterType.modifiedBy = User.Identity.Name;
            LetterType.modifiedDate = DateTime.Now;

            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        Console.WriteLine($"{kv.Key} --> {error.ErrorMessage}");
                        TempData["message"] = ("Error", $"{kv.Key} --> {error.ErrorMessage}");
                    }
                }

                return Page();
            }

            _db.LetterTypes.Add(LetterType);
            await _db.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }

}
