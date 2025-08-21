using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.ShiftAssignment
{
    public class DeleteModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DeleteModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public shiftAssignmentModel shiftAssignmentModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shiftassignmentmodel = await _context.ShiftAssignments.FirstOrDefaultAsync(m => m.shiftAssignmentID == id);

            if (shiftassignmentmodel == null)
            {
                return NotFound();
            }
            else
            {
                shiftAssignmentModel = shiftassignmentmodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shiftassignmentmodel = await _context.ShiftAssignments.FindAsync(id);
            if (shiftassignmentmodel != null)
            {
                shiftAssignmentModel = shiftassignmentmodel;
                _context.ShiftAssignments.Remove(shiftAssignmentModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
