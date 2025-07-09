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
    public class DetailsModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DetailsModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public jobPlacementModel jobPlacementModel { get; set; } = default!;
        public List<jobPlacementModel> jobPlacementList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobplacementmodel = await _context.JobPlacements
                .Include(j => j.jobModel).ThenInclude(j => j.jobGradeModel)
                .Include(j => j.jobStepModel)
                .Include(j => j.JobPlacementHistories)
                .Include(j => j.departmentModel)
                .Include(j => j.employmentModel)
                .FirstOrDefaultAsync(m => m.jobPlacementID == id);
            if (jobplacementmodel == null)
            {
                return NotFound();
            }
            else
            {
                
                jobPlacementModel = jobplacementmodel;

                //jobPlacementList = await _context.Where(jp => jp.employmentID == jobPlacementModel.employmentID)
                //    .Include(jp=>jp.jobModel).ThenInclude(j => j.jobGradeModel)
                //    .Include(jp=> jp.jobStepModel)
                //    .Include(jp => jp.departmentModel)
                //    .Include(j => j.employmentModel)
                //    .Include(jp => jp.shiftModel).ToListAsync();
            }
            return Page();
        }
    }
}
