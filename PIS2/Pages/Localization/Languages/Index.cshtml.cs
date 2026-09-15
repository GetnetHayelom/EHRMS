using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.Localization.Languages
{
    [Authorize(Roles = "ADMIN")]
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;

    public IndexModel(PISContext context)
        {
            _context = context;
        }

        public IList<LanguageModel> Languages { get; set; }
            = new List<LanguageModel>();

        public async Task OnGetAsync()
        {
            Languages = await _context.Languages
                .AsNoTracking()
                .OrderByDescending(x => x.isDefault)
                .ThenBy(x => x.languageName)
                .ToListAsync();
        }
    }


}
