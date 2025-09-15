using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using PIS2.Models;
using PIS2.Views;

namespace PIS2.Pages.Report
{
    public class LeaveReportModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;
        private readonly PIS2.Models.Core _core;

        public LeaveReportModel(PIS2.Models.PISContext context, Core core)
        {
            _context = context;
            _core = core;
        }

        public IList<leaveModel> leaveModel { get;set; } = default!;
        public IList<LeaveReportCompany> Leaves { get; set; } = default!;
        public IList<departmentModel> Departments { get; set; } = default!;
        public IList<employmentTypeModel> EmploymentTypes { get; set; } = default!;
        public IList<workSiteModel> WorkLocations { get; set; } = default!;
        public IList<companyModel> Companies { get; set; } = default!;
        public IList<leaveTypeModel> LeaveTypes { get; set; } = default!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; } = DateTime.Now;
        public int totalCount { get; set; }
        public int filteredCount { get; set; }
        [BindProperty]
        public decimal totalUnposted {  get; set; }= default!;
        [BindProperty]
        public decimal CountUnposted { get; set; } = default!;

        public async Task OnGetAsync()
        {
            EmploymentTypes = await _context.EmploymentTypes.OrderBy(e => e.employmentTypeName).ToListAsync();
            Departments = await _context.Departments.Where(d => d.departmentStatus == mainStatus.Active).OrderBy(d => d.departmentName).ToListAsync();
            
            Companies = await _context.Companies.Where(c =>c.companyStatus == mainStatus.Active).OrderBy(c => c.companyName).ToListAsync();
            LeaveTypes = await _context.LeaveTypes.Where(l => l.leaveTypeStatus==mainStatus.Active).OrderBy(lt => lt.leaveTypeName).ToListAsync();

            leaveModel = await _context.Leaves
                .Include(l => l.employmentModel)
                .Include(l => l.leaveTypeModel)
                .Where(l => l.leaveStatus != leaveStatus.Hold)
                .ToListAsync();
           
            totalUnposted = leaveModel.Sum(l => l.leaveDays);
            CountUnposted = leaveModel.Count();
            totalCount = leaveModel.Count();

            var leaves = _context.LeaveReportView.Where(lrv => _context.LeaveTypes.Where(l => l.leaveTypeImpact != leaveTypeImpact.Positive).Select(l => l.leaveTypeID).Contains( lrv.LeaveTypeID))
                .AsEnumerable()
                .GroupBy(r => r.CompanyName)
                .Select(g => new LeaveReportCompany
                {
                    CompanyID = g.First().CompanyID,
                    CompanyName = g.Key,
                    EmployeeTotal =_core.GetCompanyEmployees(g.First().CompanyID),
                    WorkingDays = _core.GetWorkingDays(g.Min(g => g.LeaveStart), g.Max(g => g.LeaveEnd)),
                    CompanyTotal = g.Count(),
                    CompanySum = g.Sum(c => c.LeaveDays),
                    StartDate =g.Min(l => l.LeaveStart),
                    EndDate =g.Max(l => l.LeaveEnd),
                    Departments = g.GroupBy(r => r.DepartmentName)
                    .Select(dg => new LeaveReportDepartment
                    {
                        DepartmentID = dg.First().DepartmentID,
                        DepartmentName = dg.Key,
                        EmployeeTotal = _core.GetDepartmentEmployees(dg.First().DepartmentID),
                        DepartmentTotal = dg.Count(),
                        DepartmentSum = dg.Sum(r => r.LeaveDays),
                        LeaveTypes = dg.GroupBy(r => r.LeaveType)
                        .Select(lg => new LeaveReportType
                        {
                            LeaveType = lg.Key,
                            LeaveTypeCount = lg.Count(),
                            LeaveTypeSum = lg.Sum(lg => lg.LeaveDays),
                        }).OrderByDescending(lg => lg.LeaveTypeCount).Take(3).ToList()
                    }).OrderByDescending(dg =>dg.DepartmentSum).ToList()
                }).OrderByDescending(g => g.CompanySum).ToList();
            Leaves = leaves;
        }
        // Post handler

    }
    public class LeaveReportCompany
    {
        public string CompanyName { get; set; }
        public int CompanyID { get; set; }
        public int EmployeeTotal { get; set; }
        public decimal WorkingDays { get; set; }
        public int CompanyTotal { get; set; }
        public decimal? CompanySum { get; set; }
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
        public List<LeaveReportType>? LeaveTypes { get; set; }
    }

    public class LeaveReportType
    {
        public string LeaveType { get; set; }
        public int LeaveTypeCount { get; set; }
        public decimal? LeaveTypeSum { get; set; }
    }


}
