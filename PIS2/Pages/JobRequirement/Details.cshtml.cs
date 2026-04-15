using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.JobRequirement
{
    [Authorize(Roles = "MIE\\PMS_HRMANAGEMENT, MIE\\PMS_HRCLERK")]
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

        public DetailsModel(PISContext context)
        {
            _context = context;
        }

        public jobRequirementModel jobRequirementModel { get; set; } = default!;
        public List<jobRequirementHistoryModel> RequestHistory { get; set; } = default!;
        public List<jobReqCost> JobReqCosts { get; set; } = default!;
        public bool IsPublished { get; set; } = false;
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobrequirementmodel = await _context.JobRequirements.Include(j => j.JobModel).Include(j => j.DepartmentModel).ThenInclude(d => d.companyModel).FirstOrDefaultAsync(m => m.jobRequirementID == id);
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

            var isPublished = await _context.Vacancies.AnyAsync(v => v.jobRequirementID == id);
            if (isPublished)
            {
                IsPublished = true;
                var jrCost = _context.JobReqCosts.Include(jr => jr.VacancyModel).Where(m => m.VacancyModel.jobRequirementID == id).ToList();
                if (jrCost.Any()) JobReqCosts = jrCost;
            }

            
            return Page();
        }
    }
}
