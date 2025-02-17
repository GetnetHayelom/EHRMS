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

        public IActionResult OnGet()
        {
        ViewData["educationLevelID"] = new SelectList(_context.EducationLevels, "educationLevelID", "educationLevelName");
        ViewData["personID"] = new SelectList(_context.Persons, "personID", "personFullName");
            return Page();
        }

        [BindProperty]
        public personEducationLevelModel personEducationLevelModel { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.PersonEducationLevels.Add(personEducationLevelModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
