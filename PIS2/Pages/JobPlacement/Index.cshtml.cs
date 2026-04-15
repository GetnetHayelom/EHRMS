using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;
using PIS2.Services;

namespace PIS2.Pages.JobPlacement
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

        public IList<jobPlacementModel> jobPlacementModel { get;set; } = default!;
        public int jpCount { get; set; }
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

            jobPlacementModel = await _context.JobPlacements
                .Include(j => j.departmentModel)
                .Include(j => j.employmentModel)
                .Include(j => j.jobModel)
                .Include(j => j.jobStepModel).ThenInclude(j => j.jobGradeModel)
                .Include(j => j.jobModel).ThenInclude(j => j.jobClassModel)
                .Where(j => (j.jobPlacementStatus == mainStatus.Active || j.jobPlacementStatus == mainStatus.Suspended) && j.departmentModel.companyID == company)
                .OrderBy(j => j.employmentModel.givenID)
                .ToListAsync();

            jpCount = jobPlacementModel.Count();
        }
    }
}
