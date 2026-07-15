using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;

namespace PIS2.Pages.Report
{
    [Authorize(Roles = "MIE\\PMS_HRCLERK, MIE\\PMS_HRMANAGER, MIE\\PMS_MANAGEMENT, MIE\\PMS_FINANCE")]
    public class CostAnalysisReportModel : PageModel
    {
        private readonly PISContext _context;

        public CostAnalysisReportModel(PISContext context)
        {
            _context = context;
        }

        // Report Properties
        public decimal TotalPayrollCost { get; set; }
        public decimal TotalPensionContribution { get; set; }
        public decimal TotalOtherPayments { get; set; }
        public decimal TotalLeaveProductivityLoss { get; set; }
        public decimal GrandTotal => TotalPayrollCost + TotalOtherPayments + TotalLeaveProductivityLoss + TotalPensionContribution;

        public async Task OnGetAsync(DateTime? start, DateTime? end, int? deptId)
        {
            var earlyPayrollDate = await _context.Payrolls.OrderBy(p => p.StartDate).FirstOrDefaultAsync();
            var startDate = start ?? earlyPayrollDate?.StartDate ?? DateTime.Now.AddYears(-1);
            var endDate = end ?? DateTime.Now;

            // 1. Payroll Costs (Approved/Posted)
            var payrolls = await _context.Payrolls
                .Where(p => p.StartDate >= startDate && p.EndDate <= endDate).ToListAsync(); 

            TotalPayrollCost = payrolls.Sum(p => p.totalGross ?? 0);

            TotalPensionContribution = payrolls?.Sum(p => p.totalPensionEmployer) ?? 0;
            // 2. Other Payments (e.g., Medical, Bonuses not in payroll)
            TotalOtherPayments = await _context.Payrolls
                .Where(o => o.modifiedDate >= startDate && o.modifiedDate <= endDate)
                .SumAsync(o => o.totalGross ?? 0);

            // 3. Leave Cost (Productivity Loss)
            // Filtering where leaveJob is false as requested
            TotalLeaveProductivityLoss = await _context.Leaves
                .Include(l => l.leaveTypeModel)
                .Where(l => (l.leaveStartDate >= startDate && l.leaveEndDate <= endDate) && l.leaveTypeModel.leaveJob == false && (l.leaveStatus == Enums.leaveStatus.Posted || l.leaveStatus == Enums.leaveStatus.Completed))
                .SumAsync(l => l.leaveCost);
        }
    }
}
