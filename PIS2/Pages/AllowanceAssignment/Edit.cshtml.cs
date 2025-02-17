using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.AllowanceAssignment
{
    public class EditModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public EditModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public allowanceAssignmentModel allowanceAssignmentModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var allowanceassignmentmodel =  await _context.AllowanceAssignments.FirstOrDefaultAsync(m => m.allowanceAssignmentID == id);
            if (allowanceassignmentmodel == null)
            {
                return NotFound();
            }
            allowanceAssignmentModel = allowanceassignmentmodel;
           ViewData["allowanceID"] = new SelectList(_context.Allowances, "allowanceID", "allowanceName");
           ViewData["employmentID"] = new SelectList(_context.Employments, "employmentID", "givenID");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(allowanceAssignmentModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!allowanceAssignmentModelExists(allowanceAssignmentModel.allowanceAssignmentID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool allowanceAssignmentModelExists(int id)
        {
            return _context.AllowanceAssignments.Any(e => e.allowanceAssignmentID == id);
        }
    }
}
