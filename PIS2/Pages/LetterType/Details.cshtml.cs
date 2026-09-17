using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models.Foundation;

namespace PIS2.Pages.LetterType
{
    public class DetailsModel : PageModel
    {
        private readonly PISContext _db;

        public DetailsModel(PISContext db)
        {
            _db = db;
        }

        public letterTypeModel LetterType { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            LetterType = await _db.LetterTypes
                .FirstOrDefaultAsync(x => x.letterTypeID == id);

            if (LetterType == null)
                return NotFound();

            return Page();
        }
    }

}
