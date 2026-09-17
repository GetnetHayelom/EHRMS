using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models.HR;
using PIS2.Services;
using static System.Formats.Asn1.AsnWriter;
using static PIS2.Pages.Leave.IndexModel;

namespace PIS2.Pages.AllowanceAssignment
{
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;
        private readonly Core _core;
        public IndexModel(PISContext context, Core core)
        {
            _context = context;
            _core = core;
        }
        public class GroupedAllowanceByDep
        {
            public string Name { get; set; } = string.Empty;
            public int Count { get; set; }
            public decimal Sum { get; set; }
            public List<allowanceAssignmentModel> Records { get; set; } = new();
        }
        public List<GroupedAllowanceByDep> GroupedAllowance { get; set; }
        public IList<allowanceAssignmentModel> allowanceAssignmentModel { get;set; } = default!;
        public int allowanceCount { get; set; }
        public decimal allowanceSum { get; set; }
        public string Company { get; set; }
        public async Task OnGetAsync()
        {
            var empID = _core.getUserEmp(User.Identity.Name);

            var company = _context.JobPlacements.Include(j => j.departmentModel)
                .FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active && j.employmentID == empID)?.departmentModel?.companyID ?? 0;
            Company = _context.Companies.FirstOrDefault(c => c.companyID == company)?.companyName ?? "";

            allowanceAssignmentModel = await _context.AllowanceAssignments
                .Include(a => a.allowanceModel)
                .Include(a => a.employmentModel).ThenInclude(e => e.personModel)
                .Include(a => a.employmentModel).ThenInclude(e => e.JobPlacements).ThenInclude(jp => jp.departmentModel)
                .Where(a => a.allowanceStatus == mainStatus.Active
                && a.employmentModel.JobPlacements.FirstOrDefault(js=> js.jobPlacementStatus == mainStatus.Active).departmentModel.companyID == company)
                .OrderBy(a => a.employmentModel.givenID)
                .ToListAsync() ?? new List<allowanceAssignmentModel>();

            GroupedAllowance = allowanceAssignmentModel.GroupBy(l => l.employmentModel.JobPlacements
                .FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active)?.departmentModel)
                .Select(g => new GroupedAllowanceByDep
                {
                    Name = g.Key?.departmentName ?? "Unknown",
                    Count = g.Count(),
                    Sum =g.Sum(a => a.allowanceAssignmentAmount),
                    Records = g.ToList()
                }).ToList();

            allowanceCount = allowanceAssignmentModel.Count();
            allowanceSum = allowanceAssignmentModel.Sum(a => a.allowanceAssignmentAmount);
        }
    }
}
