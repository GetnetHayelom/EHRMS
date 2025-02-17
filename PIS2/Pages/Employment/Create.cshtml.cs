using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PIS2.Models;

namespace PIS2.Pages.Employment
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
            ViewData["personID"] = new SelectList(_context.Persons, "personID", "personFullName");
            ViewData["employmentTypeID"] = new SelectList(_context.EmploymentTypes, "employmentTypeID", "employmentTypeName");
            return Page();
        }

        [BindProperty]
        public employmentModel employmentModel { get; set; } = default!;
       
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            ViewData["personID"] = new SelectList(_context.Persons, "personID", "personFullName");
            ViewData["employmentTypeID"] = new SelectList(_context.EmploymentTypes, "employmentTypeID", "employmentTypeName");
            
            if (!ModelState.IsValid)
            {
                
                // Log or display errors for debugging
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"{error.Key}: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                }
                // Optionally pass errors to the view for display
                TempData["successMessage"] = ModelState.Values.SelectMany(v=>v.Errors).Select(e =>e.ErrorMessage).ToList();
                return Page();
            }

            _context.Employments.Add(employmentModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
