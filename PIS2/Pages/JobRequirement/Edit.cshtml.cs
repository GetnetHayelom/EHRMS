using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class EditModel : PageModel
    {
        private readonly PISContext _context;

        public EditModel(PISContext context)
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
            ModelState.Clear();

            Enums.jobReqStatus newStatsus = jobRequirementModel.jobRequirementStatus;

            var exRow = await _context.JobRequirements.FirstOrDefaultAsync(j => j.jobRequirementID == jobRequirementModel.jobRequirementID);
            if (exRow == null)
            {
                return NotFound();
            }

            int reqID = exRow.jobRequirementID;
            Enums.jobReqStatus oldStatus = exRow.jobRequirementStatus;

            exRow.modifiedBy = User.Identity.Name;
            exRow.modifiedDate = DateTime.Now;
            exRow.jobRequirementStatus = jobRequirementModel.jobRequirementStatus;
            exRow.requiredNumber = jobRequirementModel.requiredNumber;
  
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!jobRequirementModelExists(reqID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            
            var isPublished = await _context.Vacancies.AnyAsync(v => v.jobRequirementID == reqID);
            if(!isPublished && (oldStatus == Enums.jobReqStatus.Hold && newStatsus == Enums.jobReqStatus.Approved))
            {
                return RedirectToPage("/Vacancy/Create", new { id = reqID });
            }
            return RedirectToPage("./Details", new {id = reqID });
        }

        private bool jobRequirementModelExists(int id)
        {
            return _context.JobRequirements.Any(e => e.jobRequirementID == id);
        }
    }
}
