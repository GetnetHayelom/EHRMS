using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.Vacancy
{

    public class IndexModel : PageModel
    {
        private readonly PISContext _context;

        public IndexModel(PISContext context)
        {
            _context = context;
        }

        public IList<VacancyModel> Vacancies { get; set; } = new List<VacancyModel>();

        public async Task OnGetAsync()
        {

            Vacancies = await _context.Vacancies
                .Include(v => v.jobModel)
                .Include(v => v.departmentModel)
                .Include(v => v.employmentMethodModel)
                .Include(v => v.employmentTypeModel)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var vacancy = await _context.Vacancies
             .Include(v => v.Applicants)
             .Include(v => v.JobReqCost)
             .FirstOrDefaultAsync(v => v.VacancyID == id);

            if (vacancy == null) return NotFound();

            if (vacancy.Applicants.Any() || vacancy.JobReqCost.Any())
            {
                ModelState.AddModelError("", "Cannot delete vacancy with related records.");
                return RedirectToPage();
            }

            _context.Vacancies.Remove(vacancy);
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
    }

}
