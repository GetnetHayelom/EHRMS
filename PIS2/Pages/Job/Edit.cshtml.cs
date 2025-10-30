using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Job
{
    [Authorize(Roles = "MIE\\PMS_HRMANAGER")]
    public class EditModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public EditModel(PIS2.Models.PISContext context)
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

            var jobmodel =  await _context.Jobs.FirstOrDefaultAsync(m => m.jobID == id);
            if (jobmodel == null)
            {
                return NotFound();
            }
            jobModel = jobmodel;
           ViewData["jobCategoryID"] = new SelectList(_context.JobCategories, "jobCategoryID", "jobCategoryName");
           ViewData["jobClassID"] = new SelectList(_context.JobClasses, "JobClassId", "JobClassName");
           ViewData["jobGradeID"] = new SelectList(_context.JobGrades, "jobGradeID", "jobGradeName");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
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

            _context.Attach(jobModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!jobModelExists(jobModel.jobID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool jobModelExists(int id)
        {
            return _context.Jobs.Any(e => e.jobID == id);
        }
    }
}
