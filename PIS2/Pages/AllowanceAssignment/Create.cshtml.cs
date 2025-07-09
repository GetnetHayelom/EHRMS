using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PIS2.Models;

namespace PIS2.Pages.AllowanceAssignment
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
        ViewData["allowanceID"] = new SelectList(_context.Allowances.Where(a => a.allowanceStatus == mainStatus.Active), "allowanceID", "allowanceName");
        ViewData["employmentID"] = new SelectList(_context.Employments.Where(a => a.employmentStatus == mainStatus.Active).OrderBy(e => e.givenID), "employmentID", "givenID");
            return Page();
        }

        [BindProperty]
        public allowanceAssignmentModel allowanceAssignmentModel { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("allowanceAssignmentModel.modifiedBy");

            allowanceAssignmentModel.modifiedBy = User.Identity.Name;
            allowanceAssignmentModel.allowanceStatus = mainStatus.Suspended;

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

            _context.AllowanceAssignments.Add(allowanceAssignmentModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
