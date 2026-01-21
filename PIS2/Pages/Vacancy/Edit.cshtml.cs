using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Vacancy
{
    public class EditModel : PageModel
    {
        private readonly PISContext _context;

        public EditModel(PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public VacancyModel VacancyModel { get; set; } = new();

        public List<jobModel> JobList { get; set; } = new();
        public SelectList CompanyList { get; set; }
        public SelectList DepartmentList { get; set; }
        public SelectList EmploymentType { get; set; }
        public SelectList JobRequests { get; set; }
        public SelectList EmploymentMethods { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            
            VacancyModel = await _context.Vacancies.FirstOrDefaultAsync(v => v.VacancyID == id) ?? new VacancyModel();

            if (VacancyModel == null) return NotFound();

            var department = _context.Departments.FirstOrDefault(d => d.departmentID == VacancyModel.departmentID);
            
            var jobTitle = _context.Jobs.FirstOrDefault(j => j.jobID == VacancyModel.jobID);

            JobList = await _context.Jobs
                .Where(j => j.jobStatus == mainStatus.Active)
                .ToListAsync();


            CompanyList = new SelectList(await _context.Companies
                    .Where(c => c.companyStatus == mainStatus.Active)
                    .ToListAsync(),
                "companyID",
                "companyName",
                department?.companyID
            );


            if (VacancyModel?.departmentModel?.companyID != null)
            {
                DepartmentList = new SelectList(
                    await _context.Departments
                        .ToListAsync(),
                    "departmentID",
                    "departmentName",
                    VacancyModel.departmentID
                );
            }
            else
            {
                DepartmentList = new SelectList(Enumerable.Empty<SelectListItem>());
            }

            EmploymentType = new SelectList(await _context.EmploymentTypes.Where(e => e.employmentTypeStatus == mainStatus.Active).ToListAsync(),
                "employmentTypeID",
                "employmentTypeName",
                VacancyModel?.employmentTypeID
            );

            JobRequests = new SelectList(await _context.JobRequirements.Where(j => j.jobRequirementStatus == jobReqStatus.Approved).ToListAsync(),
                "jobRequirementID",
                "jobRequirementID",
                VacancyModel?.jobRequirementID
            );


            EmploymentMethods = new SelectList(await _context.EmploymentMethods.Where(e => e.employmentMethodStatus == mainStatus.Active).ToListAsync(),
                "employmentMethodID",
                "employmentMethodName",
                VacancyModel?.employmentMethodID
            );


            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!_context.Departments.Any(v => v.departmentID == VacancyModel.departmentID && v.departmentStatus == mainStatus.Active))
            {
                TempData["message"] = ("Error", "There is no active department with the given department id!");
            }

            ModelState.Remove("VacancyModel.modifiedBy");
            VacancyModel.modifiedBy = User.Identity.Name;
            VacancyModel.modifiedDate = DateTime.Now;
            
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
                OnGetAsync(VacancyModel.VacancyID); // reload lists
                TempData["message"] = ("Error", "Check fields are filled!");
                return Page();
            }

            _context.Attach(VacancyModel).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }

    }
}
