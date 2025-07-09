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

        public IndexModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public IList<jobPlacementModel> jobPlacementModel { get;set; } = default!;

        public async Task OnGetAsync()
        {
            jobPlacementModel = await _context.JobPlacements
                .Include(j => j.departmentModel)
                .Include(j => j.employmentModel)
                .Include(j => j.jobModel).ThenInclude(j => j.jobGradeModel)
                .Include(j => j.jobStepModel)
                .Include(j => j.shiftModel).ToListAsync();
        }
    }
}
