using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Absentism
{
    public class IndexModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public IndexModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public IList<leaveModel> leaveModel { get; set; } = default!;
        public leaveModel Leave { get; set; }
        public IList<departmentModel> Departments { get; set; } = default!;
        public IList<companyModel> Companies { get; set; }
        public IList<workSiteModel> WorkLocations { get; set; } = default!;
        public DateTime StartDate {get; set;}
        public DateTime EndDate { get; set; }
        public async Task OnGetAsync()
        {
            
            Departments = await _context.Departments.Where(d => d.departmentStatus == mainStatus.Active).OrderBy(d => d.departmentName).ToListAsync();
            
            WorkLocations = await _context.WorkSites.Where(d => d.workSiteStatus == mainStatus.Active).OrderBy(w => w.workSiteName).ToListAsync();
            Companies = await _context.Companies.Where(d => d.companyStatus == mainStatus.Active).OrderBy(c => c.companyName).ToListAsync();

            leaveModel = await _context.Leaves
                .Include(l => l.employmentModel)
                .ThenInclude(e => e.JobPlacements)
                .ThenInclude(j =>j.departmentModel)
                .ThenInclude(d => d.companyModel)
                .Include(l => l.leaveTypeModel)
                .Where(l => l.leaveTypeModel.leaveGroup == leaveGroup.Absentism && l.leaveReaquestDate >= (DateTime.Now.AddMonths(-6))).ToListAsync();

            StartDate = leaveModel.Min(l => l.leaveReaquestDate);
            EndDate = leaveModel.Max(l => l.leaveReaquestDate);
        }
    }
}
