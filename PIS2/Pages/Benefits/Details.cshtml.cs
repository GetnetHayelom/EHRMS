using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.Benefits
{
    [Authorize(Roles = "HRMANAGER, FINANCE, HRCLERK")]
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

        public DetailsModel(PISContext context)
        {
            _context = context;
        }

        public payrollModel OtherPay { get; set; } = default!;
        public List<Models.Foundation.AuditLog> History { get; set; } = new();
        public List<earningType> EarningTypes { get; set; }
        public List<deductionType> DeductionTypes { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            EarningTypes = await _context.EarningTypes.ToListAsync();
            DeductionTypes = await _context.DeductionTypes.ToListAsync();
            if (id == null) return NotFound();

            // Fetch the main record
            OtherPay = await _context.Payrolls
                .Include(o => o.PayrollPays).ThenInclude(e => e.EmploymentModel).ThenInclude(e => e.personModel)
                .FirstOrDefaultAsync(m => m.payrollID == id);

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
            if (columnName == "payrollStatus")
            {
                if (int.TryParse(value, out int enumValue))
                {
                    // Cast the integer back to the Enum to get the name (e.g., 0 -> "Active")
                    return ((Enums.payrollStatus)enumValue).ToString();
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