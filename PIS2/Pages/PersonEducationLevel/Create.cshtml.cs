using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PIS2.Models;

namespace PIS2.Pages.PersonEducationLevel
{
    public class CreateModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public CreateModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public List<personEducationLevelModel> CertificationList { get; set; }
        public IActionResult OnGet(int? id)
        { 
            if (id != null)
            {
                ViewData["personID"] = new SelectList(_context.Persons, "personID", "personFullName", id);
            }
            else
            {
                ViewData["personID"] = new SelectList(_context.Persons.OrderBy(p => p.personFirstName).ThenBy(p => p.personFatherName).ThenBy(p => p.personLastName), "personID", "personFullName");
            }
            ViewData["educationLevelID"] = new SelectList(_context.EducationLevels, "educationLevelID", "educationLevelName");
            var certifications = _context.PersonEducationLevels.Where(pel => pel.personID == id).ToList();
            if (certifications.Any())
            {
                CertificationList = certifications;
            }
            else { CertificationList = new List<personEducationLevelModel>(); }
            return Page();
        }

        [BindProperty]
        public personEducationLevelModel personEducationLevelModel { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("personEducationLevelModel.modifiedBy");
            personEducationLevelModel.modifiedBy = User.Identity?.Name ?? "N\\A";

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
                return Page();
            }

            _context.PersonEducationLevels.Add(personEducationLevelModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
