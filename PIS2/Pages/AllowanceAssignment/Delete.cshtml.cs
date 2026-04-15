using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.AllowanceAssignment
{
    public class DeleteModel : PageModel
    {
        private readonly PISContext _context;

        public DeleteModel(PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public allowanceAssignmentModel allowanceAssignmentModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!User.IsInRole("MIE\\PMS_HRMANAGER"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }
            if (id == null)
            {
                return NotFound();
            }

            var allowanceassignmentmodel = await _context.AllowanceAssignments.FirstOrDefaultAsync(m => m.allowanceAssignmentID == id);

            if (allowanceassignmentmodel == null)
            {
                return NotFound();
            }
            else
            {
                allowanceAssignmentModel = allowanceassignmentmodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!User.IsInRole("MIE\\PMS_HRMANAGER"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }
            if (id == null)
            {
                return NotFound();
            }

            var allowanceassignmentmodel = await _context.AllowanceAssignments.FindAsync(id);
            if (allowanceassignmentmodel != null)
            {
                allowanceAssignmentModel = allowanceassignmentmodel;
                _context.AllowanceAssignments.Remove(allowanceAssignmentModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
