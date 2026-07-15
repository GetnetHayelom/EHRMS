using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;

namespace PIS2.Pages.JobStep
{
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;

        public IndexModel(PISContext context)
        {
            _context = context;
        }

        // DATA TO DISPLAY
        public IList<jobStepModel> jobStepModel { get; set; } = new List<jobStepModel>();
        public IList<jobGradeModel> Grades { get; set; } = new List<jobGradeModel>();

        // FILTERS
        [BindProperty(SupportsGet = true)]
        public mainStatus? Status { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? GradeId { get; set; }

        [BindProperty(SupportsGet = true)]
        public double? MinSalary { get; set; }

        [BindProperty(SupportsGet = true)]
        public double? MaxSalary { get; set; }

        // SUMMARY FIELDS
        public int TotalSteps { get; set; }
        public int ActiveSteps { get; set; }
        public double MinStepSalary { get; set; }
        public double MaxStepSalary { get; set; }
        public double AvgStepSalary { get; set; }
        public double MedianStepSalary { get; set; }
        public int TotalGrades { get; set; }

        public async Task OnGetAsync()
        {
            // Load Grades
            Grades = await _context.JobGrades.ToListAsync();

            // Base Query
            var query = _context.JobSteps
                .Include(j => j.jobGradeModel)
                .AsQueryable();

            // Apply Filters
            if (Status != null)
                query = query.Where(x => x.jobStepStatus == Status);

            if (GradeId != null)
                query = query.Where(x => x.jobGradeID == GradeId);

            if (MinSalary != null)
                query = query.Where(x => x.jobStepSalary >= MinSalary);

            if (MaxSalary != null)
                query = query.Where(x => x.jobStepSalary <= MaxSalary);

            jobStepModel = await query
                .OrderBy(x => x.jobGradeID)
                .ThenBy(x => x.jobStepNumber)
                .ToListAsync();

            // Summary Calculations
            TotalSteps = jobStepModel.Count;
            ActiveSteps = jobStepModel.Count(x => x.jobStepStatus == mainStatus.Active);
            MinStepSalary = jobStepModel.Any() ? jobStepModel.Min(x => x.jobStepSalary) : 0;
            MaxStepSalary = jobStepModel.Any() ? jobStepModel.Max(x => x.jobStepSalary) : 0;
            AvgStepSalary = jobStepModel.Any() ? jobStepModel.Average(x => x.jobStepSalary) : 0;

            MedianStepSalary = jobStepModel.Any()
                ? jobStepModel.Select(x => x.jobStepSalary)
                    .OrderBy(s => s)
                    .Skip((jobStepModel.Count - 1) / 2)
                    .Take(2 - jobStepModel.Count % 2)
                    .Average()
                : 0;

            TotalGrades = jobStepModel.Select(x => x.jobGradeID).Distinct().Count();
        }
    }
}
