using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.OvertimHistory
{
    public class DeleteModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DeleteModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public overtimeHistoryModel overtimeHistoryModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var overtimehistorymodel = await _context.OvertimeHistories.FirstOrDefaultAsync(m => m.overtimeHistoryID == id);

            if (overtimehistorymodel == null)
            {
                return NotFound();
            }
            else
            {
                overtimeHistoryModel = overtimehistorymodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var overtimehistorymodel = await _context.OvertimeHistories.FindAsync(id);
            if (overtimehistorymodel != null)
            {
                overtimeHistoryModel = overtimehistorymodel;
                _context.OvertimeHistories.Remove(overtimeHistoryModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
