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

namespace PIS2.Pages.JobRequirement
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
        public jobRequirementModel jobRequirementModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobrequirementmodel =  await _context.JobRequirements.Include(j => j.JobModel).Include(j => j.DepartmentModel).ThenInclude(d => d.companyModel).FirstOrDefaultAsync(m => m.jobRequirementID == id);
            if (jobrequirementmodel == null)
            {
                return NotFound();
            }
            jobRequirementModel = jobrequirementmodel;
           ViewData["departmentID"] = new SelectList(_context.Departments, "departmentID", "departmentName");
           ViewData["jobID"] = new SelectList(_context.Jobs, "jobID", "jobTitle");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(jobRequirementModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!jobRequirementModelExists(jobRequirementModel.jobRequirementID))
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

        private bool jobRequirementModelExists(int id)
        {
            return _context.JobRequirements.Any(e => e.jobRequirementID == id);
        }
    }
}
