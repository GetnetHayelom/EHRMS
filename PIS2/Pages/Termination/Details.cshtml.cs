using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;
using PIS2.Services;
using PIS2.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Termination
{
    [Authorize(Roles = "MIE\\PMS_HRMANAGER,MIE\\PMS_HRCLERK,MIE\\PMS_MANAGEMENT")]
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;
        private readonly Core _core;

        public DetailsModel(PISContext context, Core core)
        {
            _context = context;
            _core = core;
        }

        public terminationModel terminationModel { get; set; } = default!;
        public decimal SeverancePay { get; set; }
        public AnnualLeaveSummary? LeaveSummary { get; set; }
        public decimal YearsOfService { get; set; }
        public decimal WorkedSalary { get; set; }
        public decimal WorkedOt { get; set; }
        public jobPlacementModel? jobPlacement { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var terminationmodel = await _context.Terminations
                .Include(e => e.EmploymentModel).ThenInclude(e => e.personModel)
                .FirstOrDefaultAsync(m => m.terminationID == id);
            if (terminationmodel == null)
            {
                return NotFound();
            }
            else
            {
                terminationModel = terminationmodel;
                // Fetch Audit Logs for this specific record
                History = await _context.AuditLogs
                    .Where(a => a.TableName == "Terminations" && a.RecordID == id)
                    .OrderByDescending(a => a.ModifiedDate)
                    .ToListAsync();
                SeverancePay = await _core.GetSeverance(terminationModel.employmentID);
                LeaveSummary = await _context.AnnualLeaveSummary.FirstOrDefaultAsync(a => a.employmentID == terminationModel.employmentID);
                YearsOfService = (decimal)((terminationModel.terminationDate - terminationModel.EmploymentModel.employmentDate).TotalDays) / 365.25m;
            }
            return Page();
        }
        [BindProperty]
        public int terminationID { get; set; }
        public async Task<IActionResult> OnPostAsync()
        {
            var termination = await _context.Terminations.FindAsync(terminationID);
            if (termination == null)
            {
                return NotFound();
            }

            termination.modifiedBy = User.Identity.Name;
            termination.terminationStatus = terminationStatus.Posted;

            _context.Attach(termination).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

            return RedirectToPage("./Details", new { id = terminationID });
        }

        public List<Models.AuditLog> History { get; set; } = new();
        

        public string FormatAuditValue(string columnName, string value)
        {
            if (string.IsNullOrEmpty(value)) return "None";

            // Handle Booleans
            if (columnName.StartsWith("is"))
            {
                return value.ToLower() == "true" ? "Yes" : "No";
            }

            // Handle Enums (assuming mainStatus is 0=Active, 1=Inactive, etc.)
            if (columnName == "terminationStatus")
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
                "terminationDate" => "Termination Date",
                "terminationReason" => "Reason",
                "terminationRemark" => "Remark",
                "terminationStatus" => "Status",
                _ => columnName
            };
        }
    }
}
