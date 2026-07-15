using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;
using PIS2.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.JobPlacement
{
    [Authorize(Roles = "MIE\\PMS_HRCLERK")]
    public class CreateModel : PageModel
    {
        private readonly PISContext _context;
        private readonly Core _core;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(PISContext context, Core core, ILogger<CreateModel> logger)
        {
            _context = context;
            _core = core;
            _logger = logger;
        }

        [BindProperty]
        public jobPlacementModel jobPlacementModel { get; set; } = default!;

        [BindProperty]
        public int company { get; set; }

        [BindProperty]
        public employmentModel employmentModel { get; set; }

        public SelectList Employments { get; set; }

        public string ErrorMessage { get; set; }

        public IActionResult OnGet(int? id)
        {
            _logger.LogInformation("Job placement create page accessed by {User}", User.Identity?.Name);

            
            populateSelect();
            jobPlacementModel = new jobPlacementModel
            {
                jobPlacementDate = DateTime.Now
            };

            employmentModel = new employmentModel();

            if (id != null && id != 0)
            {
                employmentModel = _context.Employments
                    .Include(e => e.personModel)
                    .FirstOrDefault(e => e.employmentID == id && e.employmentStatus == mainStatus.Active);

                _logger.LogInformation("Employment loaded using EmploymentID {EmploymentID}", id);
            }

           

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Job placement submission started by {User}", User.Identity?.Name);

            var empStat = await _context.Employments
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.employmentID == jobPlacementModel.employmentID);

            var depStat = await _context.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.departmentID == jobPlacementModel.departmentID);

            if (empStat == null)
            {
                _logger.LogWarning("Employment not found for ID {EmploymentID}", jobPlacementModel.employmentID);

                TempData["message"] = ("Error", "Employment not found!");
                populateSelect();
                return Page();
            }

            if (empStat.employmentStatus != mainStatus.Active)
            {
                _logger.LogWarning("Attempt to assign job to inactive employment {EmploymentID}",
                    jobPlacementModel.employmentID);

                TempData["message"] = ("Error", "Employee is not active!");
                populateSelect();
                return Page();
            }
            if(empStat.employmentDate > jobPlacementModel.jobPlacementDate)
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

            if (lastestJobPlacement?.jobPlacementDate > jobPlacementModel.jobPlacementDate) 
            {
                _logger.LogWarning("Attempt to assign job placement date overlapping with previous job placement:" + User.Identity.Name,
                    jobPlacementModel.employmentID);

                TempData["message"] = ("Error", "Job plcaement date overlap!");
                populateSelect();
                return Page();
            }

            if (depStat == null)
            {
                _logger.LogWarning("Department not found {DepartmentID}", jobPlacementModel.departmentID);

                TempData["message"] = ("Error", "Department not found!");
                populateSelect();
                return Page();
            }

            if (depStat.departmentStatus != mainStatus.Active)
            {
                _logger.LogWarning("Attempt to assign job to inactive department {DepartmentID}",
                    jobPlacementModel.departmentID);

                TempData["message"] = ("Error", "Department is not active!");
                populateSelect();
                return Page();
            }

            var prohibitions = _context.Prohibitions
                .Where(p => p.employmentID == jobPlacementModel.employmentID &&
                            p.prohibitionStatus == mainStatus.Active)
                .ToList();

            if (prohibitions.Any(p =>
                new[]
                {
                    ProhibitionType.Scale,
                    ProhibitionType.Step,
                    ProhibitionType.Promotion,
                    ProhibitionType.Transfer
                }.Contains(p.prohibitionType)))
            {
                _logger.LogWarning("Job placement blocked due to active prohibition for EmploymentID {EmploymentID}",
                    jobPlacementModel.employmentID);

                TempData["message"] = ("Error", "Can not assign job to prohibited employment!");
                populateSelect();
                return Page();
            }

            ModelState.Remove("modifiedBy");
            ModelState.Remove("jobPlacementModel.modifiedBy");
            ModelState.Remove("givenID");

            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        _logger.LogWarning("ModelState error: Field {Field} -> {Error}",
                            kv.Key, error.ErrorMessage);
                    }
                }

                populateSelect();
                return Page();
            }

            var job = await _context.JobPlacements.Include(j => j.jobStepModel).ThenInclude(j => j.jobGradeModel)
                .FirstOrDefaultAsync(j =>
                    j.jobPlacementStatus == mainStatus.Active &&
                    j.employmentID == jobPlacementModel.employmentID);

            var newJobStep = await _context.JobSteps.FirstOrDefaultAsync(j => j.jobStepID == jobPlacementModel.jobStepID);
            if (job == null)
            {
                jobPlacementModel.jobPlacementCareer = JobCareer.Step;
            }
            else if (job != null && job.jobStepID == jobPlacementModel.jobStepID)
            {
                jobPlacementModel.jobPlacementCareer = JobCareer.Transfer;
            }
            else if (job != null && _core.IsPromotion(job.jobStepModel?.jobGradeModel,
                    newJobStep.jobGradeID))
            {
                jobPlacementModel.jobPlacementCareer = JobCareer.Promotion;
            }
            else if (job != null && job.jobStepID != jobPlacementModel.jobStepID && newJobStep.jobGradeID == job.jobStepModel.jobGradeID)
            {
                jobPlacementModel.jobPlacementCareer = JobCareer.Step;
            }
            else { jobPlacementModel.jobPlacementCareer = JobCareer.Demotion; }

            if (job != null && job.jobID == jobPlacementModel.jobID)
            {
                _logger.LogInformation("Updating existing job placement for EmploymentID {EmploymentID}",
                    jobPlacementModel.employmentID);

                job.jobStepID = jobPlacementModel.jobStepID;
                job.departmentID = jobPlacementModel.departmentID;
                job.jobPlacementSalary = jobPlacementModel.jobPlacementSalary;
                job.jobPlacementReason = jobPlacementModel.jobPlacementReason;
                job.jobPlacementReference = jobPlacementModel.jobPlacementReference;
                job.modifiedBy = User.Identity.Name;

                try
                {
                    _context.Attach(job).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Job placement updated successfully. PlacementID {PlacementID}",
                        job.jobPlacementID);

                    return RedirectToPage("./Details", new { id = job.jobPlacementID });
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex,
                        "Database error while updating job placement for EmploymentID {EmploymentID}",
                        jobPlacementModel.employmentID);

                    ModelState.AddModelError(string.Empty, ex.InnerException?.Message ?? ex.Message);

                    populateSelect();
                    return Page();
                }
            }
            else
            {
                _logger.LogInformation("Creating new job placement for EmploymentID {EmploymentID}",
                    jobPlacementModel.employmentID);

                jobPlacementModel.modifiedBy = User.Identity.Name;
                _context.JobPlacements.Add(jobPlacementModel);
                await _context.SaveChangesAsync();

                _logger.LogInformation("New job placement created with ID {PlacementID}",
                    jobPlacementModel.jobPlacementID);
            }

            return RedirectToPage("./Details", new { id = jobPlacementModel.jobPlacementID });
        }

        public void populateSelect()
        {
            var emps = _context.Employments.AsNoTracking().Where(e => e.employmentStatus == mainStatus.Active)
                .OrderBy(e => e.personModel.personFirstName).ThenBy(e => e.personModel.personFatherName).ThenBy(e => e.personModel.personLastName)
                .Select(e => new {
                    Value = e.employmentID,
                    Text = $"{e.givenID}-{e.personModel.personFullName}"
                }).ToList();

            Employments = new SelectList(emps, "Value", "Text");

            ViewData["companyID"] = new SelectList(
                _context.Companies.Where(c => c.companyStatus == mainStatus.Active)
                    .OrderBy(c => c.companyName),
                "companyID", "companyName");

            ViewData["workSiteID"] = new SelectList(
                _context.WorkSites.Where(s => s.workSiteStatus == mainStatus.Active)
                    .OrderBy(c => c.workSiteName),
                "workSiteID", "workSiteName");

            ViewData["jobGradeID"] = new SelectList(
                _context.JobGrades.Where(s => s.jobGradeStatus == mainStatus.Active)
                    .OrderBy(c => c.jobGradeName),
                "jobGradeID", "jobGradeName");

            ViewData["jobID"] = new SelectList(
                _context.Jobs.Where(j => j.jobStatus == mainStatus.Active)
                    .OrderBy(c => c.jobTitle),
                "jobID", "jobTitle");

            ViewData["shiftID"] = new SelectList(
                _context.Shifts.Where(s => s.shiftStatus == mainStatus.Active),
                "shiftID", "shiftName");
        }
    }
}