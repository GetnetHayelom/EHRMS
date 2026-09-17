using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;
using PIS2.Models.Organization;
using PIS2.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PIS2.Pages.Termination
{
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;
        private readonly ILogger<IndexModel> _logger;
        public IndexModel(PISContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        } 

        // ------------------------------
        // Filter properties (bound)
        // ------------------------------
        [BindProperty(SupportsGet = true)] public int? CompanyID { get; set; }
        [BindProperty(SupportsGet = true)] public int? DepartmentID { get; set; }
        [BindProperty(SupportsGet = true)] public terminationStatus? Status { get; set; }
        [BindProperty(SupportsGet = true)] public string? Reason { get; set; }
        [BindProperty(SupportsGet = true)] public int? jClass { get; set; }
        [BindProperty(SupportsGet = true)] public int? Category { get; set; }
        [BindProperty(SupportsGet = true)] public DateTime? From { get; set; }
        [BindProperty(SupportsGet = true)] public DateTime? To { get; set; } = DateTime.Now;

        public List<companyModel> Companies { get; set; } = new();
        public SelectList Departments { get; set; }
        // The filtered list
        public IList<TerminationDetailView> terminationModel { get; set; } = new List<TerminationDetailView>();
        public List<jobClassModel> JobClasses { get; set; } = new();
        public List<jobCategoryModel> JobCategories { get; set; } = new();
        public List<string> Reasons { get; set; } = new();
        // Summaries
        public int total { get; set; }
        public int male { get; set; }
        public int female { get; set; }
        public int TotalTerminations { get; set; }
        public int TerminationsThisYear { get; set; }
        public double AvgTenureYears { get; set; }

        public string MonthlyLabels { get; set; }
        public string MonthlyData { get; set; }

        public string DeptLabels { get; set; }
        public string DeptData { get; set; }

        public string ReasonLabels { get; set; }
        public string ReasonData { get; set; }

        public string Rate { get; set; }
        public async Task OnGetAsync()
        {
            try
            {
                Companies = await _context.Companies.OrderBy(c => c.companyName).ToListAsync();
                JobClasses = await _context.JobClasses.OrderBy(j => j.jobClassName).ToListAsync();
                JobCategories = await _context.JobCategories.OrderBy(j => j.jobCategoryName).ToListAsync();
                Reasons = await _context.Terminations.OrderBy(t => t.terminationReason).Select(t => t.terminationReason).Distinct().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Termination Report: Error loading filter options.");
                throw;
            }
            // ------------------------------------
            // Base Query with all navigation
            // ------------------------------------
            var q = _context.TerminationDetailView.AsQueryable();
            try
            {
                TotalTerminations = await q.CountAsync();

                TerminationsThisYear = await q
                    .CountAsync(x => x.terminationDate <= DateTime.Now && x.terminationDate >= DateTime.Now.AddYears(-1));

                    var avgDays = await q
                        .Select(x => EF.Functions.DateDiffDay(x.employmentDate, x.terminationDate))
                        .AverageAsync();

                AvgTenureYears = avgDays / 365;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Termination Report: Error loading Terminations for filtering.");
                throw;
            }
            // ------------------------------------
            // FILTERS
            // ------------------------------------
            try
            {
                if (CompanyID.HasValue && CompanyID > 0)
                {
                    q = q.Where(t => t.companyID == CompanyID);
                    var deps = await _context.Departments.Where(d => d.companyID == CompanyID).OrderBy(c => c.departmentName).Select(c => new { Name = c.departmentName, ID = c.departmentID }).ToListAsync();
                    Departments = new SelectList(deps, "ID", "Name", DepartmentID);
                }
                

            if (DepartmentID.HasValue && DepartmentID > 0)
                q = q.Where(t =>t.departmentID == DepartmentID);

            if (Status.HasValue)
                q = q.Where(t => t.terminationStatus ==(int) Status);

            if (!string.IsNullOrWhiteSpace(Reason))
                q = q.Where(t => t.terminationReason == Reason);

            if (jClass.HasValue)
                q = q.Where(t => t.jobClassId >= jClass.Value);

            if (Category.HasValue)
                q = q.Where(t => t.jobCategoryID >= Category.Value);

            if (From.HasValue)
            q = q.Where(t => t.terminationDate >= From.Value);

            if (To.HasValue)
                q = q.Where(t => t.terminationDate <= To.Value);

                _logger.LogInformation("Termination Report Filters: Company={Company}, Dept={Dept}, Status={Status}, Reason={Reason}, From={From}, To={To}",
        CompanyID, DepartmentID, Status, Reason, From, To);
                // ------------------------------------
                // EXECUTE
                // ------------------------------------
                terminationModel = await q.OrderByDescending(t => t.terminationDate)
                .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Termination Report: Error filtering Terminations.");
                throw;
            }
            // ------------------------------------
            // SUMMARIES (Safe)
            // ------------------------------------
            try { 
            total = terminationModel.Any() ? terminationModel.Count :0;

            male = terminationModel.Any() ? terminationModel
                .Count(t => t.personGender == (int) Gender.Male) : 0;

            female = terminationModel.Any() ? terminationModel
                .Count(t => t.personGender == (int) Gender.Female) : 0;

            

            // Monthly trend
            var monthly = terminationModel
                .GroupBy(x => new { x.terminationDate.Year, x.terminationDate.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new
                {
                    Label = g.Key.Year + "-" + g.Key.Month,
                    Count = g.Count()
                });

            MonthlyLabels = JsonSerializer.Serialize(monthly.Select(x => x.Label));
            MonthlyData = JsonSerializer.Serialize(monthly.Select(x => x.Count));

            // Department stats
            var dept = terminationModel
                .GroupBy(x => x.departmentName)
                .Select(g => new
                {
                    Dept = g.Key,
                    Count = g.Count()
                });

            DeptLabels = JsonSerializer.Serialize(dept.Select(x => x.Dept));
            DeptData = JsonSerializer.Serialize(dept.Select(x => x.Count));

            // Reason stats
            var reasons = terminationModel
                .GroupBy(x => x.terminationReason)
                .Select(g => new
                {
                    Reason = g.Key,
                    Count = g.Count()
                });

            ReasonLabels = JsonSerializer.Serialize(reasons.Select(x => x.Reason));
            ReasonData = JsonSerializer.Serialize(reasons.Select(x => x.Count));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Termination Report: Error summarizing filtered Terminations.");
                throw;
            }
            try
            {
                var emps = await _context.Employments.CountAsync();
                Rate = emps > 0 ? ((total * 100.0) / emps).ToString("0.0") + "%" : "0%";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Termination Report: Error gettin Termination Rate.");
                throw;
            }
        }
    }
}
