using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using PIS2.Models;
using PIS2.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PIS2.Pages.Report
{
    public class OvertimeReceordReportModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public OvertimeReceordReportModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public DateTime SDate { get; set; } 
        [BindProperty(SupportsGet = true)]
        public DateTime EDate { get; set; } 
        [BindProperty(SupportsGet = true)]
        public int OtType { get; set; }
        [BindProperty(SupportsGet = true)]
        public overtimeStatus OtStatus { get; set; }
        [BindProperty(SupportsGet = true)]
        public int Company { get; set; }

        [TempData]
        public string? OtDetailsJson { get; set; }
        public IList<OvertimeDetailView> OvertimeDetails { get; set; } = default!;        
        public IList<OvertimeSummaryView> OvertimeSummary { get;set; } = default!;
        public int TotalRecords { get; set; }
        public decimal TotalHours { get; set; }
        public decimal TotalDays { get; set; } 
        public decimal TotalCost { get; set; }
        public int InvolvedEmployees { get; set; }
        public decimal DaysPerEmployees { get; set; }
        public decimal CostPerEmployees { get; set; }
        //
        //
        /// <summary>
        /// Summary Report Summaries
        /// </summary>
        public int TotalRecords1 { get; set; }
        public decimal TotalHours1 { get; set; }
        public decimal TotalDays1 { get; set; }
        public decimal TotalCost1 { get; set; }
        public int InvolvedEmployees1 { get; set; }
        public decimal DaysPerEmployees1 { get; set; }
        public decimal CostPerEmployees1 { get; set; }
        //=======================================================================
        public List<companyModel> Companies { get; set; }
        public List<departmentModel> Departments { get; set; }
        public List<overtimeModel> OvertimeTypes { get; set; }

        public List<OvertimeSummaryVM> MonthlySummary { get; set; }
        public List<OvertimeSummaryVM> YearlySummary { get; set; }

        public async Task OnGetAsync()
        {

            var otDetails = _context.OvertimeDetailView.AsQueryable();
            var otDetails1 = _context.OvertimeDetailView.AsQueryable();

            if (Company > 0)
                otDetails = otDetails.Where(ot => ot.CompanyID == Company);

            if (SDate != default)
                otDetails = otDetails.Where(ot => ot.OvertimeDate >= SDate);

            if (EDate != default)
                otDetails = otDetails.Where(ot => ot.OvertimeDate <= EDate);

            if (OtType > 0)
                otDetails = otDetails.Where(ot => ot.OvertimeID == OtType);

            if (OtStatus != 0) // assuming 0 = default enum value (All)
                otDetails = otDetails.Where(ot => ot.OvertimeStatus == OtStatus);



            OvertimeDetails = new List<OvertimeDetailView>();

            OvertimeDetails = otDetails.ToList();
            SDate = OvertimeDetails.Min(o => o.OvertimeDate).Value;
            EDate = OvertimeDetails.Max(o => o.OvertimeDate).Value;

            var ots = OvertimeDetails
            .GroupBy(v => new
            {
                v.CompanyID,
                v.CompanyName,
                v.DepartmentID,
                v.DepartmentName,
                v.OvertimeID,
                v.OvertimeName
            })
            .Select(g => new OvertimeSummaryView
            {
                CompanyID =g.Key.CompanyID,
                CompanyName = g.Key.CompanyName,
                DepartmentID = g.Key.DepartmentID,
                DepartmentName = g.Key.DepartmentName,
                OvertimeID = g.Key.OvertimeID,
                OvertimeName = g.Key.OvertimeName,

                EmployeesInvolved = g.Select(x => x.EmploymentID).Distinct().Count(),

                TotalHours = g.Sum(x => x.TimeElapsed),
                TotalDays = g.Sum(x => x.TimeElapsed ?? 0) / 24.0m,
                TotalCost = g.Sum(x => x.TimeElapsed * x.OvertimeRate* x.EmployeeRate),
            
                HoursPerEmployee = g.Sum(x => x.TimeElapsed) / (g.Select(x => x.EmploymentID).Distinct().Count() == 0 ? 1 : g.Select(x => x.EmploymentID).Distinct().Count()),
                DaysPerEmployee = (g.Sum(x => x.TimeElapsed) / 24.0m) / (g.Select(x => x.EmploymentID).Distinct().Count() == 0 ? 1 : g.Select(x => x.EmploymentID).Distinct().Count()),
                CostPerEmployee = g.Sum(x => x.TimeElapsed * x.OvertimeRate* x.EmployeeRate) / (g.Select(x => x.EmploymentID).Distinct().Count() == 0 ? 1 : g.Select(x => x.EmploymentID).Distinct().Count()),
             })
            .ToList();


            OvertimeSummary = ots.ToList();
            TotalRecords1 = otDetails.Count();
            TotalHours1 = otDetails.Sum(os => os.TimeElapsed??0);
            TotalDays1 = TotalHours1/24m ;
            TotalCost1 = otDetails.Sum(os => os.OvertimeCost) ?? 0;
            InvolvedEmployees1 = otDetails.Select(e => e.EmploymentID).Distinct().Count();
            DaysPerEmployees1 = TotalDays1 / InvolvedEmployees1;
            CostPerEmployees1 = TotalCost1 / InvolvedEmployees1;

            TotalRecords = otDetails1.Count();
            TotalHours = otDetails1.Sum(os => os.TimeElapsed ?? 0);
            TotalDays = TotalHours / 24m;
            TotalCost = otDetails1.Sum(os => os.OvertimeCost) ?? 0;
            InvolvedEmployees = otDetails1.Select(e => e.EmploymentID).Distinct().Count();
            DaysPerEmployees = TotalDays / InvolvedEmployees;
            CostPerEmployees = TotalCost / InvolvedEmployees;

            Companies = _context.Companies.OrderBy(c => c.companyName).ToList();
            Departments = _context.Departments.ToList();
            OvertimeTypes = _context.Overtimes.ToList();

            var filteredOvertime = otDetails1.ToList();

            MonthlySummary = filteredOvertime.Where(o => o.OvertimeStatus == Models.overtimeStatus.Completed)
                .GroupBy(o => o.OvertimeDate.Value.Year)
                .Select(g => new OvertimeSummaryVM
                {
                    Period = g.Key.ToString(),
                    OvertimeCount = g.Select(o => new { o.GivenID, o.OvertimeDate }).Count(),
                    TotalCost = g.Sum(go => go.OvertimeCost) ?? 0,
                    EmployeeCount = g.Select(go => go.GivenID).Distinct().Count()
                })
                .OrderBy(x => x.Period)
                .ToList();


        }

        //================================================================================
        /// <summary>
        /// Summary Report Filter
        /// </summary>
        /// <param name="department"></param>
        /// <param name="overtimeStatus"></param>
        /// <param name="overtimeType"></param>
        /// <param name="company"></param>
        /// <param name="dateStart"></param>
        /// <param name="dateEnd"></param>
        /// <param name="empID"></param>
        /// <returns></returns>
        /// 

       
        public IActionResult OnGetFilter(int? department, int? overtimeStatus, int? overtimeType, int? company, DateTime? dateStart, DateTime? dateEnd, string? empID)
        {


            // Start with the full list of employees
            var otDetails = _context.OvertimeDetailView.AsQueryable();

            // Apply filters based on the provided query parameters

            // Filter by company (if provided)
            if (company.HasValue && company != null)
            {
                otDetails = otDetails.Where(e => e.CompanyID == company);

            }

            // Filter by Department (if provided)
            if (department.HasValue && department != null)
            {
                otDetails = otDetails.Where(e => e.DepartmentID == department);

            }

            // Filter by Status (if provided)
            if (overtimeStatus.HasValue)
            {
                otDetails = otDetails.Where(e => e.OvertimeStatus == (Models.overtimeStatus) overtimeStatus);

            }

            // Filter by type (if provided)
            if (overtimeType.HasValue && overtimeType != null)
            {

                otDetails = otDetails.Where(e => e.OvertimeID == overtimeType);
            }


            // Filter by Start TIme (if provided)
            if (dateStart.HasValue && dateStart != null)
            {
                otDetails = otDetails
                    .Where(e => e.OvertimeDate >= dateStart);
            }
            // Filter by end TIme (if provided)
            if (dateEnd.HasValue && dateEnd != null)
            {
                otDetails = otDetails
                    .Where(e => e.OvertimeDate <= dateEnd);
            }
             
           
            // Execute the query and get the filtered results
            var filteredOvertime = otDetails.ToList();

            // store in TempData (as JSON string)
            //OtDetailsJson = JsonSerializer.Serialize(otDetails);

            TotalRecords = otDetails.Count();
            TotalHours = otDetails.Sum(os => os.TimeElapsed ?? 0);
            TotalDays = (decimal)TotalHours / 24m;
            TotalCost = (decimal) otDetails.Sum(os => os.OvertimeCost);
            InvolvedEmployees = otDetails.Select(e => e.EmploymentID).Distinct().Count();
            DaysPerEmployees =InvolvedEmployees>0? TotalDays / InvolvedEmployees : 0;
            CostPerEmployees = InvolvedEmployees > 0 ? TotalCost / InvolvedEmployees :0;


            Console.WriteLine("============================================================================================");
            Console.WriteLine("============================================================================================");
            Console.WriteLine(filteredOvertime.Count());
            Console.WriteLine("============================================================================================");
            Console.WriteLine("============================================================================================");


            var grouped = filteredOvertime
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
                            Overtimes = deptGroup.ToList()
                        }).ToList()
                }).ToList();

            MonthlySummary = filteredOvertime.Where(o => o.OvertimeStatus == Models.overtimeStatus.Completed)
                .GroupBy(o => new { o.OvertimeDate.Value.Year, o.OvertimeDate.Value.Month })
                .Select(g => new OvertimeSummaryVM
                {
                    Period = g.Key.Year + "-" + g.Key.Month.ToString("D2"),
                    OvertimeCount = g.Select(o => new { o.GivenID, o.OvertimeDate }).Count(),
                    TotalCost = g.Sum(o => o.OvertimeCost) ?? 0,
                    EmployeeCount = g.Select(o => o.GivenID).Distinct().Count()
                })
                .OrderBy(x => x.Period)
                .ToList();

            var tableHtml = new StringBuilder();

            foreach (var com in grouped.OrderBy(c => c.CompanyName))
            {
                // Company header row
                tableHtml.Append(
                    $"<tr class='table-primary'><td colspan='7'><strong>{com.CompanyName}</strong> " +
                    $"(Total: {com.Departments.Sum(d => d.Overtimes.Count)})</td></tr>"
                );

                foreach (var dept in com.Departments.OrderBy(d => d.DepartmentName))
                {
                    // Department header row
                    tableHtml.Append(
                        $"<tr class='table-secondary fw-bold bg-secondary dept-row' data-deptid='{dept.DepartmentId}' style='cursor:pointer;'>" +
                        $"<td colspan='7' style='padding-left:20px;'><em>{dept.DepartmentName}</em> " +
                        $"(Total: {dept.Overtimes.Count})</td></tr>"
                    );

                    
                }
               
            }
            
            
            return new JsonResult(new { tableHtml = tableHtml.ToString(), TotalRecords, TotalHours, TotalDays, TotalCost, InvolvedEmployees, DaysPerEmployees, CostPerEmployees, MonthlySummary});

        }
        public IActionResult OnGetDepartmentDetails(int deptId, int? overtimeStatus, int? overtimeType, DateTime? dateStart, DateTime? dateEnd, string? empID)
        {
            
            var otDetails = _context.OvertimeDetailView.AsQueryable();

            // Filter by Status (if provided)
            if (overtimeStatus.HasValue)
            {
                otDetails = otDetails.Where(e => e.OvertimeStatus == (Models.overtimeStatus)overtimeStatus);

            }

            // Filter by type (if provided)
            if (overtimeType.HasValue && overtimeType != null)
            {

                otDetails = otDetails.Where(e => e.OvertimeID == overtimeType);
            }

            // Filter by Start TIme (if provided)
            if (dateStart.HasValue && dateStart != null)
            {
                otDetails = otDetails
                    .Where(e => e.OvertimeDate >= dateStart);
            }
            // Filter by end TIme (if provided)
            if (dateEnd.HasValue && dateEnd != null)
            {
                otDetails = otDetails
                    .Where(e => e.OvertimeDate <= dateEnd);
            }
            // Filter by EmployeeID (if provided)
            if (!string.IsNullOrEmpty(empID))
            {
                Console.WriteLine("##############" + empID);
                otDetails = otDetails
                    .Where(e => e.GivenID == empID);
            }


            
            var records = otDetails?
                .Where(o => o.DepartmentID == deptId)
                .Select(e => new
                {
                    e.OvertimeRecordID,
                    e.GivenID,
                    e.OvertimeName,
                    e.OvertimeDate,
                    e.TimeElapsed,
                    e.OvertimeCost,
                    e.OvertimeStatus
                })
                .ToList();

            var tableHtml = new StringBuilder();

            foreach (var e in records)
            {
                var url = Url.Page("/OvertimeRecord/Details", new { id = e.OvertimeRecordID });
                tableHtml.Append(
                    $"<tr onclick=\"location.href='{url}'\" style=\"cursor:pointer;\">" +
                    $"<td>{e.OvertimeRecordID}</td>" +
                    $"<td>{e.GivenID}</td>" +
                    $"<td>{e.OvertimeName}</td>" +
                    $"<td>{e.OvertimeDate?.ToShortDateString()}</td>" +
                    $"<td>{e.TimeElapsed}</td>" +
                    $"<td>{e.OvertimeCost}</td>" +
                    $"<td>{e.OvertimeStatus}</td></tr>"
                );
            }

            return new JsonResult(new { tableHtml = tableHtml.ToString() });
        }

    }
    public class OvertimeSummaryVM
    {
        public string Period { get; set; }    // "2025-01", "2025", etc.
        public int OvertimeCount { get; set; }
        public decimal TotalCost { get; set; }
        public int EmployeeCount { get; set; }
    }
}
