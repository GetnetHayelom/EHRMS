using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.JobGrade
{
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

        public DetailsModel(PISContext context)
        {
            _context = context;
        }

        public jobGradeModel jobGradeModel { get; set; } = default!;
        public List<Models.AuditLog> History { get; set; } = new();
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobgrademodel = await _context.JobGrades.FirstOrDefaultAsync(m => m.jobGradeID == id);
            if (jobgrademodel == null)
            {
                return NotFound();
            }
            else
            {
                jobGradeModel = jobgrademodel;
                History = await _context.AuditLogs
                    .Where(a => a.TableName == "JobGrades" && a.RecordID == id)
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
            if (columnName == "jobGradeStatus")
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
                "jobGradeName" => "Name",
                "jobGradeDescription" => "Description",
                "jobGradeBasicSalary" => "Dasic Salary",
                "jobGradeStatus" => "Status",
                "jobGradeMaxSalary" => "Max Salary",
                "jobGradeMidSalary" => "Min Salary",
                "NextJobGradeID" => "Next GradeID",
                "PreviousJobGradeID" => "Previous GradeID",
                _ => columnName
            };
        }
    }
}
