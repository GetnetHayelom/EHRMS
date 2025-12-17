using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Job
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
        ViewData["jobCategoryID"] = new SelectList(_context.JobCategories.OrderBy(j => j.jobCategoryName), "jobCategoryID", "jobCategoryName");
        ViewData["jobClassID"] = new SelectList(_context.JobClasses.OrderBy(j => j.JobClassName), "JobClassId", "JobClassName");
        ViewData["jobGradeID"] = new SelectList(_context.JobGrades.OrderBy(j => j.jobGradeName), "jobGradeID", "jobGradeName");
            return Page();
        }

        [BindProperty]
        public jobModel jobModel { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("jobModel.modifiedBy");
            jobModel.modifiedBy = User.Identity.Name;

            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        Console.WriteLine($"{kv.Key} --> {error.ErrorMessage}");
                    }
                    Console.WriteLine(kv.ToString());
                }
                return Page();
            }

            _context.Jobs.Add(jobModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
