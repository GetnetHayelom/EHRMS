using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PIS2.Models;
using PIS2.Services;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;

namespace PIS2.Pages.AllowanceAssignment
{
    public class PersonAllowanceModel : PageModel
    {
        private readonly PISContext _context;
        private readonly Core _core;
        public PersonAllowanceModel(PISContext context, Core core)
        {
            _context = context;
            _core = core;
        }
       
        public IList<allowanceAssignmentModel> allowanceAssignmentModel { get; set; } = default!;
        public IList<allowanceAssignmentModel> oldAssignmentModel { get; set; } = default!;
        public employmentModel employmentModel { get; set; }
        public int allowanceCount { get; set; }
        public decimal allowanceSum { get; set; }
        public async Task OnGetAsync(int id)
        {
            var empID = _core.getUserEmp(User.Identity.Name);

            allowanceAssignmentModel = await _context.AllowanceAssignments
                .Include(a => a.allowanceModel)
                .Where(a => a.allowanceStatus == mainStatus.Active && a.employmentID == id)
                .OrderByDescending(a => a.allowanceAssignmentAmount)
                .ToListAsync() ?? new List<allowanceAssignmentModel>();
          
            allowanceCount = allowanceAssignmentModel.Count();
            allowanceSum = allowanceAssignmentModel.Sum(a => a.allowanceAssignmentAmount);
            employmentModel = _context.Employments.Include(e => e.personModel)
                .Include(e => e.JobPlacements).ThenInclude(jp => jp.departmentModel).ThenInclude(d => d.companyModel).FirstOrDefault();

            oldAssignmentModel = await _context.AllowanceAssignments
                .Include(a => a.allowanceModel)
                .Where(a => a.allowanceStatus != mainStatus.Active && a.employmentID == id)
                .OrderByDescending(a => a.allowanceAssignmentAmount)
                .ToListAsync() ?? new List<allowanceAssignmentModel>();
        }
    }
}
