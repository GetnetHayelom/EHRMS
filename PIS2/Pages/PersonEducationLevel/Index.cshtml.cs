using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using PIS2.Models;
using PIS2.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIS2.Pages.PersonEducationLevel
{
    public class IndexModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public IndexModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public IList<CertificationDetailsView> personEducationLevelModel { get;set; } = default!;
        public IList<CertificationSummaryView> CertificationSummary { get;set; }
        

        public List<companyModel> Companies { get; set; }
        public List<departmentModel> Departments { get; set; }
        public List<jobClassModel> JobClasses { get; set; }
        public List<jobModel> Jobs { get; set; }
        public List<string> EducationFields { get; set; }
        public DateTime SDate { get; set; }
        public DateTime EDate { get; set; }

        public int Male {get; set; }
        public int Female { get; set; }
        public int ActiveEmp { get; set; }
        public int TotalRecords { get; set; }
        public List<string> EducationDescipline { get; set; }
        public async Task OnGetAsync()
        {
            Companies = _context.Companies.OrderBy(c => c.companyName).ToList();
            Departments = _context.Departments.ToList();
            JobClasses = _context.JobClasses.ToList();
            Jobs = _context.Jobs.OrderBy(j => j.jobTitle).ToList();
            EducationFields = _context.PersonEducationLevels.Select(e => e.educationField).Distinct().ToList();
            EducationDescipline = _context.PersonEducationLevels.Select(e => e.educationDiscipline.ToString()).Distinct().ToList();

            personEducationLevelModel =new List<CertificationDetailsView>();
            personEducationLevelModel = await _context.CertificationDetailsView.ToListAsync();

            SDate = personEducationLevelModel.Min(p => (DateTime?)p.EducationLevelDate) ?? DateTime.MinValue;
            EDate = personEducationLevelModel.Max(p => (DateTime?)p.EducationLevelDate) ?? DateTime.MinValue;

            CertificationSummary = _context.CertificationSummaryView.OrderByDescending(c => c.Total).ToList();
      
        }
        public IActionResult OnGetTalentExperienceFilter(int? jobClassID, string? jobTitle, int? empStatus, int? duration, int? gender)
        {
            var tableHtml = new List<object>
                {
                    new { FullName = "<tr class='table-danger text-center fw-bold'>" +
                                        "<td colspan='10'>No records found</td>" +
                                    "</tr>"
                    }
                };

            if (_context?.TalentExperienceView == null)
            {
                Male = 0;
                Female = 0;
                ActiveEmp = 0;
                TotalRecords = 0;
                return new JsonResult(new { tableHtml, Male, Female, ActiveEmp, TotalRecords });
            }

            var query = _context?.TalentExperienceView.AsQueryable();


            // Apply filters safely
            if (jobClassID.HasValue)
                query = query.Where(e => e.jobClassID == jobClassID);

            if (!string.IsNullOrEmpty(jobTitle))
                query = query.Where(e => EF.Functions.Like(e.jobTitle, $"%{jobTitle}%"));

            if (empStatus.HasValue && Enum.IsDefined(typeof(mainStatus), empStatus.Value))
                query = query.Where(e => e.EmploymentStatus == (mainStatus)empStatus);

            if (duration.HasValue)
                query = query.Where(e => e.Duration >= duration);

            if (gender.HasValue && Enum.IsDefined(typeof(Gender), gender.Value))
                query = query.Where(e => e.PersonGender == (Gender)gender);

            // Execute query safely
            var filteredTalent = query.ToList() ?? new List<TalentExperienceView>();


            Male = filteredTalent.Count(t => t.PersonGender == Gender.Male);
            Female = filteredTalent.Count(t => t.PersonGender == Gender.Female);
            ActiveEmp = filteredTalent.Count(t => t.EmploymentStatus == mainStatus.Active);
            TotalRecords = filteredTalent.Count();
            
            if (filteredTalent.Count() > 0)
            {
                tableHtml = filteredTalent.OrderByDescending(e => e.Duration)
                .Select(e => new
                {
                    FullName = e.FullName,
                    JobTitle = e.jobTitle,
                    EmploymentStatus = e.EmploymentStatus.ToString(),
                    Gender = e.PersonGender.ToString(),
                    Duration = e.Duration,
                    DurationText = $"{e.Duration} Mos ({e.Duration / 12}.{e.Duration % 12} Yrs)",
                    DetailsUrl = Url.Page("/Person/Details", new { id = e.personID })
                }).ToList<object>();
            }
            

                return new JsonResult(new { tableHtml, Male, Female, ActiveEmp, TotalRecords });
        }
 
        public JsonResult OnGetEducationReport(int? CompanyID, int? DepartmentID, int? Category, string? Discipline, string? Field, int? EmploymentStatus, DateTime? Start, DateTime? End)
        {
            var talentpool = _context.CertificationDetailsView.AsQueryable();

            if (CompanyID.HasValue)
            {
                talentpool = talentpool.Where(e => e.CompanyID == CompanyID);
            }
            //
            if (Category.HasValue)
            {
                talentpool = talentpool.Where(e => e.EducationLevelCategory == (educationCategory)Category);
            }
            //
            if (!string.IsNullOrEmpty(Discipline))
            {
                talentpool = talentpool.Where(e => EF.Functions.Like(e.EducationDiscipline, $"%{Discipline}%"));
            }
            //
            if (!string.IsNullOrEmpty(Field))
            {
                talentpool = talentpool.Where(e => EF.Functions.Like(e.EducationField, $"%{Field}%"));
            }
            //
            if (EmploymentStatus.HasValue && Enum.IsDefined(typeof(mainStatus), EmploymentStatus.Value))
            {
                talentpool = talentpool.Where(e => e.EmploymentStatus == (mainStatus)EmploymentStatus);

            }
            if (Start.HasValue)
            {
                talentpool = talentpool.Where(e => e.EducationLevelDate >= Start);
            }
            if (End.HasValue)
            {
                talentpool = talentpool.Where(e => e.EducationLevelDate <= End);
            }

            var filteredTalent = talentpool.Count() > 0? talentpool.ToList() : new List<CertificationDetailsView>();
            Male = filteredTalent.Count(t => t.PersonGender == Gender.Male);
            Female = filteredTalent.Count(t => t.PersonGender == Gender.Female);
            ActiveEmp = filteredTalent.Count(t => t.EmploymentStatus == mainStatus.Active);
            TotalRecords = filteredTalent.Count();
            var data = filteredTalent
                .OrderBy(c => c.CompanyName)
                .GroupBy(c => c.CompanyID)
                .Select(company => new
                {
                    CompanyName = company.FirstOrDefault().CompanyName,
                    CompanyID = company.Key,
                    DistinctPersons = company.Select(c => c.PersonID).Distinct().Count(),
                    TotalRecords = company.Count(),

                    Departments = company
                        .GroupBy(c => c.DepartmentID)
                        .Select(dept => new
                        {
                            DepartmentName = dept.FirstOrDefault().DepartmentName,
                            DepartmentID = dept.Key,
                            DistinctPersons = dept.Select(c => c.PersonID).Distinct().Count(),
                            TotalRecords = dept.Count(),

                            EducationLevels = dept
                                .GroupBy(c => c.EducationLevelCategory)
                                .Select(level => new
                                {
                                    LevelCategory = level.FirstOrDefault().EducationLevelCategory.ToString(),
                                    DistinctPersons = level.Select(c => c.PersonID).Distinct().Count(),
                                    TotalRecords = level.Count(),

                                    Genders = level
                                        .GroupBy(c => c.PersonGender)
                                        .Select(gender => new
                                        {
                                            Gender = gender.Key.ToString(),
                                            DistinctPersons = gender.Select(c => c.PersonID).Distinct().Count(),
                                            TotalRecords = gender.Count(),
                                            Individuals = gender
                                                .OrderBy(e => e.GivenID)
                                                .Select(d => new
                                                {
                                                    d.GivenID,
                                                    d.FullName,
                                                    d.EducationField,
                                                    Date = d.EducationLevelDate?.ToString("MM/dd/yyyy"),
                                                    d.EducationLevelMark
                                                })
                                        })
                                })
                        })
                })
                .ToList();

            return new JsonResult(new { data, Male, Female, ActiveEmp, TotalRecords });
        }


    }
}
