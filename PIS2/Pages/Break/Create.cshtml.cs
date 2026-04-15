using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.Break
{
    public class CreateModel : PageModel
    {
        private readonly PISContext _context;

        public CreateModel(PISContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            if (!User.IsInRole("MIE\\PMS_HRADMIN"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }
            ViewData["shiftID"] = new SelectList(_context.Shifts, "shiftID", "shiftName");
            return Page();
        }

        [BindProperty]
        public breakModel breakModel { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if(!User.IsInRole("MIE\\PMS_HRADMIN"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }
            ModelState.Clear();
            breakModel.modifiedBy = User.Identity.Name;
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Breaks.Add(breakModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
