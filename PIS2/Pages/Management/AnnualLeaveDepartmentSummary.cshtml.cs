using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PIS2.Views;

namespace PIS2.Pages.Management
{
    [Authorize(Roles = "MIE\\PMS_HRMANAGER,MIE\\PMS_MANAGEMENT")]
    public class AnnualLeaveDepartmentSummaryModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public AnnualLeaveDepartmentSummaryModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }
        public List<AnnualLeaveSummary> annualLeaveSummaries { get; set; }
        public string CompanyName { get; set; }
        public string DepartmentName { get; set; }
        public void OnGet(int id)
        {
            DepartmentName = _context.Departments.FirstOrDefault(c => c.departmentID == id).departmentName;
            CompanyName = _context.Companies.FirstOrDefault(c => c.companyID == _context.Departments.FirstOrDefault(c => c.departmentID == id).companyID).companyName;
            annualLeaveSummaries = _context.AnnualLeaveSummary.Where(als => als.departmentID == id)
                .OrderByDescending(d => d.adjustedLeaveBalanceCost).ToList() ?? new List<AnnualLeaveSummary>();
        }
    }
}
