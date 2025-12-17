using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Person
{
    
    public class DetailsModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;
        private readonly IWebHostEnvironment _environment;

        public DetailsModel(PIS2.Models.PISContext context, IWebHostEnvironment environment)
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


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if(!(User.IsInRole("MIE\\PMS_HRCLERK") || User.IsInRole("MIE\\PMS_HRMANAGER") || User.IsInRole("MIE\\PMS_MANAGEMENT") || isSelf))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }
            var username = _context.Users.FirstOrDefault(p => p.personID == id)?.userName;
            
            isSelf = username == User.Identity.Name ? true : false;
            
            
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

                isPersonActiveEmployee = personEmployments.Any(e => e.employmentStatus == mainStatus.Active);
                
                //Check if photo is available
                var imagesFolder = Path.Combine(_environment.WebRootPath, "images");
                var fileName = $"{personModel.personID}.jpg";
                var filePath = Path.Combine(imagesFolder, fileName);

                PhotoExists = System.IO.File.Exists(filePath);
            }
            return Page();
        }
    }
}
