using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using PIS2.Views;
using System.Linq;

namespace PIS2.Pages.Report
{
    public class EvaluationReportModel : PageModel
    {
        private readonly PISContext _context;
        public EvaluationReportModel(PISContext context) => _context = context;

        // Using your new DTO structure
        public List<EvalSingleEmployeeReport> EmployeeReports { get; set; }
        public MetricTotals Metrics { get; set; }
        public List<TrendPoint> MonthlyTrend { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? FilterCompanyID { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? FilterDepartmentID { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FilterStart { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FilterEnd { get; set; }

        // Lists for the dropdowns
        public List<companyModel> Companies { get; set; }
        public List<departmentModel> Departments { get; set; }
        public List<CompanyGroupDto> GroupedReports { get; set; }
        public async Task OnGetAsync()
        {
            // 1. Fetch flat data from the SQL View
            // Assuming 1 is the status for 'Submitted' based on your previous snippet
            var flatData = await _context.EvaluationSummaryView
                .Where(v => v.evaluationStatus == 1)
                .ToListAsync();

            // 2. Aggregate Global Metrics (Average of all valuations in the view)
            Metrics = new MetricTotals
            {
                AvgTime = flatData.Any() ? flatData.Average(v => v.timeValuation) : 0,
                AvgResource = flatData.Any() ? flatData.Average(v => v.resourceValuation) : 0,
                AvgPerformance = flatData.Any() ? flatData.Average(v => v.performanceValuation) : 0
            };

            
            // 4. Map Flat View to Hierarchical DTOs
            EmployeeReports = flatData
                .GroupBy(v => v.evaluationID)
                .Select(eg => new EvalSingleEmployeeReport
                {
                    evaluationID = eg.Key,
                    evaluationName = eg.First().evaluationName,
                    personFullName = eg.First().personFullName,
                    startDate = eg.First().evaluationStartDate,
                    endDate = eg.First().evaluationEndDate,
                    Types = eg.GroupBy(t => t.evaluationTypeID).Select(tg => new EvalTypeSummary
                    {
                        typeName = tg.First().evaluationTypeName,
                        typeWeight = tg.First().evaluationTypeWeight,
                        Tasks = tg.GroupBy(tk => tk.evaluationTaskID).Select(tkg => new EvalTaskSummary
                        {
                            taskName = tkg.First().evaluationTaskName,
                            taskWeight = tkg.First().evaluationTaskWeight,
                            avgTime = tkg.Average(m => m.timeValuation),
                            avgResource = tkg.Average(m => m.resourceValuation),
                            avgPerformance = tkg.Average(m => m.performanceValuation),
                            SubTasks = tkg.Select(st => new EvalSubTaskSummary
                            {
                                subtaskName = st.evaluationSubTaskName,
                                subtaskWeight = st.evaluationSubTaskWeight,
                                subTime = st.timeValuation,
                                subResource = st.resourceValuation,
                                subPerformance = st.performanceValuation
                            }).ToList()
                        }).ToList()
                    }).ToList()
                }).ToList();

            // Calculate Grand Total for each report
            foreach (var report in EmployeeReports)
            {
                report.FinalGrandTotal = report.Types.Sum(t => t.typeContribution);
            }

            // 3. Generate MonthlyTrend from the Calculated EmployeeReports
            if (EmployeeReports.Any())
            {
                var minDate = EmployeeReports.Min(r => r.endDate);
                var maxDate = EmployeeReports.Max(r => r.endDate);
                var totalDays = (maxDate - minDate).TotalDays;

                // Dynamic Grouping Selector
                Func<DateTime, string> periodSelector = d => d.ToString("MMM yyyy"); // Default Monthly
                if (totalDays > 730) periodSelector = d => d.Year.ToString();
                else if (totalDays > 365) periodSelector = d => $"{d.Year} Q{(d.Month - 1) / 3 + 1}";

                MonthlyTrend = EmployeeReports
                    .GroupBy(r => periodSelector(r.endDate))
                    .Select(g => new TrendPoint
                    {
                        Label = g.Key,
                        // Average of the three raw metrics across all reports in this period
                        AvgTime = (double)g.Average(r => r.Types.SelectMany(t => t.Tasks).Average(tk => tk.avgTime)),
                        AvgResource = (double)g.Average(r => r.Types.SelectMany(t => t.Tasks).Average(tk => tk.avgResource)),
                        AvgPerf = (double)g.Average(r => r.Types.SelectMany(t => t.Tasks).Average(tk => tk.avgPerformance)),
                        // The Weighted Score (FinalGrandTotal)
                        AvgOverall = (double)g.Average(r => r.FinalGrandTotal)
                    })
                    .OrderBy(x => x.Label) // Note: Complex date sorting might be needed for strings
                    .ToList();
            }

            // This maps the IDs from the view to actual names from your related tables
            var placementHierarchy = await _context.JobPlacements
                .Include(jp => jp.departmentModel).ThenInclude(d => d.companyModel)
                .ToDictionaryAsync(jp => jp.jobPlacementID);

           
        }

        public class MetricTotals { public decimal AvgTime; public decimal AvgResource; public decimal AvgPerformance; }
        public class TrendPoint
        {
            public string Label { get; set; }
            public double AvgTime { get; set; }
            public double AvgResource { get; set; }
            public double AvgPerf { get; set; }
            public double AvgOverall { get; set; }
        }
        // Support Classes for Grouping
        public class CompanyGroupDto
        {
            public string CompanyName { get; set; }
            public List<DepartmentGroupDto> Departments { get; set; }
        }

        public class DepartmentGroupDto
        {
            public string DepartmentName { get; set; }
            public List<EvalSingleEmployeeReport> Reports { get; set; }
        }
    }
}