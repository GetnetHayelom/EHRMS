using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Termination
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
            return Page();
        }

        [BindProperty]
        public terminationModel terminationModel { get; set; } = default!;
        public employmentModel employmentModel { get; set; }

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            employmentModel = await _context.Employments.FirstOrDefaultAsync(e => e.employmentID == id);
            if (employmentModel == null)
            {
                return NotFound();
            }
            if (terminationModel.terminationStatus == terminationModel.terminationStatus)
            {

            }
            _context.Terminations.Add(terminationModel);
            await _context.SaveChangesAsync();

            
            return RedirectToPage("./Index");
        }
    }
}
