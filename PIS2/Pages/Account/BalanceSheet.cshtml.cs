using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;
using PIS2.Enums;

namespace PIS2.Pages.Account
{
    public class BalanceSheetModel : PageModel
    {
        private readonly PISContext _context;
        public BalanceSheetModel(PISContext context) => _context = context;

        public List<AccountBalance> AssetAccounts { get; set; } = new();
        public List<AccountBalance> LiabilityAccounts { get; set; } = new();
        public List<AccountBalance> EquityAccounts { get; set; } = new();
        public List<TrialBalanceRow> TrialBalance { get; set; } = new();

        public decimal TotalAssets => AssetAccounts.Sum(x => x.Balance);
        public decimal TotalLiabilities => LiabilityAccounts.Sum(x => x.Balance);
        public decimal TotalEquity => EquityAccounts.Sum(x => x.Balance);
        public DateTime CurrentAsOfDate { get; set; }

        public class AccountBalance
        {
            public string Name { get; set; }
            public string Number { get; set; }
            public decimal Balance { get; set; }
        }

        public async Task OnGetAsync(DateTime? asOfDate)
        {
            CurrentAsOfDate = asOfDate ?? DateTime.Now;

            // Fetch aggregated data in one query to save database trips
            var balances = await _context.JournalEntryLines
                .Where(l => l.JournalEntry.EntryDate <= CurrentAsOfDate)
                .GroupBy(l => new
                {
                    l.Account.accountName,
                    l.Account.accountNumber,
                    l.Account.accountType
                })
                .Select(g => new
                {
                    g.Key.accountName,
                    g.Key.accountNumber,
                    Type = g.Key.accountType ?? AccountType.Asset,
                    TotalDebit = g.Sum(x => x.Debit),
                    TotalCredit = g.Sum(x => x.Credit)
                }).ToListAsync();

            // Map Assets (Debit - Credit)
            AssetAccounts = balances.Where(b => b.Type == AccountType.Asset)
                .Select(b => new AccountBalance { Name = b.accountName, Number = b.accountNumber, Balance = b.TotalDebit - b.TotalCredit })
                .OrderBy(x => x.Number).ToList();

            // Map Liabilities (Credit - Debit)
            LiabilityAccounts = balances.Where(b => b.Type == AccountType.Liability)
                .Select(b => new AccountBalance { Name = b.accountName, Number = b.accountNumber, Balance = b.TotalCredit - b.TotalDebit })
                .OrderBy(x => x.Number).ToList();

            // Map Equity (Credit - Debit)
            EquityAccounts = balances.Where(b => b.Type == AccountType.Equity)
                .Select(b => new AccountBalance { Name = b.accountName, Number = b.accountNumber, Balance = b.TotalCredit - b.TotalDebit })
                .OrderBy(x => x.Number).ToList();

            // Populate Trial Balance
            TrialBalance = balances.Select(b => new TrialBalanceRow
            {
                Name = b.accountName,
                Number = b.accountNumber,
                Type = b.Type,
                Debit = b.TotalDebit,
                Credit = b.TotalCredit
            }).OrderBy(x => x.Number).ToList();
        }
        public class TrialBalanceRow
        {
            public string Name { get; set; }
            public string Number { get; set; }
            public AccountType Type { get; set; }
            public decimal Debit { get; set; }
            public decimal Credit { get; set; }

            // Net balance calculated based on standard accounting rules:
            // Assets/Expenses are Debit-normal; Liabilities/Equity/Revenue are Credit-normal.
            public decimal Balance =>
                (Type == AccountType.Asset || Type == AccountType.Expense)
                ? Debit - Credit
                : Credit - Debit;
        }
    }
}