using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PIS2.Models;

namespace PIS2.Pages.EmploymentHistory
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
        ViewData["employmentID"] = new SelectList(_context.Employments, "employmentID", "employmentModel.personModel.personFullName");
        ViewData["employmentTypeID"] = new SelectList(_context.EmploymentTypes, "employmentTypeID", "employmentTypeName");
            return Page();
        }

        [BindProperty]
        public employmentHistoryModel employmentHistoryModel { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            employmentHistoryModel.validateAge();
            _context.EmploymentHistories.Add(employmentHistoryModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
