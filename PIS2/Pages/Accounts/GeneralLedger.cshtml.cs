using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models.Finance;

namespace PIS2.Pages.Account
{
    public class GeneralLedgerModel : PageModel
    {
        private readonly PISContext _context;
        public GeneralLedgerModel(PISContext context) => _context = context;

        public List<JournalEntry> Entries { get; set; }

        public async Task OnGetAsync()
        {
            Entries = await _context.JournalEntries
                .Include(j => j.Lines)
                    .ThenInclude(l => l.Account)
                .Include(j => j.Lines)
                    .ThenInclude(l => l.SubAccount)
                .OrderByDescending(j => j.EntryDate)
                .ToListAsync();
        }
    }
}
