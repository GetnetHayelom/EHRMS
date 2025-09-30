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
        public decimal allowedLeave { get; set; }
        public decimal leaveCost { get; set; }
        public decimal overtimeCost { get; set; }
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
        public List<shiftModel> Shifts { get; set; }
        public List<workSiteModel> WorkSites { get; set; }
        public string ModifiedBy { get; set; }
        public bool isManager { get; set; }
        public bool isMember { get; set; }
        public bool isDelegatee { get; set; }


        public async Task<IActionResult> OnGetAsync(int? id)

        {
            Shifts=await _context.Shifts.Where(s => s.shiftStatus == mainStatus.Active).ToListAsync();
            WorkSites = await _context.WorkSites.Where(s => s.workSiteStatus == mainStatus.Active).ToListAsync();

            int personID = _context.Users.FirstOrDefault(u => u.userName == User.Identity.Name).personID;
            
            int empID = _context.Employments.FirstOrDefault(e => e.personID == personID && e.employmentStatus == mainStatus.Active).employmentID;
            
            int depID = _context.JobPlacements.FirstOrDefault(jp => jp.employmentID == empID && jp.jobPlacementStatus == mainStatus.Active).departmentID;
            
            if (id == null || id==0)
            {

                if (depID != 0)
                {
                    id = depID;
                    
                }
                else {return NotFound(); }
            }
            
            var departmentmodel = await _context.Departments.Include(d => d.companyModel)
                .Include(d => d.employmentModel).ThenInclude(e => e.personModel)
                .OrderBy(d => d.departmentName).FirstOrDefaultAsync(m => m.departmentID == id);
            
            departmentModel = departmentmodel;
                isMember = depID== id? true: false;
                isManager= empID == departmentModel?.employmentID? true: false;
                isDelegatee = _context.Delegations
                .Where(d => d.delegationFrom == departmentModel.employmentID && d.delegationStatus == mainStatus.Active)?
                .FirstOrDefault()?.delegationTo ==empID ? true: false;
                
            Jobs = _context.JobPlacements.Where(j => j.departmentID ==departmentModel.departmentID && j.jobPlacementStatus == mainStatus.Active)
                .Include(j => j.employmentModel)?.ThenInclude(e => e.personModel)
                .Include(j => j.employmentModel)?.ThenInclude(e => e.employmentTypeModel)
                .Include(j=> j.jobModel).ToList() ?? new List<jobPlacementModel>(); 
            Employments =Jobs.Select(j => j.employmentModel).Distinct().ToList();
                //Jobs = _context.JobPlacements.Include(j => j.employmentModel).ThenInclude(e => e.personModel).Where(j => j.departmentID == id).ToList();
                //Employments = _context.Employments.Include(e => e.employmentTypeModel).Distinct().Where(e => Jobs.Select(j => j.employmentID).Contains(e.employmentID)).ToList();
                

                
                EmploymentView =Employments.Select(ev => new EmployeeView
                    {
                        Employment = ev,
                        Job = ev.JobPlacements.Where(j => j.jobPlacementStatus == mainStatus.Active).First(),
                        Person = ev.personModel,
                        WorkSite = _context.SiteAssignments.OrderByDescending(j => j.modifiedDate).First(ws => ws.employmentID == ev.employmentID).workSiteModel ?? new workSiteModel(),
                        Shift = _context.ShiftAssignments.OrderByDescending(sa => sa.modifiedDate).First(sa => sa.employmentID == ev.employmentID).shiftModel ?? new shiftModel(),
                        Leave = _core.leaveSummary(ev.employmentID)
                    }).OrderBy(ev => ev.Person.personFirstName).ThenBy(ev => ev.Person.personFatherName).ThenBy(ev => ev.Person.personLastName).ToList();


                DepartmentSummary = new DepartmentSummary();
                DepartmentSummary.DepartmentID = departmentmodel.departmentID;
                DepartmentSummary.DepartmentName = departmentmodel.departmentName;

                DepartmentSummary.Employees = Employments.Count();
                        
                DepartmentSummary.Salary = Jobs.Sum(j => j.jobPlacementSalary);

                DepartmentSummary.Leaves = _core.getAllLeaveSummary("Dep", DepartmentSummary.DepartmentID);
                DepartmentSummary.Overtime = (decimal)_core.getAllOvertime("Dep", departmentModel.departmentID).Sum(ot => ot.GetOtCost);
                DepartmentSummary.xEmployees = Employments
                        .Where(e => e.employmentStatus == mainStatus.Inactive).Count();

                allowedLeave =(decimal) DepartmentSummary.Leaves.AllowedLeave;
                leaveCost = (decimal)DepartmentSummary.Leaves.leaveCost;
                overtimeCost = (decimal)DepartmentSummary.Overtime;
                DepartmentSummary.Total = (decimal) leaveCost + EmploymentView.Where(ev => ev.Job.jobPlacementStatus == mainStatus.Active).Sum(ev => ev.Job.jobPlacementSalary); ;
                
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
            
            return Page();
        }
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> OnPostApprove(LeaveDecision decision)
        {
            // Check if the decisions list is null or empty
            if (decision == null )
            {
                return new JsonResult(new { success = false, message = "No decisions received." });
            }
            try
            {
                if(decision.actionType == 1)
                {
                    int leaveId = decision.leaveId;
                    int action = decision.actionId;
                    var leave = await _context.Leaves.FindAsync(leaveId);
                    if (leave != null)
                    {
                        leave.modifiedBy = User.Identity.Name!;
                        leave.leaveStatus = action == 1
                            ? Models.leaveStatus.Approved
                            : Models.leaveStatus.Declined;
                    }

                    _context.Attach(leave).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    return new JsonResult(new { success = true });
                }else if (decision.actionType == 2)
                {
                    int leaveId = decision.leaveId;
                    int action = decision.actionId;
                    var OTR = await _context.OvertimeRecords.FindAsync(leaveId);
                    if (OTR != null)
                    {
                        OTR.modifiedBy = User.Identity.Name!;
                        OTR.overtimeRecordStatus = action == 1
                            ? Models.overtimeStatus.Approved
                            : Models.overtimeStatus.Void;
                    }
                    _context.Attach(OTR).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    return new JsonResult(new { success = true });
                }
                
                
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
            return new JsonResult(new { success = false, message = "No Action" });
        }
        // Inside your PageModel class
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostUpdateShiftAsync([FromBody] ShiftUpdateRequest request)
        {
            
  
            var employmentId = request.EmploymentId;
            var shiftId = request.ShiftId;
            var target = request.Target;
            // 1. Return a JSON error for bad input
            if (employmentId <= 0 || shiftId <= 0)
            {
                return new JsonResult(new { success = false, message = "Invalid employment or shift ID." });
            }

            // 2. Return a JSON error if the record is not found
            var employment = await _context.Employments.FindAsync(employmentId);
            if (employment == null)
            {
                return new JsonResult(new { success = false, message = "Employment record not found." });
            }

            if(target == 1)
            {
                // Your database update logic
                var shiftAssignment = new shiftAssignmentModel()
                {
                    modifiedBy = User.Identity?.Name!,
                    modifiedDate = DateTime.Now,
                    employmentID = employmentId,
                    shiftID = shiftId
                };
                _context.ShiftAssignments.Add(shiftAssignment);
                await _context.SaveChangesAsync();

                // 3. Return a JSON success response
                return new JsonResult(new { success = true, message = "Shift updated successfully!" });
            }
            else if(target == 2)
            {
                var siteAssignment = new siteAssignmentModel()
                {
                    modifiedBy = User.Identity?.Name!,
                    modifiedDate = DateTime.Now,
                    employmentID = employmentId,
                    workSiteID = shiftId
                };
                _context.SiteAssignments.Add(siteAssignment);
                await _context.SaveChangesAsync();

                // 3. Return a JSON success response
                return new JsonResult(new { success = true, message = "Worksite updated successfully!" });
            }
            return new JsonResult(new { success = true, message = "No shift or site updated successfully!" });
        }
        public class ShiftUpdateRequest
        {
            public int ShiftId { get; set; }
            public int EmploymentId { get; set; }
            public int Target { get; set; } 
            
        }

    }

    public class LeaveDecision
    {
        public int leaveId { get; set; }
        public int actionId { get; set; }
        public int actionType { get; set; }
        
    }
}
