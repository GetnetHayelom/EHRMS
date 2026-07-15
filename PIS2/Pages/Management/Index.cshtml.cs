using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;
using PIS2.Services;
using PIS2.Views;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PIS2.Pages.Management
{
    [Authorize(Roles = "MIE\\PMS_HRMANAGER,MIE\\PMS_MANAGEMENT,MIE\\PMS_HRCLERK")]
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;
        private readonly Core _core;
        private readonly HRDashboardService _dashboardService;
        public IndexModel(PISContext ctx, Core methods, HRDashboardService dashboardService)
        {
            _context = ctx;
            _core = methods;
            _dashboardService = dashboardService;
        }
        public IList<companyModel> Companies { get; set; }
        public IList<departmentModel> Departments { get; set; }
        public IList<employmentModel> Employments { get; set; }
        public int totalNoEmployment { get; set; }
        public int exEmployments { get; set; }
        public int ActiveEmployments { get; set; }
        public int ActiveCompanies { get; set; }
        public int ActiveDepartments { get; set; }
        public int OldCompanies { get; set; }
        public int OldDepartments { get; set; }
        public int leaveEmployments { get; set; }
        public int permanentEmployments { get; set; }
        public int contractEmployments { get; set; }
        public int contractEnding { get; set; }
        public int pensionEmployments { get; set; }
        public List<NameAndCount> EmploymentTypes { get; set; }
        public List<YearAndCount> EmploymentHireRate { get; set; }
        public List<YearAndCount> TerminationRate { get; set; }
        public List<YearAndCount> ActiveEmployeeRate { get; set; }
        public List<NameAndCount> WorkSiteEmployees { get; set; }
        public List<NameAndCount> EduLevelSummary { get; set; }
        public List<NameAndCount> JobCategorySummary { get; set; }
        public List<NameAndCount> JobClassSummary { get; set; }
        public List<CompanySummary> CompanySummaries { get; set; }
        public List<leaveDetail> LeaveSummary { get; set; }
        public List<LeaveBalanceDepartmentView> AnnualLeaveSummaries { get; set; }
        public List<EmploymentYearlyStat> EmploymentYearlyStats { get; set; } = new List<EmploymentYearlyStat>();
        public List<DepartmentEmploymentStats> DepartmentEmploymentStats { get; set; }
        public decimal SeverancePay { get; set; }
        public List<MetricComparison> MetricData { get; set; }
        public async Task OnGetAsync()
        {
            //check if user is employee
            //get user employmentID
            //select from companies where employmentID Matches

            //select employments for the company
            //Company View
            //group employments into departments
            //var companies = new List<companyModel>().AsQueryable();

            MetricData = new List<MetricComparison>();
            Companies = await _context.Companies.ToListAsync() ?? new List<companyModel>();
            Departments = await _context.Departments.ToListAsync() ?? new List<departmentModel>();
            Employments = await _context.Employments.ToListAsync() ?? new List<employmentModel>();
            exEmployments = _context.Employments.Where(e => e.employmentStatus == mainStatus.Inactive).Count();
            ActiveEmployments = _context.Employments.Where(e => e.employmentStatus == mainStatus.Active).Count();
            ActiveCompanies = _context.Companies.Where(c => c.companyStatus == mainStatus.Active).Count();
            OldCompanies = _context.Companies.Where(c => c.companyStatus == mainStatus.Inactive).Count();
            ActiveDepartments = _context.Departments.Where(c => c.departmentStatus == mainStatus.Active).Count();
            OldDepartments = _context.Departments.Where(c => c.departmentStatus == mainStatus.Inactive).Count();
            LeaveSummary = new List<leaveDetail>();
            
            //Employment Types
            EmploymentTypes = _context.Employments
           .Where(e => e.employmentStatus == mainStatus.Active)
           .GroupBy(e => new { e.employmentTypeModel.employmentTypeName, e.employmentTypeID })
           .Select(g => new NameAndCount
           {
               zName = g.Key.employmentTypeName,
               zCount = g.Count()
           })
           .ToList();

            //Active Leaves
            leaveEmployments = _context.Leaves
                .Where(l => l.leaveStartDate <= DateTime.Now && l.leaveEndDate >= DateTime.Now
                && l.employmentModel.employmentStatus == mainStatus.Active && l.leaveStatus == leaveStatus.Posted
                && l.leaveTypeModel.leaveJob == false).Count();

            //Employment hire Rate
            EmploymentHireRate = _context.Employments
            .GroupBy(e => e.employmentDate.Year) // Group by hire year
            .Select(g => new YearAndCount
            {
                zYear = g.Key,
                zCount = g.Count()
            })
            .OrderBy(g => g.zYear)
            .ToList();

            //Termination Rate
            TerminationRate = _context.Terminations
            .GroupBy(e => e.terminationDate.Year) // Group by hire year
            .Select(g => new YearAndCount
            {
                zYear = g.Key,
                zCount = g.Count()
            })
            .OrderBy(g => g.zYear)
            .ToList();

            //jobcategory Summary
            JobCategorySummary = _context.JobPlacements.Include(j => j.jobModel).ThenInclude(j => j.jobCategoryModel).Where(j => j.jobPlacementStatus == mainStatus.Active)
                .GroupBy(j => j.jobModel.jobCategoryModel.jobCategoryName)
                .Select(g => new NameAndCount
                {
                    zName = g.Key,
                    zCount = g.Count()
                }).OrderByDescending(g => g.zCount).ToList();

            //jobclass Summary
            JobClassSummary = _context.JobPlacements.Include(j => j.jobModel).ThenInclude(j => j.jobClassModel)
                .Where(j => j.jobPlacementStatus == mainStatus.Active)
                .GroupBy(j => j.jobModel.jobClassModel.jobClassName)
                .Select(g => new NameAndCount
                {
                    zName = g.Key,
                    zCount = g.Count()
                }).ToList();

            //Employment Yearly Stats
            EmploymentYearlyStats = _context.EmploymentYearlyStats.OrderBy(eys => eys.Year).ToList();

            //Department Employee Stats
            var activeComps = await _context.Companies.Where(c => c.companyStatus == mainStatus.Active).Select(c => c.companyID).ToListAsync();

            DepartmentEmploymentStats = _context.DepartmentEmploymentStats.Where(c => c.companyID.HasValue && activeComps.Contains(c.companyID.Value))
                .GroupBy(des => des.companyID)
                .Select(d => new DepartmentEmploymentStats
                {
                    companyName = d.FirstOrDefault().companyName ?? "N\\A",
                    TotalEmployees =d.Sum(des => des.TotalEmployees ?? 0) ,
                    ActiveEmployees = d.Sum(des => des.ActiveEmployees ?? 0) ,
                    TerminatedEmployees = d.Sum(des => des.TerminatedEmployees ?? 0),
                    TerminationRatePercent = d.Sum(des => des.TerminationRatePercent ?? 0) /d.Count(),
                }).OrderByDescending(des => des.TerminationRatePercent).ToList();
            //Number of Active Employment per Year
            ActiveEmployeeRate = _context.EmploymentYearlyStats
                .Select(a => new YearAndCount
                 {
                     zYear = a.Year ?? 0,
                     zCount = a.ActiveEmployees ?? 0
                 }).OrderBy(ac => ac.zYear).ToList();

            WorkSiteEmployees = await _context.WorksiteSummaryView
                .Select(g => new NameAndCount
                {
                    zName = g.workSiteName,
                    zCount = g.Total
                }).ToListAsync();
            //edu level summary
            EduLevelSummary = _context.CertificationSummaryView
                .Select(g => new NameAndCount
                {
                    zName = g.CertificationCategory.ToString(),
                    zCount = g.Total
                }).ToList();


            var activeComp = await _context.Companies.Where(c => c.companyStatus == mainStatus.Active).Select(c => c.companyID).ToListAsync();
            var companies = _context.CompanySummaryView.ToList();
            CompanySummaries = _context.CompanySummaryView.Where(c => activeComp.Contains(c.CompanyID) && c.Employees >0).ToList();

            
            AnnualLeaveSummaries = new List<LeaveBalanceDepartmentView>();
            AnnualLeaveSummaries = await _context.LeaveBalanceDepartmentView
                .OrderByDescending(d => d.PayableLeave).ToListAsync();

            foreach (var c in companies)
            {
                c.payableLeaves = AnnualLeaveSummaries.Where(als => als.CompanyID == c.CompanyID).Sum(als => als.PayableLeave);
                //c.payableLeaves = new leaveDetail();
            }
            

        }

        
    }
    
    public class AnnuallLeaveSummaryDepartmentView
    {
        public int DepartmentID { get; set; } = 0;
        public string Department { get; set; } = "";
        public decimal LeaveBalance { get; set; } = 0;
        public decimal AllowedLeave { get; set; } = 0;
        public decimal PayableLeave { get; set; } = 0;
        //public List<AnnualLeaveSummary> EmployeeList { get; set; }
    }
}
