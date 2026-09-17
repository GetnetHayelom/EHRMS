using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models.HR;

namespace PIS2.Pages.ShiftAssignment
{
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

        public DetailsModel(PISContext context)
        {
            _context = context;
        }

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
    }
}
