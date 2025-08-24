using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.JobPlacement
{
    public class IndexModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;
        private readonly PIS2.Models.Core _core;
        public IndexModel(PIS2.Models.PISContext context, Models.Core core)
        {
            _context = context;
            _core = core;
        }

        public IList<jobPlacementModel> jobPlacementModel { get;set; } = default!;
        public int jpCount { get; set; }
        public async Task OnGetAsync()
        {
            var empID = _core.getUserEmp(User.Identity.Name);
            
            var company = _context.JobPlacements.Include(j => j.departmentModel)
                .FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active && j.employmentID == empID)?.departmentModel?.companyID;

            jobPlacementModel = await _context.JobPlacements
                .Include(j => j.departmentModel)
                .Include(j => j.employmentModel)
                .Include(j => j.jobModel).ThenInclude(j => j.jobGradeModel)
                .Include(j => j.jobStepModel)
                .Include(j => j.jobModel).ThenInclude(j => j.jobClassModel)
                .Where(j => (j.jobPlacementStatus == mainStatus.Active || j.jobPlacementStatus == mainStatus.Suspended) && j.departmentModel.companyID == company)
                .OrderBy(j => j.employmentModel.givenID)
                .ToListAsync();

            jpCount = jobPlacementModel.Count();
        }
    }
}
