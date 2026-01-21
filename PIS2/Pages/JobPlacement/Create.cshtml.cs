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
    [Authorize(Roles = "MIE\\PMS_HRCLERK")]
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
            var empStat = _context.Employments.FirstOrDefault(e => e.employmentID == jobPlacementModel.employmentID);
            var depStat = await _context.Departments.FirstOrDefaultAsync(d => d.departmentID == jobPlacementModel.departmentID);
            if(empStat == null)
            {
                TempData["message"] = ("Error", "Employment not found!");
                populateSelect();
                return Page();
            }

            if(empStat.employmentStatus != mainStatus.Active)
            {
                TempData["message"] =("Error","Employee is not active!") ;
                populateSelect();
                return Page();
            }
            if (depStat== null)
            {
                TempData["message"] = ("Error", "Department not found!");
                populateSelect();
                return Page();
            }
            if (depStat?.departmentStatus != mainStatus.Active)
            {
                TempData["message"] = ("Error", "Department is not active!");
                populateSelect();
                return Page();
            }

            var exisingPlacement = _context.JobPlacements.FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active && j.employmentID == jobPlacementModel.employmentID);
            
            if (exisingPlacement == null)
            {
                exisingPlacement = _context.JobPlacements.OrderByDescending(j => j.jobPlacementDate).FirstOrDefault(j => j.employmentID == jobPlacementModel.employmentID);
            }

            var prohibitions = _context.Prohibitions.Where(p => p.employmentID == jobPlacementModel.employmentID && p.prohibitionStatus == mainStatus.Active).ToList();

            if (prohibitions.Any(p => 
            new[] { ProhibitionType.Scale, ProhibitionType.Step, ProhibitionType.Promotion, ProhibitionType.Transfer }.Contains(p.prohibitionType) ))
            {
                TempData["message"] = ("Error", "Can not assign job to prohibited employment!");
                populateSelect();
                return Page();
            }

            ModelState.Remove("modifiedBy");
            
            ModelState.Remove("jobPlacementModel.modifiedBy");
            jobPlacementModel.modifiedBy = User.Identity.Name;
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
                
                populateSelect();
                return Page();
            }
            

            var job = _context.JobPlacements.FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active && j.employmentID == jobPlacementModel.employmentID);
            
            if(job !=null && job.jobID == jobPlacementModel.jobID)
            {
                job.jobStepID = jobPlacementModel.jobStepID;
                job.departmentID = jobPlacementModel.departmentID;
                job.jobPlacementSalary = jobPlacementModel.jobPlacementSalary;
                job.jobPlacementReason = jobPlacementModel.jobPlacementReason;
                job.jobPlacementReference = jobPlacementModel.jobPlacementReference;
                job.modifiedBy = jobPlacementModel.modifiedBy;
                
                
                Console.WriteLine("===ID IS==  #############" + jobPlacementModel.employmentID);
                try
                {
                    _context.Attach(job).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    return RedirectToPage("./Details", new { id = job.jobPlacementID });
                }
                catch (DbUpdateException ex)
                {
                  
                    ModelState.AddModelError(string.Empty, ex.InnerException.Message);

                    throw;
                }
            }
            else
            {
                _context.JobPlacements.Add(jobPlacementModel);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToPage("./Details", new {id = jobPlacementModel.jobPlacementID});
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
