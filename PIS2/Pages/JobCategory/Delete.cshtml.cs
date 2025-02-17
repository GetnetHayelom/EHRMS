using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.JobCategory
{
    public class DeleteModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DeleteModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public jobCategoryModel jobCategoryModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobcategorymodel = await _context.JobCategories.FirstOrDefaultAsync(m => m.jobCategoryID == id);

            if (jobcategorymodel == null)
            {
                return NotFound();
            }
            else
            {
                jobCategoryModel = jobcategorymodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobcategorymodel = await _context.JobCategories.FindAsync(id);
            if (jobcategorymodel != null)
            {
                jobCategoryModel = jobcategorymodel;
                _context.JobCategories.Remove(jobCategoryModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
