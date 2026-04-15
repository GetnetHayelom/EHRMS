using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Department
{
    public class ShiftSiteModel : PageModel
    {
        private readonly PISContext _context;

        public ShiftSiteModel(PISContext context)
        {
            _context = context;
        }

        public IList<shiftAssignmentModel> shiftAssignmentModel { get; set; } = new List<shiftAssignmentModel>();
        public IList<siteAssignmentModel> siteAssignmentModel { get; set; } = new List<siteAssignmentModel>();
        public departmentModel Department { get; set; } = new departmentModel();

        public List<shiftModel> Shifts { get; set; }
        public List<workSiteModel> WorkSites { get; set; }
        public bool isManager { get; set; }
        public bool isMember { get; set; }
        public bool isDelegatee { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            Shifts = await _context.Shifts.Where(s => s.shiftStatus == mainStatus.Active).ToListAsync();
            WorkSites = await _context.WorkSites.Where(s => s.workSiteStatus == mainStatus.Active).ToListAsync();

            int personID = _context.Users.FirstOrDefault(u => u.userName == User.Identity.Name).personID;

            int empID = _context.Employments.FirstOrDefault(e => e.personID == personID && e.employmentStatus == mainStatus.Active).employmentID;

            int depID = _context.JobPlacements.FirstOrDefault(jp => jp.employmentID == empID && jp.jobPlacementStatus == mainStatus.Active).departmentID;
             

            if (id == null)
            {
                TempData["message"] = "Error: Department ID is required.";
                return RedirectToPage("/Department/Index");
            }

            Department = await _context.Departments.FirstOrDefaultAsync(d => d.departmentID == id);

            if (Department == null)
            {
                TempData["message"] = "Error: Department not found.";
                return RedirectToPage("/Department/Index");
            }
            
            isMember = User.IsInRole("MIE\\PMS_MANAGEMENT") ? true : false;
            isManager = empID == Department?.employmentID ? true : false;

            var delegation = _context.Delegations
                .FirstOrDefault(d =>
                    d.delegationFrom == Department.employmentID &&
                    d.delegationStatus == mainStatus.Active);

            isDelegatee = delegation != null && delegation.delegationTo == empID;

            // Get active employees specifically for this department
            var activeEmps = await _context.JobPlacements
                .Where(j => j.departmentID == id && j.jobPlacementStatus == mainStatus.Active)
                .Select(j => j.employmentID)
                .ToListAsync();

            // Load Latest Shift Assignments
            shiftAssignmentModel = await _context.ShiftAssignments
                .Include(s => s.EmploymentModel).ThenInclude(e => e.personModel)
                .Include(s => s.shiftModel)
                .Where(s => activeEmps.Contains(s.employmentID))
                .GroupBy(s => s.employmentID)
                .Select(g => g.OrderByDescending(s => s.modifiedDate).FirstOrDefault())
                .ToListAsync();

            // Load Latest Site Assignments
            siteAssignmentModel = await _context.SiteAssignments
                .Include(s => s.employmentModel).ThenInclude(e => e.personModel)
                .Include(s => s.workSiteModel)
                .Where(s => activeEmps.Contains(s.employmentID))
                .GroupBy(s => s.employmentID)
                .Select(g => g.OrderByDescending(s => s.modifiedDate).FirstOrDefault())
                .ToListAsync();

            return Page();
        }
    }
}