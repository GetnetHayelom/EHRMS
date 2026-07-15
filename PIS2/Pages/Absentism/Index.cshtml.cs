using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;
using PIS2.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Absentism
{
    [Authorize(Roles = "MIE\\PMS_HRMANAGER, MIE\\PMS_HRCLERK, MIE\\PMS_MANAGEMENT")]
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;

        public IndexModel(PISContext context)
        {
            _context = context;
        }

        public IList<LeaveReportView> leaveModel { get; set; } = default!;

        public IList<companyModel> Companies { get; set; }
        public IList<leaveTypeModel> LeaveTypes { get; set; }
        [BindProperty(SupportsGet =true)]
        public DateTime StartDate { get; set; } = DateTime.Now.AddMonths(-6);
        [BindProperty(SupportsGet = true)]
        public DateTime EndDate { get; set; } = DateTime.Now;
        [BindProperty(SupportsGet = true)]
        public int? companyID { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? departmentID { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? leaveTypeID { get; set; }

        public async Task OnGetAsync(int? id)
        {
            var absLeaveIDs =await _context.LeaveTypes.Where(l => l.leaveGroup == leaveGroup.Absenteeism).Select(l => l.leaveTypeID).ToListAsync();
            var leaves= _context.LeaveReportView.Where(l => absLeaveIDs.Contains(l.LeaveTypeID ?? 0)).AsQueryable();

            Companies = await _context.Companies.Where(d => d.companyStatus == mainStatus.Active).OrderBy(c => c.companyName).ToListAsync();
            LeaveTypes = await _context.LeaveTypes.Where(l => absLeaveIDs.Contains(l.leaveTypeID)).ToListAsync();

            ViewData["Companies"] = new SelectList(Companies, "companyID", "companyName");
            ViewData["LeaveTypes"] = new SelectList(LeaveTypes, "leaveTypeID", "leaveTypeName");

            if (companyID.HasValue)
            {
                leaves = leaves.Where(l => l.CompanyID == companyID);
            }
            if (departmentID.HasValue)
            {
                leaves = leaves.Where(l => l.DepartmentID == departmentID);
            }
            if (leaveTypeID.HasValue)
            {
                leaves = leaves.Where(l => l.LeaveTypeID == leaveTypeID);
            }

 
            leaveModel = await leaves.Where(l => l.LeaveStart >= StartDate && l.LeaveEnd <= EndDate).ToListAsync();
           
            var newStartDate = StartDate;
            if (EndDate.Year - StartDate.Year <= 1)
            {
                newStartDate = StartDate.AddYears(1);
            }

            MonthlyTrend = leaveModel
                .Where(l => l.LeaveStart >= StartDate && l.LeaveEnd <= EndDate)
                .GroupBy(l => new
                {
                    Month = l.LeaveStart.ToString("yyyy-MM"),
                    l.LeaveType
                })
                .Select(g => new TrendPoint
                {
                    Period = g.Key.Month,
                    LeaveType = g.Key.LeaveType,
                    TotalDays = g.Sum(x => x.LeaveDays ?? 0)
                })
                .OrderBy(x => x.Period)
                .ToList();

            
            YearlyTrend = leaveModel
                .Where(l => l.LeaveStart >= newStartDate && l.LeaveEnd <= EndDate)
                .GroupBy(l => new
                {
                    Year = l.LeaveStart.Year,
                    l.LeaveType
                })
                .Select(g => new TrendPoint
                {
                    Period = g.Key.Year.ToString(),
                    LeaveType = g.Key.LeaveType,
                    TotalDays = g.Sum(x => x.LeaveDays ?? 0)
                })
                .OrderBy(x => x.Period)
                .ToList();

            ViewData["MonthlyTrend"] = System.Text.Json.JsonSerializer.Serialize(MonthlyTrend);
            ViewData["YearlyTrend"] = System.Text.Json.JsonSerializer.Serialize(YearlyTrend);

        }

        public IList<TrendPoint> MonthlyTrend { get; set; }
        public IList<TrendPoint> YearlyTrend { get; set; }

    }
    public class TrendPoint
    {
        public string Period { get; set; }
        public string LeaveType { get; set; }
        public decimal TotalDays { get; set; }
    }

}
