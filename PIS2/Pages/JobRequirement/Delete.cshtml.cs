using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.JobRequirement
{
    [Authorize(Roles = "HRMANAGER")]
    public class DeleteModel : PageModel
    {
        private readonly PISContext _context;

        public DeleteModel(PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public jobRequirementModel jobRequirementModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobrequirementmodel = await _context.JobRequirements.FirstOrDefaultAsync(m => m.jobRequirementID == id);

            if (jobrequirementmodel == null)
            {
                return NotFound();
            }
            else
            {
                jobRequirementModel = jobrequirementmodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobrequirementmodel = await _context.JobRequirements.FindAsync(id);
            if (jobrequirementmodel != null)
            {
                jobRequirementModel = jobrequirementmodel;
                _context.JobRequirements.Remove(jobRequirementModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
