using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models.Foundation;

namespace PIS2.Pages.LetterType
{
    public class IndexModel : PageModel
    {
        private readonly PISContext _db;

        public IndexModel(PISContext db)
        {
            _db = db;
        }

        public IList<letterTypeModel> LetterTypes { get; set; } = new List<letterTypeModel>();

        public async Task OnGetAsync()
        {
            LetterTypes = await _db.LetterTypes
                .OrderBy(x => x.letterTypeName)
                .ToListAsync();
        }
    }

}
