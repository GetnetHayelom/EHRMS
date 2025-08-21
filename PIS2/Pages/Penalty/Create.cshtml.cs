using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PIS2.Models;

namespace PIS2.Pages.Penalty
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
        ViewData["employmentID"] = new SelectList(_context.Employments, "employmentID", "givenID");
        ViewData["penaltyTypeID"] = new SelectList(_context.PenaltyTypes, "penaltyTypeID", "penaltyTypeID");
            return Page();
        }

        [BindProperty]
        public penaltyModel penaltyModel { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Penalties.Add(penaltyModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
