using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;
using PIS2.Pages.Report;

namespace PIS2.Services
{
    public class SummaryService
    {
        
    }
    public class HRDashboardService
    {
        private readonly PISContext _context;

        public HRDashboardService(PISContext context)
        {
            _context = context;
        }

        public async Task<List<MetricComparison>> GetDashboardMetricsAsync(ReportPeriod period, bool isOfficial = true)
        {
            var (currentStart, currentEnd, prevStart, prevEnd) =isOfficial? GetDateRanges(period): GetManualDateRanges(period);

            return new List<MetricComparison>
            {
                await GetOvertimeMetrics(currentStart, currentEnd, prevStart, prevEnd),
                await GetLeaveMetrics(currentStart, currentEnd, prevStart, prevEnd),
                await GetTerminationMetrics(currentStart, currentEnd, prevStart, prevEnd),
                await GetJobPlacementMetrics(currentStart, currentEnd, prevStart, prevEnd),
                await GetAllowanceMetrics(currentStart, currentEnd, prevStart, prevEnd)
            };
        }

        private (DateTime cs, DateTime ce, DateTime ps, DateTime pe) GetDateRanges(ReportPeriod period)
        {
            DateTime now = DateTime.Now;
            DateTime cs, ce, ps, pe;

            switch (period)
            {
                case ReportPeriod.Quarterly:
                    int currentQuarter = (now.Month - 1) / 3;
                    cs = new DateTime(now.Year, (currentQuarter * 3) + 1, 1);
                    ps = cs.AddMonths(-3);
                    pe = cs.AddTicks(-1);
                    break;
                case ReportPeriod.SemiAnnually:
                    cs = now.Month <= 6 ? new DateTime(now.Year, 1, 1) : new DateTime(now.Year, 7, 1);
                    ps = cs.AddMonths(-6);
                    pe = cs.AddTicks(-1);
                    break;
                case ReportPeriod.Annually:
                    cs = new DateTime(now.Year, 1, 1);
                    ps = cs.AddYears(-1);
                    pe = cs.AddTicks(-1);
                    break;
                default: // Monthly
                    cs = new DateTime(now.Year, now.Month, 1);
                    ps = cs.AddMonths(-1);
                    pe = cs.AddTicks(-1);
                    break;
            }
            ce = now;
            return (cs, ce, ps, pe);
        }

        private (DateTime cs, DateTime ce, DateTime ps, DateTime pe) GetManualDateRanges(ReportPeriod period)
        {
            DateTime now = DateTime.Now;
            DateTime cs, ce, ps, pe;

            switch (period)
            {
                case ReportPeriod.Quarterly:
                    cs = now.AddMonths(-3);
                    ps = cs.AddMonths(-3);
                    pe = cs.AddTicks(-1);
                    break;
                case ReportPeriod.SemiAnnually:
                    cs = now.AddMonths(-6);
                    ps = cs.AddDays(-6);
                    pe = cs.AddTicks(-1);
                    break;
                case ReportPeriod.Annually:
                    cs = now.AddYears(-1);
                    ps = cs.AddYears(-1);
                    pe = cs.AddTicks(-1);
                    break;
                default: // Monthly
                    cs = now.AddMonths(-1);
                    ps = cs.AddMonths(-1);
                    pe = cs.AddTicks(-1);
                    break;
            }
            ce = now;
            return (cs, ce, ps, pe);
        }

        private async Task<MetricComparison> GetOvertimeMetrics(DateTime cs, DateTime ce, DateTime ps, DateTime pe)
        {
            var data = await _context.OvertimeRecords
                .Where(x => x.overtimeRecordDate >= ps && x.overtimeRecordDate <= ce && (x.overtimeRecordStatus== overtimeStatus.Posted || x.overtimeRecordStatus == overtimeStatus.Completed))
                .ToListAsync();

            var current = data.Where(x => x.overtimeRecordDate >= cs);
            var prev = data.Where(x => x.overtimeRecordDate >= ps && x.overtimeRecordDate <= pe);

            return new MetricComparison
            {
                Label = "Overtime",
                CurrentCount = current.GroupBy(o => new { o.overtimeRecordDate, o.employmentID }).Count(),
                PreviousCount = prev.GroupBy(o => new { o.overtimeRecordDate, o.employmentID }).Count(),
                // Using your model's GetOtCost logic
                CurrentSum = current.Sum(x => x.GetOtCost),
                PreviousSum = prev.Sum(x => x.GetOtCost),
                CurrentStart = cs,
                CurrentEnd = ce,
                PreviousStart = ps
            };
        }

        private async Task<MetricComparison> GetLeaveMetrics(DateTime cs, DateTime ce, DateTime ps, DateTime pe)
        {
            var data = await _context.Leaves
                .Include(x => x.leaveTypeModel)
                .Include(x => x.LeaveHistories)
                .Where(x => x.LeaveHistories.FirstOrDefault().modifiedDate >= ps && x.LeaveHistories.FirstOrDefault().modifiedDate <= ce 
                && (x.leaveStatus == leaveStatus.Posted || x.leaveStatus == leaveStatus.Completed)
                && x.leaveTypeModel.leaveJob == false)
                .ToListAsync();

            return new MetricComparison
            {
                Label = "Leaves & Absenteeism",
                CurrentCount = data.Count(x => x.leaveStartDate >= cs),
                PreviousCount = data.Count(x => x.leaveStartDate <= pe),
                CurrentSum = data.Where(x => x.leaveStartDate >= cs).Sum(x => x.leaveCost),
                PreviousSum = data.Where(x => x.leaveStartDate <= pe).Sum(x => x.leaveCost),
                CurrentStart = cs,
                CurrentEnd = ce,
                PreviousStart = ps
            };
        }

