using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;
using PIS2.Services;
using PIS2.Views;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography.Pkcs;
using System.Text.RegularExpressions;
using static System.Formats.Asn1.AsnWriter;

namespace PIS2.Pages.Management
{
    [Authorize(Roles = "MIE\\PMS_HRMANAGER,MIE\\PMS_MANAGEMENT,MIE\\PMS_HRCLERK")]
    public class Companyp : PageModel
    {
        private readonly PISContext _context;
        private readonly Core _core;
        public Companyp(PISContext ctx, Core methods)
        {
            _context = ctx;
            _core = methods;
        }
        //public companyModel Company { get; set; } = default!;
        public List<departmentModel> Departments { get; set; }
        public List<employmentModel> Employments { get; set; }
        public List<DepartmentView> departmentViews { get; set; }
        public ICollection<jobPlacementModel> Jobs { get; set; }
        public List<EducationLevelData> EducationLevels { get; set; }
        public List<NameAndCount> EmploymentTypes { get; set; }
        public List<YearAndCount> EmploymentHireRate { get; set; }
        public List<YearAndCount> TerminationRate { get; set; }
        public List<YearAndCount> ActiveEmployeeRate { get; set; }
        public List<NameAndCount> WorkSiteEmployees { get; set; }
        public List<NameAndCount> EduLevelSummary { get; set; }
        public int totalNoEmployment { get; set; }
        public int exEmployments { get; set; }
        public int activeEmployments { get; set; }
        public int leaveEmployments { get; set; }
        public int permanentEmployments { get; set; }
        public int contractEmployments { get; set; }
        public int contractEnding { get; set; }
        public int pensionEmployments { get; set; }
        public CompanySummary CompanySummary { get; set; }
        public List<DepartmentSummary> DepartmentSummaries { get; set; }
        public List<leaveDetail> leaveDetails { get; set; }
        public decimal allowedLeave { get; set; }
        public decimal leaveCost {  get; set; }
        public decimal allowance { get; set; }
        public decimal overtime { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var cmp = await _context.Companies.FirstOrDefaultAsync(c => c.companyID == id);
            if (cmp == null)
            {
                return NotFound();
            }
            else
            {
                try {
                    Departments = await _context.Departments.Where(c => c.companyID == id).ToListAsync();
                    var depIDs = Departments.Select(d => d.departmentID).ToList();

                    Jobs = await _context.JobPlacements.Include(j => j.departmentModel).Where(j => depIDs.Contains(j.departmentID)).ToListAsync();
                    var activeJobs = Jobs.Where(j => j.jobPlacementStatus == mainStatus.Active).ToList();

                    var empIDs = Jobs.Select(j => j.employmentID).ToList();

                    Employments = await _context.Employments.Where(e => empIDs.Contains(e.employmentID)).Include(e => e.employmentTypeModel).ToListAsync();
                    var activeEmps = Employments.Where(e => e.employmentStatus == mainStatus.Active).ToList();
                    var activeEmpIDs = activeEmps.Select(e => e.personID).ToList();

                    var alwnc = await _context.AllowanceAssignments.Where(e => empIDs.Contains(e.employmentID) && e.allowanceStatus == mainStatus.Active).ToListAsync();
                    allowance =alwnc.Sum(aa => aa.allowanceAssignmentAmount);

                    var activePersons = await _context.Persons.Where(p => activeEmpIDs.Contains(p.personID)).ToListAsync();

                    var ots =await _context.OvertimeRecords.Where(e => empIDs.Contains(e.employmentID) && e.overtimeRecordStatus == overtimeStatus.Hold).ToListAsync();
                    overtime = 0;
                    foreach (var ot in ots)
                    {
                        overtime += ((ot.overtimeRecordEndTime - ot.overtimeRecordStartTime).Minutes / 60) * ot.overtimeRecordEmploymentRate * ot.overtimeRate;
                    }
                    
                    CompanySummary = new CompanySummary();
                    //CompanySummary.Manager = cmp.employmentModel.personModel.personFullName;
                    CompanySummary.CompanyName = cmp.companyName;
                    CompanySummary.CompanyID = cmp.companyID;
                    CompanySummary.Departments = cmp.Departments?.Count() ?? 0;
                    CompanySummary.Salary =(decimal) activeJobs.Sum(j=>j.jobPlacementSalary);
                    CompanySummary.Employees = activeEmps.Count();
                    CompanySummary.xEmployees = Employments.Count(e => e.employmentStatus == mainStatus.Inactive);

                    
                    //Employment Counts
                    totalNoEmployment = Employments.Count();
                    exEmployments = Employments.Where(e => e.employmentStatus == mainStatus.Inactive).Count();
                    activeEmployments = activeEmps.Count();

                    Jobs = Jobs.Where(j => j.jobPlacementStatus == mainStatus.Active).ToList();

                    // Education Level Data
                    EducationLevels = await _context.PersonEducationLevels.Include(p => p.educationLevelModel)
                        .GroupBy(pe => new {pe.educationLevelModel.educationLevelCategory, pe.educationLevelModel.educationLevelName })
                        .Select(g => new EducationLevelData
                        {
                            EducationLevelCategory = g.Key.educationLevelCategory,
                            EducationLevelName = g.Key.educationLevelName,
                            EducationLevelCount = g.Count()
                        }).OrderByDescending(e => e.EducationLevelCount).ToListAsync();

                    //EducationLevels = _context.PersonEducationLevels
                    //    .Join(_context.EducationLevels, pel => pel.educationLevelID, el => el.educationLevelID, (pel, el) => new { pel, el })
                    //    .Where(x => _context.Employments.Any(e => e.personID == x.pel.personID && e.employmentStatus == mainStatus.Active))
                    //    .GroupBy(x => new { x.el.educationLevelCategory, x.el.educationLevelName })
                    //    .Select(g => new EducationLevelData
                    //    {
                    //        EducationLevelCategory = g.Key.educationLevelCategory,
                    //        EducationLevelName = g.Key.educationLevelName,
                    //        EducationLevelCount = g.Count()
                    //    }).OrderByDescending(e => e.EducationLevelCount).ToList();

                    //Employment Types
                    EmploymentTypes = new List<NameAndCount>();
                    EmploymentTypes = activeEmps
                        .GroupBy(e => new { e.employmentTypeModel.employmentTypeName, e.employmentTypeID })
                       .Select(g => new NameAndCount
                       {
                           zName = g.Key.employmentTypeName,
                           zCount = g.Count()
                       })
                       .ToList();

                    //Active Leaves
                    var lvs = await _context.Leaves
                        .Where(l => l.leaveStartDate <= DateTime.Now && l.leaveEndDate >= DateTime.Now && l.leaveTypeModel.leaveTypeImpact == leaveTypeImpact.Negative
                        && l.leaveStatus == leaveStatus.Posted
                        && l.leaveTypeID != 64 && activeEmpIDs.Contains(l.employmentID)).ToListAsync();

                    leaveEmployments = lvs.Count();

                    //Employment hire Rate
                    EmploymentHireRate = activeEmps
                    .GroupBy(e => e.employmentDate.Year) // Group by hire year
                    .Select(g => new YearAndCount
                    {
                        zYear = g.Key,
                        zCount = g.Count()
                    })
                    .OrderBy(g => g.zYear)
                    .ToList();

                    //Termination Rate
                    var trmnt = await _context.Terminations.Where(t => empIDs.Contains(t.employmentID)).ToListAsync();
                    TerminationRate =trmnt
                    .GroupBy(e => e.terminationDate.Year) // Group by hire year
                    .Select(g => new YearAndCount
                    {
                        zYear = g.Key,
                        zCount = g.Count()
                    })
                    .OrderBy(g => g.zYear)
                    .ToList();

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

                     // Get the latest job placement for each employee as of year y
                     let latestPlacement = _context.JobPlacements
                         .Where(jp => jp.employmentID == e.employmentID &&
                                      jp.jobPlacementDate.Year <= y)
                         .OrderByDescending(jp => jp.jobPlacementDate)
                         .FirstOrDefault()
                     // Filter by company ID through the department
                     where latestPlacement != null &&
                           latestPlacement.departmentModel != null &&
                           latestPlacement.departmentModel.companyID == id
                     group e by y into grouped
                     orderby grouped.Key
                     select new YearAndCount
                     {
                         zYear = grouped.Key,
                         zCount = grouped.Count()
                     }).ToList();

                    //WorkSite Employee Distribution
                    WorkSiteEmployees =await _context.SiteAssignments
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
                        .ToListAsync();


                    //edu level summary
                    var els = await _context.PersonEducationLevels.Include(pe => pe.educationLevelModel)
                        .Where(l => Employments.Select(e => e.personID).Contains(l.personID)).ToListAsync();
                    EduLevelSummary = els
                        .GroupBy(pe => pe.educationLevelModel.educationLevelCategory)
                        .Select(el => new NameAndCount
                        { 
                            zName = el.Key.ToString(),
                            zCount = el.Count()
                        }).OrderByDescending(el => el.zCount).ToList();                       
                   
                    leaveDetails = new List<leaveDetail>();
                    var oneLeave = new leaveDetail();
                    foreach (var d in Departments)
                    {
                        oneLeave = await _core.getAllLeaveSummary("Dep", d.departmentID);
                        oneLeave.department = d;
                        leaveDetails.Add(oneLeave);
                    }

                    DepartmentSummaries =Jobs
                        .GroupBy(jp => new { jp.departmentID, jp.departmentModel.departmentName })
                        .Select(ds => new DepartmentSummary
                        {
                            DepartmentID =ds.Key.departmentID,
                            DepartmentName = ds.Key.departmentName,
                            Employees = ds.Count(),
                            Salary = ds.Sum(js=> js.jobPlacementSalary)
                        }).ToList();
                        

                    foreach (var d in DepartmentSummaries)
                    {
                        d.Leaves = leaveDetails.FirstOrDefault(ld =>ld.department.departmentID == d.DepartmentID);
                    }
                    allowedLeave = leaveDetails.Sum(l => l.AllowedLeave);
                    leaveCost = leaveDetails.Sum(l => l.leaveCost);
                }
                catch (Exception ex)
{
                Console.WriteLine("LINQ Query Failed: " + ex.Message);
                Console.WriteLine("Stack Trace: " + ex.StackTrace);
                throw; // ? Re-throw to see the full error in logs
            }

                }
               



            return Page();
        }
    }

}