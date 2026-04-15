using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.Report
{
    public class JobPlacementReportModel : PageModel
    {
        private readonly PISContext _context;

        public JobPlacementReportModel(PISContext context)
        {
            _context = context;
        }

        public IList<jobPlacementModel> jobPlacementModel { get; set; } = default!;
        [BindProperty(SupportsGet = true)]
        public int? CompanyID { get; set; }
        public List<companyModel> Companies { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public int? DepartmentID { get; set; }
        public List<departmentModel> Departments { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public int? jClass { get; set; }
        public List<jobClassModel> JobClasses { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public mainStatus? jStatus { get; set; } = mainStatus.Active;

        [BindProperty(SupportsGet = true)]
        public int? Category { get; set; }
        public List<jobCategoryModel> JobCategories { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public string? Reason { get; set; }
        public List<string> Reasons { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public DateTime? From { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? To { get; set; }
        public int total { get; set; }
        public int active { get; set; }
        public int  transfersThisMonth { get; set; }
        public decimal avgSalary { get; set; }
        public DateTime now { get; set; }
        public DateTime firstOfMonth { get; set; }
        public double p25 { get; set; }
        public double p50 { get; set; }
        public double p75 { get; set; }
        public double p100 { get; set; }
        public async Task OnGetAsync(int? companyID, int? departmentID, int? jstatus, int? jclass, string reason, int? category, string? from, string? to)
        {
            Companies = await _context.Companies.OrderBy(c => c.companyName).ToListAsync();
            JobClasses = await _context.JobClasses.OrderBy(j => j.jobClassName).ToListAsync();
            JobCategories = await _context.JobCategories.OrderBy(j => j.jobCategoryName).ToListAsync();
            Reasons = await _context.JobPlacements.OrderBy(j => j.jobPlacementReason).Select(j => j.jobPlacementReason).Distinct().ToListAsync();
            
            // Base query
            var q = _context.JobPlacements
                .Include(j => j.departmentModel).ThenInclude(d => d.companyModel)
                .Include(j => j.employmentModel)
                .Include(j => j.jobModel)
                .Include(j => j.jobModel).ThenInclude(jt => jt.jobClassModel)
                .Include(j => j.jobStepModel).ThenInclude(jt => jt.jobGradeModel)
                .AsNoTracking()
                .AsQueryable();

            if (companyID.HasValue && companyID.Value > 0)
            {
                q = q.Where(j => j.departmentModel != null && j.departmentModel.companyID == companyID.Value);
                 
            }

            if (departmentID.HasValue && departmentID.Value > 0)
            {
                q = q.Where(j => j.departmentID == departmentID.Value);
            }
            if (jclass.HasValue && jclass.Value > 0)
            {
                q = q.Where(j => j.jobModel.jobClassID == jclass.Value);
            }
            if (category.HasValue && category.Value > 0)
            {
                q = q.Where(j => j.jobModel.jobCategoryID == category.Value);
            }
            if (jstatus.HasValue)
            {
                q = q.Where(j => (int)j.jobPlacementStatus == jstatus.Value);
            }

            if (!string.IsNullOrWhiteSpace(reason))
            {
                q = q.Where(j => j.jobPlacementReason == reason);
            }

            if (!string.IsNullOrWhiteSpace(from) && DateTime.TryParse(from, out var fromDate))
            {
                q = q.Where(j => j.jobPlacementDate >= fromDate.Date);
            }

            if (!string.IsNullOrWhiteSpace(to) && DateTime.TryParse(to, out var toDate))
            {
                // include entire day
                q = q.Where(j => j.jobPlacementDate <= toDate.Date.AddDays(1).AddTicks(-1));
            }

            jobPlacementModel = await q.ToListAsync();
            

            // Summaries
            total = jobPlacementModel.Any() ? jobPlacementModel.Count : 0;
            // assuming mainStatus value '2' = Active (you can adjust)
            active = jobPlacementModel.Any() ? jobPlacementModel.Count(j => j.jobPlacementStatus == mainStatus.Active) : 0;
            From = jobPlacementModel.Any() ? jobPlacementModel.Min(j => j.jobPlacementDate) : DateTime.Now; // or DateTime.Today per your preference
            
            To = jobPlacementModel.Any() ? jobPlacementModel.Max(j => j.jobPlacementDate) : DateTime.Now;
            now = DateTime.UtcNow;
            firstOfMonth = new DateTime(now.Year, now.Month, 1);
            transfersThisMonth = jobPlacementModel.Any() ? jobPlacementModel.Count(j => j.jobPlacementDate >= firstOfMonth) : 0;
            avgSalary =jobPlacementModel.Any() ? Math.Round(jobPlacementModel.Average(j => j.jobPlacementSalary) , 2) : 0m;

            

        }

        double Percentile(double[] sortedData, double p)
        {
            if (p < 0 || p > 100) throw new ArgumentException("p must be between 0 and 100");
            double pos = (p / 100) * (sortedData.Length - 1);
            int lower = (int)Math.Floor(pos);
            int upper = (int)Math.Ceiling(pos);
            if (lower == upper) return sortedData[lower];
            return sortedData[lower] + (sortedData[upper] - sortedData[lower]) * (pos - lower);
        }


    }
}
