using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using PIS2.Models;
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
        private readonly PIS2.Models.PISContext _context;
        private readonly PIS2.Models.Core _core;

        public IndexModel(PIS2.Models.PISContext context, Models.Core core)
        {
            _context = context;
            _core = core;
        }
        public List<LeaveGroup> GroupedDepLeaves { get; set; } = new();
        public List<LeaveGroup> GroupedTypeLeaves { get; set; } = new();

        public class LeaveGroup
        {
            public string? CompanyName { get; set; }
            public string DepartmentName { get; set; } = string.Empty;
            public int Count { get; set; }
            public decimal SumDays { get; set; }
            public List<leaveModel> Records { get; set; } = new();
        }


        public IList<leaveModel> leaveModel { get;set; } = default!;
       
      
        [BindProperty]
        public decimal totalUnposted {  get; set; }= default!;
        [BindProperty]
        public decimal CountUnposted { get; set; } = default!;

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

            var allowedCompanies = accessibleCompanies
                .Append(company)
                .Where(c => c != null)
                .Distinct()
                .ToList();


            leaveModel = _context.Leaves
                .Include(l => l.employmentModel)?.ThenInclude(e => e.JobPlacements)?.ThenInclude(j => j.departmentModel).ThenInclude(d => d.companyModel)
                .Include(l => l.employmentModel).ThenInclude(e => e.personModel)
                .Include(l => l.leaveTypeModel)
                .Where(l => 
                    l.employmentModel.JobPlacements
                        .Any(j => j.jobPlacementStatus == mainStatus.Active && allowedCompanies.Contains(j.departmentModel.companyID))
                    && l.leaveStatus == leaveStatus.Hold || l.leaveStatus == leaveStatus.Approved)
                .OrderBy(l => l.leaveRequestDate)
                .ToList() ?? new List<leaveModel>();

            GroupedDepLeaves =leaveModel
                .GroupBy(l => l.employmentModel.JobPlacements
                .FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active)?.departmentModel)
                .Select(g => new LeaveGroup
                {
                    CompanyName= g.Key?.companyModel?.companyName ?? "Unknown",
                    DepartmentName = g.Key?.departmentName ?? "Unknown",
                    Count = g.Count(),
                    SumDays = g.Sum(l => (decimal?)l.leaveDays) ?? 0,
                    Records = g.ToList()
                }).ToList();

            GroupedTypeLeaves = leaveModel.GroupBy(l => l.leaveTypeModel)
                .Select(g => new LeaveGroup
                {
                    DepartmentName = g.Key?.leaveTypeName ?? "Unknown",
                    Count = g.Count(),
                    SumDays = (decimal)g.Sum(l => l.leaveDays),
                    Records = g.ToList()
                }).ToList();

            totalUnposted = leaveModel.Sum(l => l.leaveDays);
            CountUnposted = leaveModel.Count();
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

        private void reloadGet()
        {
            var empID = _core.getUserEmp(User.Identity.Name);

            var company = _context.JobPlacements.Include(j => j.departmentModel)
                .FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active)?.departmentModel?.companyID;


            leaveModel = _context.Leaves
                .Include(l => l.employmentModel)?.ThenInclude(e => e.JobPlacements)?.ThenInclude(j => j.departmentModel)
                .Include(l => l.employmentModel).ThenInclude(e => e.personModel)
                .Include(l => l.leaveTypeModel)
                .Where(l => l.employmentModel.JobPlacements.FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active).departmentModel.companyID == company
                && l.leaveStatus == leaveStatus.Hold || l.leaveStatus == leaveStatus.Approved)
                .OrderBy(l => l.leaveRequestDate)
                .ToList() ?? new List<leaveModel>();


            totalUnposted = leaveModel.Sum(l => l.leaveDays);
            CountUnposted = leaveModel.Count();
        }
        
    }
}
