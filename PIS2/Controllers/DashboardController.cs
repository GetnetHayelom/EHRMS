using Microsoft.AspNetCore.Mvc;
using PIS2.Services;

namespace PIS2.Controllers
{
    [Route("api/dashboard")]
    public class DashboardController : Controller
    {
        private readonly HRDashboardService _dashboardService;

        public DashboardController(HRDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // This handles the AJAX request from the dropdown
        [HttpGet]
        public async Task<IActionResult> GetSummaryPartial(ReportPeriod period = ReportPeriod.Monthly, bool isOfficial = true)
        {
            var model = await _dashboardService.GetDashboardMetricsAsync(period, isOfficial);
            // Returns the partial view with the calculated data
            return PartialView("_HRSummaryPartial", model);
        }
    }
}
