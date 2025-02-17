using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Overtime
{
    public class DeleteModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DeleteModel(PIS2.Models.PISContext context)
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
