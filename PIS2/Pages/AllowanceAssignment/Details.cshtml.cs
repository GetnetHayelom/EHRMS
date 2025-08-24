using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.AllowanceAssignment
{
    public class DetailsModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DetailsModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public allowanceAssignmentModel allowanceAssignmentModel { get; set; } = default!;
        public List<allowanceAssignmentModel> allowanceAssignments { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var allowanceassignmentmodel = await _context.AllowanceAssignments
                .Include(a => a.employmentModel).ThenInclude(e => e.personModel)
                .Include(a => a.allowanceModel)
                .Include(a => a.AllowanceAssignmentHistories)
                .FirstOrDefaultAsync(m => m.allowanceAssignmentID == id);

            if (allowanceassignmentmodel == null)
            {
                return NotFound();
            }
            else
            {
                allowanceAssignmentModel = allowanceassignmentmodel;
                allowanceAssignments = await _context.AllowanceAssignments.Where(aa => aa.employmentID == allowanceAssignmentModel.employmentID
                && aa.allowanceID != allowanceAssignmentModel.allowanceID).ToListAsync();
            }
            return Page();
        }
    }
}
