using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;
using PIS2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PIS2.Enums;

namespace PIS2.Pages.OvertimeRecord
{
    [Authorize(Roles = "HRCLERK, HRMANAGER")]
    public class DeleteModel : PageModel
    {
        private readonly PISContext _context;
        private readonly Core _core;

        public DeleteModel(PISContext context, Core core)
        {
            _context = context;
            _core = core;
        }

        [BindProperty]
        public overtimeRecordModel overtimeRecordModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var overtimerecordmodel = await _context.OvertimeRecords.FirstOrDefaultAsync(m => m.overtimeRecordID == id);

            if (overtimerecordmodel == null)
            {
                return NotFound();
            }
            else
            {
                overtimeRecordModel = overtimerecordmodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            var otr = _context.OvertimeRecords.FirstOrDefault(o => o.overtimeRecordID == id);
            if(otr.overtimeRecordStatus != overtimeStatus.Hold) { TempData["ErrorMessage"] = "Can not delete Apprved, Posted or Completed overtime record!"; return Page(); }
            
            if (id == null)
            {
                return NotFound();
            }

            var overtimerecordmodel = await _context.OvertimeRecords.FindAsync(id);
            if (overtimerecordmodel != null)
            {
                if((!_core.IsSelf(User.Identity?.Name ?? "", overtimerecordmodel.employmentID) ||!(User.IsInRole("HRPERSONNEL") || User.IsInRole("HRMANAGER")) && !(overtimeRecordModel.overtimeRecordStatus == overtimeStatus.Hold)))
                overtimeRecordModel = overtimerecordmodel;
                _context.OvertimeRecords.Remove(overtimeRecordModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
