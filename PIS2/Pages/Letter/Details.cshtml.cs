using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Letter
{
    public class DetailsModel : PageModel
    {
        private readonly PISContext _db;

        public DetailsModel(PISContext db)
        {
            _db = db;
        }
    
        public letterModel Letter { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Letter = await _db.Letters
                .Include(l => l.LetterType)
                .FirstOrDefaultAsync(l => l.letterID == id);

            ViewData["RefNo"] = Letter.letterNumber;
            if (Letter == null)
                return NotFound();

            return Page();
        }
        [BindProperty]
        public int LetterId { get; set; }
        public async Task<IActionResult> OnPostApproveAsync()
        {
            
            if (!User.IsInRole("MIE\\PMS_HRMANAGER")) 
            {
                TempData["message"] = ("Error", "Access Denied!");
            }

            var letter = await _db.Letters.FirstOrDefaultAsync(l => l.letterID == LetterId);
            if (letter == null) { return NotFound(); }
            if(letter.letterStatus != LetterStatus.Draft)
            {
                TempData["message"] = ("Error", "Only Draft Letters Can Be Approved!");
                return Page();
            }

            letter.letterStatus = LetterStatus.Approved;   // change status here
            letter.modifiedDate = DateTime.Now;
            letter.modifiedBy = User.Identity.Name;

            //TempData["message"] = ("Success", "Letter Approved!");
            await _db.SaveChangesAsync();

            return RedirectToPage("Details", new { id = letter.letterID });
        }
    }

}
