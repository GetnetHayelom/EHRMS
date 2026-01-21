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
        public Ex_In? ExperienceTypeFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public Gender? GenderFilter { get; set; }
        public List<string> ActiveEmpIds {get; set;}

        public async Task OnGetAsync(int? id)
        {
            var activeEmps = await _context.Employments.Where(e => e.employmentStatus == mainStatus.Active).Select(e => e.givenID).ToListAsync();
            ActiveEmpIds = activeEmps;

            if(id != null)
            {
                experienceModel = await _context.Experiences
                 .Include(e => e.personModel)
                     .ThenInclude(p => p.Employments)
                     .Where(e => e.personID == id)
                 .ToListAsync();

                if(experienceModel == null) { return; }
            }
            else
            {
                experienceModel = await _context.Experiences
                 .Include(e => e.personModel)
                     .ThenInclude(p => p.Employments)
                 .ToListAsync();
            }
                

            // Filter by Experience Type
            if (ExperienceTypeFilter.HasValue)
            {
                experienceModel = experienceModel
                    .Where(e => e.experienceType == ExperienceTypeFilter)
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
