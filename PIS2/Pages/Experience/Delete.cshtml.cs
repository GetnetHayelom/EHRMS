using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Exprience
{
    public class DeleteModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DeleteModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public experienceModel experienceModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var experiencemodel = await _context.Experiences.FirstOrDefaultAsync(m => m.experienceID == id);

            if (experiencemodel == null)
            {
                return NotFound();
            }
            else
            {
                experienceModel = experiencemodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            int empID=0;
            var experiencemodel = await _context.Experiences.FindAsync(id);
            if (experiencemodel != null)
            {
                experienceModel = experiencemodel;
                var emp = await _context.Employments.FirstOrDefaultAsync(e => e.personID == experienceModel.personID);
                empID = emp.employmentID;
                _context.Experiences.Remove(experienceModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/Employment/Edit", new { id = empID });
        }
    }
}
