using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;
using PIS2.Models.Organization;

namespace PIS2.Pages.Structure
{
    public class DepartmentDetailModel : PageModel
    {
        private readonly PISContext _context;
        public DepartmentDetailModel(PISContext context) => _context = context;

        public departmentModel Department { get; set; }
        public List<jobPlacementModel> Placements { get; set; }

        public async Task OnGetAsync(int id, int? jobId)
        {
            // Get Department info
            Department = await _context.Departments
                .Include(d => d.companyModel)
                .FirstOrDefaultAsync(m => m.departmentID == id);

            // Get the actual people in these roles
            var query = _context.JobPlacements
                .Include(p => p.employmentModel)
                .Include(p => p.jobModel)
                .Where(p => p.departmentID == id && p.jobPlacementStatus == mainStatus.Active);

            if (jobId.HasValue)
            {
                query = query.Where(p => p.jobID == jobId);
            }

            Placements = await query.ToListAsync();
        }
    }
}
