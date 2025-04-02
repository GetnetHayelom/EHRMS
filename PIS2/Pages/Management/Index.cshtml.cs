using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using PIS2.Views;

namespace PIS2.Pages.Management
{
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
        public int contractEmployments  { get; set; }
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
            Departments= await _context.Departments.ToListAsync();
            Employments = await _context.Employments.ToListAsync();
            exEmployments = _context.Employments.Where(e => e.employmentStatus == mainStatus.Inactive).Count();
            ActiveEmployments = _context.Employments.Where(e => e.employmentStatus == mainStatus.Active).Count();
            ActiveCompanies = _context.Companies.Where(c => c.companyStatus == mainStatus.Active).Count();
            OldCompanies = _context.Companies.Where(c => c.companyStatus == mainStatus.Inactive).Count();
            ActiveDepartments= _context.Departments.Where(c => c.departmentStatus == mainStatus.Active).Count();
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
                }).OrderByDescending(g =>g.zCount).ToList();

            //jobclass Summary
            JobClassSummary = _context.JobPlacements.Include(j => j.jobModel).ThenInclude(j => j.jobClassModel)
                .GroupBy(j => j.jobModel.jobClassModel.JobClassName)
                .Select(g => new NameAndCount
                {
                    zName = g.Key,
                    zCount = g.Count()
                }).ToList();
            //Number of Active Employment per Year
            ActiveEmployeeRate =
            (from y in (
                (from e in _context.Employments select e.employmentDate.Year)
                .Union(from t in _context.Terminations select t.terminationDate.Year)
                .Distinct())
                 from e in _context.Employments
                 where e.employmentDate.Year <= y // Employees hired before or in the given year
                 join t in _context.Terminations on e.employmentID equals t.employmentID into termGroup
                 from t in termGroup.DefaultIfEmpty()
                 where t == null || t.terminationDate.Year > y // Exclude employees terminated in or before the given year
                                                               
                 group e by y into grouped
                 orderby grouped.Key
                 select new YearAndCount
                 {
                     zYear = grouped.Key,
                     zCount = grouped.Count()
                 }).ToList();

            //WorkSite Employee Distribution
            WorkSiteEmployees = _context.JobPlacements.Where(jp => jp.jobPlacementStatus == mainStatus.Active).Include(jp => jp.workSiteModel)
                .GroupBy(w => new { w.workSiteID, w.workSiteModel.workSiteName })
                .Select(we => new NameAndCount
                {
                    zName = we.Key.workSiteName,
                    zCount = we.Count()
                }).OrderByDescending(g => g.zCount).ToList().Where(g => g.zCount>0).ToList();
            //WorkSiteEmployees = (from jp in _context.JobPlacements
            //                      join ws in _context.WorkSites on jp.workSiteID equals ws.workSiteID into wsGroup
            //                      from ws in wsGroup.DefaultIfEmpty() // Left join
            //                      where jp.jobPlacementStatus == mainStatus.Active
            //                      group jp by ws.workSiteName into grouped
            //                     select new NameAndCount
            //                      {
            //                          zName = grouped.Key,
            //                          zCount = grouped.Count()
            //                      }).ToList().Where(we => we.zCount > 0).OrderByDescending(wl =>wl.zCount).ToList();

            //edu level summary
            EduLevelSummary = (from pel in _context.PersonEducationLevels
                                  join el in _context.EducationLevels on pel.educationLevelID equals el.educationLevelID
                                  join e in _context.Employments on pel.personID equals e.personID
                                  where e.employmentStatus == mainStatus.Active
                                  group pel by el.educationLevelCategory into grouped
                                  select new NameAndCount
                                  {
                                      zName = grouped.Key,
                                      zCount = grouped.Count()
                                  }).OrderByDescending(el => el.zCount).ToList();
            CompanySummaries = (from js in _context.JobPlacements
                                join e in _context.Employments on js.employmentID equals e.employmentID into empGroup
                                from e in empGroup.DefaultIfEmpty()
                                join dp in _context.Departments on js.departmentID equals dp.departmentID into deptGroup
                                from dp in deptGroup.DefaultIfEmpty()
                                join cmp in _context.Companies on dp.companyID equals cmp.companyID into compGroup
                                from cmp in compGroup.DefaultIfEmpty()
                                join p in _context.Persons on e.personID equals p.personID into personGroup
                                from p in personGroup.DefaultIfEmpty()
                                where js.jobPlacementStatus == mainStatus.Active
                                group js by new { cmp.companyName, cmp.companyID } into grouped
                                select new CompanySummary
                                {
                                    
                                    CompanyName = grouped.Key.companyName,
                                    CompanyID = grouped.Key.companyID,
                                    Employees = grouped.Select(x => x.employmentID).Distinct().Count(),
                                    Departments = grouped.Select(x => x.departmentID).Distinct().Count(),
                                    Salary = grouped.Sum(x => x.jobPlacementSalary) 
                                }).OrderByDescending(cs => cs.Salary).ToList();
            foreach(var c in CompanySummaries)
            {
                //c.Leaves = _core.getAllLeaveSummary("Comp", c.CompanyID);
                c.Leaves = new leaveDetail();
            }
            

        }

    }
}
