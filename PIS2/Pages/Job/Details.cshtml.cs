using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;
using PIS2.Views;

namespace PIS2.Pages.Job
{
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

        public DetailsModel(PISContext context)
        {
            _context = context;
        }

        public jobModel jobModel { get; set; } = default!;
        public IList<JobPlacementView> jobPlacementModel { get; set; } = default!;
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobmodel = await _context.Jobs
                .Include(j => j.jobGradeModel)
                .Include(j => j.jobClassModel)
                .Include(j => j.jobCategoryModel).FirstOrDefaultAsync(m => m.jobID == id);
            if (jobmodel == null)
            {
                return NotFound();
            }
            else
            {
                jobModel = jobmodel;
                jobPlacementModel = await _context.JobPlacementView.Where(j => j.jobID == id).ToListAsync();
            }
            return Page();
        }
    }
}
