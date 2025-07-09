using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PIS2.Models;

namespace PIS2.Pages.JobPlacement
{
    public class CreateModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public CreateModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int company { get; set; }
        public IActionResult OnGet()
        {
            ViewData["companyID"] = new SelectList(_context.Companies.Where(c => c.companyStatus == mainStatus.Active), "companyID", "companyName");
            ViewData["departmentID"] = new SelectList(_context.Departments.Where( d=> d.departmentStatus == mainStatus.Active), "departmentID", "departmentName");
            ViewData["employmentID"] = new SelectList(_context.Employments.Where(e => e.employmentStatus == mainStatus.Active), "employmentID", "givenID");
            ViewData["jobID"] = new SelectList(_context.Jobs.Where(j => j.jobStatus == mainStatus.Active), "jobID", "jobTitle");
            ViewData["shiftID"] = new SelectList(_context.Shifts.Where(s => s.shiftStatus == mainStatus.Active), "shiftID", "shiftName");
            return Page();
        }

        [BindProperty]
        public jobPlacementModel jobPlacementModel { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.JobPlacements.Add(jobPlacementModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
