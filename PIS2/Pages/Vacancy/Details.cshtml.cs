using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.Vacancy
{
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

        public DetailsModel(PISContext context)
        {
            _context = context;
        }

        public VacancyModel Vacancy { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Vacancy = await _context.Vacancies
                .Include(v => v.jobModel)
                .Include(v => v.departmentModel).ThenInclude(v => v.companyModel)
                .Include(v => v.employmentTypeModel)
                .Include(v => v.employmentMethodModel)
                .Include(v => v.Applicants)
                    .ThenInclude(a => a.personModel)
                .FirstOrDefaultAsync(v => v.VacancyID == id);

            if (Vacancy == null)
                return NotFound();

            return Page();
        }
    }

}
