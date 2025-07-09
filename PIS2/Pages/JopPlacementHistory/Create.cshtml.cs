using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PIS2.Models;

namespace PIS2.Pages.JopPlacementHistory
{
    public class CreateModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public CreateModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }
        [BindProperty]
        public DateTime EndDate { get; set; }
        [BindProperty]
        public string JobGrade { get; set; }
        [BindProperty]
        public string JobStep { get; set; }
        public List<experienceModel> Experience { get; set; }
        public IActionResult OnGet()
        {
        ViewData["departmentID"] = new SelectList(_context.Departments, "departmentID", "departmentName");
        ViewData["employmentID"] = new SelectList(_context.Employments, "employmentID", "givenID");
        ViewData["jobID"] = new SelectList(_context.Jobs, "jobID", "jobTitle");
       
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
