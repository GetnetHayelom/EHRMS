using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.Vacancy
{
    public class PublishModel : PageModel
    {
        private readonly PISContext _context;

        public PublishModel(PISContext context)
        {
            _context = context;
        }

        public VacancyModel Vacancy { get; set; }
        public int? personId { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            personId = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name).personID;

            Vacancy = await _context.Vacancies
                .Include(v => v.jobModel)
                .Include(v => v.departmentModel)
                .Include(v => v.employmentTypeModel)
                .FirstOrDefaultAsync(v => v.VacancyID == id);

            if (Vacancy == null)
                return NotFound();

            return Page();
        }
        public async Task<IActionResult> OnPostApplyAsync(int id)
        {
            Vacancy = await _context.Vacancies.FindAsync(id);

            if (Vacancy == null || Vacancy.Status != Enums.VacancyStatus.Open)
            {
                TempData["message"] =("Error", "This vacancy is no longer open!");
                return RedirectToPage(new { id });
            }

            var personID = _context.Users?.FirstOrDefault(u => u.UserName == User.Identity.Name)?.personID ?? 0;

            if (personID == 0)
            {
                //TempData["message"] = ("Error","Please select a person before applying!");
                return RedirectToPage(new { id });
            }

            var applicant = new ApplicantModel
            {
                VacancyID = id,
                personID = personID,
                AppliedDate = DateTime.Now,
                Status = Enums.ApplicantStatus.Pending,
                modifiedBy = User.Identity?.Name ?? "System",
                modifiedDate = DateTime.Now
            };

            _context.Applicants.Add(applicant);
            await _context.SaveChangesAsync();

            //TempData["Success"] = $"Application submitted successfully. Applicant ID: {applicant.ApplicantID}";

            return RedirectToPage(new { id });
        }
    }


}
