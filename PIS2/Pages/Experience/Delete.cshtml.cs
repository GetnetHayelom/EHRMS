using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Exprience
{
    [Authorize(Roles = "HRMANAGER,HRCLERK")]
    public class DeleteModel : PageModel
    {
        private readonly PISContext _context;

        public DeleteModel(PISContext context)
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

            var experiencemodel = await _context.Experiences.Include(ex => ex.personModel).FirstOrDefaultAsync(m => m.experienceID == id);

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
