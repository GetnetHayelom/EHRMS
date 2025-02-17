using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PIS2.Models;

namespace PIS2.Pages.AllowanceAssignmentHistory
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
        ViewData["allowanceAssignmentID"] = new SelectList(_context.AllowanceAssignments, "allowanceAssignmentID", "allowanceAssignmentID");
            return Page();
        }

        [BindProperty]
        public allowanceAssignmentHistoryModel allowanceAssignmentHistoryModel { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.AllowanceAssignmentsHistories.Add(allowanceAssignmentHistoryModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
