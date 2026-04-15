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

namespace PIS2.Pages.Overtime
{
    [Authorize(Roles = "MIE\\PMS_HRADMIN")]
    public class DeleteModel : PageModel
    {
        private readonly PISContext _context;

        public DeleteModel(PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public overtimeModel overtimeModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var overtimemodel = await _context.Overtimes.FirstOrDefaultAsync(m => m.overtimeID == id);

            if (overtimemodel == null)
            {
                return NotFound();
            }
            else
            {
                overtimeModel = overtimemodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            var overtime = _context.OvertimeRecords.Any(o => o.overtimeID == id);
            if(overtime) { TempData["ErrorMessage"] = "Can not delete overtime type while there are existing overtime records with this overtime type!"; return Page(); }

            if (id == null)
            {
                return NotFound();
            }

            var overtimemodel = await _context.Overtimes.FindAsync(id);
            if (overtimemodel != null)
            {
                overtimeModel = overtimemodel;
                _context.Overtimes.Remove(overtimeModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
