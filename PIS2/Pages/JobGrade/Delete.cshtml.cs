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

namespace PIS2.Pages.JobGrade
{
    [Authorize(Roles = "MIE\\PMS_HRADMIN")]
    public class DeleteModel : PageModel
    {
        private readonly PISContext _context;

        public DeleteModel(PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public jobGradeModel jobGradeModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobgrademodel = await _context.JobGrades.FirstOrDefaultAsync(m => m.jobGradeID == id);

            if (jobgrademodel == null)
            {
                return NotFound();
            }
            else
            {
                jobGradeModel = jobgrademodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobgrademodel = await _context.JobGrades.FindAsync(id);
            if (jobgrademodel != null)
            {
                jobGradeModel = jobgrademodel;
                _context.JobGrades.Remove(jobGradeModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
