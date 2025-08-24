using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.JobPlacement
{
    public class EditModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public EditModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public jobPlacementModel jobPlacementModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobplacementmodel =  await _context.JobPlacements
                .Include(jp => jp.employmentModel).ThenInclude(e => e.personModel)
                .Include(jp => jp.departmentModel).ThenInclude(d => d.companyModel)
                .Include(jp => jp.jobModel).FirstOrDefaultAsync(m => m.jobPlacementID == id);
            if (jobplacementmodel == null)
            {
                return NotFound();
            }
            jobPlacementModel = jobplacementmodel;
            populateSelect();
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Clear();
            jobPlacementModel.modifiedBy = User.Identity.Name;
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

            _context.Attach(jobPlacementModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
            }

            
            return RedirectToPage("./Details", new { id = jobPlacementModel.jobPlacementID });
        }

        private bool jobPlacementModelExists(int id)
        {
            return _context.JobPlacements.Any(e => e.jobPlacementID == id);
        }
        public JsonResult OnGetDepartmentsByCompany(int companyID)
        {
            var departments = _context.Departments
                .Where(d => d.companyID == companyID && d.departmentStatus == mainStatus.Active)
                .OrderBy(d => d.departmentName)
                .Select(d => new { d.departmentID, d.departmentName })
                .ToList();

            return new JsonResult(departments);
        }
        public JsonResult OnGetSalary(int jobStepID)
        {
            var Salary = _context.JobSteps.FirstOrDefault(js => js.jobStepID == jobStepID).jobStepSalary;
            return new JsonResult(Salary);
        }
        public JsonResult OnGetJobGrade(int jobID)
        {
            var jobGradeID = _context.Jobs.FirstOrDefault(j => j.jobID == jobID).jobGradeID;
            return new JsonResult(jobGradeID);
        }
        public JsonResult OnGetJobStep(int jobGradeID)
        {
            var jobSteps = _context.JobSteps
                .Where(js => js.jobGradeID == jobGradeID)
                .Select(js => new { js.jobStepID, js.jobStepNumber })
                .ToList();
            return new JsonResult(jobSteps);
        }
        public void populateSelect()
        {
            ViewData["companyID"] = new SelectList(_context.Companies.Where(c => c.companyStatus == mainStatus.Active).OrderBy(c => c.companyName), "companyID", "companyName");
            ViewData["departmentID"] = new SelectList(_context.Departments.Where(c => c.departmentStatus == mainStatus.Active).OrderBy(c => c.departmentName), "departmentID", "departmentName");
            ViewData["workSiteID"] = new SelectList(_context.WorkSites.Where(s => s.workSiteStatus == mainStatus.Active).OrderBy(c => c.workSiteName), "workSiteID", "workSiteName");
            ViewData["jobGradeID"] = new SelectList(_context.JobGrades.Where(s => s.jobGradeStatus == mainStatus.Active).OrderBy(c => c.jobGradeName), "jobGradeID", "jobGradeName");
            ViewData["jobID"] = new SelectList(_context.Jobs.Where(j => j.jobStatus == mainStatus.Active).OrderBy(c => c.jobTitle), "jobID", "jobTitle");
            ViewData["shiftID"] = new SelectList(_context.Shifts.Where(s => s.shiftStatus == mainStatus.Active), "shiftID", "shiftName");
        }
    }
}
