using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using PIS2.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Employment
{
    public class DetailsModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;
        private readonly PIS2.Models.Core _core;
        public DetailsModel(PIS2.Models.PISContext context, PIS2.Models.Core core)
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

            if (!User.IsInRole("MIE\\PMS_HRCLERK") || !User.IsInRole("MIE\\PMS_HRMANAGER") || !User.IsInRole("MIE\\PMS_MANAGEMENT") || _core.IsSelf(User.Identity.Name, id))
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
                .Include(e => e.JobPlacements).ThenInclude(j => j.jobStepModel)
                .Include(e => e.JobPlacements).ThenInclude(jp => jp.jobModel).ThenInclude(j => j.jobGradeModel)
                .Include(e => e.JobPlacements).ThenInclude(jp => jp.jobModel).ThenInclude(j => j.jobCategoryModel)
                .Include(e => e.JobPlacements).ThenInclude(jp => jp.jobModel).ThenInclude(j => j.jobClassModel)
                .Include(e => e.JobPlacements).ThenInclude(jp => jp.departmentModel).ThenInclude(d => d.companyModel).FirstOrDefaultAsync(m => m.employmentID == id);
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
            var currentUser = _context.Users.FirstOrDefault(u => u.userName == User.Identity.Name);

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
                Exprience = _core.GetYearsAndMonths(employmentModel.employmentDate, DateTime.Now).Item1 + " years " +
                    _core.GetYearsAndMonths(employmentModel.employmentDate, employmentModel.employmentTerminationDate ?? DateTime.MinValue).Item2 + " months ";
            }

            EvalReport =GetSingleEvaluationReport(employmentmodel.employmentID);

            return Page();
        }

        

        public List<EvalSingleEmployeeReport> GetSingleEvaluationReport(int empId)
        {
            // 1. Fetch data from the view for the specific evaluation
            var viewData = _context.EvaluationSummaryView
                .Where(v => v.employmentID == empId)
                .ToList();

            if (!viewData.Any()) return null;

            // 2. Build the structured report using LINQ GroupBy
            var report = viewData
                .GroupBy(v => new { v.evaluationID })
                .Select(eGroup => new EvalSingleEmployeeReport
                {
                    evaluationID = eGroup.Key.evaluationID,
                    evaluationName = eGroup.FirstOrDefault().evaluationName,
                    startDate = eGroup.FirstOrDefault().evaluationStartDate,
                    endDate = eGroup.FirstOrDefault().evaluationEndDate,

                    // Total of all weighted subtask scores
                    FinalGrandTotal = eGroup.Sum(x => x.WeightedSubTaskScore),

                    Types = eGroup.GroupBy(t => new { t.evaluationTypeName, t.evaluationTypeWeight })
                        .Select(tGroup => new EvalTypeSummary
                        {
                            typeName = tGroup.Key.evaluationTypeName,
                            typeWeight = tGroup.Key.evaluationTypeWeight,
                            
                            Tasks = tGroup.GroupBy(tk => new { tk.evaluationTaskName, tk.evaluationTaskWeight })
                                .Select(tkGroup => new EvalTaskSummary
                                {
                                    taskName = tkGroup.Key.evaluationTaskName,
                                    taskWeight = tkGroup.Key.evaluationTaskWeight,
                                    avgTime = tkGroup.Average(x => x.timeValuation),
                                    avgResource = tkGroup.Average(x => x.resourceValuation),
                                    avgPerformance = tkGroup.Average(x => x.performanceValuation),

                                    // Group by SubTaskID to get unique subtasks
                                    SubTasks = tkGroup.GroupBy(st => st.evaluationSubTaskID)
                                        .Select(stGroup => new EvalSubTaskSummary
                                        {
                                            subtaskName = stGroup.First().evaluationSubTaskName,
                                            subtaskWeight = stGroup.First().evaluationSubTaskWeight,
                                            subTime = stGroup.First().timeValuation,
                                            subResource = stGroup.First().resourceValuation,
                                            subPerformance = stGroup.First().performanceValuation
                                        }).ToList()
                                    
                                }).ToList()


                        }).ToList()
                }).ToList();

            return report;
        }
    }
}
