using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Account
{
    [Authorize(Roles = "FINANCE")]
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

        public DetailsModel(PISContext context)
        {
            _context = context;
        }

        public accountModel accountModel { get; set; } = default!;
        public List<Models.AuditLog> History { get; set; } = new();
        public List<subAccountModel> SubAccounts { get; set; } = new();
        public List<JournalEntryLine> JournalEntries { get; set; } = new();
        public List<JournalEntryDTO> Journals { get; set; } = new();
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountmodel = await _context.Accounts.FirstOrDefaultAsync(m => m.accountID == id);
            if (accountmodel == null)
            {
                return NotFound();
            }
            else
            {
                accountModel = accountmodel;
                History = await _context.AuditLogs
                    .Where(a => a.TableName == "OtherPayments" && a.RecordID == id)
                    .OrderByDescending(a => a.ModifiedDate)
                    .ToListAsync();
                SubAccounts = await _context.SubAccounts.Where(s => s.accountID == id).ToListAsync() ?? new List<subAccountModel>();
                JournalEntries = await _context.JournalEntryLines
                    .Include(j => j.SubAccount)
                    .Include(j => j.JournalEntry).Where(j => j.accountID == id).ToListAsync() ?? new List<JournalEntryLine>(); ;

            }
            var grouped = JournalEntries
                .GroupBy(j => j.JournalEntryID)
                .Select(g => new JournalEntryDTO{
                    EntryID = g.Key,
                    Reference = g.FirstOrDefault()?.JournalEntry.Description ?? "",
                    Count = g.Count(),
                    TotalDebit = g.Sum(x => x.Debit),
                    TotalCredit = g.Sum(x => x.Credit),
                    Lines = g.GroupBy(l => l.subAccountID).Select(l => new JournalEntryLineDTO {
                        subAccountName = l.FirstOrDefault()?.SubAccount?.subAccountName ?? "",
                        subAccountNumber = l.FirstOrDefault()?.SubAccount?.subAccountNumber ?? "",
                        subAccountDescription = l.FirstOrDefault()?.SubAccount?.subAccountDescription ?? "",
                        Debit =l.Sum(l => l.Debit),
                        Credit =l.Sum(l => l.Credit)
                    }).ToList()
                }).ToList();
            Journals = grouped ?? new List<JournalEntryDTO>();
            return Page();
        }
        public string FormatAuditValue(string columnName, string value)
        {
            if (string.IsNullOrEmpty(value)) return "None";

            // Handle Booleans
            if (columnName.StartsWith("is"))
            {
                return value.ToLower() == "true" ? "Yes" : "No";
            }

            // Handle Enums (assuming mainStatus is 0=Active, 1=Inactive, etc.)
            if (columnName == "accountStatus")
            {
                if (int.TryParse(value, out int enumValue))
                {
                    // Cast the integer back to the Enum to get the name (e.g., 0 -> "Active")
                    return ((mainStatus)enumValue).ToString();
                }
                return value;
            }
            // Handle Enums (account type)
            if (columnName == "accountType")
            {
                if (int.TryParse(value, out int enumValue))
                {
                    // Cast the integer back to the Enum to get the name (e.g., 0 -> "Active")
                    return ((AccountType)enumValue).ToString();
                }
                return value;
            }

            return value;
        }
        public string GetFriendlyColumnName(string columnName)
        {
            return columnName switch
            {
                "accountNumber" => "Account Number",
                "accountName" => "Name",
                "accounDescription" => "Description",
                "accounStatus" => "Status",
                "accountType" => "Type",
                "ParentAccountID" => "Parrent Account ID",
                _ => columnName
            };
        }
    }

    public class JournalEntryDTO 
    {
        public int EntryID { get; set; }
        public string Reference { get; set; } = "";
        public int Count { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public List<JournalEntryLineDTO> Lines { get; set; }
    }
    public class JournalEntryLineDTO
    {
        public string subAccountName { get; set; }
        public string subAccountNumber { get; set; } = "";
        public string subAccountDescription { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }
}
