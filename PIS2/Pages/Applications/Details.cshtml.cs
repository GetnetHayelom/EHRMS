using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace PIS2.Pages.Applications
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    
    using PIS2.Models;

    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

        public DetailsModel(PISContext context)
        {
            _context = context;
        }

        public ApplicantModel Applicant { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Applicant = await _context.Applicants
                .Include(a => a.VacancyModel)
                .Include(a => a.personModel).ThenInclude(p => p.PersonEducationLevels).ThenInclude(e => e.educationLevelModel)
                .Include(a => a.personModel).ThenInclude(p => p.Experiences)
                .FirstOrDefaultAsync(a => a.ApplicantID == id);

            if (Applicant == null)
            {
                return NotFound();
            }

            return Page();
        }
    }

}
