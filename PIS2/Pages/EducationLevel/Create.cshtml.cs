using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.EducationLevel
{
    [Authorize(Roles = "MIE\\PMS_HRADMIN")]
    public class CreateModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public CreateModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            if (!User.IsInRole("MIE\\PMS_HRADMIN"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }

            return Page();
        }

        [BindProperty]
        public educationLevelModel educationLevelModel { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.IsInRole("MIE\\PMS_HRADMIN"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }
            ModelState.Remove("educationLevelModel.modifiedBy");
            educationLevelModel.modifiedBy = User.Identity.Name;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.EducationLevels.Add(educationLevelModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
