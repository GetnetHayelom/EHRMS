using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models.HR;

namespace PIS2.Pages.Report
{
    [Authorize(Roles = "HRCLERK, HRMANAGER, MANAGEMENT")]
    public class PenaltyReport : PageModel
    {
        private readonly PISContext _context;

        public PenaltyReport(PISContext context)
        {
            _context = context;
        }

        public IList<penaltyModel> PenaltyRecords { get; set; } = default!;

        // Filter Properties
        [BindProperty(SupportsGet = true)]
        public string? SearchEmpId { get; set; }

        [BindProperty(SupportsGet = true)]
        public penaltyStatus? StatusFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? TypeFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public penaltyCategory? CategoryFilter { get; set; }

        public SelectList PenaltyTypes { get; set; }

        public int TotalPenalties { get; set; }
        public int PendingActions { get; set; }
        public decimal TotalPenaltyValue { get; set; }
        public string TopReason { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }
        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }

        //TREANDING
        public List<string> TrendLabels { get; set; } = new();
        public List<int> TrendData { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            PenaltyTypes = new SelectList(await _context.PenaltyTypes.ToListAsync(), "penaltyTypeID", "penaltyName");

            var query = _context.Penalties
                .Include(p => p.employmentModel)
                .Include(p => p.penaltyTypeModel)
                .Include(p => p.departmentModel).ThenInclude(d => d.companyModel)
                .AsQueryable();

            if (query == null || !query.Any()) { return Page(); }
            // Apply Filters
            if (!string.IsNullOrEmpty(SearchEmpId))
            {
                query = query.Where(s => s.employmentModel.givenID.Contains(SearchEmpId));
            }

            if (StatusFilter.HasValue)
            {
                query = query.Where(s => s.penaltyStatus == StatusFilter);
            }
            if (TypeFilter.HasValue)
            {
                query = query.Where(s => s.penaltyTypeID == TypeFilter);
            }
            if (CategoryFilter.HasValue)
            {
                query = query.Where(s => s.penaltyTypeModel.penaltyCategory == CategoryFilter);
            }
            if (StartDate.HasValue)
            {
                query = query.Where(s => s.penaltyIssueDate >= StartDate);
            }
            else
            {
                StartDate = query?.Min(q => q.penaltyIssueDate) ?? DateTime.MinValue;
            }
            if (EndDate.HasValue)
            {
                query = query?.Where(s => s.penaltyIssueDate <= EndDate);
            }
            else
            {
                EndDate = query?.Max(q => q.penaltyIssueDate) ?? DateTime.Now;
            }

                var penalties = query?.ToList();
            TotalPenalties = penalties?.Count ?? 0;
            PendingActions = penalties?.Count(p => p.penaltyStatus == penaltyStatus.Pending || p.penaltyStatus == penaltyStatus.Hold) ?? 0;

            // Determine if we use Monthly or Quarterly bucketing
            bool useQuarterly = StartDate.HasValue
                 && EndDate.HasValue
                 && (EndDate.Value - StartDate.Value).TotalDays > 365;

            var trendGroup = penalties?
                .GroupBy(p => useQuarterly
                    ? $"{Math.Ceiling(p.penaltyIssueDate.Month / 3.0)}Qt {p.penaltyIssueDate.Year}"
                    : p.penaltyIssueDate.ToString("MMM yyyy"))
                .OrderBy(g => g.First().penaltyIssueDate);

            TrendLabels = trendGroup?.Select(g => g.Key).ToList() ?? new List<string>();
            TrendData = trendGroup?.Select(g => g.Count()).ToList() ?? new List<int>();

            // Calculate sum of amounts (handling potential nulls)
            TotalPenaltyValue = await query.SumAsync(p => p.penaltyTypeModel != null ? p.penaltyTypeModel.penaltyAmount : 0);

            TopReason = (await query.GroupBy(p => p.penaltyReason)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefaultAsync()) ?? "N/A";

            PenaltyRecords = await query.OrderByDescending(p => p.penaltyIssueDate).ToListAsync();
            return Page();
        }
    }
}