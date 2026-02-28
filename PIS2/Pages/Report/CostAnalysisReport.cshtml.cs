using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace PIS2.Pages.Report
{
    [Authorize(Roles = "MIE\\PMS_HRCLERK, MIE\\PMS_HRMANAGER, MIE\\PMS_MANAGEMENT, MIE\\PMS_FINANCE")]
    public class CostAnalysisReportModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public CostAnalysisReportModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        // Report Properties
        public decimal TotalPayrollCost { get; set; }
        public decimal TotalOtherPayments { get; set; }
        public decimal TotalLeaveProductivityLoss { get; set; }
        public decimal GrandTotal => TotalPayrollCost + TotalOtherPayments + TotalLeaveProductivityLoss;

        public async Task OnGetAsync(DateTime? start, DateTime? end, int? deptId)
        {
            var startDate = start ?? DateTime.Now.AddMonths(-1);
            var endDate = end ?? DateTime.Now;

            // 1. Payroll Costs (Approved/Posted)
            TotalPayrollCost = await _context.Payrolls
                .Where(p => p.StartDate >= startDate && p.EndDate <= endDate)
                .SumAsync(p => p.totalGross ?? 0);

            // 2. Other Payments (e.g., Medical, Bonuses not in payroll)
            TotalOtherPayments = await _context.OtherPayments
                .Where(o => o.modifiedDate >= startDate && o.modifiedDate <= endDate)
                .SumAsync(o => o.GrossPay);

            // 3. Leave Cost (Productivity Loss)
            // Filtering where leaveJob is false as requested
            TotalLeaveProductivityLoss = await _context.Leaves
                .Include(l => l.leaveTypeModel)
                .Where(l => l.leaveStartDate >= startDate && l.leaveEndDate <= endDate)
                .Where(l => l.leaveTypeModel.leaveJob == false && (l.leaveStatus == Models.leaveStatus.Posted || l.leaveStatus == Models.leaveStatus.Completed))
                .SumAsync(l => l.leaveCost);
        }
    }
}
