using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.Letter
{
    public class IndexModel : PageModel
    {
        private readonly PISContext _db;

        public IndexModel(PISContext db)
        {
            _db = db;
        }

        public IList<letterModel> Letters { get; set; } = new List<letterModel>();

        public async Task OnGetAsync()
        {
            Letters = await _db.Letters
                .Include(l => l.LetterType)
                .OrderByDescending(l => l.letterDate)
                .ToListAsync();
        }
    }

}
