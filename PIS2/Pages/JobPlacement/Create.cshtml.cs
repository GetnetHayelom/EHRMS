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
    public class CreateModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public CreateModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public jobPlacementModel jobPlacementModel { get; set; } = default!;
        [BindProperty]
        public int company { get; set; }
        [BindProperty]
        public employmentModel employmentModel { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? givenID { get; set; }
        public string ErrorMessage { get; set; }
        public IActionResult OnGet(int? id)
        {
            jobPlacementModel = new jobPlacementModel
            {
                jobPlacementDate = DateTime.Now
            };
            employmentModel = new employmentModel();
            if (!string.IsNullOrEmpty(givenID))
            {
                var emp = _context.Employments
                    .FirstOrDefault(e => e.givenID == givenID);

                if (emp != null)
                {

                    // Redirect to the Details page with employmentID
                    id = emp.employmentID;
                    Console.WriteLine("############## The ID is == " + id);
                    employmentModel = _context.Employments.Include(e => e.personModel).FirstOrDefault(e => e.employmentID == id);

                    populateSelect();

                    return Page();
                    //return RedirectToPage("Create", new { id = emp.employmentID });
                }

                ErrorMessage = "No employee found with that Given ID.";
            }
            if (id != null && id != 0)
            {
                employmentModel = _context.Employments.Include(e => e.personModel).FirstOrDefault(e => e.employmentID == id);
            }
            populateSelect();
            return Page();
        }

        

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine("########### " + jobPlacementModel.jobPlacementID);
            ModelState.Remove("modifiedBy");
            ModelState.Remove("givenID");
            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        Console.WriteLine($"{kv.Key} --> {error.ErrorMessage}");

                    }
                }
                Console.WriteLine("===ID IS===");
                populateSelect();
                return Page();
            }
            populateSelect();
            var job = _context.JobPlacements.FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active && j.employmentID == jobPlacementModel.employmentID);
            if(job !=null && job.jobID == jobPlacementModel.jobID)
            {
                job.jobStepID = jobPlacementModel.jobStepID;
                job.departmentID = jobPlacementModel.departmentID;
                job.jobPlacementSalary = jobPlacementModel.jobPlacementSalary;
                job.jobPlacementReason = jobPlacementModel.jobPlacementReason;
                job.jobPlacementReference = jobPlacementModel.jobPlacementReference;
                job.modifiedBy = jobPlacementModel.modifiedBy;
                
                //_context.Attach(job).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToPage("./Edit", new { id = job.jobPlacementID });
                }
                catch (DbUpdateConcurrencyException)
                {
                    
                        throw;
                   
                }
            }
            else
            {
                _context.JobPlacements.Add(jobPlacementModel);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToPage("./Edit", new {id = jobPlacementModel.jobPlacementID});
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
