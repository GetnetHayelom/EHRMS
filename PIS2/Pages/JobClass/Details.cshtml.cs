using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.JobClass
{
    
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

        public DetailsModel(PISContext context)
        {
            _context = context;
        }

        public jobClassModel jobClassModel { get; set; } = default!;
        public IList<jobModel> jobModel { get; set; } = default!;
        public List<Models.AuditLog> History { get; set; } = new();
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobclassmodel = await _context.JobClasses.FirstOrDefaultAsync(m => m.jobClassId == id);
            if (jobclassmodel == null)
            {
                return NotFound();
            }
            else
            {
                jobClassModel = jobclassmodel;
                jobModel = await _context.Jobs
                .Include(j => j.jobCategoryModel)
                .Include(j => j.jobClassModel)
                .Include(j => j.jobGradeModel).Where(j => j.jobClassID == id).ToListAsync();

                // Fetch Audit Logs for this specific record
                History = await _context.AuditLogs
                    .Where(a => a.TableName == "JobClasses" && a.RecordID == id)
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
            if (columnName == "jobClassStatus")
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
                "jobClassStatus" => "Status",
                "jobClassName" => "Name",
                "jobClassDescription" => "Description",
                _ => columnName
            };
        }
    }
}
