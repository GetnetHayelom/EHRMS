using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Job
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
        public jobModel jobModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobmodel = await _context.Jobs.FirstOrDefaultAsync(m => m.jobID == id);

            if (jobmodel == null)
            {
                return NotFound();
            }
            else
            {
                jobModel = jobmodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobmodel = await _context.Jobs.FindAsync(id);
            if (jobmodel != null)
            {
                jobModel = jobmodel;
                _context.Jobs.Remove(jobModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
