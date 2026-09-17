using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;
using PIS2.Models.HR;
using PIS2.Models.Organization;
using PIS2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace PIS2.Pages.JobPlacement
{
    [Authorize(Roles = "HRMANAGER")]
    public class EditModel : PageModel
    {
        private readonly PISContext _context;
        private readonly Core _core;
        private readonly ILogger<EditModel> _logger;

        public EditModel(PISContext context, Core core, ILogger<EditModel> logger)
        {
            _context = context;
            _core = core;
            _logger = logger;
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
        public List<jobPlacementHistoryModel> JobPlacementHistory { get; set; }

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
                TempData["message"] = ("Error","Job Placement Not Found.");
                return Page();
            }

            JobPlacementHistory = _context.JobPlacementHistories.Where(j => j.jobPlacementID == id).ToList();
            populateSelect();
            return Page();
        }

        

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var exisingPlacement =await _context.JobPlacements.FirstOrDefaultAsync(j => j.jobPlacementID == jobPlacementModel.jobPlacementID); 
            
            var empStat =await _context.Employments.FirstOrDefaultAsync(e => e.employmentID == exisingPlacement.employmentID);
            var depStat = await _context.Departments.FirstOrDefaultAsync(d => d.departmentID == jobPlacementModel.departmentID);
            if (empStat == null)
            {
                TempData["message"] = ("Error", "Employment not found!");
                populateSelect();
                return Page();
            }

            if (empStat.employmentStatus != mainStatus.Active)
            {
                TempData["message"] = ("Error", "Employment is not active!");
                populateSelect();
                return Page();
            }
            if (depStat == null)
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
            if (exisingPlacement == null)
            {
                TempData["message"] = ("Error", "Job Placement Not Found!");
                populateSelect();
                return Page();
            }
            if (empStat.employmentDate > jobPlacementModel.jobPlacementDate)
            {
                _logger.LogWarning("Attempt to assign job placement date before date of employment {EmploymentID}:" + User.Identity.Name,
                    jobPlacementModel.employmentID);

                TempData["message"] = ("Error", "Job plcaement date can not be before employment date!");
                populateSelect();
                return Page();
            }
            var lastestJobPlacement = await _context.JobPlacements
                .OrderByDescending(j => j.jobPlacementDate)
                .FirstOrDefaultAsync(j => j.employmentID == empStat.employmentID);

            if (lastestJobPlacement.jobPlacementDate > jobPlacementModel.jobPlacementDate)
            {
                _logger.LogWarning("Attempt to assign job placement date overlapping with previous job placement:" + User.Identity.Name,
                    jobPlacementModel.employmentID);

                TempData["message"] = ("Error", "Job plcaement date overlap!");
                populateSelect();
                return Page();
            }
            var prohibitions = _context.Prohibitions.Where(p => p.employmentID == jobPlacementModel.employmentID && p.prohibitionStatus== mainStatus.Active).ToList();

            if (prohibitions.Any(p => p.prohibitionType == ProhibitionType.Scale || p.prohibitionType == ProhibitionType.Step)
                && (exisingPlacement?.jobStepID != jobPlacementModel.jobStepID))
            {
                TempData["message"] = ("Error", "Employee is Under Step or Scale Prohibition!");
                populateSelect();
                return Page();
            }

            if (prohibitions.Any(p => p.prohibitionType == ProhibitionType.Promotion)
                && (jobPlacementModel?.jobPlacementReason != "Promotion"))
            {
                TempData["message"] = ("Error", "Employee is Under Promotion Prohibition!");
                populateSelect();
                return Page();
            }

            if (prohibitions.Any(p => p.prohibitionType == ProhibitionType.Transfer)
                && ((jobPlacementModel?.jobPlacementReason != "Transfer") || (exisingPlacement.departmentID != jobPlacementModel.departmentID)))
            {
                TempData["message"] = ("Error", "Employee is Under Transfer Prohibition!");
                populateSelect();
                return Page();
            }

            ModelState.Clear();
            

            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        Console.WriteLine($"{kv.Key} --> {error.ErrorMessage}");

                    }
                    
                }
                TempData["message"] = ("Error", "Check All Fileds Have Values!");
                populateSelect();
                return Page();
            }

            var jp = _context.JobPlacements.FirstOrDefault(j => j.jobPlacementID == jobPlacementModel.jobPlacementID);

            if (jp == null)
            {
                jobPlacementModel.jobPlacementCareer = JobCareer.Step;
            }
            else if (jp != null && jp.jobStepID == jobPlacementModel.jobStepID)
            {
                jobPlacementModel.jobPlacementCareer = JobCareer.Transfer;
            }
            else if (jp != null && _core.IsPromotion(jp.jobStepModel.jobGradeModel,
                    jobPlacementModel.jobStepModel.jobGradeID))
            {
                jobPlacementModel.jobPlacementCareer = JobCareer.Promotion;
            }
            else if (jp != null && jp.jobStepID != jobPlacementModel.jobStepID && jobPlacementModel.jobStepModel.jobGradeID == jp.jobStepModel.jobGradeID)
            {
                jobPlacementModel.jobPlacementCareer = JobCareer.Step;
            }
            else { jobPlacementModel.jobPlacementCareer = JobCareer.Demotion; }

            if (jp != jobPlacementModel)
            {
                jp.departmentID = jobPlacementModel.departmentID;
                jp.jobPlacementDate = jobPlacementModel.jobPlacementDate;
                jp.jobPlacementReference = jobPlacementModel.jobPlacementReference;
                jp.jobPlacementReason = jobPlacementModel.jobPlacementReason;
                jp.jobPlacementStatus = jobPlacementModel.jobPlacementStatus;
                jp.jobPlacementSalary = jobPlacementModel.jobPlacementSalary;
                jp.jobStepID = jobPlacementModel.jobStepID;
                jp.modifiedBy = User.Identity.Name;
            }

            _context.Attach(jp).State = EntityState.Modified;
            try
            {
                   
                await _context.SaveChangesAsync();
                //TempData["message"] = ("Success", "Job Placement Updated Successfully!");
                return RedirectToPage("./Details", new { id = jobPlacementModel.jobPlacementID });
            }
            catch (DbUpdateException ex)
            {

                throw;
            }
            
           
        }
        
        public JsonResult OnGetSalary(int jobStepID)
        {
            var Salary = _context.JobSteps.FirstOrDefault(js => js.jobStepID == jobStepID).jobStepSalary;
            return new JsonResult(Salary);
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
