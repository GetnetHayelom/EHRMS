using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.ShiftAssignment
{
    [Authorize(Roles = "MIE\\PMS_HRMANAGER,MIE\\PMS_HRCLERK,MIE\\PMS_MANAGEMENT")]
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
        ViewData["shiftID"] = new SelectList(_context.Shifts, "shiftID", "shiftName");
            return Page();
        }

        [BindProperty]
        public shiftAssignmentModel shiftAssignmentModel { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.ShiftAssignments.Add(shiftAssignmentModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
