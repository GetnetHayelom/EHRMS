using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.JopPlacementHistory
{
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;

        public IndexModel(PISContext context)
        {
            _context = context;
        }

        public IList<jobPlacementModel> jobPlacementModel { get;set; } = default!;

        public async Task OnGetAsync()
        {
            jobPlacementModel = await _context.JobPlacements
                .Include(j => j.departmentModel)
                .Include(j => j.employmentModel)
                .Include(j => j.jobModel)
                .ToListAsync();
        }
    }
}
