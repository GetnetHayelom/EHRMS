using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using PIS2.Views;

namespace PIS2.Pages.Management
{
    public class CompanyModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public CompanyModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }
        public companyModel Company { get; set; } = default!;
        public List<departmentModel> Departments { get; set; }
        public List<employmentModel> Employments { get; set; }
        public List<DepartmentView> departmentViews { get; set; }
        public int totalNoEmployment { get; set; }
        public int exEmployments { get; set; }
        public int leaveEmployments { get; set; }
        public int permanentEmployments { get; set; }
        public int contractEmployments { get; set; }
        public int contractEnding { get; set; }
        public int pensionEmployments { get; set; }
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
                Company = cmp;
                Departments = await _context.Departments.Include(d => d.JobPlacements).ThenInclude(jp => jp.jobModel)
                    .Include(d => d.JobPlacements).ThenInclude(jp => jp.employmentModel).ThenInclude(e => e.personModel)
                    .Where(d => d.companyID == Company.companyID).ToListAsync();
                var employmentModel = _context.Employments
                .Include(e => e.personModel)
                .Include(e => e.employmentTypeModel)
                .Include(e => e.JobPlacements.OrderByDescending(jp => jp.jobPlacementDate).Take(1)).ThenInclude(jp => jp.departmentModel)
                .Include(e => e.JobPlacements.OrderByDescending(jp => jp.jobPlacementDate).Take(1)).ThenInclude(jp => jp.jobModel)
                .AsQueryable(); // Using IQueryable to build a dynamic query

                var depV = new DepartmentView();
                foreach (var dv in Departments)
                {
                    depV = new DepartmentView();
                    depV.Employments = _context.Employments.Where(e => e.JobPlacements.Any(jp => jp.departmentID == dv.departmentID)).ToList();
                }
            }




            return Page();
        }
    }

}