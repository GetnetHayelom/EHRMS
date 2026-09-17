using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;

namespace PIS2.Pages.Earning
{
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

        public DetailsModel(PISContext context)
        {
            _context = context;
        }

        public earningType EarningType { get; set; }
        public List<Models.Foundation.AuditLog> AuditLogs { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            EarningType = await _context.EarningTypes.Include(e => e.Account)
                .FirstOrDefaultAsync(m => m.earningTypeID == id);

            if (EarningType == null)
            {
                return NotFound();
            }

            // Fetch the history related to this earning ID
            AuditLogs = await _context.AuditLogs
            .Where(a => a.TableName == "EarningTypes" && a.RecordID == id)
            .OrderByDescending(a => a.ModifiedDate)
            .ToListAsync();

            return Page();
        }
        
        // Helper to make column names and values friendly
        public string FormatAuditValue(string columnName, string value)
        {
            if (string.IsNullOrEmpty(value)) return "None";

            // Handle Booleans
            if (columnName.StartsWith("is"))
            {
                return value.ToLower() == "true" ? "Yes" : "No";
            }

            // Handle Enums (assuming mainStatus is 0=Active, 1=Inactive, etc.)
            if (columnName == "earningTypeStatus")
            {
                if (int.TryParse(value, out int enumValue))
                {
                    // Cast the integer back to the Enum to get the name (e.g., 0 -> "Active")
                    return ((mainStatus)enumValue).ToString();
                }
                return value;
            }

            return value;
        }
        public string GetFriendlyColumnName(string columnName)
        {
            return columnName switch
            {
                "earningTypeName" => "Name",
                "earningTypeDescription" => "Description",
                "isRecurring" => "Recurring Setting",
                "isTaxable" => "Tax Status",
                "isPayroll" => "Payroll Processing",
                "earningTypeStatus" => "Status",
                _ => columnName
            };
        }
    }
}
