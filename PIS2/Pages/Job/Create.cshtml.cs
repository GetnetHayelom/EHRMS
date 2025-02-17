using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PIS2.Models;

namespace PIS2.Pages.Job
{
    public class CreateModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public CreateModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["jobCategoryID"] = new SelectList(_context.JobCategories, "jobCategoryID", "jobCategoryName");
        ViewData["jobClassID"] = new SelectList(_context.JobClasses, "JobClassId", "JobClassName");
        ViewData["jobGradeID"] = new SelectList(_context.JobGrades, "jobGradeID", "jobGradeName");
            return Page();
        }

        [BindProperty]
        public jobModel jobModel { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Jobs.Add(jobModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
