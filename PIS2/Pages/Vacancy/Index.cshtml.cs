using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
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

        public IList<VacancyModel> Vacancies { get; set; }

        public async Task OnGetAsync()
        {
            Vacancies = await _context.Vacancies
                .Include(v => v.jobModel)
                .Include(v => v.departmentModel)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var vacancy = await _context.Vacancies.FindAsync(id);
            if (vacancy != null)
            {
                _context.Vacancies.Remove(vacancy);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }

}
