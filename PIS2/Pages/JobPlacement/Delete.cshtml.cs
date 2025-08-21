using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.JobPlacement
{
    public class DeleteModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DeleteModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public jobPlacementModel jobPlacementModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id, string? referer)
        {
            if (User.IsInRole("PMS_HRCLERK"))
            {
                if (id == null)
                {
                    return NotFound();
                }
             
                var jobplacementmodel = await _context.JobPlacements
                    .Include(j => j.employmentModel)
                    .Include(j => j.jobModel)
                    .Include(j => j.departmentModel)
                    .FirstOrDefaultAsync(m => m.jobPlacementID == id);

                if (jobplacementmodel == null)
                {
                    return NotFound();
                }
                else
                {
                    jobPlacementModel = jobplacementmodel;
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (User.IsInRole("PMS_HRCLERK"))
            {
                if (id == null)
                {
                    return NotFound();
                }

                var jobplacementmodel = await _context.JobPlacements.FindAsync(id);
                if (jobplacementmodel != null)
                {
                    jobPlacementModel = jobplacementmodel;
                    _context.JobPlacements.Remove(jobPlacementModel);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToPage("./Index");
        }
    }
}
