using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;
using PIS2.Enums;

namespace PIS2.Pages.Letter
{
    [Authorize(Roles ="MIE\\PMS_HRCLERK, MIE\\PMS_HRMANAGER")]
    public class EditModel : PageModel
    {
        private readonly PISContext _db;

        public EditModel(PISContext db)
        {
            _db = db;
        }

        [BindProperty]
        public letterModel Letter { get; set; } = null!;

        public SelectList LetterTypes { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            PersonList = new SelectList(_db.Persons, "personFullName", "personFullName");

            Letter = await _db.Letters.FindAsync(id);
            if (Letter == null)
                return NotFound();

            LetterTypes = new SelectList(
                _db.LetterTypes,
                "letterTypeID",
                "letterTypeName"
            );

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Letter.modifiedBy");
            PersonList = new SelectList(_db.Persons, "personFullName", "personFullName");
            var existing = await _db.Letters.FirstOrDefaultAsync(l => l.letterID == Letter.letterID);
            if (existing == null) { return NotFound(); }

            bool isAdmin = User.IsInRole("MIE\\PMS_HRMANAGER");

            if(existing.letterStatus == LetterStatus.Draft && Letter.letterStatus == LetterStatus.Approved && !isAdmin)
            {
                TempData["message"] = ("Error", "You dont have privilage to approve a letter!");
                return Page();
            }
            if(existing.letterStatus != LetterStatus.Draft)
            {
                TempData["message"] = ("Error", "Letter is locked!");
                return Page();
            }


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

            existing.letterGroup = Letter.letterGroup;
            existing.letterTypeID = Letter.letterTypeID;
            existing.letterSender = Letter.letterSender;
            existing.letterReceiver = Letter.letterReceiver;
            existing.letterBody = Letter.letterBody;
            existing.letterTitle = Letter.letterTitle;
            existing.letterSubject = Letter.letterSubject;
            existing.letterSignedBy = Letter.letterSignedBy;
            existing.modifiedBy = User.Identity.Name;
            existing.modifiedDate = DateTime.Now;

            await _db.SaveChangesAsync();

            return RedirectToPage("./Details", new { id = existing.letterID});
        }
        public SelectList PersonList { get; set; }
        public async Task<IActionResult> OnPostApproveAsync()
        {
            PersonList = new SelectList(_db.Persons, "personFullName", "personFullName");
            if (!User.IsInRole("MIE\\PMS_HRMANAGER"))
            {
                TempData["message"] = ("Error", "Access Denied!");
                return Page();
            }

            var letter = await _db.Letters.FirstOrDefaultAsync(l => l.letterID == Letter.letterID);
            if (letter == null) { return NotFound(); }
            if (letter.letterStatus != LetterStatus.Draft)
            {
                TempData["message"] = ("Error", "Letter Not In Draft Status!");
                return Page();
            }


            letter.letterStatus = LetterStatus.Approved;   // change status here
            letter.modifiedDate = DateTime.Now;
            letter.modifiedBy = User.Identity.Name;

            TempData["message"] = ("Success", "Letter Approved!");
            await _db.SaveChangesAsync();

            return Page();
        }
    }

}
