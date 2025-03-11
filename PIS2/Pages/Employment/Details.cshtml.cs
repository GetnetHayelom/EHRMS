using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Employment
{
    public class DetailsModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DetailsModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public employmentModel employmentModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employmentmodel = await _context.Employments.Include(e=>e.EmploymentHistories)
                .Include(e=>e.personModel)
                .Include(e=>e.employmentTypeModel).FirstOrDefaultAsync(m => m.employmentID == id);
            if (employmentmodel == null)
            {
                return NotFound();
            }
            else
            {
                employmentModel = employmentmodel;
            }
            return Page();
        }
    }
}
