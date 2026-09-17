using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;

namespace PIS2.Pages.Deduction
{
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context; // Replace with your actual DbContext name

        public DetailsModel(PISContext context)
        {
            _context = context;
        }

        public deductionType DeductionType { get; set; }
        public List<Models.Foundation.AuditLog> History { get; set; } = new();
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // We include the Account relationship to show the account name/details
            DeductionType = await _context.DeductionTypes
                .Include(d => d.Account)
                .FirstOrDefaultAsync(m => m.deductionTypeID == id);

            if (DeductionType == null)
            {
                return NotFound();
            }
            
        // Fetch Audit Logs for this specific record
            History = await _context.AuditLogs
                .Where(a => a.TableName == "DeductionTypes" && a.RecordID == id)
                .OrderByDescending(a => a.ModifiedDate)
                .ToListAsync();
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
            if (columnName == "deductionStatus")
            {
                if (int.TryParse(value, out int enumValue))
                {
                    // Cast the integer back to the Enum to get the name (e.g., 0 -> "Active")
                    return ((mainStatus)enumValue).ToString();
                }
                return value;
            }
            if (columnName == "deductBase")
            {
                if (int.TryParse(value, out int enumValue))
                {
                    // Cast the integer back to the Enum to get the name (e.g., 0 -> "Active")
                    return ((deductionBase)enumValue).ToString();
                }
                return value;
            }
            return value;
        }
        public string GetFriendlyColumnName(string columnName)
        {
            return columnName switch
            {
                "deductionName" => "Name",
                "isRecurring" => "Recurring",
                "deductBase" => "Base",
                "deductionPrioriy" => "Priority",
                "deductionStatus" => "Status",
                "isMandatory" => "Mandatory",
                "accountID" => "Account ID",
                _ => columnName
            };
        }
    }
}
