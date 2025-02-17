using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.AllowanceAssignmentHistory
{
    public class DeleteModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DeleteModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public allowanceAssignmentHistoryModel allowanceAssignmentHistoryModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var allowanceassignmenthistorymodel = await _context.AllowanceAssignmentsHistories.FirstOrDefaultAsync(m => m.allowanceAssignmentHistoryID == id);

            if (allowanceassignmenthistorymodel == null)
            {
                return NotFound();
            }
            else
            {
                allowanceAssignmentHistoryModel = allowanceassignmenthistorymodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var allowanceassignmenthistorymodel = await _context.AllowanceAssignmentsHistories.FindAsync(id);
            if (allowanceassignmenthistorymodel != null)
            {
                allowanceAssignmentHistoryModel = allowanceassignmenthistorymodel;
                _context.AllowanceAssignmentsHistories.Remove(allowanceAssignmentHistoryModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
