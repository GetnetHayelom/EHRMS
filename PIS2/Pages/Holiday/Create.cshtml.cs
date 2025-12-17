using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Holiday
{
    [Authorize(Roles = "MIE\\PMS_HRMANAGER,MIE\\PMS_HRCLERK, MIE\\PMS_HRADMIN")]
    public class CreateModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public CreateModel(PIS2.Models.PISContext context)
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
            if (!(User.IsInRole("MIE\\PMS_HRADMIN") || User.IsInRole("MIE\\PMS_HRMANAGER"))) { return RedirectToPage("/Shared/AccessDenied"); }
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
