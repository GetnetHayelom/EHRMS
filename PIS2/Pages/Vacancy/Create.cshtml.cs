using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PIS2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;

namespace PIS2.Pages.Vacancy
{
    public class CreateModel : PageModel
    {
        private readonly PISContext _context;

        public CreateModel(PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public VacancyModel VacancyModel { get; set; }

        public List<jobModel> JobList { get; set; }
        public SelectList CompanyList { get; set; }
        public SelectList EmploymentType { get; set; }
        public SelectList JobRequests { get; set; }
        public SelectList EmploymentMethods { get; set; }

        public void OnGet()
        {
            JobList = _context.Jobs.Where(j => j.jobStatus == mainStatus.Active).ToList();
            CompanyList =new SelectList(_context.Companies.Where(d => d.companyStatus == mainStatus.Active).ToList(), "companyID", "companyName");
            EmploymentType = new SelectList(_context.EmploymentTypes.Where(d => d.employmentTypeStatus == mainStatus.Active).ToList(), "employmentTypeID", "employmentTypeName");
            JobRequests = new SelectList(_context.JobRequirements.Where(j => j.jobRequirementStatus == jobReqStatus.Approved).ToList(), "jobRequirementID", "jobRequirementID");
            EmploymentMethods = new SelectList(_context.EmploymentMethods.Where(j => j.employmentMethodStatus == mainStatus.Active).ToList(), "employmentMethodID", "employmentMethodName");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(!_context.Departments.Any(v => v.departmentID == VacancyModel.departmentID && v.departmentStatus == mainStatus.Active))
            {
                TempData["message"] = ("Error", "There is no active department with the given department id!");
            }

            var vacancy = new VacancyModel()
            {
                ClosingDate = VacancyModel.ClosingDate,
                jobID = VacancyModel.jobID,
                jobRequirementID = VacancyModel.jobRequirementID ?? null,
                VacancyTitle = VacancyModel.VacancyTitle,
                departmentID = VacancyModel.departmentID,
                employmentTypeID = VacancyModel.employmentTypeID,
                Location = VacancyModel.Location,
                VacancyRequiredNumber =VacancyModel.VacancyRequiredNumber,
                DatePosted =VacancyModel.DatePosted,
                VacancyType =VacancyModel.VacancyType,
                Status = VacancyStatus.OnHold,
                Remark =VacancyModel.Remark,
                modifiedBy =User.Identity.Name,
                modifiedDate = DateTime.Now
            };
            ModelState.Remove("VacancyModel.modifiedBy");
            VacancyModel.modifiedBy = User.Identity.Name;
            VacancyModel.modifiedDate = DateTime.Now;
            VacancyModel.Status = VacancyStatus.OnHold;

           
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
                JobList = _context.Jobs.Where(j => j.jobStatus == mainStatus.Active).ToList();
                CompanyList = new SelectList(_context.Companies.Where(d => d.companyStatus == mainStatus.Active).ToList(), "companyID", "companyName");
                EmploymentType = new SelectList(_context.EmploymentTypes.Where(d => d.employmentTypeStatus == mainStatus.Active).ToList(), "employmentTypeID", "employmentTypeName");
                JobRequests = new SelectList(_context.JobRequirements.Where(j => j.jobRequirementStatus == jobReqStatus.Approved).ToList(), "jobRequirementID", "jobRequirementID");
                EmploymentMethods = new SelectList(_context.EmploymentMethods.Where(j => j.employmentMethodStatus == mainStatus.Active).ToList(), "employmentMethodID", "employmentMethodName");
                OnGet(); // reload lists
                TempData["message"] = ("Error", "Check fields are filled!");
                VacancyModel = vacancy;
                return Page();
            }
            _context.Vacancies.Add(vacancy);
            await _context.SaveChangesAsync();
            
            return RedirectToPage("Index");
        }
    }

}
