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
        public personModel Person { get; set; }
        public IActionResult OnGet(int? id)
        {
            if(id >0)
            {
                Person = _context.Persons.FirstOrDefault(p => p.personID == id);
                ViewData["personID"] = new SelectList(_context.Persons
                    .Where(p => !_context.Employments
                        .Any(e => e.personID == p.personID && e.employmentStatus == mainStatus.Active))
                    .OrderBy(p => p.personFirstName).ToList(), "personID", "personFullName", id);
            }
            else
            {
                ViewData["personID"] = new SelectList(_context.Persons
                    .Where(p => !_context.Employments
                        .Any(e => e.personID == p.personID && e.employmentStatus == mainStatus.Active))
                    .OrderBy(p => p.personFirstName).ToList(), "personID", "personFullName");
            }
            
            ViewData["employmentTypeID"] = new SelectList(_context.EmploymentTypes, "employmentTypeID", "employmentTypeName");
            ViewData["employmentMethod"] = new SelectList(_context.EmploymentMethods, "employmentMethodID", "employmentMethodName");
            ViewData["employmentRequest"] = new SelectList(_context.EmploymentRequests.Where(er => er.requestStatus != employmentRequestStatus.Completed), "employmentRequestID", "employmentRequestID");
            return Page();
        }

        [BindProperty]
        public employmentModel employmentModel { get; set; } = default!;
       
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            ViewData["personID"] = new SelectList(_context.Persons, "personID", "personFullName");
            ViewData["employmentTypeID"] = new SelectList(_context.EmploymentTypes, "employmentTypeID", "employmentTypeName");
            ModelState.Remove("employmentModel.modifiedBy");
            employmentModel.modifiedBy = User.Identity.Name;

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

            return RedirectToPage("/JobPlacement/Create", new {id= employmentModel.employmentID});
        }
    }
}
