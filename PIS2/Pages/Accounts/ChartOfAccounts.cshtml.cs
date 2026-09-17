using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models.Finance;

namespace PIS2.Pages.Account
{
    public class ChartOfAccountsModel : PageModel
    {
        private readonly PISContext _context;
        public ChartOfAccountsModel(PISContext context) => _context = context;

        public List<accountModel> Accounts { get; set; }

        public async Task OnGetAsync()
        {
            Accounts = await _context.Accounts
                .Include(a => a.SubAccounts)
                .OrderBy(a => a.accountNumber)
                .ToListAsync();
        }

        public async Task<IActionResult> OnGetSubAccounts(string accountNumber)
        {
            var subs = await _context.SubAccounts
                .Include(s => s.accountModel)
                .Where(s => s.accountModel.accountNumber == accountNumber)
                .ToListAsync();

            return Partial("_SubAccountsPartial", subs);
        }
    }
}
