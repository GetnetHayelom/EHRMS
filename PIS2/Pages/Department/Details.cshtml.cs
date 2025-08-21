using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using PIS2.Views;

namespace PIS2.Pages.Department
{
    public class DetailsModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;
        private readonly PIS2.Models.Core _core;
        public DetailsModel(PIS2.Models.PISContext context, Core core)
        {
            _context = context;
            _core = core;
        }

        public departmentModel departmentModel { get; set; } = default!;
        public DepartmentSummary DepartmentSummary { get; set; }
        public double allowedLeave { get; set; }
        public double leaveCost {  get; set; }
        public double overtimeCost { get; set; }
        public List<EducationLevelData> EducationLevels { get; set; }
        public List<NameAndCount> EmploymentTypes { get; set; }
        public List<YearAndCount> EmploymentHireRate { get; set; }
        public List<YearAndCount> TerminationRate { get; set; }
        public List<YearAndCount> ActiveEmployeeRate { get; set; }
        public List<NameAndCount> WorkSiteEmployees { get; set; }
        public List<NameAndCount> EduLevelSummary { get; set; }
        public int leaveEmployments { get; set; }
        public List<jobPlacementModel> Jobs { get; set; }
        public List<employmentModel> Employments { get; set; }
        public List<EmployeeView> EmploymentView { get; set; }
        public IList<leaveModel> leaveModel { get; set; } = default!;
        public IList<overtimeRecordModel> overtimeModel { get; set; } = default!;
        public string ModifiedBy { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)

