using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.PersonEducationLevel
{
    public class EditModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public EditModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public personEducationLevelModel personEducationLevelModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personeducationlevelmodel =  await _context.PersonEducationLevels.FirstOrDefaultAsync(m => m.personEducationLevelID == id);
            if (personeducationlevelmodel == null)
            {
                return NotFound();
            }
            personEducationLevelModel = personeducationlevelmodel;
           ViewData["educationLevelID"] = new SelectList(_context.EducationLevels, "educationLevelID", "educationLevelName");
           ViewData["personID"] = new SelectList(_context.Persons.OrderBy(e => e.personFirstName)
               .OrderBy(e => e.personLastName).OrderBy(e=> e.personLastName), "personID", "personFullName");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            personEducationLevelModel.modifiedBy = User.Identity.Name;
            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        Console.WriteLine($"{kv.Key} --> {error.ErrorMessage}");
                    }
                }
                return Page();
            }

            _context.Attach(personEducationLevelModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!personEducationLevelModelExists(personEducationLevelModel.personEducationLevelID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool personEducationLevelModelExists(int id)
        {
            return _context.PersonEducationLevels.Any(e => e.personEducationLevelID == id);
        }
    }
}
