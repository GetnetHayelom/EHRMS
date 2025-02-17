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
    public class DeleteModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DeleteModel(PIS2.Models.PISContext context)
        {
            _context = context;
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
            if (id == null)
            {
                return NotFound();
            }

            var overtimerecordmodel = await _context.OvertimeRecords.FindAsync(id);
            if (overtimerecordmodel != null)
            {
                overtimeRecordModel = overtimerecordmodel;
                _context.OvertimeRecords.Remove(overtimeRecordModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
