using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.Exprience
{
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

        public DetailsModel(PISContext context)
        {
            _context = context;
        }

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
    }
}
