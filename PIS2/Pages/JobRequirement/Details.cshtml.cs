using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.JobRequirement
{
    public class DetailsModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DetailsModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public jobRequirementModel jobRequirementModel { get; set; } = default!;
        public List<jobRequirementHistoryModel> RequestHistory { get; set; } = default!;
        public List<jobReqCost> JobReqCosts { get; set; } = default!;
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobrequirementmodel = await _context.JobRequirements.FirstOrDefaultAsync(m => m.jobRequirementID == id);
            if (jobrequirementmodel == null)
            {
                return NotFound();
            }
            else
            {
                jobRequirementModel = jobrequirementmodel;
            }
            JobReqCosts = new List<jobReqCost>();
            RequestHistory = new List<jobRequirementHistoryModel>();
            var jrHistory = _context.JobRequirementHistories.Where(m => m.jobRequirementID == id).ToList();
            if (jrHistory.Any()) RequestHistory = jrHistory;
            
            var jrCost = _context.JobReqCosts.Where(m => m.jobRequirementID == id).ToList();
            if (jrCost.Any()) JobReqCosts = jrCost;
            return Page();
        }
    }
}
