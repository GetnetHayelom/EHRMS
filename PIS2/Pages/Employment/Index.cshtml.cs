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
            var empID = _core.getUserEmp(User.Identity.Name);
            Console.WriteLine("*Your empID iS____________" + empID);
            var company = _context.JobPlacements.Include(j => j.departmentModel)
                .FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active && j.employmentID == empID)?.departmentModel?.companyID;
            Company = _context.Companies.FirstOrDefault(c => c.companyID == company).companyName;
            //Console.WriteLine("*Your Comapny iS____________" + _context.Companies.FirstOrDefault(c => c.companyID == company).companyName);
            employmentModel = await _context.Employments
                .Include(e => e.personModel)
                .Include(e => e.employmentTypeModel)
                .Include(e => e.JobPlacements).ThenInclude(jp => jp.departmentModel)
                .Include(e => e.JobPlacements).ThenInclude(jp => jp.jobModel)
                .Where(e => e.employmentStatus == mainStatus.Active
                && e.JobPlacements.First(j => j.jobPlacementStatus == mainStatus.Active || j.jobPlacementStatus == mainStatus.Suspended).departmentModel.companyID == company)
                .OrderBy(e => e.givenID)
                .ToListAsync();
            totalCount = employmentModel.Count();
            maleCount = employmentModel.Count(e => e.personModel?.personGender == Gender.Male);
            femaleCount = employmentModel.Count(e => e.personModel?.personGender == Gender.Female);

            GroupedEmployments = employmentModel.GroupBy(e => e.JobPlacements.First(j => j.jobPlacementStatus == mainStatus.Active).departmentModel)
              .Select(g => new GroupDepEmployment
              {
                  DepartmentName = g.Key?.departmentName ?? "Unknown",
                  Count = g.Count(),
                  Male=g.Count(e => e.personModel?.personGender == Gender.Male),
                  Female = g.Count(e => e.personModel?.personGender == Gender.Female),
                  Records = g.ToList()

              }).ToList() ?? new List<GroupDepEmployment>();
        }
     
        
    }
}
