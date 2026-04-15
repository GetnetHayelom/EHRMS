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

namespace PIS2.Pages.JobClass
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
        public jobClassModel jobClassModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobclassmodel = await _context.JobClasses.FirstOrDefaultAsync(m => m.jobClassId == id);

            if (jobclassmodel == null)
            {
                return NotFound();
            }
            else
            {
                jobClassModel = jobclassmodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobclassmodel = await _context.JobClasses.FindAsync(id);
            if (jobclassmodel != null)
            {
                jobClassModel = jobclassmodel;
                _context.JobClasses.Remove(jobClassModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
