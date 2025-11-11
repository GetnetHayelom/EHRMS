using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using PIS2.Views;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PIS2.Pages.Management
{
    [Authorize(Roles = "MIE\\PMS_HRMANAGER,MIE\\PMS_MANAGEMENT")]
    public class IndexModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;
        private readonly PIS2.Models.Core _core;
        public IndexModel(PISContext ctx, Core methods)
        {
            _context = ctx;
            _core = methods;
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
        public List<EducationLevelData> EducationLevels { get; set; }
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
        public List<AnnuallLeaveSummaryCompanyView> AnnualLeaveSummaries { get; set; }
        public List<EmploymentYearlyStat> EmploymentYearlyStats { get; set; }
        public List<DepartmentEmploymentStats> DepartmentEmploymentStats { get; set; }
        public decimal SeverancePay { get; set; }
        public async Task OnGetAsync()
        {
            //check if user is employee
            //get user employmentID
            //select from companies where employmentID Matches

            //select employments for the company
            //Company View
            //group employments into departments
            //var companies = new List<companyModel>().AsQueryable();
            Companies = await _context.Companies.ToListAsync();
            Departments = await _context.Departments.ToListAsync();
            Employments = await _context.Employments.ToListAsync();
            exEmployments = _context.Employments.Where(e => e.employmentStatus == mainStatus.Inactive).Count();
            ActiveEmployments = _context.Employments.Where(e => e.employmentStatus == mainStatus.Active).Count();
            ActiveCompanies = _context.Companies.Where(c => c.companyStatus == mainStatus.Active).Count();
            OldCompanies = _context.Companies.Where(c => c.companyStatus == mainStatus.Inactive).Count();
            ActiveDepartments = _context.Departments.Where(c => c.departmentStatus == mainStatus.Active).Count();
            OldDepartments = _context.Departments.Where(c => c.departmentStatus == mainStatus.Inactive).Count();
            LeaveSummary = new List<leaveDetail>();


            // Education Level Data
            EducationLevels = _context.PersonEducationLevels
                .Join(_context.EducationLevels, pel => pel.educationLevelID, el => el.educationLevelID, (pel, el) => new { pel, el })
                .Where(x => _context.Employments.Any(e => e.personID == x.pel.personID && e.employmentStatus == mainStatus.Active))
                .GroupBy(x => new { x.el.educationLevelCategory, x.el.educationLevelName })
                .Select(g => new EducationLevelData
                {
                    EducationLevelCategory = g.Key.educationLevelCategory,
                    EducationLevelName = g.Key.educationLevelName,
                    EducationLevelCount = g.Count()
                }).OrderByDescending(e => e.EducationLevelCount).ToList();

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
                .Where(l => l.leaveStartDate <= DateTime.Now && l.leaveEndDate >= DateTime.Now && l.leaveTypeModel.leaveTypeImpact == leaveTypeImpact.Negative
                && l.employmentModel.employmentStatus == mainStatus.Active && l.leaveStatus == leaveStatus.Posted
                && l.leaveTypeID != 64).Count();

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
                .GroupBy(j => j.jobModel.jobClassModel.JobClassName)
                .Select(g => new NameAndCount
                {
                    zName = g.Key,
                    zCount = g.Count()
                }).ToList();

            //Employment Yearly Stats
            EmploymentYearlyStats = _context.EmploymentYearlyStats.OrderBy(eys => eys.Year).ToList();

            //Department Employee Stats
            DepartmentEmploymentStats = _context.DepartmentEmploymentStats
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

            //WorkSite Employee Distribution
            WorkSiteEmployees = _context.SiteAssignments
                .Where(ws => ws.employmentModel.employmentStatus == mainStatus.Active)
                .GroupBy(ws => new { ws.employmentID, ws.workSiteModel.workSiteName })
                .Select(g => new
                {
                    g.Key.employmentID,
                    g.Key.workSiteName
                }) // now we have distinct employment per site
                .GroupBy(x => x.workSiteName)
                .Select(g => new NameAndCount
                {
                    zName = g.Key,
                    zCount = g.Count()
                })
                .OrderByDescending(wl => wl.zCount)
                .ToList();


            WorkSiteEmployees = await _context.SiteAssignments
                .Include(sa => sa.workSiteModel)
                .Include(sa => sa.employmentModel).Where(sa => sa.employmentModel.employmentStatus == mainStatus.Active)
                .GroupBy(sa => sa.workSiteModel.workSiteName)
                .Select(g => new NameAndCount
                {
                    zName = g.Key,
                    zCount = g.Count()
                }).ToListAsync();
            //edu level summary
            EduLevelSummary = _context.PersonEducationLevels.Include(pel => pel.educationLevelModel)
                .GroupBy(cs => cs.educationLevelModel.educationLevelCategory)
                .Select(g => new NameAndCount
                {
                    zName = g.Key,
                    zCount = g.Count()
                }).ToList();

            var companies = _context.CompanySummaryView.ToList();
            CompanySummaries = _context.CompanySummaryView.ToList();

            var annualLeaveSummary = _context.AnnualLeaveSummary.ToList();
            AnnualLeaveSummaries = new List<AnnuallLeaveSummaryCompanyView>();
            AnnualLeaveSummaries = annualLeaveSummary
                .GroupBy(als => als.companyID)
                .Select(g => new AnnuallLeaveSummaryCompanyView
                {
                    CompanyID = g.Key,
                    Company = g.First().companyName,
                    LeaveBalance = g.Sum(lb => lb.leaveBalance),
                    AllowedLeave = g.Sum(lb => lb.adjustedLeaveBalance),
                    PayableLeave =g.Sum(lb => lb.adjustedLeaveBalanceCost),
                    DepartmentList = g.GroupBy(als => als.departmentID).Select(dv => new AnnuallLeaveSummaryDepartmentView
                    {
                        DepartmentID = dv.Key,
                        Department = dv.First().departmentName,
                        LeaveBalance = dv.Sum(lb => lb.leaveBalance),
                        AllowedLeave = dv.Sum(lb => lb.adjustedLeaveBalance),
                        PayableLeave = dv.Sum(lb => lb.adjustedLeaveBalanceCost) 
                    }).OrderByDescending(d => d.PayableLeave).ToList() ?? new List<AnnuallLeaveSummaryDepartmentView>()
                }).OrderByDescending(d => d.PayableLeave).ToList();
            foreach (var c in companies)
            {
                c.payableLeaves = annualLeaveSummary.Where(als => als.companyID == c.CompanyID).Sum(als => als.adjustedLeaveBalanceCost);
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
