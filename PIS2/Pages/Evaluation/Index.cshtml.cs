using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;
using PIS2.Views;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using PIS2.Enums;

namespace PIS2.Pages.Evaluation
{
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;

        public IndexModel(PISContext context)
        {
            _context = context;
        }

        
        [BindProperty(SupportsGet = true)]
        public int? department { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? company { get; set; }
        [BindProperty(SupportsGet = true)] public evaluationStatus? Status { get; set; }
        [BindProperty(SupportsGet = true)] public DateTime? StartDate { get; set; }
        [BindProperty(SupportsGet = true)] public DateTime? EndDate { get; set; }

        public IList<EvalGrandView> Evaluations { get; set; } = default!;
        public SummaryMetrics Metrics { get; set; } = new();

        // Dropdowns
        public SelectList CompanyList { get; set; }
        public SelectList DepartmentList { get; set; }
        public async Task OnGetAsync()
        {
            // 1. Populate Dropdowns
            var companies = await _context.Companies.OrderBy(c => c.companyName).ToListAsync();
            CompanyList = new SelectList(companies, "companyID", "companyName");

            var departments = _context.Departments.AsQueryable();
            if (company.HasValue) departments = departments.Where(d => d.companyID == company);
            DepartmentList = new SelectList(await departments.Distinct().OrderBy(d => d.departmentName).ToListAsync(), "departmentID", "departmentName");

            var evals = _context.EvalGrandView
                .OrderByDescending(e => e.endDate) // Show newest first
                .AsQueryable();

            if (company.HasValue)
            {
                evals = evals.Where(e => e.companyID == company);
            }

            if (department.HasValue)
            {
                evals = evals.Where(e => e.departmentID == department);
            }
            if (Status.HasValue) evals = evals.Where(q => q.evaluationStatus == Status);
            if (StartDate.HasValue) evals = evals.Where(q => q.endDate >= StartDate);
            if (EndDate.HasValue) evals = evals.Where(q => q.endDate <= EndDate);

            Evaluations = await evals
                .GroupBy(e => new
                {
                    e.evaluationID,
                    e.evaluationName,
                    e.startDate,
                    e.endDate,
                    e.evaluationStatus,
                    e.employmentID,
                    e.givenID,
                    e.personFullName,
                    e.departmentID,
                    e.departmentName,
                    e.companyID,
                    e.companyName
                })
                .Select(g => new EvalGrandView
                {
                    evaluationID = g.Key.evaluationID,
                    evaluationName = g.Key.evaluationName,
                    startDate = g.Key.startDate,
                    endDate = g.Key.endDate,
                    evaluationStatus = g.Key.evaluationStatus,

                    employmentID = g.Key.employmentID,
                    givenID = g.Key.givenID,
                    personFullName = g.Key.personFullName,

                    departmentID = g.Key.departmentID,
                    departmentName = g.Key.departmentName,
                    companyID = g.Key.companyID,
                    companyName = g.Key.companyName,

                    FinalScore = g.Max(x => x.FinalScore)
                })
                .OrderByDescending(e => e.endDate)
                .ToListAsync();

            if (Evaluations.Any())
            {
                Metrics.TotalCount = Evaluations.Count;
                Metrics.AverageScore = Evaluations.Average(e => e.FinalScore);
                Metrics.PendingCount = Evaluations.Count(e => e.evaluationStatus == evaluationStatus.Pending);
                Metrics.HighPerformers = Evaluations.Count(e => e.FinalScore >= 85);
            }
        }

        
    }
   public class SummaryMetrics
        {
            public int TotalCount { get; set; }
            public decimal AverageScore { get; set; }
            public int PendingCount { get; set; }
            public int HighPerformers { get; set; }
        }
}