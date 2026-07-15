using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;
using PIS2.Services;
using System.Diagnostics.Metrics;


namespace PIS2.Pages.Experience
{
    
    public class _OfficialPrintModel : PageModel
    {
        private readonly PISContext _context;
        private readonly Core _core;

        public _OfficialPrintModel(PISContext context, Core core)
        {
            _context = context;
            _core = core;
        }
        public personModel personName { get; set; }
        public string successMessage { get; set; }
        public string errorMessage { get; set; }
        public decimal Salary { get; set; } = 0;
        public List<experienceModel> Experiences { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            Experiences = new List<experienceModel>();
            var person = await _context.Persons.FirstOrDefaultAsync(p => p.personID == id);
            var employment = await _context.Employments.Where(p => p.personID == id).ToListAsync();

            if(person == null)
            {
                TempData["message"] = ("Error", "Person not found!");
            }
            var experiences = await _context.Experiences.OrderBy(e => e.experienceStartDate).Where(e => e.experienceType == Ex_In.Internal && e.personID ==id).ToListAsync();

            if(!experiences.Any())
            {
                TempData["message"] = ("Error", $"No experience records found for {person?.personFullName}!");
            }
            else
            {
                Experiences = experiences;
                personName = person;
            }
            if(employment.Any(e => e.employmentStatus == mainStatus.Active))
            {
                var activeJobPlacement = await _context.JobPlacements.Include(j => j.jobModel).Where(j => j.jobPlacementStatus == mainStatus.Active && employment.Select(e => e.employmentID).ToList().Contains(j.employmentID)).FirstOrDefaultAsync();

                if(activeJobPlacement != null && activeJobPlacement.jobPlacementID > 0)
                {
                    Experiences.Add(new experienceModel
                    {
                        experienceStartDate = activeJobPlacement.jobPlacementDate,
                        experienceEndDate =DateTime.Now,
                        jobTitle = activeJobPlacement.jobModel?.jobTitle
                    });

                }
                
            }
            // SALARY (latest)
            var salary = await _context.JobPlacements
                .Include(j => j.employmentModel)
                .Where(j => j.employmentModel.personID == id)
                .OrderByDescending(j => j.jobPlacementDate)
                .FirstOrDefaultAsync();

            Salary = salary?.jobPlacementSalary ?? 0;

            return Page();
        }

        [BindProperty]
        public string LBody { get; set; }
        public async Task<IActionResult> OnPostAsync()
        {
            letterModel l = new letterModel();
            l.letterTitle = "To Whom It May Concern";
            l.letterSubject = "Work Experience Information";
            l.modifiedBy = User.Identity.Name;
            l.modifiedDate = DateTime.Now;
            l.letterBody = LBody;
            var letterType = _context.LetterTypes.FirstOrDefault(l => l.letterTypeName.Contains("Employee"));
            if (letterType == null) {
                errorMessage="'Employee' Letter type not configured";
                return Page();
            }
            l.letterTypeID = letterType.letterTypeID;
            l.letterGroup = LetterGroup.Internal;
            l.letterStatus = LetterStatus.Approved;
            l.letterSender = "";
            l.letterReceiver = "";
            _context.Letters.Add(l);
            await _context.SaveChangesAsync();


            return RedirectToPage("/Letter/Details", new { id = l.letterID });
        }
    }
}
