using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;

namespace PIS2.Pages.JobCategory
{
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

        public DetailsModel(PISContext context)
        {
            _context = context;
        }

        public jobCategoryModel jobCategoryModel { get; set; } = default!;
        public List<Models.AuditLog> History { get; set; } = new();
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobcategorymodel = await _context.JobCategories.FirstOrDefaultAsync(m => m.jobCategoryID == id);
            if (jobcategorymodel == null)
            {
                return NotFound();
            }
            else
            {
                jobCategoryModel = jobcategorymodel;
                // Fetch Audit Logs for this specific record
                History = await _context.AuditLogs
                    .Where(a => a.TableName == "JobCategories" && a.RecordID == id)
                    .OrderByDescending(a => a.ModifiedDate)
                    .ToListAsync();
            }
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
            if (columnName == "jobCategoryStatus")
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
                "jobCategoryName" => "Name",
                "jobCategoryDescription" => "Description",
                "jobCategoryStatus" => "Status",
                "modifiedBy" => "Modified By",
                "modifiedDate" => "Modified Date",
                _ => columnName
            };
        }

    }
}
