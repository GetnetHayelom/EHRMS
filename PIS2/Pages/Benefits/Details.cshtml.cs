using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Benefits
{
    [Authorize(Roles = "MIE\\PMS_HRMANAGER, MIE\\PMS_FINANCE, MIE\\PMS_HRCLERK")]
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

        public DetailsModel(PISContext context)
        {
            _context = context;
        }

        public otherPay OtherPay { get; set; } = default!;
        public List<Models.AuditLog> History { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            // Fetch the main record
            OtherPay = await _context.OtherPayments
                .Include(o => o.earningModel).ThenInclude(e => e.EmploymentModel).ThenInclude(e => e.personModel)
                .Include(e => e.earningModel).ThenInclude(e => e.earningType)
                .FirstOrDefaultAsync(m => m.paymentID == id);

            if (OtherPay == null) return NotFound();

            // Fetch Audit Logs for this specific record
            History = await _context.AuditLogs
                .Where(a => a.TableName == "OtherPayments" && a.RecordID == id)
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
            if (columnName == "paymentStatus")
            {
                if (int.TryParse(value, out int enumValue))
                {
                    // Cast the integer back to the Enum to get the name (e.g., 0 -> "Active")
                    return ((payrollStatus)enumValue).ToString();
                }
                return value;
            }

            return value;
        }
        public string GetFriendlyColumnName(string columnName)
        {
            return columnName switch
            {
                "invoiceNo" => "Invoce No",
                "GrossPay" => "Gross Pay",
                "NetPay" => "Net Pay",
                "paymentStatus" => "Status",
                "remark" => "Remark",
                _ => columnName
            };
        }
    }
}