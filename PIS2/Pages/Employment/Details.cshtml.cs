using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;
using PIS2.Services;
using PIS2.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Employment
{
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;
        private readonly Core _core;
        public DetailsModel(PISContext context, Core core)
        {
            _context = context;
            _core = core;
        }
        [BindProperty(SupportsGet = true)]
        public string givenID { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsUserSelf { get; set; }
        public bool isSelf = false;
        public employmentModel employmentModel { get; set; } = default!;
        public string Age { get; set; }
        public string Exprience { get; set; }
        public decimal Severance { get; set; }
        public leaveDetail? LeaveDetail { get; set; } = new leaveDetail();
        public List<overtimeRecordModel>? Overtimes { get; set; } = default!;
        public ICollection<leaveModel>? Leaves { get; set; } = new List<leaveModel>();
        public personModel Person { get; set; }
        public List<EvalSingleEmployeeReport> EvalReport { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {

            var isInRole =
                User.IsInRole("HRPERSONNEL") ||
                User.IsInRole("HRMANAGER") ||
                User.IsInRole("MANAGEMENT");

            var isSelf = _core.IsSelf(User.Identity.Name, id);

            if (!isInRole && !isSelf)
            {
                return RedirectToPage("/Shared/AccessDenied");
            }


            if (!string.IsNullOrEmpty(givenID))
            {
                var emp = await _context.Employments
                     .FirstOrDefaultAsync(e => e.givenID == givenID);

                if (emp != null)
                {
                    Leaves =await _context.Leaves.Include(l => l.leaveTypeModel).OrderByDescending(l => l.leaveRequestDate).Where(l => l.employmentID == emp.employmentID).ToListAsync();
                    LeaveDetail = await _core.GetLeaveSummary(emp.employmentID);
                    Overtimes = await _context.OvertimeRecords.Include(o => o.overtimeModel).OrderByDescending(l => l.overtimeRecordDate).Where(l => l.employmentID == emp.employmentID).ToListAsync();
                    Severance =await _core.GetSeverance(emp.employmentID);
                    Person = await _context.Persons.Include(p => p.Employments).FirstOrDefaultAsync(p => p.personID == emp.personID);
                    return RedirectToPage("Details", new { id = emp.employmentID });
                }

                ErrorMessage = "No employee found with that Given ID.";
            }

            if (id == null)
            {
                return NotFound();
            }
            var employmentmodel = await _context.Employments.Include(e => e.EmploymentHistories)
                .Include(e => e.TerminationModel)
                .Include(e => e.personModel).ThenInclude(p => p.PersonEducationLevels).ThenInclude(pe => pe.educationLevelModel)
                .Include(e => e.employmentTypeModel)
                .Include(e => e.Leaves)
                .Include(e => e.JobPlacements).ThenInclude(j => j.jobStepModel).ThenInclude(j => j.jobGradeModel)
                .Include(e => e.JobPlacements).ThenInclude(jp => jp.jobModel).ThenInclude(j => j.jobCategoryModel)
                .Include(e => e.JobPlacements).ThenInclude(jp => jp.jobModel).ThenInclude(j => j.jobClassModel)
                .Include(e => e.JobPlacements).ThenInclude(jp => jp.departmentModel).ThenInclude(d => d.companyModel)
                .FirstOrDefaultAsync(m => m.employmentID == id);
            if (employmentmodel == null)
            {
                return NotFound();
            }
            else
            {
                Leaves =await _context.Leaves.Include(l => l.leaveTypeModel).OrderByDescending(l => l.leaveRequestDate).Where(l => l.employmentID == employmentmodel.employmentID).ToListAsync();
                LeaveDetail = await _core.GetLeaveSummary(employmentmodel.employmentID);
                Overtimes = await _context.OvertimeRecords.Include(o => o.overtimeModel).OrderByDescending(l => l.overtimeRecordDate).Where(l => l.employmentID == employmentmodel.employmentID).ToListAsync();
                employmentModel = employmentmodel;
                Severance =await _core.GetSeverance(id ?? 0);
                Person =await _context.Persons
                    .Include(p => p.Employments)
                    .Include(p => p.addressModel).FirstOrDefaultAsync(p => p.personID == employmentModel.personID);
            }
            var currentUser = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);

            if (currentUser != null && currentUser.personID == employmentModel.personID)
            {
                isSelf = true;
            }


            Age = _core.GetYearsAndMonths(employmentModel.personModel.personDoB, DateTime.Now).Item1 + " years " +
                _core.GetYearsAndMonths(employmentModel.personModel.personDoB, DateTime.Now).Item2 + " months ";

            if (employmentmodel.employmentStatus == mainStatus.Active)
            {
                Exprience = _core.GetYearsAndMonths(employmentModel.employmentDate, DateTime.Now).Item1 + " years " +
                    _core.GetYearsAndMonths(employmentModel.employmentDate, DateTime.Now).Item2 + " months ";
            }
            else
            {
                Exprience = _core.GetYearsAndMonths(employmentModel.employmentDate, employmentModel.employmentDate).Item1 + " years " +
                    _core.GetYearsAndMonths(employmentModel.employmentDate, employmentModel.employmentTerminationDate ?? DateTime.MinValue).Item2 + " months ";
            }

            EvalReport =_core.GetSingleEvaluationReport(employmentmodel.employmentID);

            return Page();
        }
        public async Task<JsonResult> OnGetEmployeeProgress(int employmentId)
        {
            // 1. Get Employee Placement History
            var history = await _context.JobPlacements
                .Include(p => p.jobStepModel)
                .ThenInclude(s => s.jobGradeModel)
                .Where(p => p.employmentID == employmentId)
                .OrderBy(p => p.jobPlacementDate)
                .ToListAsync();

            // 2. Prepare Data Lists
            var dates = history.Select(h => h.jobPlacementDate.ToString("MM/dd/yyyy")).ToList();
            var empSalaries = history.Select(h => h.jobPlacementSalary).ToList();

            // 3. Calculate Peer Averages & Grade Path
            var avgSalaries = new List<decimal>();
            var gradeMids = new List<double>();
            var gradeMax = new List<double>();

            foreach (var placement in history)
            {
                // Average salary of all employees in the same department at the time of this placement
                var avg = _context.JobPlacements
                    .Where(p => p.departmentID == placement.departmentID && p.jobPlacementDate <= placement.jobPlacementDate)
                    .Average(p => (decimal?)p.jobPlacementSalary) ?? 0;

                avgSalaries.Add(Math.Round(avg, 2));

                // Get the Grade boundaries for the path line
                if (placement.jobStepModel?.jobGradeModel != null)
                {
                    gradeMids.Add(placement.jobStepModel.jobGradeModel.jobGradeMidSalary/12);
                    gradeMax.Add(placement.jobStepModel.jobGradeModel.jobGradeMaxSalary/12);
                }
            }

            var model = new CareerProgressionViewModel
            {
                Dates = dates,
                EmployeeSalaries = empSalaries,
                AverageSalaries = avgSalaries,
                GradeMidpoints = gradeMids,
                GradeMax = gradeMax
            };

            return new JsonResult(model);
        }
        public async Task<JsonResult> OnGetCareerPath(int employmentId)
        {
            // 1. Get the current placement to identify the starting point
            var currentPlacement = await _context.JobPlacements
                .Include(p => p.jobModel)
                .Where(p => p.employmentID == employmentId)
                .OrderByDescending(p => p.jobPlacementDate)
                .FirstOrDefaultAsync();

            if (currentPlacement == null || currentPlacement.jobModel == null)
                return new JsonResult(new { error = "No placement history found" });

            var currentJob = currentPlacement.jobModel;
            var careerPath = new List<object>();

            // 2. Start traversing the Grades
            int? nextGradeId = currentJob.jobGradeID;
            var processedGrades = new HashSet<int>(); // Prevent infinite loops

            while (nextGradeId.HasValue && !processedGrades.Contains(nextGradeId.Value))
            {
                processedGrades.Add(nextGradeId.Value);

                // Fetch the Grade details and the potential jobs in this grade
                // filtered by the SAME Class and Category
                var gradeWithJobs = await _context.JobGrades
                    .Where(g => g.jobGradeID == nextGradeId)
                    .Select(g => new
                    {
                        GradeName = g.jobGradeName,
                        SalaryRange = $"${g.jobGradeBasicSalary/12} - ${g.jobGradeMaxSalary/12}",
                        PotentialTitles = _context.Jobs
                            .Where(j => j.jobGradeID == g.jobGradeID &&
                                        j.jobClassID == currentJob.jobClassID &&
                                        j.jobStatus == mainStatus.Active)
                            .Select(j => j.jobTitle)
                            .ToList(),
                        NextGradeID = g.NextJobGradeID
                    })
                    .FirstOrDefaultAsync();

                if (gradeWithJobs != null)
                {
                    careerPath.Add(gradeWithJobs);
                    nextGradeId = gradeWithJobs.NextGradeID;
                }
                else
                {
                    break;
                }
            }

            return new JsonResult(careerPath);
        }
    }
    public class CareerProgressionViewModel
    {
        public List<string> Dates { get; set; }
        public List<decimal> EmployeeSalaries { get; set; }
        public List<decimal> AverageSalaries { get; set; }
        public List<double> GradeMidpoints { get; set; }
        public List<double> GradeMax { get; set; }
        public List<double> GradeMin { get; set; }
    }
}
