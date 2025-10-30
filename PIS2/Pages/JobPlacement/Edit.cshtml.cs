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

namespace PIS2.Pages.JobPlacement
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
        public jobPlacementModel jobPlacementModel { get; set; } = default!;
        [BindProperty]
        public string company { get; set; }
        [BindProperty]
        public departmentModel departmentModel { get; set; }

        [BindProperty]
        public string? jobGradeModel { get; set; }
        [BindProperty]
        public jobStepModel jobStepModel { get; set; }
        public string ErrorMessage { get; set; }
        public IActionResult OnGet(int id)
        {
            if (id == null && id == 0)
            {
                return NotFound();
            }
            
            jobPlacementModel = _context.JobPlacements
                .Include(jp => jp.employmentModel).ThenInclude(e => e.personModel)
                .Include(jp => jp.departmentModel).ThenInclude(d => d.companyModel)
                .Include(jp => jp.jobStepModel).ThenInclude(js => js.jobGradeModel)
                .Include(jp => jp.jobModel)
                .FirstOrDefault(jp => jp.jobPlacementID == id) ?? new jobPlacementModel();

            company = jobPlacementModel.departmentModel.companyModel.companyName;

            jobGradeModel = jobPlacementModel.jobStepModel?.jobGradeModel?.jobGradeName;
            if(jobPlacementModel == null)
            {
                ErrorMessage = "Job Placement Not Found.";
                return Page();
            }
            
           
            populateSelect();
            return Page();
        }

        

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("modifiedBy");
            ModelState.Remove("jobStepName");
            ModelState.Remove("departmentName");
            ModelState.Remove("jobPlacementModel.modifiedBy");
            jobPlacementModel.modifiedBy = User.Identity.Name;
            

            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        Console.WriteLine($"{kv.Key} --> {error.ErrorMessage}");

                    }
                    
                }

                return Page();
            }
            

            _context.Attach(jobPlacementModel).State = EntityState.Modified;
            try
            {
                   
                await _context.SaveChangesAsync();
                return RedirectToPage("./Details", new { id = jobPlacementModel.jobPlacementID });
            }
            catch (DbUpdateException ex)
            {

                throw;
            }
            
           
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
                .Select(js => new {js.jobStepID, js.jobStepNumber})
                .ToList();
            return new JsonResult(jobSteps);
        }
        public void populateSelect()
        {
            ViewData["companyID"] = new SelectList(_context.Companies.Where(c => c.companyStatus == mainStatus.Active).OrderBy(c => c.companyName), "companyID", "companyName");
            ViewData["workSiteID"] = new SelectList(_context.WorkSites.Where(s => s.workSiteStatus == mainStatus.Active).OrderBy(c => c.workSiteName), "workSiteID", "workSiteName");
            ViewData["jobGradeID"] = new SelectList(_context.JobGrades.Where(s => s.jobGradeStatus == mainStatus.Active).OrderBy(c => c.jobGradeName), "jobGradeID", "jobGradeName");
            ViewData["jobID"] = new SelectList(_context.Jobs.Where(j => j.jobStatus == mainStatus.Active).OrderBy(c => c.jobTitle), "jobID", "jobTitle");
            ViewData["shiftID"] = new SelectList(_context.Shifts.Where(s => s.shiftStatus == mainStatus.Active), "shiftID", "shiftName");
        }
       
    }
}
