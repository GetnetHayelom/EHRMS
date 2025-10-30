using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.JobStep
{
    [Authorize(Roles = "MIE\\PMS_HRMANAGER")]
    public class DeleteModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DeleteModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public jobStepModel jobStepModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobstepmodel = await _context.JobSteps.FirstOrDefaultAsync(m => m.jobStepID == id);

            if (jobstepmodel == null)
            {
                return NotFound();
            }
            else
            {
                jobStepModel = jobstepmodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobstepmodel = await _context.JobSteps.FindAsync(id);
            if (jobstepmodel != null)
            {
                jobStepModel = jobstepmodel;
                _context.JobSteps.Remove(jobStepModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
