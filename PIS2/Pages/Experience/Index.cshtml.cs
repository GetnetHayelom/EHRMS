using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Exprience
{
    [Authorize(Roles = "MIE\\PMS_HRMANAGER,MIE\\PMS_HRCLERK")]
    public class IndexModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public IndexModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public IList<experienceModel> experienceModel { get;set; } = default!;
        [BindProperty(SupportsGet = true)]
        public mainStatus? EmploymentFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public Gender? GenderFilter { get; set; } 

        public async Task OnGetAsync()
        {
            experienceModel = await _context.Experiences
     .Include(e => e.personModel)
         .ThenInclude(p => p.Employments)
     .ToListAsync();

            // Filter by Employment Status
            if (EmploymentFilter.HasValue)
            {
                experienceModel = experienceModel
                    .Where(e => e.personModel?.Employments?.Any(emp => emp.employmentStatus == EmploymentFilter) == true)
                    .ToList();
            }


            // Filter by Gender
            if (GenderFilter.HasValue)
            {
                experienceModel = experienceModel
                    .Where(e => e.personModel?.personGender == GenderFilter)
                    .ToList();
            }
           

        }
    }
}