        private async Task<MetricComparison> GetTerminationMetrics(DateTime cs, DateTime ce, DateTime ps, DateTime pe)
        {
            var data = await _context.Terminations
                .Where(x => x.terminationDate >= ps && x.terminationDate <= ce)
                .ToListAsync();

            return new MetricComparison
            {
                Label = "Terminations",
                CurrentCount = data.Count(x => x.terminationDate >= cs),
                PreviousCount = data.Count(x => x.terminationDate <= pe),
                // Terminations usually don't have a "Sum", so we use Count as the primary metric
                CurrentSum = data.Count(x => x.terminationDate >= cs),
                PreviousSum = data.Count(x => x.terminationDate <= pe),
                CurrentStart = cs,
                CurrentEnd = ce,
                PreviousStart = ps
            };
        }

        private async Task<MetricComparison> GetJobPlacementMetrics(DateTime cs, DateTime ce, DateTime ps, DateTime pe)
        {
            var data = await _context.JobPlacements
                .Include(x => x.JobPlacementHistories)
                .Where(x => x.jobPlacementDate <= ce)
                .ToListAsync();

            var previousActiveList = data.Where(placement => {
                // Was it even created yet?
                if (placement.jobPlacementDate > pe) return false;

                // Check the most recent history record up to that date
                var stateAtPoint = placement.JobPlacementHistories?
                    .Where(h => h.modifiedDate <= pe)
                    .OrderByDescending(h => h.modifiedDate)
                    .FirstOrDefault();

                if (stateAtPoint != null && stateAtPoint.jobPlacementStatus == mainStatus.Inactive)
                {
                    return false;
                }

                // If no history exists before 'pe', use the current status 
                // (assuming it hasn't changed since creation)
                return placement.jobPlacementStatus == mainStatus.Active;
            }).ToList();


            return new MetricComparison
            {
                Label = "Employee & Salary",
                CurrentCount = data.Count(x => x.jobPlacementStatus == mainStatus.Active),
                PreviousCount = previousActiveList.Count(),
                CurrentSum = data.Where(x => x.jobPlacementStatus == mainStatus.Active).Sum(x => x.jobPlacementSalary),
                PreviousSum = previousActiveList.Sum(x => x.jobPlacementSalary),
                CurrentStart = cs,
                CurrentEnd = ce,
                PreviousStart = ps
            };
        }

        private async Task<MetricComparison> GetAllowanceMetrics(DateTime cs, DateTime ce, DateTime ps, DateTime pe)
        {
            var data = await _context.AllowanceAssignments
                .Include(x => x.AllowanceAssignmentHistories)
                .Where(x => x.allowanceAssignmentDate <= ce)
                .ToListAsync();
            
            var previousActiveList = data.Where(assignment => {
                // Was it even created yet?
                if (assignment.allowanceAssignmentDate > pe) return false;

                // Check the most recent history record up to that date
                var stateAtPoint = assignment.AllowanceAssignmentHistories?
                    .Where(h => h.modifiedDate <= pe)
                    .OrderByDescending(h => h.modifiedDate)
                    .FirstOrDefault();

                if (stateAtPoint != null && stateAtPoint.allowanceAssignmentHistoryStatus == mainStatus.Inactive)
                {
                    return false;
                }

                // If no history exists before 'pe', use the current status 
                // (assuming it hasn't changed since creation)
                return assignment.allowanceStatus == mainStatus.Active;
            }).ToList();

            return new MetricComparison
            {
                Label = "Allowances",
                CurrentCount = data.Count(x => x.allowanceStatus == mainStatus.Active),
                PreviousCount = previousActiveList.Count(),
                CurrentSum = data.Where(x => x.allowanceStatus == mainStatus.Active).Sum(x => x.allowanceAssignmentAmount),
                PreviousSum = previousActiveList.Sum(x => x.allowanceAssignmentAmount),
                CurrentStart = cs,
                CurrentEnd = ce,
                PreviousStart = ps
            };
        }
    }
    public class EntitySummaryViewModel
    {
        public string EntityName { get; set; }
        public int CurrentCount { get; set; }
        public int PreviousCount { get; set; }
        public decimal CurrentSum { get; set; }
        public decimal PreviousSum { get; set; }
        public int? AverageCount { get; set; }
        public decimal? AverageSum { get; set; }
        // Calculated Properties
        public decimal CountChange => CurrentCount - PreviousCount;
        public decimal SumChange => CurrentSum - PreviousSum;
        public bool IsCountUp => CountChange >= 0;
        public bool IsSumUp => SumChange >= 0;

    }

    public enum ReportPeriod { Monthly, Quarterly, SemiAnnually, Annually }

    public class MetricComparison
    {
        public string Label { get; set; }
        public int CurrentCount { get; set; }
        public int PreviousCount { get; set; }
        public decimal CurrentSum { get; set; }
        public decimal PreviousSum { get; set; }
        public DateTime? CurrentStart { get; set; }
        public DateTime? CurrentEnd { get; set; }
        public DateTime? PreviousStart { get; set; }

        // Helpers for the View
        public double CountPercentChange => PreviousCount == 0 ? 0 : (double)(CurrentCount - PreviousCount)* 100 / PreviousCount ;
        public double SumPercentChange => PreviousSum == 0 ? 0 : (double)((CurrentSum - PreviousSum) * 100 / PreviousSum);
    }
}
