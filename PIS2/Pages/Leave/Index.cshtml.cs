using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;
using PIS2.Pages.Employment;
using PIS2.Pages.Report;
using PIS2.Services;
using PIS2.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static PIS2.Pages.OvertimeRecord.IndexModel;

namespace PIS2.Pages.Leave
{
    [Authorize(Roles = "MIE\\PMS_HRCLERK, MIE\\PMS_HRMANAGER")]
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;
        private readonly Core _core;
        private readonly LeaveService _leave;
     
        public IndexModel(PISContext context, Core core, LeaveService leave)
        {
            _context = context;
            _core = core;
            _leave = leave;
        }
        public List<LeaveGroup> GroupedDepLeaves { get; set; } = new();
        public List<LeaveGroup> GroupedTypeLeaves { get; set; } = new();            
        public List<ExpiringLeaveDto> ExpiringLeaves{ get; set; }
        public IList<LeaveReportView> leaveModel { get;set; } = default!;

        [BindProperty]
        public decimal totalUnposted {  get; set; }= default!;
        [BindProperty]
        public decimal CountUnposted { get; set; } = default!;
        [BindProperty(SupportsGet = true)]
        public EmploymentPositions Position { get; set; }
        [BindProperty(SupportsGet = true)]
        public int Years { get; set; } = 2;
        public IList<companyModel> Companies { get; set; } = default!;
        public async Task OnGetAsync()
        {
            var userID = _context.Users.FirstOrDefault(u => u.userName == User.Identity.Name)?.userID ?? 0;

            var empID = _core.getUserEmp(User.Identity.Name);
           
            var company = _context.JobPlacements.Include(j => j.departmentModel)
                .FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active && j.employmentID == empID)?.departmentModel?.companyID;

            var accessibleCompanies = _context.Accesses
                .Where(a => a.userID == userID && a.accessStatus == mainStatus.Active) // 1 = active access
                .Select(a => a.companyID)
                .ToList();

            Companies = _context.Companies.Where(c => accessibleCompanies.Contains(c.companyID)).ToList();
            var allowedCompanies = accessibleCompanies
                .Append(company)
                .Where(c => c != null)
                .Distinct()
                .ToList();

            var myEmps = await _context.EmployeeDetailViews
                .Where(e => e.EmploymentStatus == mainStatus.Active && allowedCompanies.Contains(e.CompanyID)).ToListAsync();

            var activeEmps = myEmps.Select(e => e.EmploymentID).ToList();
            
            var leaves = _context.LeaveReportView.Where(e => activeEmps.Contains(e.EmploymentID)).AsQueryable();
            leaves = leaves.Where(l => allowedCompanies.Contains(l.CompanyID)
                    && (l.LeaveStatus == leaveStatus.Hold || l.LeaveStatus == leaveStatus.Approved)); 
            
            leaveModel = leaves
                .OrderBy(l => l.LeaveRequestDate)
                .ToList() ?? new List<LeaveReportView>();

            GroupedDepLeaves = leaveModel
                .GroupBy(l => l.DepartmentID)                
                .Select(g => new LeaveGroup
                {
                    CompanyName= g.FirstOrDefault()?.CompanyName ?? "Unknown",
                    DepartmentName = g.FirstOrDefault()?.DepartmentName ?? "Unknown",
                    Count = g.Count(),
                    SumDays = g.Sum(l => (decimal?)l.LeaveDays) ?? 0,
                    Records = g.ToList()
                }).ToList();

            GroupedTypeLeaves = leaveModel.GroupBy(l => l.LeaveTypeID)
                .Select(g => new LeaveGroup
                {
                    DepartmentName = g.FirstOrDefault()?.LeaveType ?? "Unknown",
                    Count = g.Count(),
                    SumDays = (decimal)g.Sum(l => l.LeaveDays),
                    Records = g.ToList()
                }).ToList();

            totalUnposted = leaveModel?.Sum(l => l.LeaveDays) ?? 0;
            CountUnposted = leaveModel.Count();

            if(Position != null) { myEmps = myEmps.Where(e => e.EmploymentPosition == Position).ToList(); }
            var exps = await _leave.GetAllExpiringLeaves(myEmps, Years);
            ExpiringLeaves =exps.Where(e => e.days >0).ToList(); 
        }
        // Post handler
        [BindProperty]
        public int leaveId { get; set; }
        //[HttpPost]
        public async Task<IActionResult> OnPostApprove(int leaveId)
        {
            var leave = await _context.Leaves.FindAsync(leaveId);
            if (leave == null)
                return new JsonResult(new { success = false, message = "Leave not found." });

            if (!User.IsInRole("MIE\\PMS_HRCLERK"))
                return new JsonResult(new { success = false, message = "Access denied." });

            leave.leaveStatus = leaveStatus.Posted;
            leave.modifiedBy = User.Identity.Name;

            _context.Update(leave);
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true, message = "Leave posted successfully." });
        }

        //Expiring Leaves Filter
        //Expiring Leaves Filter
        public async Task<IActionResult> OnGetExpiringLeave(Filter_Ex_Leaves_DTO filter)
        {
            var emps = _context.EmployeeDetailViews.AsQueryable();
            if (filter.company.HasValue && filter.company.Value > 0)
            {
                emps = emps.Where(e => e.CompanyID == filter.company);
            }
            if (filter.department.HasValue && filter.department.Value > 0)
            {
                emps = emps.Where(e => e.DepartmentID == filter.department);
            }
            if (filter.position.HasValue && filter.position.Value > 0)
            {
                emps = emps.Where(e => e.EmploymentPosition == (EmploymentPositions)filter.position);
            }
            var filtered = emps.ToList();
            var exps = await _leave.GetAllExpiringLeaves(filtered, filter.years ?? 2);
            exps = exps.Where(e => e.days > 0).ToList();
            return Partial("_ExpiringLeaves", exps);
        }
    }
}
