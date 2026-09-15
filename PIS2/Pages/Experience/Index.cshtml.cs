using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;
using PIS2.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Exprience
{
    [Authorize(Roles = "HRMANAGER,HRCLERK")]
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;

        public IndexModel(PISContext context)
        {
            _context = context;
        }

        public IList<ExperienceView> experienceModel { get;set; } = default!;
        [BindProperty(SupportsGet = true)]
        public Ex_In? ExperienceTypeFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public Gender? GenderFilter { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? id { get; set; }
        public async Task OnGetAsync()
        {
            var experiences = _context.ExperienceView.OrderBy(e => e.experienceStartDate)
                 .AsQueryable();

            if (experiences != null)
            {
                if (id != null)
                {
                    experiences = _context.ExperienceView
                         .Where(e => e.personID == id);
                }

                // Filter by Experience Type
                if (ExperienceTypeFilter.HasValue)
                {
                    experiences = experiences
                        .Where(e => e.experienceType == ExperienceTypeFilter);
                }

                // Filter by Gender
                if (GenderFilter.HasValue)
                {
                    experiences = experiences
                        .Where(e => e.personGender == GenderFilter);
                }
                experienceModel = await experiences.ToListAsync();
            }
            
        }
    }
}
