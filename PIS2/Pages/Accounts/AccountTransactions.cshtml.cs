using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models.Finance;

namespace PIS2.Pages.Account
{
    public class AccountTransactionsModel : PageModel
    {
        private readonly PISContext _context;
        public AccountTransactionsModel(PISContext context) => _context = context;

        public accountModel Account { get; set; }
        public List<JournalEntryLine> Transactions { get; set; }

        public async Task OnGetAsync(int id)
        {
            Account = await _context.Accounts.FindAsync(id);

            Transactions = await _context.JournalEntryLines
                .Include(l => l.JournalEntry)
                .Include(l => l.SubAccount)
                .Where(l => l.accountID == id)
                .OrderByDescending(l => l.JournalEntry.EntryDate)
                .ToListAsync();
        }
    }
}
