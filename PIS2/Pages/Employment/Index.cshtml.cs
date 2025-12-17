using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace PIS2.Pages.Employment
{
    [Authorize(Roles = "MIE\\PMS_HRMANAGER,MIE\\PMS_HRCLERK,MIE\\PMS_MANAGEMENT")]
    public class IndexModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;
        private readonly PIS2.Models.Core _core;
        public IndexModel(PIS2.Models.PISContext context, Models.Core core)
        {
            _context = context;
            _core = core;
        }
        public List<GroupDepEmployment> GroupedEmployments { get; set; } = new();

        public class GroupDepEmployment
        {
            public string CompanyName { get; set; } = string.Empty;
            public string DepartmentName { get; set; } = string.Empty;
            public int Count { get; set; }
            public List<employmentModel> Records { get; set; } = new();
            public int? Male { get; set; }
            public int? Female { get; set; } 
        }
        public IList<employmentModel> employmentModel { get;set; } = default!;
        
        public int totalCount { get; set; }
        public int maleCount { get; set; }
        public int femaleCount { get; set; }
        public string Company { get; set; }
        public async Task OnGetAsync()
        {
            var userID = _context.Users.FirstOrDefault(u => u.userName == User.Identity.Name)?.userID ?? 0;
            var empID = _core.getUserEmp(User.Identity.Name);
 
            var company = _context.JobPlacements.Include(j => j.departmentModel)
                .FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active && j.employmentID == empID)?.departmentModel?.companyID ?? 0;

            var accessibleCompanies = _context.Accesses
                .Where(a => a.userID == userID && a.accessStatus == mainStatus.Active) // 1 = active access
                .Select(a => a.companyID)
                .ToList();

            var allowedCompanies = accessibleCompanies
                .Append(company)
                .Where(c => c != null)
                .Distinct()
                .ToList();

            Company = _context.Companies.FirstOrDefault(c => c.companyID == company)?.companyName ?? "";

            var employees = _context.JobPlacements.Where(j => j.jobPlacementStatus == mainStatus.Active && allowedCompanies.Contains(j.departmentModel.companyID)).Distinct().Select(e =>e.employmentID);

            
            employmentModel = await _context.Employments
                .Include(e => e.personModel)
                .Include(e => e.employmentTypeModel)
                .Include(e => e.JobPlacements)?.ThenInclude(jp => jp.departmentModel)?.ThenInclude(d => d.companyModel)
                .Include(e => e.JobPlacements)?.ThenInclude(jp => jp.jobModel)
                .Where(e => e.employmentStatus == mainStatus.Active
                && employees.Contains(e.employmentID))
                .OrderBy(e => e.givenID)
                .ToListAsync() ?? new List<employmentModel>();

            totalCount = employmentModel?.Count() ?? 0;
            maleCount = employmentModel?.Count(e => e.personModel?.personGender == Gender.Male) ?? 0;
            femaleCount = employmentModel?.Count(e => e.personModel?.personGender == Gender.Female) ?? 0;

            GroupedEmployments = employmentModel?.GroupBy(e => e.JobPlacements?.FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active).departmentModel)?
              .Select(g => new GroupDepEmployment
              {
                  CompanyName =g.Key?.companyModel?.companyName ?? "",
                  DepartmentName = g.Key?.departmentName ?? "Unknown",
                  Count = g.Count(),
                  Male=g.Count(e => e.personModel?.personGender == Gender.Male),
                  Female = g.Count(e => e.personModel?.personGender == Gender.Female),
                  Records = g.ToList()

              }).ToList() ?? new List<GroupDepEmployment>();
        }
     
        
    }
}