        {
            //if (!User.IsInRole("MIE\\PMS_MANAGER"))
            //{
            //    return BadRequest();
            //}
            if (id == null || id==0)
            {
                int personID = _context.Users.FirstOrDefault(u => u.userName == User.Identity.Name).personID;
                Console.WriteLine("SELECTED PERSON ID IS________________" + personID);
                int empID = _context.Employments.FirstOrDefault(e => e.personID == personID && e.employmentStatus == mainStatus.Active).employmentID;
                Console.WriteLine("SELECTED EMPLOYEE ID IS________________" + empID);
                int depID = _context.JobPlacements.FirstOrDefault(jp => jp.employmentID == empID && jp.jobPlacementStatus == mainStatus.Active).departmentID;

                id = depID;
                Console.WriteLine("SELECTED DEPARTMENT ID IS________________" + depID);
                if (id == null || id == 0)
                {
                    return NotFound();
                }
            }

            var departmentmodel = await _context.Departments.Include(d => d.companyModel).FirstOrDefaultAsync(m => m.departmentID == id);
            if (departmentmodel == null)
            {
                return NotFound();
            }
            else
            {
               
                departmentModel = departmentmodel;
                Jobs = _context.JobPlacements.Where(j => j.departmentID ==departmentModel.departmentID && j.jobPlacementStatus == mainStatus.Active)
                    .Include(j => j.employmentModel).ThenInclude(e => e.personModel)
                    .Include(j => j.employmentModel).ThenInclude(e => e.employmentTypeModel)
                    .Include(j=> j.jobModel).ToList();
                //Jobs = _context.JobPlacements.Include(j => j.employmentModel).ThenInclude(e => e.personModel).Where(j => j.departmentID == id).ToList();
                //Employments = _context.Employments.Include(e => e.employmentTypeModel).Distinct().Where(e => Jobs.Select(j => j.employmentID).Contains(e.employmentID)).ToList();
                Employments =Jobs.Select(j => j.employmentModel).Distinct().ToList();


                
                EmploymentView =Employments.Select(ev => new EmployeeView
                    {
                        Employment = ev,
                        Job = ev.JobPlacements.Where(j => j.jobPlacementStatus == mainStatus.Active).First(),
                        Person = ev.personModel,
                        //WorkSite = _context.SiteAssignments.OrderByDescending(j => j.modifiedDate).First(ws => ws.employmentID == ev.employmentID).workSiteModel ?? new workSiteModel(),
                        //Shift = _context.ShiftAssignments.OrderByDescending(j => j.modifiedDate).First(ws => ws.employmentID == ev.employmentID).shiftModel ?? new shiftModel(),
                        Leave = _core.leaveSummary(ev.employmentID)
                    }).OrderBy(ev => ev.Person.personFirstName).ThenBy(ev => ev.Person.personFatherName).ThenBy(ev => ev.Person.personLastName).ToList();


                DepartmentSummary = new DepartmentSummary();
                DepartmentSummary.DepartmentID = departmentmodel.departmentID;
                DepartmentSummary.DepartmentName = departmentmodel.departmentName;

                DepartmentSummary.Employees = Employments.Count();
                        
                DepartmentSummary.Salary = Jobs.Sum(j => j.jobPlacementSalary);

                DepartmentSummary.Leaves = _core.getAllLeaveSummary("Dep", DepartmentSummary.DepartmentID);
                DepartmentSummary.Overtime = _core.getAllOvertime("Dep", departmentModel.departmentID).Sum(ot => ot.GetOtCost);
                DepartmentSummary.xEmployees = Employments
                        .Where(e => e.employmentStatus == mainStatus.Inactive).Count();

                allowedLeave = DepartmentSummary.Leaves.AllowedLeave;
                leaveCost = DepartmentSummary.Leaves.leaveCost;
                overtimeCost = DepartmentSummary.Overtime;
                DepartmentSummary.Total = leaveCost + (double) EmploymentView.Where(ev => ev.Job.jobPlacementStatus == mainStatus.Active).Sum(ev => ev.Job.jobPlacementSalary); ;
                
                // Education Level Data
                
                EducationLevels = _context.PersonEducationLevels
                    .Join(_context.EducationLevels, pel => pel.educationLevelID, el => el.educationLevelID, (pel, el) => new { pel, el })
                    .Where(x => _context.Employments.Any(e => e.personID == x.pel.personID && e.employmentStatus == mainStatus.Active))
                    .GroupBy(x => new { x.el.educationLevelCategory, x.el.educationLevelName })
                    .Select(g => new EducationLevelData
                    {
                        EducationLevelCategory = g.Key.educationLevelCategory,
                        EducationLevelName = g.Key.educationLevelName,
                        EducationLevelCount = g.Count()
                    }).OrderByDescending(e => e.EducationLevelCount).ToList();

                //Employment Types
                EmploymentTypes = new List<NameAndCount>();
                EmploymentTypes = Employments
               .GroupBy(e => new { e.employmentTypeModel.employmentTypeName, e.employmentTypeID })
               .Select(g => new NameAndCount
               {
                   zName = g.Key.employmentTypeName,
                   zCount = g.Count()
               })
               .ToList();

                //Active Leaves
                leaveEmployments = _context.Leaves
                    .Where(l => l.leaveStartDate <= DateTime.Now && l.leaveEndDate >= DateTime.Now
                    && l.leaveTypeModel.leaveTypeImpact == leaveTypeImpact.Negative
                    && l.employmentModel.employmentStatus == mainStatus.Active && l.leaveStatus == leaveStatus.Posted
                    && l.leaveTypeID != 64 && Employments.Select(ae => ae.employmentID).Contains(l.employmentID)).Count();

                leaveModel = await _context.Leaves.Where(l => l.leaveStatus == leaveStatus.Hold && Employments.Select(e => e.employmentID).Contains(l.employmentID)
                && l.leaveTypeModel.leaveGroup == leaveGroup.AnnualLeave)
                .Include(l => l.employmentModel)
                .Include(l => l.leaveTypeModel).ToListAsync();

                //Overtime Model
                overtimeModel =await _context.OvertimeRecords.Where(ot => ot.overtimeRecordStatus == overtimeStatus.Hold && Employments.Select(e => e.employmentID).Contains(ot.employmentID))
                    .Include(ot => ot.employmentModel)
                    .Include(ot => ot.overtimeModel).ToListAsync();
            }
            return Page();
        }
        [HttpPost]
        public async Task<JsonResult> OnPostApprove([FromBody] List<LeaveDecision> decisions)
        {
            // Check if the decisions list is null or empty
            if (decisions == null || decisions.Count == 0)
            {
                return new JsonResult(new { success = false, message = "No decisions received." });
            }
            try
            {
                foreach (var decision in decisions)
                {
                    int leaveId = decision.LeaveId;
                    string action = decision.Action;
                    var leave = await _context.Leaves.FindAsync(leaveId);
                    if (leave != null)
                    {
                        leave.modifiedBy = User.Identity.Name!;
                        Console.WriteLine("######################Error is not here");
                        leave.leaveStatus = action == "Approve"
                            ? Models.leaveStatus.Approved
                            : Models.leaveStatus.Declined;
                    }
                }

                await _context.SaveChangesAsync();
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

    }

    public class LeaveDecision
    {
        public int LeaveId { get; set; }
        public string Action { get; set; }
    }
}
