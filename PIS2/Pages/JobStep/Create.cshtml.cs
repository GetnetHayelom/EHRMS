using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.JobStep
{
    [Authorize(Roles = "MIE\\PMS_HRADMIN")]
    public class CreateModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public CreateModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["jobGradeID"] = new SelectList(_context.JobGrades.Where(j=> j.jobGradeStatus == mainStatus.Active), "jobGradeID", "jobGradeName");
            return Page();
        }

        [BindProperty]
        public jobStepModel jobStepModel { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.JobSteps.Add(jobStepModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
