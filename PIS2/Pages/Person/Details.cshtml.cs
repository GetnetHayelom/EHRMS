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
    [Authorize(Roles = "MIE\\PMS_HRMANAGER,MIE\\PMS_HRCLERK")]
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
        
        public bool isPersonActiveEmployee { get; set; } = false;
        public bool PhotoExists { get; set; }
        public List<employmentModel>? personEmployments { get; set; } = default!;
        public List<experienceModel>? personExperiences { get; set; } = default!;
        public List<personEducationLevelModel>? personCertifications { get; set; } = default!;
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            
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
                personModel = personmodel;
                personEmployments = _context.Employments.Include(e => e.employmentTypeModel).Where(e => e.personID == id).ToList();
                personExperiences = _context.Experiences.Where(e => e.personID == id).ToList() ?? new List<experienceModel>();
                personCertifications = _context.PersonEducationLevels.Include(c => c.educationLevelModel).Where(e => e.personID == id).ToList() ?? new List<personEducationLevelModel>();
                //isPersonActiveEmployee = _context.Employments.FirstAsync(e => e.personID == id && e.employmentStatus == mainStatus.Active) != null ? true : false;

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
