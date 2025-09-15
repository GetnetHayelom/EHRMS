using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using PIS2.Views;

namespace PIS2.Pages.PersonEducationLevel
{
    public class IndexModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public IndexModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public IList<personEducationLevelModel> personEducationLevelModel { get;set; } = default!;
        public IList<CertificationView> CertificationSummary { get;set; }

        public async Task OnGetAsync()
        {
            personEducationLevelModel = await _context.PersonEducationLevels
                .Include(p => p.educationLevelModel)
                .Include(p => p.personModel)
                .OrderBy(p => p.personModel.personFirstName)
                .ThenBy(p => p.personModel.personFatherName)
                .ThenBy(p => p.personModel.personLastName).ToListAsync();

            var Certifications = _context.PersonEducationLevels
                .Include(pe => pe.educationLevelModel)
                .Include(pe => pe.personModel).ThenInclude(p => p.Employments)
                .Where(pe => pe.personModel.Employments.Any(e => e.employmentStatus == mainStatus.Active))
                .GroupBy(pe => pe.educationLevelModel.educationLevelCategory)
                .Select(g => new CertificationView
                {
                    CertificationType = g.Key,
                    MaleCount = g.Count(pe => pe.personModel.personGender == Gender.Male),
                    FemaleCount = g.Count(pe => pe.personModel.personGender == Gender.Female),
                    TotalCount = g.Count()
                })
                .ToList();

            CertificationSummary = Certifications;

        }
    }
}
