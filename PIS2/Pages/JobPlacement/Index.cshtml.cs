using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Services;
using PIS2.Views;

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

        public IList<JobPlacementView> jobPlacementModel { get;set; } = default!;
        public int jpCount { get; set; }
        public async Task OnGetAsync()
        {
            var userID = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name)?.Id ?? 0;
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

            jobPlacementModel = await _context.JobPlacementView.Where(j => j.jobPlacementStatus == mainStatus.Active)
                .OrderBy(j => j.givenID)
                .ToListAsync();

            jpCount = jobPlacementModel.Count();
        }
    }
}
