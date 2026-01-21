using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Applications
{
    

    public class EditModel : PageModel
    {
        private readonly PISContext _context;
        private readonly IWebHostEnvironment _env;

        public EditModel(PISContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [BindProperty]
        public ApplicantModel Applicant { get; set; }

        public SelectList VacancyList { get; set; }
        public SelectList PersonList { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Applicant = await _context.Applicants
                .Include(a => a.VacancyModel)
                .Include(a => a.personModel).ThenInclude(p => p.PersonEducationLevels).ThenInclude(e => e.educationLevelModel)
                .Include(a => a.personModel).ThenInclude(p => p.Experiences)
                .FirstOrDefaultAsync(a => a.ApplicantID == id);

            if (Applicant == null) return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var applicant = await _context.Applicants.FirstOrDefaultAsync(a => a.ApplicantID == Applicant.ApplicantID);

            applicant.Status = Applicant.Status;
            applicant.modifiedBy = User.Identity.Name;
            applicant.modifiedDate = DateTime.Now;


            _context.Attach(applicant).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }

}
