using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.OvertimeRecord
{
    [Authorize(Roles = "MIE\\PMS_HRCLERK, MIE\\PMS_HRMANAGER")]
    public class DeleteModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;
        private readonly PIS2.Models.Core _core;

        public DeleteModel(PIS2.Models.PISContext context, Core core)
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
                if((!_core.IsSelf(User.Identity?.Name ?? "", overtimerecordmodel.employmentID) ||!(User.IsInRole("MIE\\PMS_HRCLERK") || User.IsInRole("MIE\\PMS_HRMANAGER")) && !(overtimeRecordModel.overtimeRecordStatus == overtimeStatus.Hold)))
                overtimeRecordModel = overtimerecordmodel;
                _context.OvertimeRecords.Remove(overtimeRecordModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
