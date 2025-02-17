using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.JobPlacement
{
    public class EditModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public EditModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public jobPlacementModel jobPlacementModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobplacementmodel =  await _context.JobPlacements.FirstOrDefaultAsync(m => m.jobPlacementID == id);
            if (jobplacementmodel == null)
            {
                return NotFound();
            }
            jobPlacementModel = jobplacementmodel;
           ViewData["departmentID"] = new SelectList(_context.Departments, "departmentID", "departmentName");
           ViewData["employmentID"] = new SelectList(_context.Employments, "employmentID", "givenID");
           ViewData["jobID"] = new SelectList(_context.Jobs, "jobID", "jobTitle");
           ViewData["shiftID"] = new SelectList(_context.Shifts, "shiftID", "shiftName");
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

            _context.Attach(jobPlacementModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!jobPlacementModelExists(jobPlacementModel.jobPlacementID))
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

        private bool jobPlacementModelExists(int id)
        {
            return _context.JobPlacements.Any(e => e.jobPlacementID == id);
        }
    }
}
