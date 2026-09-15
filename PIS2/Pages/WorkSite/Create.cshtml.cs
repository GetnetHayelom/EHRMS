using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.WorkSite
{
    [Authorize(Roles = "HRADMIN")]
    public class CreateModel : PageModel
    {
        private readonly PISContext _context;

        public CreateModel(PISContext context)
        {
            _context = context;
        }
        public SelectList Manager;
        public async Task<IActionResult> OnGetAsync()
        {
            var managerData = await _context.Employments
                .Include(e => e.personModel)
                .Where(e => e.employmentStatus == mainStatus.Active)
                .Select(e => new
                {
                    EmpID = e.employmentID,
                    // Combine ID and Name for the dropdown display
                    FullName = e.givenID + " - " + e.personModel.personFullName
                })
                .ToListAsync();

            Manager = new SelectList(managerData, "EmpID", "FullName");

            ViewData["addressID"] = new SelectList(_context.Addresses, "addressID", "addressFormatted");
            return Page();
        }

        [BindProperty]
        public workSiteModel workSiteModel { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Clear();
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
