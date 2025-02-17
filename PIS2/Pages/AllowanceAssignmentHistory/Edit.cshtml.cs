using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.AllowanceAssignmentHistory
{
    public class EditModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public EditModel(PIS2.Models.PISContext context)
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

            var allowanceassignmenthistorymodel =  await _context.AllowanceAssignmentsHistories.FirstOrDefaultAsync(m => m.allowanceAssignmentHistoryID == id);
            if (allowanceassignmenthistorymodel == null)
            {
                return NotFound();
            }
            allowanceAssignmentHistoryModel = allowanceassignmenthistorymodel;
           ViewData["allowanceAssignmentID"] = new SelectList(_context.AllowanceAssignments, "allowanceAssignmentID", "allowanceAssignmentID");
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

            _context.Attach(allowanceAssignmentHistoryModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!allowanceAssignmentHistoryModelExists(allowanceAssignmentHistoryModel.allowanceAssignmentHistoryID))
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

        private bool allowanceAssignmentHistoryModelExists(int id)
        {
            return _context.AllowanceAssignmentsHistories.Any(e => e.allowanceAssignmentHistoryID == id);
        }
    }
}
