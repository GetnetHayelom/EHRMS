using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.LetterType
{
    public class EditModel : PageModel
    {
        private readonly PISContext _db;

        public EditModel(PISContext db)
        {
            _db = db;
        }

        [BindProperty]
        public letterTypeModel LetterType { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            LetterType = await _db.LetterTypes.FirstOrDefaultAsync(l => l.letterTypeID == id);
            if (LetterType == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (_db.LetterTypes.Any(x => x.letterTypeCode == LetterType.letterTypeCode && x.letterTypeID != LetterType.letterTypeID))
            {
                TempData["message"]=("Error", "Code already exists!");
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

            _db.Update(LetterType);
            await _db.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }

}
