using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using PIS2.Views;

namespace PIS2.Pages.Department
{
    public class DetailsModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;
        private readonly PIS2.Models.Core _core;
        public DetailsModel(PIS2.Models.PISContext context, Core core)
        {
            _context = context;
            _core = core;
        }

        public departmentModel departmentModel { get; set; } = default!;
        public DepartmentSummary DepartmentSummary { get; set; }
        public double allowedLeave { get; set; }
        public double leaveCost {  get; set; }
        public List<EducationLevelData> EducationLevels { get; set; }
        public List<NameAndCount> EmploymentTypes { get; set; }
        public List<YearAndCount> EmploymentHireRate { get; set; }
        public List<YearAndCount> TerminationRate { get; set; }
        public List<YearAndCount> ActiveEmployeeRate { get; set; }
        public List<NameAndCount> WorkSiteEmployees { get; set; }
        public List<NameAndCount> EduLevelSummary { get; set; }
        public int leaveEmployments { get; set; }
        public List<jobPlacementModel> Jobs { get; set; }
        public List<employmentModel> Employments { get; set; }
        public List<EmployeeView> EmploymentView { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departmentmodel = await _context.Departments.Include(d => d.companyModel).FirstOrDefaultAsync(m => m.departmentID == id);
            if (departmentmodel == null)
            {
                return NotFound();
            }
            else
            {
               
                departmentModel = departmentmodel;
                Jobs = _context.JobPlacements.Include(j => j.employmentModel).ThenInclude(e => e.personModel).Where(j => j.departmentID == id).ToList();
                Employments = _context.Employments.Include(e => e.employmentTypeModel).Distinct().Where(e => Jobs.Select(j => j.employmentID).Contains(e.employmentID)).ToList();


                EmploymentView = _context.Employments
                    .Include(e => e.personModel).ThenInclude(p => p.addressModel)
                    .Include(e => e.JobPlacements).ThenInclude(jp => jp.jobModel)
                    .Where(e => Employments.Select(e => e.employmentID).Contains(e.employmentID) && e.employmentStatus == mainStatus.Active)
                    .ToList().Select(ev => new EmployeeView
                    {
                        Employment =ev,
                        Job = ev.JobPlacements.OrderByDescending(j => j.jobPlacementDate).First(),
                        Person = ev.personModel
                    }).ToList();


                DepartmentSummary = new DepartmentSummary();
                DepartmentSummary.DepartmentID = departmentmodel.departmentID;
                DepartmentSummary.DepartmentName = departmentmodel.departmentName;

                DepartmentSummary.Employees = Employments
                        .Where(e => e.employmentStatus == mainStatus.Active).Count();
                        
                DepartmentSummary.Salary = Jobs
                        .Where(jp => jp.jobPlacementStatus == mainStatus.Active).Sum(j => j.jobPlacementSalary);

                DepartmentSummary.Leaves = _core.getAllLeaveSummary("Dep", DepartmentSummary.DepartmentID);
                DepartmentSummary.xEmployees = Employments
                        .Where(e => e.employmentStatus == mainStatus.Inactive).Count();

                allowedLeave = DepartmentSummary.Leaves.AllowedLeave;
                leaveCost = DepartmentSummary.Leaves.leaveCost;

                
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
                EmploymentTypes = new List<NameAndCount>();
                EmploymentTypes = Employments
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
                    && l.leaveTypeID != 64 && Employments.Select(ae => ae.employmentID).Contains(l.employmentID)).Count();
            }
            return Page();
        }
    }
}
