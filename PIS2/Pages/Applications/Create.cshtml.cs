using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.Applications
{   

    public class CreateModel : PageModel
    {
        private readonly PISContext _context;
        private readonly IWebHostEnvironment _env;

        public CreateModel(PISContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [BindProperty]
        public int? SelectedVacancyId { get; set; }

        [BindProperty]
        public string? SelectedVacancyTitle { get; set; }

        [BindProperty]
        public int? SelectedPersonID { get; set; }

        [BindProperty]
        public string? SelectedPersonName { get; set; }
        [BindProperty]
        public ApplicantModel Applicant { get; set; }

        public SelectList VacancyList { get; set; }
        public SelectList PersonList { get; set; }

        public async void OnGet(int? vacancyId, int? personId)
        {
            
            // Always initialize to empty lists (prevents null errors)
            VacancyList = new SelectList(Enumerable.Empty<SelectListItem>());
            PersonList = new SelectList(Enumerable.Empty<SelectListItem>());

            bool hasOpenVacancies = _context.Vacancies
                .Any(v => v.Status == VacancyStatus.Open);

            bool hasPersons = _context.Persons.Any();

            if (!hasOpenVacancies)
            {
                TempData["Message"] = "No Open Vacancy to Apply To!";
                return;
            }

            if (!hasPersons)
            {
                TempData["Message"] = "No Person Records Found!";
                return;
            }

            // Only executed if data exists
            VacancyList = new SelectList(
                _context.Vacancies.Where(v => v.Status == VacancyStatus.Open),
                "VacancyID",
                "VacancyTitle", vacancyId
            );

            PersonList = new SelectList(
                _context.Persons,
                "personID",
                "personFullName", personId
            );

            if (vacancyId.HasValue)
            {
                var selected = _context.Vacancies
                    .FirstOrDefault(v => v.VacancyID == vacancyId.Value);

                if (selected != null)
                {
                    SelectedVacancyId = selected.VacancyID;
                    SelectedVacancyTitle = selected.VacancyTitle;
                }
            }
            if (personId.HasValue)
            {
                var selected = _context.Persons
                    .FirstOrDefault(v => v.personID == personId.Value);

                if (selected != null)
                {
                    SelectedPersonID = selected.personID;
                    SelectedPersonName = selected.personFullName;
                }
            }
        }


        public async Task<IActionResult> OnPostAsync(IFormFile ResumeFile)
        {
            var existing = await _context.Applicants.FirstOrDefaultAsync(a => a.personID == Applicant.personID && a.VacancyID == Applicant.VacancyID);

            if (existing != null)
            {
                TempData["message"] = ("Error", "Applicant already exists!!");
                OnGet(Applicant.VacancyID, Applicant.personID);
                return Page();
            }
            ModelState.Remove("ResumeFile");
            ModelState.Remove("Applicant.modifiedBy");
            Applicant.Status = ApplicantStatus.Pending;
            Applicant.modifiedBy = User.Identity.Name;
            Applicant.modifiedDate =DateTime.Now;

            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        Console.WriteLine($"{kv.Key} --> {error.ErrorMessage}");
                    }
                    Console.WriteLine(kv.ToString());
                }
                OnGet(Applicant.VacancyID, Applicant.personID);
                return Page();
            }


            if (ResumeFile != null)
            {
                var filePath = Path.Combine(_env.WebRootPath, "resumes", ResumeFile.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ResumeFile.CopyToAsync(stream);
                }
                Applicant.ResumeFilePath = "/resumes/" + ResumeFile.FileName;
            }

            _context.Applicants.Add(Applicant);
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }

}
