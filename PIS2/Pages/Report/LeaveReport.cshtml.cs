using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using PIS2.Data;
using PIS2.Models;
using PIS2.Pages.Management;
using PIS2.Services;
using PIS2.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PIS2.Pages.Report
{
    public class LeaveReportModel : PageModel
    {
        private readonly PISContext _context;
        private readonly Core _core;

        public LeaveReportModel(PISContext context, Core core)
        {
            _context = context;
            _core = core;
        }

        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }
        public DateTime? SDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }
        public DateTime? EDate { get; set; } 
        [BindProperty(SupportsGet = true)]
        public int? CompanyID { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? LeaveType { get; set; }
        public string LType { get; set; }
        //[BindProperty(SupportsGet = true)]
        public DateTime? CustomStartDate { get; set; }
        public DateTime? CustomSDate { get; set; }
        public string companyName { get; set; } = "All Companies";
        public string departmentName { get; set; } = "All Departments";
        public IList<LeaveReportView>? leaveModel { get;set; } = default!;
        public IList<LeaveReportCompany> Leaves { get; set; } = default!;
        public IList<departmentModel> Departments { get; set; } = default!;
        public IList<employmentTypeModel> EmploymentTypes { get; set; } = default!;
        public IList<workSiteModel> WorkLocations { get; set; } = default!;
        public IList<companyModel> Companies { get; set; } = default!;
        public IList<leaveTypeModel> LeaveTypes { get; set; } = default!;
        
        public int totalCount { get; set; }
        public int filteredCount { get; set; }
        [BindProperty]
        public decimal totalDays {  get; set; }= default!;
        public decimal totalCost { get; set; }
        public decimal recordsPerEmployee { get; set; } = default!;
        public decimal daysPerEmployee { get; set; } = default!;
        [BindProperty]
        public int CountUnposted { get; set; } = default!;
       
        public List<LeaveBalanceDepartmentView> AnnualLeaveSummaries { get; set; }
        public async Task OnGetAsync()
        {
            //Custom Report 
            Departments = await _context.Departments.Where(d => d.departmentStatus == mainStatus.Active).OrderBy(d => d.departmentName).ToListAsync() ?? new List<departmentModel>();
            LeaveTypes = await _context.LeaveTypes.ToListAsync() ?? new List<leaveTypeModel>();
            Companies = await _context.Companies.Where(c =>c.companyStatus == mainStatus.Active).OrderBy(c => c.companyName).ToListAsync() ?? new List<companyModel>();

            var allLeaves = _context.LeaveReportView.AsQueryable();

            leaveModel = new List<LeaveReportView>();
            leaveModel = allLeaves.ToList() ?? new List<LeaveReportView>();

            totalDays = leaveModel?.Sum(l => l.LeaveDays) ?? 0;
            CountUnposted = leaveModel?.Count(l => l.LeaveStatus == leaveStatus.Hold || l.LeaveStatus == leaveStatus.Approved) ?? 0;
            totalCount = leaveModel?.Count() ?? 0;
            totalCost = leaveModel?.Sum(l => l.LeaveValue) ?? 0;
            filteredCount = leaveModel?.GroupBy(l => l.EmploymentID).Count() ?? 0;
            recordsPerEmployee =filteredCount > 0? totalCount / filteredCount : 0;
            daysPerEmployee = filteredCount > 0 ? totalDays / filteredCount : 0;

            //Start Date and end Date
            if (leaveModel != null && leaveModel.Any())
            {
                SDate = leaveModel.Min(l => l.LeaveStart);
                EDate = leaveModel.Max(l => l.LeaveEnd);
            }
            
            //
            //Leave Absentism Summary
            //
            var leaves = _context.LeaveReportView.AsQueryable();

            if (CompanyID.HasValue && CompanyID.Value > 0) { 
                companyName = _context.Companies.FirstOrDefault(c => c.companyID == CompanyID.Value).companyName;
                leaves = leaves.Where(r => r.CompanyID == CompanyID.Value);
            }
            if (StartDate.HasValue) { leaves = leaves.Where(r => r.LeaveStart >= StartDate.Value);
            SDate = StartDate;}
                
            if (EndDate.HasValue) {
                leaves = leaves.Where(r => r.LeaveEnd <= EndDate.Value);
            
                EDate = EndDate;
            }
            if (LeaveType.HasValue)
            {
                leaves = leaves.Where(r => r.LeaveTypeID == LeaveType.Value);
                LType = _context.LeaveTypes.FirstOrDefault(l => l.leaveTypeID == LeaveType.Value).leaveTypeName;
            }

            Leaves = new List<LeaveReportCompany>();

            var filteredLeave = leaves.ToList();

            //Payable Annual Leave Summary
            AnnualLeaveSummaries = new List<LeaveBalanceDepartmentView>();
            AnnualLeaveSummaries = await _context.LeaveBalanceDepartmentView
                .OrderByDescending(d => d.PayableLeave).ToListAsync();
        }

        // Post handler
        public IActionResult OnGetFilter(int? department,int? leaveGroup, int? leaveStatus, int? leaveType, int? company, DateTime? dateStart, DateTime? dateEnd, string? empEp)
        {
            var leaveModel = _context.LeaveReportView.AsQueryable();
            
            // Filter by company (if provided)
            if (company.HasValue && company != null)
            {
                leaveModel = leaveModel.Where(e => e.CompanyID == company);

            }

            // Filter by Department (if provided)
            if (department.HasValue && department != null)
            {
                leaveModel = leaveModel.Where(e => e.DepartmentID == department);

            }

            // Filter by Status (if provided)
            if (leaveStatus.HasValue)
            {
                leaveModel = leaveModel.Where(e => e.LeaveStatus == (leaveStatus)leaveStatus);

            }

            // Filter by group (if provided)
            if (leaveGroup.HasValue)
            {
                leaveModel = leaveModel.Where(e => e.LeaveGroup == (leaveGroup)leaveGroup);

            }


            // Filter by type (if provided)
            if (leaveType.HasValue && leaveType != null)
            {

                leaveModel = leaveModel.Where(e => e.LeaveTypeID == leaveType);
            }


            // Filter by Start TIme (if provided)
            if (dateStart.HasValue && dateStart != null)
            {
                leaveModel = leaveModel
                    .Where(e => e.LeaveStart >= dateStart);
            }
            // Filter by end TIme (if provided)
            if (dateEnd.HasValue && dateEnd != null)
            {
                leaveModel = leaveModel
                    .Where(e => e.LeaveEnd <= dateEnd);
            }

            // Filter by Employee Ep
            if (!String.IsNullOrEmpty(empEp))
            {
                leaveModel = leaveModel
                    .Where(e => e.GivenID == empEp);
            }


            // Execute the query and get the filtered results
            var filteredLeaves = leaveModel.OrderBy(e => e.LeaveRequestDate).ToList();
            filteredCount = filteredLeaves.Count;
            totalDays = filteredLeaves?.Sum(l => l.LeaveDays) ?? 0;
            CountUnposted = filteredLeaves.Count(l => l.LeaveStatus == Models.leaveStatus.Hold);
            totalCount = filteredLeaves?.Count() ?? 0;
            totalCost = filteredLeaves?.Sum(l => l.LeaveValue) ?? 0;
            filteredCount = filteredLeaves?.GroupBy(l => l.EmploymentID).Count() ?? 0;
            recordsPerEmployee = filteredCount > 0? totalCount / filteredCount :0;
            daysPerEmployee = filteredCount > 0 ? totalDays / filteredCount : 0;


            // DETERMINE THE FLEXIBLE GROUPING
            double totalDaysDiff = (dateEnd ?? DateTime.Now).Subtract(dateStart ?? DateTime.Now.AddYears(-1)).TotalDays;

            var flexibleSummary = filteredLeaves
                .GroupBy(l => {
                    if (totalDaysDiff <= 31) // Less than a month: Group by Day
                        return new { Sort1 = l.LeaveStart.Year, Sort2 = l.LeaveStart.DayOfYear, Label = l.LeaveStart.ToString("MMM dd") };

                    if (totalDaysDiff <= 365) // Up to a year: Group by Month
                        return new { Sort1 = l.LeaveStart.Year, Sort2 = l.LeaveStart.Month, Label = l.LeaveStart.ToString("MMM yyyy") };

                    if (totalDaysDiff <= 730) // Up to 2 years: Group by Quarter
                    {
                        int quarter = (int)Math.Ceiling(l.LeaveStart.Month / 3.0);
                        return new { Sort1 = l.LeaveStart.Year, Sort2 = quarter, Label = $"Q{quarter} {l.LeaveStart.Year}" };
                    }

                    // Over 2 years: Group by Year
                    return new { Sort1 = l.LeaveStart.Year, Sort2 = 0, Label = l.LeaveStart.Year.ToString() };
                })
                .OrderBy(g => g.Key.Sort1)
                .ThenBy(g => g.Key.Sort2)
                .Select(g => new {
                    Period = g.Key.Label,
                    Count = g.Count(),
                    Days = g.Sum(x => x.LeaveDays)
                }).ToList();

            // 2. Group by Leave Type for the Pie Chart
            var typeSummary = filteredLeaves
                .GroupBy(l => l.LeaveType)
                .Select(g => new {
                    TypeName = g.Key,
                    Count = g.Count()
                }).ToList();


            var grouped= filteredLeaves
                .GroupBy(e => e.CompanyID)
                .Select(companyGroup => new
                {
                    CompanyId = companyGroup.Key,
                    CompanyName = companyGroup.First().CompanyName, // assuming CompanyName exists
                    Departments = companyGroup
                        .GroupBy(e => e.DepartmentID)
                        .Select(deptGroup => new
                        {
                            DepartmentId = deptGroup.Key,
                            DepartmentName = deptGroup.First().DepartmentName, // assuming DepartmentName exists
                            Leaves = deptGroup.ToList()
                        }).ToList()
                }).ToList();

            var tableHtml = new StringBuilder();

            foreach (var com in grouped)
            {
                // Company header row
                tableHtml.Append(
                    $"<tr class='table-primary'>" +
                    $"<td colspan='8'><strong>{com.CompanyName}(Total: {com.Departments.Sum(d => d.Leaves.Count)})</strong></td>" +
                    $"<td><strong>{Math.Round(com.Departments.SelectMany(d => d.Leaves).Sum(l => l.LeaveValue),2).ToString("N2")} Br</strong> </td></tr>"
                );

                foreach (var dept in com.Departments)
                {
                    // Department header row
                    tableHtml.Append(
                        $"<tr class='table-secondary fw-bold bg-secondary'>" +
                        $"<td colspan='8' style='padding-left:20px;'><em>{dept.DepartmentName} (Total: {dept.Leaves.Count})</em></td>" +
                        $"<td style='padding-left:20px;'><em>{Math.Round(dept.Leaves.Sum(l => l.LeaveValue),2).ToString("N2")} Br</em> </td></tr>"
                    );

                    // Leaves rows
                    foreach (var e in dept.Leaves)
                    {
                        var url = Url.Page("/Leave/Details", new { id = e.LeaveId });
                        tableHtml.Append(
                            $"<tr onclick=\"location.href='{url}'\" style=\"cursor:pointer;\">" +
                            $"<td>{e.LeaveId}</td>" +
                            $"<td>{e.GivenID}</td>" +
                            $"<td>{e.LeaveRequestDate.ToShortDateString()}</td>" +
                            $"<td>{e.LeaveStart.ToShortDateString()}</td>" +
                            $"<td>{e.LeaveEnd.ToShortDateString()}</td>" +
                            $"<td>{e.LeaveDays}</td>" +
                            $"<td>{e.LeaveType}</td>" +
                            $"<td>{e.LeaveStatus}</td>" +
                            $"<td>{Math.Round(e.LeaveValue,2).ToString("N2")} Br</td>" +
                            $"</tr>"
                        );
                    }
                }
            }
            return new JsonResult(new { tableHtml=tableHtml.ToString(), filteredCount, totalDays, CountUnposted, totalCount, recordsPerEmployee, daysPerEmployee, totalCost, flexibleSummary, typeSummary});


        }

        [BindProperty]
        public int LeaveTypeID { get; set; }
        [BindProperty]
        public leaveGroup LeaveGroup { get; set; }
        [BindProperty]
        public bool? Legality { get; set; }
  
        public IActionResult OnGetAbsenteesm(int? CompanyID, int? leaveType, DateTime? StartDate, DateTime? EndDate, int? leaveGroup, bool? Legality, string? EmpEp)
        {
            var leaves = _context.LeaveReportView.Where(l => l.LeaveJob == false).AsQueryable();

            if (!leaves.Any())
            {
                return new JsonResult(false);
            }
            if (CompanyID.HasValue && CompanyID.Value > 0)
            {
                companyName = _context.Companies.FirstOrDefault(c => c.companyID == CompanyID).companyName ;
                leaves = leaves.Where(r => r.CompanyID == CompanyID);
            }
            if (StartDate.HasValue)
            {
                leaves = leaves.Where(r => r.LeaveStart >= StartDate.Value);
                SDate = StartDate;
            }

            if (EndDate.HasValue)
            {
                leaves = leaves.Where(r => r.LeaveEnd <= EndDate.Value);

                EDate = EndDate;
            }

            if (leaveType.HasValue && leaveType.Value > 0)
            {
                leaves = leaves.Where(r => r.LeaveTypeID == leaveType);
            }
            if (leaveGroup.HasValue && leaveGroup.Value > 0)
            {
                leaves = leaves.Where(r => r.LeaveGroup == (leaveGroup) leaveGroup);
            }
            if (Legality.HasValue)
            {
                leaves = leaves.Where(r => r.LeaveLegality == (bool) Legality);
            }
            
            // Filter by Employee Ep
            if (!String.IsNullOrEmpty(EmpEp))
            {
                leaves = leaves
                    .Where(e => e.GivenID == EmpEp);
            }

            var filteredLeave = leaves.ToList();

            // If no data, return early
            if (!filteredLeave.Any())
            {
                return new JsonResult(new
                {
                    message = "No records found for the selected filters.",
                    tableHtml = string.Empty
                });
            }

            
            var workingDays = _core.GetWorkingDays(filteredLeave.Min(g => g.LeaveStart), filteredLeave.Max(g => g.LeaveEnd));


            Leaves = filteredLeave.GroupBy(r => r.CompanyID)
                .Select(g => new LeaveReportCompany
                {
                    CompanyID = g.Key ?? 0,
                    CompanyName = g.FirstOrDefault()?.CompanyName ?? "",
                    EmployeeTotal = g.Select(c => c.EmploymentID).Distinct().Count(),
                    WorkingDays = workingDays,
                    CompanyTotal = g.Count(),
                    CompanySum = g.Sum(c => c.LeaveDays),
                    CompanyCost =g.Sum(c => c.LeaveValue),
                    StartDate = g.Min(l => l.LeaveStart),
                    EndDate = g.Max(l => l.LeaveEnd),
                    Departments = g.GroupBy(r => r.DepartmentID)
                    .Select(dg => new LeaveReportDepartment
                    {
                        DepartmentName = dg.FirstOrDefault()?.DepartmentName ?? "",
                        DepartmentID = dg.Key ?? 0,
                        EmployeeTotal = dg.GroupBy(e => e.EmploymentID).Count(),
                        DepartmentTotal = dg.Count(),
                        DepartmentSum = dg.Sum(r => r.LeaveDays),
                        DepartmentCost = dg.Sum(r => r.LeaveValue),
                        LeaveTypes = dg.GroupBy(r => r.LeaveType)
                        .Select(lg => new LeaveReportType
                        {
                            LeaveType = lg.Key,
                            LeaveTypeCount = lg.Count(),
                            LeaveTypeSum = lg.Sum(lg => lg.LeaveDays),
                            LeaveTypeCost = lg.Sum(lg => lg.LeaveValue),
                        }).OrderByDescending(lg => lg.LeaveTypeCount).Take(3).ToList()
                    }).OrderByDescending(dg => dg.DepartmentSum).ToList()
                }).OrderByDescending(g => g.CompanySum).ToList();

            

            var tableHtml = new StringBuilder();

            foreach (var comp in Leaves)
            {
                // Company header row
                tableHtml.Append($@"
                    <tr class='table-primary' id='{comp.CompanyID}'>
                        <td colspan='3'><h5 class='fw-bold'>{comp.CompanyName}</h5></td>
                        <td><h5 class='fw-bold'>{comp.EmployeeTotal}</h5></td>
                        <td><h5 class='fw-bold'>{Math.Round((decimal)comp.CompanySum, 2):N2}</h5></td>
                        <td><h5 class='fw-bold'>{(comp.EmployeeTotal * comp.WorkingDays > 0 ? Math.Round((decimal)(comp.CompanySum / (comp.EmployeeTotal * comp.WorkingDays) * 100), 2) : 0)}%</h5></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td colspan='3' class='text-secondary fw-bold'>Top 3 Absentism Reasons By Department</td>
                    </tr>
                ");

                // Department rows
                foreach (var dept in comp.Departments)
                {
                    tableHtml.Append($@"
                        <tr class='table-secondary'>
                            <td></td>
                            <td colspan='2' class='fw-bold'>{dept.DepartmentName}</td>
                            <td class='fw-bold'>{dept.EmployeeTotal}</td>
                            <td class='fw-bold'>{Math.Round((decimal)dept.DepartmentSum, 2):N2}</td>
                            <td class='fw-bold'>
                                {(dept.EmployeeTotal * comp.WorkingDays > 0 ? Math.Round((decimal)(dept.DepartmentSum / (dept.EmployeeTotal * comp.WorkingDays) * 100), 2) : 0)}%
                            </td>
                        </tr>
                    ");

                    // Leave type breakdown under department
                    foreach (var type in dept.LeaveTypes)
                    {
                        var typePercent = dept.DepartmentSum > 0 ? Math.Round((decimal)(type.LeaveTypeSum / dept.DepartmentSum) * 100, 2) : 0;
                        tableHtml.Append($@"
                <tr>
                    <td colspan='2'></td>
                    <td colspan='2'>{type.LeaveType}</td>
                    <td>{Math.Round((decimal)type.LeaveTypeSum, 2):N2}
                        <u>({typePercent:N2}%)</u>
                    </td>
                    <td></td>
                </tr>
            ");
                    }
                }
            }

            //return new JsonResult(new { tableHtml = tableHtml.ToString() });

            var employees = filteredLeave?.Select(l => l.EmploymentID)?.Distinct().Count() ?? 0;
            var totalDays = filteredLeave?.Sum(l => l.LeaveDays) ?? 0;
            var perEmployee = filteredLeave.Any()? Math.Round((filteredLeave?.Sum(l => l.LeaveDays)/filteredLeave?.Select(l => l.EmploymentID).Distinct().Count()) ?? 0 ,2):0;
            var countAbsenteesm = filteredLeave?.Count() ?? 0;
            var absenteesmRate = filteredLeave.Any() ? Math.Round((filteredLeave.Sum(l => l.LeaveDays)/(workingDays * filteredLeave.Select(l => l.EmploymentID).Distinct().Count())) ?? 0, 2) * 100 :0;
            var absenteesmCost = filteredLeave.Any() ? Math.Round(filteredLeave.Sum(l => l.LeaveValue),2) : 0;

            //GRAPH DATA
            // Calculate the time span in days
            double totalDaysSpan = (EndDate.Value - StartDate.Value).TotalDays;
            IEnumerable<object> trendData;

            if (totalDaysSpan <= 90)
            {
                // Under 3 months: Group by Month (or you could even do Week)
                trendData = filteredLeave
                    .GroupBy(l => new { l.LeaveStart.Year, l.LeaveStart.Month })
                    .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                    .Select(g => new { label = $"{g.Key.Month}/{g.Key.Year}", value = g.Sum(l => l.LeaveDays) });
            }
            else if (totalDaysSpan <= 730)
            {
                // Between 3 months and 2 years: Group by Quarter
                trendData = filteredLeave
                    .GroupBy(l => new { l.LeaveStart.Year, Quarter = (l.LeaveStart.Month - 1) / 3 + 1 })
                    .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Quarter)
                    .Select(g => new { label = $"Q{g.Key.Quarter} {g.Key.Year}", value = g.Sum(l => l.LeaveDays) });
            }
            else
            {
                // Over 2 years: Group by Year
                trendData = filteredLeave
                    .GroupBy(l => l.LeaveStart.Year)
                    .OrderBy(g => g.Key)
                    .Select(g => new { label = g.Key.ToString(), value = g.Sum(l => l.LeaveDays) });
            }

            // Add this inside public IActionResult OnGetAbsentism before the final return
            var trendList = trendData?.ToList()?? new List<object>();

            var deptData = Leaves?.SelectMany(c => c.Departments)?
                .Select(d => new { label = d.DepartmentName, value = d.DepartmentSum })?
                .OrderByDescending(x => x.value)
                .ToList();

            var typeData = filteredLeave?.GroupBy(l => l.LeaveType)
                .Select(g => new { label = g.Key, value = g.Sum(l => l.LeaveDays) })
                .ToList();


            return new JsonResult(new { tableHtml = tableHtml.ToString(), employees, totalDays, perEmployee, countAbsenteesm, absenteesmRate, absenteesmCost, trendData = trendList, deptData, typeData});
        }

    }
    public class LeaveReportCompany
    {
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }       
        public int EmployeeTotal { get; set; }
        public decimal WorkingDays { get; set; }
        public int CompanyTotal { get; set; }
        public decimal? CompanySum { get; set; }
        public decimal? CompanyCost { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<LeaveReportDepartment>? Departments { get; set; }
    }

    public class LeaveReportDepartment
    {
        public string DepartmentName { get; set; }
        public int DepartmentID { get; set; }
        public int DepartmentTotal { get; set; }
        public int EmployeeTotal { get; set; }
        public decimal? DepartmentSum { get; set; }
        public decimal? DepartmentCost { get; set; }
        public List<LeaveReportType>? LeaveTypes { get; set; }
    }

    public class LeaveReportType
    {
        public string LeaveType { get; set; }
        public int LeaveTypeCount { get; set; }
        public decimal? LeaveTypeSum { get; set; }
        public decimal? LeaveTypeCost { get; set; }
    }


}
