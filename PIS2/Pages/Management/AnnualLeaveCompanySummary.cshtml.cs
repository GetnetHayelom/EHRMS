using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PIS2.Views;

namespace PIS2.Pages.Management
{
    [Authorize(Roles = "MIE\\PMS_HRMANAGER,MIE\\PMS_MANAGEMENT")]
    public class AnnualLeaveCompanySummaryModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;
        
        public AnnualLeaveCompanySummaryModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }
        public List<AnnuallLeaveSummaryDepartmentView> annualLeaveSummaries { get; set; }
        public string CompanyName { get;set; }
        public void OnGet(int id)
        {
            CompanyName = _context.Companies.FirstOrDefault(c => c.companyID== id).companyName;
            annualLeaveSummaries =_context.AnnualLeaveSummary.Where(als => als.companyID == id)
                .GroupBy(g => g.departmentID)
                .Select(dv => new AnnuallLeaveSummaryDepartmentView
                {
                    DepartmentID = dv.Key,
                    Department = dv.First().departmentName,
                    LeaveBalance = dv.Sum(lb => lb.leaveBalance),
                    AllowedLeave = dv.Sum(lb => lb.adjustedLeaveBalance),
                    PayableLeave = dv.Sum(lb => lb.adjustedLeaveBalanceCost)
                }).OrderByDescending(d => d.PayableLeave).ToList() ?? new List<AnnuallLeaveSummaryDepartmentView>();
        }
    }
}
