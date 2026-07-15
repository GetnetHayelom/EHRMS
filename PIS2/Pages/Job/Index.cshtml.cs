using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using PIS2.Data;
using PIS2.Enums;

namespace PIS2.Pages.Job
{
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;

        public IndexModel(PISContext context)
        {
            _context = context;
        }

        public IList<jobModel> jobModel { get;set; } = default!;
        public int AssignedJobs { get; set; }

        public async Task OnGetAsync()
        {
            ViewData["jobCategoryID"] = new SelectList(_context.JobCategories, "jobCategoryID", "jobCategoryName");
            ViewData["jobClassID"] = new SelectList(_context.JobClasses, "jobClassId", "jobClassName");
            ViewData["jobGradeID"] = new SelectList(_context.JobGrades, "jobGradeID", "jobGradeName");

            jobModel = await _context.Jobs
                .Include(j => j.jobCategoryModel)
                .Include(j => j.jobClassModel)
                .Include(j => j.jobGradeModel).ToListAsync();
            var assigned = await _context.JobPlacements.Where(j => j.jobPlacementStatus == mainStatus.Active).Select(j => j.jobID).ToListAsync();
            AssignedJobs = jobModel.Where(j => assigned.Contains(j.jobID)).Distinct().Count();
        }
        public async Task<IActionResult> OnGetFilterAsync(
            int? jobGrade,
            int? jobCategory,
            int? jobClass,
            int? jobStatus)
        {
            var query = _context.Jobs
                .Include(j => j.jobGradeModel)
                .Include(j => j.jobCategoryModel)
                .Include(j => j.jobClassModel)
                .AsQueryable();

            if (jobGrade.HasValue)
            { query = query.Where(j => j.jobGradeID == jobGrade); }

            if (jobCategory.HasValue)
            { query = query.Where(j => j.jobCategoryID == jobCategory); }

            if (jobClass.HasValue)
            { query = query.Where(j => j.jobClassID == jobClass); }

            if (jobStatus.HasValue && jobStatus != null)
            { query = query.Where(j => j.jobStatus == (mainStatus) jobStatus); }

            var filteredJobs = await query.ToListAsync();

            return Partial("_JobTableRows", filteredJobs);
        }

    }
}


