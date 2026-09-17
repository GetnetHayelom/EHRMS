using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;
using PIS2.Models.Foundation;
using PIS2.Models.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Person
{
    
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;
        private readonly IWebHostEnvironment _environment;

        public DetailsModel(PISContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment= environment;
           
        }

        public personModel personModel { get; set; } = default!;
        public personModel personModel2 { get; set; } = default!;

        public bool isPersonActiveEmployee { get; set; } = false;
        public bool PhotoExists { get; set; }
        public List<employmentModel>? personEmployments { get; set; } = default!;
        public List<experienceModel>? personExperiences { get; set; } = default!;
        public List<personEducationLevelModel>? personCertifications { get; set; } = default!;
        public List<familyModel>? familyRelations { get; set; } = default!;
        public bool isSelf { get; set; }
        public jobPlacementModel currentJob { get; set; }
        public employmentModel currentEmployment { get; set; }
        public SelectList PersonList
        {
            get; set;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            PersonList = new SelectList(_context.Persons.OrderBy(p => p.personFirstName).ToList(), "personID", "personFullName", id);
            var username = await _context.Users.FirstOrDefaultAsync(p => p.personID == id);
            var un=username?.UserName;
            
            isSelf = un == User.Identity.Name ? true : false;
            
            if(!(User.IsInRole("HRPERSONNEL") || User.IsInRole("HRMANAGER") || User.IsInRole("MANAGEMENT") || isSelf))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }
            
            
            if (id == null)
            {
                return NotFound();
            }

            var personmodel = await _context.Persons
                .Include(p => p.PersonHistories).ThenInclude(ph => ph.addressModel)
                .Include(p => p.addressModel)
                .FirstOrDefaultAsync(m => m.personID == id);
            if (personmodel == null)
            {
                return NotFound();
            }
            else
            {
                var personID2 = _context.Families.FirstOrDefault(f => f.personID == id && f.relation == familyRelation.EmergencyContact)?.personID2;
                personModel2 = _context.Persons.FirstOrDefault(p => p.personID == personID2);
                personModel = personmodel;
                personEmployments = _context.Employments.Include(e => e.employmentTypeModel).Where(e => e.personID == id).ToList();
                personExperiences = _context.Experiences.Where(e => e.personID == id).ToList() ?? new List<experienceModel>();
                personCertifications = _context.PersonEducationLevels.Include(c => c.educationLevelModel).Where(e => e.personID == id).ToList() ?? new List<personEducationLevelModel>();
                
                familyRelations = _context.Families.Include(f=> f.personModel)
                    .Include(f => f.personModel2).Where(f => f.personID == id || f.personID2 == id).ToList();

                isPersonActiveEmployee = personEmployments?.Any(e => e.employmentStatus == mainStatus.Active) ?? false;

                currentEmployment = isPersonActiveEmployee ? personEmployments.Find(e => e.employmentStatus == mainStatus.Active) : new employmentModel();
                currentJob = isPersonActiveEmployee ? _context.JobPlacements.FirstOrDefault(e => e.employmentID == currentEmployment.employmentID && e.jobPlacementStatus == mainStatus.Active) : new jobPlacementModel();
                //Check if photo is available
                var imagesFolder = Path.Combine(_environment.WebRootPath, "images");
                var fileName = $"{personModel.personID}.jpg";
                var filePath = Path.Combine(imagesFolder, fileName);

                PhotoExists = System.IO.File.Exists(filePath);
            }
            return Page();
        }

        [BindProperty]
        public int personID { get; set; }
        public async Task<IActionResult> OnGetPersonAsync() {

            return RedirectToPage("/Person/Details", new { id = personID });
        }
    }
}
