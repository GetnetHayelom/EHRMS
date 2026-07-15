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
using PIS2.Services;
using static System.Formats.Asn1.AsnWriter;

namespace PIS2.Pages.OvertimeRecord
{
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;
        private readonly Core _core;

        public IndexModel(PISContext context, Core core)
        {
            _context = context;
            _core = core;
        }

        // Expose to Razor Page
        public List<DepartmentOvertimeGroup> GroupedOvertimes { get; set; } = new();

        public class DepartmentOvertimeGroup
        {
            public string DepartmentName { get; set; } = string.Empty;
            public int Count { get; set; }
            public List<overtimeRecordModel> Records { get; set; } = new();
        }
        public IList<overtimeRecordModel> overtimeRecordModel { get;set; } = default!;
        public int otrCount { get; set; }
        
        public async Task OnGetAsync(List<int>? id)
        {
            var empID = _core.getUserEmp(User.Identity.Name);

            var company = _context.JobPlacements.Include(j => j.departmentModel)
                .FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active && j.employmentID == empID)?.departmentModel?.companyID;

            // Step 1: get the filtered records from the database
            var otr = await _context.OvertimeRecords
                .Include(o => o.employmentModel).ThenInclude(e => e.JobPlacements).ThenInclude(j => j.departmentModel)
                .Include(o => o.overtimeModel)
                .Include(o => o.OvertimeHistories)
                .Where(o => (o.overtimeRecordStatus == overtimeStatus.Hold || o.overtimeRecordStatus == overtimeStatus.Approved)
                    && o.employmentModel.JobPlacements.Any(j => j.jobPlacementStatus == mainStatus.Active
                        && j.departmentModel.companyID == company))
                .ToListAsync();   // ✅ force materialization here

            // Step 2: group in memory
            GroupedOvertimes = otr
                .GroupBy(o => o.employmentModel.JobPlacements
                    .FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active)?.departmentModel)
                .Select(g => new DepartmentOvertimeGroup
                {
                    DepartmentName = g.Key?.departmentName ?? "Unknown",
                    Count = g.Count(),
                    Records = g.ToList()
                })
                .ToList();

            overtimeRecordModel = otr.ToList();
            otrCount = overtimeRecordModel.GroupBy(o => new {o.employmentID, o.overtimeRecordDate}).Count();
        }

        // Post handler
        [BindProperty]
        public int overtimeRecordID { get; set; }
        //[HttpPost]
        public async Task<IActionResult> OnPostApprove(int overtimeRecordID)
        {
            var overtimeRecord = await _context.OvertimeRecords.FindAsync(overtimeRecordID);
            if (overtimeRecord == null)
                return new JsonResult(new { success = false, message = "Overtime Record not found." });

            if (!User.IsInRole("MIE\\PMS_HRCLERK"))
                return new JsonResult(new { success = false, message = "Access denied." });

            overtimeRecord.overtimeRecordStatus = overtimeStatus.Posted;
            overtimeRecord.modifiedBy = User.Identity.Name;

            _context.Update(overtimeRecord);
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true, message = "Overtime posted successfully." });
        }
    }
}
