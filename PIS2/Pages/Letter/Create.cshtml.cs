using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models.Foundation;

namespace PIS2.Pages.Letter
{
    public class CreateModel : PageModel
    {
        private readonly PISContext _db;

        public CreateModel(PISContext db)
        {
            _db = db;
        }

        [BindProperty]
        public letterModel Letter { get; set; } = new();

        public SelectList LetterTypes { get; set; } = null!;

        public async Task OnGetAsync()
        {
            LetterTypes = new SelectList(
                await _db.LetterTypes.OrderBy(x => x.letterTypeName).ToListAsync(),
                "letterTypeID",
                "letterTypeName"
            );
        }


        public async Task<IActionResult> OnPostAsync()
        {
            Letter.modifiedBy = User.Identity?.Name ?? "System";
            Letter.letterDate = DateTime.Now;
            

            //Letter.letterNumber = await GenerateLetterNumber(Letter);

            _db.Letters.Add(Letter);
            await _db.SaveChangesAsync();

            return RedirectToPage("Details", new {id = Letter.letterID });
        }

        
    }
    
}
