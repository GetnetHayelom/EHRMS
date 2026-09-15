using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PIS2.Data;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Holiday
{
    [Authorize(Roles = "HRMANAGER,HRCLERK, HRADMIN")]
    public class CreateModel : PageModel
    {
        private readonly PISContext _context;

        public CreateModel(PISContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public holidayModel holidayModel { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!(User.IsInRole("HRADMIN") || User.IsInRole("HRMANAGER"))) { return RedirectToPage("/Shared/AccessDenied"); }
            ModelState.Clear();
            holidayModel.modifiedBy = User.Identity.Name;
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Holidays.Add(holidayModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
