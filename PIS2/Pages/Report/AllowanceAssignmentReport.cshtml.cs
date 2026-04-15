using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;
using PIS2.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PIS2.Pages.Report
{
    public class AllowanceAssignmentReportModel : PageModel
    {
        private readonly PISContext _context;

        public AllowanceAssignmentReportModel(PISContext context)
        {
            _context = context;
        }
        public int TotalRecords { get; set; }
        public decimal TotalSum { get; set; }
        public IList<AllowanceDetailView> allowanceAssignmentModel { get;set; } = default!;
        public IList<departmentModel> Departments { get; set; } = default!;
        public IList<jobClassModel> JobClasses { get; set; } = default!;
        public IList<jobModel> JobTitles { get; set; } = default!;
        public IList<companyModel> Companies { get; set; } = default!;
        public IList<allowanceModel> AllowanceTypes { get; set; } = default!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; } = DateTime.Now;
        public int GrandCount { get; set; }
        public int ActiveCount { get; set; }
        public int Employees { get; set; }
        public decimal GrandSum { get; set; }
        public decimal ActiveSum { get; set; }
        public async Task OnGetAsync()
        {
            JobClasses = await _context.JobClasses.OrderBy(e => e.jobClassName).ToListAsync();
            Departments = await _context.Departments.OrderBy(d => d.departmentName).ToListAsync();
            JobTitles = await _context.Jobs.OrderBy(j => j.jobTitle).ToListAsync();
            Companies = await _context.Companies.OrderBy(c => c.companyName).ToListAsync();
            AllowanceTypes = await _context.Allowances.OrderBy(c => c.allowanceName).ToListAsync();

            allowanceAssignmentModel = await _context.AllowanceDetailView.ToListAsync();

            GrandCount=allowanceAssignmentModel.Count();
            ActiveCount = allowanceAssignmentModel.Count(s => s.AllowanceStatus == mainStatus.Active);
            Employees = allowanceAssignmentModel.Select(e => e.EmployeeID).Distinct().Count();
            GrandSum = allowanceAssignmentModel.Where(s => s.AllowanceStatus == mainStatus.Active).Sum(s => s.AllowanceAmount);
        }
        
        public IActionResult OnGetFilter(int? department, int? jobTitle, int? jobClass, int? allowanceDuration, int? position,
            int? allowanceStatus, int? allowanceType, int? company, DateTime? dateStart, DateTime? dateEnd)
        {
            Console.WriteLine("the Date is " + dateStart);
            // Start with the full list of employees
            var allowanceModel = _context.AllowanceDetailView.AsQueryable(); // Using IQueryable to build a dynamic query
            

            // Filter by company (if provided)
            if (company.HasValue)
            {
                allowanceModel = allowanceModel.Where(e => e.CompanyID == company);
            }
           
            // Filter by Department (if provided)
            if (department.HasValue)
            {
                allowanceModel = allowanceModel.Where(e => e.DepartmentID == department);

            }

            // Filter by Status (if provided)
            if (allowanceStatus.HasValue)
            {

                allowanceModel = allowanceModel.Where(e => e.AllowanceStatus == (mainStatus)allowanceStatus);

            }

            // Filter by Job Class (if provided)
            if (jobClass.HasValue)
            {

                allowanceModel = allowanceModel.Where(e => e.JobClass == jobClass);
            }
            // Filter by Job Title (if provided)
            if (jobTitle.HasValue)
            {

                allowanceModel = allowanceModel.Where(e => e.JobID == jobTitle);
            }
            // Filter by type (if provided)
            if (allowanceType.HasValue)
            {

                allowanceModel = allowanceModel.Where(e => e.AllowanceTypeID == allowanceType);
            }

            // Filter by Position (if provided)
            if (position.HasValue)
            {

                allowanceModel = allowanceModel.Where(e => e.Position == (EmploymentPositions) position);
            }
            // Filter by Position (if provided)
            if (allowanceDuration.HasValue)
            {

                allowanceModel = allowanceModel.Where(e => e.AllowanceDuration == (allowanceDuration)allowanceDuration);
            }
            // Filter by Start TIme (if provided)
            if (dateStart.HasValue)
            {
                allowanceModel = allowanceModel
                    .Where(e => e.AllowanceStart >= dateStart);
            }
            // Filter by end TIme (if provided)
            if (dateEnd.HasValue)
            {
                allowanceModel = allowanceModel
                    .Where(e => e.AllowanceStart <= dateEnd);
            }

            // Execute the query and get the filtered results
            var filteredAllowance = allowanceModel.AsEnumerable();

            TotalRecords = allowanceModel.Count();
            TotalSum = filteredAllowance.Sum(f => f.AllowanceAmount);
            ActiveCount = filteredAllowance.Count(s => s.AllowanceStatus == mainStatus.Active);
            Employees = filteredAllowance.Select(e => e.EmployeeID).Distinct().Count();
            ActiveSum = filteredAllowance.Where(s => s.AllowanceStatus == mainStatus.Active).Sum(s => s.AllowanceAmount);



            var grouped = filteredAllowance
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
                            Allowances = deptGroup.ToList()
                        }).ToList()
                }).ToList();

            var tableHtml = new StringBuilder();

            foreach (var com in grouped.OrderBy(c => c.CompanyName))
            {
                // Company header row
                tableHtml.Append(
                    $"<tr class='table-primary'><td colspan='2'><strong>{com.CompanyName}</strong> " +
                    $"(Total: {com.Departments.Sum(d => d.Allowances.Count)})</td><td colspan='3'>{com.Departments.Sum(a => a.Allowances.Sum(a => a.AllowanceAmount)).ToString("N2")}</td></tr>"
                );

                foreach (var dept in com.Departments.OrderBy(d => d.DepartmentName))
                {
                    // Department header row
                    tableHtml.Append(
                        $"<tr class='table-secondary fw-bold bg-secondary dept-row' style='cursor:pointer;'>" +
                        $"<td colspan='2' style='padding-left:20px;'><em>{dept.DepartmentName}</em> " +
                        $"(Total: {dept.Allowances.Count})</td><td colspan='3'>{dept.Allowances.Sum(a => a.AllowanceAmount).ToString("N2")}</td></tr>"
                    );

                    var url ="";
                    //Allowances
                    foreach (var allowance in dept.Allowances.OrderBy(a => a.EmployeeID))
                    {
                        url= Url.Page("/AllowanceAssignments/Details", new { id = allowance.AllowanceID });

                        tableHtml.Append($"<tr onclick=\"location.href='{url}'\" style=\"cursor:pointer;\" class=\"allowance-row\">" +
                        $"<td>{allowance.EmployeeID}</td>" +
                        $"<td>{allowance.AllowanceTypeName}</td>" +
                        $"<td>{allowance.AllowanceAmount}</td>" +
                        $"<td>{allowance.AllowanceStart}</td>" +
                        $"<td>{allowance.AllowanceStatus}</td></tr>");
                    }

                }

            }
                        
            return new JsonResult(new { tableHtml = tableHtml.ToString(), TotalRecords=TotalRecords.ToString("N2"), TotalSum = TotalSum.ToString("N2"), ActiveCount = ActiveCount.ToString("N2"), Employees = Employees.ToString("N2"), ActiveSum=ActiveSum.ToString("N2") });
        }
    }
    
}
