using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PIS2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using PIS2.Data;

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
        public jobRequirementModel? JobReq { get; set; }
        
        public void OnGet(int? id)
        {
            var jobReq = _context.JobRequirements.Include(j => j.JobModel)
                .Include(j => j.DepartmentModel).FirstOrDefault(j => j.jobRequirementID == id);
            JobReq = jobReq ?? new jobRequirementModel();

            if(JobReq != null)
            {
                VacancyModel = new VacancyModel();
                VacancyModel.departmentID = JobReq.departmentID;
            }

            JobList = _context.Jobs.Where(j => j.jobStatus == mainStatus.Active).ToList();
            CompanyList =new SelectList(_context.Companies.Where(d => d.companyStatus == mainStatus.Active).ToList(), "companyID", "companyName");
            EmploymentType = new SelectList(_context.EmploymentTypes.Where(d => d.employmentTypeStatus == mainStatus.Active).ToList(), "employmentTypeID", "employmentTypeName");
            var jReqs = _context.JobRequirements.Include(j => j.JobModel).Include(j => j.DepartmentModel).Where(j => j.jobRequirementStatus == jobReqStatus.Approved).Select(j => new
            {
                jrID= j.jobRequirementID,
                jrTitle = $"{j.JobModel.jobTitle}, {j.DepartmentModel.departmentName}"
            }).ToList();
            JobRequests = new SelectList(jReqs, "jrID", "jrTitle", id);
            EmploymentMethods = new SelectList(_context.EmploymentMethods.Where(j => j.employmentMethodStatus == mainStatus.Active).ToList(), "employmentMethodID", "employmentMethodName");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            
            int depID = 0;
            int jobID = 0;
            if(VacancyModel.jobRequirementID != null && VacancyModel.jobRequirementID >0)
            {
                var req = await _context.JobRequirements.FirstOrDefaultAsync(j => j.jobRequirementID == VacancyModel.jobRequirementID);
                depID = req?.departmentID ?? 0;
                jobID = req?.jobID ?? 0;
            }
            var vacancy = new VacancyModel()
            {
                ClosingDate = VacancyModel.ClosingDate,
                jobID =jobID !=0 ? VacancyModel.jobID : jobID,
                jobRequirementID =VacancyModel.jobRequirementID > 0? VacancyModel.jobRequirementID : null,
                VacancyTitle = VacancyModel.VacancyTitle,
                departmentID = depID != 0? depID : VacancyModel.departmentID,
                employmentTypeID = VacancyModel.employmentTypeID,
                employmentMethodID = VacancyModel.employmentMethodID,
                Location = VacancyModel.Location,
                VacancyRequiredNumber = VacancyModel.VacancyRequiredNumber,
                DatePosted =VacancyModel.DatePosted,
                VacancyType =VacancyModel.VacancyType,
                Status = VacancyStatus.OnHold,
                Remark =VacancyModel.Remark,
                modifiedBy =User.Identity.Name,
                modifiedDate = DateTime.Now
            };
            ModelState.Remove("VacancyModel.modifiedBy");
            ModelState.Remove("VacancyModel.departmentID");
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
                        TempData["message"]= ("Error", $"{kv.Key} --> {error.ErrorMessage}");
                    }
                    Console.WriteLine(kv.ToString());
                }
                var jobReq = _context.JobRequirements.Include(j => j.JobModel)
               .Include(j => j.DepartmentModel).FirstOrDefault(j => j.jobRequirementID == VacancyModel.jobRequirementID);
                JobReq = jobReq ?? new jobRequirementModel();

                JobList = _context.Jobs.Where(j => j.jobStatus == mainStatus.Active).ToList();
                CompanyList = new SelectList(_context.Companies.Where(d => d.companyStatus == mainStatus.Active).OrderBy(c => c.companyName).ToList(), "companyID", "companyName");
                EmploymentType = new SelectList(_context.EmploymentTypes.Where(d => d.employmentTypeStatus == mainStatus.Active).ToList(), "employmentTypeID", "employmentTypeName");
                JobRequests = new SelectList(_context.JobRequirements.Where(j => j.jobRequirementStatus == jobReqStatus.Approved).ToList(), "jobRequirementID", "jobRequirementID");
                EmploymentMethods = new SelectList(_context.EmploymentMethods.Where(j => j.employmentMethodStatus == mainStatus.Active).ToList(), "employmentMethodID", "employmentMethodName");

                VacancyModel = vacancy;
                
                
                return Page();
            }
            _context.Vacancies.Add(vacancy);
            await _context.SaveChangesAsync();
            
            return RedirectToPage("Details", new {id = vacancy.VacancyID});
        }
    }

}
