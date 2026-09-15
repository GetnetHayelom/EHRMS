using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.Earning
{
    [Authorize(Roles = "HRCLERK, HRMANAGER, HRADMIN")]
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;

        public IndexModel(PISContext context)
        {
            _context = context;
        }

        public IList<earningType> EarningTypes { get; set; } = new List<earningType>();

        public async Task OnGetAsync()
        {
            EarningTypes = await _context.EarningTypes
                .Include(e => e.Account)
                .OrderBy(e => e.earningTypeName)
                .ToListAsync();
        }
    }
}
