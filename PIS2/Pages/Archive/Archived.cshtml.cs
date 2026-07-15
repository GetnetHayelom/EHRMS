using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.Archive
{
    public class ArchivedModel : PageModel
    {
        private readonly PISContext _db;

        public ArchivedModel(PISContext db)
        {
            _db = db;
        }

        public IList<letterModel> Letters { get; set; } = new List<letterModel>();
        public void OnGet()
        {
            Letters = _db.Letters
                .Include(l => l.LetterType)
                .Include(l => l.Archive)
                .Where(l => l.letterStatus == Enums.LetterStatus.Archived)
                .OrderByDescending(l => l.letterDate)
                .ToList();
        }
    }
}
