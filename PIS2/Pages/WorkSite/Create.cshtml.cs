using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.WorkSite
{
    [Authorize(Roles = "MIE\\PMS_HRMANAGER")]
    public class CreateModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public CreateModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["addressID"] = new SelectList(_context.Addresses, "addressID", "addressFormatted");
            return Page();
        }

        [BindProperty]
        public workSiteModel workSiteModel { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.WorkSites.Add(workSiteModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
