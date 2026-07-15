using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using PIS2.Data;
using PIS2.Enums;
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
            
            VacancyModel = await _context.Vacancies
                .Include(v => v.departmentModel).ThenInclude(d => d.companyModel)
                .Include(v => v.jobModel)
                .FirstOrDefaultAsync(v => v.VacancyID == id) ?? new VacancyModel();

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

            ModelState.Remove("VacancyModel.modifiedBy");
            ModelState.Remove("VacancyModel.modifiedDate");
  
            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        Console.WriteLine($"{kv.Key} --> {error.ErrorMessage}");TempData["message"] = ("Error", $"{kv.Key} --> {error.ErrorMessage}");
                    }
                    Console.WriteLine(kv.ToString());
                }

                await OnGetAsync(VacancyModel.VacancyID);
                return Page();
            }
            var existing = await _context.Vacancies
    .FirstOrDefaultAsync(v => v.VacancyID == VacancyModel.VacancyID);

            if (existing == null)
                return NotFound();

            var previousStatus = existing.Status;

            letterTypeModel? letterType = null;

            // BUSINESS VALIDATION FIRST
            if (VacancyModel.Status == VacancyStatus.Open
                && previousStatus != VacancyStatus.Open)
            {
                letterType = await _context.LetterTypes
                    .FirstOrDefaultAsync(t => t.letterTypeName.Contains("Vacancy"));

                if (letterType == null)
                {
                    ModelState.AddModelError("",
                        "Vacancy cannot be opened because letter type is not configured.");
                    await OnGetAsync(VacancyModel.VacancyID);
                    return Page();
                }
            }

            //APPLY UPDATES AFTER VALIDATION
            existing.jobID = VacancyModel.jobID;
            existing.jobRequirementID = VacancyModel.jobRequirementID;
            existing.VacancyTitle = VacancyModel.VacancyTitle;
            existing.departmentID = VacancyModel.departmentID;
            existing.Location = VacancyModel.Location;
            existing.VacancyRequiredNumber = VacancyModel.VacancyRequiredNumber;
            existing.employmentTypeID = VacancyModel.employmentTypeID;
            existing.employmentMethodID = VacancyModel.employmentMethodID;
            existing.Status = VacancyModel.Status;
            existing.VacancyType = VacancyModel.VacancyType;
            existing.DatePosted = VacancyModel.DatePosted;
            existing.ClosingDate = VacancyModel.ClosingDate;
            existing.Remark = VacancyModel.Remark;
            existing.modifiedBy = User.Identity.Name;
            existing.modifiedDate = DateTime.Now;

            await using var transaction = await _context.Database.BeginTransactionAsync();

            if (VacancyModel.Status == VacancyStatus.Open
                && previousStatus != VacancyStatus.Open)
            {
                var empType = await _context.EmploymentTypes
                    .FirstOrDefaultAsync(e => e.employmentTypeID == VacancyModel.employmentTypeID);

                var jobTitle = await _context.Jobs
                    .FirstOrDefaultAsync(j => j.jobID == VacancyModel.jobID);

                var letter = new letterModel
                {
                    letterBody = $"<p><strong>Job Title:</strong> <u>{jobTitle.jobTitle}</u></p>" +
                    $"<p><strong>Job Description:</strong></p> {jobTitle.jobDescription}" +
                    $"<p><strong>Qualifications:</strong> {jobTitle.jobQualifications}</p>" +
                    $"<p><strong>Experience:</strong> {jobTitle.jobExperience} Years </p>" +
                    $"<p><strong>Required Number:</strong> {VacancyModel.VacancyRequiredNumber}</p>" +
                    $"<p><strong>Employment Type:</strong> {empType.employmentTypeName}</p>" +
                    $"<p><strong>Closing Date:</strong> <u>{VacancyModel.ClosingDate:dd/MM/yyyy}</u></p>" +
                    $"<p><strong>Remark:</strong> {VacancyModel.Remark}</p>",

                    letterTitle = $"{VacancyModel.VacancyType} Vacancy",
                    letterDate = VacancyModel.DatePosted.Date,
                    letterSender = "",
                    letterReceiver = "",
                    letterSignedBy = "",
                    letterStatus = LetterStatus.Draft,
                    letterTypeID = letterType.letterTypeID,
                    letterGroup = LetterGroup.Outgoing,
                    letterSubject = VacancyModel.VacancyTitle ?? "",
                    modifiedBy = User.Identity.Name,
                    modifiedDate = DateTime.Now,

                };

                _context.Letters.Add(letter);
            }

            //SAVE ONCE
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return RedirectToPage("Index");
        }

    }
}
