using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PIS2.Pages.Job
{
    public class IndexModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public IndexModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public IList<jobModel> jobModel { get;set; } = default!;

        public async Task OnGetAsync()
        {
            ViewData["jobCategoryID"] = new SelectList(_context.JobCategories, "jobCategoryID", "jobCategoryName");
            ViewData["jobClassID"] = new SelectList(_context.JobClasses, "JobClassId", "JobClassName");
            ViewData["jobGradeID"] = new SelectList(_context.JobGrades, "jobGradeID", "jobGradeName");

            jobModel = await _context.Jobs
                .Include(j => j.jobCategoryModel)
                .Include(j => j.jobClassModel)
                .Include(j => j.jobGradeModel).ToListAsync();
        }
        public async Task<IActionResult> OnGetFilterAsync(
            int? jobGrade,
            int? jobCategory,
            int? jobClass,
            string jobStatus)
        {
            var query = _context.Jobs
                .Include(j => j.jobGradeModel)
                .Include(j => j.jobCategoryModel)
                .Include(j => j.jobClassModel)
                .AsQueryable();

            if (jobGrade.HasValue)
                query = query.Where(j => j.jobGradeID == jobGrade);

            if (jobCategory.HasValue)
                query = query.Where(j => j.jobCategoryID == jobCategory);

            if (jobClass.HasValue)
                query = query.Where(j => j.jobClassID == jobClass);

            if (!string.IsNullOrEmpty(jobStatus))
                query = query.Where(j => j.jobStatus.ToString() == jobStatus);

            var filteredJobs = await query.ToListAsync();

            return Partial("_JobTableRows", filteredJobs);
        }

    }
}


