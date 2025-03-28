using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.OvertimeRecord
{
    public class DetailsModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DetailsModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public overtimeRecordModel overtimeRecordModel { get; set; } = default!;
        public List<overtimeHistoryModel> OvertimeHistories { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var overtimerecordmodel = await _context.OvertimeRecords.Include(otr => otr.employmentModel).ThenInclude(e => e.personModel)
                .Include(otr => otr.overtimeModel)
                .FirstOrDefaultAsync(m => m.overtimeRecordID == id);
            OvertimeHistories = await _context.OvertimeHistories.Where(oh =>oh.overtimeRecordID == id).ToListAsync();
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
    }
}
